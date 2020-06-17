export default class ServerConfig {
    constructor(){
    }

    setHost(host) {
        this.state.host = host;
    }

    host() {
        return this.state.host;
    }

    uri(path) {
        return URL.new(path, host());
    }
}