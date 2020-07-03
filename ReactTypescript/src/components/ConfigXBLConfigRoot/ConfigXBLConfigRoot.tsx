import * as React from 'react';
import * as Base from '../../common/ConfigStepBase';
import ConfigXBL_01_XApiKey from '../ConfigXBL_01_XApiKey/ConfigXBL_01_XApiKey';
import ConfigXBL_02_TitleId from '../ConfigXBL_02_TitleId/ConfigXBL_02_TitleId';
import ConfigXBL_03_XUID from '../ConfigXBL_03_XUID/ConfigXBL_03_XUID';
import ConfigXBL_04_Confirm from '../ConfigXBL_04_Confirm/ConfigXBL_04_Confirm';

enum ConfigXBLConfigStateEnum {
    XApiUsKey = 0,
    TitleIdSearch = 1,
    XUIDSearch = 2,
    Confirm = 3,
}

type ConfigXBLConfigState = {
    currentState: ConfigXBLConfigStateEnum
}

export default class ConfigXBLConfigRoot extends React.Component<any, ConfigXBLConfigState> {
    state: ConfigXBLConfigState = {
        currentState: ConfigXBLConfigStateEnum.XApiUsKey
    };
    
    constructor(props:any){
        super(props);

        this.onValidateStep = this.onValidateStep.bind(this);
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
            throw "NotImplemented";
        }
    }

    render(){
        let config = <div></div>

        switch (this.state.currentState) {
            case ConfigXBLConfigStateEnum.XApiUsKey:
                config = <ConfigXBL_01_XApiKey onValid={this.onValidateStep} nextState={ConfigXBLConfigStateEnum.TitleIdSearch} />
                break;
            case ConfigXBLConfigStateEnum.TitleIdSearch:
                config = <ConfigXBL_02_TitleId onValid={this.onValidateStep} nextState={ConfigXBLConfigStateEnum.XUIDSearch} />
                break;
            case ConfigXBLConfigStateEnum.XUIDSearch:
                config = <ConfigXBL_03_XUID onValid={this.onValidateStep} nextState={ConfigXBLConfigStateEnum.Confirm} />
                break;
            case ConfigXBLConfigStateEnum.Confirm:
                config = <ConfigXBL_04_Confirm onValid={this.onValidateStep} nextState={null} />
                break;
            default:
                break;
        }

        return [
            <h2>xapi.us Key</h2>,
            config,
        ]
    }
}