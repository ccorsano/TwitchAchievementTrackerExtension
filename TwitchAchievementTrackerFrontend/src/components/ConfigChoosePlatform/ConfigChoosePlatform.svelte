<script lang="ts">
import { ActiveConfig, type ExtensionConfiguration } from "../../common/EBSTypes"
import XBoxLiveLogo from '../../../assets/XBox_Live_logo.svg'
import SteamLogo from '../../../assets/Steam_icon_logo.svg'
import type { TwitchExtensionConfiguration } from "../../common/TwitchExtension"
import { ConfigurationService } from "../../services/EBSConfigurationService"
import ConfigSteamConfigRoot from "../ConfigSteamConfigRoot/ConfigSteamConfigRoot.svelte"
import ConfigSummary from "../ConfigSummary/ConfigSummary.svelte"
import ConfigXBLConfigRoot from "../ConfigXBLConfigRoot/ConfigXBLConfigRoot.svelte"

type onChangeCallback = () => void

enum CurrentPlatformEnum {
    None = 0,
    XBoxLive,
    Steam,
}

export let changeHandler:onChangeCallback

let isLoading:boolean = true
let isConfirmed:boolean = false
let savedConfiguration: ExtensionConfiguration
let currentPlatform : CurrentPlatformEnum = CurrentPlatformEnum.None
let hasSaved: boolean = false
   
function isSelected(v : CurrentPlatformEnum)
{
    return currentPlatform == v;
}

function onSelect(selected: CurrentPlatformEnum)
{
    currentPlatform = selected
    changeHandler();
}

function onBack()
{
    isConfirmed = savedConfiguration != null
    currentPlatform = CurrentPlatformEnum.None
}

function onRestart()
{
    isConfirmed = false
    currentPlatform = CurrentPlatformEnum.None
}

function onSaved(_savedConfiguration: TwitchExtensionConfiguration, configurationObject: ExtensionConfiguration)
{
    isConfirmed = true
    savedConfiguration = configurationObject
    currentPlatform = CurrentPlatformEnum.None
    hasSaved = true
}

ConfigurationService.configurationPromise.then(async configurationToken => {
    if (configurationToken?.content)
    {
        let configuration = await ConfigurationService.fetchConfiguration(configurationToken);
        
        var validation = await ConfigurationService.validateConfiguration(configuration);

        isLoading = false
        isConfirmed = validation.length == 0,
        savedConfiguration = configuration,
        currentPlatform = configuration.activeConfig == ActiveConfig.XBoxLive ? CurrentPlatformEnum.XBoxLive : CurrentPlatformEnum.Steam
    }
    else
    {
        isLoading = false
        isConfirmed = false
        savedConfiguration = {
            version: null,
            activeConfig: null,
            xBoxLiveConfig: null,
            steamConfig: null,
        }
    }
})

</script>

{#if isLoading}
<div class="card">
    <div class="spinner"></div>
</div>
{:else if isConfirmed}
    {#if hasSaved} <mark class="tag tertiary">Saved and pushed to viewers !</mark> {/if}
    <ConfigSummary extensionConfig={savedConfiguration} onConfigure={onRestart} />
{:else}
    {#if currentPlatform === CurrentPlatformEnum.None}
    <div class="ConfigChoosePlatform">
        <h2>Select a platform</h2>
        <div class="row">
            <div class="selectPlatform card xboxlive">
                <h1 class="section">XBox Live</h1>
                <img src={XBoxLiveLogo} class="section media" alt="XBoxLive Logo" style:object-fit="contain" />
                <input type="button" value="Select" class="section" on:click={(e) => onSelect(CurrentPlatformEnum.XBoxLive) } />
            </div>
            <div class="selectPlatform card steam">
                <h1 class="section">Steam</h1>
                <img src={SteamLogo} class="section media" alt="Steam Logo" style:object-fit="contain" />
                <input type="button" value="Select" class="section" on:click={(e) => onSelect(CurrentPlatformEnum.Steam) } />
            </div>
        </div>
        <input type="button" value="Cancel" on:click={onBack} />
    </div>
    {:else if currentPlatform == CurrentPlatformEnum.Steam}
    <div class="ConfigSteam">
        <ConfigSteamConfigRoot onSaved={onSaved} savedConfiguration={savedConfiguration} onCancel={onRestart} />
    </div>
    {:else if currentPlatform == CurrentPlatformEnum.XBoxLive}
    <div class="ConfigXBoxLive">
        <ConfigXBLConfigRoot onSaved={onSaved} savedConfiguration={savedConfiguration} onCancel={onRestart} />
    </div>
    {/if}
{/if}