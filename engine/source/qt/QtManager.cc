//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "qt/QtManager.h"
#include "qt/QtThread.h"
#include "qt/QtAppHandler.h"
#include "qt/QtCommandPump.h"
#include "platform/event.h"

#include <QDesktopServices>

// Script bindings.
#include "qt/QtManager_ScriptBinding.h"

//-----------------------------------------------------------------------------

QtManager* QtManager::s_Manager=NULL;

QtManager* QtManager::singleton()
{
    return s_Manager;
}

//-----------------------------------------------------------------------------

IMPLEMENT_CONOBJECT( QtManager );

QtManager::QtManager()
{
#ifdef __MACOSX__
   m_UseMultithread = false;
#else
   m_UseMultithread = true;
#endif
    m_pAppThread = NULL;
    m_pAppHandler = NULL;

    m_pCommandService = NULL;

    m_WebRenderCounter = 0;

    m_HttpRequestCounter = 0;
}

QtManager::~QtManager()
{
}

bool QtManager::onAdd()
{
    AssertFatal( m_pAppThread == NULL, "QtManager::onAdd(): Qt application thread already exists" );

    if( !Parent::onAdd() )
        return false;

    bool result = false;
    if(m_UseMultithread)
    {
        result = startUpThread();
    }
    else
    {
        result = startUpQtAppHandler();
    }

    if(result)
        s_Manager = this;

    createPathExpandos();

    initNetworkManager();

    return result;
}

void QtManager::onRemove()
{
    if(m_pAppThread)
    {
        // Stop the thread and wait
        m_pAppThread->stopAndWait();

        delete m_pAppThread;
        m_pAppThread = NULL;
    }
    else if(m_pAppHandler)
    {
        // Stop the application handler
        m_pAppHandler->shutDown();
        delete m_pAppHandler;
        m_pAppHandler = NULL;
    }

    m_pCommandService = NULL;

    if(s_Manager == this)
        s_Manager = NULL;

    // Call parent.
    Parent::onRemove();
}

//-----------------------------------------------------------------------------

bool QtManager::startUpThread()
{
    bool result = false;

    if(m_pAppThread)
    {
        Con::warnf("QtManager: Qt thread thread already exists.");
    }
    else
    {
        m_pAppThread = new QtThread();

        if(m_pAppThread)
        {
            m_pCommandService = m_pAppThread;
            result = true;
        }
    }

    return result;
}

//-----------------------------------------------------------------------------

bool QtManager::startUpQtAppHandler()
{
    bool result = false;

    if(m_pAppHandler)
    {
        Con::warnf("QtManager: Qt application thread already exists.");
    }
    else
    {
        m_pAppHandler = new QtAppHandler();

        if(m_pAppHandler)
        {
            m_pAppHandler->startUp();
            m_pCommandService = m_pAppHandler;
            result = true;
        }
    }

    return result;
}

//-----------------------------------------------------------------------------

void QtManager::initNetworkManager()
{
    m_NetManager.init();
}

bool QtManager::saveCookies(const char* path)
{
    return m_NetManager.saveCookies(path);
}

bool QtManager::loadCookies(const char* path)
{
    return m_NetManager.loadCookies(path);
}

void QtManager::clearCookies()
{
    m_NetManager.clearCookies();
}

//-----------------------------------------------------------------------------

S32 QtManager::CreateWebRender(SimObjectId caller, S32 width, S32 height)
{
    if(!m_pCommandService)
        return -1;

    S32 id = getNextWebRenderId();

    IQtCommandService::Command* c = new IQtCommandService::Command();

    c->m_Id = IQtCommandService::C_WEBRENDER_CREATE;
    c->m_S32Args[0] = id;
    c->m_S32Args[1] = width;
    c->m_S32Args[2] = height;
    c->m_S32Args[3] = caller;

    m_pCommandService->sendCommand(c);

    return id;
}

void QtManager::DestroyWebRender(S32 id)
{
    if(!m_pCommandService)
        return;

    IQtCommandService::Command* c = new IQtCommandService::Command();

    c->m_Id = IQtCommandService::C_WEBRENDER_DESTROY;
    c->m_S32Args[0] = id;

    m_pCommandService->sendCommand(c);
}

void QtManager::WebRenderSetSize(S32 id, S32 width, S32 height)
{
    if(!m_pCommandService)
        return;

    IQtCommandService::Command* c = new IQtCommandService::Command();

    c->m_Id = IQtCommandService::C_WEBRENDER_SETSIZE;
    c->m_S32Args[0] = id;
    c->m_S32Args[1] = width;
    c->m_S32Args[2] = height;

    m_pCommandService->sendCommand(c);
}

void QtManager::WebRenderSetUrl(S32 id, const char* url)
{
    if(!m_pCommandService)
        return;

    IQtCommandService::Command* c = new IQtCommandService::Command();

    c->m_Id = IQtCommandService::C_WEBRENDER_SETURL;
    c->m_S32Args[0] = id;
    c->m_StringArgs[0] = url;

    m_pCommandService->sendCommand(c);
}

void QtManager::WebRenderReloadAction(S32 id)
{
    if(!m_pCommandService)
        return;

    IQtCommandService::Command* c = new IQtCommandService::Command();

    c->m_Id = IQtCommandService::C_WEBRENDER_RELOAD_ACTION;
    c->m_S32Args[0] = id;

    m_pCommandService->sendCommand(c);
}

void QtManager::WebRenderBackAction(S32 id)
{
    if(!m_pCommandService)
        return;

    IQtCommandService::Command* c = new IQtCommandService::Command();

    c->m_Id = IQtCommandService::C_WEBRENDER_BACK_ACTION;
    c->m_S32Args[0] = id;

    m_pCommandService->sendCommand(c);
}

void QtManager::WebRenderForwardAction(S32 id)
{
    if(!m_pCommandService)
        return;

    IQtCommandService::Command* c = new IQtCommandService::Command();

    c->m_Id = IQtCommandService::C_WEBRENDER_FORWARD_ACTION;
    c->m_S32Args[0] = id;

    m_pCommandService->sendCommand(c);
}

void QtManager::WebRenderStopAction(S32 id)
{
    if(!m_pCommandService)
        return;

    IQtCommandService::Command* c = new IQtCommandService::Command();

    c->m_Id = IQtCommandService::C_WEBRENDER_STOP_ACTION;
    c->m_S32Args[0] = id;

    m_pCommandService->sendCommand(c);
}

void QtManager::WebRenderUpdateView(S32 id, bool force)
{
    if(!m_pCommandService)
        return;

    IQtCommandService::Command* c = new IQtCommandService::Command();

    c->m_Id = IQtCommandService::C_WEBRENDER_UPDATEVIEW;
    c->m_S32Args[0] = id;
    c->m_S32Args[1] = force;

    m_pCommandService->sendCommand(c);
}

//-----------------------------------------------------------------------------

void QtManager::WebRenderMouseMove(S32 id, Point2I point)
{
    if(!m_pCommandService)
        return;

    IQtCommandService::Command* c = new IQtCommandService::Command();

    c->m_Id = IQtCommandService::C_WEBRENDER_MOUSEMOVE;
    c->m_S32Args[0] = id;
    c->m_S32Args[1] = point.x;
    c->m_S32Args[2] = point.y;

    m_pCommandService->sendCommand(c);
}

void QtManager::WebRenderMouseDown(S32 id, Point2I point)
{
    if(!m_pCommandService)
        return;

    IQtCommandService::Command* c = new IQtCommandService::Command();

    c->m_Id = IQtCommandService::C_WEBRENDER_MOUSEDOWN;
    c->m_S32Args[0] = id;
    c->m_S32Args[1] = point.x;
    c->m_S32Args[2] = point.y;

    m_pCommandService->sendCommand(c);
}

void QtManager::WebRenderMouseUp(S32 id, Point2I point)
{
    if(!m_pCommandService)
        return;

    IQtCommandService::Command* c = new IQtCommandService::Command();

    c->m_Id = IQtCommandService::C_WEBRENDER_MOUSEUP;
    c->m_S32Args[0] = id;
    c->m_S32Args[1] = point.x;
    c->m_S32Args[2] = point.y;

    m_pCommandService->sendCommand(c);
}

void QtManager::WebRenderMouseDragged(S32 id, Point2I point)
{
    if(!m_pCommandService)
        return;

    IQtCommandService::Command* c = new IQtCommandService::Command();

    c->m_Id = IQtCommandService::C_WEBRENDER_MOUSEDRAGGED;
    c->m_S32Args[0] = id;
    c->m_S32Args[1] = point.x;
    c->m_S32Args[2] = point.y;

    m_pCommandService->sendCommand(c);
}

//-----------------------------------------------------------------------------

void QtManager::WebRenderMouseWheelDown(S32 id, Point2I point)
{
    if(!m_pCommandService)
        return;

    IQtCommandService::Command* c = new IQtCommandService::Command();

    c->m_Id = IQtCommandService::C_WEBRENDER_MOUSEWHEELDOWN;
    c->m_S32Args[0] = id;
    c->m_S32Args[1] = point.x;
    c->m_S32Args[2] = point.y;

    m_pCommandService->sendCommand(c);
}

void QtManager::WebRenderMouseWheelUp(S32 id, Point2I point)
{
    if(!m_pCommandService)
        return;

    IQtCommandService::Command* c = new IQtCommandService::Command();

    c->m_Id = IQtCommandService::C_WEBRENDER_MOUSEWHEELUP;
    c->m_S32Args[0] = id;
    c->m_S32Args[1] = point.x;
    c->m_S32Args[2] = point.y;

    m_pCommandService->sendCommand(c);
}

//-----------------------------------------------------------------------------

void QtManager::WebRenderKeyboardInput(S32 id, U32 key, U32 keyASCII)
{
    if(!m_pCommandService)
        return;

    IQtCommandService::Command* c = new IQtCommandService::Command();

    c->m_Id = IQtCommandService::C_WEBRENDER_KEYBOARD_INPUT;
    c->m_S32Args[0] = id;
    c->m_S32Args[1] = (S32)convertTorqueKeyToQT(key);
    c->m_S32Args[2] = (S32)keyASCII;

    m_pCommandService->sendCommand(c);
}

//-----------------------------------------------------------------------------

void QtManager::DownloadFile(S32 callback, const char* url, const char* path, bool deleteExisting)
{
    if(!m_pCommandService)
        return;

    IQtCommandService::Command* c = new IQtCommandService::Command();

    c->m_Id = IQtCommandService::C_DOWNLOAD_FILE;
    c->m_S32Args[0] = callback;
    c->m_S32Args[1] = deleteExisting;
    c->m_StringArgs[0] = url;
    c->m_StringArgs[1] = path;

    m_pCommandService->sendCommand(c);
}

//-----------------------------------------------------------------------------

void QtManager::CancelDownload()
{
    if(!m_pCommandService)
        return;

    IQtCommandService::Command* c = new IQtCommandService::Command();

    c->m_Id = IQtCommandService::C_DOWNLOAD_CANCEL;

    m_pCommandService->sendCommand(c);
}

//-----------------------------------------------------------------------------

void QtManager::PauseDownload()
{
    if(!m_pCommandService)
        return;

    IQtCommandService::Command* c = new IQtCommandService::Command();

    c->m_Id = IQtCommandService::C_DOWNLOAD_PAUSE;

    m_pCommandService->sendCommand(c);
}

void QtManager::ResumeDownload()
{
    if(!m_pCommandService)
        return;

    IQtCommandService::Command* c = new IQtCommandService::Command();

    c->m_Id = IQtCommandService::C_DOWNLOAD_RESUME;

    m_pCommandService->sendCommand(c);
}

//-----------------------------------------------------------------------------

S32 QtManager::HttpPost(S32 callback, const char* url, PostDictionary* postArray)
{
    if(!m_pCommandService)
        return -1;

    S32 requestId = m_HttpRequestCounter++;;

    IQtCommandService::Command* c = new IQtCommandService::Command();

    c->m_Id = IQtCommandService::C_HTTPREQUEST_POST;
    c->m_S32Args[0] = callback;
    c->m_S32Args[1] = requestId;
    c->m_StringArgs[0] = url;

    // Copy the array into the key/value map
    for(PostDictionary::iterator itr = postArray->begin(); itr != postArray->end(); ++itr)
    {
        c->m_KeyValueMap.insert(itr->key, itr->value);
    }

    m_pCommandService->sendCommand(c);

    return requestId;
}

//-----------------------------------------------------------------------------

U32 QtManager::convertTorqueKeyToQT(U32 torqueKey)
{
    switch (torqueKey)
    {
        case KEY_BACKSPACE:  return Qt::Key_Backspace;
        case KEY_TAB:        return Qt::Key_Tab;
        case KEY_RETURN:     return Qt::Key_Return;
        case KEY_CONTROL:    return Qt::Key_Control;
        case KEY_ALT:        return Qt::Key_Alt;
        case KEY_SHIFT:      return Qt::Key_Shift;
        case KEY_PAUSE:      return Qt::Key_Pause;
        case KEY_CAPSLOCK:   return Qt::Key_CapsLock;
        case KEY_ESCAPE:     return Qt::Key_Escape;
        case KEY_SPACE:      return Qt::Key_Space;
        case KEY_PAGE_DOWN:  return Qt::Key_PageDown;
        case KEY_PAGE_UP:    return Qt::Key_PageUp;
        case KEY_END:        return Qt::Key_End;
        case KEY_HOME:       return Qt::Key_Home;
        case KEY_LEFT:       return Qt::Key_Left;
        case KEY_UP:         return Qt::Key_Up;
        case KEY_RIGHT:      return Qt::Key_Right;
        case KEY_DOWN:       return Qt::Key_Down;
        case KEY_PRINT:      return Qt::Key_Print;
        case KEY_INSERT:     return Qt::Key_Insert;
        case KEY_DELETE:     return Qt::Key_Delete;
        case KEY_HELP:       return Qt::Key_Help;
        case KEY_0:          return Qt::Key_0;
        case KEY_1:          return Qt::Key_1;
        case KEY_2:          return Qt::Key_2;
        case KEY_3:          return Qt::Key_3;
        case KEY_4:          return Qt::Key_4;
        case KEY_5:          return Qt::Key_5;
        case KEY_6:          return Qt::Key_6;
        case KEY_7:          return Qt::Key_7;
        case KEY_8:          return Qt::Key_8;
        case KEY_9:          return Qt::Key_9;
        case KEY_A:          return Qt::Key_A;
        case KEY_B:          return Qt::Key_B;
        case KEY_C:          return Qt::Key_C;
        case KEY_D:          return Qt::Key_D;
        case KEY_E:          return Qt::Key_E;
        case KEY_F:          return Qt::Key_F;
        case KEY_G:          return Qt::Key_G;
        case KEY_H:          return Qt::Key_H;
        case KEY_I:          return Qt::Key_I;
        case KEY_J:          return Qt::Key_J;
        case KEY_K:          return Qt::Key_K;
        case KEY_L:          return Qt::Key_L;
        case KEY_M:          return Qt::Key_M;
        case KEY_N:          return Qt::Key_N;
        case KEY_O:          return Qt::Key_O;
        case KEY_P:          return Qt::Key_P;
        case KEY_Q:          return Qt::Key_Q;
        case KEY_R:          return Qt::Key_R;
        case KEY_S:          return Qt::Key_S;
        case KEY_T:          return Qt::Key_T;
        case KEY_U:          return Qt::Key_U;
        case KEY_V:          return Qt::Key_V;
        case KEY_W:          return Qt::Key_W;
        case KEY_X:          return Qt::Key_X;
        case KEY_Y:          return Qt::Key_Y;
        case KEY_Z:          return Qt::Key_Z;
        case KEY_TILDE:      return Qt::Key_AsciiTilde;
        case KEY_MINUS:      return Qt::Key_Minus;
        case KEY_EQUALS:     return Qt::Key_Equal;
        case KEY_LBRACKET:   return Qt::Key_BracketLeft;
        case KEY_RBRACKET:   return Qt::Key_BracketRight;
        case KEY_BACKSLASH:  return Qt::Key_Backslash;
        case KEY_SEMICOLON:  return Qt::Key_Semicolon;
        case KEY_APOSTROPHE: return Qt::Key_Apostrophe;
        case KEY_COMMA:      return Qt::Key_Comma;
        case KEY_PERIOD:     return Qt::Key_Period;
        case KEY_SLASH:      return Qt::Key_Slash;
        case KEY_NUMPAD0:    return Qt::Key_0;
        case KEY_NUMPAD1:    return Qt::Key_1;
        case KEY_NUMPAD2:    return Qt::Key_2;
        case KEY_NUMPAD3:    return Qt::Key_3;
        case KEY_NUMPAD4:    return Qt::Key_4;
        case KEY_NUMPAD5:    return Qt::Key_5;
        case KEY_NUMPAD6:    return Qt::Key_6;
        case KEY_NUMPAD7:    return Qt::Key_7;
        case KEY_NUMPAD8:    return Qt::Key_8;
        case KEY_NUMPAD9:    return Qt::Key_9;
        case KEY_MULTIPLY:   return Qt::Key_Asterisk;
        case KEY_ADD:        return Qt::Key_Plus;
        case KEY_SUBTRACT:   return Qt::Key_Minus;
        case KEY_DECIMAL:    return Qt::Key_Period;
        case KEY_DIVIDE:     return Qt::Key_Slash;
        case KEY_NUMPADENTER:return Qt::Key_Enter;
        case KEY_F1:         return Qt::Key_F1;
        case KEY_F2:         return Qt::Key_F2;
        case KEY_F3:         return Qt::Key_F3;
        case KEY_F4:         return Qt::Key_F4;
        case KEY_F5:         return Qt::Key_F5;
        case KEY_F6:         return Qt::Key_F6;
        case KEY_F7:         return Qt::Key_F7;
        case KEY_F8:         return Qt::Key_F8;
        case KEY_F9:         return Qt::Key_F9;
        case KEY_F10:        return Qt::Key_F10;
        case KEY_F11:        return Qt::Key_F11;
        case KEY_F12:        return Qt::Key_F12;
        case KEY_F13:        return Qt::Key_F13;
        case KEY_F14:        return Qt::Key_F14;
        case KEY_F15:        return Qt::Key_F15;
        case KEY_F16:        return Qt::Key_F16;
        case KEY_F17:        return Qt::Key_F17;
        case KEY_F18:        return Qt::Key_F18;
        case KEY_F19:        return Qt::Key_F19;
        case KEY_F20:        return Qt::Key_F20;
        case KEY_F21:        return Qt::Key_F21;
        case KEY_F22:        return Qt::Key_F22;
        case KEY_F23:        return Qt::Key_F23;
        case KEY_F24:        return Qt::Key_F24;
        case KEY_NUMLOCK:    return Qt::Key_NumLock;
        case KEY_SCROLLLOCK: return Qt::Key_ScrollLock;
        case KEY_LCONTROL:   return Qt::Key_Control;
        case KEY_RCONTROL:   return Qt::Key_Control;
        case KEY_LALT:       return Qt::Key_Alt;
        case KEY_RALT:       return Qt::Key_Alt;
        case KEY_LSHIFT:     return Qt::Key_Shift;
        case KEY_RSHIFT:     return Qt::Key_Shift;
    };

    return KEY_NULL;
}

//-----------------------------------------------------------------------------

void QtManager::createPathExpandos()
{
    // user's desktop directory
    Con::addPathExpando( "DesktopFileLocation", QDesktopServices::storageLocation(QDesktopServices::DesktopLocation).toLatin1() );

    // user's document
    Con::addPathExpando( "DocumentsFileLocation", QDesktopServices::storageLocation(QDesktopServices::DocumentsLocation).toLatin1() );

    // user's fonts
    Con::addPathExpando( "FontsFileLocation", QDesktopServices::storageLocation(QDesktopServices::FontsLocation).toLatin1() );

    // user's applications
    Con::addPathExpando( "ApplicationsFileLocation", QDesktopServices::storageLocation(QDesktopServices::ApplicationsLocation).toLatin1() );

    // user's music
    Con::addPathExpando( "MusicFileLocation", QDesktopServices::storageLocation(QDesktopServices::MusicLocation).toLatin1() );

    // user's movies
    Con::addPathExpando( "MoviesFileLocation", QDesktopServices::storageLocation(QDesktopServices::MoviesLocation).toLatin1() );

    // user's pictures
    Con::addPathExpando( "PicturesFileLocation", QDesktopServices::storageLocation(QDesktopServices::PicturesLocation).toLatin1() );

    // system's temporary directory
    Con::addPathExpando( "TempFileLocation", QDesktopServices::storageLocation(QDesktopServices::TempLocation).toLatin1() );

    // user's home directory
    Con::addPathExpando( "HomeFileLocation", QDesktopServices::storageLocation(QDesktopServices::HomeLocation).toLatin1() );

    // directory location where persistent application data can be stored
    Con::addPathExpando( "DataFileLocation", QDesktopServices::storageLocation(QDesktopServices::DataLocation).toLatin1() );

    // directory location where user-specific non-essential (cached) data should be written
    Con::addPathExpando( "CacheFileLocation", QDesktopServices::storageLocation(QDesktopServices::CacheLocation).toLatin1() );
}
