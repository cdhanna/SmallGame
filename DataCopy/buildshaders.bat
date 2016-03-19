@echo off
set content=%1
set tools=%2
set profile=%3
set bin=%4

if "%1"=="" goto err
if "%2"=="" goto err
if "%3"=="" goto err
if "%4"=="" goto err

set content_rel=%content%
set content_abs=

pushd %content_rel%
set content_abs=%CD%
popd


echo content source folder %content_abs%
echo mgfx tools folder %tools%
echo output folder %bin%

for /F %%A IN ('dir %content% /B /S ^| findstr /i ".fx$"') DO (
	
	for %%F in ("%%A") do setlocal enabledelayedexpansion output=%bin%\%%~nxF
	%tools%\2mgfx %%A !output!.mgfxo /Profile:%profile%
)
goto eof

:err
echo Invalid Usage. 
echo . 
echo .	buildshader ^<content_folder^> ^<tool_folder^> ^<profile^> ^<output_folder^>
echo .		^<content_folder^> : The folder to search for shaders in
echo .		^<tool_folder^>    : The folder containing 2mgfx.exe
echo .		^<profile^>        : The profile. Must be OpenGL, DirectX_11
echo .		^<output_folder^>  : The output folder
:eof