//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//set up $cmdctrl variable so that it matches OS standards
$cmdCtrl = $platform $= "macos" ? "Cmd" : "Ctrl";

function LevelBuilderMenu::destroy( %this )
{
   LevelBuilderBase.menuGroup.delete();
}

// [neo, 5/31/2007 - #3174]
// Refactored menu attach stuff so we can call it as needed
// e.g. before and after display changes, etc.
function LevelBuilderBase::attachMenuGroup( %this )
{
   if( !isObject( %this.menuGroup ) ) 
      return;

   for( %i = 0; %i < %this.menuGroup.getCount(); %i++ )
     %this.menuGroup.getObject( %i ).attachToMenuBar();
}

// [neo, 5/31/2007 - #3174]
// Refactored menu detach stuff so we can call it as needed
// e.g. before and after display changes, etc.
function LevelBuilderBase::detachMenuGroup( %this )
{
   if( !isObject( %this.menuGroup ) ) 
      return;
      
   for( %i = 0; %i < %this.menuGroup.getCount(); %i++ )
      %this.menuGroup.getObject( %i ).removeFromMenuBar();
}

// This is a component of the LevelBuilder GUI so this is called when the GUI Sleeps.
function LevelBuilderBase::onSleep( %this )
{
   // [neo, 5/31/2007 - #3174]
   // Refactored code to detachMenuGroup();
   detachMenuBars();
}

function LevelBuilderBase::onWake( %this )
{
   if( %this.getID() != Canvas.getContent().getID() )
      return;
 
   // [neo, 5/31/2007 - #3174]
   // Refactored to attachMenuGroup();
   attachMenuBars();   
}