[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![NuGet version (NewsletterStudio.Plugins.UmbracoForms)](https://img.shields.io/nuget/v/NewsletterStudio.Plugins.UmbracoForms.svg?style=flat-square)](https://www.nuget.org/packages/NewsletterStudio.Plugins.UmbracoForms/)

# Newsletter Studio - Umbraco Forms
Here we keep the source files for the the Umbraco Forms-plugin for [Newsletter Studio](https://our.umbraco.com/packages/backoffice-extensions/newsletter-studio-the-email-studio/) for Umbraco CMS.

This plugin contains two main components:

* **Send Transactional Workflow**, a Umbraco Forms Workflow that makes it possible to send a Transactional Email designed in Newsletter Studio when a Form is submitted. This also includes a [merge field provider](https://www.newsletterstudio.org/documentation/package/15.0.0/develop/merge-field-providers/) so that any fields from the from can be picked in the email designer.

* **Add to Mailing List Workflow**, makes it possible to add a new recipient to one or more Mailing Lists when a form is submitted.  

## Install via NuGet:
```
dotnet add package NewsletterStudio.Plugins.UmbracoForms
```

## Contribute
Contributions are more than welcome, start with opening an issue.
