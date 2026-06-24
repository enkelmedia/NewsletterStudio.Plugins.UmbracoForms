using NewsletterStudio.Core.Composing;
using NewsletterStudio.Plugins.UmbracoForms.Backoffice.Api;
using NewsletterStudio.Plugins.UmbracoForms.Recipients;
using NewsletterStudio.Plugins.UmbracoForms.Transactionals;
using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Api.Management.OpenApi;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Forms.Core.Providers;
using Umbraco.Extensions;

namespace NewsletterStudio.Plugins.UmbracoForms;

public class Composer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.WithCollectionBuilder<WorkflowCollectionBuilder>().Add<SendTransactionalWorkflowType>();
        builder.WithCollectionBuilder<WorkflowCollectionBuilder>().Add<AddToMailingListWorkflowType>();

        builder.NewsletterStudio().TransactionalMergeFieldProviders.Append<UmbracoFormsMergeFieldProvider>();

#if RELEASE
        /// NO OPENAPI DOCUMENT IN RELEASE
#else
        //// OPENAPI - Only use in debug build to avoid exposing in production messing up things in the core.
        builder.AddBackOfficeOpenApiDocument(
            NewsletterStudioPluginApiConfiguration.ApiName,
            document => document
                .WithTitle(NewsletterStudioPluginApiConfiguration.ApiTitle)
                .WithBackOfficeAuthentication()
                .WithJsonOptions(Umbraco.Cms.Core.Constants.JsonOptionsNames.BackOffice)
                .ConfigureOpenApiOptions(options => options.AddOperationTransformer((operation, context, _) =>
                {
                    if (context.Description.ActionDescriptor.RouteValues.TryGetValue("action", out var actionName) &&
                        string.IsNullOrWhiteSpace(actionName) is false)
                    {
                        operation.OperationId = actionName.ToFirstLower();
                    }

                    return Task.CompletedTask;
                })));
#endif
    }
}
