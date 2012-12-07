//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include "xcodeTemplate/FileTypeDatabase.h"
#include "platform/stringFunctions.h"
#include "tinyXML/tinyxml.h"
#include "platform/types.h"

FileTypeDatabase* FileTypeDatabase::m_Singleton = NULL;

FileTypeDatabase::FileTypeDatabase()
{
    if(!m_Singleton)
        m_Singleton = this;
}

FileTypeDatabase::~FileTypeDatabase()
{
    m_Singleton = NULL;
}

bool FileTypeDatabase::BuildDatabase()
{
    TiXmlDocument xml;
    bool result = xml.LoadFile("XcodeFileTypeDefinitions.xml");
    if(!result)
    {
        printf("FileTypeDatabase::BuildDatabase(): Cannot load file type definition file\n");
        return false;
    }

    TiXmlElement* root = xml.RootElement();
    if(!root)
    {
        printf("FileTypeDatabase::BuildDatabase(): File definition file is missing root element\n");
        return false;
    }

    TiXmlElement* e = root->FirstChildElement("FileType");
    while(e)
    {
        // Get the standard attributes
        const char* extension = e->Attribute("extension");
        const char* type = e->Attribute("type");
        const char* buildPhase = e->Attribute("buildPhase");

        if(extension && type && buildPhase)
        {
            // Build the type attribute
            FileTypeAttributesMap attributes;
            attributes.insert(FileTypeAttributesPair("type", type));

            // Add the buildPhase attribute
            attributes.insert(FileTypeAttributesPair("buildPhase", buildPhase));

            // Some file types may have the explicit attribute
            int value = 0;
            const char* exp = e->Attribute("explicit", &value);
            if(exp && value == 1)
            {
                // If the explicit value is 1 then add the attribute as true
                attributes.insert(FileTypeAttributesPair("explicit", "true"));
            }
            else
            {
                // Either this attribute doesn't exist, or it is set to something
                // other than 1.
                attributes.insert(FileTypeAttributesPair("explicit", "false"));
            }

            m_Database.insert(FileTypePair(extension, attributes));
        }

        e = e->NextSiblingElement("FileType");
    }

    return true;
}

bool FileTypeDatabase::HasFileType(const string& extension)
{
    if(m_Database.count(extension) == 0)
        return false;

    return true;
}

string FileTypeDatabase::GetFileType(const string& extension)
{
    if(m_Database.count(extension) == 0)
    {
        printf("FileTypeDatabase::GetFileType(): Cannot find file type '%s'\n", extension.c_str());
        return m_EmptyString;
    }

    // Get then attributes for the extension
    FileTypeMap::iterator itr = m_Database.find(extension);
    FileTypeAttributesMap attributes = itr->second;

    // Find the type attribute
    FileTypeAttributesMap::iterator aitr = attributes.find("type");
    return aitr->second;
}

string FileTypeDatabase::GetFileBuildPhase(const string& extension)
{
    if(m_Database.count(extension) == 0)
    {
        printf("FileTypeDatabase::GetFileBuildPhase(): Cannot find file type '%s'\n", extension.c_str());
        return m_EmptyString;
    }

    // Get then attributes for the extension
    FileTypeMap::iterator itr = m_Database.find(extension);
    FileTypeAttributesMap attributes = itr->second;

    // Find the type attribute
    FileTypeAttributesMap::iterator aitr = attributes.find("buildPhase");
    return aitr->second;
}

bool FileTypeDatabase::FileTypeIsExplicit(const string& extension)
{
    if(m_Database.count(extension) == 0)
    {
        printf("FileTypeDatabase::FileTypeIsExplicit(): Cannot find file type '%s'\n", extension.c_str());
        return false;
    }

    // Get then attributes for the extension
    FileTypeMap::iterator itr = m_Database.find(extension);
    FileTypeAttributesMap attributes = itr->second;

    // Find the type attribute
    FileTypeAttributesMap::iterator aitr = attributes.find("explicit");
    return Platform::StringCompareNoCase(aitr->second, "true");
}
