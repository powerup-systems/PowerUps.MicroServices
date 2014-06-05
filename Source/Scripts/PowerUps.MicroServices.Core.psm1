$MyDir = Split-Path $MyInvocation.MyCommand.Definition
Import-Module $MyDir"\CiPsLib.Common.psm1" -Force
Import-Module $MyDir"\CiPsLib.TopShelf.psm1" -Force


function PreDeploy-MicroServicesDefault {
    $ServiceName = $OctopusParameters["svc.name"]
    $ServiceExecutable = $OctopusParameters["Octopus.Action.Package.CustomInstallationDirectory"]+'\'+$OctopusParameters["app.exeName"]
    $AppConfigPath = $OctopusOriginalPackageDirectoryPath+'\'+$OctopusParameters["app.exeName"]+'.config'

    # Print configuration
    Write-Host $ServiceName
    Write-Host $ServiceExecutable
    Write-Host $AppConfigPath

    Configure-MicroServicesDefault -ConfigPath $AppConfigPath

    # Install service
    Uninstall-TopShelfService -ServiceName $ServiceName -ExePath $ServiceExecutable

    # Wait a bit to let sevice uninstall finish
    Start-Sleep -Seconds 10
}


function PostDeploy-MicroServicesDefault {
    param
    (
        [bool] $EfMigrate
    )

    if ($EfMigrate) {
        Set-Location $OctopusParameters["Octopus.Action.Package.CustomInstallationDirectory"] -PassThru | Write-Host
        $MigrateExe = $OctopusParameters["Octopus.Action.Package.CustomInstallationDirectory"]+'\migrate.exe'
		$AppConfig = $OctopusParameters["app.exeName"]+'.config'

		Write-Host $MigrateExe
		Write-Host $AppConfig

        & $MigrateExe $OctopusParameters["app.exeName"] /startupConfigurationFile=$AppConfig /verbose | Write-Host
        if ($LastExitCode -ne 0)
        {
            throw "Error: Failed to execute migrations"
        }
    }
}


function Deploy-MicroServicesDefault {
    $ServiceExecutable = $OctopusParameters["Octopus.Action.Package.CustomInstallationDirectory"]+'\'+$OctopusParameters["app.exeName"]

    Install-TopShelfService -ExePath $ServiceExecutable
}


function Configure-MicroServicesDefault {
    param
    (
        [string] $ConfigPath
    )

    #Connection strings
    XmlPoke $AppConfigPath "//*[local-name() = 'add' and @name='RabbitMqConnectionString']/@connectionString" $OctopusParameters["mq.connectionstring"]
    XmlPoke $AppConfigPath "//*[local-name() = 'add' and @name='MsSqlConnectionString']/@connectionString" $OctopusParameters["sql.connectionstring"]

    # Application configuration
    XmlPoke $AppConfigPath "//*[local-name() = 'add' and @key='IMicroServicesCoreConfiguration_ServiceName']/@value" $OctopusParameters["svc.name"]
    XmlPoke $AppConfigPath "//*[local-name() = 'add' and @key='IMicroServicesCoreConfiguration_ServiceDescription']/@value" $OctopusParameters["svc.description"]
    XmlPoke $AppConfigPath "//*[local-name() = 'add' and @key='IMicroServicesCoreConfiguration_Port']/@value" $OctopusParameters["conf.port"]
    XmlPoke $AppConfigPath "//*[local-name() = 'add' and @key='IMicroServicesCoreConfiguration_RabbitMqExchangeName']/@value" $OctopusParameters["mq.exchange"]
    XmlPoke $AppConfigPath "//*[local-name() = 'add' and @key='IMicroServicesCoreConfiguration_LogLevel']/@value" $OctopusParameters["log.level"]
    XmlPoke $AppConfigPath "//*[local-name() = 'add' and @key='IMicroServicesCoreConfiguration_ElasticSearchUrl']/@value" $OctopusParameters["es.url"]
    XmlPoke $AppConfigPath "//*[local-name() = 'add' and @key='IMicroServicesCoreConfiguration_EnvName']/@value" $OctopusParameters["env.name"]
    XmlPoke $AppConfigPath "//*[local-name() = 'add' and @key='IMicroServicesCoreConfiguration_AppName']/@value" $OctopusParameters["app.shortName"]
}


Export-ModuleMember -function * -alias *

Write-Host 'Imported PowerUps.MicroServices.Core.psm1'

