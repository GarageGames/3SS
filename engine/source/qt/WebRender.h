//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _WEBRENDER_H
#define _WEBRENDER_H

#include "sim/simBase.h"
#include "math/mPoint.h"
#include "platform/threads/mutex.h"

#include <QtWebKit/QWebView>

class WebRender : public QWebView
{
    Q_OBJECT

public slots:
    void loadStartedSlot();
    void linkClickedSlot(const QUrl& url);
    void finishLoadingSlot(bool ok);
    void finishedSlot(QNetworkReply* reply);
    void repaintRequestedSlot(const QRect & dirtyRect);
    void urlChangedSlot(const QUrl& url);

public:
    WebRender();
    virtual ~WebRender();

    S32 GetId() { return m_Id; }
    void SetId(S32 id) { m_Id = id; }

    void SetOwnerId(S32 id) { m_OwnerId = id; }

    const char* GetURL() { return m_URL; }
    void SetURL(const char* url);

    void ReloadAction();
    void BackAction();
    void ForwardAction();
    void StopAction();

    void SetSize(S32 width , S32 height);

    void UpdateView(bool force);

    void RenderView();

protected:
    S32 m_Id;

    // The web view control we will issue callbacks on.
    SimObjectId m_OwnerId;

    StringTableEntry m_URL;

    Point2I m_ViewSize;

    S32 m_CurrentWebImage;
    bool m_CurrentWebImageDirty;
    QImage m_WebImage[2];
    bool m_WebImageFilled[2];
    Mutex m_WebImageMutex;

};

#endif // _WEBRENDER_H
