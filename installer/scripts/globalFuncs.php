<?php

function RunCmd($cmd, $dir)
{
   $descriptorspec = array(
      0 => array("pipe", "r"),  // stdin is a pipe that the child will read from
      1 => array("pipe", "w"),  // stdout is a pipe that the child will write to
      2 => array("pipe", "w")   // stderr is a pipe that the child will write to
      );

   $proc_options = array('bypass_shell' => "1");
   $process = proc_open('C:\\Windows\\System32\cmd.exe', $descriptorspec, $pipes, $dir, NULL);

   if (is_resource($process))
   {
      echo("\nRunning: ".$cmd."\n");

      fwrite($pipes[0], $cmd."\n");

      fwrite($pipes[0], "exit\n");

      while(!feof($pipes[1]))
         echo(fgets($pipes[1], 1024));

      fclose($pipes[0]);
      fclose($pipes[1]);
      fclose($pipes[2]);

      $return_value = proc_close($process);

      if ($return_value != 0)
         Error("Error running ".$cmd);

   }
   else
      Error("Unable to open process cmd.exe, permission issue?");
}

function Error($msg)
{
   echo("\n!!!ERROR!!!\n\n$msg\n\n!!!ERROR!!!\n\n");
   exit(1);
}

?>