//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "platformWin32/platformWin32.h"
#include "console/console.h"
#include "sim/simBase.h"
#include "string/unicode.h"
#include "platform/threads/thread.h"
#include "platform/threads/mutex.h"
#include "memory/safeDelete.h"

//////////////////////////////////////////////////////////////////////////
// Thread for executing in
//////////////////////////////////////////////////////////////////////////

class ExecuteThread : public Thread
{
   // [tom, 12/14/2006] mProcess is only used in the constructor before the thread
   // is started and in the thread itself so we should be OK without a mutex.
   HANDLE mProcess;

public:
   ExecuteThread(const char *executable, const char *args = NULL, const char *directory = NULL);

   virtual void run(void *arg = 0);
};

//////////////////////////////////////////////////////////////////////////
// Event for cleanup
//////////////////////////////////////////////////////////////////////////

class ExecuteCleanupEvent : public SimEvent
{
   ExecuteThread *mThread;
   bool mOK;

public:
   ExecuteCleanupEvent(ExecuteThread *thread, bool ok)
   {
      mThread = thread;
      mOK = ok;
   }

   virtual void process(SimObject *object)
   {
      Con::executef(2, "onExecuteDone", Con::getIntArg(mOK));
      SAFE_DELETE(mThread);
   }
};


//////////////////////////////////////////////////////////////////////////

ExecuteThread::ExecuteThread(const char *executable, const char *args /* = NULL */, const char *directory /* = NULL */) : Thread(0, NULL, false)
{
   //#pragma message("Implement UNICODE support for ExecuteThread [12/14/2006 tom]" )

   SHELLEXECUTEINFOA shl;
   dMemset(&shl, 0, sizeof(shl));

   shl.cbSize = sizeof(shl);
   shl.fMask = SEE_MASK_NOCLOSEPROCESS;
   
   char exeBuf[1024];
   Platform::makeFullPathName(executable, exeBuf, sizeof(exeBuf));
   
   shl.lpVerb = "open";
   shl.lpFile = exeBuf;
   shl.lpParameters = args;
   shl.lpDirectory = directory;

   shl.nShow = SW_SHOWNORMAL;

   if(ShellExecuteExA(&shl) && shl.hProcess)
   {
      mProcess = shl.hProcess;
      start();
   }
}

void ExecuteThread::run(void *arg /* = 0 */)
{
   if(mProcess == NULL)
      return;

   DWORD wait;
   while(! checkForStop() && (wait = WaitForSingleObject(mProcess, 200)) != WAIT_OBJECT_0) ;

   Sim::postEvent(Sim::getRootGroup(), new ExecuteCleanupEvent(this, wait == WAIT_OBJECT_0), -1);
}

//////////////////////////////////////////////////////////////////////////
// Console Functions
//////////////////////////////////////////////////////////////////////////

ConsoleFunction(shellExecute, bool, 2, 4, "(executable, [args], [directory]) Executes a process"
                "@param executable The program to execute\n"
                "@param args Arguments to pass to the executable\n"
                "@param directory The directory in which the program is located\n"
                "@return Returns true on success, false otherwise")
{
   ExecuteThread *et = new ExecuteThread(argv[1], argc > 2 ? argv[2] : NULL, argc > 3 ? argv[3] : NULL);
   if(! et->isAlive())
   {
      delete et;
      return false;
   }

   return true;
}

ConsoleFunction(shellExecuteBlocking, int, 2, 6, "(executable, [args], [directory])"
                "@param executable The program to execute\n"
                "@param args Arguments to pass to the executable\n"
                "@param directory The directory in which the program is located\n"
                "@return Returns true on success, false otherwise")
{
    const char* executable = argv[1];
    const char* args = argc > 2 ? argv[2] : NULL;
    const char* directory = argc > 3 ? argv[3] : NULL;

    SHELLEXECUTEINFOA shl;
    dMemset(&shl, 0, sizeof(shl));

    shl.cbSize = sizeof(shl);
    shl.fMask = SEE_MASK_NOCLOSEPROCESS;
   
    char exeBuf[1024];
    Platform::makeFullPathName(executable, exeBuf, sizeof(exeBuf));
   
    shl.lpVerb = "open";
    shl.lpFile = exeBuf;
    shl.lpParameters = args;
    shl.lpDirectory = directory;

    shl.nShow = SW_HIDE;

    ShellExecuteExA(&shl);

    if ( shl.hProcess == NULL )
        return false;

    return ( WaitForSingleObject( shl.hProcess, INFINITE) == WAIT_OBJECT_0 );
}

void Platform::openFolder(const char* path )
{
   char filePath[1024];
   Platform::makeFullPathName(path, filePath, sizeof(filePath));

   ::ShellExecuteA( NULL,"explore",filePath, NULL, NULL, SW_SHOWNORMAL);
}