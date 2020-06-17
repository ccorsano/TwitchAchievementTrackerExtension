
/**
 * Helper class for authentication against an EBS service. Allows the storage of a token to be accessed across componenents. 
 * This is not meant to be a source of truth. Use only for presentational purposes. 
 */
export default class Configuration{

    constructor(serverConfig){
        this.state = {
            serverConfig : serverConfig
        };
    }

    setToken(token,opaque_id){
        this.state.token = token;
        this.state.opaque_id = opaque_id;
    }

    // checks to ensure there is a valid token in the state
    isAuthenticated(){
        if(this.state.token && this.state.opaque_id){
            return true
        }else{
            return false
        }
    }

    searchXboxTitle(search_str) {
        this.makeCall(this.state.server.uri("/api/title/search/"));
    }

    /**
     * Makes a call against a given endpoint using a specific method. 
     * 
     * Returns a Promise with the Request() object per fetch documentation.
     * 
     */

    makeCall(url, method="GET"){
        return new Promise((resolve, reject)=>{
            if(this.isAuthenticated()){
                let headers={
                    'Content-Type':'application/json',
                    'Authorization': `Bearer ${this.state.token}`
                }
    
                fetch(url,
                    {
                        method,
                        headers,
                    })
                    .then(response=>resolve(response))
                    .catch(e=>reject(e))
            }else{
                reject('Unauthorized')
            }
        })
    }
}