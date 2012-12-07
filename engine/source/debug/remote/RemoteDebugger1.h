//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _REMOTE_DEBUGGER1_H_
#define _REMOTE_DEBUGGER1_H_

#ifndef _REMOTE_DEBUGGER_BASE_H_
#include "debug/remote/RemoteDebuggerBase.h"
#endif

//-----------------------------------------------------------------------------

class RemoteDebugger1 : public RemoteDebuggerBase
{
    typedef RemoteDebuggerBase Parent;

protected:
    // Code breakpoint.
    struct Breakpoint
    {
        StringTableEntry    mCodeFile;
        CodeBlock*          mpCodeBlock;
        U32                 mLineNumber;
        S32                 mHitCount;
        S32                 mCurrentHitCount;
        bool                mClearOnBreak;
        const char*         mpConditionalExpression;

        Breakpoint*         mNextBreakpoint;
    };

private:
    Breakpoint* mBreakPoints;

public:
    RemoteDebugger1();
    virtual ~RemoteDebugger1();

    /// Debugger commands.
    bool getCodeFiles( char* pBuffer, S32 bufferSize );
    const char* getValidBreakpoints( void );
    void addBreakpoint( const char* pCodeFile, const S32 lineNumber, const bool clear, const S32 hitCount, const char* pConditionalExpression );
    void setNextStatementBreak( const bool enabled );

    DECLARE_CONOBJECT(RemoteDebugger1);

protected:
    virtual void onClientLogin( void );
    virtual void onClientLogout( void );
};

#endif // _REMOTE_DEBUGGER1_H_
