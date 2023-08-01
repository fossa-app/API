
<#PSScriptInfo

.VERSION 1.0.0

.GUID 512d4dae-6eb0-4608-b466-919f1c1f47bf

.AUTHOR Tigran TIKSN Torosyan

.COMPANYNAME tiksn.com

.COPYRIGHT FossaApp 2023

.TAGS

.LICENSEURI https://github.com/fossa-app/API/blob/main/LICENSE

.PROJECTURI https://github.com/fossa-app/API/

.ICONURI

.EXTERNALMODULEDEPENDENCIES

.REQUIREDSCRIPTS

.EXTERNALSCRIPTDEPENDENCIES

.RELEASENOTES


.PRIVATEDATA

#>

#Requires -Module Microsoft.PowerShell.Management
#Requires -Module SecretManagement.Keybase

<#

.DESCRIPTION
 Set user secrets

#>
[CmdletBinding()]
param (
)

$connectionString = Get-Secret -Name 'FossaApp-ConnectionString' -AsPlainText

dotnet user-secrets --project .\src\API.Web\API.Web.csproj set "ConnectionStrings:MongoDB" $connectionString
