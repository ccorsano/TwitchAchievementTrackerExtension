import * as React from 'react';
import * as Base from '../../common/ConfigStepBase';
import { ActiveConfig, ExtensionConfiguration } from '../../common/EBSTypes';
import { EBSVersion } from '../../common/ServerConfig';


interface ConfigXBL_05_ConfirmProps extends Base.ConfigStepBaseProps {
    xApiKey: string,
    streamerXuid: string,
    titleId: string,
    locale: string,
}

export default class ConfigXBL_05_Confirm extends Base.ConfigStepBase<ConfigXBL_05_ConfirmProps, any> {
    constructor(props: ConfigXBL_05_ConfirmProps) {
        super(props);
    }

    onSave = (e: React.SyntheticEvent<HTMLInputElement>) => {
        let configuration: ExtensionConfiguration = {
            activeConfig: ActiveConfig.XBoxLive,
            version: EBSVersion,
            steamConfig: this.props.savedConfiguration?.steamConfig, // Make sure we keep the non-active config saved
            xBoxLiveConfig: {
                xApiKey: this.props.xApiKey,
                streamerXuid: this.props.streamerXuid,
                titleId: this.props.titleId,
                locale: this.props.locale,
            }
        };
        this.props.onValidate(this, configuration);
    }

    render(){
        return [
            <ul>
                <li>ActiveConfig: {ActiveConfig.XBoxLive}</li>
                <li>XApiKey: {this.props.xApiKey}</li>
                <li>TitleId: {this.props.titleId}</li>
                <li>StreamerXuid: {this.props.streamerXuid}</li>
                <li>Locale: {this.props.locale}</li>
            </ul>,
            <input type="button" value="Back" onClick={this.onBack} />,
            <input type="button" value="Save" onClick={this.onSave} />
        ]
    }
}