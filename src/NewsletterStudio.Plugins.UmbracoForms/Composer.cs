using NewsletterStudio.Core.Composing;
using NewsletterStudio.Plugins.UmbracoForms.Recipients;
using NewsletterStudio.Plugins.UmbracoForms.Transactionals;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Forms.Core.Providers;

namespace NewsletterStudio.Plugins.UmbracoForms;

public class Composer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.WithCollectionBuilder<WorkflowCollectionBuilder>().Add<SendTransactionalWorkflowType>();
        builder.WithCollectionBuilder<WorkflowCollectionBuilder>().Add<AddToMailingListWorkflowType>();

        builder.NewsletterStudio().TransactionalMergeFieldProviders.Append<UmbracoFormsMergeFieldProvider>();
    }
}
