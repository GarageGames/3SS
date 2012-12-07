//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "string/stringUnit.h"

ConsoleMethod(HTTPManager, SetHTTPProvider, bool, 3, 3, "(HTTPProvider) - Set the provider of http services.\n"
                                                        "@HTTPProvider The IHttpServicesProvider class that provides the http services to the manager.\n")
{
    IHttpServicesProvider* provider = dynamic_cast<IHttpServicesProvider*>(Sim::findObject(argv[2]));
    if(!provider)
    {
        Con::warnf("HTTPManager::SetHTTPProvider(): Invalid http provider");
        return false;
    }

    object->SetHTTPProvider( provider );

    return true;
}

//-----------------------------------------------------------------------------

ConsoleMethod(HTTPManager, addPostRequest, S32, 5, 5,   "(url, postParameters, callbackObjectId) - Add a HTTP post request.\n"
                                                        "@url The fully qualified URL to post to.\n"
                                                        "@postParameters The post parameter key=value pairs in tab delimited form.\n"
                                                        "@callbackObjectId The SimObject Id of the object to receive callbacks, or 0 for the HTTPManager to handle.\n"
                                                        "@return The Id of the post request, or -1 if there was a problem.")
{
    SimObjectId id = 0;
    SimObject* obj = Sim::findObject(argv[4]);
    if(obj)
        id = obj->getId();
    
    // Build the post key=value pairs list
    IHttpServicesProvider::PostDictionary* parameters = new IHttpServicesProvider::PostDictionary();

    // These bufferes are needed as StringUnit returns its own static buffer that is
    // over written after each call.
    char pairBuffer[1024];
    char keyBuffer[1024];

    U32 count = StringUnit::getUnitCount(argv[3], "\t");
    for(U32 i=0; i<count; ++i)
    {
        // Get the next key=value pair
        const char* pair = StringUnit::getUnit(argv[3], i, "\t");
        dStrcpy(pairBuffer, pair);

        // Extract the key and value
        const char* key = StringUnit::getUnit(pairBuffer, 0, "=");
        dStrcpy(keyBuffer, key);
        const char* value = StringUnit::getUnit(pairBuffer, 1, "=");

        // Insert the pair into the parameters
        parameters->insertEqual(StringTable->insert(keyBuffer, true), StringTable->insert(value, true));
    }

    // Add the post request
    return object->addPostRequest( argv[2], parameters, id);
}
