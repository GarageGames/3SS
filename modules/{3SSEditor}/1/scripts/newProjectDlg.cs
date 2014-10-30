//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------
$NewProject::ProjectMessageString = "Enter a name...";

function showNewProjectDialog(%name)
{
    $SelectedTemplate = %name; 
    Canvas.pushDialog(NewProjectDlg);
}

function TGBWorkspace::getGamesFolder( %this )
{
   if( $Pref::UserGamesFolder $= "" )
      return getUserHomeDirectory() @ "/MyProjects";
   
   return $Pref::UserGamesFolder;
}

function NewProjectDlg::onWake( %this )
{      
    %moduleID = $SelectedTemplate;
    %templateModule = ModuleDatabase.getDefinitionFromId(%moduleID);
    %icon = %templateModule.icon;
    %normalIcon = "{3SSHomeScreens}:" @ %icon @ "_normal";
    NPPreviewImage.Image = %normalIcon;
    NPNameEdit.initialize($NewProject::ProjectMessageString);
    NPCreateButton.update(); 
}

function OpenProjectButton::onClick( %this )
{
   %selectedIndex = projectList.getSelectedItem();
   %projectName   = projectList.getItemText( %selectedIndex );
   
   if ( %projectName $= "" || %selectedIndex == -1 )
      return;
     
   LBProjectObj.open( %projectName ); 
   
   Canvas.popDialog(OpenProjectDlg);   
}

function projectList::onDoubleClick( %this )
{ 
   OpenProjectButton.performClick();
}

function NPNameEdit::onKeyPressed(%this)
{
    // Disallow symbols and spaces
    %name = stripChars(%this.getValue(), "-+*/%$&§=()[].?\"#,;!~<>|°^{} ");
    %this.setText(%name);
    
    %this.update();
    NPCreateButton.update(); 
} 

function NPNameEdit::onReturn(%this)
{
    NPCreateButton.setStateOn(true);
    createNewProject(NPNameEdit.getText(), $ProjectFilesLocation, $SelectedTemplate, false, false);
}

function NPCreateButton::update(%this)
{
    %active = true;    

    // Don't allow saving if the name is blank
    if (NPNameEdit.isEmpty()) 
        %active = false;

    %this.setActive(%active);
}