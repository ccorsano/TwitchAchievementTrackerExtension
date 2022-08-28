<script lang="ts">
import { ConfigurationService } from "../../services/EBSConfigurationService";
import type { ExtensionConfiguration, PlayerInfoCard } from "../../common/EBSTypes";
import GamerCard from "../GamerCard/GamerCard.svelte"
import ConfigXBL_03_TitleId from "../ConfigXBL_03_TitleId/ConfigXBL_03_TitleId.svelte"

export let onValidate: (e: any, configuration: ExtensionConfiguration) => void
export let onBack: (e: any) => void
export let savedConfiguration: ExtensionConfiguration
export let xApiKey: string

let xuidSearch: string = ""
let xuid: string = null
let gamerCard: PlayerInfoCard = null
let isLoading: boolean = true
let isConfirmed: boolean = false

function changeXuid(newXuid: string)
{
    xuid = null
    isLoading = true

    ConfigurationService.resolveXBoxLivePlayerInfo(newXuid, xApiKey)
        .then(playerInfo => {
            xuid = newXuid
            gamerCard = playerInfo
            isLoading = false
        });
}

function onChangeGamertagSearch(value: string)
{
    xuidSearch = value
}

async function onSearch(_e: any)
{
    xuidSearch = xuidSearch
    xuid = null
    gamerCard = null
    isLoading = true

    let resolvedGamerCard = await ConfigurationService.resolveXBoxLiveGamertag(xuidSearch, xApiKey);

    xuid = resolvedGamerCard.playerId
    gamerCard = resolvedGamerCard
    isLoading = false
}

function onResetProfile(_e: any)
{
    xuid = null
    xuidSearch = ''
    gamerCard = null
}

function onContinue(_e: any)
{
    isConfirmed = true
}

function unvalidate(_e: any)
{
    isConfirmed = false
}

$:{
    let currentConfig = savedConfiguration;
    if (currentConfig?.xBoxLiveConfig?.streamerXuid){
        changeXuid(currentConfig.xBoxLiveConfig.streamerXuid);
    }
    else
    {
        isLoading = false
    }
}

let isContinueEnabled: boolean
$: isContinueEnabled = xuid != null
</script>

{#if isConfirmed}
    <ConfigXBL_03_TitleId
        savedConfiguration={savedConfiguration}
        onValidate={onValidate}
        onBack={unvalidate}
        xApiKey={xApiKey}
        streamerXuid={xuid} />
{:else}
    {#if isLoading}
        <div class="spinner"></div>
    {:else if gamerCard != null }
        <GamerCard playerInfo={gamerCard}>
            <input slot="buttonSection" type="button" value="Change" class="section" on:click={onResetProfile} />
        </GamerCard>
        <input type="button" value="Back" on:click={onBack} />
        <input type="button" value="Continue" disabled={!isContinueEnabled} on:click={onContinue} />
    {:else}
        <label for="xuidSearch">Streamer Id</label>
        <input name="xuidSearch" type="text" placeholder="Search a Gamertag" value={xuidSearch} on:change={e => onChangeGamertagSearch(e.currentTarget.value)} />
        <input type="button" value="Search" on:click={onSearch} />
        <input type="button" value="Back" on:click={onBack} />
    {/if}
{/if}