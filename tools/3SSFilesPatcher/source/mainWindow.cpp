//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include <QtGui>
#include <QApplication>
#include <QMessageBox>

#include "mainWindow.h"

#include "util.h"

QString gsLastErrorMessage;

MainWindow::MainWindow()
{
    setWindowFlags(Qt::Tool | Qt::WindowStaysOnTopHint | Qt::CustomizeWindowHint | Qt::WindowTitleHint | Qt::FramelessWindowHint);

    m_Label = new QLabel;
    m_Label->setText(tr(""));

    m_Progress = new QProgressBar;
    m_Progress->setMinimumWidth(500);

    QVBoxLayout *layout = new QVBoxLayout;
    layout->addWidget(m_Label);
    layout->addWidget(m_Progress);
    setLayout(layout);

    // Set the current state
    m_CurrentState = MW_START_UP;

    m_TotalBytesToCopy = 0;

    // Get the 3SS PID as passed along in our arguments
    m_3SSPID = 0;
    QStringList args = QCoreApplication::arguments();
    for(int i=0; i<args.size(); ++i)
    {
        if(QString::compare(args.value(i), "-3SSPID", Qt::CaseInsensitive))
        {
            int pidIndex = i + 1;
            if(i<args.size())
            {
                m_3SSPID = args.value(pidIndex).toInt();
            }
        }
    }

    // Start the event loop
    startTimer(CHECK_3SS_INTERVAL);
}

void MainWindow::timerEvent(QTimerEvent* event)
{
    switch(m_CurrentState)
    {
        case MW_START_UP:
            m_Label->setText(tr("Starting up..."));

            // Check if we need to wait for 3SS to stop
            if(m_3SSPID != 0)
            {
                // We need to wait
                m_CurrentState = MW_WAIT_FOR_3SS_TO_CLOSE;
            }
            else
            {
                // No need to wait, so move along to loading the manifest
                m_CurrentState = MW_MANIFEST_LOAD;
                killTimer(event->timerId());
                startTimer(EVENT_LOOP_INTERVAL);
            }
            break;

        case MW_WAIT_FOR_3SS_TO_CLOSE:
            if(Platform::Has3SSClosed(m_3SSPID))
            {
                // Reset timer to new interval and move on to the next step
                m_CurrentState = MW_MANIFEST_LOAD;
                killTimer(event->timerId());
                startTimer(EVENT_LOOP_INTERVAL);
            }
            break;

        case MW_MANIFEST_LOAD:
            {
                m_Label->setText(tr("Loading manifest..."));

                bool result = m_ManifestWorker.LoadManifestFile();
                if(!result)
                {
                    // TODO: Error message

                    m_CurrentState = MW_MANIFEST_ERROR;
                    break;
                }

                // On to the next state
                m_CurrentState = MW_MANIFEST_READ;
            }
            break;

        case MW_MANIFEST_READ:
            {
                bool result = m_ManifestWorker.ReadManifestFile(m_TotalBytesToCopy);
                if(!result)
                {
                    // TODO: Error message

                    m_CurrentState = MW_MANIFEST_ERROR;
                    break;
                }

                // Set up the progress bar
                m_Progress->setMinimum(0);
                m_Progress->setMaximum(m_TotalBytesToCopy);

                // On to the next state
                m_CurrentState = MW_MANIFEST_PROCESS;
            }
            break;

        case MW_MANIFEST_PROCESS:
            {
                m_Label->setText(tr("Copying files..."));

                // Allow the file copy to continue
                int bytesCopied = 0;
                bool result = m_ManifestWorker.ProcessManifestTick(bytesCopied);
                if(!result)
                {
                    // TODO: Error message

                    m_CurrentState = MW_MANIFEST_ERROR;
                    break;
                }

                // Update progress bar
                m_Progress->setValue(bytesCopied);

                if(bytesCopied >= m_TotalBytesToCopy)
                {
                    // Move on to the next state
                    m_CurrentState = MW_MANIFEST_COMPLETE;
                }
            }
            break;

        case MW_MANIFEST_COMPLETE:
            m_Label->setText(tr("Done. Restarting 3 Step Studio."));

            // Move on to restarting the application
            m_CurrentState = MW_MANIFEST_RESTART_APP;

            break;

        case MW_MANIFEST_RESTART_APP:
            {
                // Restart the app
                bool result = m_ManifestWorker.RestartApp();
                if(!result)
                {
                    // TODO: Error message

                    m_CurrentState = MW_MANIFEST_ERROR;
                    break;
                }

                // Quit this program
                m_CurrentState = MW_MANIFEST_QUIT;
                killTimer(event->timerId());
                startTimer(TIME_TO_CLOSE);
            }
            break;

        case MW_MANIFEST_QUIT:
            QApplication::quit();
            break;

        case MW_MANIFEST_ERROR:
            m_Label->setText(tr("Error processing manifest."));

            // Display error message
            if(!gsLastErrorMessage.isEmpty())
            {
                QMessageBox::critical(this, "Error updating files", gsLastErrorMessage);
            }

            QApplication::quit();

            break;
    }
}
