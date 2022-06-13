<script lang="ts">
import { ConfigurationService } from "../../services/EBSConfigurationService"
import type { ExtensionConfiguration, SupportedLanguage } from "../../common/EBSTypes"
import ConfigXBL_05_Confirm from "../ConfigXBL_05_Confirm/ConfigXBL_05_Confirm.svelte"

export let onValidate: (e: any, configuration: ExtensionConfiguration) => void
export let onBack: (e: any) => void
export let savedConfiguration: ExtensionConfiguration
export let xApiKey: string
export let streamerXuid: string
export let titleId: string

let supportedLanguages: SupportedLanguage[] = [ { langCode: '', displayName: "Loading..." } ]
let selectedLanguage: SupportedLanguage = null
let isLoading: boolean = true
let isConfirmed: boolean = false

function onSelect(value: string)
{
    supportedLanguages = supportedLanguages
    selectedLanguage = supportedLanguages.find(l => l.langCode == value)
}

function onResetLanguage()
{
    selectedLanguage = null
}

function onContinue()
{
    isConfirmed = true
}

function unvalidate()
{
    isConfirmed = false
}

const currentLocale = savedConfiguration?.xBoxLiveConfig?.locale;

ConfigurationService.getXBoxLiveSupportedLanguages(titleId, xApiKey)
    .then(languages => {
        supportedLanguages = [{ langCode: '', displayName: "Select language ..."}].concat(languages)
        selectedLanguage = currentLocale ? languages.find(l => l.langCode == currentLocale) : null
        isLoading = false
    });

let isContinueEnabled: boolean
$: isContinueEnabled = selectedLanguage != null
</script>

{#if isConfirmed}
    <ConfigXBL_05_Confirm
        savedConfiguration={savedConfiguration}
        onValidate={onValidate}
        onBack={unvalidate}
        xApiKey={xApiKey}
        streamerXuid={streamerXuid}
        titleId={titleId}
        locale={selectedLanguage.langCode} />
{:else}
    {#if isLoading}
        <div class="card">
            <div class="spinner"></div>
        </div>
    {:else if selectedLanguage != null}
        <div class="card">
            <h2>{selectedLanguage.displayName}</h2>
            <input type="button" class="section" name="LanguageChange" value="Change" on:click={onResetLanguage} />
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