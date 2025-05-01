import type { UmbLocalizationDictionary } from '@umbraco-cms/backoffice/localization-api';

export default {
  formProviderWorkflows : {
    TransactionalEmailLabel : 'Transactional Email',
    TransactionalEmailDescription : 'Pick the Transactional Email to send',

    SendToEmailLabel : 'Send to Email',
    SendToEmailDescription : 'Optional. Use this to send to a email from a form field.',

    AddToMailingListConfigurationLabel : 'Mailing List Configuration',
    AddToMailingListConfigurationDescription : 'Choose a mailing list and mapp the fields.'
  },
  nsUmbracoForms : {
    recipient : 'Recipient',
    formField : 'Form field',
    notAssigned : 'Not assigned',
    static: 'Static value',
    staticPlaceholder : 'Enter static value'
  }

} as UmbLocalizationDictionary
