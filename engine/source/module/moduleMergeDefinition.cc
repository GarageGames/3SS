//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "moduleMergeDefinition.h"

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

//-----------------------------------------------------------------------------

IMPLEMENT_CONOBJECT( ModuleMergeDefinition );

//-----------------------------------------------------------------------------

ModuleMergeDefinition::ModuleMergeDefinition() :
    mModuleMergePath( StringTable->EmptyString )
{
}

//-----------------------------------------------------------------------------

void ModuleMergeDefinition::initPersistFields()
{
    // Call parent.
    Parent::initPersistFields();

    /// Module merge.
    addField( "MergePath", TypeString, Offset(mModuleMergePath, ModuleMergeDefinition), "The path where the modules to be merged can be found." );
}
