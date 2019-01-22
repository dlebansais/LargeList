@echo off

if not exist "..\Misc-Beta-Test\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe" goto error_console1
if not exist "..\Misc-Beta-Test\packages\NUnit.ConsoleRunner.3.9.0\tools\nunit3-console.exe" goto error_console2
if not exist "..\Misc-Beta-Test\Test-LargeList\bin\x64\Debug\Test-LargeList.dll" goto error_largelist
if not exist "..\Misc-Beta-Test\Test-LargeList\bin\x64\Release\Test-LargeList.dll" goto error_largelist
if exist ..\Misc-Beta-Test\Test-LargeList\*.log del ..\Misc-Beta-Test\Test-LargeList\*.log
if exist ..\Misc-Beta-Test\Test-LargeList\Coverage-LargeList-Debug_coverage.xml del ..\Misc-Beta-Test\Test-LargeList\Coverage-LargeList-Debug_coverage.xml
if exist ..\Misc-Beta-Test\Test-LargeList\Coverage-LargeList-Release_coverage.xml del ..\Misc-Beta-Test\Test-LargeList\Coverage-LargeList-Release_coverage.xml
"..\Misc-Beta-Test\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe" -register:user -target:"..\Misc-Beta-Test\packages\NUnit.ConsoleRunner.3.9.0\tools\nunit3-console.exe" -targetargs:"..\Misc-Beta-Test\Test-LargeList\bin\x64\Debug\Test-LargeList.dll --trace=Debug --labels=All" -filter:"+[LargeList*]* -[Test-LargeList*]*" -output:"..\Misc-Beta-Test\Test-LargeList\Coverage-LargeList-Debug_coverage.xml" -showunvisited
"..\Misc-Beta-Test\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe" -register:user -target:"..\Misc-Beta-Test\packages\NUnit.ConsoleRunner.3.9.0\tools\nunit3-console.exe" -targetargs:"..\Misc-Beta-Test\Test-LargeList\bin\x64\Release\Test-LargeList.dll --trace=Debug --labels=All" -filter:"+[LargeList*]* -[Test-LargeList*]*" -output:"..\Misc-Beta-Test\Test-LargeList\Coverage-LargeList-Release_coverage.xml" -showunvisited
if exist ..\Misc-Beta-Test\Test-LargeList\Coverage-LargeList-Debug_coverage.xml ..\Misc-Beta-Test\packages\Codecov.1.1.1\tools\codecov -f "..\Misc-Beta-Test\Test-LargeList\Coverage-LargeList-Debug_coverage.xml" -t "4abfe066-5e77-4449-b893-73104119f505"
if exist ..\Misc-Beta-Test\Test-LargeList\Coverage-LargeList-Release_coverage.xml ..\Misc-Beta-Test\packages\Codecov.1.1.1\tools\codecov -f "..\Misc-Beta-Test\Test-LargeList\Coverage-LargeList-Release_coverage.xml" -t "4abfe066-5e77-4449-b893-73104119f505"
goto end

:error_console1
echo ERROR: OpenCover.Console not found.
goto end

:error_console2
echo ERROR: nunit3-console not found.
goto end

:error_largelist
echo ERROR: Test-LargeList.dll not built (both Debug and Release are required).
goto end

:end
