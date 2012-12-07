//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

// open create save import export
function TGBWorkspace::createProject( %this )
{
   newProject();
}

function TGBWorkspace::openProject( %this )
{
   %dlg = new OpenFileDialog()
   {
      Title = "Open Project";
      DefaultPath = $Pref::Dialogs::LastProjectPath;
      Filters = "T2D Projects (*.tssproj)|*.tssproj|All Files (*.*)|*.*";
      MustExist = true;
      MultipleFiles = false;
      ChangePath = false;
   };
   
   if(%dlg.Execute())
   {
      $Pref::Dialogs::LastProjectPath = filePath( %dlg.FileName );
      if(! LBProjectObj.isActive())
      {
         createProjectResPaths(%dlg.FileName);
         Projects::GetEventManager().postEvent( "_ProjectOpen", %dlg.FileName );
      }
      else 
      {
         $pref::startupProject = %dlg.FileName;
         $pref::lastProject = %dlg.FileName;
         $reloadWithStartup = true;
         reloadProject();
      }
   }

}

function TGBWorkspace::importProject( %this )
{ 
   import11XGames();
}

function TGBWorkspace::saveProject( %this )
{
}

if( !isObject(TGBWorkspace) )
   new ScriptMsgListener(TGBWorkspace);
   
function TGBStartPage::onWake( %this )
{
   tgbBetaText.text = "3 Step Studio " @ getThreeStepStudioVersion();   
}
