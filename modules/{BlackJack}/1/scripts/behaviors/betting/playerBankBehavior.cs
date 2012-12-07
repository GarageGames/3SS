//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$BankruptLabelDurationSecs = 3;

// Create this behavior only if it does not already exist
if (!isObject(PlayerBankBehavior))
{
    %template = new BehaviorTemplate(PlayerBankBehavior);

    %template.friendlyName = "Player Bank";
    %template.behaviorType = "Player";
    %template.description  = "Manages the player's total cash";
}

/// <summary>
/// Initializes currentCash and bankruptLabel, and starts update loop.
/// </summary>
function PlayerBankBehavior::init(%this)
{
    %this.currentCash = %this.startingCash;
    %this.bankruptLabel = BankruptIcon;

    if (isEventPending(%this.updateHandle))
        cancel(%this.updateHandle);

    %this.updateHandle = %this.schedule(200, "update");
}

/// <summary>
/// Sets currentCash from the given player profile.
/// </summary>
/// <param name="profile">Player profile.</param>
function PlayerBankBehavior::loadPlayerCash(%this, %profile)
{
    %this.currentCash = %profile.currentCash;
}

/// <summary>
/// Saves currentCash to the give player profile.
/// </summary>
/// <param name="profile">Player profile.</param>
function PlayerBankBehavior::savePlayerCash(%this, %profile)
{
    %profile.currentCash = %this.currentCash;
}

/// <summary>
/// Updates the numericalDisplay with the current bank cash amount
/// and schedules the next update.
/// </summary>
function PlayerBankBehavior::update(%this)
{
    if (isObject(%this.numericalDisplay))
    {
        %this.numericalDisplay.setText(mFloatLength(%this.currentCash, 2));
        //%this.numericalDisplay.updateWithAmount(%this.currentCash);
    }

    %this.updateHandle = %this.schedule(200, "update");
}

/// <summary>
/// Called when the ResetBankButton is clicked in the start level
/// Resets the player profile and bank
/// </summary>
function resetBankButtonClicked()
{
    // Reset the default player profile and player bank
    if (isObject($DefaultPlayerProfile))
    {
        alxPlay($Save::GeneralSettings::Sound::ButtonClick);

        $DefaultPlayerProfile.reset();
        %bankBehavior = $UserControlledPlayer.bank;

        if (isObject(%bankBehavior))
            %bankBehavior.loadPlayerCash($DefaultPlayerProfile);
    }
}

/// <summary>
/// Shows the bankruptcy label over the bank display.
/// </summary>
function PlayerBankBehavior::showBankruptLabel(%this)
{
    // don't allow labels to stack.
    if (isEventPending(%this.removeLabelHandle))
    {
        // handle it manually
        cancel(%this.removeLabelHandle);
        %this.removeLabel(false);
    }

    if (isObject(%this.bankruptLabel))
    {
        %this.label = %this.bankruptLabel.clone();
        %this.label.setPosition(%this.numericalDisplay.getPosition());
        %this.label.setVisible(true);
        %this.removeLabelHandle = %this.schedule($BankruptLabelDurationSecs * 1000, "removeBankruptLabel");
    }
}

/// <summary>
/// Removes the bankruptcy label.
/// </summary>
function PlayerBankBehavior::removeBankruptLabel(%this)
{
    %this.removeLabelHandle = 0;

    if (isObject(%this.label))
    {
        %this.label.safeDelete();
    }
}


