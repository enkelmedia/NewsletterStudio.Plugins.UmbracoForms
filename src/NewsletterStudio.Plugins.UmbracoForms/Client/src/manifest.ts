import type { UmbBackofficeExtensionRegistry } from "@umbraco-cms/backoffice/extension-registry";
import { PickerSettingValueConverter } from "./property-editors/picker-settings-value-converter.js";

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


const pickerSettingsValueConverter : UmbExtensionManifest = {
  type: "formsSettingValueConverter",
  alias: "Ns.Plugin.UmbracoForms.SettingValueConverter.TransactionalPicker",
  name: "Newsletter Studio Plugin Umbraco Forms Settings Converter Transactional Picker",
  //@ts-ignore
  propertyEditorUiAlias: "Ns.PropertyEditorUi.TransactionalEmailPicker",
  api: PickerSettingValueConverter,
}

export function registerManifest(registry : UmbBackofficeExtensionRegistry) {
    registry.registerMany([
    pickerSettingsValueConverter,
    ...translationManifests
	]);
}
