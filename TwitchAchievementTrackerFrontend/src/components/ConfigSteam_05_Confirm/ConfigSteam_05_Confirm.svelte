<script lang="ts">
import { EBSVersion } from "../../common/ServerConfig";
import { ActiveConfig, ExtensionConfiguration } from "../../common/EBSTypes";
import { ConfigurationService, ValidationError } from "../../services/EBSConfigurationService";
import ValidationErrorList from "../ValidationErrorList/ValidationErrorList.svelte";
import SecretKeySpan from "../SecretKeySpan/SecretKeySpan.svelte";

export let onValidate: (e: any, configuration: ExtensionConfiguration) => void
export let onBack: (e: any) => void
export let savedConfiguration: ExtensionConfiguration
export let webApiKey: string 
export let steamProfileId: string
export let steamAppId: string
export let locale: string

let isLoading: boolean = true
let errors: ValidationError[] = []
let isValid: boolean = true


function onSave(e: any)
{
    let configuration: ExtensionConfiguration = {
        activeConfig: ActiveConfig.Steam,
        version: EBSVersion,
        xBoxLiveConfig: savedConfiguration?.xBoxLiveConfig, // Make sure we keep the non-active config saved
        steamConfig: {
            webApiKey: webApiKey,
            steamId: steamProfileId,
            appId: steamAppId,
            locale: locale,
        }
    };
    onValidate(e, configuration);
}

const configuration: ExtensionConfiguration = {
    activeConfig: ActiveConfig.Steam,
    steamConfig: {
        steamId: steamProfileId,
        webApiKey: webApiKey,
        appId: steamAppId,
        locale: locale
    },
    version: EBSVersion,
    xBoxLiveConfig: null
}

ConfigurationService.validateConfiguration(configuration)
    .then(newErrors => {
        errors = newErrors.filter(e => e.path == "SteamConfig.WebApiKey" || e.path == "SteamConfig.SteamId")
        isLoading = false
        isValid = errors.length == 0
    })

let isSaveDisabled: boolean
$: isSaveDisabled = isLoading || errors.length > 0
</script>

{#if isLoading}
<div class="card">
    <div class="spinner"></div>
</div>
{:else}
<div class="card">
    <div class="section">
        ActiveConfig: Steam
    </div>
    <div class="section">
        WebApiKey: <SecretKeySpan keyValue={webApiKey} />
    </div>
    <div class="section">
        StreamerSteamId: {steamProfileId}
    </div>
    <div class="section">
        AppId: {steamAppId}
    </div>
    <div class="section">
        WebApiKey: {locale}
    </div>
</div>
{/if}
<ValidationErrorList errors={errors} />
<input type="button" value="Back" on:click={onBack} />
<input type="button" value="Save" on:click={onSave} disabled={isSaveDisabled} />
