import * as React from 'react';
import './Mobile.scss'

export default class Mobile extends React.Component {
    constructor(props: any){
        super(props);
    }

    render(){
        return (
            <div className="overlayBox open">
                <div id="achievementsPanel">
                    <div className="gameLogo noselect">
                    </div>
                    <div className="card-container">
                        <div id="completionHeadline">
                            <span className="percentage"></span><img src="nujacup.svg" alt="achievements" /><span className="completedCount">?</span>/<span className="totalCount">?</span> 
                        </div>
                        <h2 id="gameTitle">
                            Game
                        </h2>
                    </div>
                </div>
                <div id="list">
                </div>
            </div>
        )
    }
}