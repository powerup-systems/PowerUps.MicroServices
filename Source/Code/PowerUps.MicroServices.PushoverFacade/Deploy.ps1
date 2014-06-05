Write-Host "DEPLOY"

$MyDir = Split-Path $MyInvocation.MyCommand.Definition
Import-Module $MyDir"\CiPsLib.Common.psm1" -Force
Import-Module $MyDir"\CiPsLib.TopShelf.psm1" -Force
Import-Module $MyDir"\PowerUps.MicroServices.Core.psm1" -Force

Deploy-MicroServicesDefault
