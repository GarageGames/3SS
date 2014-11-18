3StepStudio.exe main.toolscompile.cs
mkdir BuildPackage
XCOPY "Modules" "BuildPackage/Modules" /E /I /H
XCOPY "Exercises" "BuildPackage/Exercises" /E /I /H
XCOPY "imageformats" "BuildPackage/imageformats" /E /I /H
XCOPY "Templates/projectFiles" "BuildPackage/Templates/projectFiles" /E /I /H
XCOPY 3StepStudio.exe "BuildPackage"
XCOPY main.cs "BuildPackage"
XCOPY *.dll "BuildPackage"
XCOPY README.md "BuildPackage"
del /s BuildPackage\modules\*.cs
del /s BuildPackage\Exercises\*.cs
del /s BuildPackage\Exercises\*main.cs.dso
XCOPY Exercises\*main.cs "BuildPackage/Exercises" /E
del /s BuildPackage\Exercises\*.exe
del /s BuildPackage\Exercises\*.torsion
del /s BuildPackage\Templates\projectFiles\main.cs.dso
del /s BuildPackage\Templates\projectFiles\cleandso.bat
del /s BuildPackage\Templates\projectFiles\Game.torsion

