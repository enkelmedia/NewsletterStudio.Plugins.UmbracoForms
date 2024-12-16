[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![NuGet version (NewsletterStudio.Plugins.UmbracoForms)](https://img.shields.io/nuget/v/NewsletterStudio.Plugins.UmbracoForms.svg?style=flat-square)](https://www.nuget.org/packages/NewsletterStudio.Plugins.UmbracoForms/)

# Newsletter Studio - Umbraco Forms
Here we keep the source files for the the Umbraco Forms-plugin for [Newsletter Studio](https://our.umbraco.com/packages/backoffice-extensions/newsletter-studio-the-email-studio/) for Umbraco CMS.

This plugin contains two main components:

* **Send Transactional Workflow**
  A Umbraco Forms Workflow that makes it possible to send a Transactional Email designed in Newsletter Studio when a Form is submitted.
* **Merge Field Provider**
  A merge field provider for Newsletter Studio to facilitate "pick and choose" for fields from a Umbraco Form.


## Install via NuGet:
```
dotnet add package NewsletterStudio.Plugins.UmbracoForms
```

## Contribute
Contributions are more than welcome, start with opening an issue.

## Known Issues

* [ ] Umb8: Listing Forms (IFormService db-error)
* [ ] Umb9: Mailing List Picker