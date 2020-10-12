var server = "https://twitchext.conceptoire.com/v4"

var urlParams = new URLSearchParams(window.location.search);
if (urlParams.get('state') == "testing")
{
    server = "http://localhost:8081"
}


export const EBSVersion = "2020.5";
export const EBSBaseUrl = server;