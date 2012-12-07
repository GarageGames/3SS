//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------
// UpdateFontFromTemplateBehavior - Behavior for bitmap fonts to have them update their
// imagemaps from a template.
//-----------------------------------------------------------------------------

// Create this behavior only if it does not already exist
if (!isObject(UpdateFontFromTemplateBehavior))
{
    %template = new BehaviorTemplate(UpdateFontFromTemplateBehavior);

    %template.friendlyName = "Update Font From Template";
    %template.behaviorType = "Bitmap Fonts";
    %template.description  = "Updates a bitmap font from a template when the level loads.";

    %template.addBehaviorField(bitmapFontTemplate, "Persistent bitmapFont to use as template", object, "", BitmapFontObject);
}

function UpdateFontFromTemplateBehavior::onBehaviorAdd(%this)
{
}

function UpdateFontFromTemplateBehavior::onLevelLoaded(%this)
{
    %this.owner.imageMap = %this.bitmapFontTemplate.imageMap;
}
