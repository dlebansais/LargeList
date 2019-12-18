@echo off

if not exist "..\packages\NUnit.ConsoleRunner.3.10.0\tools\nunit3-console.exe" goto error_console
if not exist "..\Test-LargeList\bin\x64\Release\Test-LargeList.dll" goto error_largelist
"..\packages\NUnit.ConsoleRunner.3.10.0\tools\nunit3-console.exe" --trace=Debug --labels=All "..\Test-LargeList\bin\x64\Release\Test-LargeList.dll"
goto end

:error_console
echo ERROR: nunit3-console not found.
goto end

:error_largelist
echo ERROR: Test-LargeList.dll not built.
goto end

:end
