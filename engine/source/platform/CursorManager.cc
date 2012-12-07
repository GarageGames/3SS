//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "platform/platformInput.h"
#include "platform/platformVideo.h"
#include "platform/event.h"
#include "console/console.h"

//------------------------------------------------------------------------------
//*** DAW: Cursor Manager Methods
CursorManager* Input::getCursorManager()
{
   return smCursorManager;
}

void CursorManager::pushCursor(S32 cursorID)
{
   //*** Place the new cursor shape onto the stack
   mCursors.increment();
   mCursors.last().mCursorID = cursorID;

   //*** Now actually change the shape
   changeCursorShape(cursorID);
}

void CursorManager::popCursor()
{
   //*** Before poping the stack, make sure we're not trying to remove the last cursor shape
   if(mCursors.size() <= 1)
      return;

   //*** Pop the stack
   mCursors.pop_back();

   //*** Now set the cursor shape
   changeCursorShape(mCursors.last().mCursorID);
}

void CursorManager::refreshCursor()
{
   //*** Refresh the cursor's shape
   changeCursorShape(mCursors.last().mCursorID);
}

void CursorManager::changeCursorShape(S32 cursorID)
{
   if(cursorID >= 0)
      Input::setCursorShape((U32)cursorID);
}

static EnumTable::Enums curManagerShapesEnums[] = 
{
   { CursorManager::curArrow, "Arrow" },
   { CursorManager::curWait, "Wait" },
   { CursorManager::curPlus, "Plus" },
   { CursorManager::curResizeVert, "ResizeVert" },
   { CursorManager::curResizeHorz, "ResizeHorz" },
   { CursorManager::curResizeAll, "ResizeAll" },
   { CursorManager::curIBeam, "ibeam" },
   { CursorManager::curResizeNESW, "ResizeNESW" },
   { CursorManager::curResizeNWSE, "ResizeNWSE" },
};
      
static EnumTable gCurManagerShapesTable(8, &curManagerShapesEnums[0]); 

//*** Console function to set the current cursor shape given the cursor shape
//*** name as defined in the enum above.
ConsoleFunction( inputPushCursor, void, 2, 2, "(cursorShapeName) Set's the current cursor to one specified"
                "@param cursorShapeName Name corresponding to enumerated shape value: \"Arrow\", \"Wait\", \"Plus\", \"ResizeVert\", \"ResizeHorz\", "
                "\"ResizeAll\", \"ibeam\", \"ResizeNESW\", \"ResizeNWSE\"\n"
                "@return No Return Value")
{
   S32 val = 0;

   //*** Find the cursor shape
   if(argc == 2)
   {
      for (S32 i = 0; i < gCurManagerShapesTable.size; i++)
      {
         if (! dStricmp(argv[1], gCurManagerShapesTable.table[i].label))
         {
            val = gCurManagerShapesTable.table[i].index;
            break;
         }
      }
   }

   //*** Now set it
   CursorManager* cm = Input::getCursorManager();
   if(cm)
   {
      cm->pushCursor(val);
   }
}
//*** Function to pop the current cursor shape
ConsoleFunction( inputPopCursor, void, 1, 1, "() Pops the current cursor shape from manager stack\n"
                "@return No Return Value.")
{
   CursorManager* cm = Input::getCursorManager();
   if(cm)
   {
      cm->popCursor();
   }
}
