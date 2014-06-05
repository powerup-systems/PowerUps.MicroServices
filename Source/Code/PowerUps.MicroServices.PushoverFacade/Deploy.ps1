Write-Host "DEPLOY"

$MyDir = Split-Path $MyInvocation.MyCommand.Definition
Import-Module $MyDir"\CiPsLib.Common.psm1" -Force
Import-Module $MyDir"\CiPsLib.TopShelf.psm1" -Force

$ServiceExecutable = $OctopusParameters["Octopus.Action.Package.CustomInstallationDirectory"]+'\PowerUps.MicroServices.PushoverFacade.exe'

Install-TopShelfService -ExePath $ServiceExecutable
