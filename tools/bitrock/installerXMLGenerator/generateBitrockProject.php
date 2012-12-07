<?php
include 'xml.php';

function printHelp()
{
print <<<HELP
This script generates an XML project file for Bitrock based on a
template file.
 
Command line parameters:

--projectType [string]         This is used to determine default
                               directories and filenames used by this
                               script. Affected parameters are the
                               eula directory, template file, and
                               output directory. Each of these can
                               still be specified manually on the
                               command line.

--binary                       Creates an installer with a binary license.
                               Each installer can be one of binary or pro,
                               and one of indie or commercial. These are
                               used in determining the files to include
                               and the license to use. The default is indie
                               and binary.

--pro                          Creates an installer with a pro license.

--indie                        Creates an installer with an indie license.

--commercial                   Creates an installer with a commercial
                               license.

--downloading                  Creates a downloading installer.

--mac                          Custom options for mac installers. Not absolutely
                               required for mac installers, but makes things
                               look nicer. Adjusts the instal destination path.

--version [version string]     The string to use as the version number.
                               This is printed in various places and
                               used in filenames.

--checkForUpdates [true/false] By default, updates are checked for
                               indie binary versions. Override that
                               behavior with this.

--shortName [string]           This is printed in various places by
                               the installer. It is also used as the
                               output filename of the project file.
                               The actual string is appended with the
                               license type and version number.

--fullName [string]            This is printed in various places by
                               the installer. It is also used as the
                               output filename of the installer.
                               The actual string is appended with the
                               license type and version number.

--eulaDir [directory]          The directory to find the EULA files in. The
                               default is "../installerAssets/EULA/[projectType]/".
							          The EULA files themselves should be named for
                               the license type. So, an indie binary
                               license would be named "EULA_Indie_Binary.txt".
                               A commercial pro license would be named
                               "EULA_Commercial_Pro.txt". This is relative
							          to the output directory.

--eulaFile [filename]          Overwrites the default EULA filename.

--templateFile [filename]      The template project file to use. By default
                               this is "../projects/templates
                               /[projectType].xml";

--outputDir [directory]        The directory that the project file is
                               written to. By default this is
                               "../projects/[projectType]/";

--inputDir [directory]         The directory where the installer's input files
                               are located.

--tgbDir [directory]           The directory where the tgb binary is located. By
                               default this will depend on the license type since
                               some licenses contain trial code, some include the
                               tool source, and some have neither. This is
                               relative to the input directory.

HELP;
}

$projectType = "t2d";
$pro = false;
$commercial = false;
$downloading = false;
$mac = false;

chdir(dirname($argv[0]));

// Several other default values depend on the project type, and
// license type so look for those on the command line first. And,
// if help is requested, handle it now.
for ($i = 1; $i < $argc; $i++)
{
   $hasNextArg = $i < ($argc - 1);

   switch ($argv[$i])
   {
      case "--projectType":
         if ($hasNextArg)
         {
            $projectType = $argv[$i + 1];
            $i++;
         }
         else
            print("Project type not specified. --projectType [string]\n");

         break;

      case "--indie":
         $commercial = false;
         break;

      case "--commercial":
         $commercial = true;
         break;

      case "--binary":
         $pro = false;
         break;

      case "--pro":
         $pro = true;
         break;

      case "--downloading":
         $downloading = true;
         break;
      
      case "--mac":
         $mac = true;
         break;

      case "-?":
      case "/?":
      case "-h":
      case "--help":
         printHelp();
         exit;
         break;

      default:
         break;
   }
}

$version = 0;

$checkForUpdates = 0;
//if ((!$pro && !$commercial) || $downloading)
//   $checkForUpdates = 1;

$shortNamePrefix = "TorqueGameBuilder";
$fullNamePrefix = "TorqueGameBuilder";
$eulaDirectory = "../../installerAssets/EULA/$projectType/";
$eulaOverride = "";
$outputDirectory = "../projects/$projectType/";
$inputDirectory = "../../../../build/staging/";
$templateFile = "../projects/templates/$projectType.xml";

$tgbDirectory = "tgb/tgb";
// if ($pro)             $tgbDirectory = $tgbDirectory . "/pro";
// else if ($commercial) $tgbDirectory = $tgbDirectory . "/binary";
// else                  $tgbDirectory = $tgbDirectory . "/trial";

// Parse the command line for the rest of the arguments.
for ($i = 1; $i < $argc; $i++)
{
   $hasNextArg = $i < ($argc - 1);

   switch ($argv[$i])
   {
      case "--version":
         if ($hasNextArg)
         {
            $version = $argv[$i + 1];
            
            $i++;
         }
         else
            print("Version number not specified. --version [version]\n");

         break;

      case "--checkForUdpates":
         if ($hasNextArg)
         {
            $checkForUpdates = $argv[$i + 1];
            $i++;
         }
         else
            print("Check for updates parameter not specified. --checkForUpdates [true/false]\n");

         break;

      case "--shortName":
         if ($hasNextArg)
         {
            $shortNamePrefix = $argv[$i + 1];
            $i++;
         }
         else
            print("Short name not specified. --shortName [string]\n");

         break;

      case "--fullName":
         if ($hasNextArg)
         {
            $fullNamePrefix = $argv[$i + 1];
            $i++;
         }
         else
            print("Full name not specified. --fullName [string]\n");

         break;

      case "--eulaDir":
         if ($hasNextArg)
         {
            $eulaDirectory = $argv[$i + 1];
            $i++;
         }
         else
            print("EULA directory not specified. --eulaDir [directory]\n");

         break;

      case "--eulaFile":
         if ($hasNextArg)
         {
            $eulaOverride = $argv[$i + 1];
            $i++;
         }
         else
            print("EULA file not specified. --eulaflie [filename]\n");

         break;

      case "--outputDir":
         if ($hasNextArg)
         {
            $outputDirectory = $argv[$i + 1];
            $i++;
         }
         else
            print("Output directory not specified. --outputDir [directory]\n");

         break;

      case "--inputDir":
         if ($hasNextArg)
         {
            $inputDirectory = $argv[$i + 1];
            $i++;
         }
         else
            print("Input directory not specified. --inputDir [directory]\n");

         break;

      case "--tgbDir":
         if ($hasNextArg)
         {
            $tgbDirectory = $argv[$i + 1];
            $i++;
         }
         else
            print("TGB directory not specified. --tgbDir [directory]\n");

         break;

      case "--templateFile":
         if ($hasNextArg)
         {
            $templateFile = $argv[$i + 1];
            $i++;
         }
         else
            print("Template filename not specified. --template [filename]\n");

         break;

      case "--projectType":
      case "--indie":
      case "--commercial":
      case "--binary":
      case "--pro":
      case "--downloading":
	  case "--mac":
         break;

      default:
         print("Invalid command line parameter: " . $argv[$i] . ". Use -? for help.\n");
         break;
   }
}

$licenseType = "";
$licenseTypeFull = "";
if ($commercial)
{
   $licenseType = "Commercial";
   $licenseTypeFull = "Commercial";
}
else
   $licenseTypeFull = "Indie";

if ($pro)
{
   $licenseType = trim($licenseType . " Pro");
   $licenseTypeFull = $licenseTypeFull . " Pro";
}
else
   $licenseTypeFull = $licenseTypeFull . " Binary";

$shortName = $shortNamePrefix;
$fullName = trim("$fullNamePrefix $licenseType");
$displayName = trim("$fullNamePrefix $version $licenseType");

$eulaFile = $eulaDirectory . "EULA_" . str_replace(" ", "_", $licenseTypeFull) . ".txt";
if (strcmp($eulaOverride, "") != 0)
   $eulaFile = $eulaOverride;

$outputFile = $outputDirectory . str_replace(" ", "_", $licenseTypeFull) . ".xml";
$langFile = str_replace(" ", "_", $licenseTypeFull) . ".lng";

$installerFilename = str_replace(" ", "_", $displayName);
$installerFilename = str_replace(".", "", $installerFilename) . '.${platform_exec_suffix}';

$versionId = strtok($version, " ");
$versionId = str_replace(".", "", $versionId);
while (strlen($versionId) < 4)
   $versionId = $versionId . "0";

if ($downloading)
{
   $outputFile = $outputDirectory . "Downloading.xml";
   $installerFilename = 'GetTorqueGameBuilder.${platform_exec_suffix}';
   $versionId--;
}

$file = file_get_contents($templateFile);
$data = XML_unserialize($file);

$data["project"]["shortName"] = $shortName;
$data["project"]["fullName"] = $fullName;
$data["project"]["version"] = $version;
$data["project"]["installerFilename"] = $installerFilename;
$data["project"]["licenseFile"] = $eulaFile;
$data["project"]["checkForUpdates"] = $checkForUpdates;
$data["project"]["versionId"] = $versionId;
$data["project"]["customLanguageFileList"]["language"]["file"] = $langFile;

$file = fopen("$outputDirectory$langFile", 'w+');
fwrite($file, "Installer.Setup.Title=$fullName $version - Setup");
fclose($file);

// The downloading installer doesn't contain any files.
if (!$downloading)
{
   // Always include 'tgb', 'games', 'tools', 'documentation', and the EULA.
   $data["project"]["componentList"]["component"]["folderList"]["folder"]["distributionFileList"] = array();
   $data["project"]["componentList"]["component"]["folderList"]["folder"]["distributionFileList"]["distributionFile"] = array();
   $data["project"]["componentList"]["component"]["folderList"]["folder"]["distributionFileList"]["distributionFile"][0] = array("origin" => $eulaFile);
   $data["project"]["componentList"]["component"]["folderList"]["folder"]["distributionFileList"]["distributionDirectory"] = array();
   $data["project"]["componentList"]["component"]["folderList"]["folder"]["distributionFileList"]["distributionDirectory"][0] = array("origin" => $inputDirectory . $tgbDirectory);
   $data["project"]["componentList"]["component"]["folderList"]["folder"]["distributionFileList"]["distributionDirectory"][1] = array("origin" => $inputDirectory . "games");
   $data["project"]["componentList"]["component"]["folderList"]["folder"]["distributionFileList"]["distributionDirectory"][2] = array("origin" => $inputDirectory . "documentation");
   $data["project"]["componentList"]["component"]["folderList"]["folder"]["distributionFileList"]["distributionDirectory"][5] = array("origin" => $inputDirectory . "tools");

   // Include 'engine' and 'build' with pro licenses
   if ($pro)
   {
      $data["project"]["componentList"]["component"]["folderList"]["folder"]["distributionFileList"]["distributionDirectory"][3] = array("origin" => $inputDirectory . "engine");
      $data["project"]["componentList"]["component"]["folderList"]["folder"]["distributionFileList"]["distributionDirectory"][4] = array("origin" => $inputDirectory . "build");
   }
   if ($mac)
   {
      $data["project"]["parameterList"]["directoryParameter"]["value"] = '${platform_install_prefix}/${product_shortname}';
      $data["project"]["parameterList"]["directoryParameter"]["default"] = '${platform_install_prefix}/${product_shortname}-${product_version}';
   }
}

$xml = XML_serialize($data);

$file = fopen($outputFile, 'w+');
fwrite($file, print_r($xml, true));
fclose($file);
?>
