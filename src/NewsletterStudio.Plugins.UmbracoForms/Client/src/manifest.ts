import type { UmbBackofficeExtensionRegistry } from "@umbraco-cms/backoffice/extension-registry";
import { TransactionalPickerSettingValueConverter } from "./property-editors/transactional-picker-settings-value-converter.js";
import { MailingListPickerSettingValueConverter } from "./property-editors/mailing-list-picker-settings-value-converter.js";

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

export function registerManifest(registry : UmbBackofficeExtensionRegistry) {
    registry.registerMany([
      transactionalPickerSettingsValueConverter,
      mailingListPickerSettingsValueConverter,
    ...translationManifests
	]);
}
