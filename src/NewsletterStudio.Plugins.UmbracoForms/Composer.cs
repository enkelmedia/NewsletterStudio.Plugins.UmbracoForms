using Microsoft.Extensions.DependencyInjection;
using NewsletterStudio.Core.Composing;
using NewsletterStudio.Plugins.UmbracoForms.Backoffice.Api;
using NewsletterStudio.Plugins.UmbracoForms.Recipients;
using NewsletterStudio.Plugins.UmbracoForms.Transactionals;
using Umbraco.Cms.Api.Common.OpenApi;
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

#if RELEASE
        /// NO SWAGGER IN RELEASE
#else
        //// SWAGGER - Only use in debug build to avoid exposing in production messing up things in the core.
        builder.Services.ConfigureOptions<ConfigureNewsletterStudioPluginApiSwaggerGenOptions>();
        builder.Services.AddSingleton<ISchemaIdHandler, NewsletterStudioPluginSchemaIdHandler>();
        builder.Services.AddSingleton<IOperationIdHandler, NewsletterStudioPluginOperationIdHandler>();
#endif
    }
}
