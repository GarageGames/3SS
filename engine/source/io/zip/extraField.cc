//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "platform/platform.h"
#include "io/zip/extraField.h"

namespace Zip
{

static ExtraField *gExtraFieldInitList = NULL;

//////////////////////////////////////////////////////////////////////////
// Constructor/Destructor
//////////////////////////////////////////////////////////////////////////

ExtraField::ExtraField(U16 id, ExtraFieldCreateFn fnCreate)
{
   mID = id;
   mCreateFn = fnCreate;

   mNext = gExtraFieldInitList;
   gExtraFieldInitList = this;
}

//////////////////////////////////////////////////////////////////////////
// Static Methods
//////////////////////////////////////////////////////////////////////////

ExtraField * ExtraField::create(U16 id)
{
   for(ExtraField *walk = gExtraFieldInitList;walk;walk = walk->mNext)
   {
      if(walk->getID() == id)
         return walk->mCreateFn();
   }

   return NULL;
}

} // end namespace Zip
