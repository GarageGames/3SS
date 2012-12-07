//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function EditBehaviorPolygon(%object, %behavior, %field)
{
   BehaviorPolyEditor.sourceBehavior = %behavior;
   BehaviorPolyEditor.sourceField = %field;
   BehaviorPolyEditor.open(%object);
}

function BehaviorPolyEditor::getLocalPoints(%this)
{
   if (isObject(%this.sourceBehavior))
   {
      %poly = %this.sourceBehavior.getFieldValue(%this.sourceField);
   
      for (%i=0; %i<getWordCount(%poly); %i+=2)
      {
         %this.createNewLocalPoint(getWords(%poly, %i, %i+1));
      }
   }
}

function BehaviorPolyEditor::save(%this)
{
   if (isObject(%this.sourceBehavior))
   {
      %length = getWordCount(%this.localPoints);
      %newPoints = "";
      for (%i=0; %i<getWordCount(%this.localPoints); %i++)
      {
         %obj = getWord(%this.localPoints, %i);
         %objX = getWord(%obj.position, 0);
         %objY = getWord(%obj.position, 1);
         %newPoints = setWord(%newPoints, %i*2, %objX);
         %newPoints = setWord(%newPoints, %i*2+1, %objY);
      }
   
      // register change with undo system.
      %oldPoly = %this.sourceBehavior.getFieldValue(%this.sourceField);
      %undo = new UndoScriptAction()
      {
         class = BehaviorPolyEditorUndo;
         actionName = "Modify behavior polygon";
         object = %this.baseObject;
         sourceBehavior = %this.sourceBehavior;
         sourceField = %this.sourceField;
         oldPoly = %oldPoly;
         newPoly = %newPoints;
      };
      %undo.addToManager(LevelBuilderUndoManager);
   
      %this.sourceBehavior.setFieldValue(%this.sourceField, %newPoints);
      %this.close();
   }
}

function BehaviorPolyEditorUndo::undo(%this)
{
   %this.sourceBehavior.setFieldValue(%this.sourceField, %this.oldPoly);
}

function BehaviorPolyEditorUndo::redo(%this)
{
   %this.sourceBehavior.setFieldValue(%this.sourceField, %this.newPoly);
}
