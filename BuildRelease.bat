echo off

SET BuildMode=Release
REM Set VERSION to last git annoted tag
FOR /F %%i IN ('git describe') DO set TAG=%%i
FOR /F %%i IN ('git rev-parse --short HEAD') DO set HASH=%%i
SET VERSION=%TAG%-%HASH%

ECHO %VERSION%

SET ZIP=%ProgramFiles%\7-Zip\7z.exe
SET ARCHIVE_NAME=BackForceFeederSetup_%VERSION%.zip
SET VS=2019\Community
SET BUILDER=%ProgramFiles(x86)%\Microsoft Visual Studio\%VS%\MSBuild\Current\Bin\MSBuild.exe
SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio\%VS%\Common7\IDE\devenv.com
SET Target64=BackForceFeederSetup\%BuildMode%
SET OUTPUTDIR=BackForceFeederSetup\BackForceFeederSetup
REM SET Target32=x86\%BuildMode%\


:build64
echo %DATE% %TIME%: Cleaning (x64)
"%BUILDER%"  BackForceFeederSetup.sln  /maxcpucount:1 /t:clean /p:Platform=x64;Configuration=%BuildMode%
set BUILD_STATUS=%ERRORLEVEL%
if not %BUILD_STATUS%==0 goto fail

echo %DATE% %TIME%: Building setup (x64)
"%BUILDER%"  BackForceFeederSetup.sln  /maxcpucount:4  /p:Platform=x64;Configuration=%BuildMode%
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
"%DEVENV%" BackForceFeederSetup.sln /build "Release"

:arduino
CD FeederIOBoard
"%ZIP%" a -tzip "..\FeederIOBoard.zip" .
CD ..

:archive
SET current_path="%CD%"
RMDIR /S /Q "%OUTPUTDIR%"
XCOPY /E /I "%Target64%" "%OUTPUTDIR%"
XCOPY "tools\fedit.exe" "%OUTPUTDIR%"
XCOPY "tools\vJoySetup-2.2.1-signed.exe" "%OUTPUTDIR%"
XCOPY "tools\AddvJoyFFB.reg" "%OUTPUTDIR%"
XCOPY "tools\RemvJoyFFB.reg" "%OUTPUTDIR%"
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


