import * as React from 'react';
import * as Base from '../../common/ConfigStepBase';
import { ConfigurationState } from '../../services/ConfigurationStateService';

export default class ConfigXBL_05_Confirm extends Base.ConfigStepBase<Base.ConfigStepBaseProps, any> {
    constructor(props: Base.ConfigStepBaseProps) {
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
                <li>XApiKey: {configuration.xBoxLiveConfig.xApiKey}</li>
                <li>TitleId: {configuration.xBoxLiveConfig.titleId}</li>
                <li>StreamerXuid: {configuration.xBoxLiveConfig.streamerXuid}</li>
                <li>Locale: {configuration.xBoxLiveConfig.locale}</li>
            </ul>,
            <input type="button" value="Save" onClick={this.onSave} />
        ]
    }
}