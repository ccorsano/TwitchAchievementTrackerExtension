import * as React from 'react';
import './ConfigChoosePlatform.scss';
import ConfigXBLConfigRoot from '../ConfigXBLConfigRoot/ConfigXBLConfigRoot';
import ConfigSteamConfigRoot from '../ConfigSteamConfigRoot/ConfigSteamConfigRoot';
import { ConfigurationService } from '../../services/EBSConfigurationService';
import { ExtensionConfiguration, ActiveConfig } from '../../common/EBSTypes';
import XBoxLiveLogo from '../../../assets/XBox_Live_logo.svg';
import SteamLogo from '../../../assets/Steam_icon_logo.svg';
import ConfigSummary from '../ConfigSummary/ConfigSummary';
import { TwitchExtensionConfiguration } from '../../common/TwitchExtension';

type onChangeCallback = (e: React.SyntheticEvent<HTMLElement>) => void;

type ConfigChoosePlatformProps = {
    changeHandler: onChangeCallback,
};

enum CurrentPlatformEnum {
    None = 0,
    XBoxLive,
    Steam,
}

type ConfigChoosePlatformState = {
    isLoading: boolean,
    isConfirmed: boolean,
    savedConfiguration: ExtensionConfiguration,
    currentPlatform : CurrentPlatformEnum;
    hasSaved: boolean;
};

export default class ConfigChoosePlatform extends React.Component<ConfigChoosePlatformProps, ConfigChoosePlatformState> {
    state: ConfigChoosePlatformState = {
        isLoading: true,
        isConfirmed: false,
        savedConfiguration: null,
        currentPlatform: CurrentPlatformEnum.None,
        hasSaved: false,
    };

    constructor(props : ConfigChoosePlatformProps){
        super(props)

        this.onSelect = this.onSelect.bind(this);
        this.onBack = this.onBack.bind(this);
        this.isSelected = this.isSelected.bind(this);
        this.onSaved = this.onSaved.bind(this);
    }

    componentDidMount = async () => {
        let [, configurationToken] = await ConfigurationService.configuredPromise;
        if (configurationToken)
        {
            let configuration = await ConfigurationService.fetchConfiguration(configurationToken);
            
            var validation = await ConfigurationService.validateConfiguration(configuration);

            this.setState({
                isLoading: false,
                isConfirmed: validation.length == 0,
                savedConfiguration: configuration,
                currentPlatform: configuration.activeConfig == ActiveConfig.XBoxLive ? CurrentPlatformEnum.XBoxLive : CurrentPlatformEnum.Steam,
            });
        }
        else
        {
            this.setState({
                isLoading: false,
                isConfirmed: false,
                savedConfiguration: null,
            });
        }
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

    onBack = () => {
        this.setState({
            isConfirmed: this.state.savedConfiguration != null,
            currentPlatform: CurrentPlatformEnum.None
        });
    }

    onRestart = () => {
        this.setState({
            isConfirmed: false,
            currentPlatform: CurrentPlatformEnum.None
        });
    }

    onSaved = (savedConfiguration: TwitchExtensionConfiguration, configurationObject: ExtensionConfiguration) => {
        this.setState({
            isConfirmed: true,
            savedConfiguration: configurationObject,
            currentPlatform: CurrentPlatformEnum.None,
            hasSaved: true,
        })
    }

    render(){
        let element = (<div></div>);

        if (this.state.isLoading)
        {
            element = (
                <div className="card">
                    <div className="spinner"></div>
                </div>
            )
        }
        else if (this.state.isConfirmed)
        {
            element = (
                <React.Fragment>
                    {this.state.hasSaved ? (<mark className="tag tertiary">Saved and pushed to viewers !</mark>) : null }
                    <ConfigSummary extensionConfig={this.state.savedConfiguration} onConfigure={this.onRestart} />
                </React.Fragment>
            )
        }
        else
        {
            switch(this.state.currentPlatform){
                case CurrentPlatformEnum.None: {
                    element = (
                        <div className="ConfigChoosePlatform">
                            <h2>Select a platform</h2>
                            <div className="row">
                                <div className="selectPlatform card xboxlive">
                                    <h1 className="section">XBox Live</h1>
                                    <img src={XBoxLiveLogo} className="section media" style={{objectFit: 'contain'}} />
                                    <input type="button" value="Select" className="section" onClick={(e) => this.onSelect(e, CurrentPlatformEnum.XBoxLive) } />
                                </div>
                                <div className="selectPlatform card steam">
                                    <h1 className="section">Steam</h1>
                                    <img src={SteamLogo} className="section media" style={{objectFit: 'contain'}} />
                                    <input type="button" value="Select" className="section" onClick={(e) => this.onSelect(e, CurrentPlatformEnum.Steam) } />
                                </div>
                            </div>
                            <input type="button" value="Cancel" onClick={this.onBack} />,
                        </div>
                    )
                    break;
                }
                case CurrentPlatformEnum.Steam: {
                    element = (
                        <div className="ConfigSteam">
                            <ConfigSteamConfigRoot onSaved={this.onSaved} savedConfiguration={this.state.savedConfiguration} onCancel={this.onRestart} />
                        </div>
                    );
                    break;
                }
                case CurrentPlatformEnum.XBoxLive: {
                    element = (
                        <div className="ConfigXBoxLive">
                            <ConfigXBLConfigRoot onSaved={this.onSaved} savedConfiguration={this.state.savedConfiguration} onCancel={this.onRestart} />
                        </div>
                    );
                    break;
                }
            }
        }

        return element;
    }
}