Write-Host 'POST-DEPLOY'

$MyDir = Split-Path $MyInvocation.MyCommand.Definition
Import-Module $MyDir+"\PowerUps.Deployment.ps1" -Force

