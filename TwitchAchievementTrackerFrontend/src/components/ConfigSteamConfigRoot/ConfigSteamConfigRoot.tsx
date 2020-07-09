import * as React from 'react';
import * as Base from '../../common/ConfigStepBase'
import { ConfigSteamConfigStateEnum, ConfigXBLConfigStateEnum } from '../../common/ConfigStepBase'
import ConfigSteam_01_WebAPIKey from '../ConfigSteam_01_WebAPIKey/ConfigSteam_01_WebAPIKey'
import ConfigSteam_02_SteamID from '../ConfigSteam_02_SteamID/ConfigSteam_02_SteamID';
import ConfigSteam_03_AppId from '../ConfigSteam_03_AppId/ConfigSteam_03_AppId';
import ConfigSteam_04_Locale from '../ConfigSteam_04_Locale/ConfigSteam_04_Locale';
import ConfigSteam_05_Confirm from '../ConfigSteam_05_Confirm/ConfigSteam_05_Confirm'
import { ActiveConfig, ExtensionConfiguration, SteamConfiguration } from '../../common/EBSTypes';
import { ConfigurationService } from '../../services/EBSConfigurationService';
import { Twitch } from '../../services/TwitchService';
import * as ServerConfig from '../../common/ServerConfig';
import { TwitchExtensionConfiguration } from '../../common/TwitchExtension';

type ConfigSteamConfigProps = {
    savedConfiguration: ExtensionConfiguration,
    onSaved: (savedConfig: TwitchExtensionConfiguration, configObject: ExtensionConfiguration) => void;
}

type ConfigSteamConfigState = {
    isValid: boolean,
}

export default class ConfigSteamConfigRoot extends React.Component<ConfigSteamConfigProps, ConfigSteamConfigState> {
    state: ConfigSteamConfigState = {
        isValid: false,
    }

    constructor(props:ConfigSteamConfigProps){
        super(props);

        this.onValidateStep = this.onValidateStep.bind(this);
        this.onCancelStep = this.onCancelStep.bind(this);
    }

    componentDidMount = () => {
        let currentConfig: ExtensionConfiguration = this.props.savedConfiguration ?? {activeConfig: ActiveConfig.Steam, xBoxLiveConfig: null, steamConfig: null, version: ServerConfig.EBSVersion};
        currentConfig.activeConfig = ActiveConfig.Steam;
        if (! currentConfig.steamConfig)
        {
            currentConfig.steamConfig = {
                webApiKey: null,
                steamId: null,
                locale: null,
                appId: null,
            }
        }
    }

    onValidateStep = async (e: React.Component, config: ExtensionConfiguration) => {
        // Make sure we keep the non-active config saved
        config.xBoxLiveConfig = this.props.savedConfiguration.xBoxLiveConfig;

        let result = await ConfigurationService.setConfiguration(config);
        Twitch.setConfiguration(result.configToken, ServerConfig.EBSVersion);

        Twitch.send("broadcast", "application/json", {
            "type": "set-config",
            "version": ServerConfig.EBSVersion,
            "configToken": result.configToken
        });

        this.setState({
            isValid: true,
        });

        this.props.onSaved({content: result.configToken, version: ServerConfig.EBSVersion}, config);
    }

    onCancelStep = (previousState: any) => {
    }

    render(){
        let config: JSX.Element = null;

        if (this.state.isValid){
            config = (
                <div className="card">
                    <h2 className="section">Saved !</h2>
                </div>
            )
        }
        else
        {
            config = (<ConfigSteam_01_WebAPIKey savedConfiguration={this.props.savedConfiguration} onValidate={this.onValidateStep} onBack={this.onCancelStep} />)
        }

        return [
            <h2>Steam</h2>,
            config,
        ]
    }
}