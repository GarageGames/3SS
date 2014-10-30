//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2014
//-----------------------------------------------------------------------------
$NewExercise::ExerciseMessageString = "Enter a name...";

function showNewExerciseDialog(%name)
{
    $SelectedExercise = %name; 
    Canvas.pushDialog(NewExerciseDlg);
}

function NewExerciseDlg::onWake(%this)
{      
    %moduleID = $SelectedExercise;
    
    if ($exerciseCount <= 0)
    {
        parseExercises();
    }

    for (%i = 0; %i < $exerciseCount; %i++)
    {
        if ($exerciseList[%i].moduleID $= $SelectedExercise)
        {
            %icon = $exerciseList[%i].icon;   
            break;
        }
    }
    
    %normalIcon = "{3SSHomeScreens}:" @ %icon @ "_normal";
    NED_PreviewImage.Image = %normalIcon;
    NED_NameEdit.initialize($NewExercise::ExerciseMessageString);
    NED_CreateButton.update(); 
}

function NED_NameEdit::onKeyPressed(%this)
{
    // Disallow symbols and spaces
    %name = stripChars(%this.getValue(), "-+*/%$&§=()[].?\"#,;!~<>|°^{} ");
    %this.setText(%name);
    
    %this.update();
    NED_CreateButton.update(); 
} 

function NED_NameEdit::onReturn(%this)
{
    NED_CreateButton.setStateOn(true);
    createNewExercise();
}

function NED_CreateButton::update(%this)
{
    %active = true;    

    // Don't allow saving if the name is blank
    if (NPNameEdit.isEmpty())
    {
        %active = false;
    }

    %this.setActive(%active);
}

function createNewExercise()
{
    %exerciseLocation = $ExercisesLocation;

    %exerciseFileSpec = %exerciseLocation @ "/*.exercise.taml";
    
    addResPath(%exerciseLocation);
    
    for (%file = findFirstFile(%exerciseFileSpec); %file !$= ""; %file = findNextFile(%exerciseFileSpec))
    {
        %exercise = TamlRead(%file);
        
        if ($SelectedExercise $= %exercise.ModuleId)
        {
            %project = TamlRead(filePath(%file) @ "/project.tssproj");            
            
            break;
        }
    }
    
    removeResPath(%exerciseLocation);
    
    createNewProject(NED_NameEdit.getText(), filePath(%file) @ "/", %project.sourceModule, true, true, $SelectedExercise);
    
    Canvas.popDialog(NewExerciseDlg);
}