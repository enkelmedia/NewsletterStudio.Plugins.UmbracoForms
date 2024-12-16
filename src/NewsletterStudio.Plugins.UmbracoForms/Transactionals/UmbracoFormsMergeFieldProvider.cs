using NewsletterStudio.Core.Rendering.MergeFields;
using NewsletterStudio.Core.Rendering;
using Umbraco.Forms.Core.Services;

namespace NewsletterStudio.Plugins.UmbracoForms.Transactionals;

public class UmbracoFormsMergeFieldProvider : ITransactionalEmailMergeFieldProvider
{
    public static readonly MergeField PostedFromUrlMergeField = new MergeField()
    {
        Placeholder = "postedFromUrl",
        Text = "Posted From Url",
        GroupText = "Information"
    };

    public static readonly MergeField FormNameMergeField = new MergeField()
    {
        Placeholder = "formName",
        Text = "Form Name",
        GroupText = "Information"
    };

    public static readonly MergeField AllFormFieldsUrlMergeField = new MergeField()
    {
        Placeholder = "allFormFields",
        Text = "All Form Fields",
        GroupText = "Automatic"
    };

    private readonly IFormService _formService;

    public UmbracoFormsMergeFieldProvider(IFormService formService)
    {
        _formService = formService;
    }

    public const string ProviderAlias = "umbForm";
    public string Alias => ProviderAlias;
    public string DisplayName => "Umbraco Forms";

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

            list.Add(PostedFromUrlMergeField);
            list.Add(FormNameMergeField);
            list.Add(AllFormFieldsUrlMergeField);

            //TODO v2: Notifications to add site-specific stuff.

        }

        return list;

    }

    public MergeFieldValuesCollection ExtractValues(TransactionalMergeFieldValuesRequestModel request)
    {
        if (request.CustomModel.GetType().Name == "UmbracoForms.Core.Models.Recored")
        {
            // Do something cool
        }

        // Thinking that we could pass a "Record" or something here and check if it is a record, then extract values.


        // What would we do here? Expect a "Record" from forms? Or what? Might work. Otherwise the code that calles the "send email"-method on Newsletter Studio service would have to 
        // know how to "parse" the aliases for the models.

        return new MergeFieldValuesCollection(new List<MergeFieldValue>()
            {
                new MergeFieldValue("firstname", "HardcodedFirstname"),
                new MergeFieldValue("lastname", "HardcodedLastname"),
                new MergeFieldValue("howManyPeople", "45")
            });

    }
}
