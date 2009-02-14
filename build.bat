@echo off
cls

tools\NAnt\NAnt.exe -buildfile:default.build %*
pause
build.bat %*