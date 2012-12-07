//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _APPLICATION_H
#define _APPLICATION_H

#include "platform/types.h"
#include <string>

using namespace std;

class Application
{
public:
    enum ProjectTypes {
        PT_Unknown,
        PT_Xcode,
        PT_VS2010,
    };

protected:
    static Application* m_Singleton;

    ProjectTypes m_ProjectType;

    string m_ProjectFileName;

    string m_XcodeProjectFileName;

public:
    Application();
    virtual ~Application();

    void Begin(S32 argc, const char **argv);

    string& GetProjectFileName() { return m_ProjectFileName; }

    static Application* singleton() { return m_Singleton; }
};

#endif  // _APPLICATION_H
