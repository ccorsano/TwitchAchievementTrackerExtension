export enum ActiveConfig {
    None = "",
    XBoxLive = 'XBoxLive',
    Steam = 'Steam',
}

export interface XApiConfiguration {
    xApiKey: string;
    streamerXuid: string;
    titleId: string;
    locale: string;
}

export interface SteamConfiguration {
    webApiKey: string;
    steamId: string;
    appId: string;
    locale: string;
}

export interface ExtensionConfiguration {
    version: string;
    activeConfig: ActiveConfig;
    xBoxLiveConfig: XApiConfiguration;
    steamConfig: SteamConfiguration;
}

export interface SupportedLanguage {
    langCode: string;
    displayName: string;
}

export interface PlayerInfoCard {
    playerId: string;
    playerName: string;
    avatarUrl: string;
}

export interface TitleInfo {
    titleId: string;
    productTitle: string;
    productDescription: string;
    logoUri: string;
}