//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
$LBCOther = GuiFormManager::AddFormContent( "LevelBuilderSidebarCreate", "Other", "LBCOther::CreateForm", "LBCOther::SaveForm", 2 );

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBCOther::CreateForm( %contentCtrl )
{    
   // Create necessary objects.
   %base = ObjectLibraryBaseType::CreateContent( %contentCtrl, "LBOTOther" );
   %base.sortOrder = 7;

   // Set Caption.
   %base.caption = "Other";

   // Return Base.
   return %base;

}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBCOther::SaveForm( %formCtrl, %contentObj )
{
   %formCtrl.sOtherExpanded = %contentObj.sOtherExpanded;
}

function LBOTOther::refresh( %this, %resize )
{
   %this.destroy();
   
   %this.scene = new Scene();
   
   $LB::ObjectLibraryGroup.add( %this.scene );
   
   // Find objectList
   %objectList = %this.findObjectByInternalName("ObjectList");
   
   %sceneObject = new t2dStaticSprite() { scene = %this.Scene; imageMap = sceneObjectImageMap; };
   %objectList.AddT2DObject(%sceneObject, "Scene Object", "SceneObject","Scene Object");
   
   %sceneObject = new t2dStaticSprite() { scene = %this.Scene; imageMap = triggerObjectImageMap; };
   %objectList.AddT2DObject(%sceneObject, "Trigger", "Trigger", "Trigger");
   
   %sceneObject = new t2dStaticSprite() { scene = %this.Scene; imageMap = pathObjectImageMap; };
   %objectList.AddT2DObject(%sceneObject, "Path", "Path", "Path");
   
   %sceneObject = new t2dStaticSprite() { scene = %this.Scene; imageMap = bitmapObjectImageMap; };
   %objectList.AddT2DObject(%sceneObject, "Text", "BitmapFontObject", "Bitmap Font");
   
   %sceneObject = new t2dStaticSprite() { scene = %this.Scene; imageMap = textObjectImageMap; };
   %objectList.AddT2DObject(%sceneObject, "Text", "TextObject", "Text Object");

   %sceneObject = new t2dStaticSprite() { scene = %this.Scene; imageMap = shapeVectorImageMap; };
   %objectList.AddT2DObject(%sceneObject, "Polygon", "ShapeVector", "Polygon");
}


function LBOTOther::onRemove( %this )
{
   // I don't understand this, but sometimes in onSleep 
   // for this content, the %this object is bad. :(
   if( !isObject( %this ) )
      return;

   %this.destroy();
}

function LBOTOther::destroy( %this )
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

function LBOTOther::onContentMessage(%this, %sender, %message)
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
