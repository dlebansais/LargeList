set PATH=%PATH%;"C:\Applications\cov-analysis-win64-2019.03\bin"

"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild" /t:clean
cov-build --dir cov-int "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild"

C:\Applications\7-Zip\7z a cov-int cov-int -tzip

rem curl --form token=40XkA-tCeIYeAWjOfzn0Xw --form email=dlebansais@gmail.com --form file=@/Projects/LargeList/cov-int.zip --form version="1.0.0.343" --form description="Test" "https://scan.coverity.com/builds?project=dlebansais/LargeList"
curl --form token=40XkA-tCeIYeAWjOfzn0Xw --form email=dlebansais@gmail.com --form file=@/Projects/LargeList/cov-int.zip "https://scan.coverity.com/builds?project=dlebansais/LargeList"
