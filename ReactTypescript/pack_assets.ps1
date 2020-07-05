npm run build
$timeStamp = Get-Date -Format "yyyyMMddHHmmss"
Get-ChildItem -Path $PSScriptRoot/dist | Compress-Archive -DestinationPath $PSScriptRoot/assets-$timeStamp.zip