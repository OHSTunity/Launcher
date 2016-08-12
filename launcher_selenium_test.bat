:: Checking if we are not running on build server.
IF NOT "%SC_RUNNING_ON_BUILD_SERVER%"=="True" (
    staradmin stop db default
    msbuild src\Launcher\Launcher.csproj
    msbuild test\Launcher_AcceptanceTest\Launcher_AcceptanceTest.csproj
    msbuild test\Launcher_AcceptanceHelperOne\Launcher_AcceptanceHelperOne.csproj
    msbuild test\Launcher_AcceptanceHelperTwo\Launcher_AcceptanceHelperTwo.csproj
)

IF "%Configuration%"=="" set Configuration=Debug

star --resourcedir="%~dp0src\Launcher\wwwroot" "%~dp0bin/%Configuration%/Launcher.exe"
IF ERRORLEVEL 1 EXIT /b 1

star --resourcedir="%~dp0test\Launcher_AcceptanceHelperOne\wwwroot" "%~dp0test/Launcher_AcceptanceHelperOne/bin/%Configuration%/Launcher_AcceptanceHelperOne.exe"
IF ERRORLEVEL 1 EXIT /b 1

star --resourcedir="%~dp0test\Launcher_AcceptanceHelperTwo\wwwroot" "%~dp0test/Launcher_AcceptanceHelperTwo/bin/%Configuration%/Launcher_AcceptanceHelperTwo.exe"
IF ERRORLEVEL 1 EXIT /b 1

packages\NUnit.ConsoleRunner.3.2.0\tools\nunit3-console.exe test\Launcher_AcceptanceTest\Launcher_AcceptanceTest.csproj /config:%Configuration%
IF ERRORLEVEL 1 EXIT /b 1
