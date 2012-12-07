//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function TGBWorkspace::getGamesFolder( %this )
{
   if( $Pref::UserGamesFolder $= "" )
      return getUserHomeDirectory() @ "/MyProjects";
   
   return $Pref::UserGamesFolder;
}


function NewProjectDlg::onWake( %this )
{  
    if ($templateCount <= 0)
        parseTemplates();
        
    templateListBox.clear();
    
    for(%i = 0; %i < $templateCount; %i++)
    {
        %template = $templateList[%i];
        templateListBox.add(%template.Description, %i);
    }
    
    templateListBox.setSelected(0);
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