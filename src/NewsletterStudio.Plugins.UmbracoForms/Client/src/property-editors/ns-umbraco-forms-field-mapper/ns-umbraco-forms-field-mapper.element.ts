import {LitElement,html,css,customElement,property,query, state, when} from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { UmbPropertyEditorConfigCollection,UmbPropertyEditorUiElement,UmbPropertyValueChangeEvent} from "@umbraco-cms/backoffice/property-editor";
import { NsMailingListPickerElement } from "@newsletterstudio/umbraco/components";
import "@newsletterstudio/umbraco/components";
import { tryExecuteAndNotify } from "@umbraco-cms/backoffice/resources";
import { GetConfigurationResponse, UmbracoFormsResource } from "../../backend-api";
import { FakeFormsWorkspaceContext } from "./fake-types";
import NsFieldsMapperElement, {NsMappingFieldDefinition} from './../../components/ns-fields-mapper/ns-fields-mapper.element.js'
import './../../components/ns-fields-mapper/ns-fields-mapper.element.js';
import { MailingListPropertyEditorValueModel } from "@newsletterstudio/umbraco/backend";
import { UmbFormControlMixin } from "@umbraco-cms/backoffice/validation";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";

/*
  Should: Be able to get info about "what to map", that is the custom fields needed for a given workspace.
          Ups. That would mean that we can't support lists from different workspaces. That's PROBABLY fine.
  */
@customElement("ns-umbraco-forms-field-mapper")
export default class NsUmbracoFormsFieldMapperElement extends UmbFormControlMixin<PropertyEditorValue | undefined, typeof UmbLitElement>(UmbLitElement, undefined) implements UmbPropertyEditorUiElement {

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

  pickerValue : MailingListPropertyEditorValueModel[] = [];

  async connectedCallback() {
    super.connectedCallback();

    console.log('picker connectedCallback', this.picker);

    var result = await tryExecuteAndNotify(this,UmbracoFormsResource.getConfiguration());
    this._newsletterStudioConfiguration = result.data;

    this.#ensureWorkspaceFields();

    console.log('UmbracoForms Configuration', result.data);

    console.log('picker connectedCallback2', this.picker);
    console.log('picker connectedCallback2 | this.value', this.value);

    this.consumeContext('Forms.GlobalContext', (instance)=>{
      console.log('Forms.GlobalContext',instance);

    });

    this.consumeContext('forms-context', (instance)=>{
      console.log('forms-context',instance);
    });

    this.consumeContext('UmbWorkspaceContext', (i)=>{
      var instance = i as FakeFormsWorkspaceContext;

      console.log('UmbWorkspaceContext',instance);
      instance.data.forEach((item)=>{
        console.log(item.name);
      });

      this.observe(instance.data,(forms)=>{

        const formFields : NsMappingFieldDefinition[] = [];

        // This will list form fields
        forms.pages.forEach((page)=>{
          page.fieldSets.forEach((fieldSet)=>{
            fieldSet.containers.forEach((container)=>{
              container.fields.forEach((field)=>{

                formFields.push({
                  alias : field.alias, //TODO OR id?
                  label : field.caption
                });
              })
            })
          })
        });

        this._formFields = formFields;

      });

    });

  }

  updated(changedProperties: Map<string | number | symbol, unknown>) {
    if (changedProperties.has('value')) {

      this.pickerValue = this.value?.mailingList ? [this.value.mailingList] : [];
      this._mappedFields = this.value?.mappings ?? []

      if(this.pickerValue) {
        this.#ensureWorkspaceFields();
      }

    }
  }

  #handleListPickerChange() {

    this.pickerValue = this.picker?.value ?? [];
    this.#ensureWorkspaceFields();
    this.#triggerValueUpdate();

  }

  #ensureWorkspaceFields() {

    if(this.picker?.value?.length == 0)
    {
      this._workspaceFields = [];
      return;
    }

    // Update fields
    const workspaceFields : NsMappingFieldDefinition[] = [
      {alias:'email',label:'Email'},
      {alias:'name',label:'Name'},
      {alias:'firstName',label:'First name'},
      {alias:'lastName',label:'Last name'},
      {alias:'source',label:'Source'},
    ];

    const selectedWorkspaceKey = this.picker!.value![0].workspaceKey;
    const selectedWorkspace = this._newsletterStudioConfiguration?.workspaces.filter(x=>x.key == selectedWorkspaceKey).shift();

    selectedWorkspace?.customFields.forEach((field)=>{
      workspaceFields.push({
        alias : field.alias,
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
    this.value = {
      mailingList : this.pickerValue[0],
      mappings : this._mappedFields ?? []
    } as PropertyEditorValue

    this.dispatchEvent(new UmbPropertyValueChangeEvent());
  }



  render() {
    return html`<div>
      <!--@ts-ignore-->
      <ns-mailing-list-picker
        .value=${this.pickerValue}
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

  // <umb-debug visible dialog></umb-debug>

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
  fieldAlias : string;
  valueAlias : string;
  staticValue : string | null | undefined;
}

type PropertyEditorValue = {
  mappings : NsFieldMapping[] | undefined,
  mailingList : MailingListPropertyEditorValueModel | undefined;
}
