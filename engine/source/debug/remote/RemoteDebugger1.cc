//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _REMOTE_DEBUGGER1_H_
#include "debug/remote/RemoteDebugger1.h"
#endif

#ifndef _COMPILER_H_
#include "console/compiler.h"
#endif

// Script bindings.
#include "debug/remote/RemoteDebugger1_ScriptBinding.h"

//-----------------------------------------------------------------------------

IMPLEMENT_CONOBJECT(RemoteDebugger1);

//-----------------------------------------------------------------------------

RemoteDebugger1::RemoteDebugger1() :
    mBreakPoints( NULL )
{    
}

//-----------------------------------------------------------------------------

RemoteDebugger1::~RemoteDebugger1()
{
}

//-----------------------------------------------------------------------------

bool RemoteDebugger1::getCodeFiles( char* pBuffer, S32 bufferSize )
{
    // Iterate all code-blocks.
    for( CodeBlock* pCodeBlock = CodeBlock::getCodeBlockList(); pCodeBlock != NULL; pCodeBlock = pCodeBlock->nextFile )
    {
        // Finish if out of buffer.
        if ( bufferSize <= 0 )
            return false;

        // Format code file.
        S32 offset = dSprintf( pBuffer, bufferSize, pCodeBlock->nextFile == NULL ? "%s" : "%s,", pCodeBlock->fullPath );
        pBuffer += offset;
        bufferSize -= offset;           
    }

    // Finish if out of buffer.
    if ( bufferSize < 2 )
        return false;

    // Terminate return buffer.
    dSprintf( pBuffer, bufferSize, "\n" );

    return true;
}

//-----------------------------------------------------------------------------

const char* RemoteDebugger1::getValidBreakpoints( void )
{
    for( CodeBlock* pCodeBlock = CodeBlock::getCodeBlockList(); pCodeBlock != NULL; pCodeBlock = pCodeBlock->nextFile )
    {
        Con::printf( "%s", pCodeBlock->fullPath );
        Con::printSeparator();
        for( U32 breakEntry = 0, breakIndex = 0; breakEntry < pCodeBlock->lineBreakPairCount; breakEntry++, breakIndex += 2 )
        {

            Con::printf( "Line: %d, IP: %d", pCodeBlock->lineBreakPairs[breakIndex] >> 8, pCodeBlock->lineBreakPairs[breakIndex+1] );
        }
        Con::printSeparator();
    }

    return NULL;
}

//-----------------------------------------------------------------------------

void RemoteDebugger1::setNextStatementBreak( const bool enabled )
{
   if ( enabled )
   {
      // Apply breaks on all the code blocks.
      for(CodeBlock *walk = CodeBlock::getCodeBlockList(); walk; walk = walk->nextFile)
         walk->setAllBreaks();

      //mBreakOnNextStatement = true;
   } 
   else if ( !enabled )
   {
      // Clear all the breaks on the code-blocks 
      // then go reapply the breakpoints.
      for(CodeBlock *walk = CodeBlock::getCodeBlockList(); walk; walk = walk->nextFile)
         walk->clearAllBreaks();

      //mBreakOnNextStatement = false;
   }
}

//-----------------------------------------------------------------------------

void RemoteDebugger1::onClientLogin( void )
{
    // Call parent.
    Parent::onClientLogin();
}

//-----------------------------------------------------------------------------

void RemoteDebugger1::onClientLogout( void )
{
    // Call parent.
    Parent::onClientLogout();
}

