@echo off

setlocal

call ..\Certification\set_tokens.bat

set PROJECTNAME=LargeList
set TOKEN=%LARGELIST_CODECOV_TOKEN%
set PLATFORM=x64
set RESULTFILENAME=Coverage-%PROJECTNAME%.xml
set RESULTFILEPATH="C:\Projects\LargeList\Test\%RESULTFILENAME%"

set OPENCOVER_VERSION=4.7.1221
set OPENCOVER=OpenCover.%OPENCOVER_VERSION%
set OPENCOVER_EXE=".\packages\%OPENCOVER%\tools\OpenCover.Console.exe"

set CODECOV_UPLOADER_VERSION=0.8.0
set CODECOV_UPLOADER=CodecovUploader.%CODECOV_UPLOADER_VERSION%
set CODECOV_UPLOADER_EXE=".\packages\%CODECOV_UPLOADER%\tools\codecov.exe"

set REPORTGENERATOR_VERSION=5.4.1
set REPORTGENERATOR=ReportGenerator.%REPORTGENERATOR_VERSION%
set REPORTGENERATOR_EXE=".\packages\%REPORTGENERATOR%\tools\net8.0\ReportGenerator.exe"

nuget install OpenCover -Version %OPENCOVER_VERSION% -OutputDirectory packages
nuget install CodecovUploader -Version %CODECOV_UPLOADER_VERSION% -OutputDirectory packages
nuget install ReportGenerator -Version %REPORTGENERATOR_VERSION% -OutputDirectory packages

if '%TOKEN%' == '' goto error_console1
if not exist %OPENCOVER_EXE% goto error_console2
if not exist %CODECOV_UPLOADER_EXE% goto error_console3
if not exist %REPORTGENERATOR_EXE% goto error_console4

if exist %RESULTFILEPATH% del %RESULTFILEPATH%

rem call coverage.bat Release net481 slow

for %%A in (Release Debug) do (
    for %%B in (normal strict) do (
        for %%C in (net481 net10.0) do (
            call coverage.bat %%A %%C %%B
            if not exist %RESULTFILEPATH% goto loopend
        )
    )
)
:loopend
if not exist %RESULTFILEPATH% exit /b 1

if not exist %RESULTFILEPATH% goto end
%CODECOV_UPLOADER_EXE% -f %RESULTFILEPATH% -t %TOKEN%
%REPORTGENERATOR_EXE% -reports:%RESULTFILEPATH% -targetdir:.\CoverageReports "-assemblyfilters:+%PROJECTNAME%;+%PROJECTNAME%.Test;+%PROJECTNAME%.Test.DotNetFramework" "-filefilters:-*.g.cs;-*Microsoft.NET.Test.Sdk.Program.cs"
del %RESULTFILEPATH%
goto end

:error_console1
echo ERROR: CodeCov token not set.
goto end

:error_console2
echo ERROR: OpenCover.Console not found.
goto end

:error_console3
echo ERROR: Codecov not found.
goto end

:error_console4
echo ERROR: ReportGenerator not found.
goto end

:end
