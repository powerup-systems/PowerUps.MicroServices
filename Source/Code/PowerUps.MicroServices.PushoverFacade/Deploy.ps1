Write-Host "DEPLOY"

. .\PowerUps.Deployment.ps1

$ServiceExecutable = $OctopusParameters["Octopus.Action.Package.CustomInstallationDirectory"]+'\PowerUps.MicroServices.PushoverFacade.exe'

Install-TopShelfService -ExePath $ServiceExecutable
