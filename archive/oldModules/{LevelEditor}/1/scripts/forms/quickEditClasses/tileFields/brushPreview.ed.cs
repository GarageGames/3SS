//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LBQuickEditContent::createBrushPreview(%this)
{
   %container = new GuiControl() {
      canSaveDynamicFields = "0";
      Profile = "EditorContainerProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = "300 138";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
   };
   
   %scene = new Scene();
   
   // -- render control
   %t2dContainer = new GuiSceneObjectCtrl()
   {
      class = "QuickEditBrushPreview";
      Profile = ObjectBrowserThumbProfile @ $levelEditor::ObjectLibraryBackgroundColor;
      RenderMargin = 6;
      extent = "128 128";
      position = "10 5";
      toolTipProfile = EditorToolTipProfile;
      scene = %scene;
   };
     
   %container.add( %t2dContainer );
   
   %this.addProperty(%t2dContainer);
   %this.add(%container);
   return %container;
}

function QuickEditBrushPreview::setProperty(%this)
{
   %image = ActiveBrush.getImage();
   %frame = ActiveBrush.getFrame();
   %flipX = ActiveBrush.getFlipX() > 0;
   %flipY = ActiveBrush.getFlipY() > 0;
   
   %object = "";
   
   if (isObject(%image))
   {
      if (%image.getClassName() $= "AnimationAsset")
      {
         %object = new t2dAnimatedSprite()
         {
            scene = %this.scene;
            animationName = %image;
         };
      }
         
      else if (%image.getClassName() $= "ImageAsset")
      {
         %object = new t2dStaticSprite()
         {
            scene = %this.scene;
            imageMap = %image;
            frame = %frame;
         };
      }
   }
   
   else
   {
      %object = new SceneObject()
      {
         scene = %this.scene;
      };
   }
   
   if (%object !$= "")
   {
      %object.setFlip(%flipX, %flipY);
      %object.setPosition(%object.getPosition());
   }
   
   %oldObject = %this.getSceneObject();
   if (isObject(%oldObject))
      %oldObject.delete();
   
   if (isObject(%object))
   {
      %size = ActiveBrush.getTileSize();
      %ar = getWord( %size, 0 ) / getWord( %size, 1 );
      %x = 20;
      %y = 20;
      if( %ar > 1 )
         %y /= %ar;
      else if( %ar < 1 )
         %x *= %ar;
         
      %object.setSize( %x SPC %y );
   }
   
   %this.setSceneObject( %object );
}
