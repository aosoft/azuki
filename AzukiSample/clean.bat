@echo off
setlocal

echo Cleaning AzukiSample...

rmdir /S /Q obj  2> NUL
rmdir /S /Q bin  2> NUL
rmdir /S /Q Debug  2> NUL
rmdir /S /Q Release  2> NUL
del /Q *.csproj.user  2> NUL

del ..\package\Azuki*Sample.exe  2> NUL
del ..\package\Azuki*Sample.pdb  2> NUL
del ..\package\Azuki*Sample.xml  2> NUL
del ..\package\Azuki*Sample.exe.log.txt  2> NUL
del ..\package\Azuki*Sample.vshost.exe  2> NUL
del ..\package\Azuki*Sample.vshost.exe.manifest  2> NUL

endlocal
echo.
