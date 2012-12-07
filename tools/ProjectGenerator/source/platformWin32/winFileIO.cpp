//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include "platform/types.h"
#include "platform/fileIO.h"
#include "platform/unicode.h"
#include "platformWin32/platformWin32.h"

#include <string>
#include <vector>

//-------------------------------------- Helper Functions
static void forwardslash(char *str)
{
    while(*str)
    {
        if(*str == '\\')
            *str = '/';
        str++;
    }
}

static void backslash(char *str)
{
    while(*str)
    {
        if(*str == '/')
            *str = '\\';
        str++;
    }
}

//-----------------------------------------------------------------------------

void Platform::getCurrentDirectory(string& cwd)
{
   char cwd_buf[2048];
   GetCurrentDirectoryA(2047, cwd_buf);
   forwardslash(cwd_buf);

   cwd = cwd_buf;
}

//-----------------------------------------------------------------------------

void Platform::DumpFiles(const string& path, vector<string>& outList)
{
    string searchPath;
    Platform::MakeFullPathName("*", searchPath, &path);

#ifdef UNICODE
    UTF16 search[1024];
    convertUTF8toUTF16((UTF8 *)searchPath.c_str(), search, sizeof(search));
#else
    char *search = searchPath.c_str();
#endif

    WIN32_FIND_DATA findData;
    HANDLE handle = FindFirstFile((LPCWSTR)search, &findData);
    if (handle == INVALID_HANDLE_VALUE)
    {
        return;
    }

    do
    {
#ifdef UNICODE
        char fnbuf[1024];
        Platform::convertUTF16toUTF8((UTF16*)findData.cFileName, (UTF8 *)fnbuf, sizeof(fnbuf));
#else
        char *fnbuf = findData.cFileName;
#endif

        if (findData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
            continue;

        outList.push_back(fnbuf);
    }
    while(FindNextFile(handle, &findData));

    FindClose(handle);
}

void Platform::DumpDirectories(const string& path, vector<string>& outList)
{
    string searchPath;
    Platform::MakeFullPathName("*", searchPath, &path);

#ifdef UNICODE
    UTF16 search[1024];
    convertUTF8toUTF16((UTF8 *)searchPath.c_str(), search, sizeof(search));
#else
    char *search = searchPath.c_str();
#endif

    WIN32_FIND_DATA findData;
    HANDLE handle = FindFirstFile((LPCWSTR)search, &findData);
    if (handle == INVALID_HANDLE_VALUE)
    {
        return;
    }

    do
    {
#ifdef UNICODE
        char fnbuf[1024];
        Platform::convertUTF16toUTF8((UTF16*)findData.cFileName, (UTF8 *)fnbuf, sizeof(fnbuf));
#else
        char *fnbuf = findData.cFileName;
#endif

        if (findData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
        {
            // make sure it is a directory
            if (findData.dwFileAttributes & (FILE_ATTRIBUTE_OFFLINE|FILE_ATTRIBUTE_SYSTEM) )                             
                continue;

            // skip . and .. directories
            if (strcmp(fnbuf, ".") == 0 || strcmp(fnbuf, "..") == 0)
                continue;

            // Skip excluded directores
            if(Platform::IsExcludedDirectory(fnbuf))
                continue;

            outList.push_back(fnbuf);
        }
    }
    while(FindNextFile(handle, &findData));

    FindClose(handle);
}

//-----------------------------------------------------------------------------

bool Platform::CreatePath(const char* path)
{
    char pathbuf[1024];
    const char *dir;
    pathbuf[0] = 0;
    U32 pathLen = 0;

    while((dir = strchr(path, '/')) != NULL)
    {
        strncpy(pathbuf + pathLen, path, dir - path);
        pathbuf[pathLen + dir-path] = 0;

#ifdef UNICODE
        UTF16 b[1024];
        Platform::convertUTF8toUTF16((UTF8 *)pathbuf, b, sizeof(b));
        BOOL ret = CreateDirectory((LPCWSTR)b, NULL);
#else
        BOOL ret = CreateDirectory(pathbuf, NULL);
#endif
        pathLen += dir - path;
        pathbuf[pathLen++] = '/';
        path = dir + 1;
    }
    return true;
}
