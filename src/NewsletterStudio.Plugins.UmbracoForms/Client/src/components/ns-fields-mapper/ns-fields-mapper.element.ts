import {LitElement,html,css,customElement,property,query, repeat, when} from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import "@newsletterstudio/umbraco/components";

/**
 * Component that lists a set of "known" fields to be mapped against another list of fields (and/or static values)
 */
@customElement("ns-fields-mapper")
export class NsFieldsMapperElement extends UmbElementMixin(
  LitElement
) {

  @property({type:Array})
  value : NsFieldMapping[] = [];

  @property({type:Array})
  fieldsToMap : NsMappingFieldDefinition[] = [];

  @property({type:Array})
  pickableFields : NsMappingFieldDefinition[] = [];

  pickerValue = [];

  async connectedCallback() {
    super.connectedCallback();
  }

  async firstUpdated() {

  }

  #handleFieldToChange(event : InputEvent) {
    var select = event.target as HTMLSelectElement;
    var mappedField = select.getAttribute('data-field-alias')!;
    this.#setOrUpdateValue(mappedField, select.value);
  }

  #handleStaticInputChange(event : InputEvent) {
    var input = event.target as HTMLInputElement;
    var mappedFieldAlias = input.getAttribute('data-field-alias')!;

    let newValue = [...this.value];
    let existing = newValue.find(x=>x.fieldAlias == mappedFieldAlias);

    if(existing){
      let index = newValue.indexOf(existing);
      newValue.splice(index,1,{...existing,staticValue:input.value});
    }

    this.value = newValue;

    this.dispatchEvent(new CustomEvent('change'));

  }

  #setOrUpdateValue(alias:string,value:string) {

    let newValue = [...this.value];

    let existing = newValue.find(x=>x.fieldAlias == alias);
    if(existing && value == ''){
      let index = newValue.indexOf(existing);
      newValue.splice(index,1);
    }
    if(existing){
      let index = newValue.indexOf(existing);
      newValue.splice(index,1,{fieldAlias:alias,valueAlias:value,staticValue : ''});
    }
    else {
      newValue.push({fieldAlias:alias,valueAlias:value,staticValue : ''});
    }

    this.value = newValue;

    this.dispatchEvent(new CustomEvent('change'));

  }

  render() {
    return html`<div>
      <table>
        <thead>
          <tr>
            <th>Recipient</th>
            <th>Form field</th>
          </tr>
        </thead>
        <tbody>
        ${repeat(this.fieldsToMap,(field)=>html`
          <tr>
            <td>${field.label}</td>
            <td>
              <select
                data-field-alias=${field.alias}
                @change=${this.#handleFieldToChange}>
                <option value="" ?selected=${this.value.find(x=>x.fieldAlias == field.alias)?.valueAlias == ''}>Unmapped</option>
                ${repeat(this.pickableFields,(option)=>html`
                  <option value=${option.alias} ?selected=${this.value.find(x=>x.fieldAlias == field.alias)?.valueAlias == option.alias}>${option.label}</option>
                `)}
                <option value="static" ?selected=${this.value.find(x=>x.fieldAlias == field.alias)?.valueAlias == 'static'}>Static</option>
              </select>
              ${when(this.value.find(x=>x.fieldAlias == field.alias)?.valueAlias == 'static', ()=>html`
                <div>
                  <input
                  type="text"
                  placeholder="Enter static value"
                  .value=${this.value.find(x=>x.fieldAlias == field.alias)?.staticValue ?? ''}
                  data-field-alias=${field.alias}
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
  alias : string;
  label : string;
}

export type NsFieldMapping = {
  fieldAlias : string;
  valueAlias : string;
  staticValue : string | null | undefined;
}

declare global {
  interface HTMLElementTagNameMap {
    'ns-fields-mapper': NsFieldsMapperElement;
  }
}
