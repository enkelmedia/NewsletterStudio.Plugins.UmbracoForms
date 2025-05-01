import type { UmbBackofficeExtensionRegistry } from "@umbraco-cms/backoffice/extension-registry";
import { TransactionalPickerSettingValueConverter } from "./property-editors/transactional-picker-settings-value-converter.js";
import { MailingListPickerSettingValueConverter } from "./property-editors/mailing-list-picker-settings-value-converter.js";
import { UmbracoFormsFieldMapperSettingsValueConverter } from "./property-editors/umbraco-forms-field-mapper-value-converter.js";

const translationManifests : Array<UmbExtensionManifest> = [
	{
		type: "localization",
		alias: "Ns.Plugin.UmbracoForms.Localize.EnUS",
		name: "English (United States)",
		meta: {
			"culture": "en-us"
		},
		js : ()=> import('./localization/en-us.js')
	}
]

const transactionalPickerSettingsValueConverter = {
  type: "formsSettingValueConverter",
  alias: "Ns.Plugin.UmbracoForms.SettingValueConverter.TransactionalPicker",
  name: "Newsletter Studio Plugin Umbraco Forms Settings Converter Transactional Picker",
  propertyEditorUiAlias: "Ns.PropertyEditorUi.TransactionalEmailPicker",
  api: TransactionalPickerSettingValueConverter,
}

const mailingListPickerSettingsValueConverter = {
  type: "formsSettingValueConverter",
  alias: "Ns.Plugin.UmbracoForms.SettingValueConverter.MailingListPicker",
  name: "Newsletter Studio Plugin Umbraco Forms Settings Converter Mailing List Picker",
  propertyEditorUiAlias: "Ns.PropertyEditorUi.MailingListPicker",
  api: MailingListPickerSettingValueConverter,
}

const nsUmbracoFormsAddToMailingListConfigurationValueConverter = {
  type: "formsSettingValueConverter",
  alias: "Ns.Plugin.UmbracoForms.SettingValueConverter.FormsFields",
  name: "Newsletter Studio Plugin Umbraco Forms Settings Converter Forms Add To Mailing List Configuration",
  propertyEditorUiAlias: "Ns.Plugin.UmbracoForms.AddToMailingListPropertyEditorUi",
  api: UmbracoFormsFieldMapperSettingsValueConverter,
}

const nsUmbracoFormsAddToMailingListConfigurationPropertyEditorUi : UmbExtensionManifest = {
  "type": "propertyEditorUi",
  "alias": "Ns.Plugin.UmbracoForms.AddToMailingListPropertyEditorUi",
  "name": "Newsletter Studio Plugin Umbraco Forms Add Recipients Settings Property Editor",
  "element" : ()=> import('./property-editors/ns-umbraco-forms-field-mapper/ns-umbraco-forms-field-mapper.element.ts'),
  "meta": {
   "label": "Newsletter Studio Add Config For Umbraco Forms",
   "propertyEditorSchemaAlias": "Umbraco.TextBox", // Might be this that needs to go.
   "icon": "icon-autofill",
   "group": "common"
  }
}

export function registerManifest(registry : UmbBackofficeExtensionRegistry) {
    registry.registerMany([
      nsUmbracoFormsAddToMailingListConfigurationPropertyEditorUi,
      transactionalPickerSettingsValueConverter,
      mailingListPickerSettingsValueConverter,
      nsUmbracoFormsAddToMailingListConfigurationValueConverter,
    ...translationManifests
	]);
}
