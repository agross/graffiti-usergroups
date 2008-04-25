@echo off
cls

set target=clean

tools\NAnt\NAnt.exe -buildfile:default.build %target%
pause
build.bat %target%