Write-Host "DEPLOY"

$MyDir = Split-Path $MyInvocation.MyCommand.Definition
Import-Module $MyDir"\PowerUps.Deployment.ps1" -Force

$ServiceExecutable = $OctopusParameters["Octopus.Action.Package.CustomInstallationDirectory"]+'\PowerUps.MicroServices.PushoverFacade.exe'

Install-TopShelfService -ExePath $ServiceExecutable
