import * as React from 'react';
import './VideoOverlay.scss';
import { ConfigurationService } from '../../services/EBSConfigurationService';
import { ExtensionConfiguration, AchievementSummary, Achievement, TitleInfo } from '../../common/EBSTypes';
import { AchievementsService } from '../../services/EBSAchievementsService';
import NujaCup from '../../../assets/nujacup.svg';
import NujaLogo from '../../../assets/nuja.png';
import { Twitch } from '../../services/TwitchService';

type VideoOverlayState = {
    isCollapsed: boolean,
    titleInfo: TitleInfo,
    achievementsSummary: AchievementSummary,
    achievementsDetails: Achievement[],
}

export default class VideoOverlay extends React.Component<any, VideoOverlayState> {
    state: VideoOverlayState = {
        isCollapsed: true,
        titleInfo: null,
        achievementsSummary: null,
        achievementsDetails: []
    }

    constructor(props: any){
        super(props);

        this.togglePanel = this.togglePanel.bind(this);
        this.refreshAll = this.refreshAll.bind(this);
        this.refreshSummary = this.refreshSummary.bind(this);
    }

    componentDidMount = () => {
        this.refreshAll();
        setInterval(this.refreshSummary, 60000);

        Twitch.listen("broadcast", (target, contentType, messageStr) => {
            let message = JSON.parse(messageStr);
            let configToken = AchievementsService.configuration.content;
            if (message.configToken != configToken)
            {
                AchievementsService.configuration.content = message.configToken;
                AchievementsService.configuration.version = message.version;
                this.refreshAll();
            }
        });
    }

    togglePanel = (e: React.SyntheticEvent<HTMLElement>) => {
        this.setState({
            isCollapsed: !this.state.isCollapsed
        })
        if (! this.state.isCollapsed)
        {
            this.refreshSummary();
        }
    }

    refreshAll = async () => {
        let titleInfoPromise = AchievementsService.getTitleInfo();
        let summaryPromise = AchievementsService.getSummary();
        let achievementsPromise = AchievementsService.getAchievements();

        this.setState({
            titleInfo: await titleInfoPromise,
            achievementsSummary: await summaryPromise,
            achievementsDetails: await achievementsPromise,
        });
    }

    refreshSummary = async () => {
        let summaryPromise = AchievementsService.getSummary();
        let achievementsPromise = this.state.isCollapsed ? Promise.resolve(this.state.achievementsDetails) : AchievementsService.getAchievements();

        this.setState({
            achievementsSummary: await summaryPromise,
            achievementsDetails: await achievementsPromise,
        });
    }

    render(){
        let percentage = this.state.achievementsSummary ? (this.state.achievementsSummary.completed / this.state.achievementsSummary.total) * 100.0 : 0.0;
        let completedCount = this.state.achievementsSummary?.completed ?? 0;
        let totalCount = this.state.achievementsSummary?.total ?? 0;

        return (
            <div className={this.state.isCollapsed ? "overlayBox collapsed" : "overlayBox open"}>
                <div className="gameLogo noselect" onClick={this.togglePanel} style={{backgroundImage: `url(${this.state.titleInfo?.logoUri ?? NujaLogo})`}}>
                    <div className="summaryWidget">
                        { this.state.achievementsSummary ? percentage.toFixed(0) + '%' : ''}
                    </div>
                </div>
                <div id="achievementsPanel">
                    <div className="card-container">
                        <h2 id="gameTitle">
                            {this.state.titleInfo?.productTitle ?? "Game"}
                        </h2>
                        <div id="completionHeadline">
                            <span className="percentage">{percentage.toPrecision(2)}%</span>
                            <img src={NujaCup} alt="achievements" />
                            <span className="completedCount">{completedCount}</span>/<span className="totalCount">{totalCount}</span> 
                        </div>
                    </div>
                    <ul id="list">
                        {this.state.achievementsDetails.map((achievement, i)=> 
                            (
                                <li key={"achievement_" + i + "_" + achievement.id} className={achievement.completed ? "completed" : "notCompleted"}>
                                    <div className="achievementTitle">
                                        {achievement.name}
                                    </div>
                                    <div className="achievementDescription">
                                        {achievement.description}
                                    </div>
                                </li>
                            ))
                        }
                    </ul>
                </div>
            </div>
        )
    }
}