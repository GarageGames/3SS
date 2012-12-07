//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function CollisionPolyEditor::getLocalPoints(%this)
{
   %poly = %this.baseObject.getCollisionPoly();
   
   for (%i=0; %i<getWordCount(%poly); %i+=2)
   {
      %this.createNewLocalPoint(getWords(%poly, %i, %i+1));
   }
}

function CollisionPolyEditor::save(%this)
{
   %hull = %this.calculateConvexHull();

   // need to check if we're convex or not.  Need to rotate the local points, since
   // local point 0 may not be hull point 0, even if the local points do make up
   // a convex hull.
   %numPoints = getWordCount(%this.localPoints);
   %firstPoint = getWord(%hull, 0);
   for (%f=0; %f<%numPoints; %f++)
   {
      %lp = getWord(%this.localPoints, %f);
      if (%firstPoint.getId() == %lp.getId())
         break;
   }
   
   for (%i=0; %i<getWordCount(%this.localPoints); %i++)
   {
      %offset = (%f+%i) % %numPoints;
      %rotatedLocal = setWord(%rotatedLocal, %i, getWord(%this.localPoints, %offset));
   }
   
   if ((getWordCount(%hull) != getWordCount(%this.localPoints)) || (%rotatedLocal !$= %hull))
   {
      MessageBoxYesNo("Save convex hull only?", "Polygon is not convex, save convex hull instead?", 
                      %this @ ".saveHull();", %this @ ".cancelSave();");
   }
   else
   {
      // valid collision poly.
      %this.savePoly();
   }
}

function CollisionPolyEditor::cancelSave(%this)
{
}

function CollisionPolyEditor::savePoly(%this)
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
      class = CollisionPolyEditorUndo;
      actionName = "Modify collision polygon";
      object = %this.baseObject;
      oldPoly = %this.baseObject.getCollisionPoly();
      newPoly = %newPoints;
   };
   %undo.addToManager(LevelBuilderUndoManager);
   
   %this.baseObject.setCollisionPolyCustom(%length, %newPoints);
   %this.close();
}

function CollisionPolyEditor::saveHull(%this)
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

function CollisionPolyEditorUndo::undo(%this)
{
   %this.object.setCollisionPolyCustom(getWordCount(%this.oldPoly)/2, %this.oldPoly);
}

function CollisionPolyEditorUndo::redo(%this)
{
   %this.object.setCollisionPolyCustom(getWordCount(%this.newPoly)/2, %this.newPoly);
}
