using NewsletterStudio.Core.Rendering;

namespace NewsletterStudio.Plugins.UmbracoForms;

internal static class Constants
{
    public const string PackageId = "NewsletterStudio.Plugins.UmbracoForms";

    internal static class GenericMergeFields {

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

    }
}
