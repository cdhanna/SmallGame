@echo off
set foo=C:\Temp\Test
call :strip
echo %foo%
goto :eof

:strip
if not "%foo:~-1%"=="\" (
    set foo=%foo:~0,-1%
    goto :strip
)
goto :eof