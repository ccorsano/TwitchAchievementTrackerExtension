import * as React from 'react';
import * as Base from '../../common/ConfigStepBase'
import ConfigSteam_01_WebAPIKey from '../ConfigSteam_01_WebAPIKey/ConfigSteam_01_WebAPIKey'
import ConfigSteam_02_AppId from '../ConfigSteam_02_AppId/ConfigSteam_02_AppId';
import ConfigSteam_03_SteamID from '../ConfigSteam_03_SteamID/ConfigSteam_03_SteamID';
import ConfigSteam_04_Confirm from '../ConfigSteam_04_Confirm/ConfigSteam_04_Confirm';

enum ConfigSteamConfigStateEnum {
    WebApiKey = 0,
    SteamGameSearch = 1,
    SteamProfileSetup = 2,
    Confirm = 3,
}

type ConfigSteamConfigState = {
    currentState: ConfigSteamConfigStateEnum
}

export default class ConfigSteamConfigRoot extends React.Component<any, ConfigSteamConfigState> {
    state: ConfigSteamConfigState = {
        currentState: ConfigSteamConfigStateEnum.WebApiKey
    }

    constructor(props:any){
        super(props);

        this.onValidateStep = this.onValidateStep.bind(this);
    }

    onValidateStep = (e: React.Component, nextState: any) => {

    }

    render(){
        var config = <div></div>;
        
        switch (this.state.currentState) {
            case ConfigSteamConfigStateEnum.WebApiKey: {
                config = <ConfigSteam_01_WebAPIKey onValid={this.onValidateStep} nextState={ConfigSteamConfigStateEnum.SteamGameSearch} />
                break;
            }
            case ConfigSteamConfigStateEnum.SteamGameSearch: {
                config = <ConfigSteam_02_AppId onValid={this.onValidateStep} nextState={ConfigSteamConfigStateEnum.SteamProfileSetup} />
                break;
            }
            case ConfigSteamConfigStateEnum.SteamProfileSetup: {
                config = <ConfigSteam_03_SteamID onValid={this.onValidateStep} nextState={ConfigSteamConfigStateEnum.Confirm} />
                break;
            }
            case ConfigSteamConfigStateEnum.Confirm: {
                config = <ConfigSteam_04_Confirm onValid={this.onValidateStep} nextState={null} />
                break;
            }
            default:
                break;
        }

        return [
            <h2>Steam</h2>,
            config,
        ]
    }
}