using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsletterStudio.Core.Public;
using NewsletterStudio.Plugins.UmbracoForms.Backoffice.Api;
using NewsletterStudio.Plugins.UmbracoForms.Backoffice.Models;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Web.Common.Routing;

namespace NewsletterStudio.Plugins.UmbracoForms.Backoffice;

internal static class EndpointConfiguration
{
    public const string RouteSegment = "newsletter-studio-umbraco-forms";
    public const string GroupName = "Umbraco Forms";
}

/// <summary>
/// Controller for the Newsletter Studio Umbraco Forms-Plugin
/// </summary>
[ApiExplorerSettings(GroupName = EndpointConfiguration.GroupName)]
[BackOfficeRoute($"{Umbraco.Cms.Core.Constants.Web.ManagementApiPath}{EndpointConfiguration.RouteSegment}")]
[MapToApi(NewsletterStudioPluginApiConfiguration.ApiName)]
public class UmbracoFormsController : ManagementApiControllerBase
{
    private readonly IWorkspaceService _workspaceService;

    public UmbracoFormsController(
        IWorkspaceService workspaceService
        )
    {
        _workspaceService = workspaceService;
    }

    /// <summary>
    /// Returns current Newsletter Studio configuration
    /// </summary>
    [HttpPost("get-configuration")]
    [ProducesResponseType(typeof(GetConfigurationResponse), StatusCodes.Status200OK)]
    public IActionResult GetConfiguration()
    {
        var userAccess = _workspaceService.CurrentUserAccess();
        var workspacesWithAccess = _workspaceService.GetAll().Where(x => userAccess.HasAccessTo(x.UniqueKey)).ToList();

        var response = new GetConfigurationResponse();

        foreach (var workspace in workspacesWithAccess)
        {
            var ws = new WorkspaceDefinitionFrontendModel()
            {
                Key = workspace.UniqueKey,
                Name = workspace.Name,
            };

            foreach (var customField in workspace.Settings.CustomFields)
            {
                ws.CustomFields.Add(new WorkspaceCustomFieldFrontendModel()
                {
                    Alias = customField.MergeFieldAlias,
                    Label = customField.FieldLabel
                });
            }
            
            response.Workspaces.Add(ws);

        }

        return Ok(response);
    }

}
