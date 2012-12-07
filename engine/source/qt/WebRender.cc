//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "qt/WebRender.h"
#include "qt/WebRenderEvents.h"
#include "qt/QtManager.h"

#include <QtNetwork/QNetworkReply>

WebRender::WebRender()
{
    // Set the network access manager right from the start.  It cannot
    // be changed once it has been used.
    page()->setNetworkAccessManager(&(QtManager::singleton()->getNetworkManager()));

    m_ViewSize.set(512, 512);

    for(U32 i=0; i<2; ++i)
    {
        m_WebImage[i] = QImage(m_ViewSize.x, m_ViewSize.y, QImage::Format_RGB32);
        m_WebImageFilled[i] = false;
    }
    m_CurrentWebImage = 0;
    m_CurrentWebImageDirty = true;

    // Pass along the size to QWebView
    resize(m_ViewSize.x, m_ViewSize.y);

    m_OwnerId = -1;

    m_URL = StringTable->insert("");

    // Set up for QWebView
    settings()->setAttribute(QWebSettings::DnsPrefetchEnabled, true);
    settings()->setAttribute(QWebSettings::JavascriptEnabled, true);
    settings()->setAttribute(QWebSettings::PluginsEnabled, true);
    page()->setLinkDelegationPolicy(QWebPage::DelegateAllLinks);

    // Qt callbacks
    connect(this, SIGNAL(loadStarted()),            this, SLOT(loadStartedSlot()));
    connect(this, SIGNAL(loadFinished(bool)),       this, SLOT(finishLoadingSlot(bool)));
    connect(this, SIGNAL(linkClicked(const QUrl&)), this, SLOT(linkClickedSlot(const QUrl&)));
    connect(this, SIGNAL(urlChanged(const QUrl&)), this, SLOT(urlChangedSlot(const QUrl&)));
    connect(page()->networkAccessManager(), SIGNAL(finished(QNetworkReply*)), this, SLOT(finishedSlot(QNetworkReply*)));
    connect(page(), SIGNAL(repaintRequested(const QRect &)), this, SLOT(repaintRequestedSlot(const QRect &)));
}

WebRender::~WebRender()
{
}

//-----------------------------------------------------------------------------

void WebRender::SetURL(const char* url)
{
    if (!dStrlen(url))
        return;

    bool isLocalFile = true;

    if (dStrstr(url, "http://") || dStrstr(url, "https://"))
    {
        isLocalFile = false;
        m_URL = StringTable->insert(url, true);
    }
    else if(dStrstr(url, "www."))
    {
        char buf[1024];
        dSprintf(buf, sizeof(buf), "http://%s", url);
        isLocalFile = false;
        m_URL = StringTable->insert(buf, true);
    }
    else
    {
        char buf1[128];
        Con::expandPath(buf1, sizeof(buf1), url);

        char buf2[512];
        dSprintf(buf2, sizeof(buf2), "file:///%s/%s", Platform::getCurrentDirectory(), buf1);
        m_URL = StringTable->insert(buf2, true);
    }

    load(QUrl(m_URL));
}

//-----------------------------------------------------------------------------

void WebRender::ReloadAction()
{
    page()->triggerAction(QWebPage::Reload);
}

void WebRender::BackAction()
{
    page()->triggerAction(QWebPage::Back);
}

void WebRender::ForwardAction()
{
    page()->triggerAction(QWebPage::Forward);
}

void WebRender::StopAction()
{
    page()->triggerAction(QWebPage::Stop);
}

//-----------------------------------------------------------------------------

void WebRender::SetSize(S32 width , S32 height)
{
    if(width == m_ViewSize.x && height == m_ViewSize.y)
        return;

    m_ViewSize.x = width;
    m_ViewSize.y = height;

    //m_WebImageMutex.lock();
    //m_WebImage[m_CurrentWebImage] = QImage(m_ViewSize.x, m_ViewSize.y, QImage::Format_RGB32);
    //m_WebImageFilled[m_CurrentWebImage] = false;
    m_CurrentWebImageDirty = true;
    //m_WebImageMutex.unlock();

    // Pass along the new size to the web view
    resize(width, height);
}

//-----------------------------------------------------------------------------

void WebRender::UpdateView(bool force)
{
    if(m_CurrentWebImageDirty || force)
    {
        // Do we need to resize the current image?
        if(m_WebImage[m_CurrentWebImage].width() != m_ViewSize.x || m_WebImage[m_CurrentWebImage].height() != m_ViewSize.y)
        {
            m_WebImage[m_CurrentWebImage] = QImage(m_ViewSize.x, m_ViewSize.y, QImage::Format_RGB32);
        }

        // Update the dirty image
        RenderView();
        m_CurrentWebImageDirty = false;

        WebRenderCallbackEvent::CallbackData* data = new WebRenderCallbackEvent::CallbackData();

        // Indicate that we're passing along image data
        data->m_Result = true;

        // Store the current size of the image
        data->m_S32Args[0] = m_ViewSize.x;
        data->m_S32Args[1] = m_ViewSize.y;

        // Store a pointer to the buffered image data
        data->m_pU8Pointer = m_WebImage[m_CurrentWebImage].bits();

        Sim::postCurrentEvent(m_OwnerId, new WebRenderCallbackEvent(WebRenderCallbackEvent::CALLBACK_VIEW_UPDATE, data));

        // Swap image buffers
        m_CurrentWebImage = 1 - m_CurrentWebImage;
    }
    else
    {
        WebRenderCallbackEvent::CallbackData* data = new WebRenderCallbackEvent::CallbackData();

        // Indicate that there is no update
        data->m_Result = false;

        Sim::postCurrentEvent(m_OwnerId, new WebRenderCallbackEvent(WebRenderCallbackEvent::CALLBACK_VIEW_UPDATE, data));
    }
}

//-----------------------------------------------------------------------------

void WebRender::RenderView()
{
    //m_WebImageMutex.lock();

    render(&m_WebImage[m_CurrentWebImage]);
    m_WebImageFilled[m_CurrentWebImage] = true;

    //m_WebImageMutex.unlock();
}

//-----------------------------------------------------------------------------

void WebRender::loadStartedSlot()
{
    Sim::postCurrentEvent(m_OwnerId, new WebRenderCallbackEvent(WebRenderCallbackEvent::CALLBACK_LOAD_STARTED));
}

void WebRender::finishLoadingSlot(bool ok)
{
    WebRenderCallbackEvent::CallbackData* data = new WebRenderCallbackEvent::CallbackData();

    // Copy the result into the callback data
    data->m_Result = ok;

    Sim::postCurrentEvent(m_OwnerId, new WebRenderCallbackEvent(WebRenderCallbackEvent::CALLBACK_FINISH_LOADING, data));
}

void WebRender::linkClickedSlot(const QUrl& url)
{
    WebRenderCallbackEvent::CallbackData* data = new WebRenderCallbackEvent::CallbackData();

    // Copy the URL into the callback data
    QString str = url.toString(); 
    QByteArray ba = str.toAscii();
    char* text = new char[ba.size()+1];
    dMemcpy(text, ba, ba.size());
    text[ba.size()] = '\0';
    data->m_URL = text;

    Sim::postCurrentEvent(m_OwnerId, new WebRenderCallbackEvent(WebRenderCallbackEvent::CALLBACK_LINK_CLICKED, data));
}

void WebRender::finishedSlot(QNetworkReply* reply)
{
    if(reply->error() != QNetworkReply::NoError)
    {
        WebRenderCallbackEvent::CallbackData* data = new WebRenderCallbackEvent::CallbackData();

        // Copy the error string into the callback data
        QByteArray ba = reply->errorString().toLatin1();
        char* text = new char[ba.size()+1];
        dMemcpy(text, ba, ba.size());
        text[ba.size()] = '\0';
        data->m_StringData = text;

        Sim::postCurrentEvent(m_OwnerId, new WebRenderCallbackEvent(WebRenderCallbackEvent::CALLBACK_CONNECTION_ERROR, data));
    }
}

void WebRender::repaintRequestedSlot(const QRect & dirtyRect)
{
    //Con::printf("repaintRequestedSlot: %d %d %d %d", dirtyRect.x(), dirtyRect.y(), dirtyRect.width(), dirtyRect.height());
    m_CurrentWebImageDirty = true;
}

void WebRender::urlChangedSlot(const QUrl& url)
{
    WebRenderCallbackEvent::CallbackData* data = new WebRenderCallbackEvent::CallbackData();

    // Copy the URL into the callback data
    QString str = url.toString(); 
    QByteArray ba = str.toAscii();
    char* text = new char[ba.size()+1];
    dMemcpy(text, ba, ba.size());
    text[ba.size()] = '\0';
    data->m_URL = text;

    Sim::postCurrentEvent(m_OwnerId, new WebRenderCallbackEvent(WebRenderCallbackEvent::CALLBACK_URL_CHANGED, data));
}
