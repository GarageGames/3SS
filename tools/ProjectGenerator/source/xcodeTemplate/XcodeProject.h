//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _XCODEPROJECT_H
#define _XCODEPROJECT_H

#include <iostream>
#include <fstream>

using namespace std;

class PBXProject;

class XcodeProject
{
protected:
    ofstream m_File;

protected:
    void WriteFileHeader();

    void WriteFileFooter(PBXProject* project);

public:
    XcodeProject();
    virtual ~XcodeProject();

    bool CreateProjectFile(const string& filePath, const string& xcodePath);
};

#endif  // _XCODEPROJECT_H
