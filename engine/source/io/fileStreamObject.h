//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "streamObject.h"
#include "io/fileStream.h"

#ifndef _FILESTREAMOBJECT_H_
#define _FILESTREAMOBJECT_H_

class FileStreamObject : public StreamObject
{
   typedef StreamObject Parent;

protected:
   FileStream mFileStream;

public:
   FileStreamObject();
   virtual ~FileStreamObject();
   DECLARE_CONOBJECT(FileStreamObject);

   virtual bool onAdd();

   //////////////////////////////////////////////////////////////////////////
   /// @brief Open a file
   /// 
   /// @param filename Name of file to open
   /// @param mode One of #FileStream::Read, #FileStream::Write, #FileStream::ReadWrite or #FileStream::WriteAppend
   /// @return true for success, false for failure
   /// @see close()
   //////////////////////////////////////////////////////////////////////////
   bool open(const char *filename, FileStream::AccessMode mode);

   //////////////////////////////////////////////////////////////////////////
   /// @brief Close the file
   /// 
   /// @see open()
   //////////////////////////////////////////////////////////////////////////
   void close();
};

#endif // _FILESTREAMOBJECT_H_
