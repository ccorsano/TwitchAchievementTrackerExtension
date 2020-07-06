import * as React from 'react';
import * as Base from '../../common/ConfigStepBase'
import { ConfigurationState } from '../../services/ConfigurationStateService';

export default class ConfigSteam_04_Confirm extends Base.ConfigStepBase<any, any> {
    constructor(props: any){
        super(props);
    }

    onSave = (e: React.SyntheticEvent<HTMLInputElement>) => {
        this.props.onValid(this, this.props.nextState);
    }

    render(){
        let configuration = ConfigurationState.currentConfiguration;
        return [
            <ul>
                <li>ActiveConfig: {configuration.activeConfig}</li>
                <li>WebApiKey: {configuration.steamConfig.webApiKey}</li>
                <li>AppId: {configuration.steamConfig.appId}</li>
                <li>StreamerSteamId: {configuration.steamConfig.webApiKey}</li>
                <li>Locale: {configuration.steamConfig.locale}</li>
            </ul>,
            <input type="button" value="Save" onClick={this.onSave} />
        ]
    }
}