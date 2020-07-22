import * as React from 'react';
import ConfigSteam_01_WebAPIKey from '../ConfigSteam_01_WebAPIKey/ConfigSteam_01_WebAPIKey'
import { ActiveConfig, ExtensionConfiguration } from '../../common/EBSTypes';
import { ConfigurationService } from '../../services/EBSConfigurationService';
import { Twitch } from '../../services/TwitchService';
import * as ServerConfig from '../../common/ServerConfig';
import { TwitchExtensionConfiguration } from '../../common/TwitchExtension';

type ConfigSteamConfigProps = {
    savedConfiguration: ExtensionConfiguration,
    onSaved: (savedConfig: TwitchExtensionConfiguration, configObject: ExtensionConfiguration) => void;
    onCancel: () => void;
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

    onValidateStep = async (_e: React.Component, config: ExtensionConfiguration) => {
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

    onCancelStep = () => {
        this.props.onCancel();
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
            config
        ]
    }
}