//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------
// Command = "createNewProject( NewGameNameText.getText(), NewGameGui.template);";

function newTemplateProject(%templateName)
{
   NewGameGui.template = %templateName;
   
   Canvas.pushDialog(NewGameGui);
}

function NewGameGui::onWake(%this)
{
   %previewImage = "^{EditorAssets}/gui/start/" @ NewGameGui.template @ "Preview.png";
   
   NewGamePreviewImage.bitmap = %previewImage;
}