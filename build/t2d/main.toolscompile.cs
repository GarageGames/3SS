//-----------------------------------------------------------------------------
// Torque 2D. 
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

function onExit()
{
}

//-----------------------------------------------------------------------------
// Utility function used in findFolders()
//-----------------------------------------------------------------------------
function isIgnoredPath(%path)
{
   if (%path $= ".svn")
      return true;
   else
      return false;
}

//-----------------------------------------------------------------------------
// Gets a list of all folders, ignoring ignored paths.
//-----------------------------------------------------------------------------
function findFolders()
{
   %list = getDirectoryList("/", 0);
   
   %listcount = getWordCount(%list);
   
   // snip ignored paths from the list
   for (%i = 0; %i < %listcount; %i++)
   {
      %folder = getWord(%list, %i);
      if (!isIgnoredPath(%folder))
      {
         %goodlist = %goodlist SPC %folder;
      }
   }
   %goodlist = trim(%goodlist);
   
   return %goodlist;
}

//-----------------------------------------------------------------------------
// Adds each of the paths in a list to the resource manager.
//-----------------------------------------------------------------------------
function setModPaths(%paths)
{
   %pathCount = getWordCount(%paths);
   for (%i = 0; %i < %pathCount; %i++)
   {
      %path = getWord(%paths, %i);
      addResPath(%path);
   }
}

//-----------------------------------------------------------------------------
// Compiles anything that matches %pattern that the resource manager has
// loaded.
//-----------------------------------------------------------------------------
function compileAll(%pattern)
{
   %file = findFirstFile(%pattern);
   while(%file !$= "")
   {
      echo(%file);
      compile(%file);
      %file = findNextFile(%pattern);
   }
}

//setLogMode(6);

// get a list of all folders, ignoring ignored paths.
echo("Searching for directories in the current working directory... ");
$modpaths = findFolders();
echo("Found these directories:");
echo("----------------------------------------"); 
echo(strreplace($modpaths, " ", "\n"));
echo("----------------------------------------"); 

// load paths into the resource manager.
echo("\nLoading paths into the resource manager...");
setModPaths($modpaths);

$Scripts::OverrideDSOPath = expandPath("");

// compile all ed.cs
echo("----------------------------------------"); 
echo("Compiling Editor Scripts");
echo("----------------------------------------"); 
compileAll("*.ed.cs");

// compile all ed.gui
echo("----------------------------------------"); 
echo("Compiling Editor Guis");
echo("----------------------------------------"); 
compileAll("*.ed.gui");

// quit
echo("\n Done, quitting.");
quit();




