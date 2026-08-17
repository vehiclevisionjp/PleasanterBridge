@echo off
cd /d "%~dp0"
dotnet pack -c Release -o ./nupkg