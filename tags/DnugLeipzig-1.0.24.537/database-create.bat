@echo off
cls

set target=create-database

tools\NAnt\NAnt.exe -buildfile:default.build %target%
pause
build.bat %target%