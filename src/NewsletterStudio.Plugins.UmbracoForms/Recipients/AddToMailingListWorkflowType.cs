using Microsoft.Extensions.Logging;
using NewsletterStudio.Core.Backoffice.PropertyEditors.MailingListPicker.FrontendModels;
using NewsletterStudio.Core.Public;
using NewsletterStudio.Core.Public.Models;
using NewsletterStudio.Plugins.UmbracoForms.Utilities;
using Newtonsoft.Json;
using Umbraco.Extensions;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace NewsletterStudio.Plugins.UmbracoForms.Recipients;

public class AddToMailingListWorkflowType : WorkflowType
{
    private readonly ILogger<AddToMailingListWorkflowType> _logger;
    private readonly INewsletterStudioService _newsletterStudioService;

    /// <summary>
    /// Holds a JSON object of all the configuration, use <see cref="GetConfiguration"/> to get a deserialized instance.
    /// </summary>
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
        var configuration = GetConfiguration();
        var formValues = new UmbracoFormsHelper().ExtractFormCollection(context.Record);

        // Ensure email field exists.
        var emailField = ExtractField(configuration, formValues, WellKnownFieldsAliases.Email);

        if (emailField == null || string.IsNullOrEmpty(emailField.Value))
        {
            _logger.LogWarning($"Newsletter Studio | Umbraco Forms | {nameof(AddToMailingListWorkflowType)} skipped due to invalid e-mail");
            return WorkflowExecutionStatus.Failed;
        }

        if (configuration?.MailingList is null)
        {
            _logger.LogWarning($"Newsletter Studio | Umbraco Forms | {nameof(AddToMailingListWorkflowType)} skipped due to invalid mailing list configuration");
            return WorkflowExecutionStatus.Failed;
        }
        
        var addRecipientRequest = new AddRecipientRequest(emailField.Value)
            .ForWorkspace(configuration.MailingList.WorkspaceKey)
            .SubscribeTo(configuration.MailingList.MailingListKey);
        
        if (TryExtractNonEmptyField(configuration, formValues, WellKnownFieldsAliases.Name, out ResolvedMapping nameField))
            addRecipientRequest.WithName(nameField.Value!);

        if (TryExtractNonEmptyField(configuration, formValues, WellKnownFieldsAliases.FirstName, out ResolvedMapping firstNameField))
            addRecipientRequest.WithFirstname(firstNameField.Value!);

        if (TryExtractNonEmptyField(configuration, formValues, WellKnownFieldsAliases.LastName, out ResolvedMapping lastNameField))
            addRecipientRequest.WithLastname(lastNameField.Value!);

        if (TryExtractNonEmptyField(configuration, formValues, WellKnownFieldsAliases.Source, out ResolvedMapping sourceField))
        {
            addRecipientRequest.WithSource(sourceField.Value!);
        }
        else
        {
            addRecipientRequest.WithSource("Umbraco Forms");
        }

        // Mapp all fields that is not well known
        foreach (var mappedField in configuration.Mappings!)
        {
            if(WellKnownFieldsAliases.All.Contains(mappedField.FieldId))
                continue;

            var formField = ExtractField(configuration, formValues, mappedField.FieldId);

            if(formField == null || formField.Value == null)
                continue;

            addRecipientRequest.WithCustomField(formField.RecipientFieldAlias, formField.Value);

        }

        var result = _newsletterStudioService.AddRecipient(addRecipientRequest);
        if (result.Failed)
        {
            _logger.LogError("Newsletter Studio | Umbraco Forms | Error when adding recipient: {ErrorMessage}.", result.Message);
            return WorkflowExecutionStatus.Failed;
        }

        return WorkflowExecutionStatus.Completed;
    }

    

    public override List<Exception> ValidateSettings()
    {
        var exceptions = new List<Exception>();
        var configuration = this.GetConfiguration();

        if(configuration?.MailingList is null)
            exceptions.Add(new Exception("Please choose a mailing list."));

        var emailMapping = configuration!.Mappings?.FirstOrDefault(x => x.FieldId.Equals(WellKnownFieldsAliases.Email));
        if (emailMapping == null || string.IsNullOrEmpty(emailMapping.ValueId))
            exceptions.Add(new Exception("Please provide a mapping for recipient email"));
        
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

    private bool TryExtractNonEmptyField(AddToMailingListConfigurationModel? configuration, FormFieldCollection formValues, string recipientFieldAlias, out ResolvedMapping formField)
    {
        formField = ExtractField(configuration, formValues, recipientFieldAlias)!;

        if (formField == null! || formField.Value == null)
            return false;

        return true;

    }

    private ResolvedMapping? ExtractField(AddToMailingListConfigurationModel? configuration, FormFieldCollection formValues, string recipientFieldAlias)
    {
        if (configuration?.Mappings == null)
            return null;

        var mapping = configuration?.Mappings.FirstOrDefault(x => x.FieldId == recipientFieldAlias);

        if (mapping == null)
            return null;

        if (string.IsNullOrEmpty(mapping.ValueId))
            return null;

        if (mapping.ValueId.InvariantEquals("static"))
            return new ResolvedMapping(recipientFieldAlias) { Value = mapping.StaticValue };

        if (Guid.TryParse(mapping.ValueId, out Guid fieldId))
        {
            var formField = formValues.GetById(fieldId);
            if (formField == null)
                return null;

            return new ResolvedMapping(recipientFieldAlias) { Value = formField.Value, FormField = formField };

        }

        return null;
    }

}

internal class ResolvedMapping
{
    public ResolvedMapping(string recipientFieldAlias)
    {
        RecipientFieldAlias = recipientFieldAlias;
    }

    public string RecipientFieldAlias { get; set; }

    public string? Value { get; set; }

    public FormFieldValue? FormField { get; set; }
}

internal static class WellKnownFieldsAliases
{
    public const string Email = "email";
    public const string Name = "name";
    public const string FirstName = "firstName";
    public const string LastName = "lastName";
    public const string Source = "source";

    public static List<string> All = [Email, Name, FirstName, LastName, Source];

}

public class AddToMailingListConfigurationModel
{
    public MailingListPropertyEditorValueModel? MailingList { get; set; }
    public List<NewsletterStudioFieldMapping>? Mappings { get; set; }
}

public class NewsletterStudioFieldMapping
{
    /// <summary>
    /// Identifier or alias for field to map, expected to be a field alias.
    /// </summary>
    public string FieldId { get; set; } = "";

    /// <summary>
    /// Identifier or alias for value field, expected to be a GUID id for the form field
    /// </summary>
    public string ValueId { get; set; } = "";

    /// <summary>
    /// Optional static value
    /// </summary>
    public string? StaticValue { get; set; }
}
