//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "2d/core/Vector2.h"

// Script bindings.
#include "Vector2_ScriptBinding.h"

//-----------------------------------------------------------------------------

ConsoleType( Vector2, TypeVector2, sizeof(Vector2), "" )

ConsoleGetType( TypeVector2 )
{
    return ((Vector2*)dptr)->scriptThis();
}

ConsoleSetType( TypeVector2 )
{
    // "x y".
    if( argc == 1 )
    {
        dSscanf(argv[0], "%g %g", &((Vector2*)dptr)->x, &((Vector2*)dptr)->y);
        return;
    }

    // "x,y".
    if( argc == 2 )
    {
        *((Vector2*)dptr) = Vector2(dAtof(argv[0]), dAtof(argv[1]));
        return;
    }

    // Warn.
    Con::printf("Vector2 must be set as { x, y } or \"x y\"");
}
