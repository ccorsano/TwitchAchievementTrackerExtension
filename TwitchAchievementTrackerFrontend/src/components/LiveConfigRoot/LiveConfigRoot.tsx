import * as React from 'react';
import { ConfigurationService, ValidationError } from '../../services/EBSConfigurationService';
import { ExtensionConfiguration, ActiveConfig, RateLimits } from '../../common/EBSTypes';
import ConfigSummary from '../ConfigSummary/ConfigSummary';
import { Twitch } from '../../services/TwitchService';
import { EBSVersion } from '../../common/ServerConfig' 
import { AchievementsService } from '../../services/EBSAchievementsService';
import CountDown from '../CountDown/CountDown';

interface LiveConfigRootState {
    isLoading: boolean;
    isRefreshing: boolean;
    wasModified: boolean;
    configuration: ExtensionConfiguration;
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
        errors: [],
        xApiRateLimits: null,
    }

    constructor(props: any){
        super(props);

        this.onForceRefresh = this.onForceRefresh.bind(this);
    }

    componentDidMount = async () => {
        await ConfigurationService.configuredPromise;

        let configuration = await ConfigurationService.getConfiguration();
        let validation = await ConfigurationService.validateConfiguration(configuration);

        this.setState({
            isLoading: false,
            configuration: configuration,
            errors: validation,
        });
        
        if (configuration.activeConfig == ActiveConfig.XBoxLive)
        {
            this.refreshRateLimit();
            this.refreshRateLimitsInterval = setInterval(this.refreshRateLimit, 5000);
        }
        
        Twitch.listen("broadcast", async (target, contentType, messageStr) => {
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
                        let config = await ConfigurationService.getConfiguration();
                        this.setState({
                            configuration: config
                        });
                        
                        if (message. config.activeConfig == ActiveConfig.XBoxLive)
                        {
                            if (! this.refreshRateLimitsInterval)
                            {
                                this.refreshRateLimitsInterval = setInterval(this.refreshRateLimit, 5000);
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
            return;
        }

        let rateLimits = await ConfigurationService.getXApiRateLimits();
        this.setState({
            xApiRateLimits: rateLimits
        })
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
                <h2 className="section">Achievements Tracker</h2>
                <button className="section" onClick={this.onForceRefresh} disabled={this.state.isRefreshing}>Force refresh</button>
                
                {this.state.xApiRateLimits ? (
                    <div className="section">
                        xApi rate limits: {this.state.xApiRateLimits.remaining}/{this.state.xApiRateLimits.hourlyLimit}<br/>
                        rate limit reset: <CountDown targetDate={this.state.xApiRateLimits.resetTime} />
                    </div>
                ) : null}
            </div>
        )
    }
}