import {LitElement,html,css,customElement,property,query, repeat, when} from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import "@newsletterstudio/umbraco/components";

/**
* Component that lists a set of "known" fields to be mapped against another list of fields (and/or static values)
* @element ns-fields-mapper
* @fires CustomEvent#change - When the mapping changes changes
*/
@customElement("ns-fields-mapper")
export class NsFieldsMapperElement extends UmbElementMixin(
  LitElement
) {

  /**
   * List of defined mappings
   */
  @property({type:Array})
  value : NsFieldMapping[] = [];

  /**
   * Fields to map. Each field in this array will be mappable to a pickable field or static value.
   */
  @property({type:Array})
  fieldsToMap : NsMappingFieldDefinition[] = [];

  /**
   * Pickable fields. Each field to map will be mappable to any of the fields from this property
   */
  @property({type:Array})
  pickableFields : NsMappingFieldDefinition[] = [];

  #handleFieldToChange(event : InputEvent) {
    var select = event.target as HTMLSelectElement;
    var mappedField = select.getAttribute('data-field-id')!;
    this.#setOrUpdateValue(mappedField, select.value);
  }

  #handleStaticInputChange(event : InputEvent) {
    var input = event.target as HTMLInputElement;
    var mappedFieldAlias = input.getAttribute('data-field-id')!;

    let newValue = [...this.value];
    let existing = newValue.find(x=>x.fieldId == mappedFieldAlias);

    if(existing){
      let index = newValue.indexOf(existing);
      newValue.splice(index,1,{...existing,staticValue:input.value});
    }

    this.value = newValue;

    this.dispatchEvent(new CustomEvent('change'));
  }

  /**
   * Sets the mapping value for a given field alias
   * @param alias
   * @param value
   */
  #setOrUpdateValue(alias:string, value:string) {

    let newValue = [...this.value];

    let existing = newValue.find(x=>x.fieldId == alias);
    if(existing && value == ''){
      let index = newValue.indexOf(existing);
      newValue.splice(index,1);
    }
    if(existing){
      let index = newValue.indexOf(existing);
      newValue.splice(index,1,{fieldId:alias,valueId:value,staticValue : ''});
    }
    else {
      newValue.push({fieldId:alias,valueId:value,staticValue : ''});
    }

    this.value = newValue;

    this.dispatchEvent(new CustomEvent('change'));

  }

  #fieldValueIdEquals(fieldId : string, valueId : string) {
    const field = this.value.find(x=>x.fieldId == fieldId);
    return field?.valueId == valueId;
  }

  render() {
    return html`<div>
      <table>
        <thead>
          <tr>
            <th>${this.localize.term('nsUmbracoForms_recipient')}</th>
            <th>${this.localize.term('nsUmbracoForms_formField')}</th>
          </tr>
        </thead>
        <tbody>
        ${repeat(this.fieldsToMap,(field)=>html`
          <tr>
            <td>${field.label}</td>
            <td>
              <select
                data-field-id=${field.id}
                @change=${this.#handleFieldToChange}>
                <option value="" ?selected=${this.#fieldValueIdEquals(field.id,'')}>${this.localize.term('nsUmbracoForms_notAssigned')}</option>
                ${repeat(this.pickableFields,(option)=>html`
                  <option value=${option.id} ?selected=${this.#fieldValueIdEquals(field.id,option.id)}>${option.label}</option>
                `)}
                <option value="static" ?selected=${this.#fieldValueIdEquals(field.id,'static')}>${this.localize.term('nsUmbracoForms_static')}</option>
              </select>
              ${when(this.#fieldValueIdEquals(field.id,'static'), ()=>html`
                <div>
                  <input
                  type="text"
                  placeholder=${this.localize.term('nsUmbracoForms_staticPlaceholder')}
                  .value=${this.value.find(x=>x.fieldId == field.id)?.staticValue ?? ''}
                  data-field-id=${field.id}
                  @change=${this.#handleStaticInputChange}
                  />
                </div>
              `)}
            </td>
          </tr>
        `)}
        </tbody>
      </table>
    </div>`;
  }

  static styles = [
    css`
      * {
        box-sizing:border-box;
      }
      uui-input {
        width: 100%;
      }

      table {
        width: 100%;

        & td {
          vertical-align:top;
          padding:6px 0;
        }

        & th {
          text-align:left;
        }
      }

      select, input {
        width:100%;
        line-height: 20px;
        background-color: white;
        padding: 5.7px 18px 5.7px 8px;
        font-size:14px;
        border: 1px solid var(--ns-control-border-color);
      }

      select {
        margin: 0;
        -webkit-appearance: none;

        position: relative;
        background: var(--ns-control-background-color-neutral);
        background-image: url("data:image/svg+xml;utf8,<svg fill='black' height='24' viewBox='0 0 24 24' width='24' xmlns='http://www.w3.org/2000/svg'><path d='M7 10l5 5 5-5z'/><path d='M0 0h24v24H0z' fill='none'/></svg>");
        background-repeat: no-repeat;
        background-position-x: 100%;
        background-position-y: 3px;

        cursor:pointer;

      }

      select:focus,
      input:focus {
        outline: calc(2px * var(--uui-show-focus-outline, 1)) solid var(--uui-select-outline-color, var(--uui-color-focus));
      }

      input {
        margin-top:3px;
      }
    `,
  ];
}

export default NsFieldsMapperElement;

export type NsMappingFieldDefinition = {
  /** Id or alias for field */
  id : string;

  /** UI label for field */
  label : string;
}

export type NsFieldMapping = {
  fieldId : string;
  valueId : string;
  staticValue : string | null | undefined;
}

declare global {
  interface HTMLElementTagNameMap {
    'ns-fields-mapper': NsFieldsMapperElement;
  }
}
