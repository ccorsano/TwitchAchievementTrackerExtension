import * as React from 'react';

export type ConfigStepBaseProps = {
    onValid: (e: React.Component, nextState: any) => void,
    nextState: any,
}

export class ConfigStepBase<PropsType extends ConfigStepBaseProps, StateType> extends React.Component<PropsType, StateType> {
    constructor(props: PropsType){
        super(props);
        this.onValid = this.onValid.bind(this);
    }

    onValid = this.props.onValid;
}