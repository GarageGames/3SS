//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _CONSOLE_EXPREVALSTATE_H_
#define _CONSOLE_EXPREVALSTATE_H_

#ifndef _CONSOLE_DICTIONARY_H_
#include "console/consoleDictionary.h"
#endif

//-----------------------------------------------------------------------------

class ExprEvalState
{
public:
    /// @name Expression Evaluation
    /// @{

    ///
    SimObject *thisObject;
    Dictionary::Entry *currentVariable;
    bool traceOn;

    ExprEvalState();
    ~ExprEvalState();

    /// @}

    /// @name Stack Management
    /// @{

    ///
    Dictionary globalVars;
    Vector<Dictionary *> stack;
    void setCurVarName(StringTableEntry name);
    void setCurVarNameCreate(StringTableEntry name);
    S32 getIntVariable();
    F64 getFloatVariable();
    const char *getStringVariable();
    void setIntVariable(S32 val);
    void setFloatVariable(F64 val);
    void setStringVariable(const char *str);

    void pushFrame(StringTableEntry frameName, Namespace *ns);
    void popFrame();

    /// Puts a reference to an existing stack frame
    /// on the top of the stack.
    void pushFrameRef(S32 stackIndex);

    /// @}
};

#endif // _CONSOLE_EXPREVALSTATE_H_
