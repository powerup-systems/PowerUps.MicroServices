Write-Host "DEPLOY"

Import-Module "PowerUps.Deployment.ps1" -Force

$ServiceExecutable = $OctopusParameters["Octopus.Action.Package.CustomInstallationDirectory"]+'\PowerUps.MicroServices.PushoverFacade.exe'

Install-TopShelfService -ExePath $ServiceExecutable
