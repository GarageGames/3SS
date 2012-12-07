//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "io/zip/zipArchive.h"
#include "string/stringTable.h"

#ifndef _ZIPTEMPSTREAM_H_
#define _ZIPTEMPSTREAM_H_

namespace Zip
{

/// @addtogroup zipint_group
/// @ingroup zip_group
// @{

class ZipTempStream : public FileStream
{
   typedef FileStream Parent;

protected:
   CentralDir *mCD;
   bool mDeleteOnClose;
   StringTableEntry mFilename;

public:
   ZipTempStream() : mCD(NULL), mDeleteOnClose(false) {}
   ZipTempStream(CentralDir *cd) : mCD(cd), mDeleteOnClose(false) {}

   void setCentralDir(CentralDir *cd)     { mCD = cd; }
   CentralDir *getCentralDir()            { return mCD; }

   void setDeleteOnClose(bool del)        { mDeleteOnClose = del; }

   virtual bool open(const char *filename, AccessMode mode);
   
   /// Open a temporary file in ReadWrite mode. The file will be deleted when the stream is closed.
   virtual bool open()
   {
      return open(NULL, ReadWrite);
   }

   virtual void close()
   {
      Parent::close();

      if(mDeleteOnClose)
         Platform::fileDelete(mFilename);
   }

   /// Disallow setPosition() 
   virtual bool setPosition(const U32 i_newPosition)        { return false; }

   /// Seek back to the start of the file.
   /// This is used internally by the zip code and should never be called whilst
   /// filters are attached (e.g. when reading or writing in a zip file)
   bool rewind()
   {
      mStreamCaps |= U32(StreamPosition);
      bool ret = Parent::setPosition(0);
      mStreamCaps &= ~U32(StreamPosition);

      return ret;
   }
};

// @}

} // end namespace Zip

#endif // _ZIPTEMPSTREAM_H_
