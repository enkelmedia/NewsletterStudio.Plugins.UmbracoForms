import { Observable } from "@umbraco-cms/backoffice/observable-api"

export type FakeFormsWorkspaceContext = {
  data : Observable<FakeFormsWorkspaceData>
}

export type FakeFormsWorkspaceData = {
  name:string;
  pages : FakeFormsPage[];
}

export type FakeFormsPage = {
  fieldSets : FakeFormsFieldset[]
}

export type FakeFormsFieldset = {
  containers : FakeFormsContainer[]
}

export type FakeFormsContainer = {
  fields : FakeFormsField[]
}

export type FakeFormsField = {
  caption:string;
  alias:string;
  id:string;
}
