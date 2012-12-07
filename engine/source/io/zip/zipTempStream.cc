//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "platform/platform.h"
#include "console/console.h"
#include "io/zip/zipTempStream.h"
#include "algorithm/crc.h"

namespace Zip
{

//////////////////////////////////////////////////////////////////////////
// Public Methods
//////////////////////////////////////////////////////////////////////////

bool ZipTempStream::open(const char *filename, AccessMode mode)
{
   if(filename == NULL)
   {
      filename = Platform::getTemporaryFileName();
      mDeleteOnClose = true;
   }

   mFilename = StringTable->insert(filename, true);

   if(! Parent::open(filename, mode))
      return false;

   mStreamCaps &= ~U32(StreamPosition);

   return true;
}

} // end namespace Zip
