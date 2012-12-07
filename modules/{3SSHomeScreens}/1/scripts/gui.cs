//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function TemplateSelector::OpenProject(%projectPath)
{
   $Pref::Dialogs::LastProjectPath = filePath( %projectPath );
   
   createProjectResPaths(%projectPath);
   Projects::GetEventManager().postEvent( "_ProjectOpen", %projectPath );
}

function openWebSite()
{
   gotoWebpage("http://www.3stepstudio.com");
}

function openCommunityPage()
{
   gotoWebpage("http://boards.3stepstudio.com");
}

function openDocumentation()
{
   gotoWebpage("http://docs.3stepstudio.com");
}

function openMarketplace()
{
   gotoWebpage("http://shop.3stepstudio.com");
}