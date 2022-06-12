<script lang="ts">
import { EBSVersion } from "../../common/ServerConfig"
import { ActiveConfig, ExtensionConfiguration, TitleInfo } from "../../common/EBSTypes"
import { ConfigurationService, ValidationError } from "../../services/EBSConfigurationService"
import ValidationErrorList from "../ValidationErrorList/ValidationErrorList.svelte";
import GameCard from "../GameCard/GameCard.svelte"

export let onValidate: (e: any, configuration: ExtensionConfiguration) => void
export let onBack: (e: any) => void
export let savedConfiguration: ExtensionConfiguration
export let webApiKey: string
export let steamProfileId: string

let titleSearch: string = ''
let isLoading: boolean = true
let isConfirmed: boolean = false
let ownedApps: TitleInfo[] = []
let filteredApps: TitleInfo[] = []
let selectedTitle: TitleInfo = null
let errors: ValidationError[] = []

async function onContinue()
{
    isConfirmed = true
}

async function onChangeTitleSearch(value: string)
{
    titleSearch = value.toLowerCase()
    filteredApps = ownedApps.filter(app => app.productTitle.toLowerCase().search(titleSearch) != -1)
}

async function onSelectTitle(titleId: string)
{
    let titleInfo = ownedApps.find(t => t.titleId == titleId);

    isLoading = true
    titleSearch = ""
    filteredApps = ownedApps
    selectedTitle = titleInfo

    // Validate and move on
    const configuration: ExtensionConfiguration = {
        activeConfig: ActiveConfig.Steam,
        steamConfig: {
            steamId: steamProfileId,
            webApiKey: webApiKey,
            appId: titleInfo.titleId,
            locale: "english",
        },
        version: EBSVersion,
        xBoxLiveConfig: null
    }

    let newErrors = await ConfigurationService.validateConfiguration(configuration);
    errors = newErrors.filter(e => e.path == "SteamConfig.WebApiKey" || e.path == "SteamConfig.SteamId");
    isLoading = false
    selectedTitle = errors.length == 0 ? titleInfo : null
}

function onResetTitle()
{
    titleSearch = "",
    filteredApps = ownedApps
    selectedTitle = null
}

function unvalidate()
{
    isConfirmed = false
}


ConfigurationService.getSteamOwnedGames(steamProfileId, webApiKey)
    .then(gameList => {
        let gameInfo: TitleInfo = null;
        if (selectedTitle)
        {
            gameInfo = gameList.find(g => g.titleId == selectedTitle.titleId);
        }
        
        isLoading = false
        filteredApps = gameList
        ownedApps = gameList
        selectedTitle = gameInfo
    });
let isContinueEnabled: boolean
$: isContinueEnabled = selectedTitle != null
</script>

{#if isConfirmed}
<!-- <ConfigSteam_04_Locale
    savedConfiguration={savedConfiguration}
    onValidate={onValidate}
    onBack={unvalidate}
    webApiKey={webApiKey}
    steamProfileId={steamProfileId}
    steamAppId={selectedTitle.titleId} /> -->
{:else}
    <ValidationErrorList errors={errors} />
    {#if isLoading}
        <div class="card">
            <div class="spinner"></div>
        </div>
    {:else if selectedTitle}
        <GameCard titleInfo={selectedTitle}>
            <input slot="buttonSection" type="button" class="section" name="TitleChange" value="Change" on:click={onResetTitle} />
        </GameCard>
    {:else}
        <label for="titleSearch">Owned Game list</label>,
        <input name="titleSearch" type="text" placeholder="Filter your Steam games" on:change={e => onChangeTitleSearch(e.currentTarget.value)} />,,
        <div class="searchResult container">
            <div class="row">
                {#each filteredApps as titleInfo}
                    <GameCard titleInfo={titleInfo}>
                        <input slot="buttonSection" class="section" type="button" name="steamTitleChange" value="Select" on:click={_e => onSelectTitle(titleInfo.titleId)} />
                    </GameCard>
                {/each}
            </div>
        </div>
    {/if}
    <input type="button" value="Back" on:click={onBack} />
    <input type="button" value="Continue" disabled={!isContinueEnabled} on:click={onContinue} />
{/if}