import * as EBSTypes from '../common/EBSTypes';
import * as ServerConfig from '../common/ServerConfig';
import { Twitch } from './TwitchService';

export default class ConfigurationStateService {
    currentConfiguration: EBSTypes.ExtensionConfiguration = {
        activeConfig: EBSTypes.ActiveConfig.None,
        version: ServerConfig.EBSVersion,
        steamConfig: null,
        xBoxLiveConfig: null,
    };

    constructor(){
    }
}

export const ConfigurationState = new ConfigurationStateService();