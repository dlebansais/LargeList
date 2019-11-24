@echo off

if not exist ".\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe" goto error_console1
if not exist ".\packages\NUnit.ConsoleRunner.3.9.0\tools\nunit3-console.exe" goto error_console2
if not exist ".\Test-LargeList\bin\x64\Debug\Test-LargeList.dll" goto error_not_built
if not exist ".\Test-LargeList\bin\x64\Release\Test-LargeList.dll" goto error_not_built
if exist .\Test-LargeList\*.log del .\Test-LargeList\*.log
if exist .\Test-LargeList\obj\x64\Debug\Coverage-LargeList-Debug_coverage.xml del .\Test-LargeList\obj\x64\Debug\Coverage-LargeList-Debug_coverage.xml
if exist .\Test-LargeList\obj\x64\Release\Coverage-LargeList-Release_coverage.xml del .\Test-LargeList\obj\x64\Release\Coverage-LargeList-Release_coverage.xml
".\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe" -register:user -target:".\packages\NUnit.ConsoleRunner.3.9.0\tools\nunit3-console.exe" -targetargs:".\Test-LargeList\bin\x64\Debug\Test-LargeList.dll --trace=Debug --labels=All" -filter:"+[LargeList*]* -[Test-LargeList*]*" -output:".\Test-LargeList\obj\x64\Debug\Coverage-LargeList-Debug_coverage.xml" -showunvisited
".\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe" -register:user -target:".\packages\NUnit.ConsoleRunner.3.9.0\tools\nunit3-console.exe" -targetargs:".\Test-LargeList\bin\x64\Release\Test-LargeList.dll --trace=Debug --labels=All" -filter:"+[LargeList*]* -[Test-LargeList*]*" -output:".\Test-LargeList\obj\x64\Release\Coverage-LargeList-Release_coverage.xml" -showunvisited
if exist .\Test-LargeList\obj\x64\Debug\Coverage-LargeList-Debug_coverage.xml .\packages\Codecov.1.1.1\tools\codecov -f ".\Test-LargeList\obj\x64\Debug\Coverage-LargeList-Debug_coverage.xml" -t "4abfe066-5e77-4449-b893-73104119f505"
if exist .\Test-LargeList\obj\x64\Release\Coverage-LargeList-Release_coverage.xml .\packages\Codecov.1.1.1\tools\codecov -f ".\Test-LargeList\obj\x64\Release\Coverage-LargeList-Release_coverage.xml" -t "4abfe066-5e77-4449-b893-73104119f505"
goto end

:error_console1
echo ERROR: OpenCover.Console not found.
goto end

:error_console2
echo ERROR: nunit3-console not found.
goto end

:error_not_built
echo ERROR: Test-LargeList.dll not built (both Debug and Release are required).
goto end

:end
