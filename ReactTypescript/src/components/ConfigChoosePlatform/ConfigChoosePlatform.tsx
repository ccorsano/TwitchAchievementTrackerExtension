import * as React from 'react';
import './ConfigChoosePlatform.scss';
import { NoEmitOnErrorsPlugin } from 'webpack';
import ConfigXBLConfigRoot from '../ConfigXBLConfigRoot/ConfigXBLConfigRoot';
import ConfigSteamConfigRoot from '../ConfigSteamConfigRoot/ConfigSteamConfigRoot';
import { EBSBase } from '../../services/EBSBase';
import EBSConfigurationService from '../../services/EBSConfigurationService';

type onChangeCallback = (e: React.SyntheticEvent<HTMLInputElement>) => void;

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

    onSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
        console.log(e.currentTarget.value);

        let value = Number.parseInt(e.currentTarget.value);

        this.setState({
            currentPlatform: value
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
                        <label htmlFor="xboxlive">XBox Live</label>
                        <input type="radio" name="configSelect" value={CurrentPlatformEnum.XBoxLive} onChange={this.onSelect} checked={this.isSelected(CurrentPlatformEnum.XBoxLive)} />
                        <label htmlFor="steam">Steam</label>
                        <input type="radio" name="configSelect" value={CurrentPlatformEnum.Steam} onChange={this.onSelect} checked={this.isSelected(CurrentPlatformEnum.Steam)} />
                    </div>
                )
                break;
            }
            case CurrentPlatformEnum.Steam: {
                element = (
                    <div className="ConfigSteam">
                        <h2>Steam !!!!</h2>
                        <ConfigSteamConfigRoot />
                        <input type="button" value="Back" onClick={this.onBack} />
                    </div>
                );
                break;
            }
            case CurrentPlatformEnum.XBoxLive: {
                element = (
                    <div className="ConfigXBoxLive">
                        <h2>XBoxLive !!!!</h2>
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