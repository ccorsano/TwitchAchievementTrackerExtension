<script lang="ts">
import { ConfigurationService } from "../../services/EBSConfigurationService"
import type { ExtensionConfiguration, SupportedLanguage } from "../../common/EBSTypes"
import ConfigSteam_05_Confirm from "../ConfigSteam_05_Confirm/ConfigSteam_05_Confirm.svelte"

export let onValidate: (e: any, configuration: ExtensionConfiguration) => void
export let onBack: (e: any) => void
export let savedConfiguration: ExtensionConfiguration
export let webApiKey: string
export let steamProfileId: string
export let steamAppId: string

let isLoading: boolean =  true
let supportedLanguages: SupportedLanguage[] =  [ { langCode: '', displayName: "Loading..." } ]
let selectedLanguage: SupportedLanguage =  null
let isConfirmed: boolean =  false

function onSelect(value: string)
{
    selectedLanguage = supportedLanguages.find(l => l.langCode == value)
}

function onResetLanguage(_e: any)
{
    selectedLanguage = null
}

function onContinue(_e: any)
{
    isConfirmed = true
}

function unvalidate(_e: any)
{
    isConfirmed = false
}

let currentLanguage: string
$: currentLanguage = savedConfiguration?.steamConfig?.locale

ConfigurationService.getSteamSupportedLanguages(steamAppId, webApiKey)
    .then(languages => {
        isLoading = false
        supportedLanguages = [{ langCode: '', displayName: "Select language ..."}].concat(languages)
        selectedLanguage = currentLanguage ? languages.find(l => l.langCode == currentLanguage) : null
    });

let isContinueEnabled
$: isContinueEnabled = selectedLanguage != null;
</script>


{#if isConfirmed}
<ConfigSteam_05_Confirm
    savedConfiguration={savedConfiguration}
    onValidate={onValidate}
    onBack={unvalidate}
    webApiKey={webApiKey}
    steamProfileId={steamProfileId}
    steamAppId={steamAppId}
    locale={selectedLanguage.langCode} />
{:else}
    {#if isLoading}
        <div class="card">
            <div class="spinner"></div>
        </div>
    {:else if selectedLanguage != null}
        <div class="card">
            <h2>{selectedLanguage.displayName}</h2>
            <input type="button" class="section" name="LanguageChange" value="Change" onClick={onResetLanguage} />
        </div>
    {:else if supportedLanguages != null}
    <div class="card">
        <select name="language" class="selectLanguage" on:change={e => onSelect(e.currentTarget.value)}>
            {#each supportedLanguages as language}
                <option value={language.langCode}>{language.displayName}</option>
            {/each}
        </select>
    </div>
    {/if}
    <input type="button" value="Back" on:click={onBack} />
    <input type="button" disabled={!isContinueEnabled} value="Continue" on:click={onContinue} />
{/if}