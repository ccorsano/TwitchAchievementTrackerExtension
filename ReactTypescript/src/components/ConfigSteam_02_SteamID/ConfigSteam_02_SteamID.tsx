import * as React from 'react';
import * as Base from '../../common/ConfigStepBase'
import { ConfigurationService, ValidationError } from '../../services/EBSConfigurationService';
import { ConfigurationState } from '../../services/ConfigurationStateService';
import { ExtensionConfiguration, ActiveConfig, PlayerInfoCard } from '../../common/EBSTypes';
import * as ServerConfig from '../../common/ServerConfig'
import EBSAchievementsService from '../../services/EBSAchievementsService';
import GamerCardComponent from '../GamerCard/GamerCard';

type ConfigSteam_03_SteamIDState = {
    isLoading: boolean;
    isProfileUrlFormatValid: boolean;
    isProfileValid: boolean;
    steamProfileUrl: string;
    steamProfileId: string;
    steamProfile: PlayerInfoCard;
    errors: ValidationError[],
}

export default class ConfigSteam_03_SteamID extends Base.ConfigStepBase<Base.ConfigStepBaseProps, ConfigSteam_03_SteamIDState> {
    formatRegexp: RegExp = /^https:\/\/steamcommunity\.com\/id\/([^/]+)\/?$/i;
    steamIdRegexp: RegExp = /^[0-9]+$/;

    state: ConfigSteam_03_SteamIDState = {
        isLoading: true,
        isProfileUrlFormatValid: false,
        isProfileValid: false,
        steamProfileUrl: null,
        steamProfileId: null,
        steamProfile: null,
        errors: [],
    }

    constructor(props: Base.ConfigStepBaseProps){
        super(props);
    }

    componentDidMount = () => {
        let steamId = ConfigurationState.currentConfiguration.steamConfig.steamId;
        if (steamId && this.steamIdRegexp.test(steamId)) {
            ConfigurationService.resolveSteamPlayerInfo(steamId, ConfigurationState.currentConfiguration.steamConfig.webApiKey)
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
        let split = this.formatRegexp.exec(this.state.steamProfileUrl);
        let profileId = split[1];
        let resolvedProfile: PlayerInfoCard = null;

        if (! this.steamIdRegexp.test(profileId))
        {
            resolvedProfile = await ConfigurationService.resolveSteamVanityUrl(profileId, ConfigurationState.currentConfiguration.steamConfig.webApiKey);
            profileId = resolvedProfile.playerId;
        }
        else
        {
            resolvedProfile = await ConfigurationService.resolveSteamPlayerInfo(profileId, ConfigurationState.currentConfiguration.steamConfig.webApiKey);
        }
        
        const configuration: ExtensionConfiguration = {
            activeConfig: ActiveConfig.Steam,
            steamConfig: {
                steamId: profileId,
                webApiKey: ConfigurationState.currentConfiguration.steamConfig.webApiKey,
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
    
    onResetProfile = (e: React.SyntheticEvent<HTMLInputElement>) => {
        this.setState({
            isProfileUrlFormatValid: false,
            isProfileValid: false,
            steamProfileId: null,           
            steamProfile: null,
        });
    }

    onContinue = (e: React.SyntheticEvent<HTMLInputElement>) => {
        ConfigurationState.currentConfiguration.steamConfig.steamId = this.state.steamProfileId;
        this.props.onValid(this, this.props.nextState);
    }

    render(){
        let isCheckEnabled = this.state.isProfileUrlFormatValid;
        let isContinueEnabled = this.state.isProfileValid;

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
                    <input name="vanityUrl" type="text" pattern="https:\/\/steamcommunity\.com\/id\/([^/]+)\/?$" placeholder="Enter your Steam profile URL" onChange={this.onChangeProfileUrl} />,
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
                <div>Loading ...</div>
            )
        }

        return [
            playerInfoCard,
            <input type="button" value="Continue" disabled={!isContinueEnabled} onClick={this.onContinue} />
        ]
    }
}