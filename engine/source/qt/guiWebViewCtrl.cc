//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "console/console.h"
#include "console/consoleTypes.h"
#include "graphics/dgl.h"
#include "gui/guiControl.h"
#include "graphics/TextureManager.h"
#include "sim/simBase.h"
#include "qt/QtManager.h"
#include "qt/guiWebViewCtrl.h"

IMPLEMENT_CONOBJECT(GuiWebViewCtrl);

#define GL_BGRA_EXT                       0x80E1
#define GUIWEBVIEWCTRL_UPDATETICK         32
#define GUIWEBVIEWCTRL_WHEELSCROLL        66

//-----------------------------------------------------------------------------

GuiWebViewCtrl::GuiWebViewCtrl() : mWebViewTexture(0)
{
    mWebSize.set(512, 512);
    mLastUpdateTime = 0;
    mReceivedLastImage = true;

    m_WebRenderId = -1;

    mURL = StringTable->insert("");
}

GuiWebViewCtrl::~GuiWebViewCtrl()
{
}

//-----------------------------------------------------------------------------

void GuiWebViewCtrl::setSize(int width , int height)
{
    // Check if we're already at the given size
    if(mWebSize.x == width && mWebSize.y == height)
        return;

    mWebSize.x = width;
    mWebSize.y = height;

    //rebuildTexture(width, height);

    // Notify the WebRender of the new size
    QtManager::singleton()->WebRenderSetSize(m_WebRenderId, width, height);
}

void GuiWebViewCtrl::rebuildTexture(int width , int height)
{
    // Delete any previous texture
    if (mWebViewTexture)
        glDeleteTextures(1, &mWebViewTexture);

    // Create the correctly sized texture
    glGenTextures(1, &mWebViewTexture);
    glBindTexture(GL_TEXTURE_2D, mWebViewTexture);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, width, height, 0, GL_RGB, GL_UNSIGNED_BYTE, 0);

    mTextureSize.x = width;
    mTextureSize.y = height;
}

//-----------------------------------------------------------------------------

Point2I& GuiWebViewCtrl::windowPosToTexturePos( int x, int y)
{
    F32 ax = F32(mWebSize.x)/F32(mBounds.extent.x);
    F32 ay = F32(mWebSize.y)/F32(mBounds.extent.y);

    Point2I localPosition = globalToLocalCoord(Point2I(x,y));
    mMousePos.x = localPosition.x * ax;
    mMousePos.y = localPosition.y * ay;

    return mMousePos;
}

//-----------------------------------------------------------------------------

void GuiWebViewCtrl::initPersistFields()
{
    addField("URL", TypeString, Offset(mURL, GuiWebViewCtrl));
    Parent::initPersistFields();
}

//-----------------------------------------------------------------------------

bool GuiWebViewCtrl::onAdd()
{
    if (!Parent::onAdd())
        return false;

    mWebSize.x = mBounds.extent.x;
    mWebSize.y = mBounds.extent.y;

    // Generate the web render
    m_WebRenderId = QtManager::singleton()->CreateWebRender(getId(), mWebSize.x, mWebSize.y);
    rebuildTexture(mWebSize.x, mWebSize.y);

    mLastUpdateTime = Platform::getVirtualMilliseconds();

    return true;
}

void GuiWebViewCtrl::onRemove()
{
    // Destroy the web render
	if (QtManager::singleton())
	{
		QtManager::singleton()->DestroyWebRender(m_WebRenderId);
	}
    m_WebRenderId = -1;

    // Call parent.
    Parent::onRemove();
}

bool GuiWebViewCtrl::onWake()
{
    if (!Parent::onWake())
        return false;

    setActive(true);

    setURL(mURL);

    return true;
}

void GuiWebViewCtrl::onSleep()
{
    Parent::onSleep();
}

void GuiWebViewCtrl::inspectPostApply()
{
    Parent::inspectPostApply();

    setURL(mURL);
}

//-----------------------------------------------------------------------------

void GuiWebViewCtrl::resize(const Point2I &newPosition, const Point2I &newExtent)
{
    Parent::resize(newPosition, newExtent);

    S32 sizeX = mBounds.extent.x;
    S32 sizeY = mBounds.extent.y;

    // Resize the texture we render into
    setSize(sizeX, sizeY);
}

//-----------------------------------------------------------------------------

void GuiWebViewCtrl::setURL(const char *url)
{
    if (!dStrlen(url))
        return;

    bool isLocalFile = true;

    if (dStrstr(url, "http://") || dStrstr(url, "https://"))
    {
        isLocalFile = false;
        mURL = StringTable->insert(url, true);
    }
    else if(dStrstr(url, "www."))
    {
        char buf[1024];
        dSprintf(buf, sizeof(buf), "http://%s", url);
        isLocalFile = false;
        mURL = StringTable->insert(buf, true);
    }
    else
    {
        char buf1[128];
        Con::expandPath(buf1, sizeof(buf1), url);

        char buf2[512];
        dSprintf(buf2, sizeof(buf2), "file:///%s/%s", Platform::getCurrentDirectory(), buf1);
        mURL = StringTable->insert(buf2, true);
    }

    QtManager::singleton()->WebRenderSetUrl(m_WebRenderId, mURL);
}

//-----------------------------------------------------------------------------

void GuiWebViewCtrl::reloadAction()
{
    QtManager::singleton()->WebRenderReloadAction(m_WebRenderId);
}

void GuiWebViewCtrl::backAction()
{
    QtManager::singleton()->WebRenderBackAction(m_WebRenderId);
}

void GuiWebViewCtrl::forwardAction()
{
    QtManager::singleton()->WebRenderForwardAction(m_WebRenderId);
}

void GuiWebViewCtrl::stopAction()
{
    QtManager::singleton()->WebRenderStopAction(m_WebRenderId);
}

//-----------------------------------------------------------------------------

void GuiWebViewCtrl::setFirstResponder()
{
    Parent::setFirstResponder();
    Platform::enableKeyboardTranslation();
}

void GuiWebViewCtrl::onLoseFirstResponder()
{
    Platform::disableKeyboardTranslation();
    Parent::onLoseFirstResponder();
}

//-----------------------------------------------------------------------------

void GuiWebViewCtrl::onMouseDown(const GuiEvent &event)
{
    QtManager::singleton()->WebRenderMouseDown(m_WebRenderId, windowPosToTexturePos( event.mousePoint.x, event.mousePoint.y));
}

bool GuiWebViewCtrl::onMouseWheelUp(const GuiEvent &event)
{
    if (!mAwake || !mVisible)
        return false;

    QtManager::singleton()->WebRenderMouseWheelUp(m_WebRenderId, windowPosToTexturePos( event.mousePoint.x, event.mousePoint.y));

    GuiControl* parent = getParent();
    if (parent)
        return parent->onMouseWheelUp(event);

    return true;
}

bool GuiWebViewCtrl::onMouseWheelDown(const GuiEvent &event)
{
    if (!mAwake || !mVisible)
        return false;

    QtManager::singleton()->WebRenderMouseWheelDown(m_WebRenderId, windowPosToTexturePos( event.mousePoint.x, event.mousePoint.y));

    GuiControl* parent = getParent();
    if (parent)
        return parent->onMouseWheelDown(event);

    return true;
}
void GuiWebViewCtrl::onMouseDragged(const GuiEvent &event)
{
    QtManager::singleton()->WebRenderMouseDragged(m_WebRenderId, windowPosToTexturePos( event.mousePoint.x, event.mousePoint.y));
}

void GuiWebViewCtrl::onMouseUp(const GuiEvent &event)
{
    QtManager::singleton()->WebRenderMouseUp(m_WebRenderId, windowPosToTexturePos( event.mousePoint.x, event.mousePoint.y));
}

void GuiWebViewCtrl::onMouseMove(const GuiEvent &event)
{
    if (!mVisible || !mAwake)
        return;

    QtManager::singleton()->WebRenderMouseMove(m_WebRenderId, windowPosToTexturePos( event.mousePoint.x, event.mousePoint.y));

    GuiControl *parent = getParent();
    if(parent)
        parent->onMouseMove(event);
}

bool GuiWebViewCtrl::onKeyDown(const GuiEvent &event)
{
    if (mVisible && mAwake)
    {
        QtManager::singleton()->WebRenderKeyboardInput(m_WebRenderId, event.keyCode, event.ascii);
    }


    return Parent::onKeyDown(event);
}

//-----------------------------------------------------------------------------

void GuiWebViewCtrl::onRender(Point2I offset, const RectI &updateRect)
{
    if(!mAwake || !mVisible)
        return;

    if(m_WebRenderId > -1)
    {
        F32 x0 = offset.x;
        F32 x1 = offset.x + mBounds.extent.x;
        F32 y0 = offset.y;
        F32 y1 = offset.y + mBounds.extent.y;

        glEnable(GL_TEXTURE_2D);
        glColor3f(1.0f, 1.0f, 1.0f);
        F32 currentTime = Platform::getVirtualMilliseconds(); 
        if((currentTime - mLastUpdateTime) > GUIWEBVIEWCTRL_UPDATETICK && mReceivedLastImage)
        {
            mLastUpdateTime = currentTime;
            mReceivedLastImage = false;

            //QSize size(mWebSize.x, mWebSize.y);
            //QImage webImage(size, QImage::Format_RGB32);
            //mQtWebView->render(&webImage);

            //glBindTexture(GL_TEXTURE_2D, mWebViewTexture);
            //glTexSubImage2D(GL_TEXTURE_2D, 0, 0, 0, mWebSize.x, mWebSize.y, GL_BGRA_EXT, GL_UNSIGNED_BYTE, webImage.bits());

            // Notify the WebRender that we're ready for the next rendered image
            QtManager::singleton()->WebRenderUpdateView(m_WebRenderId, false);
        };

        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
        glBindTexture(GL_TEXTURE_2D, mWebViewTexture);

        F32 u = mWebSize.x/mTextureSize.x;
        F32 v = mWebSize.y/mTextureSize.y;

        glBegin(GL_QUADS);
            glTexCoord2f(0.0f, 0.0f); glVertex3f(x0, y0,  0.0f);
            //glTexCoord2f(1.0f, 0.0f); glVertex3f(x1, y0,  0.0f);
            glTexCoord2f(u,    0.0f); glVertex3f(x1, y0,  0.0f);
            //glTexCoord2f(1.0f, 1.0f); glVertex3f(x1, y1,  0.0f);
            glTexCoord2f(u,    v);    glVertex3f(x1, y1,  0.0f);
            //glTexCoord2f(0.0f, 1.0f); glVertex3f(x0, y1,  0.0f);
            glTexCoord2f(0.0f, v);    glVertex3f(x0, y1,  0.0f);
        glEnd();

        glDisable(GL_TEXTURE_2D);
    }

    renderChildControls(offset, updateRect);
}

//-----------------------------------------------------------------------------

void GuiWebViewCtrl::onLoadStarted()
{
    Con::executef(this , 1, "onLoadStarted");
}

void GuiWebViewCtrl::onFinishedLoading(bool state)
{
    Con::executef(this , 2, "onFinishLoading", Con::getBoolArg(state));
}

void GuiWebViewCtrl::onConnectionError(const char* error)
{
    Con::executef(this , 2, "onConnectionError", error);
}

void GuiWebViewCtrl::onLinkClicked(const char* url)
{
    if (isMethod("onLinkClicked"))
    {
        // If the method exists then check if we should go to the URL
        const char* result = Con::executef(this , 2, "onLinkClicked", url);
        if( dAtob( result ) == true )
        {
            setURL(url);
        }
    }
    else
    {
        // Without the callback, automatically go to the URL
        setURL(url);
    }
}

void GuiWebViewCtrl::onUpdateView(bool newImage, S32 imageWidth, S32 imageHeight, U8* bits)
{
    if(newImage)
    {
        // Do we need to resize the texture to match the data sent?
        if(mTextureSize.x != imageWidth || mTextureSize.y != imageHeight)
        {
            rebuildTexture(imageWidth, imageHeight);
        }

        // Copy the rendered web image into the texture
        glEnable(GL_TEXTURE_2D);
        glColor3f(1.0f, 1.0f, 1.0f);
        glBindTexture(GL_TEXTURE_2D, mWebViewTexture);
        glTexSubImage2D(GL_TEXTURE_2D, 0, 0, 0, imageWidth, imageHeight, GL_BGRA_EXT, GL_UNSIGNED_BYTE, bits);
        glDisable(GL_TEXTURE_2D);
    }

    mReceivedLastImage = true;
}

void GuiWebViewCtrl::onURLChanged(const char* url)
{
    mURL = StringTable->insert(url, true);
    Con::executef(this , 2, "onURLChanged", url);
}

//-----------------------------------------------------------------------------

ConsoleMethod(GuiWebViewCtrl, setURL, void, 3, 3, "setURL(\"http://www.3stepstudio.com\");")
{
    object->setURL(argv[2]);
}

ConsoleMethod(GuiWebViewCtrl, reloadAction, void, 2, 2, "() - Reload the current URL")
{
    object->reloadAction();
}

ConsoleMethod(GuiWebViewCtrl, backAction, void, 2, 2, "() - Move back in the page history")
{
    object->backAction();
}

ConsoleMethod(GuiWebViewCtrl, forwardAction, void, 2, 2, "() - Move forward in the page history")
{
    object->forwardAction();
}

ConsoleMethod(GuiWebViewCtrl, stopAction, void, 2, 2, "() - Stop the web page loading")
{
    object->stopAction();
}
