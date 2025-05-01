using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewsletterStudio.Core.Backoffice.PropertyEditors.TransactionalEmailPicker.FrontendModels;
using NewsletterStudio.Core.Public;
using NewsletterStudio.Core.Public.Models;
using NewsletterStudio.Core.Rendering;
using NewsletterStudio.Plugins.UmbracoForms.Utilities;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using Polly;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;

namespace NewsletterStudio.Plugins.UmbracoForms.Transactionals;

public class SendTransactionalWorkflowType : WorkflowType
{
    private readonly ILogger<SendTransactionalWorkflowType> _logger;
    private readonly IUmbracoContextFactory _umbracoContextFactory;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Holds the configured transactional email.
    /// </summary>
    [Setting(nameof(TransactionalEmail),View= "Ns.PropertyEditorUi.TransactionalEmailPicker", IsMandatory = true, DisplayOrder = 50)]
    public string? TransactionalEmail { get; set; }

    /// <summary>
    /// Email to which the translational email will be sent, would replace the "To"-property of the transactional email.
    /// </summary>
    [Setting(nameof(SendToEmail), View= "Forms.PropertyEditorUi.TextWithFieldPicker", IsMandatory = false, DisplayOrder = 100)]
    public string? SendToEmail { get; set; }

    /// <summary>
    /// WorkflowType that sends emails using Newsletter Studios Transactional Email-feature
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="newsletterStudioService"></param>
    /// <param name="umbracoContextFactory"></param>
    public SendTransactionalWorkflowType(
        ILogger<SendTransactionalWorkflowType> logger,
        IUmbracoContextFactory umbracoContextFactory,
        IServiceProvider serviceProvider
        )
    {
        _logger = logger;
        _umbracoContextFactory = umbracoContextFactory;
        _serviceProvider = serviceProvider;

        this.Id = new Guid("C873DF4D-2D5A-402E-B534-C15C09CC323F");
        this.Name = "Send Transactional";
        this.Description = "Sends transactional e-mail";
        this.Icon = "icon-paper-plane";
        this.Group = "Email";
    }

    public override async Task<WorkflowExecutionStatus> ExecuteAsync(WorkflowExecutionContext context)
    {
        var configuredTransactionalEmails = GetConfiguredTransactionals();

        if (!configuredTransactionalEmails.Any())
        {
            _logger.LogError("Newsletter Studio | Umbraco Forms | Form Submission: Could not trigger workflow since no valid Transactional Email was configured.");
            return WorkflowExecutionStatus.Completed;
        }

        var helper = new UmbracoFormsHelper();
        var allFieldsAndValues = helper.ExtractFormValues(context.Record);

        var req = new SendTransactionalEmailRequest();
        this.PopulateMergeFields(req, context, allFieldsAndValues);

        if (!string.IsNullOrEmpty(SendToEmail))
        {
            req.SendTo(GetValueFromField(SendToEmail, allFieldsAndValues));
        }

        req.TransactionalEmailKey = configuredTransactionalEmails.First().TransactionalEmailKey;
        req.NotInBackground();

        //TODO: Update this code and inject the INewsletterStudioService into workflow when the services have been updated to handle these situations.
        using var serviceScope = _serviceProvider.CreateScope();
        var newsletterStudioService = serviceScope.ServiceProvider.GetRequiredService<INewsletterStudioService>();
        
        var result = await newsletterStudioService.SendTransactionalAsync(req);

        if (result.Failed)
        {
            _logger.LogWarning("Error sending Transactional Email in Forms Workflow {error}", result.Message);
            return WorkflowExecutionStatus.Failed;
        }

        return WorkflowExecutionStatus.Completed;
    }

    public override List<Exception> ValidateSettings()
    {
        var transactionalTemplates = this.GetConfiguredTransactionals();

        var exceptions = new List<Exception>();

        if (transactionalTemplates.Count < 1)
        {
            exceptions.Add(new Exception("Must choose a Transactional email template."));
        }

        return exceptions;
    }

    private void PopulateMergeFields(SendTransactionalEmailRequest req, WorkflowExecutionContext context,List<FormFieldValue> allFieldsAndValues)
    {
        req.MergeFields.AddRange(GetFormFieldsAndValues(allFieldsAndValues));

        using var cxtRef = _umbracoContextFactory.EnsureUmbracoContext();
        var postedFromPage = cxtRef.UmbracoContext.Content!.GetById(context.Record.UmbracoPageId);
        if (postedFromPage != null)
        {
            var url = postedFromPage.Url(mode: UrlMode.Absolute);
            req.MergeFields.Add(Constants.GenericMergeFields.PostedFromUrlMergeField.Placeholder, url);
        }
        req.MergeFields.Add(Constants.GenericMergeFields.FormNameMergeField.Placeholder, context.Form.Name);

        PopulateAllFormFieldsMergeField(req, allFieldsAndValues);
    }

    private string GetValueFromField(string fieldValue, List<FormFieldValue> formFields)
    {
        bool isFormField = fieldValue.StartsWith("{") && fieldValue.EndsWith("}");

        if (isFormField)
        {
            var propertyAlias = fieldValue.Substring(1).Substring(0, fieldValue.Length - 2);

            var mergeFieldForPropertyAlias = formFields.FirstOrDefault(x => x.Alias == propertyAlias);

            if (mergeFieldForPropertyAlias != null)
                return ReplaceLineBreaks(mergeFieldForPropertyAlias.Value);

        }

        return fieldValue;

    }


    private List<MergeFieldValue> GetFormFieldsAndValues(List<FormFieldValue> allFieldsAndValues)
    {

        var list = new List<MergeFieldValue>();

        foreach (var fieldAndValue in allFieldsAndValues)
        {
            list.Add(new MergeFieldValue(fieldAndValue.Alias, fieldAndValue.Value));
        }

        return list;

    }

    private void PopulateAllFormFieldsMergeField(SendTransactionalEmailRequest req, List<FormFieldValue> formFields)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var formField in formFields)
        {
            sb.Append("<p>");
            sb.Append($"<strong>{formField.Caption}");

            if (!formField.Caption.EndsWith("?") && !formField.Caption.EndsWith(":"))
            {
                sb.Append(":");
            }
            sb.Append($"</strong>");

            // Always new line before the value
            sb.Append("<br/>");

            sb.Append(ReplaceLineBreaks(formField.Value));

            sb.Append("</p>");

        }

        var html = sb.ToString();

        // Add to merge fields.
        req.MergeFields.Add(Constants.GenericMergeFields.AllFormFieldsUrlMergeField.Placeholder, html);

    }

    private string ReplaceLineBreaks(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return "";

        return value.Replace(Environment.NewLine, "<br/>");
    }

    private List<TransactionalEmailPropertyEditorValueModel> GetConfiguredTransactionals()
    {
        
        if (string.IsNullOrEmpty(this.TransactionalEmail))
            return new List<TransactionalEmailPropertyEditorValueModel>();

        try
        {
            List<TransactionalEmailPropertyEditorValueModel>? configuredTransactionalEmails = JsonConvert.DeserializeObject<List<TransactionalEmailPropertyEditorValueModel>>(this.TransactionalEmail);
            return configuredTransactionalEmails ?? new List<TransactionalEmailPropertyEditorValueModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Newsletter Studio | Umbraco Forms | Error when parsing stored Transactional Email Picker-value");
            return new List<TransactionalEmailPropertyEditorValueModel>();
        }

    }
}
