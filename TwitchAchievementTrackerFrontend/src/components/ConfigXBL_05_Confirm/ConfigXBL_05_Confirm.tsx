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
            <div className="card">
                <div className="section">
                    ActiveConfig: XBoxLive
                </div>
                <div className="section">
                    XApiKey: {this.props.xApiKey}
                </div>
                <div className="section">
                    StreamerXuid: {this.props.streamerXuid}
                </div>
                <div className="section">
                    TitleId: {this.props.titleId}
                </div>
                <div className="section">
                    WebApiKey: {this.props.locale}
                </div>
                <div className="section">
                    Locale: {this.props.locale}
                </div>
            </div>,
            <input type="button" value="Back" onClick={this.onBack} />,
            <input type="button" value="Save" onClick={this.onSave} />
        ]
    }
}