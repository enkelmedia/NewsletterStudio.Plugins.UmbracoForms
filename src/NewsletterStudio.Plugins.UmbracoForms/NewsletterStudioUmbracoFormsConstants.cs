using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsletterStudio.Plugins.UmbracoForms;

public static class NewsletterStudioUmbracoFormsConstants
{
    /// <summary>
    /// List of field keys (Umbraco.Forms.Core.Constants.FieldTypes) that is known to be multi-line.
    /// </summary>
    public static List<string> KnownMultiLineFieldKeys = [
        Umbraco.Forms.Core.Constants.FieldTypes.RichText,
        Umbraco.Forms.Core.Constants.FieldTypes.Textarea
    ];

    internal static Lazy<List<Guid>> _knownMultiLineFields = new Lazy<List<Guid>>(() =>
    {
        var fieldKeys = new List<Guid>();

        foreach (var knownFieldKey in KnownMultiLineFieldKeys)
        {
            if (Guid.TryParse(knownFieldKey, out Guid fieldKey))
            {
                fieldKeys.Add(fieldKey);
            }
        }


        return fieldKeys;

    });

}
