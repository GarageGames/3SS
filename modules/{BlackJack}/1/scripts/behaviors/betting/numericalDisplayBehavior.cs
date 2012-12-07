//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------
// Behavior for a text display.
//-----------------------------------------------------------------------------

//file: numericalDisplayBehavior.cs

// Create this behavior only if it does not already exist
if (!isObject(NumericalDisplayBehavior))
{
    // Create this behavior from the blank BehaviorTemplate
    // Name it NumericalDisplayBehavior
    %template = new BehaviorTemplate(NumericalDisplayBehavior);

    // friendlyName will be what is displayed in the editor
    // behaviorType organize this behavior in the editor
    // description briefly explains what this behavior does
    %template.friendlyName = "Numerical Display";
    %template.behaviorType = "Betting Component";
    %template.description  = "Displays the players bank amount";

    %template.addBehaviorField(numberSheetImageObject, "Number sheet object to use for displaying digits", object, "", SceneObject);
    %template.addBehaviorField(displayZero, "Controls whether to display anything for zero amounts", bool, false);
    %template.addBehaviorField(decimalPlaces, "How many decimal places to display", int, 0);
    %template.addBehaviorField(decimalCell, "The cell index of the decimal symbol", int, 11);
    %anchorOptions = "Center" TAB "Center Decimal" TAB "Right" TAB "Left";
    %template.addBehaviorField(anchorPoint, "Where to anchor the number string", enum, "Center", %anchorOptions);
    %template.addBehaviorField(spacingModifier, "How much space to put between digits (can be negative for overlapping digits", float, 0.0);
}

/// <summary>
/// Called when behavior is added to an object.
/// </summary>
function NumericalDisplayBehavior::onBehaviorAdd(%this)
{
    %this.digitObjectList = "";
}

/// <summary>
/// Updates the text display with the current bank cash amount
/// and schedules the next update.
/// </summary>
function NumericalDisplayBehavior::update(%this)
{
    %this.schedule(200, "update");
}

/// <summary>
/// Updates the text display with the current bank cash amount
/// and schedules the next update.
/// </summary>
/// <param name="amount">Number amount to update display with.</param>
function NumericalDisplayBehavior::updateWithAmount(%this, %amount)
{
    // Do not update in the Editor
    if ($LevelEditorActive)
        return;

    // Calculate digit count (including decimal point and places)
    %digitCount = %this.getDigitCount(%amount);

    // Clear all digit objects
    %this.clearAllDigits();

    // If the amount is zero, and displayZero is false, return
    if (%amount == 0 && !%this.displayZero)
        return;


    if (%this.decimalPlaces)
    {
        // Add digits to the right of the decimal
        for (%i = 0; %i < %this.decimalPlaces; %i++)
        {
            %tempAmount = %amount * mPow(10, %this.decimalPlaces) / mPow(10, %i);
            %digitValue = mFloor(%tempAmount % 10);
            %this.setDigit(%i, %digitValue, %digitCount);
        }

        // Add the decimal point
        %this.setDigit(%this.decimalPlaces, %this.decimalCell, %digitCount);
    }

    // Set digits to the left of the decimal
    for (%i = 0; %i < %this.getWholeNumberDigitCount(%amount); %i++)
    {
        // Calculate the digit value for the current index
        %tempAmount = %amount / mPow(10, %i);
        %digitValue = mFloor(%tempAmount % 10);

        %digitPosition = %i;

        // Add more digits for the decimal places
        if (%this.decimalPlaces)
            %digitPosition += %this.decimalPlaces + 1;

        %this.setDigit(%digitPosition, %digitValue, %digitCount);
    }
}

/// <summary>
/// Gets the number of digits necessary to display the given amount
/// including the decimal point and decimal places.
/// </summary>
/// <param name="amount">Number amount.</param>
/// <return>int containing the number of digits.</return>
function NumericalDisplayBehavior::getDigitCount(%this, %amount)
{
    // Get the number of digits for the whole number amount
    %count = %this.getWholeNumberDigitCount(%amount);

    // Add more digits for the decimal places
    if (%this.decimalPlaces)
        %count += %this.decimalPlaces + 1;

    return %count;
}

/// <summary>
/// Gets the number of digits necessary to display whole-number component of the given amount.
/// </summary>
/// <param name="amount">Number amount.</param>
/// <return>int containing the number of digits.</return>
function NumericalDisplayBehavior::getWholeNumberDigitCount(%this, %amount)
{
    %remaining = mAbs(%amount);
    %count = 1;

    while (%remaining >= 10)
    {
        %remaining = %remaining / 10;
        %count++;
    }

    return %count;
}

/// <summary>
/// Sets a digit object for the digitIndex with the given number sheet cell.
/// </summary>
/// <param name="digitIndex">Index position of the digit.</param>
/// <param name="numberSheetCell">Celled number sheet imagemap.</param>
/// <param name="digitCount">Number of digits.</param>
function NumericalDisplayBehavior::setDigit(%this, %digitIndex, %numberSheetCell, %digitCount)
{
    // Check if the digit object already exists, if not create it
    if (!isObject(%this.digitArray[%digitIndex]))
    {
        %this.digitArray[%digitIndex] = %this.numberSheetImageObject.clone();
        %this.digitObjectList = %this.digitArray[%digitIndex] SPC %this.digitObjectList;
    }

    // Set the position of the digit
    %this.digitArray[%digitIndex].position = %this.getDigitPosition(%digitIndex, %digitCount);

    // Set the value of the digit
    %this.digitArray[%digitIndex].frame = %numberSheetCell;

    // Set digit object to visible
    %this.digitArray[%digitIndex].setVisible(true);
}



/// <summary>
/// Returns the world position to place the digit object
/// for the given digit index.
/// </summary>
/// <param name="digitIndex">Index position of the digit.</param>
/// <param name="digitCount">Number of digits.</param>
/// <return>The "x y" position to place the digit</return>
function NumericalDisplayBehavior::getDigitPosition(%this, %digitIndex, %digitCount)
{
    // Calculate the digit position by spacing to the left
    %width = %this.numberSheetImageObject.getWidth();

    // Set the starting position based on the anchorPoint
    %startingPosition = %this.owner.getPosition();

    switch$ (%this.anchorPoint)
    {
    case "Center":
        %startingPosition.x += (%width * %digitCount) / 2;

    case "Center Decimal":
        %startingPosition.x += %width * %this.decimalPlaces + %width / 2;

    case "Left":
        %startingPosition.x += %width * %digitCount;

    case "Right":

    default:

    }

    %positionX = %startingPosition.x - %digitIndex * (%width + %this.spacingModifier);
    %positionY = %startingPosition.y;

    return %positionX SPC %positionY;
}

/// <summary>
/// Resets the frame number and sets invisible all digit objects
/// created by this behavior.
/// </summary>
function NumericalDisplayBehavior::clearAllDigits(%this)
{
    for (%i = 0; %i < getWordCount(%this.digitObjectList); %i++)
    {
        %digitObject = getWord(%this.digitObjectList, %i);
        if (isObject(%digitObject))
        {
            %digitObject.frame = 0;
            %digitObject.setVisible(false);
        }
    }
}

/// <summary>
/// Removes all digit objects that have been created by this behavior.
/// </summary>
function NumericalDisplayBehavior::deleteAllDigits(%this)
{
    for (%i = 0; %i < getWordCount(%this.digitObjectList); %i++)
    {
        %digitObject = getWord(%this.digitObjectList, %i);
        if (isObject(%digitObject))
        {
            %digitObject.safeDelete();
        }
    }
}



