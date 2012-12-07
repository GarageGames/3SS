//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function EditorMenuBar::initialize( %this )
{
   %this.clearMenus();
   %this.menuCount = 0;
}

function EditorMenuBar::addMenu( %this, %name )
{
   Parent::addMenu( %this, %name, %this.menuCount );
   %this.menuCount++;
   %this.itemCount[%name] = 0;
}

function EditorMenuBar::addMenuItem( %this, %menu, %text, %command, %hotkey )
{
   %item = %this.itemCount[%menu];
   
   Parent::addMenuItem( %this, %menu, %text, %item, %hotkey );
   %this.scriptCommand[%menu, %item] = %command;
   %this.itemCount[%menu]++;
   
   return %item;
}


function detachMenuBars()
{
   %content = Canvas.getContent();
   if( !isObject( %content ) || !isObject( %content.menuGroup ) )
      return;
      
   if( %content.isMethod( "detachMenuGroup" ) )
      %content.detachMenuGroup();
}

function attachMenuBars()
{
   %content = Canvas.getContent();
   if( !isObject( %content ) || !isObject( %content.menuGroup ) )
      return;
      
   if( %content.isMethod( "attachMenuGroup" ) )
      %content.attachMenuGroup();   
}