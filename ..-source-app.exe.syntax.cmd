set PATH=C:\D\dmd2\bin;C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Tools\MSVC\14.14.26428\bin\HostX86\x64;C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE;C:\Program Files (x86)\Windows Kits\10\bin;%PATH%
set LIB=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Tools\MSVC\14.14.26428\lib\x86;C:\Program Files (x86)\Windows Kits\10\Lib\10.0.17134.0\ucrt\x86;C:\Program Files (x86)\Windows Kits\10\lib\10.0.17134.0\um\x86
set VCINSTALLDIR=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\
set VCTOOLSINSTALLDIR=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Tools\MSVC\14.14.26428\
set VSINSTALLDIR=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\
set WindowsSdkDir=C:\Program Files (x86)\Windows Kits\10\
set WindowsSdkVersion=10.0.17134.0
set UniversalCRTSdkDir=C:\Program Files (x86)\Windows Kits\10\
set UCRTVersion=10.0.17134.0
echo Compiling ..\source\app.d...
set VCINSTALLDIR=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\
set VCTOOLSINSTALLDIR=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Tools\MSVC\14.14.26428\
set VSINSTALLDIR=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\
set WindowsSdkDir=C:\Program Files (x86)\Windows Kits\10\
set WindowsSdkVersion=10.0.17134.0
set UniversalCRTSdkDir=C:\Program Files (x86)\Windows Kits\10\
set UCRTVersion=10.0.17134.0
"C:\Program Files (x86)\VisualD\pipedmd.exe" rdmd -g -gf -debug -w -op -I"..\source" -IC:\D\dmd2\import\std -version=Have_dlang_prs -of"C:\Users\sewer\Desktop\Github\dlang-PRS\\..-source-app.exe" -map "obj\debug\dummy\dlang-prs\dlang-prs.map"   --build-only -unittest ..\source\app.d
:reportError
if %errorlevel% neq 0 echo Building C:\Users\sewer\Desktop\Github\dlang-PRS\\..-source-app.exe failed!
if %errorlevel% == 0 echo Compilation successful.
if %errorlevel% neq 0 exit %ERRORLEVEL% /B
C:\Users\sewer\Desktop\Github\dlang-PRS\\..-source-app.exe
echo Execution result code: %ERRORLEVEL%
