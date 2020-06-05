&$PSScriptRoot/flatc --csharp -o $PSScriptRoot/../Model/Generated $PSScriptRoot/../Model/configurationToken.fbs

Push-Location $PSScriptRoot/../Model/Generated

$find = "using global::FlatBuffers;"
$replace = "using global::Google.FlatBuffers;"
foreach ($sourceFile in (Get-ChildItem -Recurse -Filter *.cs)) {
    $tmpFile = New-TemporaryFile
    (Get-Content -Path $sourceFile.FullName) -replace $find, $replace | Add-Content $tmpFile
    Remove-Item -Path $sourceFile.FullName
    Move-Item -Path $tmpFile -Destination $sourceFile.FullName
}

Pop-Location