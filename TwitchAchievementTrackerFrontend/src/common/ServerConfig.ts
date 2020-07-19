var server = "https://twitchext.conceptoire.com/v4"
var intervalTimer = false;

var urlParams = new URLSearchParams(window.location.search);
if (urlParams.get('state') == "testing")
{
    server = "http://localhost:8081"
}


export const EBSVersion = "2020.4";
export const EBSBaseUrl = server;