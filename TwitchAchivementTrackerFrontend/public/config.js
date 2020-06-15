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

function onSearchTitle(result)
{
  $('#searchTitle').prop('disabled', false);
  $('#gameQueryResult > ul').empty();
  result.forEach(titleInfo => {
    if (titleInfo.titleId)
    {
      listItem = document.createElement("li");
      $(listItem).text(titleInfo.titleId + ': ' + titleInfo.productTitle);
      $('#gameQueryResult > ul').append(listItem);
    }
  });
}

function onSearchSteamTitle(result)
{
  $('#searchSteamTitle').prop('disabled', false);
  $('#steamGameQueryResult > ul').empty();
  result.forEach(titleInfo => {
    if (titleInfo.titleId)
    {
      listItem = document.createElement("li");
      $(listItem).text(titleInfo.titleId + ': ' + titleInfo.productTitle);
      $('#steamGameQueryResult > ul').append(listItem);
    }
  });
}

function onGetConfig(config)
{
  if (config.xBoxLiveConfig)
  {
    $('input[name=xapiKey]').val(config.xBoxLiveConfig.xApiKey);
    $('input[name=streamerXuid]').val(config.xBoxLiveConfig.streamerXuid);
    $('input[name=titleId]').val(config.xBoxLiveConfig.titleId);
    $('input[name=locale]').val(config.xBoxLiveConfig.locale);
  }
  if (config.steamConfig)
  {
    $('input[name=steamWebApiKey]').val(config.steamConfig.webApiKey);
    $('input[name=streamerSteamId]').val(config.steamConfig.steamId);
    $('input[name=steamAppId]').val(config.steamConfig.appId);
    $('input[name=steamLocale]').val(config.steamConfig.locale);
  }
  if (config.activeConfig == 'Steam')
  {
    $('#collapse-steam').attr('checked', true);
  }
}

function xboxLiveConfig()
{
  return {
    'xApiKey': $('input[name=xapiKey]').val(),
    'streamerXuid': $('input[name=streamerXuid]').val(),
    'titleId': $('input[name=titleId]').val(),
    'locale': $('input[name=locale]').val(),
  };
}

function steamConfig()
{
  return {
    'webApiKey': $('input[name=steamWebApiKey]').val(),
    'steamId': $('input[name=streamerSteamId]').val(),
    'appId': $('input[name=steamAppId]').val(),
    'locale': $('input[name=steamLocale]').val(),
  };
}

function onPackConfig(config)
{
  twitch.configuration.set('broadcaster', '0.0.2', config.configToken);
}

function onResolveXuid(result)
{
  $('#resolveXuid').prop('disabled', false);
  $('#resolvedXuid').text(result);
}

window.onload = function()
{
  $('#resolveXuid').click(function()
  {
    $('#resolvedXuid').text('');

    $('#resolveXuid').prop('disabled', true);
    var reenableButton = function(result, error, status)
    {
      $('#resolveXuid').prop('disabled', false);
      logError(result, error, status);
    };

    var query = $('input[name=gamertag]').val();
    var request = {
      type: 'GET',
      url: location.protocol + '//' + server + '/api/xuid/' + encodeURIComponent(query),
      success: onResolveXuid,
      error: reenableButton,
      contentType : 'application/json; charset=UTF-8',
      headers: {
        'Authorization': 'Bearer ' + token
      }
    };
    $.ajax(request);
  });

  $('#searchTitle').click(function()
  {
    $('#gameQueryResult > ul').empty();
    
    $('#searchTitle').prop('disabled', true);
    var reenableButton = function(result, error, status)
    {
      $('#searchTitle').prop('disabled', false);
      logError(result, error, status);
    };

    var query = $('input[name=gameQuery]').val();
    var request = {
      type: 'GET',
      url: location.protocol + '//' + server + '/api/title/search/' + encodeURIComponent(query),
      success: onSearchTitle,
      error: reenableButton,
      contentType : 'application/json; charset=UTF-8',
      headers: {
        'Authorization': 'Bearer ' + token
      }
    };
    $.ajax(request);
  });
  $('#saveConfig').click(function()
  {
    var config = {
      'version': '0.0.1',
      'activeConfig': 'XBoxLive',
      'xBoxLiveConfig': xboxLiveConfig(),
      'steamConfig': steamConfig()
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
  
  $('#steamSearchTitle').click(function()
  {
    $('#steamGameQueryResult > ul').empty();
    
    $('#steamSearchTitle').prop('disabled', true);
    var reenableButton = function(result, error, status)
    {
      $('#steamSearchTitle').prop('disabled', false);
      logError(result, error, status);
    };

    var query = $('input[name=steamGameQuery]').val();
    var request = {
      type: 'GET',
      url: location.protocol + '//' + server + '/api/title/steam/search/' + encodeURIComponent(query),
      success: onSearchSteamTitle,
      error: reenableButton,
      contentType : 'application/json; charset=UTF-8',
      headers: {
        'Authorization': 'Bearer ' + token
      }
    };
    $.ajax(request);
  });

  $('#saveSteamConfig').click(function()
  {
    var config = {
      'version': '0.0.2',
      'activeConfig': 'Steam',
      'xBoxLiveConfig': xboxLiveConfig(),
      'steamConfig': steamConfig()
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

  $('#saveActiveConfig').click(function()
  {
    var checkedId = $('div.collapse > input[checked=checked]').attr('id')
  });
}