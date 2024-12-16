using NewsletterStudio.Core.Rendering.MergeFields;
using NewsletterStudio.Core.Rendering;
using Umbraco.Forms.Core.Services;

namespace NewsletterStudio.Plugins.UmbracoForms.Transactionals;

public class UmbracoFormsMergeFieldProvider : ITransactionalEmailMergeFieldProvider
{
    
    private readonly IFormService _formService;

    public UmbracoFormsMergeFieldProvider(IFormService formService)
    {
        _formService = formService;
    }

    public const string ProviderAlias = "umbForm";
    public string Alias => ProviderAlias;
    public string DisplayName => "Umbraco Forms";

    /// <summary>
    /// Returns "Groups" for the "Transactional Data Model" drop down. Basically this returns all forms.
    /// </summary>
    /// <returns></returns>
    public List<MergeFieldSourceGroup> GetGroups()
    {
        var list = new List<MergeFieldSourceGroup>();
        var group = new MergeFieldSourceGroup();
        group.Label = "Umbraco Forms";
        group.Sources = new List<MergeFieldSource>();

        var allForms = _formService.GetSlim();

        foreach (var form in allForms)
        {
            group.Sources.Add(new MergeFieldSource()
            {
                Alias = form.Id.ToString(),
                Label = form.Name
            });
        }

        list.Add(group);

        return list;
    }

    public List<MergeField> GetFieldsByAlias(string sourceId)
    {

        var list = new List<MergeField>();

        var form = _formService.Get(Guid.Parse(sourceId));

        if (form != null)
        {
            foreach (var field in form.AllFields)
            {
                list.Add(new MergeField()
                {
                    GroupText = "Fields",
                    Placeholder = field.Alias,
                    Text = field.Caption
                });
            }

            list.Add(Constants.GenericMergeFields.PostedFromUrlMergeField);
            list.Add(Constants.GenericMergeFields.FormNameMergeField);
            list.Add(Constants.GenericMergeFields.AllFormFieldsUrlMergeField);

            //TODO v2: Provide notification to add site-specific generic merge fields.

        }

        return list;

    }

    public MergeFieldValuesCollection ExtractValues(TransactionalMergeFieldValuesRequestModel request)
    {
        //NOTE: Merge fields are added in SendTransactionalWorkflowType since we need access to the form that's being sent.
        //      Just returning an empty collection here.
        return new MergeFieldValuesCollection();
    }
}
