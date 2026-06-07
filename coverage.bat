@echo off

setlocal

call ..\Certification\set_tokens.bat

set PROJECTNAME=LargeList
set TOKEN=%LARGELIST_CODECOV_TOKEN%
set PLATFORM=x64
set CONFIGURATION=%1
set FRAMEWORK=%2

if /I NOT "%~3" == "slow" (
    set FILTER=--filter TestCategory!=SlowTest
)

if /I "%~3" == "strict" (
    set ENABLE_STRICT=true
)

if /I "%FRAMEWORK%" == "net10.0" (
    cd .\Test\%PROJECTNAME%.Test

    dotnet build /p:Platform=%PLATFORM% -c %CONFIGURATION% -f %FRAMEWORK%
    if %ERRORLEVEL% NEQ 0 goto error

    rem Execute tests within OpenCover.
    ..\..\%OPENCOVER_EXE% -register:user -target:"C:\Program Files\dotnet\dotnet.exe" -targetargs:"test /p:Platform=%PLATFORM% -c %CONFIGURATION% -f %FRAMEWORK% --output:detailed %FILTER%" -output:%RESULTFILEPATH% -returntargetcode -mergeoutput
    if %ERRORLEVEL% NEQ 0 goto error
)

if /I "%FRAMEWORK%" == "net481" (
    dotnet build ./Test/%PROJECTNAME%.Test.DotNetFramework /p:Platform=%PLATFORM% -c %CONFIGURATION% -f %FRAMEWORK%
    if %ERRORLEVEL% NEQ 0 goto error

    rem Execute tests within OpenCover.
    %OPENCOVER_EXE% -register:user -target:"C:\Program Files\dotnet\dotnet.exe" -targetargs:"test ./Test/%PROJECTNAME%.Test.DotNetFramework/bin/%PLATFORM%/%CONFIGURATION%/%FRAMEWORK%/%PROJECTNAME%.Test.DotNetFramework.dll -l console;verbosity=detailed %FILTER%" -output:%RESULTFILEPATH% -returntargetcode -mergeoutput
    if %ERRORLEVEL% NEQ 0 goto error
)

goto end

:error
if exist %RESULTFILEPATH% del %RESULTFILEPATH%

:end