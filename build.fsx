#r @"tools\FAKE.Core\tools\FakeLib.dll"
open Fake 
open System

let buildMode = getBuildParamOrDefault "buildMode" "Release"
let buildVersion = getBuildParamOrDefault "buildVersion" "0.0.0.1"
let nugetApikey = getBuildParamOrDefault "nugetApikey" ""

let dirPackages = "./Deploy/Packages"
let dirReports = "./Reports"
let filePathUnitTestReport = dirReports + "/NUnit.xml"
let fileListUnitTests = !! ("**/bin/" @@ buildMode @@ "/PowerUps.MicroServices.Test.*.dll")
let toolNUnit = "./Tools/NUnit.Runners/tools"

Target "Clean" (fun _ -> 
    CleanDirs ["./Deploy/"; dirPackages; dirReports]
    )

Target "BuildApp" (fun _ ->
    MSBuild null "Build" ["Configuration", buildMode] ["./Source/Solutions/PowerUps.MicroServices.sln"]
    |> Log "AppBuild-Output: "
    )

Target "UnitTest" (fun _ ->
    fileListUnitTests
        |> NUnit (fun p -> 
            {p with
                DisableShadowCopy = true;
                Framework = "net-4.0";
                ToolPath = "./Tools/NUnit.Runners/tools";
                ToolName = "nunit-console-x86.exe";
                OutputFile = filePathUnitTestReport})
    )

Target "Default" DoNothing

"Clean"
    ==> "BuildApp"
//    ==> "UnitTest"
    ==> "Default"

RunTargetOrDefault "Default"
