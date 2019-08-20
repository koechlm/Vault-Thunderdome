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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Autodesk.Connectivity.Extensibility.Framework;
using Autodesk.Connectivity.Explorer.Extensibility;
using Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.WebServicesTools;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using Autodesk.DataManagement.Client.Framework.Vault.Settings;
using Autodesk.DataManagement.Client.Framework.Vault.Results;
using VDF = Autodesk.DataManagement.Client.Framework;

using ICSharpCode.SharpZipLib.Zip;

[assembly: ApiVersion("13.0")]
[assembly: ExtensionId("2AAE56F1-3E44-4B69-8AF0-15566D7A2E49")]

namespace Thunderdome
{
    public class TheExtension : IExplorerExtension
    {
        public static string USER_FOLDER_OPTION = "autodesk.thunderdome.usersFolder";
        public static string DEFAULT_FOLDER_OPTION = "autodesk.thunderdome.defaultFolder";
        public static string DEPLOYMENT_CONFIG_OPTION = "autodesk.thunderdome.deploymentConfig";
        public static string VAULT_SETTINGS_FOLDER_NAME = "VaultSettings";
        public static string VAULT_COMMON_FOLDER_NAME = "VaultCommon";
        public static string EXTENSIONS_FOLDER_NAME = "Extensions";

        public static string PACKAGE_NAME = "Deployment.td";

        private bool m_restartPending = false;

        #region IExtension Members

        public IEnumerable<CommandSite> CommandSites()
        {
            CommandSite site = new CommandSite("autodesk.thunderdome.site", "Thunderdome")
            {
                DeployAsPulldownMenu = true,
                Location = CommandSiteLocation.ToolsMenu
            };

            CommandItem backupCmd = new CommandItem("autodesk.thunderdome.command.backup", "Backup Vault Settings")
            {
                Description = "Backup your Vault settings",
                Hint = "Backup your Vault settings",
                Image = Icons.OnesAndZeros,
                ToolbarPaintStyle = PaintStyle.TextAndGlyph
            };
            backupCmd.Execute += new EventHandler<CommandItemEventArgs>(backupCmd_Execute);
            site.AddCommand(backupCmd);

            CommandItem configCmd = new CommandItem("autodesk.thunderdome.command.config", "Create Deployment")
            {
                Description = "Decide which components to push to Vault users",
                Hint = "Create Deployment",
                Image = Icons.OnesAndZeros,
                ToolbarPaintStyle = PaintStyle.TextAndGlyph
            };
            configCmd.Execute += new EventHandler<CommandItemEventArgs>(command_Execute);
            site.AddCommand(configCmd);

            return new CommandSite[] { site };
        }

        public IEnumerable<DetailPaneTab> DetailTabs()
        {
            return null;
        }

        public IEnumerable<string> HiddenCommands()
        {
            return null;
        }

        public IEnumerable<CustomEntityHandler> CustomEntityHandlers()
        {
            return null;
        }

        public void OnLogOff(IApplication application)
        {
        }

        public void OnLogOn(IApplication application)
        {
            try
            {
                CheckForUpdates(application.Connection.Server, application.Connection.Vault, 
                    application.Connection);
            }
            catch
            { }
        }

        public void OnShutdown(IApplication application)
        {
            try
            {
                if (m_restartPending)
                {
                    string codeFolder = Util.GetAssemblyPath();
                    string exePath = Path.Combine(codeFolder, "deploy.exe");

                    System.Diagnostics.Process.Start(exePath);
                }
            }
            catch { }
        }

        public void OnStartup(IApplication application)
        {
            
        }

        #endregion

        private void CheckForUpdates(string serverName, string vaultName, Connection conn)
        {
            if (m_restartPending)
                return;

            Settings settings = Settings.Load();

            // user previously indicated not to download updates and not to be prompted
            if (settings.NeverDownload)
                return;

            string defaultFolder = conn.WebServiceManager.KnowledgeVaultService.GetVaultOption(DEFAULT_FOLDER_OPTION);
            if (defaultFolder == null || defaultFolder.Length == 0)
                return;
            if (!defaultFolder.EndsWith("/"))
                defaultFolder += "/";

            string deploymentPath = defaultFolder + PACKAGE_NAME;
            Autodesk.Connectivity.WebServices.File [] files = conn.WebServiceManager.DocumentService.FindLatestFilesByPaths(
                deploymentPath.ToSingleArray());

            if (files == null || files.Length == 0 || files[0].Id <= 0 || files[0].Cloaked)
                return; // no package found

            VaultEntry entry = settings.GetOrCreateEntry(serverName, vaultName);

            if (entry != null && entry.LastUpdate >= files[0].CkInDate)
                return;  // we are up to date

            StringBuilder updateList = new StringBuilder();

            string deploymentXml = conn.WebServiceManager.KnowledgeVaultService.GetVaultOption(DEPLOYMENT_CONFIG_OPTION);
            DeploymentModel deploymentModel = DeploymentModel.Load(deploymentXml);
            if (deploymentModel == null || deploymentModel.Containers == null)
                return;

            foreach (DeploymentContainer container in deploymentModel.Containers)
            {
                updateList.AppendLine(container.DisplayName);

                foreach (DeploymentItem item in container.DeploymentItems)
                {
                    updateList.AppendLine("- " + item.DisplayName);
                }

                updateList.AppendLine();
            }

            AskDialog ask = new AskDialog(updateList.ToString());
            DialogResult result = ask.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (ask.AskResult == AskDialog.AskResultEnum.No)
                    return;
                else if (ask.AskResult == AskDialog.AskResultEnum.Never)
                {
                    settings.NeverDownload = true;
                    settings.Save();
                    return;
                }
            }
            else
                return;

            // if we got here, the user cliecked yes.


            string tempPath = Path.Combine(Path.GetTempPath(), "Thunderdome");
            if (!tempPath.EndsWith("\\"))
                tempPath += "\\";

            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, true);
            DirectoryInfo dirInfo = Directory.CreateDirectory(tempPath);

            UtilSettings utilSettings = new UtilSettings();
            utilSettings.TempFolder = tempPath;

            // the first arg should be the EXE path
            utilSettings.VaultClient = Environment.GetCommandLineArgs()[0];

            string zipPath = Path.Combine(tempPath, PACKAGE_NAME);

            VDF.Vault.Currency.Entities.FileIteration vdfFile = new VDF.Vault.Currency.Entities.FileIteration(conn, files[0]);
            VDF.Currency.FilePathAbsolute vdfPath = new VDF.Currency.FilePathAbsolute(zipPath);
            AcquireFilesSettings acquireSettings = new AcquireFilesSettings(conn, false);
            acquireSettings.AddFileToAcquire(vdfFile, AcquireFilesSettings.AcquisitionOption.Download, vdfPath);
            AcquireFilesResults acquireResults = conn.FileManager.AcquireFiles(acquireSettings);
            foreach (FileAcquisitionResult acquireResult in acquireResults.FileResults)
            {
                if (acquireResult.Exception != null)
                    throw acquireResult.Exception;
            }
            
            // clear the read-only bit
            System.IO.File.SetAttributes(zipPath, FileAttributes.Normal);

            FastZip zip = new FastZip();
            zip.ExtractZip(zipPath, tempPath, null);

            MasterController mc = new MasterController(conn, vaultName);
            mc.SetMoveOperations(tempPath, utilSettings);
            
            utilSettings.Save();
           
            MessageBox.Show("Updates downloaded. " +
                "You need exit the Vault client to complete the update process. " + Environment.NewLine +
                "The Vault Client will restart when the update is complete.",
                "Exit Required");

            m_restartPending = true;

            entry.LastUpdate = files[0].CkInDate;
            settings.Save();
        }

        private string ConvertToLocalPath(string vaultPath, string localRoot, string vaultRoot)
        {
            string retval = vaultPath.Replace(vaultRoot, localRoot);
            retval = retval.Replace('/', '\\');

            return retval;
        }

        void backupCmd_Execute(object sender, CommandItemEventArgs e)
        {
            try
            {
                Backup();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Backup()
        {
            string commonPath = Util.GetLocalVaultCommonFolder();
            string settingsPath = Util.GetLocalVaultSettingsFolder();
            if (settingsPath == null)
            {
                MessageBox.Show("Error unknown Vault product");
                return;
            }

            if (!Directory.Exists(settingsPath))
            {
                MessageBox.Show("Error folder " + settingsPath + " does not exist");
                return;
            }


            SaveFileDialog dlg = new SaveFileDialog();
            DateTime now = DateTime.Now;
            dlg.FileName = string.Format("VaultSettingsBackup-{0}-{1}-{2}.zip",
                now.Year, now.Month, now.Day);
            dlg.Title = "Backup Vault User Settings";

            DialogResult result = dlg.ShowDialog();
            if (result != DialogResult.OK)
                return;

            string backupPath = dlg.FileName;

            ZipFile zip = ZipFile.Create(backupPath);
            zip.BeginUpdate();

            ZipFolder(zip, commonPath, commonPath);
            ZipFolder(zip, settingsPath, settingsPath);

            zip.CommitUpdate();
            zip.Close();

            MessageBox.Show("Backup completed.");
        }

        private HashSet<string> m_excludeFiles = new HashSet<string>()
        {
            "ApplicationPreferences.xml".ToLower(),
            "LoginHistory.xml".ToLower(),
            "Login.xml".ToLower()
        };

        private void ZipFolder(ZipFile zip, string rootPath, string currentPath)
        {
            string rootFolderName = System.IO.Path.GetFileName(rootPath);
            string[] files = Directory.GetFiles(currentPath);

            if (files != null)
            {
                foreach (string file in files)
                {
                    string filename = Path.GetFileName(file);
                    if (m_excludeFiles.Contains(filename.ToLower()))
                        continue;

                    string zipPath = rootFolderName + "\\" + file.Remove(0, rootPath.Length + 1);
                    zipPath = zipPath.Replace('\\', '/');

                    zip.Add(new FileDataSource(file), zipPath);
                }
            }

            string[] folders = Directory.GetDirectories(currentPath);
            if (folders != null)
            {
                foreach (string folder in folders)
                {
                    ZipFolder(zip, rootPath, folder);
                }
            }
        }

        

        void command_Execute(object sender, CommandItemEventArgs e)
        {
            try
            {
                string vaultName = e.Context.Application.Connection.Vault;
                string serverName = e.Context.Application.Connection.Server;
                ConfigureThunderdome(serverName, vaultName, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ConfigureThunderdome(string serverName, string vaultName, CommandItemEventArgs e)
        {
            Connection conn = e.Context.Application.Connection;

            if (!Util.IsAdmin(conn))
            {
                MessageBox.Show("Only administrators can use this function");
                return;
            }

            //string userFolder = m_mgr.KnowledgeVaultService.GetVaultOption(USER_FOLDER_OPTION);
            string deployFolderPath = conn.WebServiceManager.KnowledgeVaultService.GetVaultOption(DEFAULT_FOLDER_OPTION);
            string deploymentXml = conn.WebServiceManager.KnowledgeVaultService.GetVaultOption(DEPLOYMENT_CONFIG_OPTION);
            DeploymentModel deploymentModel = DeploymentModel.Load(deploymentXml);

            Configure cfgDialog = new Configure(deployFolderPath, deploymentModel, vaultName, conn);
            DialogResult result = cfgDialog.ShowDialog();
            if (result != DialogResult.OK)
                return;

            DeploymentModel deploymentModel2 = cfgDialog.GetSelectedDataModel();

            if (!deploymentModel2.Containers.Any())
                return;

            // zip up the files and upload to Vault
            string tempFile = System.IO.Path.GetTempFileName();
            ZipFile zip = ZipFile.Create(tempFile);
            zip.BeginUpdate();


            foreach (DeploymentContainer container in deploymentModel2.Containers)
            {
                foreach (DeploymentItem item in container.DeploymentItems)
                {
                    item.Zip(zip, container);
                }
            }

            zip.CommitUpdate();
            zip.Close();

            Folder deployFolder = cfgDialog.DeploymentFolder;
            conn.WebServiceManager.KnowledgeVaultService.SetVaultOption(DEFAULT_FOLDER_OPTION, deployFolder.FullName);

            Autodesk.Connectivity.WebServices.File vaultPackage = Util.AddOrUpdateFile(
                tempFile, PACKAGE_NAME, deployFolder, conn);

            System.IO.File.Delete(tempFile);

            // we just updated the package, so we are definately up to date.
            Settings settings = Settings.Load();
            VaultEntry entry = settings.GetOrCreateEntry(serverName, vaultName);
            entry.LastUpdate = vaultPackage.CkInDate;
            settings.Save();

            deploymentXml = deploymentModel2.ToXml();
            conn.WebServiceManager.KnowledgeVaultService.SetVaultOption(DEPLOYMENT_CONFIG_OPTION, deploymentXml);

            MessageBox.Show("Deployment Created");
            e.Context.ForceRefresh = true;
            e.Context.GoToLocation = new LocationContext(SelectionTypeId.File, deployFolder.FullName + "/" + PACKAGE_NAME);
            return;
        }


        
    }
}

