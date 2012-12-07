//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _BUFFERSTREAM_H_
#define _BUFFERSTREAM_H_

#ifndef _FILEIO_H_
#include "io/fileio.h"
#endif
#ifndef _STREAM_H_
#include "io/stream.h"
#endif


//-Mat this class is completely stripped down, it's only purpose is
//to allow writing to a buffer when a Stream is expected, Used for compiling DSO
//straight to memory without saving to a filesystem


class BufferStream : public Stream
{
public:
   enum
   {
      BUFFER_SIZE = 8 * 1024,         // this can be changed to anything appropriate
      BUFFER_INVALID = 0xffffffff      // file offsets must all be less than this
   };

protected:
    U32 mBufferLen;
    U32 mReadPosition;
    //putting this after U32's will long align the data
    U8 mBuffer[BUFFER_SIZE];

public:
   BufferStream();                       // default constructor
   virtual ~BufferStream();              // destructor

   // mandatory methods from Stream base class...
   virtual bool hasCapability(const Capability i_cap) const { return false; }

   virtual U32  getPosition() const { return mReadPosition; }
   virtual bool setPosition(const U32 i_newPosition) { return mReadPosition = i_newPosition; }
   virtual U32  getStreamSize() { return mBufferLen; }

   virtual void close();
   virtual void open();

   U8 *getBuffer() { return mBuffer; }
   U32 getBufferLength() { return mBufferLen; }

protected:
   // more mandatory methods from Stream base class...
   virtual bool _read(const U32 i_numBytes, void *o_pBuffer);
   virtual bool _write(const U32 i_numBytes, const void* i_pBuffer);

   void init();
   bool fillBuffer(const U32 i_startPosition) { return false; }
   void clearBuffer() {}
   static void calcBlockHead(const U32 i_position, U32 *o_blockHead) {}
   static void calcBlockBounds(const U32 i_position, U32 *o_blockHead, U32 *o_blockTail) {}
   void setStatus() {}
};

#endif // _BUFFERSTREAM_H_
