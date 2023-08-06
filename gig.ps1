#For PowerShell v3
Function gig {
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$list
    )
    $params = ($list | ForEach-Object { [uri]::EscapeDataString($_) }) -join ','
    Invoke-WebRequest -Uri "https://www.toptal.com/developers/gitignore/api/$params" | Select-Object -ExpandProperty content | Out-File -FilePath $(Join-Path -Path $pwd -ChildPath '.gitignore') -Encoding ascii
}


gig -list visualstudio, visualstudiocode, rider, powershell, fsharp

Add-Content -Path .\.gitignore -Value '# Repository Specific' -Encoding ascii
Add-Content -Path .\.gitignore -Value '*.received.*' -Encoding ascii
Add-Content -Path .\.gitignore -Value '*.received/' -Encoding ascii
Add-Content -Path .\.gitignore -Value 'src/API.Web/database.sqlite' -Encoding ascii
Add-Content -Path .\.gitignore -Value 'src/API.Web/database.sqlite-shm' -Encoding ascii
Add-Content -Path .\.gitignore -Value 'src/API.Web/database.sqlite-wal' -Encoding ascii
Add-Content -Path .\.gitignore -Value 'src/API.Web/log*.txt' -Encoding ascii
