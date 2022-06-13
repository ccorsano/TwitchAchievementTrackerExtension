import * as EBSConfig from "../common/ServerConfig"
import * as EBS from './EBSBase'
import type { ExtensionConfiguration, SupportedLanguage, PlayerInfoCard, TitleInfo, RateLimits } from '../common/EBSTypes';
import type { TwitchExtensionConfiguration } from "../common/TwitchExtension";

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
        this.onConfigured = () => {
            this.getConfiguration()
                .then(config=> {
                    if (config)
                    {
                        console.log("Existing configuration loaded");
                    }
                    else
                    {
                        console.log("No existing configuration");
                    }
                });
        };
    }

    setConfiguration = async (config: ExtensionConfiguration): Promise<EncryptedConfigurationResponse> => {
        return this.serviceFetch("/", {
            method: 'POST',
            body: JSON.stringify(config)
        });
    }

    fetchConfiguration = async (configuration: TwitchExtensionConfiguration): Promise<ExtensionConfiguration> => {
        return this.serviceFetch("/", null, configuration.content, configuration.version);
    }

    getConfiguration = async (): Promise<ExtensionConfiguration> => {
        if (! this.configuration?.content)
        {
            return Promise.resolve(null);
        }
        return this.serviceFetch("/");
    }

    validateTitle = async (config: ExtensionConfiguration): Promise<ValidationError[]> => {
        return this.serviceFetch("/title/validate", {
            method: 'POST',
            body: JSON.stringify(config)
        });
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

    getSteamOwnedGames = async (steamId: string, webApiKey: string): Promise<TitleInfo[]> => {
        return this.serviceFetch("/steam/" + encodeURIComponent(steamId) +  "/ownedGames?webApiKey=" + encodeURIComponent(webApiKey) );
    }

    getSteamSupportedLanguages = async (gameId: string, webApiKey: string): Promise<SupportedLanguage[]> => {
        return this.serviceFetch("/steam/languages/" + encodeURIComponent(gameId) + "?webApiKey=" + encodeURIComponent(webApiKey) );
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

    getXBoxLiveSupportedLanguages = async (titleId: string, xapiKey: string): Promise<SupportedLanguage[]> => {
        return this.serviceFetch("/xapi/languages/" + encodeURIComponent(titleId) + "?xApiKey=" + encodeURIComponent(xapiKey));
    }

    forceRefresh = async (): Promise<boolean> => {
        return this.serviceFetch("/liveconfig/forcerefresh");
    }

    getXApiRateLimits = async (): Promise<RateLimits> => {
        return this.serviceFetch("/liveconfig/ratelimits")
    }
}

export const ConfigurationService = new EBSConfigurationService();
