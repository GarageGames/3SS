//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function showProjectOptionsDialog(%gameProject, %invokingGui)
{
    Canvas.pushDialog(ProjectOptionsGui);
    
    %name = %gameProject.projectName;
    
    if (%name !$= "")
    {
        PONameEdit.text = %name;
        ProjectOptionsGui.originalName = %name;
        ProjectOptionsGui.project = %gameProject;
    }
    
    if (isObject(%invokingGui))
        ProjectOptionsGui.invokingGui = %invokingGui;
}

function ProjectOptionsGui::onWake(%this)
{
    PONameEdit.text = "Enter a game name";
    PODoneText.profile = "Inactive";
    POOkButton.setActive(false);   
    %this.nameUpdated = false;
    %this.invokingGui = "";
}

function ProjectOptionsGui::updateProject(%this)
{
    %project = ProjectOptionsGui.project;
    
    if (PONameEdit.text !$= %this.originalName)
    {
        %project.projectName = PONameEdit.text;
        %fileLocation = findProjectFileByName(%this.originalName);
        
        TamlWrite(%project, %fileLocation);
    }
    
    if (isObject(%this.invokingGui) && %this.invokingGui.isMethod("refresh"))
    {
        %this.invokingGui.refresh();
    }
    
    Canvas.popDialog(%this);
}

function ProjectOptionsGui::duplicateProject(%this)
{
    %counter = 2;
    
    %originalName = %this.originalName;
    %duplicateName = %originalName @ "_" @ %counter;
    %sourceModuleName = %this.project.sourceModule;
    
    %fileLocation = findProjectFileByName(%duplicateName);
    
    while (%fileLocation !$= "")
    {
        %counter++;
        %duplicateName = %originalName @ "_" @ %counter;
        %fileLocation = findProjectFileByName(%duplicateName);
    }
    
    createNewProject(%duplicateName, filePath(findProjectFileByName(%originalName)) @ "/", %sourceModuleName, true, false, %this.project.Type);

    if (isObject(%this.invokingGui) && %this.invokingGui.isMethod("refresh"))
            %this.invokingGui.refresh();

    Canvas.popDialog(%this);
}

function ProjectOptionsGui::showDeletePrompt(%this)
{
    %msg = "Are you sure you want to delete " @ %this.originalName @ " and all assets associated with it?";
    TSSConfirmDeleteProjectGui.display(%msg , "ProjectOptionsGui", "deleteProject", "");
    //%this.deleteProject();
}

function ProjectOptionsGui::deleteProject(%this)
{
    %fileLocation = findProjectFileByName(%this.originalName);
    %module = %this.project.sourceModule;
    
    if (%fileLocation !$= "")
    {
        %localDataDirectory = $UserGamesLocation @ "/My Games/" @ $Game::CompanyName @ "/" @ %module @ "/" @ %this.originalName;
        %directory = filePath(%fileLocation);
        
        directoryDelete(%directory);
        directoryDelete(%localDataDirectory);
        
        if (isObject(%this.invokingGui) && %this.invokingGui.isMethod("refresh"))
            %this.invokingGui.refresh();
        
        Canvas.popDialog(%this);
    }
}

function PONameEdit::validateName(%this, %newText)
{
    if (%newText !$= "" && ProjectOptionsGui.originalName !$= %newText)
        POOkButton.setActive(true);        
    else
        POOkButton.setActive(false);
    
    %this.text = %newText;
    ProjectOptionsGui.nameUpdated = true;
}
