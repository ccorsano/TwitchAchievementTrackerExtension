import * as React from "react"
import * as moment from 'moment';

interface CountDownProps {
    targetDate: Date,
}

export default class CountDown extends React.Component<CountDownProps, any>
{
    tickInterval: NodeJS.Timeout = null;

    constructor(props: CountDownProps)
    {
        super(props);
    }

    componentDidMount = () => {
        this.tickInterval = setInterval(this.tick, 1000);
    }

    componentWillUnmount = () => {
        clearInterval(this.tickInterval);
        this.tickInterval = null;
    }

    tick = () => {
        this.forceUpdate();
    }

    render(){
        let targetMoment = moment(this.props.targetDate);
        let diffToTarget = targetMoment.diff(moment.utc());
        let timeToTarget = moment.utc(diffToTarget);

        return (
            <div>
                <span className="rounded">{timeToTarget.format("HH")}</span>:<span className="rounded">{timeToTarget.format("mm")}</span>:<span className="rounded">{timeToTarget.format("ss")}</span>
            </div>
        )
    }
}