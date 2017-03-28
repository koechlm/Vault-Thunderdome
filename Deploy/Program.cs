/*=====================================================================
  
  This file is part of the Autodesk Vault API Code Samples.

  Copyright (C) Autodesk Inc.  All rights reserved.

THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
PARTICULAR PURPOSE.
=====================================================================*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using Thunderdome;

namespace Deploy
{
    class Program
    {
        
        static void Main(string[] args)
        {
            // the first arg should be the EXE path
            string exePath = Environment.GetCommandLineArgs()[0];
            exePath = Path.GetDirectoryName(exePath);

            Console.Out.WriteLine("Waiting for Vault client to exit.");

            try
            {
                string inputPath = Path.Combine(exePath, UtilSettings.FILE_NAME);

                if (!File.Exists(inputPath))
                    return; // nothing to do


                UtilSettings commands = UtilSettings.Load();

                // wait until Vault Explorer exits
                string veName = Path.GetFileNameWithoutExtension(commands.VaultClient);
                while (true)
                {
                    Process [] ps = Process.GetProcessesByName(veName);

                    if (ps != null && ps.Length > 0)
                        System.Threading.Thread.Sleep(10000);  // wait 10 sec
                    else
                        break;  // vault explorer has exited
                }

                Console.Out.WriteLine("Performing Vault updates.");

                // perform the delete operations
                foreach (string del in commands.DeleteOperations)
                {
                    if (File.Exists(del))
                        File.Delete(del);
                }

                // perform the folder move operations
                foreach (FolderMove move in commands.FolderMoveOperations)
                {
                    if (Directory.Exists(move.To))
                        continue;   // don't overwrite existing folders

                    Directory.Move(move.From, move.To);
                }

                // perfrorm the file move operations
                foreach (FileMove move in commands.FileMoveOperations)
                {
                    string toDir = Path.GetDirectoryName(move.To);
                    if (!Directory.Exists(toDir))
                        Directory.CreateDirectory(toDir);

                    if (File.Exists(move.From))
                    {
                        if (File.Exists(move.To))
                            File.Delete(move.To);

                        File.Copy(move.From, move.To);

                        File.Delete(move.From);
                    } 
                }


                try
                {
                    // delete the input file
                    File.Delete(inputPath);

                    // delete the temp folder
                    Directory.Delete(commands.TempFolder, true);
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Path.Combine(exePath, "deployErrorLog.txt"), ex.ToString());
                }

                // restart Vault Explorer
                Process.Start(commands.VaultClient);
            }
            catch (Exception e)
            {

                File.AppendAllText(Path.Combine(exePath, "deployErrorLog.txt"), e.ToString());
            }
        }
    }
}
