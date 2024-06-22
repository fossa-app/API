<#
.Synopsis
    Build script

.Description
    TASKS AND REQUIREMENTS
    Initialize and Clean repository
    Restore packages, workflows, tools
    Format code
    Build projects and the solution
    Run Tests
    Pack
    Publish
#>

[System.Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSReviewUnusedParameter', '', Justification = 'Parameter is used actually.')]
param(
    # Build Version
    [Parameter()]
    [string]
    $Version,
    # Build Instance
    [Parameter()]
    [string]
    $Instance
)

Set-StrictMode -Version Latest


# Synopsis: Estimate Next Version
Task EstimateVersion Restore, {
    $state = Import-Clixml -Path ".\.trash\$Instance\state.clixml"
    if ($Version) {
        $state.NextVersion = [System.Management.Automation.SemanticVersion]$Version
    }
    else {
        $gitversion = Exec { dotnet tool run dotnet-gitversion } | ConvertFrom-Json
        $state.NextVersion = [System.Management.Automation.SemanticVersion]::Parse($gitversion.SemVer)
    }

    $state | Export-Clixml -Path ".\.trash\$Instance\state.clixml"
    Write-Output "Next version estimated to be $($state.NextVersion)"
    Write-Output $state
}

# Synopsis: Format
Task Format Restore, FormatXmlFiles, FormatWhitespace, FormatStyle, FormatAnalyzers

# Synopsis: Format Analyzers
Task FormatAnalyzers Restore, FormatAnalyzersSharedKernel, FormatAnalyzersPersistence, FormatAnalyzersCore, FormatAnalyzersInfrastructure, FormatAnalyzersSolution

# Synopsis: Format Analyzers Solution
Task FormatAnalyzersSolution Restore, {
    # $solution = Resolve-Path -Path 'API.sln'
    # Exec { dotnet format analyzers --severity info --verbosity diagnostic $solution }
}

# Synopsis: Format Analyzers Infrastructure
Task FormatAnalyzersInfrastructure Restore, {
    $project = Resolve-Path -Path 'src/API.Infrastructure/API.Infrastructure.csproj'

    Exec { dotnet format analyzers --severity info --verbosity diagnostic $project }
}

# Synopsis: Format Analyzers Persistence
Task FormatAnalyzersPersistence Restore, {
    $project = Resolve-Path -Path 'src/API.Persistence/API.Persistence.csproj'

    Exec { dotnet format analyzers --severity info --verbosity diagnostic $project }
}

# Synopsis: Format Analyzers Core
Task FormatAnalyzersCore Restore, {
    $project = Resolve-Path -Path 'src/API.Core/API.Core.csproj'

    Exec { dotnet format analyzers --severity info --verbosity diagnostic $project }
}

# Synopsis: Format Analyzers Shared Kernel
Task FormatAnalyzersSharedKernel Restore, {
    $project = Resolve-Path -Path 'src/API.SharedKernel/API.SharedKernel.csproj'

    Exec { dotnet format analyzers --severity info --verbosity diagnostic $project }
}

# Synopsis: Format Style
Task FormatStyle Restore, FormatStyleSharedKernel, FormatStylePersistence, FormatStyleCore, FormatStyleInfrastructure, FormatStyleSolution

# Synopsis: Format Style Solution
Task FormatStyleSolution Restore, {
    # $solution = Resolve-Path -Path 'API.sln'
    # Exec { dotnet format style --severity info --verbosity diagnostic $solution }
}

# Synopsis: Format Style Persistence
Task FormatStylePersistence Restore, {
    $project = Resolve-Path -Path 'src/API.Persistence/API.Persistence.csproj'

    Exec { dotnet format style --severity info --verbosity diagnostic $project }
}

# Synopsis: Format Style Infrastructure
Task FormatStyleInfrastructure Restore, {
    $project = Resolve-Path -Path 'src/API.Infrastructure/API.Infrastructure.csproj'

    Exec { dotnet format style --severity info --verbosity diagnostic $project }
}

# Synopsis: Format Style Core
Task FormatStyleCore Restore, {
    $project = Resolve-Path -Path 'src/API.Core/API.Core.csproj'

    Exec { dotnet format style --severity info --verbosity diagnostic $project }
}

# Synopsis: Format Style Shared Kernel
Task FormatStyleSharedKernel Restore, {
    $project = Resolve-Path -Path 'src/API.SharedKernel/API.SharedKernel.csproj'

    Exec { dotnet format style --severity info --verbosity diagnostic $project }
}

# Synopsis: Format Whitespace
Task FormatWhitespace Restore, {
    $solution = Resolve-Path -Path 'API.sln'
    Exec { dotnet format whitespace --verbosity diagnostic $solution }
}

# Synopsis: Format XML Files
Task FormatXmlFiles Clean, {
    Get-ChildItem -Include *.xml, *.config, *.props, *.targets, *.nuspec, *.resx, *.ruleset, *.vsixmanifest, *.vsct, *.xlf -Recurse -File
    | Where-Object { -not (git check-ignore $PSItem) }
    | ForEach-Object {
        Write-Output "Formatting XML File: $PSItem"
        $content = Get-Content -Path $PSItem -Raw
        $xml = [xml]$content
        $xml.Save($PSItem)
    }
}

# Synopsis: Scan with DevSkim for security issues
Task DevSkim Restore, {
    $state = Import-Clixml -Path ".\.trash\$Instance\state.clixml"
    $trashFolder = $state.TrashFolder
    $sarifFile = Join-Path -Path $trashFolder -ChildPath 'DevSkim.sarif'
    Exec { dotnet tool run devskim analyze --source-code . --output-file $sarifFile }
    Exec { dotnet tool run devskim fix --source-code . --sarif-result $sarifFile --all }
}

# Synopsis: Restore
Task Restore RestoreWorkloads, RestoreTools, RestorePackages

# Synopsis: Restore workloads
Task RestoreWorkloads Clean, {
    Exec { dotnet workload restore }
}

# Synopsis: Restore tools
Task RestoreTools Clean, {
    Exec { dotnet tool restore }
}

# Synopsis: Restore packages
Task RestorePackages Clean, {
    $solution = Resolve-Path -Path 'API.sln'
    Exec { dotnet restore $solution }
}

# Synopsis: Clean previous build leftovers
Task Clean Init, {
    Get-ChildItem -Directory
    | Where-Object { -not $_.Name.StartsWith('.') }
    | ForEach-Object { Get-ChildItem -Path $_ -Recurse -Directory }
    | Where-Object { ( $_.Name -eq 'bin') -or ( $_.Name -eq 'obj') }
    | ForEach-Object { Remove-Item -Path $_ -Recurse -Force }
}

# Synopsis: Initialize folders and variables
Task Init {
    $trashFolder = Join-Path -Path . -ChildPath '.trash'
    $trashFolder = Join-Path -Path $trashFolder -ChildPath $Instance
    New-Item -Path $trashFolder -ItemType Directory | Out-Null
    $trashFolder = Resolve-Path -Path $trashFolder

    $buildArtifactsFolder = Join-Path -Path $trashFolder -ChildPath 'artifacts'
    New-Item -Path $buildArtifactsFolder -ItemType Directory | Out-Null

    $anyBuildArtifactsFolder = Join-Path -Path $buildArtifactsFolder -ChildPath 'any'
    New-Item -Path $anyBuildArtifactsFolder -ItemType Directory | Out-Null

    $state = [PSCustomObject]@{
        NextVersion             = $null
        TrashFolder             = $trashFolder
        BuildArtifactsFolder    = $buildArtifactsFolder
        AnyBuildArtifactsFolder = $anyBuildArtifactsFolder
    }

    $state | Export-Clixml -Path ".\.trash\$Instance\state.clixml"
    Write-Output $state
}
