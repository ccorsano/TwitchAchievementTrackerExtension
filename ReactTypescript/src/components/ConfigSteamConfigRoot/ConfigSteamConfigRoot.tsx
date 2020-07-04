import * as React from 'react';
import * as Base from '../../common/ConfigStepBase'
import ConfigSteam_01_WebAPIKey from '../ConfigSteam_01_WebAPIKey/ConfigSteam_01_WebAPIKey'
import ConfigSteam_02_SteamID from '../ConfigSteam_02_SteamID/ConfigSteam_02_SteamID';
import ConfigSteam_03_AppId from '../ConfigSteam_03_AppId/ConfigSteam_03_AppId';
import ConfigSteam_04_Locale from '../ConfigSteam_04_Locale/ConfigSteam_04_Locale';
import ConfigSteam_05_Confirm from '../ConfigSteam_05_Confirm/ConfigSteam_05_Confirm'
import { ConfigurationState } from '../../services/ConfigurationStateService';
import { ActiveConfig } from '../../common/EBSTypes';
import { ConfigurationService } from '../../services/EBSConfigurationService';
import { Twitch } from '../../services/TwitchService';
import * as ServerConfig from '../../common/ServerConfig';

enum ConfigSteamConfigStateEnum {
    WebApiKey = 0,
    SteamGameSearch = 1,
    SteamProfileSetup = 2,
    Locale = 3,
    Confirm = 4,
}

type ConfigSteamConfigState = {
    currentState: ConfigSteamConfigStateEnum,
    isValid: boolean,
}

export default class ConfigSteamConfigRoot extends React.Component<any, ConfigSteamConfigState> {
    state: ConfigSteamConfigState = {
        currentState: ConfigSteamConfigStateEnum.WebApiKey,
        isValid: false,
    }

    constructor(props:any){
        super(props);

        this.onValidateStep = this.onValidateStep.bind(this);
    }

    componentDidMount = () => {
        let currentConfig = ConfigurationState.currentConfiguration;
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

    onValidateStep = (e: React.Component, nextState: any) => {
        if (nextState)
        {
            this.setState({
                currentState: nextState
            });
        }
        else
        {
            ConfigurationService.setConfiguration(ConfigurationState.currentConfiguration).then(result => {
                Twitch.setConfiguration(result.configToken, ServerConfig.EBSVersion);

                this.setState({
                    currentState: this.state.currentState,
                    isValid: true,
                });
            });
        }
    }

    render(){
        var config = <div></div>;
        
        switch (this.state.currentState) {
            case ConfigSteamConfigStateEnum.WebApiKey: {
                config = <ConfigSteam_01_WebAPIKey onValid={this.onValidateStep} nextState={ConfigSteamConfigStateEnum.SteamProfileSetup} />
                break;
            }
            case ConfigSteamConfigStateEnum.SteamProfileSetup: {
                config = <ConfigSteam_02_SteamID onValid={this.onValidateStep} nextState={ConfigSteamConfigStateEnum.SteamGameSearch} />
                break;
            }
            case ConfigSteamConfigStateEnum.SteamGameSearch: {
                config = <ConfigSteam_03_AppId onValid={this.onValidateStep} nextState={ConfigSteamConfigStateEnum.Locale} />
                break;
            }
            case ConfigSteamConfigStateEnum.Locale: {
                config = <ConfigSteam_04_Locale onValid={this.onValidateStep} nextState={ConfigSteamConfigStateEnum.Confirm} />
                break;
            }
            case ConfigSteamConfigStateEnum.Confirm: {
                config = <ConfigSteam_05_Confirm onValid={this.onValidateStep} nextState={null} />
                break;
            }
            default:
                break;
        }

        if (this.state.isValid){
            config = (
                <div>Saved !</div>
            )
        }

        return [
            <h2>Steam</h2>,
            config,
        ]
    }
}