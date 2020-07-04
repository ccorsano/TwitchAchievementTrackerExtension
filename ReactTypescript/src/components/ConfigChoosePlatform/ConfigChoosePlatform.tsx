import * as React from 'react';
import './ConfigChoosePlatform.scss';
import { NoEmitOnErrorsPlugin } from 'webpack';
import ConfigXBLConfigRoot from '../ConfigXBLConfigRoot/ConfigXBLConfigRoot';
import ConfigSteamConfigRoot from '../ConfigSteamConfigRoot/ConfigSteamConfigRoot';
import { EBSBase } from '../../services/EBSBase';
import EBSConfigurationService from '../../services/EBSConfigurationService';

type onChangeCallback = (e: React.SyntheticEvent<HTMLElement>) => void;

type ConfigChoosePlatformProps = {
    changeHandler: onChangeCallback,
};

enum CurrentPlatformEnum {
    None = 0,
    XBoxLive,
    Steam
}

type ConfigChoosePlatformState = {
    currentPlatform : CurrentPlatformEnum;
};

export default class ConfigChoosePlatform extends React.Component<ConfigChoosePlatformProps, ConfigChoosePlatformState> {
    state: ConfigChoosePlatformState = {
        currentPlatform: CurrentPlatformEnum.None
    };

    constructor(props : ConfigChoosePlatformProps){
        super(props)

        this.onSelect = this.onSelect.bind(this);
        this.onBack = this.onBack.bind(this);
        this.isSelected = this.isSelected.bind(this);
    }
    
    isSelected = (v : CurrentPlatformEnum) => {
        return this.state.currentPlatform == v;
    }

    onSelect = (e: React.SyntheticEvent<HTMLDivElement>, selected: CurrentPlatformEnum) => {
        this.setState({
            currentPlatform: selected
        });

        this.props.changeHandler(e);
    }

    onBack = (e: React.MouseEvent<HTMLInputElement>) => {
        this.setState({
            currentPlatform: CurrentPlatformEnum.None
        });
    }

    render(){
        let element = <div></div>;
        switch(this.state.currentPlatform){
            case CurrentPlatformEnum.None: {
                element = (
                    <div className="ConfigChoosePlatform">
                        <h2>Select a platform</h2>
                        <div className="row">
                            <div className="selectPlatform xboxlive button" onClick={(e) => this.onSelect(e, CurrentPlatformEnum.XBoxLive) }>
                                XBox Live
                            </div>
                            <div className="selectPlatform steam button" onClick={(e) => this.onSelect(e, CurrentPlatformEnum.Steam) }>
                                Steam
                            </div>
                        </div>
                    </div>
                )
                break;
            }
            case CurrentPlatformEnum.Steam: {
                element = (
                    <div className="ConfigSteam">
                        <ConfigSteamConfigRoot />
                        <input type="button" value="Back" onClick={this.onBack} />
                    </div>
                );
                break;
            }
            case CurrentPlatformEnum.XBoxLive: {
                element = (
                    <div className="ConfigXBoxLive">
                        <ConfigXBLConfigRoot />
                        <input type="button" value="Back" onClick={this.onBack} />
                    </div>
                );
                break;
            }
        }
        return element;
    }
}