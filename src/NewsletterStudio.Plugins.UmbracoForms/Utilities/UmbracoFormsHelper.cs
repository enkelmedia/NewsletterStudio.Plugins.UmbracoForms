using Umbraco.Forms.Core.Persistence.Dtos;

namespace NewsletterStudio.Plugins.UmbracoForms.Utilities;

internal sealed class UmbracoFormsHelper
{
    //TODO: make this extendable
    internal static Lazy<List<Guid>> KnownMultiLineFields = new Lazy<List<Guid>>(() => new List<Guid>() {
       Guid.Parse(Umbraco.Forms.Core.Constants.FieldTypes.RichText),
       Guid.Parse(Umbraco.Forms.Core.Constants.FieldTypes.Textarea)
    });

    internal FormFieldCollection ExtractFormCollection(Record record)
    {
        return new FormFieldCollection(ExtractFormValues(record));
    }

    internal List<FormFieldValue> ExtractFormValues(Record record)
    {
        var list = new List<FormFieldValue>();

        foreach (var recordField in record.RecordFields)
        {
            var fieldAlias = recordField.Value.Alias;

            object[] objArray;
            if (recordField.Value == null || !recordField.Value.HasValue())
                objArray = new object[1] { (object)string.Empty };
            else
                objArray = recordField.Value.Values.ToArray();

            var value = string.Join(" ", objArray);

            var existingField = list.FirstOrDefault(x => x.Alias == fieldAlias);

            if (existingField != null)
            {
                // Updates if already exists which is unlikely.
                existingField.Value = value;
            }
            else
            {
                list.Add(new FormFieldValue(fieldAlias, value, recordField!.Value!.Field!.Caption, IsMultiLine(recordField.Value)));

            }

        }

        return list;
    }

    private bool IsMultiLine(RecordField? field)
    {
        if (field?.Field == null)
            return false;

        if (KnownMultiLineFields.Value.Contains(field.Field.FieldTypeId))
            return true;

        return false;
    }

}

internal class FormFieldCollection
{
    public List<FormFieldValue> Fields { get; private set; }

    public FormFieldValue? GetByAlias(string alias) => Fields.FirstOrDefault(x => x.Alias.Equals(alias));

    public bool TryGetByAlias(string alias, out FormFieldValue field)
    {
        field = GetByAlias(alias)!;

        return field != null!;

    }

    public FormFieldCollection(List<FormFieldValue> fields)
    {
        Fields = fields;
    }
}

internal class FormFieldValue
{
    public FormFieldValue(string alias, string value, string caption, bool isMultiLine)
    {
        Alias = alias;
        Caption = caption;
        Value = value;
        IsMultiLine = isMultiLine;
    }

    public string Alias { get; set; }
    public string Caption { get; set; }
    public string Value { get; set; }
    public bool IsMultiLine { get; set; }
}
