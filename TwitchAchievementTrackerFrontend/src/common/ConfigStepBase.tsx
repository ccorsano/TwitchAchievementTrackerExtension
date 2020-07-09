import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { ExtensionConfiguration, SteamConfiguration, XApiConfiguration } from './EBSTypes';

export enum ConfigSteamConfigStateEnum {
    WebApiKey = 0,
    SteamProfileSetup = 1,
    SteamGameSearch = 2,
    Locale = 3,
    Confirm = 4,
}

export enum ConfigXBLConfigStateEnum {
    XApiUsKey = 0,
    XUIDSearch = 1,
    TitleIdSearch = 2,
    Locale = 3,
    Confirm = 4,
}

export type ConfigStepBaseProps = {
    onValidate: (e: React.Component, configuration: ExtensionConfiguration) => void,
    onBack: (e: any) => void,
    savedConfiguration: ExtensionConfiguration,
}

export type  ConfigStepBaseState = {
    isValid: boolean;
}

export class ConfigStepBase<PropsType extends ConfigStepBaseProps, StateType> extends React.Component<PropsType, StateType> {
    constructor(props: PropsType){
        super(props);
        this.onBack = this.onBack.bind(this);
    }

    onBack = (e: React.MouseEvent<HTMLElement>) => this.props.onBack(e);
}