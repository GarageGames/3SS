//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
$LBCParticleEffect = GuiFormManager::AddFormContent( "LevelBuilderSidebarCreate", "ParticleEffects", "LBCParticleEffect::CreateForm", "LBCParticleEffect::SaveForm", 2 );

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBCParticleEffect::CreateForm( %contentCtrl )
{    

   // Create necessary objects.
   %base = ObjectLibraryBaseType::CreateContent( %contentCtrl, "LBOTParticleEffect" );
   %base.sortOrder = 4;

   // Set Caption.
   %base.caption = "Particle Effects";

   // Return Base.
   return %base;

}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBCParticleEffect::SaveForm( %formCtrl, %contentObj )
{
   %formCtrl.sSpritesExpanded = %contentObj.sSpritesExpanded;
}
   
function LBOTParticleEffect::refresh( %this, %resize )
{
   %this.destroy();
   
   %this.scene = new Scene();
   
   $LB::ObjectLibraryGroup.add( %this.scene );
   
   // Find objectList
   %objectList = %this.findObjectByInternalName("ObjectList");

   // Add Empty Effect
   %defaultEffectFile = expandPath("./newEffect.eff");
   %particleEffect = new t2dStaticSprite()
   {
      scene = %this.Scene;
      imageMap = particleEffectIconImageMap;
   };
   %objectList.AddT2DObject( %particleEffect, %defaultEffectFile, "ParticleEffect", fileBase( %defaultEffectFile ) );
   
   %objectsAdded = 0;
   
   
   %path = expandPath( "^project/data/particles/" );
   removeResPath( %path @ "*" );
   addResPath( %path );
   
   %fileSpec = %path @ "*.eff";
   for (%file = findFirstFile(%fileSpec); %file !$= ""; %file = findNextFile(%fileSpec))
   {

	   // Create Effect Object
      %particleEffect = new ParticleEffect()  { scene = %this.Scene; };
      %particleEffect.loadEffect( %file );
      %particleEffect.setSize( "15 15" );
      %particleEffect.moveEffectTo(2.0, 0.5);
      %particleEffect.setEffectPaused( true );
      %particleEffect.setEffectLifeMode("CYCLE", 2.0);

      %objectList.AddT2DObject( %particleEffect, %file, "ParticleEffect", fileBase( %file ) );
      
      %objectsAdded++;
   }
   
   if( %resize )
   {
      if( %objectsAdded > 0 )
         %this.sizeToContents();
      else
         %this.instantCollapse();
   }
}


function LBOTParticleEffect::onRemove( %this )
{
   // I don't understand this, but sometimes in onSleep 
   // for this content, the %this object is bad. :(
   if( !isObject( %this ) )
      return;
      
   %this.destroy();
}

function LBOTParticleEffect::destroy( %this )
{
   if (isObject(%this.scene))
      %this.scene.delete();
      
   // Find objectList
   %objectList = %this.findObjectByInternalName("ObjectList");

   if( !isObject( %objectList ) )
      return;

   while( %objectList.getCount() > 0 )
   {
      %object = %objectList.getObject( 0 );
      if( isObject( %object ) )
         %object.delete();
      else
         %objectList.remove( %object );
   }
}

function LBOTParticleEffect::onContentMessage(%this, %sender, %message)
{
   %messageCommand = GetWord( %message, 0 );
   switch$( %messageCommand )
   {
      case "refresh":
         %this.refresh();
         
      case "destroy":
         %this.destroy();
   }
}
