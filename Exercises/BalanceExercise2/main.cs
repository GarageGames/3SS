//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

// Set log mode.
setLogMode(2);

// Controls whether the execution or script files or compiled DSOs are echoed to the console or not.
// Being able to turn this off means far less spam in the console during typical development.
setScriptExecEcho(false);

// Controls whether all script execution is traced (echoed) to the console or not.
trace(false);

// Scan modules.
ModuleDatabase.scanModules("modules");
ModuleDatabase.scanModules("game");

// Load boot module.
ModuleDatabase.LoadExplicit("{GameBoot}");