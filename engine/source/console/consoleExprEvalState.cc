//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "consoleExprEvalState.h"
#include "consoleNamespace.h"

#include "platform/platform.h"
#include "console/console.h"

#include "console/ast.h"
#include "collection/findIterator.h"
#include "io/resource/resourceManager.h"

#include "string/findMatch.h"
#include "io/fileStream.h"
#include "console/compiler.h"

extern ExprEvalState gEvalState;

void ExprEvalState::pushFrame(StringTableEntry frameName, Namespace *ns)
{
   Dictionary *newFrame = new Dictionary(this);
   newFrame->scopeName = frameName;
   newFrame->scopeNamespace = ns;
   stack.push_back(newFrame);
}

void ExprEvalState::popFrame()
{
   Dictionary *last = stack.last();
   stack.pop_back();
   delete last;
}

void ExprEvalState::pushFrameRef(S32 stackIndex)
{
   AssertFatal( stackIndex >= 0 && stackIndex < stack.size(), "You must be asking for a valid frame!" );
   Dictionary *newFrame = new Dictionary(this, stack[stackIndex]);
   stack.push_back(newFrame);
}

ExprEvalState::ExprEvalState()
{
   VECTOR_SET_ASSOCIATION(stack);
   globalVars.setState(this);
   thisObject = NULL;
   traceOn = false;
}

ExprEvalState::~ExprEvalState()
{
   while(stack.size())
      popFrame();
}

ConsoleFunction(backtrace, void, 1, 1, "() Use the backtrace function to print the current callstack to the console. This is used to trace functions called from withing functions and can help discover what functions were called (and not yet exited) before the current point in your scripts.\n"
                                                                "@return No return value")
{
   U32 totalSize = 1;

   for(U32 i = 0; i < (U32)gEvalState.stack.size(); i++)
   {
      totalSize += dStrlen(gEvalState.stack[i]->scopeName) + 3;
      if(gEvalState.stack[i]->scopeNamespace && gEvalState.stack[i]->scopeNamespace->mName)
         totalSize += dStrlen(gEvalState.stack[i]->scopeNamespace->mName) + 2;
   }

   char *buf = Con::getReturnBuffer(totalSize);
   buf[0] = 0;
   for(U32 i = 0; i < (U32)gEvalState.stack.size(); i++)
   {
      dStrcat(buf, "->");
      if(gEvalState.stack[i]->scopeNamespace && gEvalState.stack[i]->scopeNamespace->mName)
      {
         dStrcat(buf, gEvalState.stack[i]->scopeNamespace->mName);
         dStrcat(buf, "::");
      }
      dStrcat(buf, gEvalState.stack[i]->scopeName);
   }
   Con::printf("BackTrace: %s", buf);

}
