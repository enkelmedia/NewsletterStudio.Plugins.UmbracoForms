import { UmbPropertyValueData } from "@umbraco-cms/backoffice/property";
import { UmbPropertyEditorConfig } from "@umbraco-cms/backoffice/property-editor";

export class UmbracoFormsFieldMapperSettingsValueConverter {

  /**
   * Called when the view is loaded, convert stored value to property values expected value
   */
  async getSettingValueForEditor(setting : string, alias: string, value: string) {

    if (value.length === 0) {
      return Promise.resolve(undefined);
    }

    var obj = JSON.parse(value);

    if(obj)
    {
      return Promise.resolve(obj);
    }

    return Promise.resolve(undefined);

  }

  /**
   * Called when view it storing the value, convert value from property values expected to storage.
   */
  async getSettingValueForPersistence(setting : string, valueData: UmbPropertyValueData) {

    if(!valueData.value){
      return Promise.resolve(undefined);
    }
    if((valueData.value!  as Array<any>).length == 0){
      return Promise.resolve(undefined);
    }
    var json = JSON.stringify(valueData.value);
    return Promise.resolve(json);
  }

  async getSettingPropertyConfig(setting : string, alias: string, values: UmbPropertyValueData[]) {

    const config: UmbPropertyEditorConfig = [];

    return Promise.resolve(config);
  }
}
