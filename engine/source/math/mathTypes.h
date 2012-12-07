//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _MATHTYPES_H_
#define _MATHTYPES_H_

#ifndef _CONSOLE_BASE_TYPE_H_
#include "console/consoleBaseType.h"
#endif

void RegisterMathFunctions(void);

// Define Math Console Types
DefineConsoleType( TypePoint2I )
DefineConsoleType( TypePoint2F )
DefineConsoleType( TypePoint3F )
DefineConsoleType( TypePoint4F )
DefineConsoleType( TypePoint2FVector )
DefineConsoleType( TypeRectI )
DefineConsoleType( TypeRectF )
DefineConsoleType( TypeMatrixPosition )
DefineConsoleType( TypeMatrixRotation )
DefineConsoleType( TypeBox3F )

#endif
