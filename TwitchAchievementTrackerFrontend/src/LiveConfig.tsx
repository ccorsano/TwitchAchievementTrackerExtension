import * as React from 'react';
import * as ReactDOM from 'react-dom';
import ConfigurationRoot from './components/ConfigurationRoot/ConfigurationRoot';
import '../public/mini-default.min.css';

ReactDOM.render(
    <ConfigurationRoot />,
    document.getElementById("root") as HTMLElement
);