Write-Host "PRE-DEPLOY"

$MyDir = Split-Path $MyInvocation.MyCommand.Definition
Import-Module $MyDir"\PowerUps.MicroServices.Core.psm1" -Force

PreDeploy-MicroServicesDefault -UseRabbitMQ $true -UseMsSql $false
