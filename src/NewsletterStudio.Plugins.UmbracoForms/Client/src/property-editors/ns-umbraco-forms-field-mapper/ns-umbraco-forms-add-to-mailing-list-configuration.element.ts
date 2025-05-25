import {html,css,customElement,property,query, state, when} from "@umbraco-cms/backoffice/external/lit";
import { UmbPropertyEditorConfigCollection,UmbPropertyEditorUiElement,UmbPropertyValueChangeEvent} from "@umbraco-cms/backoffice/property-editor";
import { NsMailingListPickerElement } from "@newsletterstudio/umbraco/components";
import { tryExecute, tryExecuteAndNotify } from "@umbraco-cms/backoffice/resources";
import { GetConfigurationResponse, UmbracoFormsResource } from "../../backend-api/index.js";
import { FakeFormsWorkspaceContext } from "../../utilities/umbraco-forms/fake-types.js";
import NsFieldsMapperElement, {NsMappingFieldDefinition} from '../../components/ns-fields-mapper/ns-fields-mapper.element.js'
import { MailingListPropertyEditorValueModel } from "@newsletterstudio/umbraco/backend";
import { UmbFormControlMixin } from "@umbraco-cms/backoffice/validation";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { getAllFields } from "../../utilities/umbraco-forms/umbraco-forms-utilities.js";
import "@newsletterstudio/umbraco/components";
import '../../components/ns-fields-mapper/ns-fields-mapper.element.js';

/**
* Property editor for configuration of the "Add to Mailing List" workflow for Umbraco Forms.
* @element ns-umbraco-forms-field-mapper
* @fires CustomEvent#change - When the mapping changes changes
*/
@customElement("ns-umbraco-forms-add-to-mailing-list-configuration")
export default class NsUmbracoFormsAddToMailingListConfigurationElement extends
  UmbFormControlMixin<PropertyEditorValue | undefined, typeof UmbLitElement>(UmbLitElement, undefined) implements UmbPropertyEditorUiElement
{

  @query("ns-mailing-list-picker")
  picker? : NsMailingListPickerElement;

  @property({ attribute: false })
  public config: UmbPropertyEditorConfigCollection | undefined;

  /** Holds list of fields from the current form */
  @state()
  _formFields : NsMappingFieldDefinition[] = [];

  /** Holds list of fields for the selected mailing list (or it's workspace to be more precise) */
  @state()
  _workspaceFields : NsMappingFieldDefinition[] = [];

  @state()
  _newsletterStudioConfiguration? : GetConfigurationResponse;

  @state()
  _mappedFields : NsFieldMapping[] = [];

  @state()
  _pickerValue : MailingListPropertyEditorValueModel[] = [];

  async connectedCallback() {
    super.connectedCallback();

    var result = await tryExecute(this,UmbracoFormsResource.getConfiguration());
    this._newsletterStudioConfiguration = result.data;

    this.#ensureWorkspaceFields();

    this.consumeContext('UmbWorkspaceContext', (i) => {

      if(!i)
        return;

      var instance = i as FakeFormsWorkspaceContext;

      this.observe(instance.data,(forms)=>{
        const allFields = getAllFields(forms);
        const formFields : NsMappingFieldDefinition[] = allFields.map(field=> ({id:field.id, label:field.caption}));
        this._formFields = formFields;
      });
    });

  }

  updated(changedProperties: Map<string | number | symbol, unknown>) {
    if (changedProperties.has('value')) {

      this._pickerValue = this.value?.mailingList ? [this.value.mailingList] : [];
      this._mappedFields = this.value?.mappings ?? []

      if(this._pickerValue) {
        this.#ensureWorkspaceFields();
      }

    }
  }

  #handleListPickerChange() {
    this._pickerValue = this.picker?.value ?? [];
    this.#ensureWorkspaceFields();
    this.#triggerValueUpdate();
  }

  /** Ensure that _workspaceFields are up to date, this is reflected in the ns-field-mapper element. */
  #ensureWorkspaceFields() {

    if(this.picker?.value?.length == 0)
    {
      this._workspaceFields = [];
      return;
    }

    // Well known fields
    const workspaceFields : NsMappingFieldDefinition[] = [
      {id:'email',label:'Email'},
      {id:'name',label:'Name'},
      {id:'firstName',label:'First name'},
      {id:'lastName',label:'Last name'},
      {id:'source',label:'Source'},
    ];

    const selectedWorkspaceKey = this.picker!.value![0].workspaceKey;
    const selectedWorkspace = this._newsletterStudioConfiguration?.workspaces.filter(x=>x.key == selectedWorkspaceKey).shift();

    selectedWorkspace?.customFields.forEach((field)=>{
      workspaceFields.push({
        id : field.alias,
        label : field.label
      })
    });

    this._workspaceFields = workspaceFields;

  }

  #handleMappingChange(event : Event){
    var mappingElement = event.target as NsFieldsMapperElement;

    this._mappedFields = mappingElement.value;
    this.#triggerValueUpdate();

  }

  #triggerValueUpdate(){

    const mappedFields : NsFieldMapping[] = [];

    if(this._mappedFields) {
      this._mappedFields.forEach((field)=>{

        // ensure that the mapped field exists on the model (might have been removed).
        var fieldToMap = this._workspaceFields.find(x=>x.id == field.fieldId);
        if(fieldToMap) {
          mappedFields.push(field);
        }

      });
    }

    this.value = {
      mailingList : this._pickerValue[0],
      mappings : mappedFields
    } as PropertyEditorValue

    this.dispatchEvent(new UmbPropertyValueChangeEvent());
  }

  render() {
    return html`<div>
      <!--@ts-ignore-->
      <ns-mailing-list-picker
        .value=${this._pickerValue}
        @change=${this.#handleListPickerChange}
        min=${1}
        max=${1}
      ></ns-mailing-list-picker>
      ${when(this._workspaceFields.length > 0,()=>html`
        <hr/>
        <ns-fields-mapper
          .fieldsToMap=${this._workspaceFields}
          .pickableFields=${this._formFields}
          .value=${this._mappedFields}
          @change=${this.#handleMappingChange}
        ></ns-fields-mapper>
      `)}
    </div>`;
  }

  static styles = [
    css`
      * {
        box-sizing:border-box;
      }

      hr {
        border: none;
        height: 1px;
        background-color: var(--uui-color-divider);
      }

      uui-input {
        width: 100%;
      }
    `,
  ];
}

export type NsFieldMapping = {
  fieldId : string;
  valueId : string;
  staticValue : string | null | undefined;
}

type PropertyEditorValue = {
  mappings : NsFieldMapping[] | undefined,
  mailingList : MailingListPropertyEditorValueModel | undefined;
}
