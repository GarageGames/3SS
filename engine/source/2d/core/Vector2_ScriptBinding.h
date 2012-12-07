//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _CONSOLE_H_
#include "console/console.h"
#endif

#ifndef _SCENE_OBJECT_H_
#include "2d/sceneobject/SceneObject.h"
#endif



ConsoleFunctionGroupBegin( Vector2Math, "Vector2 math functions.");

//-----------------------------------------------------------------------------

ConsoleFunction( Vector2Add, const char*, 3, 3, "(Vector2 v1$, Vector2 v2$) - Returns v1+v2.")
{
    // Check Parameters.
    if (Utility::mGetStringElementCount(argv[1]) < 2 ||Utility::mGetStringElementCount(argv[2]) < 2 )
    {
        Con::warnf("Vector2Add() - Invalid number of parameters!");
        return NULL;
    }

    // Input Vectors.
    Vector2 v1(0,0),v2(0,0);
    // Scan-in vectors.
    dSscanf(argv[1],"%g %g", &v1.x, &v1.y);
    dSscanf(argv[2],"%g %g", &v2.x, &v2.y);
    // Do Vector Operation.
    Vector2 v = v1 + v2;
    // Create Returnable Buffer.
    char* pBuffer = Con::getReturnBuffer(32);
    // Format Buffer.
    dSprintf(pBuffer, 32, "%g %g", v.x, v.y);
    // Return Velocity.
    return pBuffer;
}


//-----------------------------------------------------------------------------
// Subtract two 2D Vectors.
//-----------------------------------------------------------------------------
ConsoleFunction( Vector2Sub, const char*, 3, 3, "(Vector2 v1$, Vector2 v2$) - Returns v1-v2.")
{
    // Check Parameters.
    if (Utility::mGetStringElementCount(argv[1]) < 2 ||Utility::mGetStringElementCount(argv[2]) < 2 )
    {
        Con::warnf("Vector2Sub() - Invalid number of parameters!");
        return NULL;
    }

    // Input Vectors.
    Vector2 v1(0,0),v2(0,0);
    // Scan-in vectors.
    dSscanf(argv[1],"%g %g", &v1.x, &v1.y);
    dSscanf(argv[2],"%g %g", &v2.x, &v2.y);
    // Do Vector Operation.
    Vector2 v = v1 - v2;
    // Create Returnable Buffer.
    char* pBuffer = Con::getReturnBuffer(32);
    // Format Buffer.
    dSprintf(pBuffer, 32, "%g %g", v.x, v.y);
    // Return Velocity.
    return pBuffer;
}


//-----------------------------------------------------------------------------
// Multiply two 2D Vectors (Not Dot-Product!)
//-----------------------------------------------------------------------------
ConsoleFunction( Vector2Mult, const char*, 3, 3, "(Vector2 v1$, Vector2 v2$) - Returns v1 mult v2.")
{
    // Check Parameters.
    if (Utility::mGetStringElementCount(argv[1]) < 2 ||Utility::mGetStringElementCount(argv[2]) < 2 )
    {
        Con::warnf("Vector2Mult() - Invalid number of parameters!");
        return NULL;
    }

    // Input Vectors.
    Vector2 v1(0,0),v2(0,0);
    // Scan-in vectors.
    dSscanf(argv[1],"%g %g", &v1.x, &v1.y);
    dSscanf(argv[2],"%g %g", &v2.x, &v2.y);
    // Do Vector Operation.
    Vector2 v( v1.x*v2.x, v1.y*v2.y );
    // Create Returnable Buffer.
    char* pBuffer = Con::getReturnBuffer(32);
    // Format Buffer.
    dSprintf(pBuffer, 32, "%g %g", v1.x*v2.x, v1.y*v2.y );
    // Return Velocity.
    return pBuffer;
}


//-----------------------------------------------------------------------------
// Scale a 2D Vector.
//-----------------------------------------------------------------------------
ConsoleFunction( Vector2Scale, const char*, 3, 3, "(Vector2 v1$, scale) - Returns v1 scaled by scale.")
{
    // Check Parameters.
    if (Utility::mGetStringElementCount(argv[1]) < 2 )
    {
        Con::warnf("Vector2Scale() - Invalid number of parameters!");
        return NULL;
    }

    // Input Vectors.
    Vector2 v1(0,0);
    // Scan-in vectors.
    dSscanf(argv[1],"%g %g", &v1.x, &v1.y);
    // Do Vector Operation.
    v1 *= dAtof(argv[2]);
    // Create Returnable Buffer.
    char* pBuffer = Con::getReturnBuffer(32);
    // Format Buffer.
    dSprintf(pBuffer, 32, "%g %g", v1.x, v1.y);
    // Return Velocity.
    return pBuffer;
}


//-----------------------------------------------------------------------------
// Normalize a 2D Vector.
//-----------------------------------------------------------------------------
ConsoleFunction( Vector2Normalize, const char*, 2, 2, "(Vector2 v1$) - Returns Normalized v1.")
{
    // Check Parameters.
    if (Utility::mGetStringElementCount(argv[1]) < 2 )
    {
        Con::warnf("Vector2Normalize() - Invalid number of parameters!");
        return NULL;
    }

    // Input Vectors.
    Vector2 v1(0,0);
    // Scan-in vectors.
    dSscanf(argv[1],"%g %g", &v1.x, &v1.y);
    // Do Vector Operation.
    v1.Normalize();
    // Create Returnable Buffer.
    char* pBuffer = Con::getReturnBuffer(32);
    // Format Buffer.
    dSprintf(pBuffer, 32, "%g %g", v1.x, v1.y);
    // Return Velocity.
    return pBuffer;
}


//-----------------------------------------------------------------------------
// Dot-Product of two 2D Vectors.
//-----------------------------------------------------------------------------
ConsoleFunction(Vector2Dot, F32, 3, 3, "(Vector2 v1$, Vector2 v2$) - Returns dot-product of v1 and v2.")
{
    // Check Parameters.
    if (Utility::mGetStringElementCount(argv[1]) < 2 ||Utility::mGetStringElementCount(argv[2]) < 2 )
    {
        Con::warnf("Vector2Dot() - Invalid number of parameters!");
        return 0.0f;
    }

    // Input Vectors.
    Vector2 v1(0,0), v2(0,0);
    // Scan-in vectors.
    dSscanf(argv[1],"%g %g", &v1.x, &v1.y);
    dSscanf(argv[2],"%g %g", &v2.x, &v2.y);
    // Do Vector Operation.
    return v1.dot( v2 );
}


//-----------------------------------------------------------------------------
// Equality of two 2D Points.
//-----------------------------------------------------------------------------
ConsoleFunction( Vector2Compare, bool, 3, 4, "(Vector2 p1$, Vector2 p2$, [epsilon]) - Compares points p1 and p2 with optional difference (epsilon).")
{
    // Check Parameters.
    if (Utility::mGetStringElementCount(argv[1]) < 2 ||Utility::mGetStringElementCount(argv[2]) < 2 )
    {
        Con::warnf("Vector2Compare() - Invalid number of parameters!");
        return NULL;
    }

    // Input Vectors.
    Vector2 p1(0,0), p2(0,0);
    // Scan-in vectors.
    dSscanf(argv[1],"%g %g", &p1.x, &p1.y);
    dSscanf(argv[2],"%g %g", &p2.x, &p2.y);
    // Do Vector Operation.
    const F32 delta = (p2 - p1).Length();
    // Calculate Epsilon.
    const F32 epsilon = (argc >= 4) ? dAtof(argv[3]) : FLT_EPSILON;
    // Return  epsilon delta.
    return mIsEqualRange( delta, 0.0f, epsilon );
}


//-----------------------------------------------------------------------------
// Distance between two 2D Points.
//-----------------------------------------------------------------------------
ConsoleFunction( Vector2Distance, F32, 3, 3, "(Vector2 p1$, Vector2 p2$) - Returns the distance between points p1 and p2.")
{
    // Check Parameters.
    if (Utility::mGetStringElementCount(argv[1]) < 2 ||Utility::mGetStringElementCount(argv[2]) < 2 )
    {
        Con::warnf("Vector2Distance() - Invalid number of parameters!");
        return NULL;
    }

    // Input Vectors.
    Vector2 p1(0,0), p2(0,0);
    // Scan-in vectors.
    dSscanf(argv[1],"%g %g", &p1.x, &p1.y);
    dSscanf(argv[2],"%g %g", &p2.x, &p2.y);
    // Do Vector Operation.
    return (p2 - p1).Length();
}


//-----------------------------------------------------------------------------
// Angle between two 2D Vectors.
//-----------------------------------------------------------------------------
ConsoleFunction( t2dAngleBetween, F32, 3, 3, "(Vector2 v1$, Vector2 v2$) - Returns the angle between v1 and v2.")
{
    // Check Parameters.
    if (Utility::mGetStringElementCount(argv[1]) < 2 ||Utility::mGetStringElementCount(argv[2]) < 2 )
    {
        Con::warnf("t2dAngleBetween() - Invalid number of parameters!");
        return NULL;
    }

    Vector2 v1(0,0), v2(0,0);
    // Scan-in vectors.
    dSscanf(argv[1],"%g %g", &v1.x, &v1.y);
    dSscanf(argv[2],"%g %g", &v2.x, &v2.y);

    v1.Normalize();
    v2.Normalize();

    // Do Vector Operation.
    return mRadToDeg( mAcos( v1.dot(v2) ) );
}


//-----------------------------------------------------------------------------
// Angle from one point to another.
//-----------------------------------------------------------------------------
ConsoleFunction( t2dAngleToPoint, F32, 3, 3, "(Vector2 p1, Vector2 p1) - Returns the angle from p1 to p2.")
{
    // Check Parameters.
    if (Utility::mGetStringElementCount(argv[1]) < 2 ||Utility::mGetStringElementCount(argv[2]) < 2 )
    {
        Con::warnf("t2dAngleToPoint() - Invalid number of parameters!");
        return NULL;
    }

    Vector2 p1(0,0), p2(0,0);
    // Scan-in vectors.
    dSscanf(argv[1],"%g %g", &p1.x, &p1.y);
    dSscanf(argv[2],"%g %g", &p2.x, &p2.y);

    // Do Operation.
    return mRadToDeg( mAtan((p2.x - p1.x), (p1.y - p2.y)) );
}


//-----------------------------------------------------------------------------
// Length of a 2D Vector.
//-----------------------------------------------------------------------------
ConsoleFunction( Vector2Length, F32, 2, 2, "(Vector2 v1$) - Returns the length of v1.")
{
    // Check Parameters.
    if (Utility::mGetStringElementCount(argv[1]) < 2 )
    {
        Con::warnf("Vector2Length() - Invalid number of parameters!");
        return 0.0f;
    }

    // Input Vectors.
    Vector2 v1(0,0);
    // Scan-in vectors.
    dSscanf(argv[1],"%g %g", &v1.x, &v1.y);
    // Do Vector Operation.
    return v1.Length();
}


//-----------------------------------------------------------------------------
// Normalize Rectangle (two 2D Vectors) with relation to each other.
//-----------------------------------------------------------------------------
ConsoleFunction( t2dRectNormalize, const char*, 3, 3, "(Vector2 v1$, Vector2 v2$) - Returns Normalize rectangle of v1 and v2.")
{
    // Check Parameters.
    if (Utility::mGetStringElementCount(argv[1]) < 2 ||Utility::mGetStringElementCount(argv[2]) < 2 )
    {
        Con::warnf("t2dRectNormalize() - Invalid number of parameters!");
        return NULL;
    }

    // Input Vectors.
    Vector2 v1(0,0), v2(0,0);
    // Scan-in vectors.
    dSscanf(argv[1],"%g %g", &v1.x, &v1.y);
    dSscanf(argv[2],"%g %g", &v2.x, &v2.y);
    // Do Vector Operation.
    Vector2 topLeft( (v1.x <= v2.x) ? v1.x : v2.x, (v1.y <= v2.y) ? v1.y : v2.y );
    Vector2 bottomRight( (v1.x > v2.x) ? v1.x : v2.x, (v1.y > v2.y) ? v1.y : v2.y );

    // Create Returnable Buffer.
    char* pBuffer = Con::getReturnBuffer( 64 );
    // Format Buffer.
    dSprintf(pBuffer, 64, "%g %g %g %g", topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    // Return Velocity.
    return pBuffer;
}

//-----------------------------------------------------------------------------

ConsoleFunctionGroupEnd( Vector2Math );


