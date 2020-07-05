import * as React from 'react';
import './VideoOverlay.scss';

export default class VideoOverlay extends React.Component {
    constructor(props: any){
        super(props);
    }

    render(){
        return (
            <div className="overlayBox collapsed">
                <div className="gameLogo noselect">
                    <div className="summaryWidget">
        
                    </div>
                </div>
                <div id="achievementsPanel">
                    <div className="card-container">
                        <h2 id="gameTitle">
                            Game
                        </h2>
                        <div id="completionHeadline">
                            <span className="percentage"></span><img src="nujacup.svg" alt="achievements" /><span className="completedCount">?</span>/<span className="totalCount">?</span> 
                        </div>
                    </div>
                    <div id="list">
                    </div>
                </div>
            </div>
        )
    }
}