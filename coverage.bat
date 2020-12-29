@echo off

set PROJECTNAME=LargeList
set TESTPROJECTNAME=Test-%PROJECTNAME%

set OPENCOVER_VERSION=4.7.922
set OPENCOVER=OpenCover.%OPENCOVER_VERSION%
set CODECOV_VERSION=1.12.2
set CODECOV=Codecov.%CODECOV_VERSION%
set NUINT_CONSOLE_VERSION=3.11.1
set NUINT_CONSOLE=NUnit.ConsoleRunner.%NUINT_CONSOLE_VERSION%

set RESULTFILENAME=Coverage-%PROJECTNAME%.xml
set FRAMEWORK=net48

nuget install OpenCover -Version %OPENCOVER_VERSION% -OutputDirectory packages
if not exist ".\packages\%OPENCOVER%\tools\OpenCover.Console.exe" goto error_console1
nuget install CodeCov -Version %CODECOV_VERSION% -OutputDirectory packages
if not exist ".\packages\%CODECOV%\tools\codecov.exe" goto error_console2
nuget install NUnit.ConsoleRunner -Version %NUINT_CONSOLE_VERSION% -OutputDirectory packages
if not exist ".\packages\%NUINT_CONSOLE%\tools\nunit3-console.exe" goto error_console3

dotnet publish Test\%TESTPROJECTNAME% -c Debug -f %FRAMEWORK% /p:Platform=x64 -o ./Test/%TESTPROJECTNAME%/bin/x64/Debug/publish
dotnet publish Test\%TESTPROJECTNAME% -c Release -f %FRAMEWORK% /p:Platform=x64 -o ./Test/%TESTPROJECTNAME%/bin/x64/Release/publish
if not exist ".\Test\%TESTPROJECTNAME%\bin\x64\Debug\publish\%TESTPROJECTNAME%.dll" goto error_not_built
if not exist ".\Test\%TESTPROJECTNAME%\bin\x64\Release\publish\%TESTPROJECTNAME%.dll" goto error_not_built

if exist .\Test\%TESTPROJECTNAME%\*.log del .\Test\%TESTPROJECTNAME%\*.log
if exist .\Test\%TESTPROJECTNAME%\obj\x64\Debug\%RESULTFILENAME% del .\Test\%TESTPROJECTNAME%\obj\x64\Debug\%RESULTFILENAME%
if exist .\Test\%TESTPROJECTNAME%\obj\x64\Release\%RESULTFILENAME% del .\Test\%TESTPROJECTNAME%\obj\x64\Release\%RESULTFILENAME%

rem ".\packages\%OPENCOVER%\tools\OpenCover.Console.exe" -register:user -target:".\packages\%NUINT_CONSOLE%\tools\nunit3-console.exe" -targetargs:".\Test\%TESTPROJECTNAME%\bin\x64\Debug\publish\%TESTPROJECTNAME%.dll --trace=Debug --labels=Before" -filter:"+[%PROJECTNAME%*]* -[%TESTPROJECTNAME%*]*" -output:".\Test\%TESTPROJECTNAME%\obj\x64\Debug\%RESULTFILENAME%"
".\packages\%OPENCOVER%\tools\OpenCover.Console.exe" -register:user -target:".\packages\%NUINT_CONSOLE%\tools\nunit3-console.exe" -targetargs:".\Test\%TESTPROJECTNAME%\bin\x64\Release\publish\%TESTPROJECTNAME%.dll --trace=Debug --labels=Before" -filter:"+[%PROJECTNAME%*]* -[%TESTPROJECTNAME%*]*" -output:".\Test\%TESTPROJECTNAME%\obj\x64\Release\%RESULTFILENAME%"
goto end

call ..\Certification\set_tokens.bat
if exist .\Test\%TESTPROJECTNAME%\obj\x64\Debug\%RESULTFILENAME% .\packages\%CODECOV%\tools\codecov -f ".\Test\%TESTPROJECTNAME%\obj\x64\Debug\%RESULTFILENAME%" -t %LARGELIST_CODECOV_TOKEN%

goto end

:error_console1
echo ERROR: OpenCover.Console not found.
goto end

:error_console2
echo ERROR: Codecov not found.
goto end

:error_console3
echo ERROR: nunit3-console not found.
goto end

:error_not_built
echo ERROR: Test-LargeList.dll not built (both Debug and Release are required).
goto end

:end
del *.log
