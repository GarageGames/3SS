//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "io/filterStream.h"
#include "algorithm/crc.h"
#include "io/zip/centralDir.h"

// [tom, 1/29/2007] ZipStatFilter allows us to track CRC and uncompressed size
// on the fly. This is necessary when dealing with compressed files as the
// CRC must be of the uncompressed data.
//
// The alternative would be to compress the files when updating the zip file,
// but this could potentially cause ZipArchive::closeArchive() to take a long
// time to complete. With compression done on the fly the time consuming code
// is pushed out to when writing to files, which is significantly easier to
// do asynchronously.

#ifndef _ZIPSTATFILTER_H_
#define _ZIPSTATFILTER_H_

//////////////////////////////////////////////////////////////////////////
/// \brief Namespace for the zip code.
///
/// @see Zip::ZipArchive, \ref zip_group "Zip Code Module"
//////////////////////////////////////////////////////////////////////////
namespace Zip
{

/// @addtogroup zipint_group Zip Code Internals
/// 
/// The zip code internals are mostly undocumented, but should be fairly
/// obvious to anyone who is familiar with the zip file format.

/// @ingroup zip_group
// @{

//////////////////////////////////////////////////////////////////////////
/// \brief Helper class for tracking CRC and uncompressed size
/// 
/// ZipStatFilter allows us to track CRC and uncompressed size
/// on the fly. This is necessary when dealing with compressed files as the
/// CRC must be of the uncompressed data.
/// 
/// ZipStatFilter is mostly intended for internal use by the zip code.
/// However, it can be useful when reading zips sequentially using the
/// stream interface to provide CRC checking.
/// 
/// <b>Example</b>
/// 
/// @code
/// // It's assumed that you would use proper error checking and that
/// // zip is a valid pointer to a ZipArchive and otherStream is a pointer
/// // to a valid stream.
/// Zip::ZipArchive *zip;
/// Stream *otherStream;
/// 
/// // We need the real central directory to compare the CRC32
/// Zip::CentralDir *realCD = zip->findFileInfo("file.txt");
/// Stream *stream = zip->openFile("file.txt", ZipArchive::Read);
/// 
/// Zip::CentralDir fakeCD;
/// Zip::ZipStatFilter zsf(&fakeCD);
/// 
/// zsf.attachStream(stream);
/// 
/// // ... read <i>entire</i> file sequentially using zsf instead of stream
/// otherStream->copyFrom(&zsf);
/// 
/// zsf.detachStream();
/// 
/// // fakeCD.mCRC32 now contains the CRC32 of the stream
/// if(fakeCD.mCRC32 != realCD->mCRC32)
/// {
///    // ... handle CRC failure ...
/// }
/// 
/// zip->closeFile(stream);
/// @endcode
/// 
/// A more complete example of this may be found in the code for the
/// ZipArchive::extractFile() method in zipArchive.cc 
/// 
//////////////////////////////////////////////////////////////////////////
class ZipStatFilter : public FilterStream
{
   typedef FilterStream Parent;

protected:
   Stream *mStream;

   CentralDir *mCD;

   virtual bool _write(const U32 numBytes, const void *buffer)
   {
      if(! mStream->write(numBytes, buffer))
         return false;

      mCD->mUncompressedSize += numBytes;
      mCD->mCRC32 = calculateCRC(buffer, numBytes, mCD->mCRC32);

      return true;
   }

   virtual bool _read(const U32 numBytes, void *buffer)
   {
      if(! mStream->read(numBytes, buffer))
         return false;

      mCD->mUncompressedSize += numBytes;
      mCD->mCRC32 = calculateCRC(buffer, numBytes, mCD->mCRC32);

      return true;
   }

public:
   ZipStatFilter() : mCD(NULL), mStream(NULL) {}
   ZipStatFilter(CentralDir *cd) : mCD(cd), mStream(NULL) {}
   virtual ~ZipStatFilter()
   {
      detachStream();
   }

   virtual bool attachStream(Stream *stream)
   {
      if(mCD == NULL)
         return false;

      mStream = stream;
      mCD->mUncompressedSize = 0;
      mCD->mCRC32 = INITIAL_CRC_VALUE;
      return true;
   }

   virtual void detachStream()
   {
      if(mStream == NULL)
         return;

      // Post condition the CRC
      mCD->mCRC32 ^= CRC_POSTCOND_VALUE;
      mStream = NULL;
   }

   virtual Stream *getStream()                     { return mStream; }

   void setCentralDir(CentralDir *cd)              { mCD = cd; }
   CentralDir *getCentralDir()                     { return mCD; }
};

// @}

} // end namespace Zip

#endif // _ZIPSTATFILTER_H_
