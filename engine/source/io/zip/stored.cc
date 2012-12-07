//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "platform/platform.h"
#include "io/zip/compressor.h"

#include "io/resizeStream.h"

namespace Zip
{

ImplementCompressor(Stored, Stored);

CompressorCreateReadStream(Stored)
{
   ResizeFilterStream *resStream = new ResizeFilterStream;
   resStream->attachStream(zipStream);
   resStream->setStreamOffset(zipStream->getPosition(), cdir->mCompressedSize);

   return resStream;
}

CompressorCreateWriteStream(Stored)
{
   return zipStream;
}

} // end namespace Zip
