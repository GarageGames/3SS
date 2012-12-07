//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#import <Cocoa/Cocoa.h>
#import <Carbon/Carbon.h>
#include <unistd.h>
#include "platform/platform.h"
#include "console/console.h"
#include "string/stringTable.h"
#include "platform/platformInput.h"
#include "platformMacCarb/cocoaUtils.h"
#include "platform/threads/thread.h"

#pragma mark ---- Various Directories ----
//-----------------------------------------------------------------------------
// 1.4.1 - Fixed to use the right method, expanded to create paths when needed.
// Special things to Ronny "orb" Bangsund
const char* Platform::getUserDataDirectory() 
{
	// orb: Getting the path of the Application Support directory is only the first step.
	NSArray *paths = NSSearchPathForDirectoriesInDomains(NSApplicationSupportDirectory, NSUserDomainMask, YES);
	if([paths count] == 0)
	{
		// orb: This is a catastrophic failure - the system doesn't know where ~/Library/Application Support is!
		return NULL;
	}
	NSString *fullPath = [paths objectAtIndex:0];

	BOOL exists;
	BOOL isDir;
	NSFileManager *fm = [NSFileManager defaultManager];
	exists = [fm fileExistsAtPath:fullPath isDirectory:&isDir];
	if(!exists || !isDir)
	{
		// orb: This is probably an extremely rare occurence, but I have seen a few disk checks
		// converting directories into files before.
		if(exists)
		{
			[fm removeItemAtPath:fullPath error:nil];
		}
		BOOL success = [fm createDirectoryAtPath:fullPath withIntermediateDirectories:YES
					   attributes:nil error:nil];
		if(!success) return NULL;
	}
	// The directory exists and can be returned.
   return StringTable->insert([fullPath UTF8String]);
}

//-----------------------------------------------------------------------------
const char* Platform::getUserHomeDirectory() 
{
   return StringTable->insert([[@"~/Documents" stringByStandardizingPath] UTF8String]);
}

//-----------------------------------------------------------------------------
StringTableEntry Platform::osGetTemporaryDirectory()
{
   NSString *tdir = NSTemporaryDirectory();
   const char *path = [tdir UTF8String];
   return StringTable->insert(path);
}

#pragma mark ---- Administrator ----
//-----------------------------------------------------------------------------
bool Platform::getUserIsAdministrator()
{
   // if we can write to /Library, we're probably an admin
   // HACK: this is not really very good, because people can chmod Library.
   return (access("/Library", W_OK) == 0);
}

#pragma mark ---- MessageBox ----
struct _NSStringMap
{
   S32 num;
   NSString* ok;
   NSString* cancel;
   NSString* third;
};

static _NSStringMap sgButtonTextMap[] =
{
   { MBOk,                 @"Ok",    nil,        nil },
   { MBOkCancel,           @"Ok",    @"Cancel",  nil },
   { MBRetryCancel,        @"Retry", @"Cancel",  nil },
   { MBSaveDontSave,       @"Save",  @"Discard", nil },
   { MBSaveDontSaveCancel, @"Save",  @"Cancel",  @"Don't Save" },
   { -1, nil, nil, nil }
};

struct _NSAlertResultMap
{
   S32 num;
   S32 ok;
   S32 cancel;
   S32 third;
};

static _NSAlertResultMap sgAlertResultMap[] = 
{
   { MBOk,                 MROk,    0,          0 },
   { MBOkCancel,           MROk,    MRCancel,   0 },
   { MBRetryCancel,        MRRetry, MRCancel,   0 },
   { MBSaveDontSave,       MROk,    MRDontSave, 0 },
   { MBSaveDontSaveCancel, MROk,    MRCancel,   MRDontSave },
   { -1, nil, nil, nil }
};

//-----------------------------------------------------------------------------
S32 Platform::messageBox(const UTF8 *title, const UTF8 *message, MBButtons buttons, MBIcons icon)
{
// TODO: paxorr: put this on the main thread

   // determine the button text
   NSString *okBtn      = nil;
   NSString *cancelBtn  = nil;
   NSString *thirdBtn   = nil;
   U32 i;
   for(i = 0; sgButtonTextMap[i].num != -1; i++)
   {
      if(sgButtonTextMap[i].num != buttons)
         continue;

      okBtn = sgButtonTextMap[i].ok;
      cancelBtn = sgButtonTextMap[i].cancel;
      thirdBtn = sgButtonTextMap[i].third;
      break;
   }
   if(sgButtonTextMap[i].num == -1)
      Con::errorf("Unknown message box button set requested. Mac Platform::messageBox() probably needs to be updated.");
   
   // convert title and message to NSStrings
   NSString *nsTitle = [NSString stringWithUTF8String:title];
   NSString *nsMessage = [NSString stringWithUTF8String:message];
   Input::setCursorShape(CursorManager::curIBeam);
   // show the alert
   S32 result = -2;
   switch(icon)
   {
   case MIWarning: 
   case MIStop:
      result = NSRunCriticalAlertPanel( nsTitle, nsMessage, okBtn, thirdBtn, cancelBtn);
      break;
      
   case MIInformation:
      result = NSRunInformationalAlertPanel( nsTitle, nsMessage, okBtn, thirdBtn, cancelBtn);
      break;
   
   case MIQuestion:
      result = NSRunAlertPanel( nsTitle, nsMessage, okBtn, thirdBtn, cancelBtn);
      break;
      
   default:
      Con::errorf("Unknown message box icon requested. Mac Platform::messageBox() probably needs to be updated.");
      result = NSRunAlertPanel( nsTitle, nsMessage, okBtn, thirdBtn, cancelBtn);
   };
   
   S32 ret = 0;
   for(U32 i = 0; sgAlertResultMap[i].num != -1; i++)
   {
      if(sgAlertResultMap[i].num != buttons)
         continue;
      
      switch(result)
      {
      case NSAlertDefaultReturn:
         ret = sgAlertResultMap[i].ok;     break;
      case NSAlertOtherReturn:
         ret = sgAlertResultMap[i].cancel; break;
      case NSAlertAlternateReturn:
         ret = sgAlertResultMap[i].third;  break;
      }
   }
   
   return ret;
      
// Here's a gem: how to get a cocoa window from your carbon window. Oh, Joy! Happy, Happy!
//   NSWindow *cocoaFromCarbonWin = NULL;
//   if(platState.appWindow)
//      cocoaFromCarbonWin = [[NSWindow alloc] initWithWindowRef:platState.appWindow];
}

#pragma mark ---- File IO ----
//-----------------------------------------------------------------------------
bool Platform::pathCopy(const char* source, const char* dest, bool nooverwrite)
{
   NSFileManager *manager = [NSFileManager defaultManager];
   NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
   
   NSString *nsource = [[NSString stringWithUTF8String:source] stringByStandardizingPath];
   NSString *ndest   = [[NSString stringWithUTF8String:dest] stringByStandardizingPath];
   NSString *ndestFolder = [ndest stringByDeletingLastPathComponent];
   
   if(! [manager fileExistsAtPath:nsource])
   {
      Con::errorf("Platform::pathCopy: no file exists at %s",source);
      return false;
   }
   
   if( [manager fileExistsAtPath:ndest] )
   {
      if(nooverwrite)
      {
         Con::errorf("Platform::pathCopy: file already exists at %s",dest);
         return false;
      }
      Con::warnf("Deleting files at path: %s", dest);
      bool deleted = [manager removeFileAtPath:ndest handler:nil];
      if(!deleted)
      {
         Con::errorf("Copy failed! Could not delete files at path: %s", dest);
         return false;
      }
   }
   
   if([manager fileExistsAtPath:ndestFolder] == NO)
   {
      ndestFolder = [ndestFolder stringByAppendingString:@"/"]; // createpath requires a trailing slash
      Platform::createPath([ndestFolder UTF8String]);
   }
   
   bool ret = [manager copyPath:nsource toPath:ndest handler:nil];
   
   [pool release];
   return ret;
   
}

//-----------------------------------------------------------------------------
bool Platform::fileRename(const char *source, const char *dest)
{
   if(source == NULL || dest == NULL)
      return false;
      
   NSFileManager *manager = [NSFileManager defaultManager];
   
   NSString *nsource = [manager stringWithFileSystemRepresentation:source length:dStrlen(source)];
   NSString *ndest   = [manager stringWithFileSystemRepresentation:dest length:dStrlen(dest)];
   
   if(! [manager fileExistsAtPath:nsource])
   {
      Con::errorf("Platform::fileRename: no file exists at %s",source);
      return false;
   }
   
   if( [manager fileExistsAtPath:ndest] )
   {
      Con::warnf("Platform::fileRename: Deleting files at path: %s", dest);
   }
   
   bool ret = [manager movePath:nsource toPath:ndest handler:nil];
  
   return ret;
}

#pragma mark ---- Cursors ----

// a repository of custom cursors.
@interface TorqueCursors : NSObject { }
+(NSCursor*)resizeAll;
+(NSCursor*)resizeNWSE;
+(NSCursor*)resizeNESW;
@end
@implementation TorqueCursors
+(NSCursor*)resizeAll
{
   static NSCursor* cur = nil;
   if(!cur)
      cur = [[NSCursor alloc] initWithImage:[NSImage imageNamed:@"resizeAll"] hotSpot:NSMakePoint(8, 8)];
   return cur;
}
+(NSCursor*)resizeNWSE
{
   static NSCursor* cur = nil;
   if(!cur)
      cur = [[NSCursor alloc] initWithImage:[NSImage imageNamed:@"resizeNWSE"] hotSpot:NSMakePoint(8, 8)];
   return cur;
}
+(NSCursor*)resizeNESW
{
   static NSCursor* cur = nil;
   if(!cur)
      cur = [[NSCursor alloc] initWithImage:[NSImage imageNamed:@"resizeNESW"] hotSpot:NSMakePoint(8, 8)];
   return cur;
}
@end

void Input::setCursorShape(U32 cursorID)
{
   NSCursor *cur;
   switch(cursorID)
   {
   case CursorManager::curArrow:
      [[NSCursor arrowCursor] set];
      break;
   case CursorManager::curWait:
      // hack: black-sheep carbon call
      SetThemeCursor(kThemeWatchCursor);
      break;
   case CursorManager::curPlus:
      [[NSCursor crosshairCursor] set];
      break;
   case CursorManager::curResizeVert:
      [[NSCursor resizeLeftRightCursor] set];
      break;
   case CursorManager::curIBeam:
      [[NSCursor IBeamCursor] set];
      break;
   case CursorManager::curResizeAll:
      cur = [TorqueCursors resizeAll];
      [cur set];
      break;
   case CursorManager::curResizeNESW:
      [[TorqueCursors resizeNESW] set];
      break;
   case CursorManager::curResizeNWSE:
      [[TorqueCursors resizeNWSE] set];
      break;
   case CursorManager::curResizeHorz:
      [[NSCursor resizeUpDownCursor] set];
      break;
   }
}

void Input::setCursorState(bool on)
{
   PlatStateMac::get().setHideCursor(!on);
}

#pragma mark -
#pragma mark ---- ShellExecute ----
class ExecuteThread : public Thread
{
   const char* zargs;
   const char* directory;
   const char* executable;
public:
   ExecuteThread(const char *_executable, const char *_args /* = NULL */, const char *_directory /* = NULL */) : Thread(0, NULL, false, true)
   {
      zargs = dStrdup(_args);
      directory = dStrdup(_directory);
      executable = dStrdup(_executable);
      start();
   }
   static U32 runNoThread(const char *_executable, const char *_args /* = NULL */, const char *_directory /* = NULL */);
   virtual void run(void* arg);
};

static char* _unDoubleQuote(char* arg)
{
   U32 len = dStrlen(arg);
   if(!len)
      return arg;
   
   if(arg[0] == '"' && arg[len-1] == '"')
   {
      arg[len - 1] = '\0';
      return arg + 1;
   }
   return arg;
}

// this is an externably callable blocking shellExecute!!!
U32 ExecuteThread::runNoThread( const char* executable, const char* zargs, const char* directory  )
{
	printf("creating nstask\n");
	NSTask *aTask = [[NSTask alloc] init];
	NSMutableArray *array = [NSMutableArray array];
	
	// scan the args list, breaking it up, space delimited, backslash escaped.
	U32 len = dStrlen(zargs);
	char args[len+1];
	dStrncpy(args, zargs, len+1);
	char *lastarg = args;
	bool escaping = false;
	for(int i = 0; i< len; i++)
	{
		char c = args[i];
		// a backslash escapes the next character
		if(escaping)      
			continue;
		if(c == '\\')
			escaping = true;
		
		if(c == ' ')
		{
			args[i] = '\0';
			if(*lastarg)
				[array addObject:[NSString stringWithUTF8String: _unDoubleQuote(lastarg)]];
			lastarg = args + i + 1;
		}
	}
	if(*lastarg)
		[array addObject:[NSString stringWithUTF8String: _unDoubleQuote(lastarg)]];
	
	[aTask setArguments: array];
	
	[aTask setCurrentDirectoryPath:[NSString stringWithUTF8String: directory]];
	[aTask setLaunchPath:[NSString stringWithUTF8String:executable]];
	[aTask launch];
	[aTask waitUntilExit];
	U32 ret = [aTask terminationStatus];
	return ret;
}

void ExecuteThread::run(void* arg)
{
	// call the common run, but since we're in a thread, this won't block other processes
	U32 ret = runNoThread( this->executable, this->zargs, this->directory );
	Con::executef(2, "onExecuteDone", Con::getIntArg(ret));
	printf("done nstask\n");
}

ConsoleFunction(shellExecute, bool, 2, 4, "(executable, [args], [directory])")
{
   ExecuteThread *et = new ExecuteThread(argv[1], argc > 2 ? argv[2] : NULL, argc > 3 ? argv[3] : NULL);
   return true; // Bug: BPNC error: need feedback on whether the command was sucessful
}

ConsoleFunction(shellExecuteBlocking, int, 2, 4, "(executable, [args], [directory])")
{
	return (int)ExecuteThread::runNoThread( argv[1], argc > 2 ? argv[2] : NULL, argc > 3 ? argv[3] : NULL );
}

StringTableEntry Platform::createUUID( void )
{
    CFUUIDRef ref = CFUUIDCreate(nil);
    NSString* uuid = (NSString *)CFUUIDCreateString(nil,ref);
    CFRelease(ref);
    
    StringTableEntry uuidString = StringTable->insert([uuid UTF8String]);
    [uuid release];
    return uuidString;
}
