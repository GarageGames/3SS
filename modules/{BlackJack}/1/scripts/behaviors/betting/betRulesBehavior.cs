//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------


//file: betRulesBehavior.cs

// Create this behavior only if it does not already exist
if (!isObject(BetRulesBehavior))
{
    // Create this behavior from the blank BehaviorTemplate
    // Name it BetRulesBehavior
    %template = new BehaviorTemplate(BetRulesBehavior);

    // friendlyName will be what is displayed in the editor
    // behaviorType organize this behavior in the editor
    // description briefly explains what this behavior does
    %template.friendlyName = "Bet Rules";
    %template.behaviorType = "Betting Component";
    %template.description  = "Defines bet restrictions for the table";

    %template.addBehaviorField(minBet, "The minimum amount the player is allowed to bet", int, 1);
    %template.addBehaviorField(maxBet, "The maximum amount the player is allowed to bet", int, 250);
//    %template.addBehaviorField(payoutWin, "The payout ratio when you win (but with no blackjack)", float, 1);
    %template.addBehaviorField(payoutWinNumerator, "The win payout ratio numerator", int, 1);
    %template.addBehaviorField(payoutWinDenominator, "The win payout ratio denominator", int, 1);
    //  %template.addBehaviorField(payoutBlackjack, "The payout ratio when you win with blackjack", float, 0.5);
    %template.addBehaviorField(payoutBlackjackNumerator, "The blackjack payout ratio numerator", int, 3);
    %template.addBehaviorField(payoutBlackjackDenominator, "The blackjack payout ratio denominator", int, 2);
    %template.addBehaviorField(payoutPush, "The payout ratio when you push", float, 0);
    %template.addBehaviorField(payoutLose, "The payout ratio when you lose", float, -1);
}

/// <summary>
/// Called when the behavior is added to the scene
/// </summary>
/// <param name="scene">The scene object the behavior was added to.</param>
function BetRulesBehavior::onBehaviorAdd(%this, %scene)
{
    $CurrentBetRules = %this;
    %this.setWinPayout(%this.payoutWinNumerator, %this.payoutWinDenominator);
    %this.setBlackjackPayout(%this.payoutBlackjackNumerator, %this.payoutBlackjackDenominator);
}

/// <summary>
/// Checks if a bet amount is legal with this rule set
/// </summary>
/// <param name="betAmount">Amount of the bet that is being checked.</param>
/// <return>bool that is true if the bet is legal, false otherwise.</return>
function BetRulesBehavior::betIsLegal(%this, %betAmount)
{
    if (%betAmount > %this.maxBet)
        return false;

    if (%betAmount < %this.minBet)
        return false;

    return true;
}

/// <summary>
/// Set the win payout
/// </summary>
/// <param name="numerator">Numerator of the payout ratio.</param>
/// <param name="denominator">Denominator of the payout ratio.</param>
function BetRulesBehavior::setWinPayout(%this, %numerator, %denominator)
{
    %this.payoutWinNumerator = %numerator;
    %this.payoutWinDenominator = %denominator;
    %this.payoutWin = %numerator / %denominator;
}

/// <summary>
/// Get the win payout
/// </summary>
/// <return>2-element list containing the ratio "numerator denominator".</return>
function BetRulesBehavior::getWinPayout(%this)
{
    return %this.payoutWinNumerator SPC %this.payoutWinDenominator;
}

/// <summary>
/// Set the blackjack payout
/// </summary>
/// <param name="numerator">Numerator of the payout ratio.</param>
/// <param name="denominator">Denominator of the payout ratio.</param>
function BetRulesBehavior::setBlackjackPayout(%this, %numerator, %denominator)
{
    %this.payoutBlackjackNumerator = %numerator;
    %this.payoutBlackjackDenominator = %denominator;
    %this.payoutBlackjack = %numerator / %denominator;
}

/// <summary>
/// Get the blackjack payout
/// </summary>
/// <return>2-element list containing the ratio "numerator denominator".</return>
function BetRulesBehavior::getBlackjackPayout(%this)
{
    return %this.payoutBlackjackNumerator SPC %this.payoutBlackjackDenominator;
}