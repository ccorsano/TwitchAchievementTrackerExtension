<script lang="ts">
import { EBSVersion } from "../../common/ServerConfig"
import { ActiveConfig, type ExtensionConfiguration } from "../../common/EBSTypes"
import { ConfigurationService, type ValidationError } from "../../services/EBSConfigurationService"
import SecretKeyInput from "../SecretKeyInput/SecretKeyInput.svelte"
import ValidationErrorList from "../ValidationErrorList/ValidationErrorList.svelte"
import ConfigXBL_02_XUID from "../ConfigXBL_02_XUID/ConfigXBL_02_XUID.svelte"
    
export let onValidate: (e: any, configuration: ExtensionConfiguration) => void
export let onBack: (e: any) => void
export let savedConfiguration: ExtensionConfiguration
export let isSyntaxValid: boolean = false
export let isSyntaxError: boolean = false
export let isValidating: boolean = false
export let enteredApiKey: string  =''
export let errors: ValidationError[]  =[]
export let isConfirmed: boolean = false

const formatRegexp = /^[0-9a-zA-Z]+$/i
const formatFullMatchRegexp = (/^[0-9a-z]{30,50}$/i)

function onChangeXApiValue(value: string)
{
    changeXApiValue(value)
}

function changeXApiValue(value: string)
{
    let formatCheck = formatFullMatchRegexp.test(value);
    let syntaxValid = formatRegexp.test(value);
    isSyntaxValid = formatCheck
    isSyntaxError = !syntaxValid
    enteredApiKey = value
}

async function onContinue() {
    isValidating = true

    const configuration: ExtensionConfiguration = {
        activeConfig: ActiveConfig.XBoxLive,
        steamConfig: null,
        version: EBSVersion,
        xBoxLiveConfig: {
            xApiKey: enteredApiKey,
        }
    }

    let newErrors: ValidationError[] = [];
    try
    {
        newErrors = await ConfigurationService.validateConfiguration(configuration);
        newErrors = newErrors.filter(e => e.path == "XBoxLiveConfig.XApiKey");
    }
    catch(e)
    {
        newErrors.push({
            errorCode: "EBSError",
            errorDescription: "Error validating WebApiKey",
            path: "",
        });
    }

    errors = newErrors
    isValidating = false
    isConfirmed = errors.length == 0

    if (errors.length == 0)
    {
    }
}

function unvalidate()
{
    isConfirmed = false
}

let currentConfig = savedConfiguration;
if (currentConfig?.xBoxLiveConfig?.xApiKey)
{
    changeXApiValue(currentConfig.xBoxLiveConfig.xApiKey);
}

$: isContinueEnabled = isSyntaxValid && !isValidating
</script>

{#if isConfirmed}
    <ConfigXBL_02_XUID
        savedConfiguration={savedConfiguration}
        onValidate={onValidate}
        onBack={unvalidate}
        xApiKey={enteredApiKey} />
{:else}
    <label for="xapikey">XApi Key</label>
    <SecretKeyInput
        isSyntaxValid={isSyntaxValid}
        onChangeValue={e => onChangeXApiValue(e.currentTarget.value)}
        keyValue={enteredApiKey}
        size={50}
        pattern="[0-9a-zA-Z]&lcub;30,50&rcub;"
        placeholder="Enter your XApi.us key" />

    {#if errors.some(e => e.errorCode == "ExpiredXBLToken")}
    <div>
        Your XApi.us Microsoft token has expired.<br/>
        <a href='https://xapi.us/profile'>Log into your xapi.us account</a> and refresh your token using the "Sign in to XBox LIVE" button.
    </div>
    {:else}
    <div>
        <span class="icon-info"></span> You can register and get your xapi.us API Key on <a href="https://xapi.us/" target="_blank">https://xapi.us/</a>.<br/>
        A free account should be enough in most use case, and once configured the extension will limit its request frequency.<br/>
        Note: This extension is not affiliated with xapi.us in any way or form.
    </div>
    {/if}

    <ValidationErrorList errors={errors} />
    <input type="button" value="Cancel" on:click={onBack} />
    <input type="button" value="Continue" disabled={!isContinueEnabled} on:click={onContinue} />
{/if}