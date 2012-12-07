//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------
#ifndef _WIN32DIRECTORYRESOLVER_H_
#define _WIN32DIRECTORYRESOLVER_H_

/// Utility class to resolve a path name and check if it is a directory.
/// If the path points to a short cut it will resolve the real path to
/// the short cut target and check if that is a directory. This allows 
/// listing of short cuts to directories in the file dialogs.
class Win32DirectoryResolver
{
   private:
      IShellLink*    mPSL; /// We cache the IShellLink interface pointer
      IPersistFile*  mPPF; /// We cache the IPersistFile interface pointer 
   
   public:
      /// Constructor
      Win32DirectoryResolver();
      
      /// Destructor
      virtual ~Win32DirectoryResolver();

      /// Check if a file path points to a directory or if it is a shortcut
      /// resolve the short cut path and see if that points to a directory.
      bool isDirectory( LPSTR strPathName ) const;
};

#endif