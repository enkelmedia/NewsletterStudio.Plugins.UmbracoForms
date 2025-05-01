import { FakeFormsField, FakeFormsWorkspaceData } from "./fake-types";

/**
 * Iterates the form data to extract all fields
 * @param umbracoFormsData
 * @returns
 */
export function getAllFields(umbracoFormsData : FakeFormsWorkspaceData) {

  var allFields : FakeFormsField[] = [];

  // This will list form fields
  umbracoFormsData.pages.forEach((page)=>{
    page.fieldSets.forEach((fieldSet)=>{
      fieldSet.containers.forEach((container)=>{
        container.fields.forEach((field)=>{

          allFields.push(field);

        })
      })
    })
  });

  return allFields;

}
