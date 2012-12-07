//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _FILEIO_H
#define _FILEIO_H

#include <string>
#include <vector>

using namespace std;

namespace Platform
{
    void MakeFullPathName(const string& inPath, string& outPath, const string* cwd=NULL);

    bool IsFullPath(const char *path);

    bool CreatePath(const char* path);

    void GetFileExtension(const string& file, string& ext);

    void getCurrentDirectory(string& cwd);

    void DumpFiles(const string& path, vector<string>& outList);

    void DumpDirectories(const string& path, vector<string>& outList);

    void AddExcludedDirectory(const string& dir);

    void CleanExcludedDirectories();

    bool IsExcludedDirectory(const string& dir);
}

#endif  // _FILEIO_H
