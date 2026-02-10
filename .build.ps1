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

# Synopsis: Publish Docker images
Task Publish Pack, {
    $state = Import-Clixml -Path ".\.trash\$Instance\state.clixml"
    $dockerImageVersionTag = $state.DockerImageVersionTag
    $dockerImageLatestTag = $state.DockerImageLatestTag
    $buildArtifactsFolder = $state.BuildArtifactsFolder
    $dockerImageMultiArchArchiveName = $state.DockerImageMultiArchArchiveName
    $dockerImageMultiArchArchive = Join-Path -Path $buildArtifactsFolder -ChildPath $dockerImageMultiArchArchiveName
    $dockerImageMultiArchArchive = Resolve-Path -Path $dockerImageMultiArchArchive

    Exec { docker image load --input $dockerImageMultiArchArchive }

    if ($null -eq $env:DOCKER_ACCESS_TOKEN) {
        Import-Module -Name Microsoft.PowerShell.SecretManagement
        $credential = Get-Secret -Name 'Fossa-DockerHub-Credential'
    }
    else {
        $securePassword = New-Object SecureString
        foreach ($char in $env:DOCKER_ACCESS_TOKEN.ToCharArray()) {
            $securePassword.AppendChar($char)
        }
        $credential = [PSCredential]::New('tiksn', $securePassword)
    }

    $username = $credential.UserName
    $password = $credential.GetNetworkCredential().Password

    Exec { docker login --username $username --password $password }
    Exec { docker push $dockerImageVersionTag }
    Exec { docker push $dockerImageLatestTag }
}

# Synopsis: Pack Docker image artifact
Task Pack Build, Test, Ward, {
    $state = Import-Clixml -Path ".\.trash\$Instance\state.clixml"
    $dockerImageName = $state.DockerImageName
    $nextVersion = $state.NextVersion
    $dockerFilePath = Resolve-Path -Path '.\src\API.Web\Dockerfile'
    $buildArtifactsFolder = $state.BuildArtifactsFolder

    $dockerImageVersionTag = "$($dockerImageName):$nextVersion"
    $dockerImageLatestTag = "$($dockerImageName):latest"

    $dockerImageMultiArchArchiveName = $state.DockerImageMultiArchArchiveName
    $dockerImageMultiArchArchive = Join-Path -Path $buildArtifactsFolder -ChildPath $dockerImageMultiArchArchiveName

    Exec { docker buildx build --platform 'linux/amd64,linux/arm64' --output "type=oci,dest=$dockerImageMultiArchArchive" --load --file $dockerFilePath --tag $dockerImageVersionTag --tag $dockerImageLatestTag . }

    $state.DockerImageVersionTag = $dockerImageVersionTag
    $state.DockerImageLatestTag = $dockerImageLatestTag

    $state | Export-Clixml -Path ".\.trash\$Instance\state.clixml"
    Write-Output $state
}

# Synopsis: Test
Task Test UnitTest, FunctionalTest, IntegrationTest

# Synopsis: Integration Test
Task IntegrationTest Build, {
    if (-not $env:CI) {
        Exec { dotnet test 'tests/API.IntegrationTests/API.IntegrationTests.csproj' }
    }
}

# Synopsis: Functional Test
Task FunctionalTest Build, {
    Exec { dotnet test 'tests/API.FunctionalTests/API.FunctionalTests.csproj' }
}

# Synopsis: Unit Test
Task UnitTest Build, {
    Exec { dotnet test 'tests/API.UnitTests/API.UnitTests.csproj' }
}

# Synopsis: Build
Task Build Format, BuildWeb, {
    $solution = Resolve-Path -Path 'API.slnx'
    Exec { dotnet build $solution }
}

# Synopsis: Build Web
Task BuildWeb EstimateVersion, {
    $state = Import-Clixml -Path ".\.trash\$Instance\state.clixml"
    $linuxX64BuildArtifactsFolder = $state.LinuxX64BuildArtifactsFolder
    $linuxArm64BuildArtifactsFolder = $state.LinuxArm64BuildArtifactsFolder
    $winX64BuildArtifactsFolder = $state.WinX64BuildArtifactsFolder
    $project = Resolve-Path -Path 'src/API.Web/API.Web.csproj'
    $nextVersion = $state.NextVersion

    Exec { dotnet build $project /v:m /p:Configuration=Release /p:version=$nextVersion /p:OutDir=$linuxX64BuildArtifactsFolder --runtime linux-x64 }
    Exec { dotnet build $project /v:m /p:Configuration=Release /p:version=$nextVersion /p:OutDir=$linuxArm64BuildArtifactsFolder --runtime linux-arm64 }
    Exec { dotnet build $project /v:m /p:Configuration=Release /p:version=$nextVersion /p:OutDir=$winX64BuildArtifactsFolder --runtime win-x64 }
}

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
Task FormatAnalyzers Restore, FormatAnalyzersPersistence, FormatAnalyzersCore, FormatAnalyzersInfrastructure, FormatAnalyzersWeb, FormatAnalyzersSolution

# Synopsis: Format Analyzers Solution
Task FormatAnalyzersSolution Restore, {
    $solution = Resolve-Path -Path 'API.slnx'
    Exec { dotnet format analyzers --severity info --verbosity diagnostic $solution }
}

# Synopsis: Format Analyzers Web
Task FormatAnalyzersWeb Restore, {
    $project = Resolve-Path -Path 'src/API.Web/API.Web.csproj'

    Exec { dotnet format analyzers --severity info --verbosity diagnostic $project }
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

# Synopsis: Format Style
Task FormatStyle Restore, FormatStylePersistence, FormatStyleCore, FormatStyleInfrastructure, FormatStyleWeb, FormatStyleSolution

# Synopsis: Format Style Solution
Task FormatStyleSolution Restore, {
    $solution = Resolve-Path -Path 'API.slnx'
    Exec { dotnet format style --severity info --verbosity diagnostic $solution }
}

# Synopsis: Format Style Web
Task FormatStyleWeb Restore, {
    $project = Resolve-Path -Path 'src/API.Web/API.Web.csproj'

    Exec { dotnet format style --severity info --verbosity diagnostic $project }
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

# Synopsis: Format Whitespace
Task FormatWhitespace Restore, {
    $solution = Resolve-Path -Path 'API.slnx'
    Exec { dotnet format whitespace --verbosity diagnostic $solution }
}

# Synopsis: Format XML Files
Task FormatXmlFiles Clean, {
    Get-ChildItem -Include *.xml, *.config, *.props, *.targets, *.nuspec, *.resx, *.ruleset, *.vsixmanifest, *.vsct, *.xlf, *.csproj, *.fsproj, *.vbproj, *.slnx -Recurse -File
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
Task RestorePackages Clean, EnsureCentralPackageVersions, {
    $solution = Resolve-Path -Path 'API.slnx'
    Exec { dotnet restore $solution }
}

# Synopsis: Store away public API contracts
Task Ward Init, {
    $state = Import-Clixml -Path ".\.trash\$Instance\state.clixml"
    $contractsArtifactsFolder = $state.ContractsArtifactsFolder

    $inputFiles = Get-ChildItem -Path *AssemblyHasNoPublicAPIChangesAsync.verified.txt -Recurse
    $outoutFiles = $inputFiles
    | ForEach-Object {
        $outputFileName = $PSItem.Name -replace '.txt', '.cs'

        $outputFilePath = Join-Path -Path $contractsArtifactsFolder -ChildPath $outputFileName

        Copy-Item -Path $PSItem -Destination $outputFilePath -Force
        Resolve-Path -Path $outputFilePath
    }

    $outoutFiles | Out-Host
}

# Synopsis: Ensure Central Package Versions compliance
Task EnsureCentralPackageVersions Clean, {

    $projectFiles = Get-ChildItem -Path . `
        -Recurse `
        -Include *.csproj, *.fsproj, *.vbproj `
        -File

    $violations = @()

    foreach ($projectFile in $projectFiles) {
        try {
            [xml]$xml = Get-Content $projectFile.FullName -Raw
        }
        catch {
            throw "Failed to parse XML: $($projectFile.FullName)"
        }

        $ns = New-Object System.Xml.XmlNamespaceManager($xml.NameTable)
        $ns.AddNamespace('msb', $xml.DocumentElement.NamespaceURI)

        $nodes = $xml.SelectNodes('//*[@VersionOverride]', $ns)

        foreach ($node in $nodes) {
            $violations += [PSCustomObject]@{
                File  = $projectFile.FullName
                Node  = $node.Name
                Value = $node.GetAttribute('VersionOverride')
            }
        }
    }

    if ($violations.Count -gt 0) {
        throw "VersionOverride attributes are not allowed. File: $($violations[0].File) Node: <$($violations[0].Node)>"
    }
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

    $contractsArtifactsFolder = Join-Path -Path $buildArtifactsFolder -ChildPath 'contracts'
    New-Item -Path $contractsArtifactsFolder -ItemType Directory | Out-Null

    $linuxX64BuildArtifactsFolder = Join-Path -Path $buildArtifactsFolder -ChildPath 'linux-x64'
    New-Item -Path $linuxX64BuildArtifactsFolder -ItemType Directory | Out-Null

    $linuxArm64BuildArtifactsFolder = Join-Path -Path $buildArtifactsFolder -ChildPath 'linux-arm64'
    New-Item -Path $linuxArm64BuildArtifactsFolder -ItemType Directory | Out-Null

    $winX64BuildArtifactsFolder = Join-Path -Path $buildArtifactsFolder -ChildPath 'win-x64'
    New-Item -Path $winX64BuildArtifactsFolder -ItemType Directory | Out-Null

    $state = [PSCustomObject]@{
        NextVersion                     = $null
        TrashFolder                     = $trashFolder
        BuildArtifactsFolder            = $buildArtifactsFolder
        ContractsArtifactsFolder        = $contractsArtifactsFolder
        LinuxX64BuildArtifactsFolder    = $linuxX64BuildArtifactsFolder
        LinuxArm64BuildArtifactsFolder  = $linuxArm64BuildArtifactsFolder
        WinX64BuildArtifactsFolder      = $winX64BuildArtifactsFolder
        DockerImageName                 = 'tiksn/fossa-api'
        DockerImageVersionTag           = $null
        DockerImageLatestTag            = $null
        DockerImageMultiArchArchiveName = 'fossa-api-multiarch.tar'
    }

    $state | Export-Clixml -Path ".\.trash\$Instance\state.clixml"
    Write-Output $state
}
