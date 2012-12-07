Stuff needed for this to work on Mac and Linux
   ProjectGenerator templates for trial and regular projects.
   Doxygen binary and path set in common.sh.
   Bitrock binary and path set in common.sh.
   PHP binary and path set in common.sh.
   TGBGame binaries In tgb/gameData/T2DProject directory.
   Scripts to compile the engine in buildTrial.sh and buildEngine.sh.

The build process starts with packageT2D.sh. It pretty much just calls into a
bunch of specialized scripts. Each of these sub-scripts is designed to be able
to run by itself with as few dependencies as possible.

It is necessary to have the files laid out in a specific directory structure
beyond just what is in the repository. That is:

Root
   |-build
   |-engine
   |-games
   |-tgb
   |-tools
   |-documentation (Currently the entire documentation tree from DocsAndDemos.
                    In the future it will be just the required doc files)
   |-demos (From the DocsAndDemos repo for this specific version)
   |-trial (Complete checkout of the branches/trial repo)

If any of these directories move, it should just be a matter of updating the
reference to it in common.sh.

This is the basic flow of the scripts:

Build T2D (version, outputDirectory, demoList)
   Build Trial
      Run projectGenerator with t2d trial config.
      If Windows run VC8 with Torque solution and TorqueGameBuilder project.
      If Mac run xCode with Torque solution and TorqueGameBuilder project.
      If Linux run make with Torque solution and TorqueGameBuilder project.
      Run projectGenerator with t2d config.
   Build Engine
      Run projectGenerator with t2d config.
      If Windows run VC8 with Torque solution.
      If Mac run xCode with Torque solution.
      If Linux run make with Torque solution.
   Build Documentation
      Generate Engine Doxygen
         Run Doxygen with engineDoc.conf and manifest from engine build.
      Generate Script Doxygen
         Run TorqueGameBuilder with -dumpDocs
         Run Doxygen with consoleDoc.conf
      Build Documentation
         Run docGenerator with t2d config and version.
   Build Staging (demoList)
      Create staging directory.
      Create Engine Staging
         Run projectGenerator with t2d config.
         Svn export engine/bin.
         Svn export engine/compilers.
         Svn export engine/lib.
         Create source directory.
         Copy source files from manifest.
      Create Games Staging (demoList)
         Create games directory.
         Svn export games in demoList from games and demos.
      Create TGB Pro Staging
         Svn export TGB.
         If Windows run UPX on TorqueGameBuilder.exe.
         Delete files from other platforms.
      Create TGB Trial Staging
         Svn export TGB.
         Svn export trial.
         If Windows run UPX on TorqueGameBuilder.exe.
         Copy main.toolsCompile.cs.
         Run TorqueGameBuilder with main.toolsCompile.cs
         Delete all ed.cs and ed.gui files.
         Delete files from other platforms.
      Create TGB Binary Staging
         Svn export TGB.
         If Windows run UPX on TorqueGameBuilder.exe.
         Copy main.toolsCompile.cs.
         Run TorqueGameBuilder with main.toolsCompile.cs
         Delete all ed.cs and ed.gui files.
         Delete files from other platforms.
      Create Tools Staging
         Svn export tools/docBuilder.
         Svn export tools/docGenerator.
         Svn export tools/doxygen.
         Svn export tools/projectGenerator.
         Svn export tools/upx.
         Svn export tools/php.
      Create Documentation Staging
         Copy documentation output.
   Build Installers (version, outputDirectory)
      Generate Installer Projects (version)
         Run installerProjectGenerator with version.
         Run installerProjectGenerator with version and pro.
         Run installerProjectGenerator with version and commercial.
         Run installerProjectGenerator with version, commercial, and pro.
         Run installerProjectGenerator with version and downloading.
      Build Installers (outputDirectory)
         Run bitrock with indie binary project.
         Run bitrock with indie pro project.
         Run bitrock with commercial binary project.
         Run bitrock with commercial pro project.
         Run bitrock with downloading project.
      Copy installers to outputDirectory.
   Build Zips (outputDirectory)
      Build binary zip.
      Build Pro zip.
      Copy zips to outputDirectory.