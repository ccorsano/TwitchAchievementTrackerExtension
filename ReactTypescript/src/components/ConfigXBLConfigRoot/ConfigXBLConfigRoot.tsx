import * as React from 'react';
import * as Base from '../../common/ConfigStepBase';
import { ConfigurationService, ValidationError, EncryptedConfigurationResponse } from '../../services/EBSConfigurationService';
import { ConfigurationState } from '../../services/ConfigurationStateService'
import ConfigXBL_01_XApiKey from '../ConfigXBL_01_XApiKey/ConfigXBL_01_XApiKey';
import ConfigXBL_02_TitleId from '../ConfigXBL_02_TitleId/ConfigXBL_02_TitleId';
import ConfigXBL_03_XUID from '../ConfigXBL_03_XUID/ConfigXBL_03_XUID';
import ConfigXBL_04_Locale from '../ConfigXBL_04_Locale/ConfigXBL_04_Locale';
import ConfigXBL_05_Confirm from '../ConfigXBL_05_Confirm/ConfigXBL_05_Confirm';
import { ActiveConfig } from '../../common/EBSTypes';
import { Twitch } from '../../services/TwitchService';
import * as ServerConfig from '../../common/ServerConfig'

enum ConfigXBLConfigStateEnum {
    XApiUsKey = 0,
    TitleIdSearch = 1,
    XUIDSearch = 2,
    Locale = 3,
    Confirm = 4,
}

type ConfigXBLConfigState = {
    currentState: ConfigXBLConfigStateEnum,
    isValid: boolean
}

export default class ConfigXBLConfigRoot extends React.Component<any, ConfigXBLConfigState> {
    state: ConfigXBLConfigState = {
        currentState: ConfigXBLConfigStateEnum.XApiUsKey,
        isValid: false,
    };
    
    constructor(props:any){
        super(props);

        this.onValidateStep = this.onValidateStep.bind(this);
    }

    // Using componentWillMoubt: componentDidMount will be executed on child elements first, and we need to setup the current config first
    componentWillMount = () => {
        let currentConfig = ConfigurationState.currentConfiguration;
        currentConfig.activeConfig = ActiveConfig.XBoxLive;
        if (! currentConfig.xBoxLiveConfig)
        {
            currentConfig.xBoxLiveConfig = {
                xApiKey: null,
                streamerXuid: null,
                locale: null,
                titleId: null,
            }
        }
    }

    onValidateStep = async (e: React.Component, nextState: any) => {
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
        let config = <div>Loading...</div>

        switch (this.state.currentState) {
            case ConfigXBLConfigStateEnum.XApiUsKey:
                config = <ConfigXBL_01_XApiKey onValid={this.onValidateStep} nextState={ConfigXBLConfigStateEnum.TitleIdSearch} />
                break;
            case ConfigXBLConfigStateEnum.TitleIdSearch:
                config = <ConfigXBL_02_TitleId onValid={this.onValidateStep} nextState={ConfigXBLConfigStateEnum.XUIDSearch} />
                break;
            case ConfigXBLConfigStateEnum.XUIDSearch:
                config = <ConfigXBL_03_XUID onValid={this.onValidateStep} nextState={ConfigXBLConfigStateEnum.Locale} />
                break;
            case ConfigXBLConfigStateEnum.Locale:
                config = <ConfigXBL_04_Locale onValid={this.onValidateStep} nextState={ConfigXBLConfigStateEnum.Confirm} />
                break;
            case ConfigXBLConfigStateEnum.Confirm:
                config = <ConfigXBL_05_Confirm onValid={this.onValidateStep} nextState={null} />
                break;
            default:
                break;
        }

        if (this.state.isValid){
            config = (
                <div>Saved !</div>
            )
        }

        return [
            <h2>Configure XBoxLive Achievements</h2>,
            config,
        ]
    }
}