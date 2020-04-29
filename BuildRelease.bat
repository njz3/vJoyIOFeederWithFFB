
REM echo off

SET BuildMode=Release

SET ZIP=%ProgramFiles%\7-Zip\7z.exe
SET ARCHIVE_NAME=vJoyIOFeederSetup.zip
SET VS=2019\Community
SET BUILDER=%ProgramFiles(x86)%\Microsoft Visual Studio\%VS%\MSBuild\Current\Bin\MSBuild.exe
SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio\%VS%\Common7\IDE\devenv.com
SET Target64=vJoyIOFeederSetup\%BuildMode%
SET OUTPUTDIR=vJoyIOFeederSetup\vJoyIOFeederSetup
REM SET Target32=x86\%BuildMode%\


:build64
echo %DATE% %TIME%: Cleaning (x64)
"%BUILDER%"  vJoyIOFeederSetup.sln  /maxcpucount:1 /t:clean /p:Platform=x64;Configuration=%BuildMode%
set BUILD_STATUS=%ERRORLEVEL%
if not %BUILD_STATUS%==0 goto fail

echo %DATE% %TIME%: Building setup (x64)
"%BUILDER%"  vJoyIOFeederSetup.sln  /maxcpucount:4  /p:Platform=x64;Configuration=%BuildMode%
set BUILD_STATUS=%ERRORLEVEL%
if not %BUILD_STATUS%==0 goto fail

:installer
SET current_path="%CD%"
set current_drive=%cd:~0,2%
CD "%ProgramFiles(x86)%\Microsoft Visual Studio\%VS%\Common7\IDE\CommonExtensions\Microsoft\VSI\DisableOutOfProcBuild"
C:
CALL DisableOutOfProcBuild.exe
set BUILD_STATUS=%ERRORLEVEL%
if not %BUILD_STATUS%==0 goto fail

CD "%current_path%"
%current_drive%
"%DEVENV%" vJoyIOFeederSetup.sln /build "Release"

:arduino
CD FeederIOBoard
"%ZIP%" a -tzip "..\FeederIOBoard.zip" .
CD ..

:archive
SET current_path="%CD%"
RMDIR /S /Q "%OUTPUTDIR%"
XCOPY /E /I "%Target64%" "%OUTPUTDIR%"
XCOPY "tools\fedit.exe" "%OUTPUTDIR%"
XCOPY "tools\vJoySetup 2.2.0 signed.exe" "%OUTPUTDIR%"
XCOPY /E /I "gameassets" "%OUTPUTDIR%\gameassets"
XCOPY /Y /I LICENSE "%OUTPUTDIR%"
XCOPY /Y /I README.md "%OUTPUTDIR%"
XCOPY /Y /I FAQ.md "%OUTPUTDIR%"
MOVE /Y FeederIOBoard.zip "%OUTPUTDIR%"

CD "%OUTPUTDIR%"
DEL /F /Q "..\%ARCHIVE_NAME%"
"%ZIP%" a -tzip "..\%ARCHIVE_NAME%" .
CD %current_path%
set BUILD_STATUS=%ERRORLEVEL%
if not %BUILD_STATUS%==0 goto fail

:fail
exit /b 1


