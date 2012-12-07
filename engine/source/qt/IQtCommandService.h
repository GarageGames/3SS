//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _IQTCOMMANDSERVICE_H
#define _IQTCOMMANDSERVICE_H

#include <QString>
#include <QMap>

class IQtCommandService
{
public:
    enum CommandId {
        C_WEBRENDER_CREATE,
        C_WEBRENDER_DESTROY,
        C_WEBRENDER_SETSIZE,
        C_WEBRENDER_SETURL,
        C_WEBRENDER_RELOAD_ACTION,
        C_WEBRENDER_BACK_ACTION,
        C_WEBRENDER_FORWARD_ACTION,
        C_WEBRENDER_STOP_ACTION,
        C_WEBRENDER_UPDATEVIEW,
        C_WEBRENDER_MOUSEMOVE,
        C_WEBRENDER_MOUSEDOWN,
        C_WEBRENDER_MOUSEUP,
        C_WEBRENDER_MOUSEDRAGGED,
        C_WEBRENDER_MOUSEWHEELDOWN,
        C_WEBRENDER_MOUSEWHEELUP,
        C_WEBRENDER_KEYBOARD_INPUT,
        C_DOWNLOAD_FILE,
        C_DOWNLOAD_CANCEL,
        C_DOWNLOAD_PAUSE,
        C_DOWNLOAD_RESUME,
        C_HTTPREQUEST_POST,
    };

    typedef QMap<QString, QString> KeyValueMap;

    struct Command {
        CommandId   m_Id;

        F32         m_F32Args[16];
        QString     m_StringArgs[16];
        S32         m_S32Args[16];
        KeyValueMap m_KeyValueMap;
    };

public:
    virtual void sendCommand(IQtCommandService::Command* c) = 0;
};

#endif // _IQTCOMMANDSERVICE_H
