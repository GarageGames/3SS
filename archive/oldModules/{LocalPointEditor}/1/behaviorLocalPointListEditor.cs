//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function EditBehaviorLocalPointList(%object, %behavior, %field)
{
   BehaviorLocalPointListEditor.sourceBehavior = %behavior;
   BehaviorLocalPointListEditor.sourceField = %field;
   BehaviorLocalPointListEditor.open(%object);
}

function BehaviorLocalPointListEditor::getLocalPoints(%this)
{
   if (isObject(%this.sourceBehavior))
   {
      %lplist = %this.sourceBehavior.getFieldValue(%this.sourceField);
   
      for (%i=0; %i<getWordCount(%lplist); %i+=2)
      {
         %this.createNewLocalPoint(getWords(%lplist, %i, %i+1));
      }
   }
}

function BehaviorLocalPointListEditor::save(%this)
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
      %oldPointList = %this.sourceBehavior.getFieldValue(%this.sourceField);
      %undo = new UndoScriptAction()
      {
         class = BehaviorLocalPointListEditorUndo;
         actionName = "Modify behavior point list";
         object = %this.baseObject;
         sourceBehavior = %this.sourceBehavior;
         sourceField = %this.sourceField;
         oldList = %oldPointList;
         newList = %newPoints;
      };
      %undo.addToManager(LevelBuilderUndoManager);
   
      %this.sourceBehavior.setFieldValue(%this.sourceField, %newPoints);
      %this.close();
   }
}

function BehaviorLocalPointListEditorUndo::undo(%this)
{
   %this.sourceBehavior.setFieldValue(%this.sourceField, %this.oldList);
}

function BehaviorLocalPointListEditorUndo::redo(%this)
{
   %this.sourceBehavior.setFieldValue(%this.sourceField, %this.newList);
}
