//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

// Set log mode.
setLogMode(2);

// Controls whether the execution or script files or compiled DSOs are echoed to the console or not.
// Being able to turn this off means far less spam in the console during typical development.
setScriptExecEcho( false );

// Controls whether all script execution is traced (echoed) to the console or not.
trace( false );

// The name of the company. Used to form the path to save preferences. Defaults to GarageGames
// if not specified.
// The name of the game. Used to form the path to save preferences. Defaults to C++ engine define TORQUE_GAME_NAME
// if not specified.
// Appending version string to avoid conflicts with existing versions and other versions.
setCompanyAndProduct("GarageGames", "3StepStudio" @ getThreeStepStudioVersion());

// Set module database information echo.
ModuleDatabase.EchoInfo = false;

// Set asset database information echo.
AssetDatabase.EchoInfo = false;

// Is a module merge available?
if ( ModuleDatabase.isModuleMergeAvailable() )
{
    // Yes, so merge modules.
    if ( ModuleDatabase.mergeModules( "modules", true, false ) == false )
    {
        error( "A serious error occurred merging modules!" );
        quit();
    }
}

// Scan modules.
ModuleDatabase.scanModules( "modules" );

// Load boot module.
ModuleDatabase.LoadExplicit( "{EditorBoot}" );
