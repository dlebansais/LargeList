if not exist .\Nuget\lib md .\Nuget\lib
if not exist .\Nuget\lib\net452 md .\Nuget\lib\net452
copy .\LargeList\bin\x64\Release\LargeList.dll .\Nuget\lib\net452
copy .\LargeList\bin\x64\Release\LargeList.xml .\Nuget\lib\net452
copy .\LargeList\LargeList.nuspec .\Nuget\CSharp.LargeList.nuspec

cd Nuget
C:\Applications\7-Zip\7z u CSharp.LargeList.zip lib\net452\LargeList.dll
C:\Applications\7-Zip\7z u CSharp.LargeList.zip lib\net452\LargeList.xml
C:\Applications\7-Zip\7z u CSharp.LargeList.zip CSharp.LargeList.nuspec
copy CSharp.LargeList.zip CSharp.LargeList.1.0.0.358.nupkg
rd /S /Q lib
del CSharp.LargeList.nuspec
cd ..

:pub

call ..\Certification\publish .\Nuget\CSharp.LargeList.1.0.0.358.nupkg

del .\Nuget\CSharp.LargeList.1.0.0.358.nupkg
