//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$LastSelectedRadioButton = TestGameRadioButtonPCMac;

function TestGameWindowGui::onWake(%this)
{
    $LastSelectedRadioButton.setStateOn(true);
    $LastSelectedRadioButton.onClick();
}

function TestGameButton::onClick(%this)
{
    switch$($SelectedDevice)
    {
        case "iPhone":
            echo("Now playing iPhone build");
            $pref::Video::defaultResolution = "480 320";

        case "iPad":
            echo("Now running iPad resolution");
            $pref::Video::defaultResolution = "1024 768";

        case "Desktop":
            echo("Now running Desktop Resolution");
            $pref::Video::defaultResolution = "1024 768";
    }    
    
    // Save config data
    _saveGameConfigurationData();
    
    SaveTemplateData();
    
    synchronizeGame();
    
    runGame();
}

function TestGameCancelButton::onClick(%this)
{
    Canvas.popDialog(TestGameWindowGui);
}

function TestGameRadioButtoniPad::onClick(%this)
{
    $LastSelectedRadioButton = TestGameRadioButtoniPad;
    $SelectedDevice = "iPad";
}

function TestGameRadioButtoniPhone::onClick(%this)
{
    $LastSelectedRadioButton = TestGameRadioButtoniPhone;
    $SelectedDevice = "iPhone";
}

function TestGameRadioButtonPCMac::onClick(%this)
{
    $LastSelectedRadioButton = TestGameRadioButtonPCMac;
    $SelectedDevice = "Desktop";
}
