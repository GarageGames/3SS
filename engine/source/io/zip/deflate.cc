//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "platform/platform.h"
#include "io/zip/compressor.h"

#include "io/zip/zipSubStream.h"

namespace Zip
{

ImplementCompressor(Deflate, Deflated);

CompressorCreateReadStream(Deflate)
{
   ZipSubRStream *stream = new ZipSubRStream;
   stream->attachStream(zipStream);
   stream->setUncompressedSize(cdir->mUncompressedSize);

   return stream;
}

CompressorCreateWriteStream(Deflate)
{
   ZipSubWStream *stream = new ZipSubWStream;
   stream->attachStream(zipStream);

   return stream;
}

} // end namespace Zip
