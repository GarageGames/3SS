@echo off

REM Handle our optional parameters
SET MODE=%1
SET OUTDIR=%2
SET OUTFORMAT=%3

IF NOT DEFINED MODE SET MODE=Offline
IF NOT DEFINED OUTDIR SET OUTDIR=None
IF NOT DEFINED OUTFORMAT SET OUTFORMAT=Full

REM Setting up some variables
SET THISCWD=%CD%
SET XCOPYOPTS=EIY
SET SEARCHCONFIG=..\..\documentation2\SourceDocumentation\TGB\SEC_TGB.ini

IF %MODE%==Online SET SEARCHCONFIG=..\..\documentatino2\SourceDocumentation\TGB\SEC_TGB_Online.ini

REM Build the docs
REM We are going to use the pre-existing shell scripts in the interest of time
c:\cygwin\bin\sh -c "export PATH=/cygdrive/c/cygwin/bin:. && buildDocumentation.sh

cd %THISCWD%

echo    -    o Running DocGenerator
cd    ..\..\documentation2\docGenerator\TGB
call  generateDocs.bat %MODE%


IF %MODE%==Online (

    del /F /Q ..\..\Output\TGB\documentation\content\top.html

    rename ..\..\Output\TGB\documentation\content\top_online.html top.html

    rename "..\..\Output\TGB\documentation\Documentation Overview.html" index.html
)


IF EXIST "C:\Program Files\Search Engine Composer\search.exe" (
    echo -    o Generating Search Index

    REM Run Search Engine Composer with a 30 min timeout
    processChecker.exe 1800 "C:\Program Files\Search Engine Composer\search.exe" %SEARCHCONFIG% -is

    echo -    o Search Indexing done
)

REM Handle the cleanup and creation of our output directory if we are going to directly copy the files there
IF %OUTFORMAT%==Full (
    IF NOT %OUTDIR%==None (
        echo    -    o Copying docs to staging directory
        IF EXIST %OUTDIR%\documentation rmdir /S /Q %OUTDIR%\documentation
        mkdir %OUTDIR%

         move  /Y ..\..\Output\TGB\documentation %OUTDIR%\documentation
    )
)

REM If we are outputting an archive we will zip up our directory and copy that to the output directory
IF %OUTFORMAT%==Zip (
    IF EXIST ..\..\Output\TGB\TGB_Official_Docs_%MODE%.rar del /Q ..\..\Output\TGB\TGB_Official_Docs_%MODE%.rar
    call "C:\Program Files\WinRAR\Rar.exe" a -ep1 ..\..\Output\TGB\TGB_Official_Docs_%MODE%.rar ..\..\Output\TGB\documentation

    move /Y ..\..\Output\TGB\TGB_Official_Docs_%MODE%.rar %OUTDIR%
)