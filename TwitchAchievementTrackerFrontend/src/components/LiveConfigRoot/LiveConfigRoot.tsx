import * as React from 'react';
import { ConfigurationService, ValidationError } from '../../services/EBSConfigurationService';
import { ExtensionConfiguration, ActiveConfig, RateLimits, TitleInfo } from '../../common/EBSTypes';
import { Twitch } from '../../services/TwitchService';
import { EBSVersion } from '../../common/ServerConfig' 
import { AchievementsService } from '../../services/EBSAchievementsService';
import CountDown from '../CountDown/CountDown';
import './LiveConfigRoot.scss';

interface LiveConfigRootState {
    isLoading: boolean;
    isRefreshing: boolean;
    wasModified: boolean;
    configuration: ExtensionConfiguration;
    titleInfo: TitleInfo;
    errors: ValidationError[];
    xApiRateLimits: RateLimits;
}

export default class LiveConfigRoot extends React.Component<any,LiveConfigRootState>
{
    refreshRateLimitsInterval: NodeJS.Timeout = null;

    state: LiveConfigRootState = {
        isLoading: true,
        isRefreshing: false,
        wasModified: false,
        configuration: null,
        titleInfo: null,
        errors: [],
        xApiRateLimits: null,
    }

    constructor(props: any){
        super(props);

        this.onForceRefresh = this.onForceRefresh.bind(this);
    }

    componentDidMount = async () => {
        let titleInfo: TitleInfo = null;

        await ConfigurationService.configuredPromise;

        let configuration = await ConfigurationService.getConfiguration();
        let validation = await ConfigurationService.validateConfiguration(configuration);
        if (validation.length == 0)
        {
            titleInfo = await AchievementsService.getTitleInfo();
        }

        this.setState({
            isLoading: false,
            configuration: configuration,
            titleInfo: titleInfo,
            errors: validation,
        });
        
        if (configuration.activeConfig == ActiveConfig.XBoxLive)
        {
            this.refreshRateLimit();
            this.refreshRateLimitsInterval = setInterval(this.refreshRateLimit, 1000);
        }
        
        Twitch.listen("broadcast", async (_target, _contentType, messageStr) => {
            let message = JSON.parse(messageStr);
            let configToken = AchievementsService.configuration.content;
            
            switch (message.type) {
                case "refresh":
                    break;
                case "set-config":
                    if (message.configToken != configToken)
                    {
                        AchievementsService.configuration.content = message.configToken;
                        AchievementsService.configuration.version = message.version;
                        ConfigurationService.configuration.content = message.configToken;
                        ConfigurationService.configuration.version = message.version;

                        this.setState({
                            isLoading: true,
                        });
                        let config = await ConfigurationService.getConfiguration();
                        let titleInfo = await AchievementsService.getTitleInfo();

                        this.setState({
                            isLoading: false,
                            configuration: config,
                            titleInfo: titleInfo,
                            errors: validation,
                        });
                        
                        if (config.activeConfig == ActiveConfig.XBoxLive)
                        {
                            if (! this.refreshRateLimitsInterval)
                            {
                                this.refreshRateLimitsInterval = setInterval(this.refreshRateLimit, 1000);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        });
    }

    refreshRateLimit = async () => {
        if (this.state.configuration.activeConfig != ActiveConfig.XBoxLive)
        {
            clearInterval(this.refreshRateLimitsInterval);
            this.refreshRateLimitsInterval = null;
            this.setState({
                xApiRateLimits: null
            });
            return;
        }

        let rateLimits = await ConfigurationService.getXApiRateLimits();
        this.setState({
            xApiRateLimits: rateLimits
        });
    }

    onForceRefresh = async () => {
        this.setState({
            isRefreshing: true
        });

        let didChange = await ConfigurationService.forceRefresh();
        if (didChange)
        {
            Twitch.send("broadcast", "application/json", {
                "type": "refresh",
                "version": EBSVersion,
            });
        }

        this.setState({
            isRefreshing: false,
            wasModified: didChange,
        });
    }

    render(){
        if (this.state.isLoading)
        {
            return (
                <div className="card small">
                    <div className="spinner"></div> Loading configuration
                </div>
            )
        }

        return (
            <div className="card small">
                <h3 className="section">Achievements Tracker</h3>
                <div className="section titleinfo row">
                    <div className="col-sm-4" >
                        <img src={this.state.titleInfo.logoUri} />
                    </div>
                    <div className="col-sm-8">
                        <button className="large primary" onClick={this.onForceRefresh} disabled={this.state.isRefreshing}>Force refresh</button>
                    </div>
                </div>
                
                {this.state.xApiRateLimits ? [
                    <div className="section">
                        <h4>xApi.us calls</h4>
                        <div className="row">
                            <div className="col-sm-6">
                                Remaining
                            </div>
                            <div className="col-sm-6">
                                {this.state.xApiRateLimits.remaining}/{this.state.xApiRateLimits.hourlyLimit}
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-sm-6">
                                Resets in
                            </div>
                            <div className="col-sm-6">
                                <CountDown targetDate={this.state.xApiRateLimits.resetTime} />
                            </div>
                        </div>
                    </div>,
                    <div className="section">
                        Force refresh typically uses 1 call.<br/>
                        15 auto-refresh per hour.
                    </div>
                ] : null}
            </div>
        )
    }
}