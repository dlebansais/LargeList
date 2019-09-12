set PATH=%PATH%;"C:\Applications\cov-analysis-win64-2019.03\bin"

"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild" /t:clean
cov-build --dir cov-int "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild"

C:\Applications\7-Zip\7z a cov-int cov-int -tzip

call ..\Certification\coverity_scan_LargeList.bat

rd /S /Q cov-int
del cov-int.zip