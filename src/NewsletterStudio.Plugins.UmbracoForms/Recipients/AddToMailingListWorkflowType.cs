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

    /// <summary>
    /// Email to which the translational email will be sent, would replace the "To"-property of the transactional email.
    /// </summary>
    [Setting(nameof(MailingLists), View = "Ns.PropertyEditorUi.MailingListPicker", IsMandatory = true, DisplayOrder = 50)]
    public string? MailingLists { get; set; }

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

        var mailingListsToAddTo = GetConfiguredMailingLists();

        if (mailingListsToAddTo.Any())
        {
            var groupedByWorkspace = mailingListsToAddTo.GroupBy(x => x.WorkspaceKey);

            foreach (var workspaceGroupedLists in groupedByWorkspace)
            {
                var addRecipientRequest = new AddRecipientRequest(valueEmail)
                    .ForWorkspace(workspaceGroupedLists.Key)
                    .WithName(valueName)
                    .WithSource($"Umbraco Form: " + context.Form.Name); //Max 255 characters should be fine

                foreach (var list in workspaceGroupedLists)
                {
                    // By not passing a status, any double opt-in configuration will be respected.
                    addRecipientRequest.SubscribeTo(list.MailingListKey);
                }

                //TODO: Handle result, log errors.
                var result = _newsletterStudioService.AddRecipient(addRecipientRequest);

            }

        }

        return WorkflowExecutionStatus.Completed;
    }

    public override List<Exception> ValidateSettings()
    {
        var exceptions = new List<Exception>();

        if (GetConfiguredMailingLists().Count < 1)
            exceptions.Add(new Exception("Please choose at least one mailing list."));

        return exceptions;
    }

    private List<MailingListPropertyEditorValueModel> GetConfiguredMailingLists()
    {
        if (string.IsNullOrEmpty(this.MailingLists))
            return new List<MailingListPropertyEditorValueModel>();

        try
        {
            return JsonConvert.DeserializeObject<List<MailingListPropertyEditorValueModel>>(this.MailingLists) ?? new List<MailingListPropertyEditorValueModel>();
        }
        catch 
        {
        }

        return new List<MailingListPropertyEditorValueModel>();

    }
}
