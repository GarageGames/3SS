//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _GUIWEBVIEWCTRL_H_
#define _GUIWEBVIEWCTRL_H_

#include "gui/guiControl.h"
#include "graphics/TextureManager.h"

/// Renders a web page to a GuiControl
class GuiWebViewCtrl : public GuiControl
{
    typedef GuiControl Parent;

protected:
    StringTableEntry mURL;

    Point2I mWebSize;
    Point2I mTextureSize;
    Point2I mMousePos;

    S32 m_WebRenderId;

    GLuint mWebViewTexture;

    F32 mLastUpdateTime;

    bool mReceivedLastImage;

protected:
    void setSize(int width , int height);
    void rebuildTexture(int width , int height);
    Point2I& windowPosToTexturePos(int x, int y);

public:
    DECLARE_CONOBJECT(GuiWebViewCtrl);
    GuiWebViewCtrl();
    virtual ~GuiWebViewCtrl();

    static void initPersistFields();

    virtual bool onAdd();
    virtual void onRemove();
    virtual bool onWake();
    virtual void onSleep();

    void inspectPostApply();

    virtual void resize(const Point2I &newPosition, const Point2I &newExtent);

    const char* getURL() { return mURL; }
    void setURL(const char* url);

    void reloadAction();
    void backAction();
    void forwardAction();
    void stopAction();

    virtual void setFirstResponder();
    virtual void onLoseFirstResponder();

    virtual void onMouseMove(const GuiEvent &event);
    virtual void onMouseUp(const GuiEvent &event);
    virtual void onMouseDown(const GuiEvent &event);
    virtual void onMouseDragged(const GuiEvent &event);
    virtual bool onMouseWheelUp(const GuiEvent &event);
    virtual bool onMouseWheelDown(const GuiEvent &event);

    virtual bool onKeyDown(const GuiEvent &event);

    virtual void onRender(Point2I offset, const RectI &updateRect);

    // WebRender callbacks
    void onLoadStarted();
    void onFinishedLoading(bool state);
    void onConnectionError(const char* error);
    void onLinkClicked(const char* url);
    void onUpdateView(bool newImage, S32 imageWidth, S32 imageHeight, U8* bits);
    void onURLChanged(const char* url);
};

#endif  // _GUIWEBVIEWCTRL_H_
