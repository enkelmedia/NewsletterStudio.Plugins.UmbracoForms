import type { UmbEntryPointOnInit } from '@umbraco-cms/backoffice/extension-api';
import { registerManifest } from './manifest.js';

export const onInit: UmbEntryPointOnInit = (host, extensionRegistry) => {
  registerManifest(extensionRegistry);
};