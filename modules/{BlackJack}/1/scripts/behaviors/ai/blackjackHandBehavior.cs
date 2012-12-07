//-----------------------------------------------------------------------------
// Torque Card Template
// Copyright © GarageGames, LLC.
//-----------------------------------------------------------------------------
// Blackjack "Hand" behavior.
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Designer Values
//-----------------------------------------------------------------------------

/// Number of cards per row before wrap around.
$BlackjackHandWrapCount = 6;

/// X Offset of each card.
$BlackjackHandCardXOffset = 0.234;

/// Additional X Offset of each card row.
$BlackjackHandCardRowXOffset = 0.039;

/// Y Offset of each card row.
$BlackjackHandCardYOffset = 0.117;

/// X/Y Offsets of the hand numerical displays.
$BlackjackHandValueDisplayXOffset = -0.375;
$BlackjackHandValueDisplayYOffset = 0.0;


/// <summary>
/// Set up the hand
/// </summary>
/// <param "obj">Object this is associated with.</param>
/// <param "%offset">Offset from object's position to draw the hand</param>
function BlackjackHandBehavior::init(%this, %obj, %offset)
{
    %this.obj = %obj;
    %this.offset = %offset;
    %this.cardArrayCount = 0;
}

/// <summary>
/// Echo out the current hand held.
/// </summary>
function BlackjackHandBehavior::echoCards(%this)
{
    %str = "";
    for (%i = 0; %i < %this.cardArrayCount; %i++)
    {
        if (%i == 0)
            %str = %this.cardArray[%i];
        else
            %str = %str SPC %this.cardArray[%i];
    }

    return %str;
}

/// <summary>
/// Reset the current hand.
/// </summary>
function BlackjackHandBehavior::reset(%this)
{
    // handle user "turbo-clicking" the deal button
    if (isObject(%this.label))
    {
        %this.label.safeDelete();
    }

    %this.hasBlackjack = false;

    for (%i = 0; %i < %this.cardArrayCount; %i++)
    {
        %this.playingCardArray[%i].disposeCard();
        %this.playingCardArray[%i] = 0;
    }

    %this.cardArrayCount = 0;

    // Hide the card value display
    %this.removeHandValueDisplay();
}

/// <summary>
/// Show the hand value numerical display
/// </summary>
function BlackjackHandBehavior::showHandValueDisplay(%this, %numericalDisplayTemplate)
{
    if (!isObject(%this.handValueDisplay))
    {
        %this.handValueDisplay = %numericalDisplayTemplate.clone();
        %offsetX = %this.offset.x + $BlackjackHandValueDisplayXOffset;
        %offsetY = %this.offset.y + $BlackjackHandValueDisplayYOffset;

        // Rotate
        %targetRotationDegrees = %this.obj.getAngle();
        %cosOfAngle = mCos(mDegToRad(%targetRotationDegrees));
        %sinOfAngle = mSin(mDegToRad(%targetRotationDegrees));
        %rotatedOffsetX = %offsetX * %cosOfAngle - %offsetY * %sinOfAngle;
        %rotatedOffsetY = %offsetX * %sinOfAngle + %offsetY * %cosOfAngle;

        %posX = %this.obj.position.x + %rotatedOffsetX;
        %posY = %this.obj.position.y + %rotatedOffsetY;
        %this.handValueDisplay.position = %posX SPC %posY;
    }

    if ($CurrentTable.showHandValues && %this.getVisibleValue() > 0)
        %this.handValueDisplay.setText(%this.getVisibleValue());
    else
        %this.handValueDisplay.setText("");
}

/// <summary>
/// Remove the hand value numerical display
/// </summary>
function BlackjackHandBehavior::removeHandValueDisplay(%this)
{
    if (isObject(%this.handValueDisplay))
    {
        %this.handValueDisplay.safeDelete();
    }
}

/// <summary>
/// Show a label over the hand.
/// </summary>
function BlackjackHandBehavior::showLabel(%this, %label, %pos, %time)
{
    // don't allow labels to stack.
    if (isObject(%this.label))
    {
        %this.label.safeDelete();
    }

    %this.label = %label;
    if (isObject(%label))
    {
        %this.label.setPosition(%pos);
        %this.label.setVisible(true);
        %this.label.setAngle(%this.getCardRotation());
    }
    //%this.removeLabelHandle = %this.schedule(%time * 1000, "removeLabel", true);
}

/// <summary>
/// Sets the layer for the entire hand
/// </summary>
function BlackjackHandBehavior::setHandLayer(%this, %layer)
{
    for (%i = 0; %i < %this.cardArrayCount; %i++)
    {
        %this.playingCardArray[%i].setSceneLayer(%layer);
    }
}

/// <return>
/// The card position for the %i-th card.
/// </return>
function BlackjackHandBehavior::getCardPosition(%this, %i)
{
    %y = 0;
    while (%i >= $BlackjackHandWrapCount)
    {
        %i -= $BlackjackHandWrapCount;
        %y++;
    }

    %targetPos = %this.obj.getWorldPoint(%this.obj.getLocalCenter());
    %offsetX = %this.offset.x + (%i * $BlackjackHandCardXOffset) + (%y * $BlackjackHandCardRowXOffset);
    %offsetY = %this.offset.y - (%y * $BlackjackHandCardYOffset);

    // rotate
    %targetRotationDegrees = %this.obj.getAngle();
    %cosOfAngle = mCos(mDegToRad(%targetRotationDegrees));
    %sinOfAngle = mSin(mDegToRad(%targetRotationDegrees));
    %rotatedOffsetX = (%offsetX * %cosOfAngle) - (%offsetY * %sinOfAngle);
    %rotatedOffsetY = (%offsetX * %sinOfAngle) + (%offsetY * %cosOfAngle);

    %targetPos.x += %rotatedOffsetX;
    %targetPos.y += %rotatedOffsetY;

    return %targetPos;
}

/// <return>
/// The position to place a label based on the number of cards.
/// </return>
function BlackjackHandBehavior::getLabelPosition(%this)
{
    %y = 1;
    %i = %this.cardArrayCount;
    // only worry about the top row of cards.
    if (%i > $BlackjackHandWrapCount)
        %i = $BlackjackHandWrapCount /2;
    else
        %i /= 2;
    
    %targetPos = %this.obj.getPosition();
    %offsetX = %this.offset.x + (%i * $BlackjackHandCardXOffset);
    %offsetY = %this.offset.y + (%y * $BlackjackHandCardYOffset);

    // rotate
    %targetRotationDegrees = %this.obj.getAngle();
    %cosOfAngle = mCos(mDegToRad(%targetRotationDegrees));
    %sinOfAngle = mSin(mDegToRad(%targetRotationDegrees));
    %rotatedOffsetX = %offsetX * %cosOfAngle - %offsetY * %sinOfAngle;
    %rotatedOffsetY = %offsetX * %sinOfAngle + %offsetY * %cosOfAngle;

    %targetPos.x += %rotatedOffsetX;
    %targetPos.y += %rotatedOffsetY;

    return %targetPos;
}

/// <return>
/// The angle of the card.
/// </return>
function BlackjackHandBehavior::getCardRotation(%this, %i)
{
    return %this.obj.getAngle();
}

/// <summary>
/// Draw a card and add it to the hand.
/// </summary>
/// <param "%faceDown">If true, keep the card face down.</param>
function BlackjackHandBehavior::drawCard(%this, %faceDown)
{
    if (!isObject($CurrentShoe))
    {
        error("$CurrentShoe doesn't exist!");
        return;
    }

    %this.playingCardArray[%this.cardArrayCount] = $CurrentShoe.drawCard(%this);
    %this.playingCardArray[%this.cardArrayCount].callOnBehaviors(setIsFaceDown, %faceDown);
    %this.cardArray[%this.cardArrayCount] = %this.playingCardArray[%this.cardArrayCount].cardValue;

    %targetPos = %this.getCardPosition(%this.cardArrayCount);
    %targetRotation = %this.getCardRotation(%this.cardArrayCount);
    %this.playingCardArray[%this.cardArrayCount].callOnBehaviors(DealCardTo, %targetPos, %targetRotation);

    %travelTimeMSecs = %this.playingCardArray[%this.cardArrayCount].callOnBehaviors(getTravelTimeMSecs, %targetPos);

    %this.cardArrayCount++;

    // Display the card value numerical display
    %this.schedule(%travelTimeMSecs, "showHandValueDisplay", $CurrentTable.handNumericalDisplay);
}

/// <summary>
/// Draw a card from another hand and add it to this one.
/// </summary>
function BlackjackHandBehavior::drawCardFromHand(%this, %hand)
{
    %hand.cardArrayCount--;
    %this.playingCardArray[%this.cardArrayCount] = %hand.playingCardArray[%hand.cardArrayCount];
    %this.cardArray[%this.cardArrayCount] = %this.playingCardArray[%this.cardArrayCount].cardValue;

    %targetPos = %this.getCardPosition(%this.cardArrayCount);
    %targetRotation = %this.getCardRotation(%this.cardArrayCount);
    %this.playingCardArray[%this.cardArrayCount].DealCardTo(%targetPos, %targetRotation);

    %this.cardArrayCount++;

    // Display the card value numerical display
    %this.showHandValueDisplay($CurrentTable.handNumericalDisplay);
    %hand.showHandValueDisplay($CurrentTable.handNumericalDisplay);
}

/// <summary>
/// Flip all the cards in the hand to showing.
/// </summary>
function BlackjackHandBehavior::showAllCards(%this)
{
    for (%i = 0; %i < %this.cardArrayCount; %i++)
        %this.playingCardArray[%i].flipCard();

    // Display the card value numerical display
    %this.showHandValueDisplay($CurrentTable.handNumericalDisplay);
}

/// <summary>
/// Get the 'raw' face value of the card at an index.
/// </summary>
/// <param name="%n">Index of card who's value we're interested in.</param>
/// <return>Value of the card as 2-9, T, J, Q, K, or A (0 if not a valid card).</return>
function BlackjackHandBehavior::getCardFaceRawValue(%this, %n)
{
    if ((%n < 0) || (%n >= %this.cardArrayCount))
        return 0;

    return firstWord(%this.cardArray[%n]);
}

/// <summary>
/// Get the value of the card at an index.
/// Note: Aces always return 11 with this function.
/// </summary>
/// <param name="%n">Index of card who's value we're interested in.</param>
/// <return>Value of the card as 2-11 (0 if not a valid card).</return>
function BlackjackHandBehavior::getCardValue(%this, %n)
{
    %rtn = %this.getCardFaceRawValue(%n);

    if ((%rtn >= 2) && (%rtn <= 9))
        return %rtn;

    if (%rtn $= "A")
        return 11;

    if ((%rtn $= "J") || (%rtn $= "Q") || (%rtn $= "K") || (%rtn $= "T"))
        return 10;

    return 0;
}

/// <summary>
/// Get the value of the card at an index.
/// </summary>
/// <param name="%n">Index of card who's value we're interested in.</param>
/// <return>Value of the card as 2-9, T, or A (0 if not a valid card).</return>
function BlackjackHandBehavior::getCardFaceValue(%this, %n)
{
    %rtn = %this.getCardFaceRawValue(%n);

    %rtn = firstWord(%this.cardArray[%n]);

    if ((%rtn >= 2) && (%rtn <= 9))
        return %rtn;

    if ((%rtn $= "A") || (%rtn $= "T"))
        return %rtn;

    if ((%rtn $= "J") || (%rtn $= "Q") || (%rtn $= "K"))
        return "T";

    return 0;
}

/// <summary>
/// Convert an int to a card value.
/// </summary>
/// <return>A, T, or 2-9.</return>
function BlackjackHandBehavior::IntToCardValue(%this, %n)
{
    if ((%n == 1) || (%n == 11))
        return "A";

    if (%n == 10)
        return "T";

    return %n;
}

/// <summary>
/// Get the total value of the current hand.
/// If the hand contains one or more Aces, get the highest value this hand can be *iff*  that value is less than 22.
/// </summary>
/// <return>The total value of the hand as an int.</return>
function BlackjackHandBehavior::getValue(%this)
{
    %aceCount = 0;
    %total = 0;

    for (%i = 0; %i < %this.cardArrayCount; %i++)
    {
        %n = %this.getCardValue(%i);
        if (%n == 11)
            %aceCount++;

        %total += %n;
    }

    while ((%total > 21) && (%aceCount > 0))
    {
        %aceCount--;
        %total -= 10;
    }

    return %total;
}

/// <return>The value of all face up cards.</return>
function BlackjackHandBehavior::getVisibleValue(%this)
{
    %aceCount = 0;
    %total = 0;

    for (%i = 0; %i < %this.cardArrayCount; %i++)
    {
        %n = %this.getCardValue(%i);

        %isFaceDown = %this.playingCardArray[%i].callOnBehaviors(getFieldValue, isFaceDown);
        if (%isFaceDown)
            %n = 0;

        if (%n == 11)
            %aceCount++;

        %total += %n;
    }

    while ((%total > 21) && (%aceCount > 0))
    {
        %aceCount--;
        %total -= 10;
    }

    return %total;
}

/// <summary>
/// If the hand contains one or more Aces, and the ......
/// </summary>
/// <return>true if hand has both soft and hard values, otherwise false.</return>
function BlackjackHandBehavior::hasSoftValue(%this)
{
    %aceCount = 0;
    %total = 0;

    for (%i = 0; %i < %this.cardArrayCount; %i++)
    {
        %n = %this.getCardValue(%i);
        if (%n == 11)
        {
            %total += 1;
            %aceCount++;
        }
        else
            %total += %n;
    }

    if (%aceCount <= 0)
        return false;

    return ((%total + 10) <= 21) ? true : false;
}

/// <summary>
/// If the hand contains one or more Aces, get the lowest value this hand can be.
/// </summary>
/// <return>The 'soft' value of the hand if one exists, otherwise return the hard value.</return>
function BlackjackHandBehavior::getSoftValue(%this)
{
    %total = 0;

    for (%i = 0; %i < %this.cardArrayCount; %i++)
    {
        %n = %this.getCardValue(%i);
        if (%n == 11)
            %total += 1;
        else
            %total += %n;
    }

    return %total;
}

/// <return>true if the current hand can be split.</return>
function BlackjackHandBehavior::isSplittable(%this)
{
    if (%this.cardArrayCount != 2)
        return false;

    if (!$CurrentTable.canSplit)
        return false;

    if ($CurrentTable && !$CurrentTable.canSplitTens)
    {
        // only split 'matching' tens.
        return (%this.getCardFaceRawValue(1) $= %this.getCardFaceRawValue(0)) ? true : false;
    }

    return (%this.getCardFaceValue(1) $= %this.getCardFaceValue(0)) ? true : false;
}
