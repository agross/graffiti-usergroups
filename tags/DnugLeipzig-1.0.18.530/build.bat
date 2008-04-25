@echo off
cls

tools\NAnt\NAnt.exe -buildfile:default.build %1
pause
build.bat %1