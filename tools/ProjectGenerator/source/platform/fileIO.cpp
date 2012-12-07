//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include "platform/types.h"
#include "platform/fileIO.h"
#include "platform/stringFunctions.h"

static vector<string> sg_ExcludedDirectoryList;

inline void catPath(char *dst, const char *src, U32 len)
{
   if(*dst != '/')
   {
      ++dst; --len;
      *dst = '/';
   }

   ++dst; --len;

   strncpy(dst, src, len);
   dst[len - 1] = 0;
}

#ifdef WIN32
static inline void _resolveLeadingSlash(string& buf)
{
   if(buf[0] != '/')
      return;

   buf = "c:" + buf;
}
#endif

void Platform::MakeFullPathName(const string& inPath, string& outPath, const string* cwd)
{
    char bspath[1024];
    strncpy(bspath, inPath.c_str(), sizeof(bspath));
    bspath[sizeof(bspath)-1] = 0;
   
    for(U32 i = 0;i < strlen(bspath);++i)
    {
        if(bspath[i] == '\\')
            bspath[i] = '/';
    }

    if(Platform::IsFullPath(bspath))
    {
        // Already a full path
        outPath = bspath;
        #if defined(WIN32)
            _resolveLeadingSlash(outPath);
        #endif
        
        return;
    }

    string currentDir;
    if(cwd == NULL)
        Platform::getCurrentDirectory(currentDir);
    else
        currentDir = *cwd;

    const U32 size = 1024;
    char buffer[size];
    strncpy(buffer, currentDir.c_str(), size);
    buffer[size-1] = 0;

    char *ptr = bspath;
    char *slash = NULL;
    char *endptr = buffer + strlen(buffer) - 1;

    do
    {
        slash = strchr(ptr, '/');
        if(slash)
        {
            *slash = 0;

            // Directory

            if(strcmp(ptr, "..") == 0)
            {
                // Parent
                endptr = strrchr(buffer, '/');
                *endptr = 0;
                endptr--;
            }
            else if(strcmp(ptr, ".") == 0)
            {
                // Current dir
            }
            else if(endptr)
            {
                catPath(endptr, ptr, size - (endptr - buffer));
                endptr += strlen(endptr) - 1;
            }
         
            ptr = slash + 1;
        }
        else if(endptr)
        {
            // File

            catPath(endptr, ptr, size - (endptr - buffer));
            endptr += strlen(endptr) - 1;
        }

    } while(slash);

    outPath = buffer;
}

bool Platform::IsFullPath(const char *path)
{
    // Quick way out
    if(path[0] == '/' || path[1] == ':')
        return true;

    return false;
}

void Platform::GetFileExtension(const string& file, string& ext)
{
    const char* result = strrchr(file.c_str(), '.');
    if(result)
    {
        // Remove the '.' from the extension
        if(strlen(result) >= 1)
        {
            result++;
        }

        ext = result;
    }
}

//-----------------------------------------------------------------------------

void Platform::AddExcludedDirectory(const string& dir)
{
    sg_ExcludedDirectoryList.push_back(dir);
}

void Platform::CleanExcludedDirectories()
{
    sg_ExcludedDirectoryList.clear();
}

bool Platform::IsExcludedDirectory(const string& dir)
{
    for(vector<string>::const_iterator itr = sg_ExcludedDirectoryList.begin(); itr != sg_ExcludedDirectoryList.end(); ++itr)
    {
        if(Platform::StringCompare(dir, *itr))
            return true;
    }

    return false;
}
