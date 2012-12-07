//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "bufferStream.h"
#include "platform/platform.h"

//-----------------------------------------------------------------------------
// BufferStream methods...
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
BufferStream::BufferStream()
{
   // initialize the buffer stream
   init();
}

//-----------------------------------------------------------------------------
BufferStream::~BufferStream()
{
   // make sure the file stream is closed
   close();
}

//-----------------------------------------------------------------------------
void BufferStream::close() {
	mReadPosition = 0;//we are done reading for this go-around
}

void BufferStream::open() {
	init();
}
//-----------------------------------------------------------------------------
bool BufferStream::_read(const U32 i_numBytes, void *o_pBuffer)
{
	if( mReadPosition + i_numBytes > mBufferLen ) {
		//-Mat reading too far, error our here
		Platform::debugBreak();//we are about to have a buffer overrun
		return false;
	}
	
	//is off by 16 bytes the second time through (byte alignment after 0 is off?)
	U8 *bufferStart = &mBuffer[mReadPosition * sizeof(U8)];
	dMemcpy( o_pBuffer, bufferStart, i_numBytes );
	mReadPosition += i_numBytes;

	return true;
}

//-----------------------------------------------------------------------------
bool BufferStream::_write(const U32 i_numBytes, const void *i_pBuffer)
{
	if( (mBufferLen + i_numBytes) >= BUFFER_SIZE ) {
		//too many bytes, we'll have to try resizing (ugh!)
		return false;
	}
	//copy to "fresh" part of mBuffer
	U8 *bufferStart = &mBuffer[mBufferLen] ;
	dMemcpy( bufferStart, i_pBuffer, i_numBytes );
	mBufferLen += i_numBytes;
	return true;
}

//-----------------------------------------------------------------------------
void BufferStream::init()
{
	mBuffer[0] = '\0';
	mBufferLen = 0;
	mReadPosition = 0;
}

