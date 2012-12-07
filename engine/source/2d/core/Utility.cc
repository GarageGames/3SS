//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "platform/platform.h"
#include "platform/platformGL.h"
#include "console/consoleTypes.h"
#include "console/console.h"
#include "math/mRandom.h"
#include "2d/sceneobject/SceneObject.h"
#include "2d/core/Utility.h"

//-----------------------------------------------------------------------------

namespace Utility
{

//-----------------------------------------------------------------------------
// Return a string containing the common elements of two space-separated strings of elements.
//-----------------------------------------------------------------------------
ConsoleFunction( t2dGetCommonElements, const char*, 3, 3, "(set1, set2) - Returns the common elements in two sets.")
{
    if (argc != 3)
    {
        Con::warnf("t2dGetCommonElements - Invalid number of parameters!");
        return NULL;
    }

    // Grab the element count of the first set.
    const U32 elementCount1 =Utility::mGetStringElementCount(argv[1]);

    // Make sure we get at least one number.
    if (elementCount1 < 1)
    {
        return NULL;
    }

    // Grab the element count of the second set.
    const U32 elementCount2 =Utility::mGetStringElementCount(argv[2]);

    // Make sure we get at least one number.
    if (elementCount2 < 1)
    {
        return NULL;
    }

    char* buffer = Con::getReturnBuffer(dStrlen(argv[1]) + 1);
    buffer[0] = '\0';
    bool first = true;
    
    // Individual elements assumed to be 1024 or less in length
    char element[1024];

    // Check for common elements
    for (U32 i = 0; i < elementCount1; i++)
    {
        dStrcpy(element,  Utility::mGetStringElement(argv[1], i, true));
        
        for (U32 j = 0; j < elementCount2; j++)
        {
            if (!dStrcmp(element, Utility::mGetStringElement(argv[2], j, true)))
            {
                if (!first)
                    dStrcat(buffer, " ");
                else
                    first = false;

                dStrcat(buffer, element);
            }
        }
    }

    return buffer;
}

//-----------------------------------------------------------------------------

const char* mGetFirstNonWhitespace( const char* inString )
{
    // Search for first non-whitespace.
   while(*inString == ' ' || *inString == '\n' || *inString == '\t')
      inString++;

   // Return found point.
   return inString;
}

//------------------------------------------------------------------------------

// NOTE:-   You must verify that elements (index/index+1) are valid first!
Vector2 mGetStringElementVector( const char* inString, const U32 index )
{
    if ((index + 1) >=Utility::mGetStringElementCount(inString))
       return Vector2::getZero();

    // Get String Element Vector.
    return Vector2( dAtof(mGetStringElement(inString,index)), dAtof(Utility::mGetStringElement(inString,index+1)) );
}

//------------------------------------------------------------------------------

// NOTE:-   You must verify that elements (index/index+1/index+2) are valid first!
VectorF mGetStringElementVector3D( const char* inString, const U32 index )
{
    if ((index + 2) >=Utility::mGetStringElementCount(inString))
       return VectorF(0.0f, 0.0f, 0.0f);

    // Get String Element Vector.
    return VectorF( dAtof(mGetStringElement(inString,index)), dAtof(Utility::mGetStringElement(inString,index+1)), dAtof(Utility::mGetStringElement(inString,index+2)) );
}

//-----------------------------------------------------------------------------

const char* mGetStringElement( const char* inString, const U32 index, const bool copyBuffer )
{
    // Non-whitespace chars.
    static const char* set = " \t\n";

    U32 wordCount = 0;
    U8 search = 0;
    const char* pWordStart = NULL;

    // End of string?
    if ( *inString != 0 )
    {
        // No, so search string.
        while( *inString )
        {
            // Get string element.
            search = *inString;

            // End of string?
            if ( search == 0 )
                break;

            // Move to next element.
            inString++;

            // Search for separators.
            for( U32 i = 0; set[i]; i++ )
            {
                // Found one?
                if( search == set[i] )
                {
                    // Yes...
                    search = 0;
                    break;
                }   
            }

            // Found a separator?
            if ( search == 0 )
                continue;

            // Found are word?
            if ( wordCount == index )
            {
                // Yes, so mark it.
                pWordStart = inString-1;
            }

            // We've found a non-separator.
            wordCount++;

            // Search for end of non-separator.
            while( 1 )
            {
                // Get string element.
                search = *inString;

                // End of string?
                if ( search == 0 )
                    break;

                // Move to next element.
                inString++;

                // Search for separators.
                for( U32 i = 0; set[i]; i++ )
                {
                    // Found one?
                    if( search == set[i] )
                    {
                        // Yes...
                        search = 0;
                        break;
                    }   
                }

                // Found separator?
                if ( search == 0 )
                    break;
            }

            // Have we found our word?
            if ( pWordStart )
            {
                // Yes, so return position if not copying buffer.
                if ( !copyBuffer )
                    return pWordStart;

                // Result Buffer.
                static char buffer[4096];

                // Calculate word length.
                const U32 length = inString - pWordStart - ((*inString)?1:0);

                // Copy Word.
                dStrncpy( buffer, pWordStart, length);
                buffer[length] = '\0';

                // Return Word.
                return buffer;
            }

            // End of string?
            if ( *inString == 0 )
            {
                // Bah!
                break;
            }
        }
    }

    // Sanity!
    AssertFatal( false, "Utility::mGetStringElement() - Couldn't find specified string element!" );

    // Didn't find it
    return StringTable->EmptyString;
}   

//-----------------------------------------------------------------------------

U32 mGetStringElementCount( const char* inString )
{
    // Non-whitespace chars.
    static const char* set = " \t\n";

    // End of string.
    if ( *inString == 0 )
        return 0;

    U32 wordCount = 0;
    U8 search = 0;

    // Search String.
    while( *inString )
    {
        // Get string element.
        search = *inString;

        // End of string?
        if ( search == 0 )
            break;

        // Move to next element.
        inString++;

        // Search for separators.
        for( U32 i = 0; set[i]; i++ )
        {
            // Found one?
            if( search == set[i] )
            {
                // Yes...
                search = 0;
                break;
            }   
        }

        // Found a separator?
        if ( search == 0 )
            continue;

        // We've found a non-separator.
        wordCount++;

        // Search for end of non-separator.
        while( 1 )
        {
            // Get string element.
            search = *inString;

            // End of string?
            if ( search == 0 )
                break;

            // Move to next element.
            inString++;

            // Search for separators.
            for( U32 i = 0; set[i]; i++ )
            {
                // Found one?
                if( search == set[i] )
                {
                    // Yes...
                    search = 0;
                    break;
                }   
            }

            // Found Separator?
            if ( search == 0 )
                break;
        }

        // End of string?
        if ( *inString == 0 )
        {
            // Bah!
            break;
        }
    }

    // We've finished.
    return wordCount;
}

} // Namespace Utility