//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _MODULE_CALLBACKS_H_
#define _MODULE_CALLBACKS_H_

#ifndef _MODULE_DEFINITION_H
#include "moduleDefinition.h"
#endif

//-----------------------------------------------------------------------------

class ModuleCallbacks
{
    friend class ModuleManager;

private:
    // Called when a module is about to be loaded.
    virtual void onModulePreLoad( ModuleDefinition* pModuleDefinition ) {}

    // Called when a module has been loaded.
    virtual void onModulePostLoad( ModuleDefinition* pModuleDefinition ) {}

    // Called when a module is about to be unloaded.
    virtual void onModulePreUnload( ModuleDefinition* pModuleDefinition ) {}

    // Called when a module has been unloaded.
    virtual void onModulePostUnload( ModuleDefinition* pModuleDefinition ) {}
};

#endif // _MODULE_CALLBACKS_H_
