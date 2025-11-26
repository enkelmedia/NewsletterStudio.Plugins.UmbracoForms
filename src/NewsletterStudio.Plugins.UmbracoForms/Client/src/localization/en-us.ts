import type { UmbLocalizationDictionary } from '@umbraco-cms/backoffice/localization-api';

export default {
  formProviderWorkflows : {
    TransactionalEmailLabel : 'Transactional Email',
    TransactionalEmailDescription : 'Pick the Transactional Email Template to send',

    SendToEmailLabel : 'Send to Email',
    SendToEmailDescription : 'Optional. Sends the email to this address in addition to the recipients configured in the Transactional Email Template.',

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
