//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _PLATFORM_H_
#define _PLATFORM_H_

#ifndef _TORQUECONFIG_H_
#include "torqueConfig.h"
#endif

#ifndef _TORQUE_TYPES_H_
#include "platform/types.h"
#endif

#ifndef _PLATFORMASSERT_H_
#include "platform/platformAssert.h"
#endif

#ifndef _MSGBOX_H_
#include "platform/nativeDialogs/msgBox.h"
#endif

#ifndef TORQUE_OS_WIN32
#include <algorithm>
#endif

#ifndef _PLATFORM_ENDIAN_H_
#include "platform/platformEndian.h"
#endif

#ifndef _PLATFORM_CPU_H_
#include "platform/platformCPU.h"
#endif

#ifndef _PLATFORM_STRING_H_
#include "platform/platformString.h"
#endif

#ifndef _PLATFORM_NETWORK_H_
#include "platform/platformNetwork.h"
#endif

#ifndef _PLATFORM_MEMORY_H_
#include "platform/platformMemory.h"
#endif

#ifndef _PLATFORMFONT_H_
#include "platform/platformFont.h"
#endif

#ifndef _PLATFORM_MATH_H_
#include "platform/platformMath.h"
#endif

#ifndef _PLATFORM_TIME_MANAGER_H_
#include "platform/platformTimeManager.h"
#endif

//------------------------------------------------------------------------------

template <class T> class Vector;
class Point2I;

//------------------------------------------------------------------------------

struct Platform
{
    struct LocalTime
    {
        U8  sec;        // seconds after minute (0-59)
        U8  min;        // Minutes after hour (0-59)
        U8  hour;       // Hours after midnight (0-23)
        U8  month;      // Month (0-11; 0=january)
        U8  monthday;   // Day of the month (1-31)
        U8  weekday;    // Day of the week (0-6, 6=sunday)
        U16 year;       // current year minus 1900
        U16 yearday;    // Day of year (0-365)
        bool isdst;     // true if daylight savings time is active
    };

    struct FileInfo
    {
        const char* pFullPath;
        const char* pFileName;
        U32 fileSize;

        bool equal( const FileInfo& fileInfo )
        {
            return
                fileInfo.pFullPath == pFullPath &&
                fileInfo.pFileName == pFileName &&
                fileInfo.fileSize == fileSize;
        }
    };

    struct VolumeInformation
    {
        StringTableEntry  RootPath;
        StringTableEntry  Name;
        StringTableEntry  FileSystem;
        U32               SerialNumber;
        U32               Type;
        bool              ReadOnly;
    };

    typedef void* FILE_HANDLE;
    enum DFILE_STATUS
    {
        DFILE_OK = 1
    };

	enum ALERT_ASSERT_RESULT
	{
		ALERT_ASSERT_DEBUG,
		ALERT_ASSERT_IGNORE,
		ALERT_ASSERT_IGNORE_ALL,
		ALERT_ASSERT_EXIT
	};


    /// Application.
    static void init();
    static void initConsole();
    static void process();
    static void shutdown();
    static void sleep(U32 ms);
    static bool excludeOtherInstances(const char *string);
    static bool checkOtherInstances(const char *string);
    static void restartInstance();
    static void postQuitMessage(const U32 in_quitVal);
    static void forceShutdown(S32 returnValue);

    /// User.
    static StringTableEntry getUserHomeDirectory();
    static StringTableEntry getUserDataDirectory();
    static bool getUserIsAdministrator();

    /// Window.
    static void initWindow(const Point2I &initialSize, const char *name);
    static void setWindowTitle( const char* title );
    static void setWindowSize( U32 newWidth, U32 newHeight );
    static const Point2I &getWindowSize();
    static void minimizeWindow();
    static void restoreWindow();
    static void setWindowLocked(bool locked);

    /// GUI.
    static void AlertOK(const char *windowTitle, const char *message);
    static bool AlertOKCancel(const char *windowTitle, const char *message);
    static bool AlertRetry(const char *windowTitle, const char *message);
    static bool AlertYesNo(const char *windowTitle, const char *message);
	static ALERT_ASSERT_RESULT AlertAssert(const char *windowTitle, const char *message);
    static S32 messageBox(const UTF8 *title, const UTF8 *message, MBButtons buttons = MBOkCancel, MBIcons icon = MIInformation);
    static void CreateMenuBar();
    static void DestroyMenuBar();
    static bool HasMenuBar();

    /// Input.
    static void enableKeyboardTranslation(void);
    static void disableKeyboardTranslation(void);

    /// Date & Time.
    static U32 getTime( void );
    static U32 getVirtualMilliseconds( void );
    static U32 getRealMilliseconds( void );
    static void advanceTime(U32 delta);
    static S32 getBackgroundSleepTime();
    static void getLocalTime(LocalTime &);
    static void fileToLocalTime(const FileTime &ft, LocalTime *lt);
    static S32 compareFileTimes(const FileTime &a, const FileTime &b);
    static bool stringToFileTime(const char * string, FileTime * time);
    static bool fileTimeToString(FileTime * time, char * string, U32 strLen);

    /// Math.
    static float getRandom();

    /// Debug.
    static void debugBreak();
    static void outputDebugString(const char *string);
    static void cprintf(const char* str);

    /// File IO.
    static bool fileExistsAtPath(const char *filePath);
    static bool cdFileExists(const char *filePath, const char *volumeName, S32 serialNum);

    static StringTableEntry getCurrentDirectory();
    static bool setCurrentDirectory(StringTableEntry newDir);
    static StringTableEntry getTemporaryDirectory();
    static StringTableEntry getTemporaryFileName();
    #ifdef TORQUE_OS_IOS
        static StringTableEntry getWorkingDirectory();
    #endif
    static StringTableEntry getExecutableName();
    static StringTableEntry getExecutablePath(); 
    static void setMainDotCsDir(const char *dir);
    static StringTableEntry getMainDotCsDir();
    static StringTableEntry getPrefsPath(const char *file = NULL);
    static char *makeFullPathName(const char *path, char *buffer, U32 size, const char *cwd = NULL);
    static StringTableEntry stripBasePath(const char *path);
    static bool isFullPath(const char *path);
    static StringTableEntry makeRelativePathName(const char *path, const char *to);
    static bool dumpPath(const char *in_pBasePath, Vector<FileInfo>& out_rFileVector, S32 recurseDepth = -1);
    static bool dumpDirectories( const char *path, Vector<StringTableEntry> &directoryVector, S32 depth = 0, bool noBasePath = false );
    static bool hasSubDirectory( const char *pPath );
    static bool getFileTimes(const char *filePath, FileTime *createTime, FileTime *modifyTime);
    static bool isFile(const char *pFilePath);
    static S32  getFileSize(const char *pFilePath);
    static bool hasExtension(const char* pFilename, const char* pExtension);
    static bool isDirectory(const char *pDirPath);
    static bool isSubDirectory(const char *pParent, const char *pDir);
    static void addExcludedDirectory(const char *pDir);
    static void clearExcludedDirectories();
    static bool isExcludedDirectory(const char *pDir);
    static void getVolumeNamesList( Vector<const char*>& out_rNameVector, bool bOnlyFixedDrives = false );
    static void getVolumeInformationList( Vector<VolumeInformation>& out_rVolumeInfoVector, bool bOnlyFixedDrives = false );
    static bool createPath(const char *path);
    static bool deleteDirectory( const char* pPath );
    static bool fileDelete(const char *name);
    static bool fileRename(const char *oldName, const char *newName);
    static bool fileTouch(const char *name);
    static bool pathCopy(const char *fromName, const char *toName, bool nooverwrite = true);
    static StringTableEntry osGetTemporaryDirectory();
    static S32 runBatchFile( const char* fileName, const char* batchArgs, bool blocking = false );
    static bool isBatchFileDone( int batchID );

    /// Misc.
    static StringTableEntry createUUID( void );
    static bool openWebBrowser( const char* webAddress );
    static void openFolder( const char* path );
    static const char* getClipboard();
    static bool setClipboard(const char *text);
};

#endif // _PLATFORM_H_
