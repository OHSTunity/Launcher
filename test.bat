star bin\debug\Launcher.exe
star --resourcedir=test\Launcher_AcceptanceHelperOne\wwwroot test\Launcher_AcceptanceHelperOne\bin\debug\Launcher_AcceptanceHelperOne.exe
star --resourcedir=test\Launcher_AcceptanceHelperTwo\wwwroot test\Launcher_AcceptanceHelperTwo\bin\debug\Launcher_AcceptanceHelperTwo.exe
packages\NUnit.ConsoleRunner.3.2.0\tools\nunit3-console.exe test\Launcher_AcceptanceTest\Launcher_AcceptanceTest.csproj