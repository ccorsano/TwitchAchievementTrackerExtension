<script lang="ts">
import { ConfigurationService, type ValidationError } from "../../services/EBSConfigurationService";
import { ActiveConfig, type ExtensionConfiguration, type SteamConfiguration } from "../../common/EBSTypes";
import { EBSVersion } from "../../common/ServerConfig";
import SecretKeyInput from "../SecretKeyInput/SecretKeyInput.svelte";
import ValidationErrorList from "../ValidationErrorList/ValidationErrorList.svelte";
import ConfigSteam_02_SteamID from "../ConfigSteam_02_SteamID/ConfigSteam_02_SteamID.svelte"

export let onValidate: (e: any, configuration: ExtensionConfiguration) => void
export let onBack: (e: any) => void
export let savedConfiguration: ExtensionConfiguration

let isSyntaxValid: boolean = false
let isSyntaxError: boolean = false
let isValidating: boolean = false
let isKeyValid: boolean = false
let enteredApiKey: string = ""
let errors: ValidationError[] = []
let isContinueEnabled = false

const formatRegexp = /^[0-9a-f]+$/i
const formatFullMatchRegexp = (/^[0-9a-f]{32}$/i)

function changeWebApiValue(value: string)
{
    console.log(value)
    let formatCheck = formatFullMatchRegexp.test(value)
    let isValid = formatRegexp.test(value)

    isSyntaxValid = formatCheck
    isSyntaxError = !isValid
    enteredApiKey = value
}

async function onContinue(_e: any)
{
    isValidating = true

    const configuration: ExtensionConfiguration = {
        activeConfig: ActiveConfig.Steam,
        xBoxLiveConfig: null,
        version: EBSVersion,
        steamConfig: {
            webApiKey: enteredApiKey,
        }
    }

    let errors: ValidationError[] = [];
    try
    {
        errors = await ConfigurationService.validateConfiguration(configuration);
        errors = errors.filter(e => e.path == "SteamConfig.WebApiKey");
    }
    catch(e)
    {
        errors.push({
            errorCode: "EBSError",
            errorDescription: "Error validating WebApiKey",
            path: "",
        });
    }

    errors = errors
    isValidating = false
    isKeyValid = errors.length == 0

    if (errors.length == 0)
    {
    }
}

$:{
    let currentConfig: SteamConfiguration | null = savedConfiguration?.steamConfig;
    if (currentConfig?.webApiKey)
    {
        changeWebApiValue(currentConfig.webApiKey);
    }
}

$: isContinueEnabled = isSyntaxValid && !isValidating
</script>

{#if isKeyValid}
<ConfigSteam_02_SteamID
            savedConfiguration={savedConfiguration}
            onValidate={onValidate}
            onBack={_e => isKeyValid = false}
            webApiKey={enteredApiKey} />
{:else}
<label for="webApiKey">Steam WebApi Key</label>
<SecretKeyInput isSyntaxValid={isSyntaxValid} onChangeValue={e => changeWebApiValue(e.currentTarget.value)} keyValue={enteredApiKey} size={37} pattern="[0-9a-fA-F]{32}" placeholder="Enter your Steam WebAPI key" />
<div>
    <span class="icon-info"></span> You can apply for a WebAPI Key on <a href="https://steamcommunity.com/dev/apikey" target="_blank">https://steamcommunity.com/dev/apikey</a>.
</div>
<ValidationErrorList errors={errors} />
<input type="button" value="Cancel" on:click={onBack} />
<input type="button" value="Continue" disabled={!isContinueEnabled} on:click={onContinue} />
{/if}