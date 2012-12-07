//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function validateDatablockName(%name)
{
   // remove whitespaces at beginning and end
   %name = trim( %name );
   
   // remove numbers at the beginning
   %numbers = "0123456789";   
   while( strlen(%name) > 0 )
   {
      // the first character
      %firstChar = getSubStr( %name, 0, 1 );
      // if the character is a number remove it
      if( strpos( %numbers, %firstChar ) != -1 )
      {
         %name = getSubStr( %name, 1, strlen(%name) -1 );
         %name = ltrim( %name );
      }
      else
         break;
   }
   
   // replace whitespaces with underscores
   %name = strreplace( %name, " ", "_" );
   
   // remove any other invalid characters
   %invalidCharacters = "-+*/%$&§=()[].?\"#,;!~<>|°^{}";
   %name = stripChars( %name, %invalidCharacters );
   
   if( %name $= "" )
      %name = "Unnamed";
   
   return %name;
}

//--------------------------------------------------------------------------
// Finds location of %word in %text, starting at %start.  Works just like strPos
//--------------------------------------------------------------------------

function wordPos(%text, %word, %start)
{
   if (%start $= "") %start = 0;
   
   if (strpos(%text, %word, 0) == -1) return -1;
   %count = getWordCount(%text);
   if (%start >= %count) return -1;
   for (%i = %start; %i < %count; %i++)
   {
      if (getWord( %text, %i) $= %word) return %i;
   }
   return -1;
}

//--------------------------------------------------------------------------
// Finds location of %field in %text, starting at %start.  Works just like strPos
//--------------------------------------------------------------------------

function fieldPos(%text, %field, %start)
{
   if (%start $= "") %start = 0;
   
   if (strpos(%text, %field, 0) == -1) return -1;
   %count = getFieldCount(%text);
   if (%start >= %count) return -1;
   for (%i = %start; %i < %count; %i++)
   {
      if (getField( %text, %i) $= %field) return %i;
   }
   return -1;
}

//--------------------------------------------------------------------------
// returns the text in a file with "\n" at the end of each line
//--------------------------------------------------------------------------

function loadFileText( %file)
{
   %fo = new FileObject();
   %fo.openForRead(%file);
   %text = "";
   while(!%fo.isEOF())
   {
      %text = %text @ %fo.readLine();
      if (!%fo.isEOF()) %text = %text @ "\n";
   }

   %fo.delete();
   return %text;
}

function setValueSafe(%dest, %val)
{
   %cmd = %dest.command;
   %alt = %dest.altCommand;
   %dest.command = "";
   %dest.altCommand = "";

   %dest.setValue(%val);
   
   %dest.command = %cmd;
   %dest.altCommand = %alt;
}

function shareValueSafe(%source, %dest)
{
   setValueSafe(%dest, %source.getValue());
}

function shareValueSafeDelay(%source, %dest, %delayMs)
{
   schedule(%delayMs, 0, shareValueSafe, %source, %dest);
}


//------------------------------------------------------------------------------
// An Aggregate Control is a plain GuiControl that contains other controls, 
// which all share a single job or represent a single value.
//------------------------------------------------------------------------------

// AggregateControl.setValue( ) propagates the value to any control that has an 
// internal name.
function AggregateControl::setValue(%this, %val)
{
   for(%i = 0; %i < %this.getCount(); %i++)
   {
      %obj = %this.getObject(%i);
      if(%obj.internalName !$= "")
         setValueSafe(%obj, %val);
   }
}

// AggregateControl.getValue() uses the value of the first control that has an
// internal name, if it has not cached a value via .setValue
function AggregateControl::getValue(%this)
{
   for(%i = 0; %i < %this.getCount(); %i++)
   {
      %obj = %this.getObject(%i);
      if(%obj.internalName !$= "")
      {
         error("obj = " @ %obj.getId() @ ", " @ %obj.getName() @ ", " @ %obj.internalName );
         error(" value = " @ %obj.getValue());
         return %obj.getValue();
      }
   }
}

// AggregateControl.updateFromChild( ) is called by child controls to propagate
// a new value, and to trigger the onAction() callback.
function AggregateControl::updateFromChild(%this, %child)
{
   %val = %child.getValue();
   %this.setValue(%val);
   %this.onAction();
}

// default onAction stub, here only to prevent console spam warnings.
function AggregateControl::onAction(%this) 
{
}

