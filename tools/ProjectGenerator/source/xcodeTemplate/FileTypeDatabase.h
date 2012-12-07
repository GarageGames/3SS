//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _FILETYPEDATABASE_H
#define _FILETYPEDATABASE_H

#include <map>
#include <string>

using namespace std;

class FileTypeDatabase
{
protected:
    static FileTypeDatabase* m_Singleton;

    typedef map<string, string> FileTypeAttributesMap;
    typedef pair<string, string> FileTypeAttributesPair;

    typedef map<string, FileTypeAttributesMap> FileTypeMap;
    typedef pair<string, FileTypeAttributesMap> FileTypePair;

    FileTypeMap m_Database;

    string m_EmptyString;

public:
    FileTypeDatabase();
    virtual ~FileTypeDatabase();

    bool BuildDatabase();

    bool HasFileType(const string& extension);

    string GetFileType(const string& extension);

    string GetFileBuildPhase(const string& extension);

    bool FileTypeIsExplicit(const string& extension);

    static FileTypeDatabase* singleton() { return m_Singleton; }
};

#endif  // _FILETYPEDATABASE_H
