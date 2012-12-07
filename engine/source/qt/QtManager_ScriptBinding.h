//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include <QApplication>
#include <QProcess>

ConsoleMethod(QtManager, LaunchModuleApp, bool, 4, 4,   "(moduleId, app) - Launch the requested application located in the given module.\n"
                                                        "@moduleId The module Id.\n"
                                                        "@app The application to launch.\n"
                                                        "@return True if the application was launched.\n")
{
    char buffer[1024];
    dSprintf(buffer, 1024, "^%s/%s", argv[2], argv[3]);

    char fullPath[1024];
    Con::expandPath(fullPath, 1024, buffer);

    dSprintf(buffer, 1024, "^%s", argv[2]);

    char modulePath[1024];
    Con::expandPath(modulePath, 1024, buffer);

    // Add this application's PID as an argument
    QString pid;
    pid.setNum(QApplication::applicationPid());

    QStringList arguments;
    arguments.append("-3SSPID");
    arguments.append(pid);

    bool result = QProcess::startDetached(fullPath, arguments, modulePath);

    return result;
}

//-----------------------------------------------------------------------------

ConsoleMethod(QtManager, saveCookies, bool, 3, 3,   "(path) - Save network cookies to the given path.\n"
                                                    "@path The path to save the cookies to.\n"
                                                    "@return True if the save was successful.\n")
{
    return object->saveCookies(argv[2]);
}

//-----------------------------------------------------------------------------

ConsoleMethod(QtManager, loadCookies, bool, 3, 3,   "(path) - Load network cookies from the given path.\n"
                                                    "@path The path to load the cookies from.\n"
                                                    "@return True if the load was successful.\n")
{
    return object->loadCookies(argv[2]);
}

//-----------------------------------------------------------------------------

ConsoleMethod(QtManager, clearCookies, void, 2, 2,  "() - Clear all network cookies.\n")
{
    object->clearCookies();
}
