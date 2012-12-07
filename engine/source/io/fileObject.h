//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _FILEOBJECT_H_
#define _FILEOBJECT_H_

#ifndef _SIMBASE_H_
#include "sim/simBase.h"
#endif
#ifndef _RESMANAGER_H_
#include "io/resource/resourceManager.h"
#endif
#ifndef _FILESTREAM_H_
#include "io/fileStream.h"
#endif

class FileObject : public SimObject
{
   typedef SimObject Parent;
   U8 *mFileBuffer;
   U32 mBufferSize;
   U32 mCurPos;
   FileStream stream;
public:
   FileObject();
   ~FileObject();
   
   bool openForWrite(const char *fileName, const bool append = false);
   bool openForRead(const char *fileName);
   bool readMemory(const char *fileName);
   const U8 *readLine();
   void peekLine(U8 *line, S32 length);
   bool isEOF();
   void writeLine(const U8 *line);
   void close();
   void writeObject( SimObject* object, const U8* objectPrepend = NULL );

   //Luma:	ccess to the buffer and the size
   U8 *getBuffer(void)		{ return mFileBuffer; }
   U32 getBufferSize(void)	{ return mBufferSize; }

   DECLARE_CONOBJECT(FileObject);
};

#endif
