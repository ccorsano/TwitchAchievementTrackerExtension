import * as EBSConfig from "../common/ServerConfig"
import * as EBS from './EBSBase'
import { ExtensionConfiguration } from '../common/EBSTypes';

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
    }

    setConfiguration = async(config: ExtensionConfiguration): Promise<EncryptedConfigurationResponse> => {
        return this.serviceFetch("/", {
            method: 'POST',
            body: JSON.stringify(config)
        });
    }

    getConfiguration = async (): Promise<ExtensionConfiguration> => {
        return this.serviceFetch("/");
    }

    validateConfiguration = async (config: ExtensionConfiguration): Promise<ValidationError[]> => {
        return this.serviceFetch("/validate", {
            method: 'POST',
            body: JSON.stringify(config)
        });
    }
}

export const ConfigurationService = new EBSConfigurationService();