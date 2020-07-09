import * as React from 'react';
import * as ReactDOM from 'react-dom';

export type ConfigStepBaseProps = {
    onValid: (e: React.Component, nextState: any) => void,
    onBack: (previousState: any) => void,
    nextState: any,
    previousState: any,
}

export type  ConfigStepBaseState = {
    isValid: boolean;
}

export class ConfigStepBase<PropsType extends ConfigStepBaseProps, StateType extends ConfigStepBaseState> extends React.Component<PropsType, StateType> {
    constructor(props: PropsType){
        super(props);
        this.onValid = this.onValid.bind(this);
        this.onBack = this.onBack.bind(this);
    }

    onValid = this.props.onValid;
    onBack = (e: React.MouseEvent<HTMLElement>) => this.props.onBack(this.props.previousState);
}