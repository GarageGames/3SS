//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include "xcodeTemplate/XCConfigurationList.h"
#include "xcodeTemplate/XCBuildConfiguration.h"

IMPLEMENT_GLOBAL_LIST(XCConfigurationList)

XCConfigurationList::XCConfigurationList()
{
    m_Owner = NULL;

    m_DefaultConfigurationIsVisible = "0";

    m_DefaultConfigurationName = "Release";

    // Add ourselves to the global list
    ADD_TO_GLOBAL_LIST()
}

XCConfigurationList::~XCConfigurationList()
{
}

bool XCConfigurationList::ProcessBuildConfigXML(TiXmlElement& buildConfigs)
{
    TiXmlElement* buildConfig = buildConfigs.FirstChildElement("BuildConfig");
    while(buildConfig)
    {
        XCBuildConfiguration* config = new XCBuildConfiguration();
        config->SetName(buildConfig->Attribute("name"));
        config->SetBaseConfigurationReferencePath(m_BaseConfigurationReferencePath);

        // Process the build settings
        config->ProcessConfigXML(*buildConfig);

        m_BuildConfigurations.push_back(config);

        buildConfig = buildConfig->NextSiblingElement("BuildConfig");
    }

    return true;
}

IMPLEMENT_WRITE_GLOBAL_LIST(XCConfigurationList)
{
    stream << endl << "/* Begin XCConfigurationList section */" << endl;
    U32 size = m_List.size();
    for(U32 i=0; i<size; ++i)
    {
        XCConfigurationList* config = m_List[i];

        stream << "\t\t";
        config->GetUUID().Write(stream);
        stream << " /* Build configuration list for " << config->GetOwner()->GetISA() << " \"" << config->GetOwner()->GetName() << "\" */";
        stream << " = {" << endl;

        stream << "\t\t\tisa = " << config->GetISA() << ";" << endl;

        if(config->m_BuildConfigurations.size() > 0)
        {
            stream << "\t\t\tbuildConfigurations = (" << endl;

            for(U32 j=0; j<config->m_BuildConfigurations.size(); ++j)
            {
                stream << "\t\t\t\t";
                config->m_BuildConfigurations[j]->GetUUID().Write(stream);
                if(!config->m_BuildConfigurations[j]->GetName().empty())
                {
                    stream << " /* " << config->m_BuildConfigurations[j]->GetName() << " */";
                }
                stream << "," << endl;
            }

            stream << "\t\t\t);" << endl;
        }

        stream << "\t\t\tdefaultConfigurationIsVisible = " << config->m_DefaultConfigurationIsVisible << ";" << endl;
        stream << "\t\t\tdefaultConfigurationName = " << config->m_DefaultConfigurationName << ";" << endl;

        stream << "\t\t};" << endl;
    }

    stream << "/* End XCConfigurationList section */" << endl;

    return true;
}
