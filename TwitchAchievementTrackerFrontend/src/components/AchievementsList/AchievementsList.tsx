import * as React from "react";
import { Achievement } from "../../common/EBSTypes";

interface AchievementsListProps {
    achievements: Achievement[],
}

interface AchievementsListState {
}

export default class AchievementsList extends React.Component<AchievementsListProps, AchievementsListState> {
    constructor(props: AchievementsListProps){
        super(props);
    }

    render(){
        return (
            <ul id="list">
                {this.props.achievements.map((achievement, i)=> 
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
        )
    }
}