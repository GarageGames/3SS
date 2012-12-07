//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------


//-----------------------------------------------------------------------------
// TGBEffectFieldList Class - Example Use
//-----------------------------------------------------------------------------
//
//%MySelectionList = new GuiControl() 
//{
//   superClass              = TGBEmitterFieldList;
//   class                   = TGBGraphFieldList;
//   fieldsObject            = %field.getID();
//};

function TGBEmitterFieldList::onAdd(%this)
{
   // Parent first.
   Parent::onAdd(%this);
   
   // Verify Emitter Object.
   if( !isObject( %this.fieldsObject ) )
   { 
      warn("TGBEmitterFieldList::onAdd - Invalid Emitter Object Specified!");
      return false;
   }
         
   // Initialize with Data Keys!
   %this.initGraph();
}

function TGBEmitterFieldList::onRemove(%this)
{
   // Parent Last!
   Parent::onRemove( %this );
}

function TGBEmitterFieldList::UpdateDisplay( %this )
{
   // Parent First! (Does Sanity Checking)
   Parent::UpdateDisplay( %this );
}

//-----------------------------------------------------------------------------
// Initialize Emitter Graphs
//-----------------------------------------------------------------------------
function TGBEmitterFieldList::initGraph(%this)
{
   // here we init all the graphs for the effect with the default settings
   // and set the max and min
   %fieldGraph = %this.Graph.findObjectByInternalName( "FieldGraph" );
   
   if( !isObject( %fieldGraph ) )
      return;
   
   // particlelife_base
   %this.AddGraphEntry( "Particle Life Base", "particlelife_base", 0, 100, 0, 1 );   
   // particlelife_var
   %this.AddGraphEntry( "Particle Life Variance", "particlelife_var", 0, 200, 0, 1 );  
   // quantity_base
   %this.AddGraphEntry( "Quantity Base", "quantity_base", 0, 1000, 0, 1 );   
   // quantity_var
   %this.AddGraphEntry( "Quantity Variance", "quantity_var", 0, 1000, 0, 1 );   
   // sizex_base
   %this.AddGraphEntry( "Size X Base", "sizex_base", 0, 100, 0, 1 );  
   // sizex_var
   %this.AddGraphEntry( "Size X Variance", "sizex_var", 0, 200, 0, 1 );   
   // sizex_life
   %this.AddGraphEntry( "Size X Life", "sizex_life", -100, 100, 0, 1 );   
   // sizey_base
   %this.AddGraphEntry( "Size Y Base", "sizey_base", 0, 100, 0, 1 );   
   // sizey_var
   %this.AddGraphEntry( "Size Y Variance", "sizey_var", 0, 200, 0, 1 );
   // sizey_life
   %this.AddGraphEntry( "Size Y Life", "sizey_life", -100, 100, 0, 1 );
   // speed_base
   %this.AddGraphEntry( "Speed Base", "speed_base", 0, 100, 0, 1 );
   // speed_var
   %this.AddGraphEntry( "Speed Variance", "speed_var", 0, 200, 0, 1 );   
   // speed_life
   %this.AddGraphEntry( "Speed Life", "speed_life", -100, 100, 0, 1 );   
   // spin_base
   %this.AddGraphEntry( "Spin Base", "spin_base", -1000, 1000, 0, 1 );      
   // spin_var
   %this.AddGraphEntry( "Spin Variance", "spin_var", 0, 2000, 0, 1 );      
   // spin_life
   %this.AddGraphEntry( "Spin Life", "spin_life", -1000, 1000, 0, 1 );      
   // fixedforce_base
   %this.AddGraphEntry( "Fixed Force Base", "fixedforce_base", -100, 100, 0, 1 );      
   // fixedforce_var
   %this.AddGraphEntry( "Fixed Force Variance", "fixedforce_var", 0, 200, 0, 1 );      
   // fixedforce_life
   %this.AddGraphEntry( "Fixed Force Life", "fixedforce_life", -100, 100, 0, 1 );      
   // randommotion_base
   %this.AddGraphEntry( "Random Motion Base", "randommotion_base", 0, 100, 0, 1 );      
   // randommotion_var
   %this.AddGraphEntry( "Random Motion Variance", "randommotion_var", 0, 200, 0, 1 );    
   // randommotion_life
   %this.AddGraphEntry( "Random Motion Life", "randommotion_life", -100, 100, 0, 1 );    
   // emissionforce_base
   %this.AddGraphEntry( "Emission Force Base", "emissionforce_base", -100, 100, 0, 1 );       
   // emissionforce_var
   %this.AddGraphEntry( "Emission Force Variance", "emissionforce_var", 0, 200, 0, 1 );       
   // emissionangle_base
   %this.AddGraphEntry( "Emission Angle Base", "emissionangle_base", -180, 180, 0, 1 );       
   // emissionangle_var
   %this.AddGraphEntry( "Emission Angle Variance", "emissionangle_var", 0, 360, 0, 1 );       
   // emissionarc_base
   %this.AddGraphEntry( "Emission Arc Base", "emissionarc_base", 0, 360, 0, 1 );       
   // emissionarc_var
   %this.AddGraphEntry( "Emission Arc Variance", "emissionarc_var", 0, 720, 0, 1 );       
   // red_life
   %this.AddGraphEntry( "Red Color Life", "red_life", 0, 1, 0, 1 );       
   // green_life
   %this.AddGraphEntry( "Green Color Life", "green_life", 0, 1, 0, 1 );       
   // blue_life
   %this.AddGraphEntry( "Blue Color Life", "blue_life", 0, 1, 0, 1 );       
   // visibility_life
   %this.AddGraphEntry( "Visibility Life", "visibility_life", 0, 1, 0, 1 );       
}