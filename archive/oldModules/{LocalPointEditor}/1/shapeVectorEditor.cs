//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function ShapeVectorEditor::getLocalPoints(%this)
{
   %poly = %this.baseObject.getPoly();
   
   for (%i=0; %i<getWordCount(%poly); %i+=2)
   {
      %this.createNewLocalPoint(getWords(%poly, %i, %i+1));
   }
}

function ShapeVectorEditor::save(%this)
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
   %undo = new UndoScriptAction()
   {
      class = ShapeVectorEditorUndo;
      actionName = "Modify polygon";
      object = %this.baseObject;
      oldPoly = %this.baseObject.getPoly();
      newPoly = %newPoints;
   };
   %undo.addToManager(LevelBuilderUndoManager);

   %this.baseObject.setPolyCustom(%length, %newPoints);
   %this.close();
}

function ShapeVectorEditor::saveHullAsCollisionPoly(%this)
{
   %hull = %this.calculateConvexHull();
   %length = getWordCount(%hull);
   %newPoints = "";
   for (%i=0; %i<getWordCount(%hull); %i++)
   {
      %obj = getWord(%hull, %i);
      %objX = getWord(%obj.position, 0);
      %objY = getWord(%obj.position, 1);
      %newPoints = setWord(%newPoints, %i*2, %objX);
      %newPoints = setWord(%newPoints, %i*2+1, %objY);
   }
   
   // register change with undo system.
   %undo = new UndoScriptAction()
   {
      class = CollisionPolyEditorUndo;
      actionName = "Modify collsion polygon";
      object = %this.baseObject;
      oldPoly = %this.baseObject.getCollisionPoly();
      newPoly = %newPoints;
   };
   %undo.addToManager(LevelBuilderUndoManager);

   %this.baseObject.setCollisionPolyCustom(%length, %newPoints);
   %this.close();   
}

function ShapeVectorEditorUndo::undo(%this)
{
   %this.object.setPolyCustom(getWordCount(%this.oldPoly)/2, %this.oldPoly);
}

function ShapeVectorEditorUndo::redo(%this)
{
   %this.object.setPolyCustom(getWordCount(%this.newPoly)/2, %this.newPoly);
}
