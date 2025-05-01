namespace NewsletterStudio.Plugins.UmbracoForms.Backoffice.Models;

public class GetConfigurationResponse
{
    public List<WorkspaceDefinitionFrontendModel> Workspaces { get; set; } = new List<WorkspaceDefinitionFrontendModel>();
}

public class WorkspaceDefinitionFrontendModel
{
    public required Guid Key { get; set; }
    public required string Name { get; set; }

    public List<WorkspaceCustomFieldFrontendModel> CustomFields { get; set; } = new List<WorkspaceCustomFieldFrontendModel>();
}

public class WorkspaceCustomFieldFrontendModel
{
    public required string Label { get; set; }
    public required string Alias { get; set; }
}
