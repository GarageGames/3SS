//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function HttpQueue::onRequestError(%this, %error)
{
    error("HttpQueue::onRequestError(): " @ %error);
}

function HttpQueue::onRequestFinished(%this, %response)
{
    echo("HttpQueue::onRequestFinished():");
    echo(%response);
}
