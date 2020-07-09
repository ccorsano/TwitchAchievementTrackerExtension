import * as React from 'react';
import * as Base from '../../common/ConfigStepBase'
import { ConfigurationService, ValidationError } from '../../services/EBSConfigurationService';
import { ExtensionConfiguration, ActiveConfig, PlayerInfoCard } from '../../common/EBSTypes';
import * as ServerConfig from '../../common/ServerConfig'
import EBSAchievementsService from '../../services/EBSAchievementsService';
import GamerCardComponent from '../GamerCard/GamerCard';
import { ConfigSteamConfigStateEnum } from '../../common/ConfigStepBase';
import ConfigSteam_03_AppId from '../ConfigSteam_03_AppId/ConfigSteam_03_AppId'

interface ConfigSteam_03_SteamIDProps extends Base.ConfigStepBaseProps {
    webApiKey: string;
}

type ConfigSteam_03_SteamIDState = {
    isLoading: boolean;
    isProfileUrlFormatValid: boolean;
    isProfileValid: boolean;
    isConfirmed: boolean;
    steamProfileUrl: string;
    steamProfileId: string;
    steamProfile: PlayerInfoCard;
    errors: ValidationError[],
}

export default class ConfigSteam_03_SteamID extends Base.ConfigStepBase<ConfigSteam_03_SteamIDProps, ConfigSteam_03_SteamIDState> {
    formatRegexp: RegExp = /^https:\/\/steamcommunity\.com\/id\/([^\/]+)(\/.*)?$/i;
    steamIdRegexp: RegExp = /^[0-9]+$/;

    state: ConfigSteam_03_SteamIDState = {
        isLoading: true,
        isProfileUrlFormatValid: false,
        isProfileValid: false,
        isConfirmed: false,
        steamProfileUrl: null,
        steamProfileId: null,
        steamProfile: null,
        errors: [],
    }

    constructor(props: ConfigSteam_03_SteamIDProps){
        super(props);

        this.onChangeProfileUrl = this.onChangeProfileUrl.bind(this);
        this.onValidate = this.onValidate.bind(this);
        this.onResetProfile = this.onResetProfile.bind(this);
        this.onContinue = this.onContinue.bind(this);
        this.unvalidate = this.unvalidate.bind(this);
    }

    componentDidMount = () => {
        let steamId = this.props.savedConfiguration?.steamConfig?.steamId;
        if (steamId && this.steamIdRegexp.test(steamId)) {
            ConfigurationService.resolveSteamPlayerInfo(steamId, this.props.webApiKey)
                .then(playerInfo => {
                    this.setState({
                        isLoading: false,
                        isProfileUrlFormatValid: false,
                        isProfileValid: true,
                        steamProfileId: steamId,
                        steamProfile: playerInfo,
                    });
                });
        }
        else
        {
            this.setState({
                isLoading: false,
            });
        }
    }

    onChangeProfileUrl = (e: React.ChangeEvent<HTMLInputElement>) => {
        let formatCheck = this.formatRegexp.test(e.currentTarget.value);
        this.setState({
            isProfileUrlFormatValid: formatCheck,
            isProfileValid: false,
            steamProfileUrl: formatCheck ? e.currentTarget.value : null,
        });
    }

    onValidate = async (e: React.SyntheticEvent<HTMLInputElement>) => {
        this.setState({
            isLoading: true,
        })

        try {
            let split = this.formatRegexp.exec(this.state.steamProfileUrl);
            let profileId = split[1];
            let resolvedProfile: PlayerInfoCard = null;
    
            if (! this.steamIdRegexp.test(profileId))
            {
                resolvedProfile = await ConfigurationService.resolveSteamVanityUrl(profileId, this.props.webApiKey);
                profileId = resolvedProfile.playerId;
            }
            else
            {
                resolvedProfile = await ConfigurationService.resolveSteamPlayerInfo(profileId, this.props.webApiKey);
            }
            
            const configuration: ExtensionConfiguration = {
                activeConfig: ActiveConfig.Steam,
                steamConfig: {
                    steamId: profileId,
                    webApiKey: this.props.webApiKey,
                    appId: null,
                    locale: null
                },
                version: ServerConfig.EBSVersion,
                xBoxLiveConfig: null
            }
            
            let errors = await ConfigurationService.validateConfiguration(configuration);
            errors = errors.filter(e => e.path == "SteamConfig.WebApiKey" || e.path == "SteamConfig.SteamId");
            
            this.setState({
                errors: errors,
            });
    
            if (this.state.errors.length == 0)
            {
                this.setState({
                    isProfileValid: true,
                    steamProfileId: profileId,
                    steamProfile: resolvedProfile,
                });
            }
        }
        finally
        {
            this.setState({
                isLoading: false,
            });
        }
    }
    
    onResetProfile = (e: React.SyntheticEvent<HTMLInputElement>) => {
        this.setState({
            isProfileUrlFormatValid: false,
            isProfileValid: false,
            steamProfileId: null,           
            steamProfile: null,
        });
    }

    onContinue = (e: React.SyntheticEvent<HTMLInputElement>) => {
        this.setState({
            isConfirmed: true
        });
    }

    unvalidate = (e: React.SyntheticEvent<HTMLElement>) => {
        this.setState({
            isConfirmed: false,
        });
    }

    render(){
        let isCheckEnabled = this.state.isProfileUrlFormatValid;
        let isContinueEnabled = this.state.isProfileValid;

        if (this.state.isConfirmed){
            return (
                <ConfigSteam_03_AppId
                    savedConfiguration={this.props.savedConfiguration}
                    onValidate={this.props.onValidate}
                    onBack={this.unvalidate}
                    webApiKey={this.props.webApiKey}
                    steamProfileId={this.state.steamProfileId} />
            )
        }

        let playerInfoCard = null;
        if (! this.state.isLoading)
        {
            if (this.state.steamProfile)
            {
                let changeButton = (
                    <input type="button" value="Change" className="section" onClick={this.onResetProfile} />
                )
                playerInfoCard = [
                    <GamerCardComponent playerInfo={this.state.steamProfile} buttonSection={changeButton} />,
                ]
            }
            else
            {
                playerInfoCard = [
                    <label htmlFor="vanityUrl">Steam Profile URL</label>,
                    <input name="vanityUrl" type="text" pattern="https:\/\/steamcommunity\.com\/id\/([^\/]+)(\/.*)?$" placeholder="Enter your Steam profile URL" onChange={this.onChangeProfileUrl} />,
                    <ul>
                        {this.state.errors.map((error, i) => (
                            <li key={error.path + '_' + i}>
                                {error.path}: {error.errorDescription}
                            </li>
                        ))}
                    </ul>,
                    <input type="button" value="Search" disabled={!isCheckEnabled} onClick={this.onValidate} />
                ]
            }
        }
        else
        {
            playerInfoCard = (
                <div className="card">
                    <div className="spinner"></div>
                </div>
            )
        }

        return [
            playerInfoCard,
            <input type="button" value="Back" onClick={this.onBack} />,
            <input type="button" value="Continue" disabled={!isContinueEnabled} onClick={this.onContinue} />
        ]
    }
}