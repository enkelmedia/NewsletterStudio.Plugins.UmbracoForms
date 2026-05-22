import { Field, Page } from "@umbraco-forms/backoffice";

/**
 * Iterates the form data to extract all fields
 * @param umbracoFormsData
 * @returns
 */
export function getAllFields(pages : Page[]) {

  var allFields : Field[] = [];

  // This will list form fields
  pages.forEach((page)=>{
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
