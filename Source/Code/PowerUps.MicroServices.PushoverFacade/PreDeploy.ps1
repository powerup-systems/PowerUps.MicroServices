Write-Host "PRE-DEPLOY"

$MyDir = Split-Path $MyInvocation.MyCommand.Definition
Import-Module $MyDir"\PowerUps.Deployment.ps1" -Force

$ServiceName = $OctopusParameters["svc.name"]
$ServiceExecutable = $OctopusParameters["Octopus.Action.Package.CustomInstallationDirectory"]+'\PowerUps.MicroServices.PushoverFacade.exe'
$AppConfigPath = $OctopusOriginalPackageDirectoryPath+'\PowerUps.MicroServices.PushoverFacade.exe.config'

# Print configuration
Write-Host $ServiceName
Write-Host $ServiceExecutable
Write-Host $AppConfigPath

#Connection strings
XmlPoke $AppConfigPath "//*[local-name() = 'add' and @name='RabbitMqConnectionString']/@connectionString" $OctopusParameters["mq.connectionstring"]
XmlPoke $AppConfigPath "//*[local-name() = 'add' and @name='MsSqlConnectionString']/@connectionString" $OctopusParameters["sql.connectionstring"]

# Application configuration
XmlPoke $AppConfigPath "//*[local-name() = 'add' and @key='IPushoverFacadeConfiguration_ServiceName']/@value" $OctopusParameters["svc.name"]
XmlPoke $AppConfigPath "//*[local-name() = 'add' and @key='IPushoverFacadeConfiguration_ServiceDescription']/@value" $OctopusParameters["svc.description"]
XmlPoke $AppConfigPath "//*[local-name() = 'add' and @key='IPushoverFacadeConfiguration_Port']/@value" $OctopusParameters["conf.port"]
XmlPoke $AppConfigPath "//*[local-name() = 'add' and @key='IPushoverFacadeConfiguration_RabbitMqExchangeName']/@value" $OctopusParameters["mq.exchange"]
XmlPoke $AppConfigPath "//*[local-name() = 'add' and @key='IPushoverFacadeConfiguration_LogLevel']/@value" $OctopusParameters["log.level"]
XmlPoke $AppConfigPath "//*[local-name() = 'add' and @key='IPushoverFacadeConfiguration_ElasticSearchUrl']/@value" $OctopusParameters["es.url"]
XmlPoke $AppConfigPath "//*[local-name() = 'add' and @key='IPushoverFacadeConfiguration_EnvName']/@value" $OctopusParameters["env.name"]
XmlPoke $AppConfigPath "//*[local-name() = 'add' and @key='IPushoverFacadeConfiguration_AppName']/@value" $OctopusParameters["app.shortName"]

# Install service
Uninstall-TopShelfService -ServiceName $ServiceName -ExePath $ServiceExecutable

# Wait a bit to let sevice uninstall finish
Start-Sleep -Seconds 10
