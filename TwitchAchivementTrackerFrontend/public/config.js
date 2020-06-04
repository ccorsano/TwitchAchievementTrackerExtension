let token, userId;
var server = "twitchext.conceptoire.com"

var urlParams = new URLSearchParams(this.location.search);
if (urlParams.get('state') == "testing")
{
    server = "localhost:8081"
}

const twitch = window.Twitch.ext;

twitch.onContext((context) => {
  twitch.rig.log(context);
});

twitch.onAuthorized((auth) => {
  token = auth.token;
  userId = auth.userId;
  
  if (twitch.configuration.broadcaster)
  {
    var request = {
      type: 'GET',
      url: location.protocol + '//' + server + '/api/configuration',
      success: onGetConfig,
      error: logError,
      accept: 'application/json',
      headers: {
        'Authorization': 'Bearer ' + token,
        'X-Config-Token': twitch.configuration.broadcaster.content,
      }
    };
    $.ajax(request);
  }
});

function createConfigRequest(type, method, callback) {
    twitch.rig.log('Create request ' + location.protocol + '//' + server + '/api/configuration/' + method);
    return {
        type: type,
        url: location.protocol + '//' + server + '/api/configuration/' + method,
        success: callback,
        dataType: 'json',
        error: logError
    }
}

function logError(_, error, status) {
  twitch.rig.log('EBS request returned '+status+' ('+error+')');
}

function onGetConfig(config)
{
  $('input[name=xapiKey]').val(config.xApiKey)
  $('input[name=streamerXuid]').val(config.streamerXuid)
  $('input[name=titleId]').val(config.titleId)
  $('input[name=locale]').val(config.locale)
}

function onPackConfig(config)
{
  twitch.configuration.set('broadcaster', '0.0.1', config.configToken);
}

window.onload = function()
{
  $('#saveConfig').click(function()
  {
    var config = {
      'xApiKey': 'a5c19b0eef68641675ce55579704b030a1ce6026',
      'streamerXuid': '2535467661815558',
      'titleId': '1659804324',
      'locale': 'fr-FR',
    };
    var request = {
      type: 'POST',
      url: location.protocol + '//' + server + '/api/configuration',
      success: onPackConfig,
      error: logError,
      data: JSON.stringify(config),
      contentType : 'application/json; charset=UTF-8',
      headers: {
        'Authorization': 'Bearer ' + token
      }
    };
    $.ajax(request);
  });
}