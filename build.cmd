rem @echo off

powershell -Command "if ((Test-Path 'Source\Solutions\.nuget\NuGet.exe') -eq $false) {(New-Object System.Net.WebClient).DownloadFile('http://nuget.org/nuget.exe', 'Source\Solutions\.nuget\NuGet.exe')}"

"Source\Solutions\.nuget\NuGet.exe" "install" "FAKE.Core" "-OutputDirectory" "Tools" "-ExcludeVersion" "-version" "2.13.1"
"Source\Solutions\.nuget\NuGet.exe" "install" "NUnit.Runners" "-OutputDirectory" "Tools" "-ExcludeVersion" "-version" "2.6.3"

"Source\Solutions\.nuget\NuGet.exe" restore .\Source\Solutions\PowerUps.MicroServices.sln
"Source\Solutions\.nuget\NuGet.exe" restore .\Modules\Blocks\Source\Blocks.sln

"Tools\FAKE.Core\tools\Fake.exe" "build.fsx" "nugetApikey=%NUGET_APIKEY%" "buildVersion=%BUILD_VERSION%"

exit /b %errorlevel%
