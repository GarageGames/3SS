//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _UTILITY_H_
#define _UTILITY_H_

#ifndef _SIMBASE_H_
#include "sim/simBase.h"
#endif

#ifndef _PLATFORM_H_
#include "platform/platform.h"
#endif

#ifndef _PLATFORMGL_H_
#include "platform/platformGL.h"
#endif

#ifndef _MMATH_H_
#include "math/mMath.h"
#endif


//-----------------------------------------------------------------------------
// Miscellaneous Defines.
//-----------------------------------------------------------------------------

#define MASK_ALL                        (U32_MAX)
#define MASK_BITCOUNT                   (32)
#define DEBUG_MODE_COUNT                (8)

#define MAX_LAYERS_SUPPORTED            (32)

#define MAX_CONTACTPOINTS_SUPPORTED     (1024)

#define NO_IMAGE_RENDER_PROXY_NAME      "$NoImageRenderProxy"

//-----------------------------------------------------------------------------

class Scene; // Yuk!
class SceneObject;
struct Vector2;

//-----------------------------------------------------------------------------

typedef Vector<SceneObject*> typeSceneObjectVector;
typedef const Vector<SceneObject*>& typeSceneObjectVectorConstRef;

namespace Utility
{

//-----------------------------------------------------------------------------

#define STATIC_VOID_CAST_TO( pointerType, castToType, obj ) static_cast<castToType*>( reinterpret_cast<pointerType*>(obj) )
#define DYNAMIC_VOID_CAST_TO( pointerType, castToType, obj ) dynamic_cast<castToType*>( reinterpret_cast<pointerType*>(obj) )

//-----------------------------------------------------------------------------

/// String helpers.
const char* mGetFirstNonWhitespace( const char* inString );
Vector2 mGetStringElementVector( const char* inString, const U32 index = 0 );
VectorF mGetStringElementVector3D( const char* inString, const U32 index = 0 );
const char* mGetStringElement( const char* inString, const U32 index, const bool copyBuffer = true );
U32 mGetStringElementCount( const char *string );

} // Namespace Utility.

#endif // _UTILITY_H_
