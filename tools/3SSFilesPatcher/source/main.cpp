//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include <QApplication>
#include <QDesktopWidget>

#include "mainWindow.h"

int main(int argc, char* argv[])
{
    //Q_INIT_RESOURCE(application);

    QApplication app(argc, argv);
    app.setOrganizationName("GarageGames LLC");
    app.setApplicationName("3SS Files Patcher");

    MainWindow mainWin;

    mainWin.show();

    // Center the window
    QRect r = mainWin.geometry();
    r.moveCenter(QApplication::desktop() ->availableGeometry().center());
    mainWin.setGeometry(r);

    return app.exec();
}
