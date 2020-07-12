import * as React from "react"
import * as moment from 'moment';

interface CountDownProps {
    targetDate: Date,
}

export default class CountDown extends React.Component<CountDownProps, any>
{
    constructor(props: CountDownProps)
    {
        super(props);
    }

    componentDidMount = () => {
        setInterval(this.tick, 1000);
    }

    tick = () => {
        this.forceUpdate();
    }

    render(){
        let targetMoment = moment(this.props.targetDate);
        let milliseconds = targetMoment.diff(moment.utc());
        let f = moment.utc(milliseconds).format("HH:mm:ss.SSS");

        return (
            <div>{f}</div>
        )
    }
}