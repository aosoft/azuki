@echo off
setlocal

:: check environment
(7za.exe > NUL) 2> NUL
if "%ERRORLEVEL%" == "9009" (
	echo This script needs 7za.exe.
	echo Please visit www.7-zip.org and get 7-Zip of command line version.
	goto :EOF
)
set ver=%1
if "%ver%"=="" (
	echo Please specify version string to be used for archive file name like "1.0.0":
	set /p ver=
)
if "%ver%" == "" goto :EOF


echo     Building environment
echo -----------------------------------------------------------
pushd ..
echo Created target will be place at:
cd
echo.
popd

echo Using 7za.exe located at:
for %%i in (7za.exe) do echo %%~$PATH:i
echo.


call clean > NUL


echo     Creating source package as ..\Azuki-%ver%-src.zip
echo -----------------------------------------------------------
if exist Azuki-%ver%-src.zip (
	del Azuki-%ver%-src.zip
)
pushd ..
	echo (1/1) archiving solution sources...
	7za a -mx=9 -tzip Azuki-%ver%-src.zip Azuki > NUL
popd
echo done.


echo.
echo     Creating binary package as ..\Azuki-%ver%.zip
echo -----------------------------------------------------------

:: build ff edition
echo (1/3) building Azuki for .NET Framework...
nant -nologo -q build
if "%ERRORLEVEL%" == "1" (
	goto build_failure
)

:: build cf edition
echo (2/3) building Azuki for .NET Compact Framework...
nant -nologo -q cf
if "%ERRORLEVEL%" == "1" (
	goto build_failure
)

:: package
if exist Azuki-%ver%.zip (
	del Azuki-%ver%.zip
)
echo (3/3) archiving binary package...
pushd package
	7za a -mx=9 -tzip ..\..\Azuki-%ver%.zip * > NUL
popd
echo done.
echo.

goto :EOF



:build_failure
echo !!!!!!!!!!!!!!!!!!!!!!!!!!!!!
echo !! failed to build program !!
echo !!!!!!!!!!!!!!!!!!!!!!!!!!!!!
goto :EOF
