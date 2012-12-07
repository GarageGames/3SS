//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LBQuickEditContent::sizeStack( %this )
{
   // Get to the children first.   
   %count = %this.getCount();
   for( %i = 0; %i < %count; %i++ )
      LBQuickEditContent::sizeStack( %this.getObject( %i ) );

   // Now act appropriately.   
   if( isObject( %this.rolloutCtrl ) && %this.rolloutCtrl.isExpanded() )
      %this.rolloutCtrl.sizeToContents();    
   if( %this.getClassName() $= "GuiStackControl" )
      %this.updateStack();
   else if( %this.getClassName() $= "GuiRolloutCtrl" && %this.isExpanded() )
      %this.sizeToContents();
}