import * as EBSConfig from "../common/ServerConfig"
import * as EBS from './EBSBase'
import { ExtensionConfiguration, ActiveConfig, SupportedLanguage, PlayerInfoCard, TitleInfo } from '../common/EBSTypes';
import { ConfigurationState } from "./ConfigurationStateService";

export interface EncryptedConfigurationResponse {
    configToken: string;
}

export interface ValidationError {
    path: string;
    errorCode: string;
    errorDescription: string;
}

export interface SteamLanguage {
    englishName: string;
    code: string;
    codeName: string;
}   

export default class EBSConfigurationService extends EBS.EBSBase {
    constructor(){
        super(EBSConfig.EBSBaseUrl + "/api/configuration");
        this.onConfigured = (auth: TwitchAuthCallbackContext, config: TwitchExtensionConfiguration) => {
            this.getConfiguration()
                .then(config=> {
                    console.log("Existing configuration loaded");
                    ConfigurationState.currentConfiguration = config;
                });
        };
    }

    setConfiguration = async (config: ExtensionConfiguration): Promise<EncryptedConfigurationResponse> => {
        return this.serviceFetch("/", {
            method: 'POST',
            body: JSON.stringify(config)
        });
    }

    getConfiguration = async (): Promise<ExtensionConfiguration> => {
        return this.serviceFetch("/");
    }

    getSupportedLanguages = async (config: ActiveConfig, gameId: string): Promise<SupportedLanguage[]> => {
        return this.serviceFetch("/languages?activeConfig=" + config + "&gameId=" + gameId);
    }

    validateConfiguration = async (config: ExtensionConfiguration): Promise<ValidationError[]> => {
        return this.serviceFetch("/validate", {
            method: 'POST',
            body: JSON.stringify(config)
        });
    }

    resolveSteamVanityUrl = async (vanityUrl: string, webApiKey: string): Promise<PlayerInfoCard> => {
        return this.serviceFetch("/steam/resolveVanity/" + encodeURIComponent(vanityUrl) + "?webApiKey=" + encodeURIComponent(webApiKey) );
    }

    resolveSteamPlayerInfo = async (steamId: string, webApiKey: string): Promise<PlayerInfoCard> => {
        return this.serviceFetch("/steam/playerInfo/" + encodeURIComponent(steamId) + "?webApiKey=" + encodeURIComponent(webApiKey) );
    }

    resolveXBoxLiveGamertag = async (gamertag: string, xapiKey: string): Promise<PlayerInfoCard> => {
        return this.serviceFetch("/xapi/gamertag/" + encodeURIComponent(gamertag) + "?xApiKey=" + encodeURIComponent(xapiKey) );
    }

    resolveXBoxLivePlayerInfo = async (xuid: string, xapiKey: string): Promise<PlayerInfoCard> => {
        return this.serviceFetch("/xapi/playerInfo/" + encodeURIComponent(xuid) + "?xApiKey=" + encodeURIComponent(xapiKey) );
    }

    resolveXBoxLiveTitleInfo = async (titleId: string, xapiKey: string): Promise<TitleInfo> => {
        return this.serviceFetch("/xapi/titleInfo/" + encodeURIComponent(titleId) + "?xApiKey=" + encodeURIComponent(xapiKey) );
    }

    getRecentXBoxLiveTitleInfo = async (xuid: string, xapiKey: string): Promise<TitleInfo[]> => {
        return this.serviceFetch("/xapi/recentTitles/" + encodeURIComponent(xuid) + "?xApiKey=" + encodeURIComponent(xapiKey) );
    }

    getSteamOwnedGames = async (steamId: string, webApiKey: string): Promise<TitleInfo[]> => {
        return this.serviceFetch("/steam/" + encodeURIComponent(steamId) +  "/ownedGames?webApiKey=" + encodeURIComponent(webApiKey) );
    }
}

export const ConfigurationService = new EBSConfigurationService();