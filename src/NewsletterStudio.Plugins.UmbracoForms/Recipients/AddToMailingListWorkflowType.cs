using Microsoft.Extensions.Logging;
using NewsletterStudio.Core.Backoffice.PropertyEditors.MailingListPicker.FrontendModels;
using NewsletterStudio.Core.Models;
using NewsletterStudio.Core.Public;
using NewsletterStudio.Core.Public.Models;
using NewsletterStudio.Plugins.UmbracoForms.Utilities;
using Newtonsoft.Json;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;

namespace NewsletterStudio.Plugins.UmbracoForms.Recipients;

public class AddToMailingListWorkflowType : WorkflowType
{
    private readonly ILogger<AddToMailingListWorkflowType> _logger;
    private readonly INewsletterStudioService _newsletterStudioService;

    
    [Setting(nameof(AddToMailingListConfiguration),View = "Ns.Plugin.UmbracoForms.AddToMailingListPropertyEditorUi")]
    public string? AddToMailingListConfiguration { get; set; }

    public AddToMailingListWorkflowType(
        ILogger<AddToMailingListWorkflowType> logger,
        INewsletterStudioService newsletterStudioService
        )
    {
        _logger = logger;
        _newsletterStudioService = newsletterStudioService;
        this.Id = new Guid("575ADFDB-7C9F-4935-82D8-8A5B37225DAE");
        this.Name = "Add to Mailing List";
        this.Description = "Adds a new recipient to a mailing list";
        this.Icon = "icon-paper-plane"; //TODO Change this
        this.Group = "Newsletter Studio";
    }

    public override async Task<WorkflowExecutionStatus> ExecuteAsync(WorkflowExecutionContext context)
    {
        
        var formValues = new UmbracoFormsHelper().ExtractFormCollection(context.Record);

        //TODO: Need to be able to configure which fields to use.
        var nameField = formValues.GetByAlias("name");
        var emailField = formValues.GetByAlias("email");

        if (emailField == null || string.IsNullOrEmpty(emailField.Value))
        {
            _logger.LogWarning($"Newsletter Studio | Umbraco Forms | {nameof(AddToMailingListWorkflowType)} skipped due to invalid e-mail");
            return WorkflowExecutionStatus.Failed;
        }

        var valueEmail = emailField.Value;
        var valueName = nameField?.Value ?? "";

        var configuration = GetConfiguration();

        if (configuration?.MailingList is null)
            return WorkflowExecutionStatus.Failed;


        var addRecipientRequest = new AddRecipientRequest(valueEmail)
            .ForWorkspace(configuration.MailingList.WorkspaceKey)
            .SubscribeTo(configuration.MailingList.MailingListKey)
            .WithName(valueName) //TODO: Use based on form
            .WithSource($"Umbraco Form: " + context.Form.Name); //Max 255 characters should be fine


        //TODO: Create some kind of helper that extracts and maps values based on the mappings.
        //      Static values also need to be supported.

        if (formValues.TryGetByAlias("fieldBasedOnMapping", out nameField))
        {
            addRecipientRequest.WithName(nameField.Value);
        }

        //TODO: Source, use mapping OR fallback. 

        //TODO: Fields
        

        //TODO: Handle result, log errors.
        var result = _newsletterStudioService.AddRecipient(addRecipientRequest);

        
        return WorkflowExecutionStatus.Completed;
    }

    public override List<Exception> ValidateSettings()
    {
        var exceptions = new List<Exception>();
        var configuration = this.GetConfiguration();

        if(configuration?.MailingList is null)
            exceptions.Add(new Exception("Please choose a mailing list."));

        return exceptions;
    }

    private AddToMailingListConfigurationModel? GetConfiguration()
    {
        if (string.IsNullOrEmpty(this.AddToMailingListConfiguration))
            return null;

        try
        {
            return JsonConvert.DeserializeObject<AddToMailingListConfigurationModel>(this.AddToMailingListConfiguration);
        }
        catch
        {
        }

        return null;
    }

}

internal static class WellKnownFieldsAliases
{
    public const string Email = "email";
    public const string Name = "name";
    public const string FirstName = "firstName";
    public const string LastName = "lastName";
    public const string Source = "source";

}

public class AddToMailingListConfigurationModel
{
    public MailingListPropertyEditorValueModel? MailingList { get; set; }
    public List<NewsletterStudioFieldMapping>? Mappings { get; set; }
}

public class NewsletterStudioFieldMapping
{
    public string FieldAlias { get; set; }
    public string ValueAlias { get; set; }
    public string StaticValue { get; set; }
}
