@echo off
set content=%1
set tools=%2
set profile=%3
set bin=%4
if "%1"=="" goto err
if "%2"=="" goto err
if "%3"=="" goto err
if "%4"=="" goto err
set bin_abs=
pushd %bin%
set bin_abs=%CD%
popd
echo content source folder %content%
echo mgfx tools folder %tools%
echo output folder %bin%

set olddir=%CD%
cd %content%

for %%F IN (*.fx) do (
	echo %tools%\2mgfx.exe "%%F" "%bin_abs%\%%F.mgfxo" /Profile:%profile%
	%tools%\2mgfx.exe "%%F" "%bin_abs%\%%F.mgfxo" /Profile:%profile%

)
echo Done converting
cd %olddir%

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