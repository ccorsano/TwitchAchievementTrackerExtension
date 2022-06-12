<script lang="ts">
import { ConfigurationService } from "../../services/EBSConfigurationService";
import { ActiveConfig, ExtensionConfiguration, PlayerInfoCard, SupportedLanguage, TitleInfo } from "../../common/EBSTypes";
import XBoxLiveLogo from '../../../assets/XBox_Live_logo.svg';
import SteamLogo from '../../../assets/Steam_icon_logo.svg';

export let extensionConfig: ExtensionConfiguration
export let onConfigure: (e: any) => void

let playerInfo: PlayerInfoCard
let gameInfo: TitleInfo
let locale: SupportedLanguage

async function updateFromProps(configuration: ExtensionConfiguration)
{
    switch (configuration.activeConfig) {
        case ActiveConfig.XBoxLive:
                ConfigurationService.resolveXBoxLivePlayerInfo(configuration.xBoxLiveConfig.streamerXuid, configuration.xBoxLiveConfig.xApiKey)
                    .then(gamerCard => playerInfo = gamerCard)
                
                ConfigurationService.resolveXBoxLiveTitleInfo(configuration.xBoxLiveConfig.titleId, configuration.xBoxLiveConfig.xApiKey)
                    .then(titleInfo => gameInfo = titleInfo)
                
                ConfigurationService.getXBoxLiveSupportedLanguages(configuration.xBoxLiveConfig.titleId, configuration.xBoxLiveConfig.xApiKey)
                    .then(languages => locale = languages.find(l => l.langCode == configuration.xBoxLiveConfig.locale))
            break;
            case ActiveConfig.Steam:
                ConfigurationService.resolveSteamPlayerInfo(configuration.steamConfig.steamId, configuration.steamConfig.webApiKey)
                    .then(gamerCard => playerInfo = gamerCard)
                ConfigurationService.getSteamOwnedGames(configuration.steamConfig.steamId, configuration.steamConfig.webApiKey)
                    .then(titles => gameInfo = titles.find(t => t.titleId == configuration.steamConfig.appId));
                ConfigurationService.getSteamSupportedLanguages(configuration.steamConfig.appId, configuration.steamConfig.webApiKey)
                    .then(languages => locale = languages.find(l => l.langCode == configuration.steamConfig.locale))
            break;
        default:
            break;
    }
}

updateFromProps(extensionConfig)

let platformLogo: string;
let platformPlayerNaming: string;
$:{
    platformLogo = extensionConfig.activeConfig == ActiveConfig.XBoxLive ? XBoxLiveLogo : SteamLogo
    platformPlayerNaming = extensionConfig.activeConfig == ActiveConfig.XBoxLive ? "GamerTag" : "Profile"
}
</script>


{#if gameInfo && playerInfo && locale}
    <div class="card large">
        <img class="section" src={platformLogo} style:object-fit="contain" style:max-height="5em" alt={ActiveConfig.XBoxLive ? "XBoxLive Logo" : "Steam Logo"} />
        <h1 class="section">{gameInfo.productTitle}</h1>
        <h3 class="section">{playerInfo.playerName} <small>{platformPlayerNaming}</small></h3>
        <h4 class="section">{locale.displayName}  <small>Language</small></h4>
        {#if onConfigure}<input type="button" value="Configure" class="section" on:click={onConfigure}>{/if}
    </div>
{:else}
    <div class="card">
        <div class="spinner"></div>
    </div>
{/if}