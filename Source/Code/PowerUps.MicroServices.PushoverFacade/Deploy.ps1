Write-Host "DEPLOY"

$MyDir = Split-Path $MyInvocation.MyCommand.Definition
Import-Module $MyDir"\PowerUps.MicroServices.Core.psm1" -Force

Deploy-MicroServicesDefault
