<script lang="ts">
import { ConfigurationService, ValidationError } from "../../services/EBSConfigurationService"
import { ActiveConfig, ExtensionConfiguration, TitleInfo } from "../../common/EBSTypes"
import { AchievementsService } from "../../services/EBSAchievementsService"
import { EBSVersion } from "../../common/ServerConfig"
import ValidationErrorList from "../ValidationErrorList/ValidationErrorList.svelte"
import GameCard from "../GameCard/GameCard.svelte"
import ConfigXBL_04_Locale from "../ConfigXBL_04_Locale/ConfigXBL_04_Locale.svelte"

export let onValidate: (e: any, configuration: ExtensionConfiguration) => void
export let onBack: (e: any) => void
export let savedConfiguration: ExtensionConfiguration
export let xApiKey: string
export let streamerXuid: string

let titleSearch: string = ""
let searchResults: TitleInfo[] = []
let selectedTitle: TitleInfo = null
let isLoading: boolean = true
let isConfirmed: boolean = false
let errors: ValidationError[] = []


function fetchRecentTitles(xuid: string, xApiKey: string)
{
    searchResults = []
    selectedTitle = null
    isLoading = true
    
    ConfigurationService.getRecentXBoxLiveTitleInfo(xuid, xApiKey)
    .then(titleList => {
        searchResults = titleList
        selectedTitle = null
        isLoading = false
    })
    .catch( () => {
        isLoading = false
    });
}

async function onContinue()
{
    isConfirmed = true
}

async function onChangeTitleSearch(value: string)
{
    titleSearch = value
    searchResults = searchResults
}

async function onSearch()
{
    searchResults = []
    selectedTitle = null
    isLoading = true
    errors = []
    
    let titleInfos = await AchievementsService.searchXApiTitleInfo(titleSearch, xApiKey)

    searchResults = titleInfos
    selectedTitle = selectedTitle
    isLoading = false
}

async function onSelectTitle(_e: any, titleId: string)
{
    let titleInfo = searchResults.find(t => t.titleId == titleId);

    searchResults = searchResults
    selectedTitle = titleInfo
    isLoading = true
    errors = []

    // Validate and move on
    await validateTitle(titleInfo);
}

async function validateTitle(titleInfo: TitleInfo)
{
    isLoading = true
    errors = []

    const configuration: ExtensionConfiguration = {
        activeConfig: ActiveConfig.XBoxLive,
        steamConfig: null,
        version: EBSVersion,
        xBoxLiveConfig: {
            xApiKey: xApiKey,
            locale: savedConfiguration?.xBoxLiveConfig?.locale ?? "en",
            streamerXuid: streamerXuid,
            titleId: titleInfo.titleId,
        }
    }

    let newErrors: ValidationError[] = [];
    try
    {
        newErrors = await ConfigurationService.validateTitle(configuration);
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
    isLoading = false
    selectedTitle = titleInfo
}

function onResetTitle()
{
    selectedTitle = null
    fetchRecentTitles(streamerXuid, xApiKey);
}

function unvalidate()
{
    isConfirmed = false
}


let currentConfig = savedConfiguration;
if (currentConfig?.xBoxLiveConfig?.titleId){
    ConfigurationService.resolveXBoxLiveTitleInfo(currentConfig.xBoxLiveConfig.titleId, xApiKey)
    .then(titleInfo => {
        validateTitle(titleInfo);
    })
    .catch(() => {
        selectedTitle = null
        isLoading = false
    })
}
else
{
    fetchRecentTitles(streamerXuid, xApiKey);
}
let isContinueEnabled: boolean
$: isContinueEnabled = selectedTitle && errors?.length == 0;
</script>

{#if isConfirmed}
    <ConfigXBL_04_Locale
        savedConfiguration={savedConfiguration}
        onValidate={onValidate}
        onBack={unvalidate}
        xApiKey={xApiKey}
        streamerXuid={streamerXuid}
        titleId={selectedTitle.titleId} />
{:else}
    {#if isLoading}
        <div class="spinner"></div>
    {:else if selectedTitle != null}
        <ValidationErrorList errors={errors} />
        <GameCard titleInfo={selectedTitle}>
            <input slot="buttonSection" class="section" type="button" name="xBoxTitleChange" value="Change" on:click={onResetTitle} />
        </GameCard>
    {:else}
        <label for="titleSearch">Search Game</label>
        <input name="titleSearch" type="text" placeholder="Search a game title" on:change={e => onChangeTitleSearch(e.currentTarget.value)} />
        <input type="button" value="Search" on:click={onSearch} />
        <div class="searchResult container">
            <div class="row">
            {#each searchResults as titleInfo}
                <GameCard titleInfo={titleInfo}>
                    <input slot="buttonSection" class="section" type="button" name="xBoxTitleChange" value="Select" on:click={(e) => onSelectTitle(e, titleInfo.titleId)} />
                </GameCard>
            {/each}
            </div>
        </div>
    {/if}
    <input type="button" value="Back" on:click={onBack} />
    <input type="button" value="Continue" disabled={!isContinueEnabled} on:click={onContinue} />
{/if}