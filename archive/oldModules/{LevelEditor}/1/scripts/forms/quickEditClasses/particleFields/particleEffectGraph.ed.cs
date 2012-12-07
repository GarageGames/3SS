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
//   superClass              = TGBEffectFieldList;
//   class                   = TGBGraphFieldList;
//   fieldsObject            = %field.getID();
//};

function TGBEffectFieldList::onAdd(%this)
{
   // Parent first.
   Parent::onAdd(%this);
   
   // Verify Emitter Object.
   if( !isObject( %this.fieldsObject ) )
   { 
      warn("TGBEffectFieldList::onAdd - Invalid Effect Object Specified!");
      return false;
   }
         
   // Initialize with Data Keys!
   %this.initGraph();
}

function TGBEffectFieldList::onRemove(%this)
{
   // Parent Last!
   Parent::onRemove( %this );
}

function TGBEffectFieldList::UpdateDisplay( %this )
{
   // Parent First! (Does Sanity Checking)
   Parent::UpdateDisplay( %this );
}

//-----------------------------------------------------------------------------
// Initialize Emitter Graphs
//-----------------------------------------------------------------------------
function TGBEffectFieldList::initGraph(%this)
{
   // here we init all the graphs for the effect with the default settings
   // and set the max and min
   if( !isObject( %this.Graph ) )
      return;
   %fieldGraph = %this.Graph.findObjectByInternalName( "FieldGraph" );

   if( !isObject( %fieldGraph ) )
      return;
   
   // particlelife_scale
   %this.AddGraphEntry( "Particle Life Scale", "particlelife_scale", 0, 100, 0, 1 );   
   // quantity_scale
   %this.AddGraphEntry( "Quantity Scale", "quantity_scale", 0, 100, 0, 1 );     
   // sizex_scale
   %this.AddGraphEntry( "Size X Scale", "sizex_scale", 0, 100, 0, 1 );     
   // sizey_scale
   %this.AddGraphEntry( "Size Y Scale", "sizey_scale", 0, 100, 0, 1 );     
   // speed_scale
   %this.AddGraphEntry( "Speed Scale", "speed_scale", 0, 100, 0, 1 );        
   // spin_scale
   %this.AddGraphEntry( "Spin Scale", "spin_scale", -100, 100, 0, 1 );   
   // fixedforce_scale
   %this.AddGraphEntry( "Fixed Force Scale", "fixedforce_scale", -100, 100, 0, 1 );
   // randommotion_scale
   %this.AddGraphEntry( "Random Motion Scale", "randommotion_scale", 0, 100, 0, 1 );  
   // visibility_scale
   %this.AddGraphEntry( "Visibility Scale", "visibility_scale", 0, 100, 0, 1 );  
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

}



function LBQuickEditContent::createParticleEffectToolbar( %this )
{
   %container = new GuiControl() {
      canSaveDynamicFields = "0";
      Profile = "EditorContainerProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = "300 30";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
   };
   
   // Create Effect Graph Editing Button
   %button = new GuiIconButtonCtrl(ShowEffectGraphButton) {
      canSaveDynamicFields = "0";
      class  = TGBEffectFieldList;
      superClass = TGBGraphFieldList;
      Profile = "EditorButtonMiddle";
      HorizSizing = "left";
      VertSizing = "bottom";
      Position = "268 4";
      Extent = "18 18";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltipprofile = "EditorToolTipProfile";
      tooltip = "Edit This Effects Properties Graph";
      iconBitmap = "^{EditorAssets}/gui/iconGraphLine.png";
      sizeIconToButton = "1";
      ButtonMargin = "0 0";
      fieldsObject = %quickEditObj;
   };
   %container.add( %button );
   
   // Create Save Effect Button
   %button = new GuiIconButtonCtrl() {
      canSaveDynamicFields = "0";
      class  = SaveEffectButton;
      Profile = "EditorButtonMiddle";
      HorizSizing = "left";
      VertSizing = "bottom";
      Position = "248 4";
      Extent = "18 18";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltipprofile = "EditorToolTipProfile";
      tooltip = "Save this effect as an effect file";
      iconBitmap = "^{EditorAssets}/gui/iconSave.png";
      sizeIconToButton = "1";
      ButtonMargin = "0 0";
      fieldsObject = %quickEditObj;
   };
   %container.add( %button );
   
   // Create Load Effect Button
   %button = new GuiIconButtonCtrl() {
      canSaveDynamicFields = "0";
      class  = LoadEffectButton;
      Profile = "EditorButtonMiddle";
      HorizSizing = "left";
      VertSizing = "bottom";
      Position = "228 4";
      Extent = "18 18";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltipprofile = "EditorToolTipProfile";
      tooltip = "Load an Existing Particle Effect into this Object";
      iconBitmap = "^{EditorAssets}/gui/iconOpen.png";
      sizeIconToButton = "1";
      ButtonMargin = "0 0";
      fieldsObject = %quickEditObj;
   };
   %container.add( %button );
   
   // Return Base
   return %container;   
}