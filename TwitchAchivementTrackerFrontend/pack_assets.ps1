$timeStamp = Get-Date -Format "yyyyMMddHHmmss"
Get-ChildItem -Path $PSScriptRoot/public | Compress-Archive -DestinationPath $PSScriptRoot/assets-$timeStamp.zip