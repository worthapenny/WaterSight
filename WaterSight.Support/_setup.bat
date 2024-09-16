@echo off
echo %~f0 %1 %2

set installdir="C:\Program Files\Bentley\OpenFlows Water"


pushd %installdir%
for %%I in (*.*) do (if not exist "%~1%%~nxI" mklink "%~1%%~nxI" "%%~fI")
popd