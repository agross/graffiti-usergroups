@echo off
cls

set target=import-testdata

tools\NAnt\NAnt.exe -buildfile:default.build %target%
pause
build.bat %target%