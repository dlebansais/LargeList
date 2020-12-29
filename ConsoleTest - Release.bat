@echo off

set PROJECTNAME=LargeList
set TESTPROJECTNAME=Test-%PROJECTNAME%

set NUINT_CONSOLE_VERSION=3.11.1
set NUINT_CONSOLE=NUnit.ConsoleRunner.%NUINT_CONSOLE_VERSION%

set FRAMEWORK=net48

nuget install NUnit.ConsoleRunner -Version %NUINT_CONSOLE_VERSION% -OutputDirectory packages
if not exist ".\packages\%NUINT_CONSOLE%\tools\nunit3-console.exe" goto error_console

dotnet publish Test\%TESTPROJECTNAME% -c Release -f %FRAMEWORK% /p:Platform=x64 -o ./Test/%TESTPROJECTNAME%/bin/x64/Release/publish
if not exist ".\Test\%TESTPROJECTNAME%\bin\x64\Release\publish\%TESTPROJECTNAME%.dll" goto error_not_built

".\packages\%NUINT_CONSOLE%\tools\nunit3-console.exe" --trace=Debug --labels=Before ".\Test\%TESTPROJECTNAME%\bin\x64\Release\publish\%TESTPROJECTNAME%.dll"
goto end

:error_console
echo ERROR: nunit3-console not found.
goto end

:error_not_built
echo ERROR: Test-LargeList.dll not built.
goto end

:end
del *.log
