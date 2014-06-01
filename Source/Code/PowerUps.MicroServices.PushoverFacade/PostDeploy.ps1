﻿Write-Host 'POST-DEPLOY'

$MyDir = Split-Path $MyInvocation.MyCommand.Definition
Import-Module $MyDir"\PowerUps.Deployment.ps1" -Force

Set-Location $OctopusParameters["Octopus.Action.Package.CustomInstallationDirectory"] -PassThru | Write-Host
$MigrateExe = $OctopusParameters["Octopus.Action.Package.CustomInstallationDirectory"]+'\migrate.exe'
& $MigrateExe PowerUps.MicroServices.PushoverFacade.exe /startupConfigurationFile="PowerUps.MicroServices.PushoverFacade.exe.config" /verbose | Write-Host
if ($LastExitCode -ne 0)
{
    throw "Error: Failed to execute migrations"
}
