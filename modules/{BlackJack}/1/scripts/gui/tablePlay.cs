//-----------------------------------------------------------------------------
// Torque Card Template
// Copyright © GarageGames, LLC.
//-----------------------------------------------------------------------------

function TablePlayGui::onWake(%this)
{
   
}

function TablePlayGUI::setButtonVisibility(%this, %buttonName, %visible)
{
   %buttonName.setVisible(%visible);
}

function TablePlayCloseButton::onClick(%this)
{
   $CurrentTable.exitButtonClicked();
}

function TablePlayRebetButton::onClick(%this)
{
   $CurrentTable.rebetButtonClicked();
}

function TablePlayDealButton::onClick(%this)
{
   $CurrentTable.dealButtonClicked();
}

function TablePlayHitButton::onClick(%this)
{
   $CurrentTable.hitButtonClicked();
}

function TablePlayStandButton::onClick(%this)
{
   $CurrentTable.standButtonClicked();
}

function TablePlayDoubleButton::onClick(%this)
{
   $CurrentTable.doubleDownButtonClicked();
}

function TablePlaySplitButton::onClick(%this)
{
   $CurrentTable.splitButtonClicked();
}