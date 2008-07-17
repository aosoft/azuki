@echo off

cd Azuki
call clean.bat
cd ..

cd AzukiSample
call clean.bat
cd ..

del /ah *.suo 2> NUL
rmdir /s /q reference 2> NUL
del package\log.txt 2> NUL
