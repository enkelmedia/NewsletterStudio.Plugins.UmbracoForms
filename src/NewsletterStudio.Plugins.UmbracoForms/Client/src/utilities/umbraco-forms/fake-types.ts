import { UmbContextMinimal } from "@umbraco-cms/backoffice/context-api";
import { FormsFormWorkspaceContext } from "@umbraco-forms/backoffice";

/**
 * Faked types for the "Umbraco Forms Context", this is not exported by the official npm package.
 */
export type FakeFormsWorkspaceContext = UmbContextMinimal & {
  formWorkspaceContext : FormsFormWorkspaceContext
}
