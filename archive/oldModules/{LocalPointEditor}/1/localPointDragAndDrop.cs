//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LocalPointHolder::onTouchDown(%this, %modifier, %pos, %clickCount)
{
   %offset = Vector2Sub(%pos, %this.getGlobalPosition());
   %dragger = new GuiDragAndDropControl()
   {
      size = %this.size;
      extent = %this.extent;
      profile = EditorPanelTransparentModeless;
      deleteOnMouseUp = true;
      localPoint = %this.localPoint;
   };
   
   %dotLabelBG = new GuiControl() {
      canSaveDynamicFields = "0";
      Profile = LocalPointEditorDotBGProfile;
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "8 8";
      Extent = "16 16";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
   };
      
   %dotLabel = new GuiTextCtrl() {
      canSaveDynamicFields = "0";
      Profile = LocalPointEditorDotTextProfile;
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "8 8";
      Extent = "16 16";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      text = "X";
      justify = "Center";
   };
   %dotLabel.text = %this.localPoint.index;

   
   %dragger.add(%dotLabelBG);
   %dragger.add(%dotLabel);
   
   LocalPointEditorGui.add(%dragger);
   %offset = Vector2Sub(%pos, %this.getGlobalPosition());
   %dragger.startDragging(getWord(%offset, 0), getWord(%offset, 1));
   
}

function LocalPointHolder::onControlDropped(%this, %dropped, %pos)
{
   // find the editor we're serving.
   %editor = LocalPointEditorGui.editorObject;

   %offset = Vector2Sub(%pos, %this.getGlobalPosition());
   %oy = getWord(%offset, 1);
   %ey = getWord(%this.extent, 1);
   
   if (%dropped.localPoint != %this.localPoint)
   {
      if (%oy > (%ey/2))
      {
         // insert after!
         %editor.movePoint(%dropped.localPoint.index, %this.localPoint.index+1, true);
      }
      else
      {
         // insert before!
         %editor.movePoint(%dropped.localPoint.index, %this.localPoint.index, true);
      }
   }
}

function LETextScroll::onControlDropped(%this, %dropped, %pos)
{
   // find the editor we're serving.
   %editor = LocalPointEditorGui.editorObject;
   %editor.movePoint(%dropped.localPoint.index, getWordCount(%editor.localPoints) - 1);
}
