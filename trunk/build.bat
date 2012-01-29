@echo off
cls

tools\NAnt\NAnt.exe -targetframework:net-3.5 -buildfile:default.build %*
pause
build.bat %*