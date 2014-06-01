Write-Host "PRE-DEPLOY"

Import-Module "PowerUps.Deployment.ps1" -Force

$ServiceName = $OctopusParameters["svc.name"]
$ServiceExecutable = $OctopusParameters["Octopus.Action.Package.CustomInstallationDirectory"]+'\PowerUps.MicroServices.PushoverFacade.exe'
$AppConfigPath = $OctopusOriginalPackageDirectoryPath+'\PowerUps.MicroServices.PushoverFacade.config'

# Print configuration
Write-Host $ServiceName
Write-Host $ServiceExecutable
Write-Host $AppConfigPath

# Application configuration
XmlPoke $AppConfigPath "//*[local-name() = 'add' and @key='IPushoverFacadeConfiguration_ServiceName']/@value" $OctopusParameters["svc.name"]
XmlPoke $AppConfigPath "//*[local-name() = 'add' and @key='IPushoverFacadeConfiguration_ServiceDescription']/@value" $OctopusParameters["svc.description"]
XmlPoke $AppConfigPath "//*[local-name() = 'add' and @key='IPushoverFacadeConfiguration_Port']/@value" $OctopusParameters["conf.port"]
XmlPoke $AppConfigPath "//*[local-name() = 'add' and @key='IPushoverFacadeConfiguration_RabbitMqExchangeName']/@value" $OctopusParameters["mq.exchange"]
XmlPoke $AppConfigPath "//*[local-name() = 'add' and @key='RabbitMqConnectionString']/@value" $OctopusParameters["mq.connectionstring"]

# Log configuration
XmlPoke $AppConfigPath "/configuration/log4net/root/level/@value" $OctopusParameters["log.level"]
XmlPoke $AppConfigPath "/configuration/log4net/root/appender-ref/@ref" $OctopusParameters["log.appender"]
XmlPoke $AppConfigPath "/configuration/log4net/appender[@name='GelfUdpAppender']/layout/param[@name='AdditionalFields']/@value" $OctopusParameters["log.additionalFields"] -Verbose

# Install service
Uninstall-TopShelfService -ServiceName $ServiceName -ExePath $ServiceExecutable

# Wait a bit to let sevice uninstall finish
Start-Sleep -Seconds 10
