import * as React from "react";
import { TwitchExtensionConfiguration, TwitchAuthCallbackContext } from "../../common/TwitchExtension";
import { ExtensionConfiguration, PlayerInfoCard, TitleInfo, SupportedLanguage, ActiveConfig } from "../../common/EBSTypes";
import { ConfigurationService } from "../../services/EBSConfigurationService";
import XBoxLiveLogo from '../../../assets/XBox_Live_logo.svg';
import SteamLogo from '../../../assets/Steam_icon_logo.svg';

type ConfigSummaryProps = {
    extensionConfig: ExtensionConfiguration;
    onConfigure: (e: React.MouseEvent<HTMLInputElement>) => void;
}

type ConfigSummaryState = {
    playerInfo: PlayerInfoCard,
    gameInfo: TitleInfo,
    locale: SupportedLanguage,
}

export default class ConfigSummary extends React.Component<ConfigSummaryProps, ConfigSummaryState> {
    state: ConfigSummaryState = {
        playerInfo: null,
        gameInfo: null,
        locale: null,
    }

    constructor(props: ConfigSummaryProps){
        super(props);

        this.componentDidMount = this.componentDidMount.bind(this);
        this.componentWillReceiveProps = this.componentWillReceiveProps.bind(this);
        this.updateFromProps = this.updateFromProps.bind(this);
    }

    componentDidMount = () => {
        this.updateFromProps(this.props);
    }

    componentWillReceiveProps = (props: ConfigSummaryProps) => {
        this.updateFromProps(props);
    }

    updateFromProps = async (props: ConfigSummaryProps) => {
        let configuration = props.extensionConfig;

        switch (configuration.activeConfig) {
            case ActiveConfig.XBoxLive:
                    ConfigurationService.resolveXBoxLivePlayerInfo(configuration.xBoxLiveConfig.streamerXuid, configuration.xBoxLiveConfig.xApiKey)
                        .then(gamerCard => this.setState({
                            playerInfo: gamerCard
                        }));
                    ConfigurationService.resolveXBoxLiveTitleInfo(configuration.xBoxLiveConfig.titleId, configuration.xBoxLiveConfig.xApiKey)
                        .then(titleInfo => this.setState({
                            gameInfo: titleInfo
                        }));
                    ConfigurationService.getXBoxLiveSupportedLanguages(configuration.xBoxLiveConfig.titleId, configuration.xBoxLiveConfig.xApiKey)
                        .then(languages => this.setState({
                            locale: languages.find(l => l.langCode == configuration.xBoxLiveConfig.locale)
                        }));
                break;
                case ActiveConfig.Steam:
                    ConfigurationService.resolveSteamPlayerInfo(configuration.steamConfig.steamId, configuration.steamConfig.webApiKey)
                        .then(gamerCard => this.setState({
                            playerInfo: gamerCard
                        }));
                    ConfigurationService.getSteamOwnedGames(configuration.steamConfig.steamId, configuration.steamConfig.webApiKey)
                        .then(titles => this.setState({
                            gameInfo: titles.find(t => t.titleId == configuration.steamConfig.appId)
                        }));
                    ConfigurationService.getSteamSupportedLanguages(configuration.steamConfig.appId, configuration.steamConfig.webApiKey)
                        .then(languages => this.setState({
                            locale: languages.find(l => l.langCode == configuration.steamConfig.locale)
                        }));
                break;
            default:
                break;
        }
    }

    render(){
        if (this.state.gameInfo && this.state.playerInfo && this.state.locale)
        {
            let platformLogo = this.props.extensionConfig.activeConfig == ActiveConfig.XBoxLive ? XBoxLiveLogo : SteamLogo;
            let platformPlayerNaming = this.props.extensionConfig.activeConfig == ActiveConfig.XBoxLive ? "GamerTag" : "Profile";
    
            return (
                <div className="card large">
                    <img className="section" src={platformLogo} style={{objectFit: 'contain', maxHeight: '5em'}}  />
                    <h1 className="section">{this.state.gameInfo.productTitle}</h1>
                    <h3 className="section">{this.state.playerInfo.playerName} <small>{platformPlayerNaming}</small></h3>
                    <h4 className="section">{this.state.locale.displayName}  <small>Language</small></h4>
                    { this.props.onConfigure ? (<input type="button" value="Configure" className="section" onClick={this.props.onConfigure}></input>) : null }
                </div>
            )
        }
        else
        {
            return (
                <div className="card">
                    <div className="spinner"></div>
                </div>
            )
        }
    }
}