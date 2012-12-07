//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _FINDMATCH_H_
#define _FINDMATCH_H_

#ifndef _VECTOR_H_
#include "collection/vector.h"
#endif

class   FindMatch
{
   char*  expression;
   U32 maxMatches;

  public:
   static bool isMatch( const char *exp, const char *string, bool caseSensitive = false );
   static bool isMatchMultipleExprs( const char *exps, const char *str, bool caseSensitive );
   Vector<char *> matchList;

   FindMatch( U32 _maxMatches = 256 );
   FindMatch( const char *_expression, U32 _maxMatches = 256 );
   ~FindMatch();

   bool findMatch(const char *string, bool caseSensitive = false);
   void setExpression( const char *_expression );

   S32  numMatches() const   { return(matchList.size());                }
   bool isFull() const       { return (U32)matchList.size() >= maxMatches; }
   void clear()              { matchList.clear();                       }
};

#endif // _FINDMATCH_H_
