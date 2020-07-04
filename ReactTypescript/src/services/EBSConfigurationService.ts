import * as EBSConfig from "../common/ServerConfig"
import * as EBS from './EBSBase'
import { ExtensionConfiguration, ActiveConfig, SupportedLanguage } from '../common/EBSTypes';
import { ConfigurationState } from "./ConfigurationStateService";

export interface EncryptedConfigurationResponse {
    configToken: string;
}

export interface ValidationError {
    path: string;
    errorCode: string;
    errorDescription: string;
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
}

export const ConfigurationService = new EBSConfigurationService();