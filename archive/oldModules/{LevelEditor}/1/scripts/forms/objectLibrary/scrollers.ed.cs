//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
$LBCScroller = GuiFormManager::AddFormContent( "LevelBuilderSidebarCreate", "Scrollers", "LBCScroller::CreateForm", "LBCScroller::SaveForm", 2 );

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBCScroller::CreateForm( %contentCtrl )
{    

   // Create necessary objects.
   %base = ObjectLibraryBaseType::CreateContent( %contentCtrl, "LBOTScroller" );
   %base.sortOrder = 3;

   // Set Caption.
   %base.caption = "Scrollers";

   // Return Base.
   return %base;

}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBCScroller::SaveForm( %formCtrl, %contentObj )
{
   %formCtrl.sSpritesExpanded = %contentObj.sSpritesExpanded;
}
   
function LBOTScroller::refresh( %this, %resize )
{
   %this.destroy();
   
   %this.scene = new Scene();
   
   
   $LB::ObjectLibraryGroup.add( %this.scene );
   
   %datablockSet = DataBlockGroup;
   %datablockCount = %datablockSet.getCount();

   // Find objectList
   %objectList = %this.findObjectByInternalName("ObjectList");
   %objectsAdded = 0;
   
   for (%i = 0; %i < %datablockCount; %i++ )
   {
      %object = %datablockSet.getObject( %i );
      if( %object.getClassName() $= "ImageAsset" )
      {
         // Create Sprite Object
        %staticSprite = new Scroller()  { scene = %this.Scene; };
        %staticSprite.setImageMap( %object.getName() );
        %staticSprite.setImageFrame( 0 );
        %staticSprite.setSize( %object.getFrameSize(0) );

        %objectList.AddT2DObject( %staticSprite, %object.getName(), "Scroller" );
        %objectsAdded++;
      }
   }
   
   if( %resize )
   {
      if( %objectsAdded > 0 )
         %this.sizeToContents();
      else
         %this.instantCollapse();
   }
}


function LBOTScroller::onRemove( %this )
{
   // I don't understand this, but sometimes in onSleep 
   // for this content, the %this object is bad. :(
   if( !isObject( %this ) )
      return;
      
   %this.destroy();
}

function LBOTScroller::destroy( %this )
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

function LBOTScroller::onContentMessage(%this, %sender, %message)
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
