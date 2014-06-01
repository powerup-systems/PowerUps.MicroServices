
function Install-TopShelfService {
    param
    (
        [string] $ExePath
    )
    Write-Host "Installing service"
    & $ExePath install | Write-Host
    if ($LastExitCode -ne 0) { throw 'Failed to install service' }

    Write-Host "Starting service"
    & $ExePath start | Write-Host
    if ($LastExitCode -ne 0) { throw 'Failed to start service' }    
}


function Create-EventSource {
    param
    (
        [string] $EventSourceName
    )
    if ([System.Diagnostics.EventLog]::SourceExists($EventSourceName) -eq $false)
    {
        Write-Host 'Creating event source'
        [System.Diagnostics.EventLog]::CreateEventSource($EventSourceName, "Application")
    }
    else
    {
        Write-Host 'Event source already created'
    }
}


function Uninstall-TopShelfService {
    param
    (
        [string] $ServiceName,
        [string] $ExePath
    )
    $Service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
    if ($Service) {
        Write-Host 'Stopping and uninstalling service'
        & $ExePath stop | Write-Host
        & $ExePath uninstall | Write-Host
    }
    else
    {
        Write-Host 'Nothing to uninstall'
    }
}


function XmlPoke {
    param
    (
        [string] $FilePath,
        [string] $XPath,
        [string] $Value,
        [switch] $Verbose
    )
    if ($Verbose) {
        $VerbosePreference = 'Continue'
    }
    [xml] $xml = Get-Content $FilePath -Encoding UTF8
        $FoundNode = $false
    $xml.SelectNodes($XPath) | ForEach-Object {
                Write-Verbose "XmlPoke: Changing '$XPath' to '$Value'"
        $_.InnerText = $Value
                $FoundNode = $true
        }
        if ($FoundNode -eq $true) {
            $xml.Save($FilePath)
        } else {
                throw "XmlPoke ERROR: Failed to find any nodes with XPath '$XPath'"
        }
}

Write-Host "PowerUps.Deployment.ps1 included"
