$DeckTool::cardSuit = "S";
$DeckTool::cardValue = "A";

$GuiCardRowWidth = 672;
$GuiCardRowHeight = 256;

$DeckPath = "^project/managed/generatedDecks/";

$DeckNameBadChars = " /\\*~`!@#$%^&*()-=+[]{}'\";:?,.<>";

//Name
//internalName
//faceStyle - "standard" - "tall" - "full"
//allFacesSpecified - true or false
//valueStyle - "standard" - "different"
//pipStyle - "standard" - "face" - "all"
if(!isObject(NewDeck))
{
   $DeckTool::NewDeck = new ScriptObject(NewDeck)
   {
      Name = "NewDeck";
      internalName = "NewDeck";
      faceStyle = "standard";
      allFacesSpecified = false;
      valueStyle = "standard";
      pipStyle = "standard";
      
      cardBackImage = "Card_Back.png";
      cardFrontImage = "Card_Front.png";
      
      spadesFaceAce = "Face_Ace_b.png";
      spadesFaceJack = "Face_Jack_b.png";
      spadesFaceKing = "Face_King_b.png";
      spadesFaceQueen = "Face_Queen_b.png";
      spadesFace2 = "";
      spadesFace3 = "";
      spadesFace4 = "";
      spadesFace5 = "";
      spadesFace6 = "";
      spadesFace7 = "";
      spadesFace8 = "";
      spadesFace9 = "";
      spadesFace10 = "";
      
      clubsFaceAce = "Face_Ace_b.png";
      clubsFaceJack = "Face_Jack_b.png";
      clubsFaceKing = "Face_King_b.png";
      clubsFaceQueen = "Face_Queen_b.png";
      clubsFace2 = "";
      clubsFace3 = "";
      clubsFace4 = "";
      clubsFace5 = "";
      clubsFace6 = "";
      clubsFace7 = "";
      clubsFace8 = "";
      clubsFace9 = "";
      clubsFace10 = "";
      
      diamondsFaceAce = "Face_Ace_r.png";
      diamondsFaceJack = "Face_Jack_r.png";
      diamondsFaceKing = "Face_King_r.png";
      diamondsFaceQueen = "Face_Queen_r.png";
      diamondsFace2 = "";
      diamondsFace3 = "";
      diamondsFace4 = "";
      diamondsFace5 = "";
      diamondsFace6 = "";
      diamondsFace7 = "";
      diamondsFace8 = "";
      diamondsFace9 = "";
      diamondsFace10 = "";
      
      heartsFaceAce = "Face_Ace_r.png";
      heartsFaceJack = "Face_Jack_r.png";
      heartsFaceKing = "Face_King_r.png";
      heartsFaceQueen = "Face_Queen_r.png";
      heartsFace2 = "";
      heartsFace3 = "";
      heartsFace4 = "";
      heartsFace5 = "";
      heartsFace6 = "";
      heartsFace7 = "";
      heartsFace8 = "";
      heartsFace9 = "";
      heartsFace10 = "";
      
      topValueAceSpade = "Ace_b.png";
      topValue2Spade = "2_b.png";
      topValue3Spade = "3_b.png";
      topValue4Spade = "4_b.png";
      topValue5Spade = "5_b.png";
      topValue6Spade = "6_b.png";
      topValue7Spade = "7_b.png";
      topValue8Spade = "8_b.png";
      topValue9Spade = "9_b.png";
      topValue10Spade = "10_b.png";
      topValueJackSpade = "Jack_b.png";
      topValueQueenSpade = "Queen_b.png";
      topValueKingSpade = "King_b.png";
      
      bottomValueAceSpade = "";
      bottomValue2Spade = "";
      bottomValue3Spade = "";
      bottomValue4Spade = "";
      bottomValue5Spade = "";
      bottomValue6Spade = "";
      bottomValue7Spade = "";
      bottomValue8Spade = "";
      bottomValue9Spade = "";
      bottomValue10Spade = "";
      bottomValueJackSpade = "";
      bottomValueQueenSpade = "";
      bottomValueKingSpade = "";
      
      topValueAceClub = "Ace_b.png";
      topValue2Club = "2_b.png";
      topValue3Club = "3_b.png";
      topValue4Club = "4_b.png";
      topValue5Club = "5_b.png";
      topValue6Club = "6_b.png";
      topValue7Club = "7_b.png";
      topValue8Club = "8_b.png";
      topValue9Club = "9_b.png";
      topValue10Club = "10_b.png";
      topValueJackClub = "Jack_b.png";
      topValueQueenClub = "Queen_b.png";
      topValueKingClub = "King_b.png";
      
      bottomValueAceClub = "";
      bottomValue2Club = "";
      bottomValue3Club = "";
      bottomValue4Club = "";
      bottomValue5Club = "";
      bottomValue6Club = "";
      bottomValue7Club = "";
      bottomValue8Club = "";
      bottomValue9Club = "";
      bottomValue10Club = "";
      bottomValueJackClub = "";
      bottomValueQueenClub = "";
      bottomValueKingClub = "";
      
      topValueAceDiamond = "Ace_r.png";
      topValue2Diamond = "2_r.png";
      topValue3Diamond = "3_r.png";
      topValue4Diamond = "4_r.png";
      topValue5Diamond = "5_r.png";
      topValue6Diamond = "6_r.png";
      topValue7Diamond = "7_r.png";
      topValue8Diamond = "8_r.png";
      topValue9Diamond = "9_r.png";
      topValue10Diamond = "10_r.png";
      topValueJackDiamond = "Jack_r.png";
      topValueQueenDiamond = "Queen_r.png";
      topValueKingDiamond = "King_r.png";
      
      bottomValueAceDiamond = "";
      bottomValue2Diamond = "";
      bottomValue3Diamond = "";
      bottomValue4Diamond = "";
      bottomValue5Diamond = "";
      bottomValue6Diamond = "";
      bottomValue7Diamond = "";
      bottomValue8Diamond = "";
      bottomValue9Diamond = "";
      bottomValue10Diamond = "";
      bottomValueJackDiamond = "";
      bottomValueQueenDiamond = "";
      bottomValueKingDiamond = "";
      
      topValueAceHeart = "Ace_r.png";
      topValue2Heart = "2_r.png";
      topValue3Heart = "3_r.png";
      topValue4Heart = "4_r.png";
      topValue5Heart = "5_r.png";
      topValue6Heart = "6_r.png";
      topValue7Heart = "7_r.png";
      topValue8Heart = "8_r.png";
      topValue9Heart = "9_r.png";
      topValue10Heart = "10_r.png";
      topValueJackHeart = "Jack_r.png";
      topValueQueenHeart = "Queen_r.png";
      topValueKingHeart = "King_r.png";
      
      bottomValueAceHeart = "";
      bottomValue2Heart = "";
      bottomValue3Heart = "";
      bottomValue4Heart = "";
      bottomValue5Heart = "";
      bottomValue6Heart = "";
      bottomValue7Heart = "";
      bottomValue8Heart = "";
      bottomValue9Heart = "";
      bottomValue10Heart = "";
      bottomValueJackHeart = "";
      bottomValueQueenHeart = "";
      bottomValueKingHeart = "";
      
      topPipClub = "Clover_64.png";
      topPipDiamond = "Diamond_64.png";
      topPipHeart = "Heart_64.png";
      topPipSpade = "Spade_64.png";
      
      bottomPipSpade = "";
      bottomPipClub = "";
      bottomPipDiamond = "";
      bottomPipHeart = "";
      
      facePipSpade = "";
      facePipClub = "";
      facePipDiamond = "";
      facePipHeart = "";
      
      
   };
   
   $DeckTool::currentDeckEdit = $DeckTool::NewDeck;
   
}

if(!isObject(GeneratedDecks))
{
   $DeckTool::GeneratedDecks = new SimSet(GeneratedDecks);
}

/// <summary>
/// Called when another tab is selected
/// </summary>
function tabs::onTabSelected(%this, %tab)
{
   if(%tab $= "Preview")
   {
      CustomDeckToolGui.refreshPreviewWindow();
   }
}

/// <summary>
/// Launch help page.
/// </summary>
function CustomDeckEditorHelpButton::onClick(%this)
{
   gotoWebPage("http://docs.3stepstudio.com/blackjack/customdeck/");
}

/// <summary>
/// Opens the file dialog to browse to an image and select it.
/// </summary> 
function CustomDeckToolGui::browseButtonImage(%this)
{
   %container = $ThisControl.getParent();
   %textEdit = %container.findObjectByInternalName("textEdit", true);
   %preview = %container.findObjectByInternalName("ImagePreview", true);
   OpenImageFileForFields(%preview, %textEdit, "", "/managed/generatedDecks/");
   %textEdit.setText(fileName(%textEdit.getText()));
}

/// <summary>
/// Sets faceStyle of the deck currently being edited
/// </summary>
/// <param="%styleSelected">Style to set the faceStyle to</param> 
function CustomDeckToolGui::selectFaceStyle(%this, %styleSelected)
{
   //set the variables in the script object
   $DeckTool::currentDeckEdit.faceStyle = %styleSelected;
   
}

/// <summary>
/// Refreshes the PreviewWindow to display what the selected card looks like
/// </summary>
function CustomDeckToolGui::refreshPreviewWindow(%this)
{
   // don't refresh before we're ready...
   if (!PreviewWindow-->CardFrontBackground.isMethod("setBitmap"))
      return;    
    
   //set the card front background
   PreviewWindow-->CardFrontBackground.setBitmap(CardFrontPreview.bitmap);
   
  
   switch$($DeckTool::cardValue)
   {
      case "A":
      
         if($DeckTool::currentDeckEdit.faceStyle $= "full")
            setPipPattern(PreviewWindow, "default");
         else
            setPipPattern(PreviewWindow, "A");   
         
         setFaceStyle(PreviewWindow, $DeckTool::currentDeckEdit.faceStyle);
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(PreviewWindow, AceSpadesPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, TopASpade.bitmap);
               setBottomValueImage(PreviewWindow, TopASpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, TopASpade.bitmap);
               setBottomValueImage(PreviewWindow, BottomASpade.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, BottomSpade.bitmap);
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(PreviewWindow, AceClubsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, TopAClub.bitmap);
               setBottomValueImage(PreviewWindow, TopAClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, TopAClub.bitmap);
               setBottomValueImage(PreviewWindow, BottomAClub.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, BottomClub.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(PreviewWindow, AceDiamondsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, TopADiamond.bitmap);
               setBottomValueImage(PreviewWindow, TopADiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, TopADiamond.bitmap);
               setBottomValueImage(PreviewWindow, BottomADiamond.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, BottomDiamond.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(PreviewWindow, AceHeartsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, TopAHeart.bitmap);
               setBottomValueImage(PreviewWindow, TopAHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, TopAHeart.bitmap);
               setBottomValueImage(PreviewWindow, BottomAHeart.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, BottomHeart.bitmap);
            }
         }
      
      case "2":
         if($DeckTool::currentDeckEdit.faceStyle $= "full" && $DeckTool::currentDeckEdit.allFacesSpecified)
            setPipPattern(PreviewWindow, "default");
         else
            setPipPattern(PreviewWindow, "2");   
         
         if($DeckTool::currentDeckEdit.allFacesSpecified)
            setFaceStyle(PreviewWindow, $DeckTool::currentDeckEdit.faceStyle);
         else
            setFaceStyle(PreviewWindow, "default");   
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(PreviewWindow, TwoSpadesPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top2Spade.bitmap);
               setBottomValueImage(PreviewWindow, Top2Spade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top2Spade.bitmap);
               setBottomValueImage(PreviewWindow, Bottom2Spade.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, BottomSpade.bitmap);
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(PreviewWindow, TwoClubsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top2Club.bitmap);
               setBottomValueImage(PreviewWindow, Top2Club.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top2Club.bitmap);
               setBottomValueImage(PreviewWindow, Bottom2Club.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, BottomClub.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(PreviewWindow, TwoDiamondsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top2Diamond.bitmap);
               setBottomValueImage(PreviewWindow, Top2Diamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top2Diamond.bitmap);
               setBottomValueImage(PreviewWindow, Bottom2Diamond.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, BottomDiamond.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(PreviewWindow, TwoHeartsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top2Heart.bitmap);
               setBottomValueImage(PreviewWindow, Top2Heart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top2Heart.bitmap);
               setBottomValueImage(PreviewWindow, Bottom2Heart.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, BottomHeart.bitmap);
            }
         }
         
      
      case "3":
         if($DeckTool::currentDeckEdit.faceStyle $= "full" && $DeckTool::currentDeckEdit.allFacesSpecified)
            setPipPattern(PreviewWindow, "default");
         else
            setPipPattern(PreviewWindow, "3");   
         
         if($DeckTool::currentDeckEdit.allFacesSpecified)
            setFaceStyle(PreviewWindow, $DeckTool::currentDeckEdit.faceStyle);
         else
            setFaceStyle(PreviewWindow, "default");
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(PreviewWindow, ThreeSpadesPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top3Spade.bitmap);
               setBottomValueImage(PreviewWindow, Top3Spade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top3Spade.bitmap);
               setBottomValueImage(PreviewWindow, Bottom3Spade.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, BottomSpade.bitmap);
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(PreviewWindow, ThreeClubsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top3Club.bitmap);
               setBottomValueImage(PreviewWindow, Top3Club.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top3Club.bitmap);
               setBottomValueImage(PreviewWindow, Bottom3Club.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, BottomClub.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(PreviewWindow, ThreeDiamondsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top3Diamond.bitmap);
               setBottomValueImage(PreviewWindow, Top3Diamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top3Diamond.bitmap);
               setBottomValueImage(PreviewWindow, Bottom3Diamond.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, BottomDiamond.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(PreviewWindow, ThreeHeartsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top3Heart.bitmap);
               setBottomValueImage(PreviewWindow, Top3Heart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top3Heart.bitmap);
               setBottomValueImage(PreviewWindow, Bottom3Heart.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, BottomHeart.bitmap);
            }
         }
         
      
      case "4":
         if($DeckTool::currentDeckEdit.faceStyle $= "full" && $DeckTool::currentDeckEdit.allFacesSpecified)
            setPipPattern(PreviewWindow, "default");
         else
            setPipPattern(PreviewWindow, "4");   
         
         if($DeckTool::currentDeckEdit.allFacesSpecified)
            setFaceStyle(PreviewWindow, $DeckTool::currentDeckEdit.faceStyle);
         else
            setFaceStyle(PreviewWindow, "default");
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(PreviewWindow, FourSpadesPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top4Spade.bitmap);
               setBottomValueImage(PreviewWindow, Top4Spade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top4Spade.bitmap);
               setBottomValueImage(PreviewWindow, Bottom4Spade.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, BottomSpade.bitmap);
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(PreviewWindow, FourClubsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top4Club.bitmap);
               setBottomValueImage(PreviewWindow, Top4Club.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top4Club.bitmap);
               setBottomValueImage(PreviewWindow, Bottom4Club.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, BottomClub.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(PreviewWindow, FourDiamondsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top4Diamond.bitmap);
               setBottomValueImage(PreviewWindow, Top4Diamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top4Diamond.bitmap);
               setBottomValueImage(PreviewWindow, Bottom4Diamond.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, BottomDiamond.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(PreviewWindow, FourHeartsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top4Heart.bitmap);
               setBottomValueImage(PreviewWindow, Top4Heart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top4Heart.bitmap);
               setBottomValueImage(PreviewWindow, Bottom4Heart.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, BottomHeart.bitmap);
            }
         }
      
      case "5":
         if($DeckTool::currentDeckEdit.faceStyle $= "full" && $DeckTool::currentDeckEdit.allFacesSpecified)
            setPipPattern(PreviewWindow, "default");
         else
            setPipPattern(PreviewWindow, "5");   
         
         if($DeckTool::currentDeckEdit.allFacesSpecified)
            setFaceStyle(PreviewWindow, $DeckTool::currentDeckEdit.faceStyle);
         else
            setFaceStyle(PreviewWindow, "default");
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(PreviewWindow, FiveSpadesPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top5Spade.bitmap);
               setBottomValueImage(PreviewWindow, Top5Spade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top5Spade.bitmap);
               setBottomValueImage(PreviewWindow, Bottom5Spade.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, BottomSpade.bitmap);
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(PreviewWindow, FiveClubsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top5Club.bitmap);
               setBottomValueImage(PreviewWindow, Top5Club.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top5Club.bitmap);
               setBottomValueImage(PreviewWindow, Bottom5Club.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, BottomClub.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(PreviewWindow, FiveDiamondsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top5Diamond.bitmap);
               setBottomValueImage(PreviewWindow, Top5Diamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top5Diamond.bitmap);
               setBottomValueImage(PreviewWindow, Bottom5Diamond.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, BottomDiamond.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(PreviewWindow, TwoHeartsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top5Heart.bitmap);
               setBottomValueImage(PreviewWindow, Top5Heart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top5Heart.bitmap);
               setBottomValueImage(PreviewWindow, Bottom5Heart.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, BottomHeart.bitmap);
            }
         }
         
      
      case "6":
         if($DeckTool::currentDeckEdit.faceStyle $= "full" && $DeckTool::currentDeckEdit.allFacesSpecified)
            setPipPattern(PreviewWindow, "default");
         else
            setPipPattern(PreviewWindow, "6");   
         
         if($DeckTool::currentDeckEdit.allFacesSpecified)
            setFaceStyle(PreviewWindow, $DeckTool::currentDeckEdit.faceStyle);
         else
            setFaceStyle(PreviewWindow, "default");
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(PreviewWindow, SixSpadesPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top6Spade.bitmap);
               setBottomValueImage(PreviewWindow, Top6Spade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top6Spade.bitmap);
               setBottomValueImage(PreviewWindow, Bottom6Spade.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, BottomSpade.bitmap);
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(PreviewWindow, SixClubsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top6Club.bitmap);
               setBottomValueImage(PreviewWindow, Top6Club.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top6Club.bitmap);
               setBottomValueImage(PreviewWindow, Bottom6Club.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, BottomClub.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(PreviewWindow, SixDiamondsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top6Diamond.bitmap);
               setBottomValueImage(PreviewWindow, Top6Diamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top6Diamond.bitmap);
               setBottomValueImage(PreviewWindow, Bottom6Diamond.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, BottomDiamond.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(PreviewWindow, SixClubsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top6Heart.bitmap);
               setBottomValueImage(PreviewWindow, Top6Heart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top6Heart.bitmap);
               setBottomValueImage(PreviewWindow, Bottom6Heart.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, BottomHeart.bitmap);
            }
         }
         
      
      case "7":
         if($DeckTool::currentDeckEdit.faceStyle $= "full" && $DeckTool::currentDeckEdit.allFacesSpecified)
            setPipPattern(PreviewWindow, "default");
         else
            setPipPattern(PreviewWindow, "7");   
         
         if($DeckTool::currentDeckEdit.allFacesSpecified)
            setFaceStyle(PreviewWindow, $DeckTool::currentDeckEdit.faceStyle);
         else
            setFaceStyle(PreviewWindow, "default");
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(PreviewWindow, SevenSpadesPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top7Spade.bitmap);
               setBottomValueImage(PreviewWindow, Top7Spade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top7Spade.bitmap);
               setBottomValueImage(PreviewWindow, Bottom7Spade.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, BottomSpade.bitmap);
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(PreviewWindow, SevenClubsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top7Club.bitmap);
               setBottomValueImage(PreviewWindow, Top7Club.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top7Club.bitmap);
               setBottomValueImage(PreviewWindow, Bottom7Club.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, BottomClub.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(PreviewWindow, SevenDiamondsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top7Diamond.bitmap);
               setBottomValueImage(PreviewWindow, Top7Diamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top7Diamond.bitmap);
               setBottomValueImage(PreviewWindow, Bottom7Diamond.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, BottomDiamond.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(PreviewWindow, SevenHeartsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top7Heart.bitmap);
               setBottomValueImage(PreviewWindow, Top7Heart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top7Heart.bitmap);
               setBottomValueImage(PreviewWindow, Bottom7Heart.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, BottomHeart.bitmap);
            }
         }
         
      
      case "8":
         if($DeckTool::currentDeckEdit.faceStyle $= "full" && $DeckTool::currentDeckEdit.allFacesSpecified)
            setPipPattern(PreviewWindow, "default");
         else
            setPipPattern(PreviewWindow, "8");   
         
         if($DeckTool::currentDeckEdit.allFacesSpecified)
            setFaceStyle(PreviewWindow, $DeckTool::currentDeckEdit.faceStyle);
         else
            setFaceStyle(PreviewWindow, "default");
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(PreviewWindow, EightSpadesPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top8Spade.bitmap);
               setBottomValueImage(PreviewWindow, Top8Spade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top8Spade.bitmap);
               setBottomValueImage(PreviewWindow, Bottom8Spade.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, BottomSpade.bitmap);
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(PreviewWindow, EightClubsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top8Club.bitmap);
               setBottomValueImage(PreviewWindow, Top8Club.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top8Club.bitmap);
               setBottomValueImage(PreviewWindow, Bottom8Club.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, BottomClub.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(PreviewWindow, EightDiamondsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top8Diamond.bitmap);
               setBottomValueImage(PreviewWindow, Top8Diamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top8Diamond.bitmap);
               setBottomValueImage(PreviewWindow, Bottom8Diamond.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, BottomDiamond.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(PreviewWindow, EightHeartsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top8Heart.bitmap);
               setBottomValueImage(PreviewWindow, Top8Heart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top8Heart.bitmap);
               setBottomValueImage(PreviewWindow, Bottom8Heart.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, BottomHeart.bitmap);
            }
         }
         
      
      case "9":
         if($DeckTool::currentDeckEdit.faceStyle $= "full" && $DeckTool::currentDeckEdit.allFacesSpecified)
            setPipPattern(PreviewWindow, "default");
         else
            setPipPattern(PreviewWindow, "9");   
         
         if($DeckTool::currentDeckEdit.allFacesSpecified)
            setFaceStyle(PreviewWindow, $DeckTool::currentDeckEdit.faceStyle);
         else
            setFaceStyle(PreviewWindow, "default");
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(PreviewWindow, NineSpadesPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top9Spade.bitmap);
               setBottomValueImage(PreviewWindow, Top9Spade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top9Spade.bitmap);
               setBottomValueImage(PreviewWindow, Bottom9Spade.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, BottomSpade.bitmap);
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(PreviewWindow, NineClubsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top9Club.bitmap);
               setBottomValueImage(PreviewWindow, Top9Club.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top9Club.bitmap);
               setBottomValueImage(PreviewWindow, Bottom9Club.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, BottomClub.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(PreviewWindow, NineDiamondsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top9Diamond.bitmap);
               setBottomValueImage(PreviewWindow, Top9Diamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top9Diamond.bitmap);
               setBottomValueImage(PreviewWindow, Bottom9Diamond.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, BottomDiamond.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(PreviewWindow, NineHeartsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top9Heart.bitmap);
               setBottomValueImage(PreviewWindow, Top9Heart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top9Heart.bitmap);
               setBottomValueImage(PreviewWindow, Bottom9Heart.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, BottomHeart.bitmap);
            }
         }
         
      
      case "10":
         if($DeckTool::currentDeckEdit.faceStyle $= "full" && $DeckTool::currentDeckEdit.allFacesSpecified)
            setPipPattern(PreviewWindow, "default");
         else
            setPipPattern(PreviewWindow, "10");   
         
         if($DeckTool::currentDeckEdit.allFacesSpecified)
            setFaceStyle(PreviewWindow, $DeckTool::currentDeckEdit.faceStyle);
         else
            setFaceStyle(PreviewWindow, "default");
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(PreviewWindow, TenSpadesPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top10Spade.bitmap);
               setBottomValueImage(PreviewWindow, Top10Spade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top10Spade.bitmap);
               setBottomValueImage(PreviewWindow, Bottom10Spade.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, BottomSpade.bitmap);
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(PreviewWindow, TenClubsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top10Club.bitmap);
               setBottomValueImage(PreviewWindow, Top10Club.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top10Club.bitmap);
               setBottomValueImage(PreviewWindow, Bottom10Club.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, BottomClub.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(PreviewWindow, TenDiamondsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top10Diamond.bitmap);
               setBottomValueImage(PreviewWindow, Top10Diamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top10Diamond.bitmap);
               setBottomValueImage(PreviewWindow, Bottom10Diamond.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, BottomDiamond.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(PreviewWindow, TenHeartsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, Top10Heart.bitmap);
               setBottomValueImage(PreviewWindow, Top10Heart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, Top10Heart.bitmap);
               setBottomValueImage(PreviewWindow, Bottom10Heart.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, BottomHeart.bitmap);
            }
         }
         
      
      case "J":
         if($DeckTool::currentDeckEdit.faceStyle $= "full")
            setPipPattern(PreviewWindow, "default");
         else
            setPipPattern(PreviewWindow, "J");   
         
         setFaceStyle(PreviewWindow, $DeckTool::currentDeckEdit.faceStyle);
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(PreviewWindow, JackSpadesPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, TopJSpade.bitmap);
               setBottomValueImage(PreviewWindow, TopJSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, TopJSpade.bitmap);
               setBottomValueImage(PreviewWindow, BottomJSpade.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, BottomSpade.bitmap);
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(PreviewWindow, JackClubsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, TopJClub.bitmap);
               setBottomValueImage(PreviewWindow, TopJClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, TopJClub.bitmap);
               setBottomValueImage(PreviewWindow, BottomJClub.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, BottomClub.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(PreviewWindow, JackDiamondsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, TopJDiamond.bitmap);
               setBottomValueImage(PreviewWindow, TopJDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, TopJDiamond.bitmap);
               setBottomValueImage(PreviewWindow, BottomJDiamond.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, BottomDiamond.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(PreviewWindow, JackHeartsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, TopJHeart.bitmap);
               setBottomValueImage(PreviewWindow, TopJHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, TopJHeart.bitmap);
               setBottomValueImage(PreviewWindow, BottomJHeart.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, BottomHeart.bitmap);
            }
         }
         
      
      case "Q":
         if($DeckTool::currentDeckEdit.faceStyle $= "full")
            setPipPattern(PreviewWindow, "default");
         else
            setPipPattern(PreviewWindow, "Q");   
         
         setFaceStyle(PreviewWindow, $DeckTool::currentDeckEdit.faceStyle);
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(PreviewWindow, QueenSpadesPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, TopQSpade.bitmap);
               setBottomValueImage(PreviewWindow, TopQSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, TopQSpade.bitmap);
               setBottomValueImage(PreviewWindow, BottomQSpade.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, BottomSpade.bitmap);
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(PreviewWindow, QueenClubsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, TopQClub.bitmap);
               setBottomValueImage(PreviewWindow, TopQClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, TopQClub.bitmap);
               setBottomValueImage(PreviewWindow, BottomQClub.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, BottomClub.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(PreviewWindow, QueenDiamondsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, TopQDiamond.bitmap);
               setBottomValueImage(PreviewWindow, TopQDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, TopQDiamond.bitmap);
               setBottomValueImage(PreviewWindow, BottomQDiamond.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, BottomDiamond.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(PreviewWindow, QueenHeartsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, TopQHeart.bitmap);
               setBottomValueImage(PreviewWindow, TopQHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, TopQHeart.bitmap);
               setBottomValueImage(PreviewWindow, BottomQHeart.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, BottomHeart.bitmap);
            }
         }
         
      
      case "K":
         if($DeckTool::currentDeckEdit.faceStyle $= "full")
            setPipPattern(PreviewWindow, "default");
         else
            setPipPattern(PreviewWindow, "K");   
         
         setFaceStyle(PreviewWindow, $DeckTool::currentDeckEdit.faceStyle);
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(PreviewWindow, KingSpadesPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, TopKSpade.bitmap);
               setBottomValueImage(PreviewWindow, TopKSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, TopKSpade.bitmap);
               setBottomValueImage(PreviewWindow, BottomKSpade.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, TopSpade.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopSpade.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceSpade.bitmap);
               setBottomPipImage(PreviewWindow, BottomSpade.bitmap);
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(PreviewWindow, KingClubsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, TopKClub.bitmap);
               setBottomValueImage(PreviewWindow, TopKClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, TopKClub.bitmap);
               setBottomValueImage(PreviewWindow, BottomKClub.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, TopClub.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopClub.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceClub.bitmap);
               setBottomPipImage(PreviewWindow, BottomClub.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(PreviewWindow, KingDiamondsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, TopKDiamond.bitmap);
               setBottomValueImage(PreviewWindow, TopKDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, TopKDiamond.bitmap);
               setBottomValueImage(PreviewWindow, BottomKDiamond.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, TopDiamond.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopDiamond.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceDiamond.bitmap);
               setBottomPipImage(PreviewWindow, BottomDiamond.bitmap);
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(PreviewWindow, KingHeartsPreview.bitmap);
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(PreviewWindow, TopKHeart.bitmap);
               setBottomValueImage(PreviewWindow, TopKHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(PreviewWindow, TopKHeart.bitmap);
               setBottomValueImage(PreviewWindow, BottomKHeart.bitmap);
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, TopHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, TopHeart.bitmap);
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(PreviewWindow, TopHeart.bitmap);
               setFacePipImages(PreviewWindow, $DeckTool::cardSuit, FaceHeart.bitmap);
               setBottomPipImage(PreviewWindow, BottomHeart.bitmap);
            }
         }
         
      default:
         error("$DeckTool::cardValue set to an invalid value.  $DeckTool::cardValue = " @ $DeckTool::cardValue);
         
   }
}

/// <summary>
/// Sets the face style for use in a preview window.
/// This manipulates which components to display and which to hide.
/// </summary>
/// <param="%cardPreview">The GuiCtrl container that is the preview window to update</param>
/// <param="%style">The style to set the preview window to display</param>
function setFaceStyle(%cardPreview, %style)
{
   //echo("SETTING FACE STYLE: " @ %style);
   
   switch$(%style)
   {
      case "standard":
         %cardPreview-->FaceImageStandard.setVisible(true);
         %cardPreview-->FaceImageTall.setVisible(false);
         %cardPreview-->FaceImageFull.setVisible(false);
         %cardPreview-->LeftCornerPip.setVisible(true);
         %cardPreview-->LeftCornerValue.setVisible(true);
         %cardPreview-->RightCornerPip.setVisible(true);
         %cardPreview-->RightCornerValue.setVisible(true);
         
      case "tall":
         %cardPreview-->FaceImageStandard.setVisible(false);
         %cardPreview-->FaceImageTall.setVisible(true);
         %cardPreview-->FaceImageFull.setVisible(false);
         %cardPreview-->LeftCornerPip.setVisible(true);
         %cardPreview-->LeftCornerValue.setVisible(true);
         %cardPreview-->RightCornerPip.setVisible(true);
         %cardPreview-->RightCornerValue.setVisible(true);
         
      case "full":
         %cardPreview-->FaceImageStandard.setVisible(false);
         %cardPreview-->FaceImageTall.setVisible(false);
         %cardPreview-->FaceImageFull.setVisible(true);
         %cardPreview-->LeftCornerPip.setVisible(false);
         %cardPreview-->LeftCornerValue.setVisible(false);
         %cardPreview-->RightCornerPip.setVisible(false);
         %cardPreview-->RightCornerValue.setVisible(false);
         
      default:
         %cardPreview-->FaceImageStandard.setVisible(false);
         %cardPreview-->FaceImageTall.setVisible(false);
         %cardPreview-->FaceImageFull.setVisible(false);
         %cardPreview-->LeftCornerPip.setVisible(true);
         %cardPreview-->LeftCornerValue.setVisible(true);
         %cardPreview-->RightCornerPip.setVisible(true);
         %cardPreview-->RightCornerValue.setVisible(true);   
   }
}


function setCardFaceImage(%cardPreview, %source)
{
   %cardPreview-->FaceImageStandard.setBitmap(%source);
   %cardPreview-->FaceImageTall.setBitmap(%source);
   %cardPreview-->FaceImageFull.setBitmap(%source);
}

function setCardBackImage(%cardPreview, %source)
{
   setPipPattern(%cardPreview, "default");
   
   %cardPreview-->RightCornerPip.setVisible(false);
   %cardPreview-->RightCornerValue.setVisible(false);
   %cardPreview-->LeftCornerPip.setVisible(false);
   %cardPreview-->LeftCornerValue.setVisible(false);
   
   %cardPreview-->CardCardFrontBackground.setBitmap(%source);
}

function setFacePipImages(%cardPreview, %suit, %source)
{
   %cardPreview-->Pip00.setBitmap(%source);
   %cardPreview-->Pip01.setBitmap(%source);
   %cardPreview-->Pip02.setBitmap(%source);
   %cardPreview-->Pip03.setBitmap(%source);
   %cardPreview-->Pip04.setBitmap(%source);
   %cardPreview-->Pip10.setBitmap(%source);
   %cardPreview-->Pip11.setBitmap(%source);
   %cardPreview-->Pip12.setBitmap(%source);
   %cardPreview-->Pip13.setBitmap(%source);
   %cardPreview-->Pip14.setBitmap(%source);
   %cardPreview-->Pip20.setBitmap(%source);
   %cardPreview-->Pip21.setBitmap(%source);
   %cardPreview-->Pip22.setBitmap(%source);
   %cardPreview-->Pip23.setBitmap(%source);
   %cardPreview-->Pip24.setBitmap(%source);
}

function setTopPipImage(%cardPreview, %source)
{
   %cardPreview-->LeftCornerPip.setBitmap(%source);
}

function setBottomPipImage(%cardPreview, %source)
{
   %cardPreview-->RightCornerPip.setBitmap(%source);
}

function setTopValueImage(%cardPreview, %source)
{
   %cardPreview-->LeftCornerValue.setBitmap(%source);
}

function setBottomValueImage(%cardPreview, %source)
{
   %cardPreview-->RightCornerValue.setBitmap(%source);
}

function setPipPattern(%cardPreview, %value)
{
   switch$(%value)
   {
      case "A":
         %cardPreview-->Pip00.setVisible(false);
         %cardPreview-->Pip01.setVisible(false);
         %cardPreview-->Pip02.setVisible(false);
         %cardPreview-->Pip03.setVisible(false);
         %cardPreview-->Pip04.setVisible(false);
         %cardPreview-->Pip10.setVisible(false);
         %cardPreview-->Pip11.setVisible(false);
         %cardPreview-->Pip12.setVisible(false);
         %cardPreview-->Pip13.setVisible(false);
         %cardPreview-->Pip14.setVisible(false);
         %cardPreview-->Pip20.setVisible(false);
         %cardPreview-->Pip21.setVisible(false);
         %cardPreview-->Pip22.setVisible(false);
         %cardPreview-->Pip23.setVisible(false);
         %cardPreview-->Pip24.setVisible(false);
      
      case "2":
         %cardPreview-->Pip00.setVisible(false);
         %cardPreview-->Pip01.setVisible(false);
         %cardPreview-->Pip02.setVisible(false);
         %cardPreview-->Pip03.setVisible(false);
         %cardPreview-->Pip04.setVisible(false);
         %cardPreview-->Pip10.setVisible(true);
         %cardPreview-->Pip11.setVisible(false);
         %cardPreview-->Pip12.setVisible(false);
         %cardPreview-->Pip13.setVisible(false);
         %cardPreview-->Pip14.setVisible(true);
         %cardPreview-->Pip20.setVisible(false);
         %cardPreview-->Pip21.setVisible(false);
         %cardPreview-->Pip22.setVisible(false);
         %cardPreview-->Pip23.setVisible(false);
         %cardPreview-->Pip24.setVisible(false);
      
      case "3":
         %cardPreview-->Pip00.setVisible(false);
         %cardPreview-->Pip01.setVisible(false);
         %cardPreview-->Pip02.setVisible(false);
         %cardPreview-->Pip03.setVisible(false);
         %cardPreview-->Pip04.setVisible(false);
         %cardPreview-->Pip10.setVisible(true);
         %cardPreview-->Pip11.setVisible(false);
         %cardPreview-->Pip12.setVisible(true);
         %cardPreview-->Pip13.setVisible(false);
         %cardPreview-->Pip14.setVisible(true);
         %cardPreview-->Pip20.setVisible(false);
         %cardPreview-->Pip21.setVisible(false);
         %cardPreview-->Pip22.setVisible(false);
         %cardPreview-->Pip23.setVisible(false);
         %cardPreview-->Pip24.setVisible(false);
      
      case "4":
         %cardPreview-->Pip00.setVisible(true);
         %cardPreview-->Pip01.setVisible(false);
         %cardPreview-->Pip02.setVisible(false);
         %cardPreview-->Pip03.setVisible(false);
         %cardPreview-->Pip04.setVisible(true);
         %cardPreview-->Pip10.setVisible(false);
         %cardPreview-->Pip11.setVisible(false);
         %cardPreview-->Pip12.setVisible(false);
         %cardPreview-->Pip13.setVisible(false);
         %cardPreview-->Pip14.setVisible(false);
         %cardPreview-->Pip20.setVisible(true);
         %cardPreview-->Pip21.setVisible(false);
         %cardPreview-->Pip22.setVisible(false);
         %cardPreview-->Pip23.setVisible(false);
         %cardPreview-->Pip24.setVisible(true);
      
      case "5":
         %cardPreview-->Pip00.setVisible(true);
         %cardPreview-->Pip01.setVisible(false);
         %cardPreview-->Pip02.setVisible(false);
         %cardPreview-->Pip03.setVisible(false);
         %cardPreview-->Pip04.setVisible(true);
         %cardPreview-->Pip10.setVisible(false);
         %cardPreview-->Pip11.setVisible(false);
         %cardPreview-->Pip12.setVisible(true);
         %cardPreview-->Pip13.setVisible(false);
         %cardPreview-->Pip14.setVisible(false);
         %cardPreview-->Pip20.setVisible(true);
         %cardPreview-->Pip21.setVisible(false);
         %cardPreview-->Pip22.setVisible(false);
         %cardPreview-->Pip23.setVisible(false);
         %cardPreview-->Pip24.setVisible(true);
      
      case "6":
         %cardPreview-->Pip00.setVisible(true);
         %cardPreview-->Pip01.setVisible(false);
         %cardPreview-->Pip02.setVisible(true);
         %cardPreview-->Pip03.setVisible(false);
         %cardPreview-->Pip04.setVisible(true);
         %cardPreview-->Pip10.setVisible(false);
         %cardPreview-->Pip11.setVisible(false);
         %cardPreview-->Pip12.setVisible(false);
         %cardPreview-->Pip13.setVisible(false);
         %cardPreview-->Pip14.setVisible(false);
         %cardPreview-->Pip20.setVisible(true);
         %cardPreview-->Pip21.setVisible(false);
         %cardPreview-->Pip22.setVisible(true);
         %cardPreview-->Pip23.setVisible(false);
         %cardPreview-->Pip24.setVisible(true);
      
      case "7":
         %cardPreview-->Pip00.setVisible(true);
         %cardPreview-->Pip01.setVisible(false);
         %cardPreview-->Pip02.setVisible(true);
         %cardPreview-->Pip03.setVisible(false);
         %cardPreview-->Pip04.setVisible(true);
         %cardPreview-->Pip10.setVisible(false);
         %cardPreview-->Pip11.setVisible(true);
         %cardPreview-->Pip12.setVisible(false);
         %cardPreview-->Pip13.setVisible(false);
         %cardPreview-->Pip14.setVisible(false);
         %cardPreview-->Pip20.setVisible(true);
         %cardPreview-->Pip21.setVisible(false);
         %cardPreview-->Pip22.setVisible(true);
         %cardPreview-->Pip23.setVisible(false);
         %cardPreview-->Pip24.setVisible(true);
      
      case "8":
         %cardPreview-->Pip00.setVisible(true);
         %cardPreview-->Pip01.setVisible(false);
         %cardPreview-->Pip02.setVisible(true);
         %cardPreview-->Pip03.setVisible(false);
         %cardPreview-->Pip04.setVisible(true);
         %cardPreview-->Pip10.setVisible(false);
         %cardPreview-->Pip11.setVisible(true);
         %cardPreview-->Pip12.setVisible(false);
         %cardPreview-->Pip13.setVisible(true);
         %cardPreview-->Pip14.setVisible(false);
         %cardPreview-->Pip20.setVisible(true);
         %cardPreview-->Pip21.setVisible(false);
         %cardPreview-->Pip22.setVisible(true);
         %cardPreview-->Pip23.setVisible(false);
         %cardPreview-->Pip24.setVisible(true);
      
      case "9":
         %cardPreview-->Pip00.setVisible(true);
         %cardPreview-->Pip01.setVisible(true);
         %cardPreview-->Pip02.setVisible(false);
         %cardPreview-->Pip03.setVisible(true);
         %cardPreview-->Pip04.setVisible(true);
         %cardPreview-->Pip10.setVisible(false);
         %cardPreview-->Pip11.setVisible(false);
         %cardPreview-->Pip12.setVisible(true);
         %cardPreview-->Pip13.setVisible(false);
         %cardPreview-->Pip14.setVisible(false);
         %cardPreview-->Pip20.setVisible(true);
         %cardPreview-->Pip21.setVisible(true);
         %cardPreview-->Pip22.setVisible(false);
         %cardPreview-->Pip23.setVisible(true);
         %cardPreview-->Pip24.setVisible(true);
      
      case "10":
         %cardPreview-->Pip00.setVisible(true);
         %cardPreview-->Pip01.setVisible(true);
         %cardPreview-->Pip02.setVisible(false);
         %cardPreview-->Pip03.setVisible(true);
         %cardPreview-->Pip04.setVisible(true);
         %cardPreview-->Pip10.setVisible(false);
         %cardPreview-->Pip11.setVisible(true);
         %cardPreview-->Pip12.setVisible(false);
         %cardPreview-->Pip13.setVisible(true);
         %cardPreview-->Pip14.setVisible(false);
         %cardPreview-->Pip20.setVisible(true);
         %cardPreview-->Pip21.setVisible(true);
         %cardPreview-->Pip22.setVisible(false);
         %cardPreview-->Pip23.setVisible(true);
         %cardPreview-->Pip24.setVisible(true);
      
      case "J":
         %cardPreview-->Pip00.setVisible(false);
         %cardPreview-->Pip01.setVisible(false);
         %cardPreview-->Pip02.setVisible(false);
         %cardPreview-->Pip03.setVisible(false);
         %cardPreview-->Pip04.setVisible(false);
         %cardPreview-->Pip10.setVisible(false);
         %cardPreview-->Pip11.setVisible(false);
         %cardPreview-->Pip12.setVisible(false);
         %cardPreview-->Pip13.setVisible(false);
         %cardPreview-->Pip14.setVisible(false);
         %cardPreview-->Pip20.setVisible(false);
         %cardPreview-->Pip21.setVisible(false);
         %cardPreview-->Pip22.setVisible(false);
         %cardPreview-->Pip23.setVisible(false);
         %cardPreview-->Pip24.setVisible(false);
      
      case "Q":
         %cardPreview-->Pip00.setVisible(false);
         %cardPreview-->Pip01.setVisible(false);
         %cardPreview-->Pip02.setVisible(false);
         %cardPreview-->Pip03.setVisible(false);
         %cardPreview-->Pip04.setVisible(false);
         %cardPreview-->Pip10.setVisible(false);
         %cardPreview-->Pip11.setVisible(false);
         %cardPreview-->Pip12.setVisible(false);
         %cardPreview-->Pip13.setVisible(false);
         %cardPreview-->Pip14.setVisible(false);
         %cardPreview-->Pip20.setVisible(false);
         %cardPreview-->Pip21.setVisible(false);
         %cardPreview-->Pip22.setVisible(false);
         %cardPreview-->Pip23.setVisible(false);
         %cardPreview-->Pip24.setVisible(false);
      
      case "K":
         %cardPreview-->Pip00.setVisible(false);
         %cardPreview-->Pip01.setVisible(false);
         %cardPreview-->Pip02.setVisible(false);
         %cardPreview-->Pip03.setVisible(false);
         %cardPreview-->Pip04.setVisible(false);
         %cardPreview-->Pip10.setVisible(false);
         %cardPreview-->Pip11.setVisible(false);
         %cardPreview-->Pip12.setVisible(false);
         %cardPreview-->Pip13.setVisible(false);
         %cardPreview-->Pip14.setVisible(false);
         %cardPreview-->Pip20.setVisible(false);
         %cardPreview-->Pip21.setVisible(false);
         %cardPreview-->Pip22.setVisible(false);
         %cardPreview-->Pip23.setVisible(false);
         %cardPreview-->Pip24.setVisible(false);
      
      default:
         %cardPreview-->Pip00.setVisible(false);
         %cardPreview-->Pip01.setVisible(false);
         %cardPreview-->Pip02.setVisible(false);
         %cardPreview-->Pip03.setVisible(false);
         %cardPreview-->Pip04.setVisible(false);
         %cardPreview-->Pip10.setVisible(false);
         %cardPreview-->Pip11.setVisible(false);
         %cardPreview-->Pip12.setVisible(false);
         %cardPreview-->Pip13.setVisible(false);
         %cardPreview-->Pip14.setVisible(false);
         %cardPreview-->Pip20.setVisible(false);
         %cardPreview-->Pip21.setVisible(false);
         %cardPreview-->Pip22.setVisible(false);
         %cardPreview-->Pip23.setVisible(false);
         %cardPreview-->Pip24.setVisible(false);
   }
}

function CustomDeckToolGui::selectFaceEdit(%this, %editAllFaces)
{
   //manipulate extent of Royals window
   if(%editAllFaces)
      Royals.setExtent($GuiCardRowWidth, 13*$GuiCardRowHeight);
   else
      Royals.setExtent($GuiCardRowWidth, 4*$GuiCardRowHeight);
      
      
   $DeckTool::currentDeckEdit.allFacesSpecified = %editAllFaces;
}

function CustomDeckToolGui::selectValueStyle(%this, %style)
{
   switch$(%style)
   {
      case "standard":
         BottomValuesContainer.setVisible(false);
         
      case "different":
         BottomValuesContainer.setVisible(true);
   }
   
   $DeckTool::currentDeckEdit.valueStyle = %style;
}

function CustomDeckToolGui::selectPipStyle(%this, %pipStyle)
{
   
   
   switch$(%pipStyle)
   {
      case "standard":
         TopPips.setVisible(true);
         BottomPips.setVisible(false);
         BottomPipsLable.setVisible(false);
         FacePips.setVisible(false);
         FacePipsLable.setVisible(false);
         
      case "face":
         TopPips.setVisible(true);
         BottomPips.setVisible(false);
         BottomPipsLable.setVisible(false);
         FacePips.setVisible(true);
         FacePipsLable.setVisible(true);
      
      case "all":
         TopPips.setVisible(true);
         BottomPips.setVisible(true);
         BottomPipsLable.setVisible(true);
         FacePips.setVisible(true);
         FacePipsLable.setVisible(true);
   }
   
   $DeckTool::currentDeckEdit.pipStyle = %pipStyle;
}

function CustomDeckToolGui::selectCardSuit(%this, %suit)
{
   $DeckTool::cardSuit = %suit;
   %this.refreshPreviewWindow();
}

function CustomDeckToolGui::selectCardValue(%this, %value)
{
   $DeckTool::cardValue = %value;
   %this.refreshPreviewWindow();
}

function CustomDeckToolGui::toggleValueSuits(%this, %selectedSuit)
{
   switch$(%selectedSuit)
   {
      case "S":
            TopValuesSpade.setVisible(true);
            TopValuesClub.setVisible(false);
            TopValuesDiamond.setVisible(false);
            TopValuesHeart.setVisible(false);
            
            BottomValuesSpade.setVisible(true);
            BottomValuesClub.setVisible(false);
            BottomValuesDiamond.setVisible(false);
            BottomValuesHeart.setVisible(false);
         
      case "C":
            TopValuesSpade.setVisible(false);
            TopValuesClub.setVisible(true);
            TopValuesDiamond.setVisible(false);
            TopValuesHeart.setVisible(false);
            
            BottomValuesSpade.setVisible(false);
            BottomValuesClub.setVisible(true);
            BottomValuesDiamond.setVisible(false);
            BottomValuesHeart.setVisible(false);
         
      case "D":
            TopValuesSpade.setVisible(false);
            TopValuesClub.setVisible(false);
            TopValuesDiamond.setVisible(true);
            TopValuesHeart.setVisible(false);
            
            BottomValuesSpade.setVisible(false);
            BottomValuesClub.setVisible(false);
            BottomValuesDiamond.setVisible(true);
            BottomValuesHeart.setVisible(false);
         
      case "H":
            TopValuesSpade.setVisible(false);
            TopValuesClub.setVisible(false);
            TopValuesDiamond.setVisible(false);
            TopValuesHeart.setVisible(true);
            
            BottomValuesSpade.setVisible(false);
            BottomValuesClub.setVisible(false);
            BottomValuesDiamond.setVisible(false);
            BottomValuesHeart.setVisible(true);      
   }
}

function CustomDeckToolGui::onWake(%this)
{
   loadDeck();
   
   setRadioButtons();
   
   tabs.selectPage(0);
}

function setRadioButtons()
{
   //FaceStyles
   switch$($DeckTool::currentDeckEdit.faceStyle)
   {
      case "standard":
         FaceStyles.findObjectByInternalName("standard", true).setStateOn(true);
         CustomDeckToolGui.selectFaceStyle("standard");
         
      case "tall":
         FaceStyles.findObjectByInternalName("tall", true).setStateOn(true);
         CustomDeckToolGui.selectFaceStyle("tall");
         
      case "full":
         FaceStyles.findObjectByInternalName("full", true).setStateOn(true);
         CustomDeckToolGui.selectFaceStyle("full");
         
      default:
         error("Invalid state for setting the face style");         
   }
   
   
   //ViewFaces
   if($DeckTool::currentDeckEdit.allSpecified)
   {
      ViewFaces.findObjectByInternalName("all", true).setStateOn(true);
      CustomDeckToolGui.selectFaceEdit(true);
   }
   else
   {
      ViewFaces.findObjectByInternalName("royals", true).setStateOn(true);
      CustomDeckToolGui.selectFaceEdit(false);
   }
   

   //ValueFaceStyles
   switch$($DeckTool::currentDeckEdit.valueStyle)
   {
      case "standard":
         ValueFaceStyles.findObjectByInternalName("standard", true).setStateOn(true);
         CustomDeckToolGui.selectValueStyle("standard");
         
      case "different":
         ValueFaceStyles.findObjectByInternalName("different", true).setStateOn(true);
         CustomDeckToolGui.selectValueStyle("different");
         
      default:
         error("Invalid state for setting the value style");
   }
   
   //ValueSuits
   //we'll default this to spades
   ValueSuits.findObjectByInternalName("spades", true).setStateOn(true);
   
   
   //PipFaceStyles
   switch$($DeckTool::currentDeckEdit.pipStyle)
   {
      case "standard":
         PipFaceStyles.findObjectByInternalName("standard", true).setStateOn(true);
         CustomDeckToolGui.selectPipStyle("standard");
         
      case "face":
         PipFaceStyles.findObjectByInternalName("face", true).setStateOn(true);
         CustomDeckToolGui.selectPipStyle("face");
         
      case "all":
         PipFaceStyles.findObjectByInternalName("fall", true).setStateOn(true);
         CustomDeckToolGui.selectPipStyle("all");
         
      default:
         error("Invalid state for setting the face style");         
   }
   
   PreviewValue.findObjectByInternalName("Ace", true).setStateOn(true);
   PreviewSuit.findObjectByInternalName("Spade", true).setStateOn(true);
   CustomDeckToolGui.selectCardValue("A");
   CustomDeckToolGui.selectCardSuit("S");
}

function saveDeck()
{
   //first check if the already exists
   for(%i = 0; %i < GeneratedDecks.getCount(); %i++)
   {     
      if(GeneratedDecks.getObject(%i).Name $= deckNameField.getText())
      {
         WarningDialog.setupAndShow(CustomDeckToolGui.getGlobalCenter(), "Edit Existing Deck?", "Create a deck with a new name or overwrite the existing deck?",
         "Create New", "createNewDeck();", "Overwrite", "doDeckSave();", "Cancel", "");
         
         return;         
      }
   }
   
   //now check if the name is still NewDeck, blank, or has a bad character in it.
   if(deckNameField.getText() $= "NewDeck")
   {
      WarningDialog.setupAndShow(CustomDeckToolGui.getGlobalCenter(), "Name Reserved", "The name you have chosen is reserved.  Please specify a new name.",
         "", "", "Ok", "", "", ""); 
      return;
   }
   else if(deckNameField.getText() $= "")
   {
      WarningDialog.setupAndShow(CustomDeckToolGui.getGlobalCenter(), "Name Field Empty", "Please specify a name.",
         "", "", "Ok", "", "", ""); 
      return;
   }
   else if(deckNameField.getText() !$= stripChars(deckNameField.getText(), $DeckNameBadChars))
   {
      WarningDialog.setupAndShow(CustomDeckToolGui.getGlobalCenter(), "Bad Characters", "The name you chose has a space or other invalid characters in it.",
         "", "", "Ok", "", "", "");
      
      deckNameField.setText(stripChars(deckNameField.getText(), $DeckNameBadChars));
      return;
   }
   else
   {   
      doDeckSave();
   }
}

function createNewDeck()
{
   %deckName = "DefaultDeck1";
   for(%i = 0; %i < GeneratedDecks.getCount(); %i++)
   {     
      if(GeneratedDecks.getObject(%i).Name $= "DefaultDeck" @ %i)
      {
         %deckName = "DefaultDeck" @ %i+1;         
      }
   }
   
   %newDeck = new ScriptObject(%deckName)
   {
      Name = %deckName;
      
      internalName = %deckName;
      
      cardBackImage = fileName(CardBackPreview.bitmap);
      cardFrontImage = fileName(CardFrontPreview.bitmap);
      
      faceStyle = $DeckTool::currentDeckEdit.faceStyle;
      allFacesSpecified = $DeckTool::currentDeckEdit.allFacesSpecified;
      valueStyle = $DeckTool::currentDeckEdit.valueStyle;
      pipStyle = $DeckTool::currentDeckEdit.pipStyle;
      
      spadesFaceAce = fileName(AceSpadesPreview.bitmap);
      spadesFaceKing = fileName(KingSpadesPreview.bitmap);
      spadesFaceQueen = fileName(QueenSpadesPreview.bitmap);
      spadesFaceJack = fileName(JackSpadesPreview.bitmap);
      spadesFace2 = fileName(TwoSpadesPreview.bitmap);
      spadesFace3 = fileName(ThreeSpadesPreview.bitmap);
      spadesFace4 = fileName(FourSpadesPreview.bitmap);
      spadesFace5 = fileName(FiveSpadesPreview.bitmap);
      spadesFace6 = fileName(SixSpadesPreview.bitmap);
      spadesFace7 = fileName(SevenSpadesPreview.bitmap);
      spadesFace8 = fileName(EightSpadesPreview.bitmap);
      spadesFace9 = fileName(NineSpadesPreview.bitmap);
      spadesFace10 = fileName(TenSpadesPreview.bitmap);
      
      clubsFaceAce = fileName(AceClubsPreview.bitmap);
      clubsFaceKing = fileName(KingClubsPreview.bitmap);
      clubsFaceQueen = fileName(QueenClubsPreview.bitmap);
      clubsFaceJack = fileName(JackClubsPreview.bitmap);
      clubsFace2 = fileName(TwoClubsPreview.bitmap);
      clubsFace3 = fileName(ThreeClubsPreview.bitmap);
      clubsFace4 = fileName(FourClubsPreview.bitmap);
      clubsFace5 = fileName(FiveClubsPreview.bitmap);
      clubsFace6 = fileName(SixClubsPreview.bitmap);
      clubsFace7 = fileName(SevenClubsPreview.bitmap);
      clubsFace8 = fileName(EightClubsPreview.bitmap);
      clubsFace9 = fileName(NineClubsPreview.bitmap);
      clubsFace10 = fileName(TenClubsPreview.bitmap);
      
      diamondsFaceAce = fileName(AceDiamondsPreview.bitmap);
      diamondsFaceKing = fileName(KingDiamondsPreview.bitmap);
      diamondsFaceQueen = fileName(QueenDiamondsPreview.bitmap);
      diamondsFaceJack = fileName(JackDiamondsPreview.bitmap);
      diamondsFace2 = fileName(TwoDiamondsPreview.bitmap);
      diamondsFace3 = fileName(ThreeDiamondsPreview.bitmap);
      diamondsFace4 = fileName(FourDiamondsPreview.bitmap);
      diamondsFace5 = fileName(FiveDiamondsPreview.bitmap);
      diamondsFace6 = fileName(SixDiamondsPreview.bitmap);
      diamondsFace7 = fileName(SevenDiamondsPreview.bitmap);
      diamondsFace8 = fileName(EightDiamondsPreview.bitmap);
      diamondsFace9 = fileName(NineDiamondsPreview.bitmap);
      diamondsFace10 = fileName(TenDiamondsPreview.bitmap);
      
      heartsFaceAce = fileName(AceHeartsPreview.bitmap);
      heartsFaceKing = fileName(KingHeartsPreview.bitmap);
      heartsFaceQueen = fileName(QueenHeartsPreview.bitmap);
      heartsFaceJack = fileName(JackHeartsPreview.bitmap);
      heartsFace2 = fileName(TwoHeartsPreview.bitmap);
      heartsFace3 = fileName(ThreeHeartsPreview.bitmap);
      heartsFace4 = fileName(FourHeartsPreview.bitmap);
      heartsFace5 = fileName(FiveHeartsPreview.bitmap);
      heartsFace6 = fileName(SixHeartsPreview.bitmap);
      heartsFace7 = fileName(SevenHeartsPreview.bitmap);
      heartsFace8 = fileName(EightHeartsPreview.bitmap);
      heartsFace9 = fileName(NineHeartsPreview.bitmap);
      heartsFace10 = fileName(TenHeartsPreview.bitmap);
      
      topValueAceSpade = fileName(TopASpade.bitmap);
      topValue2Spade = fileName(Top2Spade.bitmap);
      topValue3Spade = fileName(Top3Spade.bitmap);
      topValue4Spade = fileName(Top4Spade.bitmap);
      topValue5Spade = fileName(Top5Spade.bitmap);
      topValue6Spade = fileName(Top6Spade.bitmap);
      topValue7Spade = fileName(Top7Spade.bitmap);
      topValue8Spade = fileName(Top8Spade.bitmap);
      topValue9Spade = fileName(Top9Spade.bitmap);
      topValue10Spade = fileName(Top10Spade.bitmap);
      topValueJackSpade = fileName(TopJSpade.bitmap);
      topValueQueenSpade = fileName(TopQSpade.bitmap);
      topValueKingSpade = fileName(TopKSpade.bitmap);
      
      bottomValueAceSpade = fileName(BottomASpade.bitmap);
      bottomValue2Spade = fileName(Bottom2Spade.bitmap);
      bottomValue3Spade = fileName(Bottom3Spade.bitmap);
      bottomValue4Spade = fileName(Bottom4Spade.bitmap);
      bottomValue5Spade = fileName(Bottom5Spade.bitmap);
      bottomValue6Spade = fileName(Bottom6Spade.bitmap);
      bottomValue7Spade = fileName(Bottom7Spade.bitmap);
      bottomValue8Spade = fileName(Bottom8Spade.bitmap);
      bottomValue9Spade = fileName(Bottom9Spade.bitmap);
      bottomValue10Spade = fileName(Bottom10Spade.bitmap);
      bottomValueJackSpade = fileName(BottomJSpade.bitmap);
      bottomValueQueenSpade = fileName(BottomQSpade.bitmap);
      bottomValueKingSpade = fileName(BottomKSpade.bitmap);
      
      topValueAceClub = fileName(TopAClub.bitmap);
      topValue2Club = fileName(Top2Club.bitmap);
      topValue3Club = fileName(Top3Club.bitmap);
      topValue4Club = fileName(Top4Club.bitmap);
      topValue5Club = fileName(Top5Club.bitmap);
      topValue6Club = fileName(Top6Club.bitmap);
      topValue7Club = fileName(Top7Club.bitmap);
      topValue8Club = fileName(Top8Club.bitmap);
      topValue9Club = fileName(Top9Club.bitmap);
      topValue10Club = fileName(Top10Club.bitmap);
      topValueJackClub = fileName(TopJClub.bitmap);
      topValueQueenClub = fileName(TopQClub.bitmap);
      topValueKingClub = fileName(TopKClub.bitmap);
      
      bottomValueAceClub = fileName(BottomAClub.bitmap);
      bottomValue2Club = fileName(Bottom2Club.bitmap);
      bottomValue3Club = fileName(Bottom3Club.bitmap);
      bottomValue4Club = fileName(Bottom4Club.bitmap);
      bottomValue5Club = fileName(Bottom5Club.bitmap);
      bottomValue6Club = fileName(Bottom6Club.bitmap);
      bottomValue7Club = fileName(Bottom7Club.bitmap);
      bottomValue8Club = fileName(Bottom8Club.bitmap);
      bottomValue9Club = fileName(Bottom9Club.bitmap);
      bottomValue10Club = fileName(Bottom10Club.bitmap);
      bottomValueJackClub = fileName(BottomJClub.bitmap);
      bottomValueQueenClub = fileName(BottomQClub.bitmap);
      bottomValueKingClub = fileName(BottomKClub.bitmap);
      
      topValueAceDiamond = fileName(TopADiamond.bitmap);
      topValue2Diamond = fileName(Top2Diamond.bitmap);
      topValue3Diamond = fileName(Top3Diamond.bitmap);
      topValue4Diamond = fileName(Top4Diamond.bitmap);
      topValue5Diamond = fileName(Top5Diamond.bitmap);
      topValue6Diamond = fileName(Top6Diamond.bitmap);
      topValue7Diamond = fileName(Top7Diamond.bitmap);
      topValue8Diamond = fileName(Top8Diamond.bitmap);
      topValue9Diamond = fileName(Top9Diamond.bitmap);
      topValue10Diamond = fileName(Top10Diamond.bitmap);
      topValueJackDiamond = fileName(TopJDiamond.bitmap);
      topValueQueenDiamond = fileName(TopQDiamond.bitmap);
      topValueKingDiamond = fileName(TopKDiamond.bitmap);
      
      bottomValueAceDiamond = fileName(BottomADiamond.bitmap);
      bottomValue2Diamond = fileName(Bottom2Diamond.bitmap);
      bottomValue3Diamond = fileName(Bottom3Diamond.bitmap);
      bottomValue4Diamond = fileName(Bottom4Diamond.bitmap);
      bottomValue5Diamond = fileName(Bottom5Diamond.bitmap);
      bottomValue6Diamond = fileName(Bottom6Diamond.bitmap);
      bottomValue7Diamond = fileName(Bottom7Diamond.bitmap);
      bottomValue8Diamond = fileName(Bottom8Diamond.bitmap);
      bottomValue9Diamond = fileName(Bottom9Diamond.bitmap);
      bottomValue10Diamond = fileName(Bottom10Diamond.bitmap);
      bottomValueJackDiamond = fileName(BottomJDiamond.bitmap);
      bottomValueQueenDiamond = fileName(BottomQDiamond.bitmap);
      bottomValueKingDiamond = fileName(BottomKDiamond.bitmap);
      
      topValueAceHeart = fileName(TopAHeart.bitmap);
      topValue2Heart = fileName(Top2Heart.bitmap);
      topValue3Heart = fileName(Top3Heart.bitmap);
      topValue4Heart = fileName(Top4Heart.bitmap);
      topValue5Heart = fileName(Top5Heart.bitmap);
      topValue6Heart = fileName(Top6Heart.bitmap);
      topValue7Heart = fileName(Top7Heart.bitmap);
      topValue8Heart = fileName(Top8Heart.bitmap);
      topValue9Heart = fileName(Top9Heart.bitmap);
      topValue10Heart = fileName(Top10Heart.bitmap);
      topValueJackHeart = fileName(TopJHeart.bitmap);
      topValueQueenHeart = fileName(TopQHeart.bitmap);
      topValueKingHeart = fileName(TopKHeart.bitmap);
      
      bottomValueAceHeart = fileName(BottomAHeart.bitmap);
      bottomValue2Heart = fileName(Bottom2Heart.bitmap);
      bottomValue3Heart = fileName(Bottom3Heart.bitmap);
      bottomValue4Heart = fileName(Bottom4Heart.bitmap);
      bottomValue5Heart = fileName(Bottom5Heart.bitmap);
      bottomValue6Heart = fileName(Bottom6Heart.bitmap);
      bottomValue7Heart = fileName(Bottom7Heart.bitmap);
      bottomValue8Heart = fileName(Bottom8Heart.bitmap);
      bottomValue9Heart = fileName(Bottom9Heart.bitmap);
      bottomValue10Heart = fileName(Bottom10Heart.bitmap);
      bottomValueJackHeart = fileName(BottomJHeart.bitmap);
      bottomValueQueenHeart = fileName(BottomQHeart.bitmap);
      bottomValueKingHeart = fileName(BottomKHeart.bitmap);
      
      topPipSpade = fileName(TopSpade.bitmap);
      topPipClub = fileName(TopClub.bitmap);
      topPipDiamond = fileName(TopDiamond.bitmap);
      topPipHeart = fileName(TopHeart.bitmap);
      
      bottomPipSpade = fileName(BottomSpade.bitmap);
      bottomPipClub = fileName(BottomClub.bitmap);
      bottomPipDiamond = fileName(BottomDiamond.bitmap);
      bottomPipHeart = fileName(BottomHeart.bitmap);
      
      facePipSpade = fileName(FaceSpade.bitmap);
      facePipClub = fileName(FaceClub.bitmap);
      facePipDiamond = fileName(FaceDiamond.bitmap);
      facePipHeart = fileName(FaceHeart.bitmap);
   };
   
   $DeckTool::currentDeckEdit = %newDeck;
   
   loadDeck();
}

function doDeckSave()
{
   $DeckTool::currentDeckEdit.Name = deckNameField.getText();
   $DeckTool::currentDeckEdit.setName(deckNameField.getText());
   $DeckTool::currentDeckEdit.internalName = deckNameField.getText() @ "Sprite";
   
   $DeckTool::currentDeckEdit.cardBackImage = fileName(CardBackPreview.bitmap);
   $DeckTool::currentDeckEdit.cardFrontImage = fileName(CardFrontPreview.bitmap);
   
   $DeckTool::currentDeckEdit.spadesFaceAce = fileName(AceSpadesPreview.bitmap);
   $DeckTool::currentDeckEdit.spadesFaceKing = fileName(KingSpadesPreview.bitmap);
   $DeckTool::currentDeckEdit.spadesFaceQueen = fileName(QueenSpadesPreview.bitmap);
   $DeckTool::currentDeckEdit.spadesFaceJack = fileName(JackSpadesPreview.bitmap);
   $DeckTool::currentDeckEdit.spadesFace2 = fileName(TwoSpadesPreview.bitmap);
   $DeckTool::currentDeckEdit.spadesFace3 = fileName(ThreeSpadesPreview.bitmap);
   $DeckTool::currentDeckEdit.spadesFace4 = fileName(FourSpadesPreview.bitmap);
   $DeckTool::currentDeckEdit.spadesFace5 = fileName(FiveSpadesPreview.bitmap);
   $DeckTool::currentDeckEdit.spadesFace6 = fileName(SixSpadesPreview.bitmap);
   $DeckTool::currentDeckEdit.spadesFace7 = fileName(SevenSpadesPreview.bitmap);
   $DeckTool::currentDeckEdit.spadesFace8 = fileName(EightSpadesPreview.bitmap);
   $DeckTool::currentDeckEdit.spadesFace9 = fileName(NineSpadesPreview.bitmap);
   $DeckTool::currentDeckEdit.spadesFace10 = fileName(TenSpadesPreview.bitmap);
   
   $DeckTool::currentDeckEdit.clubsFaceAce = fileName(AceClubsPreview.bitmap);
   $DeckTool::currentDeckEdit.clubsFaceKing = fileName(KingClubsPreview.bitmap);
   $DeckTool::currentDeckEdit.clubsFaceQueen = fileName(QueenClubsPreview.bitmap);
   $DeckTool::currentDeckEdit.clubsFaceJack = fileName(JackClubsPreview.bitmap);
   $DeckTool::currentDeckEdit.clubsFace2 = fileName(TwoClubsPreview.bitmap);
   $DeckTool::currentDeckEdit.clubsFace3 = fileName(ThreeClubsPreview.bitmap);
   $DeckTool::currentDeckEdit.clubsFace4 = fileName(FourClubsPreview.bitmap);
   $DeckTool::currentDeckEdit.clubsFace5 = fileName(FiveClubsPreview.bitmap);
   $DeckTool::currentDeckEdit.clubsFace6 = fileName(SixClubsPreview.bitmap);
   $DeckTool::currentDeckEdit.clubsFace7 = fileName(SevenClubsPreview.bitmap);
   $DeckTool::currentDeckEdit.clubsFace8 = fileName(EightClubsPreview.bitmap);
   $DeckTool::currentDeckEdit.clubsFace9 = fileName(NineClubsPreview.bitmap);
   $DeckTool::currentDeckEdit.clubsFace10 = fileName(TenClubsPreview.bitmap);
   
   $DeckTool::currentDeckEdit.diamondsFaceAce = fileName(AceDiamondsPreview.bitmap);
   $DeckTool::currentDeckEdit.diamondsFaceKing = fileName(KingDiamondsPreview.bitmap);
   $DeckTool::currentDeckEdit.diamondsFaceQueen = fileName(QueenDiamondsPreview.bitmap);
   $DeckTool::currentDeckEdit.diamondsFaceJack = fileName(JackDiamondsPreview.bitmap);
   $DeckTool::currentDeckEdit.diamondsFace2 = fileName(TwoDiamondsPreview.bitmap);
   $DeckTool::currentDeckEdit.diamondsFace3 = fileName(ThreeDiamondsPreview.bitmap);
   $DeckTool::currentDeckEdit.diamondsFace4 = fileName(FourDiamondsPreview.bitmap);
   $DeckTool::currentDeckEdit.diamondsFace5 = fileName(FiveDiamondsPreview.bitmap);
   $DeckTool::currentDeckEdit.diamondsFace6 = fileName(SixDiamondsPreview.bitmap);
   $DeckTool::currentDeckEdit.diamondsFace7 = fileName(SevenDiamondsPreview.bitmap);
   $DeckTool::currentDeckEdit.diamondsFace8 = fileName(EightDiamondsPreview.bitmap);
   $DeckTool::currentDeckEdit.diamondsFace9 = fileName(NineDiamondsPreview.bitmap);
   $DeckTool::currentDeckEdit.diamondsFace10 = fileName(TenDiamondsPreview.bitmap);
   
   $DeckTool::currentDeckEdit.heartsFaceAce = fileName(AceHeartsPreview.bitmap);
   $DeckTool::currentDeckEdit.heartsFaceKing = fileName(KingHeartsPreview.bitmap);
   $DeckTool::currentDeckEdit.heartsFaceQueen = fileName(QueenHeartsPreview.bitmap);
   $DeckTool::currentDeckEdit.heartsFaceJack = fileName(JackHeartsPreview.bitmap);
   $DeckTool::currentDeckEdit.heartsFace2 = fileName(TwoHeartsPreview.bitmap);
   $DeckTool::currentDeckEdit.heartsFace3 = fileName(ThreeHeartsPreview.bitmap);
   $DeckTool::currentDeckEdit.heartsFace4 = fileName(FourHeartsPreview.bitmap);
   $DeckTool::currentDeckEdit.heartsFace5 = fileName(FiveHeartsPreview.bitmap);
   $DeckTool::currentDeckEdit.heartsFace6 = fileName(SixHeartsPreview.bitmap);
   $DeckTool::currentDeckEdit.heartsFace7 = fileName(SevenHeartsPreview.bitmap);
   $DeckTool::currentDeckEdit.heartsFace8 = fileName(EightHeartsPreview.bitmap);
   $DeckTool::currentDeckEdit.heartsFace9 = fileName(NineHeartsPreview.bitmap);
   $DeckTool::currentDeckEdit.heartsFace10 = fileName(TenHeartsPreview.bitmap);
   
   $DeckTool::currentDeckEdit.topValueAceSpade = fileName(TopASpade.bitmap);
   $DeckTool::currentDeckEdit.topValue2Spade = fileName(Top2Spade.bitmap);
   $DeckTool::currentDeckEdit.topValue3Spade = fileName(Top3Spade.bitmap);
   $DeckTool::currentDeckEdit.topValue4Spade = fileName(Top4Spade.bitmap);
   $DeckTool::currentDeckEdit.topValue5Spade = fileName(Top5Spade.bitmap);
   $DeckTool::currentDeckEdit.topValue6Spade = fileName(Top6Spade.bitmap);
   $DeckTool::currentDeckEdit.topValue7Spade = fileName(Top7Spade.bitmap);
   $DeckTool::currentDeckEdit.topValue8Spade = fileName(Top8Spade.bitmap);
   $DeckTool::currentDeckEdit.topValue9Spade = fileName(Top9Spade.bitmap);
   $DeckTool::currentDeckEdit.topValue10Spade = fileName(Top10Spade.bitmap);
   $DeckTool::currentDeckEdit.topValueJackSpade = fileName(TopJSpade.bitmap);
   $DeckTool::currentDeckEdit.topValueQueenSpade = fileName(TopQSpade.bitmap);
   $DeckTool::currentDeckEdit.topValueKingSpade = fileName(TopKSpade.bitmap);
   
   $DeckTool::currentDeckEdit.bottomValueAceSpade = fileName(BottomASpade.bitmap);
   $DeckTool::currentDeckEdit.bottomValue2Spade = fileName(Bottom2Spade.bitmap);
   $DeckTool::currentDeckEdit.bottomValue3Spade = fileName(Bottom3Spade.bitmap);
   $DeckTool::currentDeckEdit.bottomValue4Spade = fileName(Bottom4Spade.bitmap);
   $DeckTool::currentDeckEdit.bottomValue5Spade = fileName(Bottom5Spade.bitmap);
   $DeckTool::currentDeckEdit.bottomValue6Spade = fileName(Bottom6Spade.bitmap);
   $DeckTool::currentDeckEdit.bottomValue7Spade = fileName(Bottom7Spade.bitmap);
   $DeckTool::currentDeckEdit.bottomValue8Spade = fileName(Bottom8Spade.bitmap);
   $DeckTool::currentDeckEdit.bottomValue9Spade = fileName(Bottom9Spade.bitmap);
   $DeckTool::currentDeckEdit.bottomValue10Spade = fileName(Bottom10Spade.bitmap);
   $DeckTool::currentDeckEdit.bottomValueJackSpade = fileName(BottomJSpade.bitmap);
   $DeckTool::currentDeckEdit.bottomValueQueenSpade = fileName(BottomQSpade.bitmap);
   $DeckTool::currentDeckEdit.bottomValueKingSpade = fileName(BottomKSpade.bitmap);
   
   $DeckTool::currentDeckEdit.topValueAceClub = fileName(TopAClub.bitmap);
   $DeckTool::currentDeckEdit.topValue2Club = fileName(Top2Club.bitmap);
   $DeckTool::currentDeckEdit.topValue3Club = fileName(Top3Club.bitmap);
   $DeckTool::currentDeckEdit.topValue4Club = fileName(Top4Club.bitmap);
   $DeckTool::currentDeckEdit.topValue5Club = fileName(Top5Club.bitmap);
   $DeckTool::currentDeckEdit.topValue6Club = fileName(Top6Club.bitmap);
   $DeckTool::currentDeckEdit.topValue7Club = fileName(Top7Club.bitmap);
   $DeckTool::currentDeckEdit.topValue8Club = fileName(Top8Club.bitmap);
   $DeckTool::currentDeckEdit.topValue9Club = fileName(Top9Club.bitmap);
   $DeckTool::currentDeckEdit.topValue10Club = fileName(Top10Club.bitmap);
   $DeckTool::currentDeckEdit.topValueJackClub = fileName(TopJClub.bitmap);
   $DeckTool::currentDeckEdit.topValueQueenClub = fileName(TopQClub.bitmap);
   $DeckTool::currentDeckEdit.topValueKingClub = fileName(TopKClub.bitmap);
   
   $DeckTool::currentDeckEdit.bottomValueAceClub = fileName(BottomAClub.bitmap);
   $DeckTool::currentDeckEdit.bottomValue2Club = fileName(Bottom2Club.bitmap);
   $DeckTool::currentDeckEdit.bottomValue3Club = fileName(Bottom3Club.bitmap);
   $DeckTool::currentDeckEdit.bottomValue4Club = fileName(Bottom4Club.bitmap);
   $DeckTool::currentDeckEdit.bottomValue5Club = fileName(Bottom5Club.bitmap);
   $DeckTool::currentDeckEdit.bottomValue6Club = fileName(Bottom6Club.bitmap);
   $DeckTool::currentDeckEdit.bottomValue7Club = fileName(Bottom7Club.bitmap);
   $DeckTool::currentDeckEdit.bottomValue8Club = fileName(Bottom8Club.bitmap);
   $DeckTool::currentDeckEdit.bottomValue9Club = fileName(Bottom9Club.bitmap);
   $DeckTool::currentDeckEdit.bottomValue10Club = fileName(Bottom10Club.bitmap);
   $DeckTool::currentDeckEdit.bottomValueJackClub = fileName(BottomJClub.bitmap);
   $DeckTool::currentDeckEdit.bottomValueQueenClub = fileName(BottomQClub.bitmap);
   $DeckTool::currentDeckEdit.bottomValueKingClub = fileName(BottomKClub.bitmap);
   
   $DeckTool::currentDeckEdit.topValueAceDiamond = fileName(TopADiamond.bitmap);
   $DeckTool::currentDeckEdit.topValue2Diamond = fileName(Top2Diamond.bitmap);
   $DeckTool::currentDeckEdit.topValue3Diamond = fileName(Top3Diamond.bitmap);
   $DeckTool::currentDeckEdit.topValue4Diamond = fileName(Top4Diamond.bitmap);
   $DeckTool::currentDeckEdit.topValue5Diamond = fileName(Top5Diamond.bitmap);
   $DeckTool::currentDeckEdit.topValue6Diamond = fileName(Top6Diamond.bitmap);
   $DeckTool::currentDeckEdit.topValue7Diamond = fileName(Top7Diamond.bitmap);
   $DeckTool::currentDeckEdit.topValue8Diamond = fileName(Top8Diamond.bitmap);
   $DeckTool::currentDeckEdit.topValue9Diamond = fileName(Top9Diamond.bitmap);
   $DeckTool::currentDeckEdit.topValue10Diamond = fileName(Top10Diamond.bitmap);
   $DeckTool::currentDeckEdit.topValueJackDiamond = fileName(TopJDiamond.bitmap);
   $DeckTool::currentDeckEdit.topValueQueenDiamond = fileName(TopQDiamond.bitmap);
   $DeckTool::currentDeckEdit.topValueKingDiamond = fileName(TopKDiamond.bitmap);
   
   $DeckTool::currentDeckEdit.bottomValueAceDiamond = fileName(BottomADiamond.bitmap);
   $DeckTool::currentDeckEdit.bottomValue2Diamond = fileName(Bottom2Diamond.bitmap);
   $DeckTool::currentDeckEdit.bottomValue3Diamond = fileName(Bottom3Diamond.bitmap);
   $DeckTool::currentDeckEdit.bottomValue4Diamond = fileName(Bottom4Diamond.bitmap);
   $DeckTool::currentDeckEdit.bottomValue5Diamond = fileName(Bottom5Diamond.bitmap);
   $DeckTool::currentDeckEdit.bottomValue6Diamond = fileName(Bottom6Diamond.bitmap);
   $DeckTool::currentDeckEdit.bottomValue7Diamond = fileName(Bottom7Diamond.bitmap);
   $DeckTool::currentDeckEdit.bottomValue8Diamond = fileName(Bottom8Diamond.bitmap);
   $DeckTool::currentDeckEdit.bottomValue9Diamond = fileName(Bottom9Diamond.bitmap);
   $DeckTool::currentDeckEdit.bottomValue10Diamond = fileName(Bottom10Diamond.bitmap);
   $DeckTool::currentDeckEdit.bottomValueJackDiamond = fileName(BottomJDiamond.bitmap);
   $DeckTool::currentDeckEdit.bottomValueQueenDiamond = fileName(BottomQDiamond.bitmap);
   $DeckTool::currentDeckEdit.bottomValueKingDiamond = fileName(BottomKDiamond.bitmap);
   
   $DeckTool::currentDeckEdit.topValueAceHeart = fileName(TopAHeart.bitmap);
   $DeckTool::currentDeckEdit.topValue2Heart = fileName(Top2Heart.bitmap);
   $DeckTool::currentDeckEdit.topValue3Heart = fileName(Top3Heart.bitmap);
   $DeckTool::currentDeckEdit.topValue4Heart = fileName(Top4Heart.bitmap);
   $DeckTool::currentDeckEdit.topValue5Heart = fileName(Top5Heart.bitmap);
   $DeckTool::currentDeckEdit.topValue6Heart = fileName(Top6Heart.bitmap);
   $DeckTool::currentDeckEdit.topValue7Heart = fileName(Top7Heart.bitmap);
   $DeckTool::currentDeckEdit.topValue8Heart = fileName(Top8Heart.bitmap);
   $DeckTool::currentDeckEdit.topValue9Heart = fileName(Top9Heart.bitmap);
   $DeckTool::currentDeckEdit.topValue10Heart = fileName(Top10Heart.bitmap);
   $DeckTool::currentDeckEdit.topValueJackHeart = fileName(TopJHeart.bitmap);
   $DeckTool::currentDeckEdit.topValueQueenHeart = fileName(TopQHeart.bitmap);
   $DeckTool::currentDeckEdit.topValueKingHeart = fileName(TopKHeart.bitmap);
   
   $DeckTool::currentDeckEdit.bottomValueAceHeart = fileName(BottomAHeart.bitmap);
   $DeckTool::currentDeckEdit.bottomValue2Heart = fileName(Bottom2Heart.bitmap);
   $DeckTool::currentDeckEdit.bottomValue3Heart = fileName(Bottom3Heart.bitmap);
   $DeckTool::currentDeckEdit.bottomValue4Heart = fileName(Bottom4Heart.bitmap);
   $DeckTool::currentDeckEdit.bottomValue5Heart = fileName(Bottom5Heart.bitmap);
   $DeckTool::currentDeckEdit.bottomValue6Heart = fileName(Bottom6Heart.bitmap);
   $DeckTool::currentDeckEdit.bottomValue7Heart = fileName(Bottom7Heart.bitmap);
   $DeckTool::currentDeckEdit.bottomValue8Heart = fileName(Bottom8Heart.bitmap);
   $DeckTool::currentDeckEdit.bottomValue9Heart = fileName(Bottom9Heart.bitmap);
   $DeckTool::currentDeckEdit.bottomValue10Heart = fileName(Bottom10Heart.bitmap);
   $DeckTool::currentDeckEdit.bottomValueJackHeart = fileName(BottomJHeart.bitmap);
   $DeckTool::currentDeckEdit.bottomValueQueenHeart = fileName(BottomQHeart.bitmap);
   $DeckTool::currentDeckEdit.bottomValueKingHeart = fileName(BottomKHeart.bitmap);
   
   $DeckTool::currentDeckEdit.topPipSpade = fileName(TopSpade.bitmap);
   $DeckTool::currentDeckEdit.topPipClub = fileName(TopClub.bitmap);
   $DeckTool::currentDeckEdit.topPipDiamond = fileName(TopDiamond.bitmap);
   $DeckTool::currentDeckEdit.topPipHeart = fileName(TopHeart.bitmap);
   
   $DeckTool::currentDeckEdit.bottomPipSpade = fileName(BottomSpade.bitmap);
   $DeckTool::currentDeckEdit.bottomPipClub = fileName(BottomClub.bitmap);
   $DeckTool::currentDeckEdit.bottomPipDiamond = fileName(BottomDiamond.bitmap);
   $DeckTool::currentDeckEdit.bottomPipHeart = fileName(BottomHeart.bitmap);
   
   $DeckTool::currentDeckEdit.facePipSpade = fileName(FaceSpade.bitmap);
   $DeckTool::currentDeckEdit.facePipClub = fileName(FaceClub.bitmap);
   $DeckTool::currentDeckEdit.facePipDiamond = fileName(FaceDiamond.bitmap);
   $DeckTool::currentDeckEdit.facePipHeart = fileName(FaceHeart.bitmap);
   
   GeneratedDecks.add($DeckTool::currentDeckEdit);
   GeneratedDecks.save("data/files/generatedDecks.cs");
   
   Canvas.pushDialog(CardGenerationGUI);
}

function loadDeck()
{
   %editTextString = "Textedit"; 
   %filePath = expandPath($DeckPath);   
   
   
    
   deckNameField.setText($DeckTool::currentDeckEdit.Name);
   
   
   CardBackPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.cardBackImage;
   setTextEditForBitmapPreview(CardBackPreview, %editTextString);
   CardFrontPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.cardFrontImage;
   setTextEditForBitmapPreview(CardFrontPreview, %editTextString);
   
   AceSpadesPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.spadesFaceAce;
   setTextEditForBitmapPreview(AceSpadesPreview, %editTextString);
   KingSpadesPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.spadesFaceKing;
   setTextEditForBitmapPreview(KingSpadesPreview, %editTextString);
   QueenSpadesPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.spadesFaceQueen;
   setTextEditForBitmapPreview(QueenSpadesPreview, %editTextString);
   JackSpadesPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.spadesFaceJack;
   setTextEditForBitmapPreview(JackSpadesPreview, %editTextString);
   TwoSpadesPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.spadesFace2;
   setTextEditForBitmapPreview(TwoSpadesPreview, %editTextString);
   ThreeSpadesPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.spadesFace3;
   setTextEditForBitmapPreview(ThreeSpadesPreview, %editTextString);
   FourSpadesPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.spadesFace4;
   setTextEditForBitmapPreview(FourSpadesPreview, %editTextString);
   FiveSpadesPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.spadesFace5;
   setTextEditForBitmapPreview(FiveSpadesPreview, %editTextString);
   SixSpadesPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.spadesFace6;
   setTextEditForBitmapPreview(SixSpadesPreview, %editTextString);
   SevenSpadesPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.spadesFace7;
   setTextEditForBitmapPreview(SevenSpadesPreview, %editTextString);
   EightSpadesPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.spadesFace8;
   setTextEditForBitmapPreview(EightSpadesPreview, %editTextString);
   NineSpadesPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.spadesFace9;
   setTextEditForBitmapPreview(NineSpadesPreview, %editTextString);
   TenSpadesPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.spadesFace10;
   setTextEditForBitmapPreview(TenSpadesPreview, %editTextString);
   
   AceClubsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.clubsFaceAce;
   setTextEditForBitmapPreview(AceClubsPreview, %editTextString);
   KingClubsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.clubsFaceKing;
   setTextEditForBitmapPreview(KingClubsPreview, %editTextString);
   QueenClubsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.clubsFaceQueen;
   setTextEditForBitmapPreview(QueenClubsPreview, %editTextString);
   JackClubsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.clubsFaceJack;
   setTextEditForBitmapPreview(JackClubsPreview, %editTextString);
   TwoClubsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.clubsFace2;
   setTextEditForBitmapPreview(TwoClubsPreview, %editTextString);
   ThreeClubsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.clubsFace3;
   setTextEditForBitmapPreview(ThreeClubsPreview, %editTextString);
   FourClubsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.clubsFace4;
   setTextEditForBitmapPreview(FourClubsPreview, %editTextString);
   FiveClubsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.clubsFace5;
   setTextEditForBitmapPreview(FiveClubsPreview, %editTextString);
   SixClubsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.clubsFace6;
   setTextEditForBitmapPreview(SixClubsPreview, %editTextString);
   SevenClubsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.clubsFace7;
   setTextEditForBitmapPreview(SevenClubsPreview, %editTextString);
   EightClubsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.clubsFace8;
   setTextEditForBitmapPreview(EightClubsPreview, %editTextString);
   NineClubsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.clubsFace9;
   setTextEditForBitmapPreview(NineClubsPreview, %editTextString);
   TenClubsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.clubsFace10;
   setTextEditForBitmapPreview(TenClubsPreview, %editTextString);
   
   AceDiamondsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.diamondsFaceAce;
   setTextEditForBitmapPreview(AceDiamondsPreview, %editTextString);
   KingDiamondsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.diamondsFaceKing;
   setTextEditForBitmapPreview(KingDiamondsPreview, %editTextString);
   QueenDiamondsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.diamondsFaceQueen;
   setTextEditForBitmapPreview(QueenDiamondsPreview, %editTextString);
   JackDiamondsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.diamondsFaceJack;
   setTextEditForBitmapPreview(JackDiamondsPreview, %editTextString);
   TwoDiamondsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.diamondsFace2;
   setTextEditForBitmapPreview(TwoDiamondsPreview, %editTextString);
   ThreeDiamondsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.diamondsFace3;
   setTextEditForBitmapPreview(ThreeDiamondsPreview, %editTextString);
   FourDiamondsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.diamondsFace4;
   setTextEditForBitmapPreview(FourDiamondsPreview, %editTextString);
   FiveDiamondsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.diamondsFace5;
   setTextEditForBitmapPreview(FiveDiamondsPreview, %editTextString);
   SixDiamondsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.diamondsFace6;
   setTextEditForBitmapPreview(SixDiamondsPreview, %editTextString);
   SevenDiamondsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.diamondsFace7;
   setTextEditForBitmapPreview(SevenDiamondsPreview, %editTextString);
   EightDiamondsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.diamondsFace8;
   setTextEditForBitmapPreview(EightDiamondsPreview, %editTextString);
   NineDiamondsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.diamondsFace9;
   setTextEditForBitmapPreview(NineDiamondsPreview, %editTextString);
   TenDiamondsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.diamondsFace10;
   setTextEditForBitmapPreview(TenDiamondsPreview, %editTextString);
   
   AceHeartsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.heartsFaceAce;
   setTextEditForBitmapPreview(AceHeartsPreview, %editTextString);
   KingHeartsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.heartsFaceKing;
   setTextEditForBitmapPreview(KingHeartsPreview, %editTextString);
   QueenHeartsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.heartsFaceQueen;
   setTextEditForBitmapPreview(QueenHeartsPreview, %editTextString);
   JackHeartsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.heartsFaceJack;
   setTextEditForBitmapPreview(JackHeartsPreview, %editTextString);
   TwoHeartsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.heartsFace2;
   setTextEditForBitmapPreview(TwoHeartsPreview, %editTextString);
   ThreeHeartsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.heartsFace3;
   setTextEditForBitmapPreview(ThreeHeartsPreview, %editTextString);
   FourHeartsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.heartsFace4;
   setTextEditForBitmapPreview(FourHeartsPreview, %editTextString);
   FiveHeartsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.heartsFace5;
   setTextEditForBitmapPreview(FiveHeartsPreview, %editTextString);
   SixHeartsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.heartsFace6;
   setTextEditForBitmapPreview(SixHeartsPreview, %editTextString);
   SevenHeartsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.heartsFace7;
   setTextEditForBitmapPreview(SevenHeartsPreview, %editTextString);
   EightHeartsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.heartsFace8;
   setTextEditForBitmapPreview(EightHeartsPreview, %editTextString);
   NineHeartsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.heartsFace9;
   setTextEditForBitmapPreview(NineHeartsPreview, %editTextString);
   TenHeartsPreview.bitmap = %filePath @ $DeckTool::currentDeckEdit.heartsFace10;
   setTextEditForBitmapPreview(TenHeartsPreview, %editTextString);
   
   TopASpade.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValueAceSpade;
   setTextEditForBitmapPreview(TopASpade, %editTextString);
   Top2Spade.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue2Spade;
   setTextEditForBitmapPreview(Top2Spade, %editTextString);
   Top3Spade.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue3Spade;
   setTextEditForBitmapPreview(Top3Spade, %editTextString);
   Top4Spade.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue4Spade;
   setTextEditForBitmapPreview(Top4Spade, %editTextString);
   Top5Spade.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue5Spade;
   setTextEditForBitmapPreview(Top5Spade, %editTextString);
   Top6Spade.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue6Spade;
   setTextEditForBitmapPreview(Top6Spade, %editTextString);
   Top7Spade.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue7Spade;
   setTextEditForBitmapPreview(Top7Spade, %editTextString);
   Top8Spade.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue8Spade;
   setTextEditForBitmapPreview(Top8Spade, %editTextString);
   Top9Spade.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue9Spade;
   setTextEditForBitmapPreview(Top9Spade, %editTextString);
   Top10Spade.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue10Spade;
   setTextEditForBitmapPreview(Top10Spade, %editTextString);
   TopJSpade.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValueJackSpade;
   setTextEditForBitmapPreview(TopJSpade, %editTextString);
   TopQSpade.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValueQueenSpade;
   setTextEditForBitmapPreview(TopQSpade, %editTextString);
   TopKSpade.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValueKingSpade;
   setTextEditForBitmapPreview(TopKSpade, %editTextString);
   
   BottomASpade.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValueAceSpade;
   setTextEditForBitmapPreview(BottomASpade, %editTextString);
   Bottom2Spade.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue2Spade;
   setTextEditForBitmapPreview(Bottom2Spade, %editTextString);
   Bottom3Spade.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue3Spade;
   setTextEditForBitmapPreview(Bottom3Spade, %editTextString);
   Bottom4Spade.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue4Spade;
   setTextEditForBitmapPreview(Bottom4Spade, %editTextString);
   Bottom5Spade.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue5Spade;
   setTextEditForBitmapPreview(Bottom5Spade, %editTextString);
   Bottom6Spade.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue6Spade;
   setTextEditForBitmapPreview(Bottom6Spade, %editTextString);
   Bottom7Spade.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue7Spade;
   setTextEditForBitmapPreview(Bottom7Spade, %editTextString);
   Bottom8Spade.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue8Spade;
   setTextEditForBitmapPreview(Bottom8Spade, %editTextString);
   Bottom9Spade.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue9Spade;
   setTextEditForBitmapPreview(Bottom9Spade, %editTextString);
   Bottom10Spade.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue10Spade;
   setTextEditForBitmapPreview(Bottom10Spade, %editTextString);
   BottomJSpade.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValueJackSpade;
   setTextEditForBitmapPreview(BottomJSpade, %editTextString);
   BottomQSpade.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValueQueenSpade;
   setTextEditForBitmapPreview(BottomQSpade, %editTextString);
   BottomKSpade.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValueKingSpade;
   setTextEditForBitmapPreview(BottomKSpade, %editTextString);
   
   TopAClub.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValueAceClub;
   setTextEditForBitmapPreview(TopAClub, %editTextString);
   Top2Club.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue2Club;
   setTextEditForBitmapPreview(Top2Club, %editTextString);
   Top3Club.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue3Club;
   setTextEditForBitmapPreview(Top3Club, %editTextString);
   Top4Club.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue4Club;
   setTextEditForBitmapPreview(Top4Club, %editTextString);
   Top5Club.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue5Club;
   setTextEditForBitmapPreview(Top5Club, %editTextString);
   Top6Club.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue6Club;
   setTextEditForBitmapPreview(Top6Club, %editTextString);
   Top7Club.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue7Club;
   setTextEditForBitmapPreview(Top7Club, %editTextString);
   Top8Club.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue8Club;
   setTextEditForBitmapPreview(Top8Club, %editTextString);
   Top9Club.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue9Club;
   setTextEditForBitmapPreview(Top9Club, %editTextString);
   Top10Club.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue10Club;
   setTextEditForBitmapPreview(Top10Club, %editTextString);
   TopJClub.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValueJackClub;
   setTextEditForBitmapPreview(TopJClub, %editTextString);
   TopQClub.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValueQueenClub;
   setTextEditForBitmapPreview(TopQClub, %editTextString);
   TopKClub.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValueKingClub;
   setTextEditForBitmapPreview(TopKClub, %editTextString);
   
   BottomAClub.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValueAceClub;
   setTextEditForBitmapPreview(BottomAClub, %editTextString);
   Bottom2Club.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue2Club;
   setTextEditForBitmapPreview(Bottom2Club, %editTextString);
   Bottom3Club.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue3Club;
   setTextEditForBitmapPreview(Bottom3Club, %editTextString);
   Bottom4Club.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue4Club;
   setTextEditForBitmapPreview(Bottom4Club, %editTextString);
   Bottom5Club.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue5Club;
   setTextEditForBitmapPreview(Bottom5Club, %editTextString);
   Bottom6Club.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue6Club;
   setTextEditForBitmapPreview(Bottom6Club, %editTextString);
   Bottom7Club.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue7Club;
   setTextEditForBitmapPreview(Bottom7Club, %editTextString);
   Bottom8Club.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue8Club;
   setTextEditForBitmapPreview(Bottom8Club, %editTextString);
   Bottom9Club.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue9Club;
   setTextEditForBitmapPreview(Bottom9Club, %editTextString);
   Bottom10Club.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue10Club;
   setTextEditForBitmapPreview(Bottom10Club, %editTextString);
   BottomJClub.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValueJackClub;
   setTextEditForBitmapPreview(BottomJClub, %editTextString);
   BottomQClub.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValueQueenClub;
   setTextEditForBitmapPreview(BottomQClub, %editTextString);
   BottomKClub.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValueKingClub;
   setTextEditForBitmapPreview(BottomKClub, %editTextString);
   
   TopADiamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValueAceDiamond;
   setTextEditForBitmapPreview(TopADiamond, %editTextString);
   Top2Diamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue2Diamond;
   setTextEditForBitmapPreview(Top2Diamond, %editTextString);
   Top3Diamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue3Diamond;
   setTextEditForBitmapPreview(Top3Diamond, %editTextString);
   Top4Diamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue4Diamond;
   setTextEditForBitmapPreview(Top4Diamond, %editTextString);
   Top5Diamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue5Diamond;
   setTextEditForBitmapPreview(Top5Diamond, %editTextString);
   Top6Diamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue6Diamond;
   setTextEditForBitmapPreview(Top6Diamond, %editTextString);
   Top7Diamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue7Diamond;
   setTextEditForBitmapPreview(Top7Diamond, %editTextString);
   Top8Diamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue8Diamond;
   setTextEditForBitmapPreview(Top8Diamond, %editTextString);
   Top9Diamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue9Diamond;
   setTextEditForBitmapPreview(Top9Diamond, %editTextString);
   Top10Diamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue10Diamond;
   setTextEditForBitmapPreview(Top10Diamond, %editTextString);
   TopJDiamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValueJackDiamond;
   setTextEditForBitmapPreview(TopJDiamond, %editTextString);
   TopQDiamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValueQueenDiamond;
   setTextEditForBitmapPreview(TopQDiamond, %editTextString);
   TopKDiamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValueKingDiamond;
   setTextEditForBitmapPreview(TopKDiamond, %editTextString);
   
   BottomADiamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValueAceDiamond;
   setTextEditForBitmapPreview(BottomADiamond, %editTextString);
   Bottom2Diamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue2Diamond;
   setTextEditForBitmapPreview(Bottom2Diamond, %editTextString);
   Bottom3Diamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue3Diamond;
   setTextEditForBitmapPreview(Bottom3Diamond, %editTextString);
   Bottom4Diamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue4Diamond;
   setTextEditForBitmapPreview(Bottom4Diamond, %editTextString);
   Bottom5Diamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue5Diamond;
   setTextEditForBitmapPreview(Bottom5Diamond, %editTextString);
   Bottom6Diamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue6Diamond;
   setTextEditForBitmapPreview(Bottom6Diamond, %editTextString);
   Bottom7Diamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue7Diamond;
   setTextEditForBitmapPreview(Bottom7Diamond, %editTextString);
   Bottom8Diamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue8Diamond;
   setTextEditForBitmapPreview(Bottom8Diamond, %editTextString);
   Bottom9Diamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue9Diamond;
   setTextEditForBitmapPreview(Bottom9Diamond, %editTextString);
   Bottom10Diamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue10Diamond;
   setTextEditForBitmapPreview(Bottom10Diamond, %editTextString);
   BottomJDiamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValueJackDiamond;
   setTextEditForBitmapPreview(BottomJDiamond, %editTextString);
   BottomQDiamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValueQueenDiamond;
   setTextEditForBitmapPreview(BottomQDiamond, %editTextString);
   BottomKDiamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValueKingDiamond;
   setTextEditForBitmapPreview(BottomKDiamond, %editTextString);
   
   TopAHeart.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValueAceHeart;
   setTextEditForBitmapPreview(TopAHeart, %editTextString);
   Top2Heart.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue2Heart;
   setTextEditForBitmapPreview(Top2Heart, %editTextString);
   Top3Heart.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue3Heart;
   setTextEditForBitmapPreview(Top3Heart, %editTextString);
   Top4Heart.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue4Heart;
   setTextEditForBitmapPreview(Top4Heart, %editTextString);
   Top5Heart.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue5Heart;
   setTextEditForBitmapPreview(Top5Heart, %editTextString);
   Top6Heart.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue6Heart;
   setTextEditForBitmapPreview(Top6Heart, %editTextString);
   Top7Heart.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue7Heart;
   setTextEditForBitmapPreview(Top7Heart, %editTextString);
   Top8Heart.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue8Heart;
   setTextEditForBitmapPreview(Top8Heart, %editTextString);
   Top9Heart.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue9Heart;
   setTextEditForBitmapPreview(Top9Heart, %editTextString);
   Top10Heart.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValue10Heart;
   setTextEditForBitmapPreview(Top10Heart, %editTextString);
   TopJHeart.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValueJackHeart;
   setTextEditForBitmapPreview(TopJHeart, %editTextString);
   TopQHeart.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValueQueenHeart;
   setTextEditForBitmapPreview(TopQHeart, %editTextString);
   TopKHeart.bitmap = %filePath @ $DeckTool::currentDeckEdit.topValueKingHeart;
   setTextEditForBitmapPreview(TopKHeart, %editTextString);
   
   BottomAHeart.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValueAceHeart;
   setTextEditForBitmapPreview(BottomAHeart, %editTextString);
   Bottom2Heart.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue2Heart;
   setTextEditForBitmapPreview(Bottom2Heart, %editTextString);
   Bottom3Heart.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue3Heart;
   setTextEditForBitmapPreview(Bottom3Heart, %editTextString);
   Bottom4Heart.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue4Heart;
   setTextEditForBitmapPreview(Bottom4Heart, %editTextString);
   Bottom5Heart.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue5Heart;
   setTextEditForBitmapPreview(Bottom5Heart, %editTextString);
   Bottom6Heart.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue6Heart;
   setTextEditForBitmapPreview(Bottom6Heart, %editTextString);
   Bottom7Heart.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue7Heart;
   setTextEditForBitmapPreview(Bottom7Heart, %editTextString);
   Bottom8Heart.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue8Heart;
   setTextEditForBitmapPreview(Bottom8Heart, %editTextString);
   Bottom9Heart.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue9Heart;
   setTextEditForBitmapPreview(Bottom9Heart, %editTextString);
   Bottom10Heart.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValue10Heart;
   setTextEditForBitmapPreview(Bottom10Heart, %editTextString);
   BottomJHeart.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValueJackHeart;
   setTextEditForBitmapPreview(BottomJHeart, %editTextString);
   BottomQHeart.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValueQueenHeart;
   setTextEditForBitmapPreview(BottomQHeart, %editTextString);
   BottomKHeart.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomValueKingHeart;
   setTextEditForBitmapPreview(BottomKHeart, %editTextString);
   
   TopSpade.bitmap = %filePath @ $DeckTool::currentDeckEdit.topPipSpade;
   setTextEditForBitmapPreview(TopSpade, %editTextString);
   TopClub.bitmap = %filePath @ $DeckTool::currentDeckEdit.topPipClub;
   setTextEditForBitmapPreview(TopClub, %editTextString);
   TopDiamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.topPipDiamond;
   setTextEditForBitmapPreview(TopDiamond, %editTextString);
   TopHeart.bitmap = %filePath @ $DeckTool::currentDeckEdit.topPipHeart;
   setTextEditForBitmapPreview(TopHeart, %editTextString);
   
   BottomSpade.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomPipSpade;
   setTextEditForBitmapPreview(BottomSpade, %editTextString);
   BottomClub.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomPipClub;
   setTextEditForBitmapPreview(BottomClub, %editTextString);
   BottomDiamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomPipDiamond;
   setTextEditForBitmapPreview(BottomDiamond, %editTextString);
   BottomHeart.bitmap = %filePath @ $DeckTool::currentDeckEdit.bottomPipHeart;
   setTextEditForBitmapPreview(BottomHeart, %editTextString);
   
   FaceSpade.bitmap = %filePath @ $DeckTool::currentDeckEdit.facePipSpade;
   setTextEditForBitmapPreview(FaceSpade, %editTextString);
   FaceClub.bitmap = %filePath @ $DeckTool::currentDeckEdit.facePipClub;
   setTextEditForBitmapPreview(FaceClub, %editTextString);
   FaceDiamond.bitmap = %filePath @ $DeckTool::currentDeckEdit.facePipDiamond;
   setTextEditForBitmapPreview(FaceDiamond, %editTextString);
   FaceHeart.bitmap = %filePath @ $DeckTool::currentDeckEdit.facePipHeart;
   setTextEditForBitmapPreview(FaceHeart, %editTextString);
}

function setTextEditForBitmapPreview(%bitmapPreview, %textEditInternalName)
{
   %parent = %bitmapPreview.getParent();
   
   if (isObject(%parent))
   {
        %textEdit = %parent.findObjectByInternalName(%textEditInternalName);
        
        if (isObject(%textEdit))
        {
            %textEdit.setText(fileName(%bitmapPreview.bitmap));
            %textEdit.setActive(false);  
        }
   }
}

function DeckNameClearButton::onClick(%this)
{
    deckNameField.text = "";
}
