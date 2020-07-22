import * as React from 'react';
import dayjs from 'dayjs';
import duration from 'dayjs/plugin/duration';

dayjs.extend(duration);

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

        let targetMoment = dayjs(this.props.targetDate);
        let diffToTarget = targetMoment.diff(dayjs());
        let timeToTarget = dayjs.duration(diffToTarget);

        return (
            <div>
                <span className="rounded">{timeToTarget.hours()}</span>:<span className="rounded">{timeToTarget.minutes()}</span>:<span className="rounded">{timeToTarget.seconds()}</span>
            </div>
        )
    }
}