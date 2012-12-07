//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _MODULE_MERGE_DEFINITION_H
#define _MODULE_MERGE_DEFINITION_H

#ifndef _SIMBASE_H_
#include "sim/simBase.h"
#endif

//-----------------------------------------------------------------------------

class ModuleMergeDefinition : public SimObject
{
private:
    typedef SimObject Parent;

    /// Module update
    StringTableEntry        mModuleMergePath;

public:
    ModuleMergeDefinition();
    virtual ~ModuleMergeDefinition() {}

    /// Engine.
    static void             initPersistFields();

    /// Module merge.
    inline void             setModuleMergePath( const char* pModuleMergePath )          { mModuleMergePath = StringTable->insert(pModuleMergePath); }
    inline StringTableEntry getModuleMergePath( void ) const                            { return mModuleMergePath; }

    /// Declare Console Object.
    DECLARE_CONOBJECT( ModuleMergeDefinition );
};

#endif // _MODULE_MERGE_DEFINITION_H

