//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _MAINWINDOW_H
#define _MAINWINDOW_H

#include <QMainWindow>
#include <QWidget>

#include "manifestWorker.h"

class QLabel;
class QProgressBar;

class MainWindow : public QWidget
{
    Q_OBJECT

public:
    enum MainWindowStates {
        MW_START_UP,
        MW_WAIT_FOR_3SS_TO_CLOSE,
        MW_MANIFEST_LOAD,
        MW_MANIFEST_READ,
        MW_MANIFEST_PROCESS,
        MW_MANIFEST_COMPLETE,
        MW_MANIFEST_RESTART_APP,
        MW_MANIFEST_QUIT,
        MW_MANIFEST_ERROR,
    };

public:
    MainWindow();

protected:
    // PID of 3SS as passed to us from 3SS
    int m_3SSPID;

    // Tracks current state
    MainWindowStates    m_CurrentState;

    // GUI
    QLabel*         m_Label;
    QProgressBar*   m_Progress;

    ManifestWorker  m_ManifestWorker;

    int             m_TotalBytesToCopy;

protected:
    // The event loop
    virtual void timerEvent(QTimerEvent* event);
};

#endif  // _MAINWINDOW_H
