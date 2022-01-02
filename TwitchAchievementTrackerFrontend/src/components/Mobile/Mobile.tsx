import * as React from 'react';
import './Mobile.scss'
import { TitleInfo, AchievementSummary, Achievement, ActiveConfig } from '../../common/EBSTypes';
import { AchievementsService } from '../../services/EBSAchievementsService';
import { Twitch } from '../../services/TwitchService';
import Cup from '../../../assets/cup.svg';
import Logo from '../../../assets/logo.png';
import AchievementsList from '../AchievementsList/AchievementsList';

type VideoOverlayState = {
    isConfigurationValid: boolean,
    titleInfo: TitleInfo,
    achievementsSummary: AchievementSummary,
    achievementsDetails: Achievement[],
}

export default class Mobile extends React.Component<any, VideoOverlayState> {
    state: VideoOverlayState = {
        isConfigurationValid: false,
        titleInfo: null,
        achievementsSummary: null,
        achievementsDetails: []
    }

    constructor(props: any){
        super(props);

        this.refreshAll = this.refreshAll.bind(this);
        this.refreshSummary = this.refreshSummary.bind(this);
    }
    
    componentDidMount = () => {
        this.refreshAll();
        setInterval(this.refreshSummary, 60000);

        Twitch.listen("broadcast", (_target, _contentType, messageStr) => {
            let message = JSON.parse(messageStr);
            let configToken = AchievementsService.configuration.content;
            
            switch (message.type) {
                case "refresh":
                    this.refreshAll();
                    break;
                case "set-config":
                    if (message.configToken != configToken)
                    {
                        AchievementsService.configuration.content = message.configToken;
                        AchievementsService.configuration.version = message.version;
                        this.refreshAll();
                    }
                    break;
                default:
                    break;
            }
        });
    }

    refreshAll = async () => {
        let titleInfoPromise = AchievementsService.getTitleInfo();
        AchievementsService.getSummary().then(summary => this .setState({achievementsSummary: summary}));;
        AchievementsService.getAchievements().then(achievements => this .setState({achievementsDetails: achievements}));

        try {
            let titleInfo = await titleInfoPromise;
            this.setState({
                isConfigurationValid: true,
                titleInfo: titleInfo,
            });
        }
        catch(error)
        {
            this.setState({
                isConfigurationValid: false,
            });
        }
    }

    refreshSummary = async () => {
        let summaryPromise = AchievementsService.getSummary();
        let achievementsPromise = AchievementsService.getAchievements();

        this.setState({
            achievementsSummary: await summaryPromise,
            achievementsDetails: await achievementsPromise,
        });
    }


    render(){
        let percentage = this.state.achievementsSummary ? (this.state.achievementsSummary.completed / this.state.achievementsSummary.total) * 100.0 : 0.0;
        let completedCount = this.state.achievementsSummary?.completed ?? 0;
        let totalCount = this.state.achievementsSummary?.total ?? 0;
        let platformClass = this.state.titleInfo?.platform == ActiveConfig.Steam ?  "steam" : "xboxlive";
        let logoClassName = "gameLogo noselect " + platformClass;

        return (
            <div className="overlayBox open">
                <div id="achievementsPanel" className={platformClass}>
                    <div className={logoClassName} style={{backgroundImage: `url(${this.state.titleInfo?.logoUri ?? Logo})`}}>
                    </div>
                    <div className="card-container">
                        <div id="completionHeadline">
                            <span className="percentage">{percentage.toPrecision(2)}%</span>
                            <img src={Cup} alt="achievements" />
                            <span className="completedCount">{completedCount}</span>/<span className="totalCount">{totalCount}</span> 
                        </div>
                        <h2 id="gameTitle" className={platformClass}>
                            {this.state.titleInfo?.productTitle ?? "Game"}
                        </h2>
                    </div>
                </div>
                <AchievementsList achievements={this.state.achievementsDetails} platform={this.state.titleInfo?.platform} />
            </div>
        )
    }
}