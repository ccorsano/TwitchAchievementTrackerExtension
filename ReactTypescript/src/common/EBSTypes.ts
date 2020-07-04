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
    WebApiKey: string;
    SteamId: string;
    AppId: string;
    Locale: string;
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