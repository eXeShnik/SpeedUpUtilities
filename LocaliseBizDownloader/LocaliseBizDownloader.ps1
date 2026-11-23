#Requires -Version 7.0
<#
.SYNOPSIS
    Downloads .resx translations from localise.biz (Loco), skipping locales that have not changed.

.DESCRIPTION
    Exports every project locale as Strings.<code>.resx; the native locale is saved as Strings.resx.
    ETag / Last-Modified values from previous exports are kept in .loco-state.json inside the
    output folder and sent back as If-None-Match / If-Modified-Since, so unchanged locales get
    304 Not Modified and their files are left untouched.
    Paths of updated files are written to the output stream; progress goes to the host.

.EXAMPLE
    ./LocaliseBizDownloader.ps1 -f ./Resources/Strings -k <API_KEY>

.EXAMPLE
    ./LocaliseBizDownloader.ps1 -Folder ./Resources/Strings -Key <API_KEY> -Force
#>
[CmdletBinding()]
param(
    [Alias('f')]
    [string]$Folder = (Join-Path (Get-Location) 'Translations'),

    [Alias('k')]
    [Parameter(Mandatory)]
    [string]$Key,

    # Re-download every locale, ignoring stored ETag / Last-Modified values.
    [switch]$Force
)

$ErrorActionPreference = 'Stop'

$baseUrl = 'https://localise.biz/api'
$authHeaders = @{ Authorization = "Loco $Key" }
$statePath = Join-Path $Folder '.loco-state.json'

New-Item -ItemType Directory -Path $Folder -Force | Out-Null

$state = @{}
if (-not $Force -and (Test-Path $statePath)) {
    $state = Get-Content $statePath -Raw | ConvertFrom-Json -AsHashtable
}

$locales = Invoke-RestMethod -Uri "$baseUrl/locales" -Headers $authHeaders
$updatedCount = 0

# Sequential on purpose: Loco answers parallel requests with 429.
foreach ($locale in $locales) {
    $fileName = $locale.native ? 'Strings.resx' : "Strings.$($locale.code).resx"
    $target = Join-Path $Folder $fileName

    $requestHeaders = $authHeaders.Clone()
    $previous = $state[$fileName]
    if ($previous -and (Test-Path $target)) {
        if ($previous.etag) { $requestHeaders['If-None-Match'] = $previous.etag }
        if ($previous.lastModified) { $requestHeaders['If-Modified-Since'] = $previous.lastModified }
    }

    $response = Invoke-WebRequest -Uri "$baseUrl/export/locale/$($locale.code).resx?index=id" `
        -Headers $requestHeaders -SkipHttpErrorCheck

    if ($response.StatusCode -eq 304) {
        Write-Host "$($locale.code): up to date"
        continue
    }
    if ($response.StatusCode -ne 200) {
        throw "$($locale.code): export failed with HTTP $($response.StatusCode)"
    }

    [System.IO.File]::WriteAllBytes($target, $response.RawContentStream.ToArray())
    $state[$fileName] = @{
        etag         = $response.Headers['ETag'] | Select-Object -First 1
        lastModified = $response.Headers['Last-Modified'] | Select-Object -First 1
    }
    $updatedCount++
    Write-Host "$($locale.code): updated -> $fileName"
    Write-Output $target
}

$state | ConvertTo-Json | Set-Content -Path $statePath -Encoding utf8

Write-Host "$updatedCount of $(@($locales).Count) locale(s) updated."
