@echo off
cls

set target=deploy

tools\NAnt\NAnt.exe -buildfile:default.build %target%
pause
build.bat %target%