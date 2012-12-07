//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LocalPointVisualEdit::onRightMouseDown(%this, %modifier, %worldPosition, %clicks)
{
   // find the editor we're serving.
   %editor = LocalPointEditorGui.editorObject;

   %pickedObjects = %this.getScene().pickpoint(%worldPosition, 0x1);
   
   %selected = NULL;
   for (%i=0; %i<getWordCount(%pickedObjects); %i++)
   {
      %candidateObject = getWord(%pickedObjects, %i);
      if ((!isObject(%selected)) || (Vector2Distance(%candidateObject.position, %worldPosition) <
                                     Vector2Distance(%selected.position, %worldPosition)))
      {
         %selected = %candidateObject;
      }
   }

   if ((isObject(%selected)) && (isObject(%selected.lpobject)))
   {
      // clicked a dot, delete it!
      %editor.deletePoint(%selected.lpobject);
      %this.activeDot = NULL;
   }
}

function LocalPointVisualEdit::onTouchDown(%this, %modifier, %worldPosition, %clicks)
{
   // find the editor we're serving.
   %editor = LocalPointEditorGui.editorObject;

   // get all objects from group 0 (i.e. the graphical dots)
   %pickedObjects = %this.getScene().pickpoint(%worldPosition, 0x1);
   
   %selected = NULL;
   for (%i=0; %i<getWordCount(%pickedObjects); %i++)
   {
      %candidateObject = getWord(%pickedObjects, %i);
      if ((!isObject(%selected)) || (Vector2Distance(%candidateObject.position, %worldPosition) <
                                     Vector2Distance(%selected.position, %worldPosition)))
      {
         if(isObject(%candidateObject.lpobject))
            %selected = %candidateObject;
      }
   }

   if (isObject(%selected))
   {
      %this.activeDot = %selected;
   }
   else
   {
      %localPosition = %editor.SceneSpaceToLocalSpace(%worldPosition);
      %this.activeDot = (%editor.newLocalPoint(%localPosition)).dot;
   }
}

function LocalPointVisualEdit::onTouchUp(%this, %modifier, %worldPosition, %clicks)
{
   // find the editor we're serving.
   %editor = LocalPointEditorGui.editorObject;

   if (isObject(%this.activeDot))
   {
      %this.activeDot.lpObject.setPosition(%editor.SceneSpaceToLocalSpace(%worldPosition), true);
   }
   %this.activeDot = NULL;
}

function LocalPointVisualEdit::onTouchDragged(%this, %modifier, %worldPosition, %clicks)
{
   // find the editor we're serving.
   %editor = LocalPointEditorGui.editorObject;

   if (isObject(%this.activeDot))
   {
      %this.activeDot.lpObject.setPosition(%editor.SceneSpaceToLocalSpace(%worldPosition));      
   }
}

function LocalPointVisualEdit::onMouseLeave(%this, %modifier, %worldPosition, %clicks)
{
   %this.onTouchUp(%modifier, %worldPosition, %clicks);
}
