//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------
function ConsoleEntry::eval()
{
    %text = trim(ConsoleEntry.getValue());

    if(strpos(%text, "(") == -1)
    {
        if(strpos(%text, "=") == -1 && strpos(%text, " ") == -1)
        {
            if(strpos(%text, "{") == -1 && strpos(%text, "}") == -1)
            {
                %text = %text @ "()";
            }
        }
    }

    %pos = strlen(%text) - 1;
    
    if(strpos(%text, ";", %pos) == -1 && strpos(%text, "}") == -1)
        %text = %text @ ";";

    echo("==>" @ %text);
    eval(%text);
    ConsoleEntry.setValue("");

}

function ToggleConsole(%make)
{
    if (%make)
    {
        if (ConsoleDlg.isAwake())
        {
            // Deactivate the console.
            if ( $enableDirectInput )
                activateKeyboard();

            Canvas.popDialog(ConsoleDlg);
        }
        else
        {
            if ( $enableDirectInput )
                deactivateKeyboard();
            
            Canvas.pushDialog(ConsoleDlg, 99);         
            ConsoleEntry.setFirstResponder();
        }
    }
}