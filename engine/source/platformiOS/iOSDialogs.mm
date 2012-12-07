//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "platformiOS/platformiOS.h"
#include "sim/simBase.h"
#include "platform/nativeDialogs/fileDialog.h"
#include "platform/threads/mutex.h"
#include "memory/safeDelete.h"
#include "math/mMath.h"
#include "string/Unicode.h"
#include "console/consoleTypes.h"
#include "platform/threads/thread.h"

class FileDialogOpaqueData
{
public:
   Semaphore *sem;
   FileDialogOpaqueData() { sem = new Semaphore(0);  }
   ~FileDialogOpaqueData() { delete sem; }
};

class FileDialogFileExtList
{
public:
   Vector<UTF8*> list;
   UTF8* data;
   
   FileDialogFileExtList(const char* exts) { data = dStrdup(exts); }
   ~FileDialogFileExtList() { SAFE_DELETE(data); }
};

class FileDialogFileTypeList
{
public:
   UTF8* filterData;
   Vector<UTF8*> names;
   Vector<FileDialogFileExtList*> exts;
   bool any;
   
   FileDialogFileTypeList(const char* filter) { filterData = dStrdup(filter); any = false;}
   ~FileDialogFileTypeList()
   { 
      SAFE_DELETE(filterData);
      for(U32 i = 0; i < exts.size(); i++)
         delete exts[i];
   }
};



//-----------------------------------------------------------------------------
// PlatformFileDlgData Implementation
//-----------------------------------------------------------------------------
FileDialogData::FileDialogData()
{
   // Default Path
   //
   //  Try to provide consistent experience by recalling the last file path
   // - else
   //  Default to Working Directory if last path is not set or is invalid
   mDefaultPath = StringTable->insert( Con::getVariable("Tools::FileDialogs::LastFilePath") );
   if( mDefaultPath == StringTable->lookup("") || !Platform::isDirectory( mDefaultPath ) )
      mDefaultPath = Platform::getCurrentDirectory();

   mDefaultFile = StringTable->insert("");
   mFilters = StringTable->insert("");
   mFile = StringTable->insert("");
   mTitle = StringTable->insert("");

   mStyle = 0;
   
   mOpaqueData = new FileDialogOpaqueData();

}

FileDialogData::~FileDialogData()
{
	//Luma : delete stuff kthx
	delete mOpaqueData;
}

//-----------------------------------------------------------------------------
// FileDialog Implementation
//-----------------------------------------------------------------------------
IMPLEMENT_CONOBJECT(FileDialog);

FileDialog::FileDialog() : mData()
{
   // Default to File Must Exist Open Dialog style
   mData.mStyle = FileDialogData::FDS_OPEN | FileDialogData::FDS_MUSTEXIST;
   mChangePath = false;
}

FileDialog::~FileDialog()
{
}

void FileDialog::initPersistFields()
{
   // why is this stuff buried in another class?
   addProtectedField("DefaultPath", TypeString, Offset(mData.mDefaultPath, FileDialog), &setDefaultPath, &defaultProtectedGetFn, "Default Path when Dialog is shown");
   addProtectedField("DefaultFile", TypeString, Offset(mData.mDefaultFile, FileDialog), &setDefaultFile, &defaultProtectedGetFn, "Default File when Dialog is shown");
   addProtectedField("FileName", TypeString, Offset(mData.mFile, FileDialog), &setFile, &defaultProtectedGetFn, "Default File when Dialog is shown");
   addProtectedField("Filters", TypeString, Offset(mData.mFilters, FileDialog), &setFilters, &defaultProtectedGetFn, "Default File when Dialog is shown");
   addField("Title", TypeString, Offset(mData.mTitle, FileDialog), "Default File when Dialog is shown");
   addProtectedField("ChangePath", TypeBool, Offset(mChangePath, FileDialog), &setChangePath, &getChangePath, "True/False whether to set the working directory to the directory returned by the dialog" );
   Parent::initPersistFields();
}


static FileDialogFileExtList* _iOSGetFileExtensionsFromString(const char* filter)
{
   FileDialogFileExtList* list = new FileDialogFileExtList(filter);
   
   char* token = list->data;
   char* place = list->data;
   
   for( ; *place; place++)
   {
      if(*place != ';')
         continue;
      
      *place = '\0';
      
      list->list.push_back(token);
      
      ++place;
      token = place;
   }
   // last token   
   list->list.push_back(token);
   
   return list;
   
}

static FileDialogFileTypeList* _iOSGetFileTypesFromString(const char* filter)
{
   FileDialogFileTypeList &list = *(new FileDialogFileTypeList(filter));

   char* token = list.filterData;
   char* place = list.filterData;
   
   // scan the filter list until we hit a null.
   // when we see the separator '|', replace it with a null, and save the token
   // format is description|extension|description|extension
   bool isDesc = true;
   for( ; *place; place++)
   {
      if(*place != '|')
         continue;
      
      *place = '\0';

      if(isDesc)
         list.names.push_back(token);
      else
      {
         // detect *.*
         if(dStrstr((const char*)token, "*.*"))
            list.any = true;
         
         list.exts.push_back(_iOSGetFileExtensionsFromString(token));
      }
   
      
      isDesc = !isDesc;
      ++place;
      token = place;
   }
   list.exts.push_back(_iOSGetFileExtensionsFromString(token));
   
   return &list;
}


bool FileDialog::Execute()
{
    return true;
}






//-----------------------------------------------------------------------------
// Dialog Filters
//-----------------------------------------------------------------------------
bool FileDialog::setFilters(void* obj, const char* data)
{
   // Will do validate on write at some point.
   if( !data )
      return true;

   return true;

};


//-----------------------------------------------------------------------------
// Default Path Property - String Validated on Write
//-----------------------------------------------------------------------------
bool FileDialog::setDefaultPath(void* obj, const char* data)
{

   if( !data )
      return true;

   return true;

};

//-----------------------------------------------------------------------------
// Default File Property - String Validated on Write
//-----------------------------------------------------------------------------
bool FileDialog::setDefaultFile(void* obj, const char* data)
{
   if( !data )
      return true;

   return true;
};

//-----------------------------------------------------------------------------
// ChangePath Property - Change working path on successful file selection
//-----------------------------------------------------------------------------
bool FileDialog::setChangePath(void* obj, const char* data)
{
   bool bChangePath = dAtob( data );

   FileDialog *pDlg = static_cast<FileDialog*>( obj );

   if( bChangePath )
      pDlg->mData.mStyle |= FileDialogData::FDS_CHANGEPATH;
   else
      pDlg->mData.mStyle &= ~FileDialogData::FDS_CHANGEPATH;

   return true;
};

const char* FileDialog::getChangePath(void* obj, const char* data)
{
   FileDialog *pDlg = static_cast<FileDialog*>( obj );
   if( pDlg->mData.mStyle & FileDialogData::FDS_CHANGEPATH )
      return StringTable->insert("true");
   else
      return StringTable->insert("false");
}

bool FileDialog::setFile(void* obj, const char* data)
{
   return false;
};

//-----------------------------------------------------------------------------
// OpenFileDialog Implementation
//-----------------------------------------------------------------------------
OpenFileDialog::OpenFileDialog()
{
   // Default File Must Exist
   mData.mStyle = FileDialogData::FDS_OPEN | FileDialogData::FDS_MUSTEXIST;
}

OpenFileDialog::~OpenFileDialog()
{
   mMustExist = true;
   mMultipleFiles = false;
}

IMPLEMENT_CONOBJECT(OpenFileDialog);

//-----------------------------------------------------------------------------
// Console Properties
//-----------------------------------------------------------------------------
void OpenFileDialog::initPersistFields()
{
   addProtectedField("MustExist", TypeBool, Offset(mMustExist, OpenFileDialog), &setMustExist, &getMustExist, "True/False whether the file returned must exist or not" );
   addProtectedField("MultipleFiles", TypeBool, Offset(mMultipleFiles, OpenFileDialog), &setMultipleFiles, &getMultipleFiles, "True/False whether multiple files may be selected and returned or not" );
   
   Parent::initPersistFields();
}

//-----------------------------------------------------------------------------
// File Must Exist - Boolean
//-----------------------------------------------------------------------------
bool OpenFileDialog::setMustExist(void* obj, const char* data)
{
   bool bMustExist = dAtob( data );

   OpenFileDialog *pDlg = static_cast<OpenFileDialog*>( obj );
   
   if( bMustExist )
      pDlg->mData.mStyle |= FileDialogData::FDS_MUSTEXIST;
   else
      pDlg->mData.mStyle &= ~FileDialogData::FDS_MUSTEXIST;

   return true;
};

const char* OpenFileDialog::getMustExist(void* obj, const char* data)
{
   OpenFileDialog *pDlg = static_cast<OpenFileDialog*>( obj );
   if( pDlg->mData.mStyle & FileDialogData::FDS_MUSTEXIST )
      return StringTable->insert("true");
   else
      return StringTable->insert("false");
}

//-----------------------------------------------------------------------------
// Can Select Multiple Files - Boolean
//-----------------------------------------------------------------------------
bool OpenFileDialog::setMultipleFiles(void* obj, const char* data)
{
   bool bMustExist = dAtob( data );

   OpenFileDialog *pDlg = static_cast<OpenFileDialog*>( obj );

   if( bMustExist )
      pDlg->mData.mStyle |= FileDialogData::FDS_MULTIPLEFILES;
   else
      pDlg->mData.mStyle &= ~FileDialogData::FDS_MULTIPLEFILES;

   return true;
};

const char* OpenFileDialog::getMultipleFiles(void* obj, const char* data)
{
   OpenFileDialog *pDlg = static_cast<OpenFileDialog*>( obj );
   if( pDlg->mData.mStyle & FileDialogData::FDS_MULTIPLEFILES )
      return StringTable->insert("true");
   else
      return StringTable->insert("false");
}

//-----------------------------------------------------------------------------
// SaveFileDialog Implementation
//-----------------------------------------------------------------------------
SaveFileDialog::SaveFileDialog()
{
   // Default File Must Exist
   mData.mStyle = FileDialogData::FDS_SAVE | FileDialogData::FDS_OVERWRITEPROMPT;
   mOverwritePrompt = true;
}

SaveFileDialog::~SaveFileDialog()
{
}

IMPLEMENT_CONOBJECT(SaveFileDialog);

//-----------------------------------------------------------------------------
// Console Properties
//-----------------------------------------------------------------------------
void SaveFileDialog::initPersistFields()
{
   addProtectedField("OverwritePrompt", TypeBool, Offset(mOverwritePrompt, SaveFileDialog), &setOverwritePrompt, &getOverwritePrompt, "True/False whether the dialog should prompt before accepting an existing file name" );
   
   Parent::initPersistFields();
}

//-----------------------------------------------------------------------------
// Prompt on Overwrite - Boolean
//-----------------------------------------------------------------------------
bool SaveFileDialog::setOverwritePrompt(void* obj, const char* data)
{
   bool bOverwrite = dAtob( data );

   SaveFileDialog *pDlg = static_cast<SaveFileDialog*>( obj );

   if( bOverwrite )
      pDlg->mData.mStyle |= FileDialogData::FDS_OVERWRITEPROMPT;
   else
      pDlg->mData.mStyle &= ~FileDialogData::FDS_OVERWRITEPROMPT;

   return true;
};

const char* SaveFileDialog::getOverwritePrompt(void* obj, const char* data)
{
   SaveFileDialog *pDlg = static_cast<SaveFileDialog*>( obj );
   if( pDlg->mData.mStyle & FileDialogData::FDS_OVERWRITEPROMPT )
      return StringTable->insert("true");
   else
      return StringTable->insert("false");
}

//-----------------------------------------------------------------------------
// OpenFolderDialog Implementation
//-----------------------------------------------------------------------------

OpenFolderDialog::OpenFolderDialog()
{
   mData.mStyle = FileDialogData::FDS_OPEN | FileDialogData::FDS_OVERWRITEPROMPT | FileDialogData::FDS_BROWSEFOLDER;

   mMustExistInDir = "";
}

IMPLEMENT_CONOBJECT(OpenFolderDialog);

void OpenFolderDialog::initPersistFields()
{
   addField("fileMustExist", TypeFilename, Offset(mMustExistInDir, OpenFolderDialog), "File that must in selected folder for it to be valid");

   Parent::initPersistFields();
}
