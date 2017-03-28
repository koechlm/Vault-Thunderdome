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
using System.Text.RegularExpressions;
using System.Xml;

using Autodesk.Connectivity.Extensibility.Framework;
using Autodesk.Connectivity.WebServicesTools;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;

namespace Thunderdome
{

    /// <summary>
    /// Controlls the entire data model
    /// </summary>
    public class MasterController
    {
        public List<DeploymentContainerController> ContainerControllers;

        public MasterController(Connection conn, string vaultName)
        {
            ContainerControllers = new List<DeploymentContainerController>()
            {
                new ConfigFilesController(conn, vaultName),
                new SavedSearchesController(conn, vaultName),
                new ExtensionsController(conn, vaultName),
                new DecoController(conn, vaultName),
                new DataStandardController(conn, vaultName),
                //new VLogicController(conn, vaultName),
            };
        }

        /// <summary>
        /// Constructs a data model based on files on disk.
        /// </summary>
        /// <returns></returns>
        public DeploymentModel DetectDeploymentModel()
        {
            DeploymentModel model = new DeploymentModel();
            model.Containers = new List<DeploymentContainer>();

            foreach (DeploymentContainerController containerController in ContainerControllers)
            {
                model.Containers.Add(containerController.DetectItems());
            }

            return model;
        }


        public void SetMoveOperations(string folder, UtilSettings utilSettings)
        {
            string [] subFolders = Directory.GetDirectories(folder);
            if (subFolders == null)
                return;

            foreach (string subFolder in subFolders)
            {
                string subName = Path.GetFileName(subFolder);
                DeploymentContainerController controller = ContainerControllers.SingleOrDefault(
                    n => string.Equals(n.Key, subName, StringComparison.InvariantCultureIgnoreCase));

                if (controller == null)
                    continue;

                controller.SetMoveOperations(subFolder, utilSettings);
            }
        }
    }


    /// <summary>
    /// controls how things are packaged, deployed for a set of items
    /// </summary>
    public abstract class DeploymentContainerController
    {
        protected Connection m_conn;
        protected string m_vaultName;
        public string Key {get; protected set;}

        public DeploymentContainerController(Connection conn, string vaultName, string key)
        {
            m_conn = conn;
            m_vaultName = vaultName;
            this.Key = key;
        }

        protected void SetFileMoveOperation(string filename, string tempFolder, string localFolder, UtilSettings utilSettings)
        {
            // if the file exists in the temp location, mark it for move
            string tempFile = Path.Combine(tempFolder, filename);
            if (System.IO.File.Exists(tempFile))
                utilSettings.FileMoveOperations.Add(new FileMove(tempFile, Path.Combine(localFolder, filename)));
        }


        /// <param name="recurseFolders">If false, only get the files within the folder.
        /// If true, get the files in the folder and any sub-folders</param>
        protected void SetFolderMergeMoveOperations(string tempFolder, string localFolder, UtilSettings utilSettings, bool recurseFolders)
        {
            if (System.IO.Directory.Exists(tempFolder))
            {
                if (!tempFolder.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    tempFolder = tempFolder + Path.DirectorySeparatorChar;

                string[] controlPaths = null;

                if (!recurseFolders)
                    controlPaths = Directory.GetFiles(tempFolder);
                else
                    controlPaths = Directory.GetFiles(tempFolder, "*", SearchOption.AllDirectories);

                if (controlPaths != null)
                {
                    foreach (string controlPath in controlPaths)
                    {
                        //string fileName = Path.GetFileName(controlPath);
                        string fileName = controlPath.Substring(tempFolder.Length);
                        SetFileMoveOperation(fileName, tempFolder, localFolder, utilSettings);
                    }
                }
            }
        }

        public abstract DeploymentContainer DetectItems();
        public abstract void SetMoveOperations(string folder, UtilSettings utilSettings);
    }

    public class ConfigFilesController : DeploymentContainerController
    {
        public ConfigFilesController(Connection conn, string vaultName)
            : base(conn, vaultName, "Autodesk.ConfigXML")
        { }

        public override void SetMoveOperations(string folder, UtilSettings utilSettings)
        {
            try
            {
                Uri uri = new Uri(m_conn.WebServiceManager.AuthService.Url);

                string localSettingFolder = Util.GetCurrentVaultCommonFolder(uri.Host, m_vaultName);
                SetFileMoveOperation("Shortcuts.xml", folder, Path.Combine(localSettingFolder, "Objects"), utilSettings);

                localSettingFolder = Util.GetCurrentVaultCommonFolder2(uri.Host, m_vaultName);
                SetFileMoveOperation("WorkingFolders.xml", folder, Path.Combine(localSettingFolder, "Objects"), utilSettings);

                localSettingFolder = Util.GetCurrentVaultSettingsFolder(uri.Host, m_vaultName);
                SetFileMoveOperation("FilterConfig.xml", folder, localSettingFolder, utilSettings);
                SetFileMoveOperation("GridConfiguration.xml", folder, localSettingFolder, utilSettings);

                localSettingFolder = Path.Combine(localSettingFolder, "Objects");
                SetFileMoveOperation("GridState.xml", folder, localSettingFolder, utilSettings);
                SetFileMoveOperation("ViewStyles.xml", folder, localSettingFolder, utilSettings);
            }
            catch
            { }
        }

        public override DeploymentContainer DetectItems()
        {
            DeploymentContainer container = new DeploymentContainer("Configuration Files", Key );
            try
            {
                Uri uri = new Uri(m_conn.WebServiceManager.AuthService.Url);
                string localSettingFolder = Util.GetCurrentVaultCommonFolder(uri.Host, m_vaultName);

                string shortcutsPath = System.IO.Path.Combine(localSettingFolder, "Objects", "Shortcuts.xml");
                if (System.IO.File.Exists(shortcutsPath))
                    container.DeploymentItems.Add(new DeploymentFile(shortcutsPath, "Shortcuts"));

                localSettingFolder = Util.GetCurrentVaultCommonFolder2(uri.Host, m_vaultName);
                string workingFolder = System.IO.Path.Combine(localSettingFolder, "Objects", "WorkingFolders.xml");
                if (System.IO.File.Exists(workingFolder))
                    container.DeploymentItems.Add(new DeploymentFile(workingFolder, "Working Folders"));

                localSettingFolder = Util.GetCurrentVaultSettingsFolder(uri.Host, m_vaultName);

                string filterConfigPath = System.IO.Path.Combine(localSettingFolder, "FilterConfig.xml");
                if (System.IO.File.Exists(filterConfigPath))
                    container.DeploymentItems.Add(new DeploymentFile(filterConfigPath, "Filter Configuration"));

                string gridConfigPath = System.IO.Path.Combine(localSettingFolder, "GridConfiguration.xml");
                if (System.IO.File.Exists(gridConfigPath))
                    container.DeploymentItems.Add(new DeploymentFile(gridConfigPath, "Grid Configuration"));

                string gridStatePath = System.IO.Path.Combine(localSettingFolder, "Objects", "GridState.xml");
                if (System.IO.File.Exists(gridStatePath))
                    container.DeploymentItems.Add(new DeploymentFile(gridStatePath, "Grid State"));

                string viewStylesPath = System.IO.Path.Combine(localSettingFolder, "Objects", "ViewStyles.xml");
                if (System.IO.File.Exists(viewStylesPath))
                    container.DeploymentItems.Add(new DeploymentFile(viewStylesPath, "View Styles"));
            }
            catch
            { }

            return container;
        }
    }

    public class SavedSearchesController : DeploymentContainerController
    {
        public SavedSearchesController(Connection conn, string vaultName)
            : base(conn, vaultName, "Autodesk.SavedSearch")
        { }

        public override DeploymentContainer DetectItems()
        {
            DeploymentContainer container = new DeploymentContainer("Saved Searches", Key);
            try
            {
                Uri uri = new Uri(m_conn.WebServiceManager.AuthService.Url);
                string localSettingFolder = Util.GetCurrentVaultCommonFolder(uri.Host, m_vaultName);

                string searchesPath = System.IO.Path.Combine(localSettingFolder, "Searches");
                if (System.IO.Directory.Exists(searchesPath))
                {
                    string[] searchPaths = System.IO.Directory.GetFiles(searchesPath);
                    foreach (string searchPath in searchPaths)
                    {
                        string name = "Saved Search: " + System.IO.Path.GetFileName(searchPath);
                        container.DeploymentItems.Add(new DeploymentFile(searchPath, name));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ex.ToString());
            }

            return container;
        }

        public override void SetMoveOperations(string folder, UtilSettings utilSettings)
        {
            try
            {
                Uri uri = new Uri(m_conn.WebServiceManager.AuthService.Url);
                string localSettingFolder = Util.GetCurrentVaultCommonFolder(uri.Host, m_vaultName);

                string searchesPath = System.IO.Path.Combine(localSettingFolder, "Searches");
                
                if (System.IO.Directory.Exists(folder))
                {
                    string[] searchPaths = System.IO.Directory.GetFiles(folder);
                    foreach (string searchPath in searchPaths)
                    {
                        string filename = Path.GetFileName(searchPath);
                        utilSettings.FileMoveOperations.Add(new FileMove(
                            searchPath, Path.Combine(searchesPath, filename)));
                    }
                }
            }
            catch
            { }
        }
    }


    public class ExtensionsController : DeploymentContainerController
    {
        public ExtensionsController(Connection mgr, string vaultName)
            : base(mgr, vaultName, "Autodesk.Extensions")
        { }

        public override DeploymentContainer DetectItems()
        {
            DeploymentContainer container = new DeploymentContainer("Plug-ins", Key);
            try
            {
                ExtensionLoader loader = new ExtensionLoader();
                HashSet<string> folders = new HashSet<string>();

                ICollection<Extension<Autodesk.Connectivity.Explorer.Extensibility.IExplorerExtension>> extensions1 = 
                    loader.FindExtensions<Autodesk.Connectivity.Explorer.Extensibility.IExplorerExtension>();
                foreach (Extension ext in extensions1)
                {
                    AddExtension(container, ext, folders);
                }

                ICollection<Extension<Autodesk.Connectivity.WebServices.IWebServiceExtension>> extensions2 = 
                    loader.FindExtensions<Autodesk.Connectivity.WebServices.IWebServiceExtension>();
                foreach (Extension ext in extensions2)
                {
                    AddExtension(container, ext, folders);
                }

                ICollection<Extension<Autodesk.Connectivity.JobProcessor.Extensibility.IJobHandler>> extensions3 = 
                    loader.FindExtensions<Autodesk.Connectivity.JobProcessor.Extensibility.IJobHandler>();
                foreach (Extension ext in extensions3)
                {
                    AddExtension(container, ext, folders);
                }
            }
            catch
            { }

            return container;
        }

        private void AddExtension(DeploymentContainer container, Extension ext, HashSet<string> folders) 
        {
            string[] tokens = ext.ExtensionTypeString.Split(','.ToSingleArray());
            string assemblyName = tokens[1].Trim();

            if (assemblyName == "Thunderdome")
                return;

            string location = Path.GetDirectoryName(ext.Location);
            location = Path.Combine(location, assemblyName + ".dll");

            if (!CheckExtension(location))
                return;

            // make sure we don't add an entry more than once
            if (folders.Contains(location))
                return;

            folders.Add(location);
            string name = System.IO.Path.GetFileName(location);
            string folder = System.IO.Path.GetDirectoryName(location);
            container.DeploymentItems.Add(new DeploymentFolder(folder, name));
        }

        public override void SetMoveOperations(string folder, UtilSettings utilSettings)
        {
            try
            {
                string extFolder = ExtensionLoader.DefaultExtensionsFolder;

                if (System.IO.Directory.Exists(folder))
                {
                    ExtensionLoader loader = new ExtensionLoader();
                    ExtensionFolder loaderFolder = new ExtensionFolder(folder, ExtensionFolder.SearchTypeEnum.OneLevelOnly, false);
                    loader.SetExtensionFolders(loaderFolder.ToSingleArray());

                    List<string> validExtensions = new List<string>();

                    ICollection<Extension<Autodesk.Connectivity.Explorer.Extensibility.IExplorerExtension>> extensions1 =
                        loader.FindExtensions<Autodesk.Connectivity.Explorer.Extensibility.IExplorerExtension>();
                    foreach (Extension ext in extensions1)
                    {
                        validExtensions.Add(System.IO.Directory.GetParent(ext.Location).FullName);
                    }

                    ICollection<Extension<Autodesk.Connectivity.WebServices.IWebServiceExtension>> extensions2 =
                        loader.FindExtensions<Autodesk.Connectivity.WebServices.IWebServiceExtension>();
                    foreach (Extension ext in extensions2)
                    {
                        validExtensions.Add(System.IO.Directory.GetParent(ext.Location).FullName);
                    }

                    ICollection<Extension<Autodesk.Connectivity.JobProcessor.Extensibility.IJobHandler>> extensions3 =
                        loader.FindExtensions<Autodesk.Connectivity.JobProcessor.Extensibility.IJobHandler>();
                    foreach (Extension ext in extensions3)
                    {
                        validExtensions.Add(System.IO.Directory.GetParent(ext.Location).FullName);
                    }

                    //string[] subFolders = System.IO.Directory.GetDirectories(folder);
                    foreach (string subFolder in validExtensions)
                    {
                        string folderName = Path.GetFileName(subFolder);
                        utilSettings.FolderMoveOperations.Add(new FolderMove(
                            subFolder, Path.Combine(extFolder, folderName)));
                    }
                }
            }
            catch
            { }
        }


        /// <summary>
        /// Check to make sure it's an allowed plug-in
        /// </summary>
        private bool CheckExtension(string assemblyFile)
        {
            byte [] onesAndZerosKey = {0x0c,0xc2,0x9e,0x10,0x18,0x77,0x03,0x14};
            byte[] tdKey = { 0xbd, 0xd0, 0x06, 0xd5, 0x31, 0x5f, 0x17, 0x27 };
            List<byte[]> keys = new List<byte[]>() 
            {
                onesAndZerosKey, 
                tdKey
            };

            try
            {

                if (!System.IO.File.Exists(assemblyFile))
                    return false;

                System.Reflection.Assembly assembly = System.Reflection.Assembly.ReflectionOnlyLoadFrom(assemblyFile);
                if (assembly == null)
                    return false;

                byte [] assemblyKey = assembly.GetName().GetPublicKeyToken();
                if (keys.Any(n => Util.BinCompare(n, assemblyKey)))
                    return true;  
            }
            catch
            { }

            return false;
        }
    }


    public class DecoController : DeploymentContainerController
    {
        public DecoController(Connection conn, string vaultName)
            : base(conn, vaultName, "Autodesk.Deco")
        { }

        public override DeploymentContainer DetectItems()
        {
            DeploymentContainer container = new DeploymentContainer("DECO Files", Key);
            try
            {
                string decoFolder = ExtensionLoader.DefaultExtensionsFolder;
                decoFolder = System.IO.Path.Combine(decoFolder, "Deco");

                string decoSettingsPath = System.IO.Path.Combine(decoFolder, "Settings.xml");
                if (System.IO.File.Exists(decoSettingsPath))
                    container.DeploymentItems.Add(new DeploymentFile(decoSettingsPath, "DECO settings"));

                string decoControlsPath = System.IO.Path.Combine(decoFolder, "Controls");
                if (System.IO.Directory.Exists(decoControlsPath))
                {
                    string[] controlPaths = System.IO.Directory.GetFiles(decoControlsPath);
                    if (controlPaths != null)
                    {
                        foreach (string controlPath in controlPaths)
                        {
                            string name = "Deco File: " + System.IO.Path.GetFileName(controlPath);
                            container.DeploymentItems.Add(new DeploymentFile(controlPath, name));
                        }
                    }
                }
            }
            catch
            { }

            return container;
        }

        public override void SetMoveOperations(string folder, UtilSettings utilSettings)
        {
            try
            {
                string decoFolder = ExtensionLoader.DefaultExtensionsFolder;
                decoFolder = System.IO.Path.Combine(decoFolder, "Deco");
                string decoControlsPath = System.IO.Path.Combine(decoFolder, "Controls");

                string [] tempFiles = Directory.GetFiles(folder);

                if (tempFiles == null)
                    return;

                foreach (string tempFile in tempFiles)
                {
                    string filename = Path.GetFileName(tempFile);

                    if (string.Equals(filename, "Settings.xml", StringComparison.InvariantCultureIgnoreCase))
                        SetFileMoveOperation(filename, folder, decoFolder, utilSettings);
                    else
                        SetFileMoveOperation(filename, folder, decoControlsPath, utilSettings);
                }
            }
            catch
            { }
        }
    }

    public class DataStandardController : DeploymentContainerController
    {
        public DataStandardController(Connection conn, string vaultName)
            : base(conn, vaultName, "Autodesk.dataStandard")
        { }


        public override DeploymentContainer DetectItems()
        {
            DeploymentContainer container = new DeploymentContainer("Data Standard Customizations", Key);
            try
            {
                string dsFolder = ExtensionLoader.DefaultExtensionsFolder;
                dsFolder = System.IO.Path.Combine(dsFolder, "DataStandard");

                string[] folders = System.IO.Directory.GetDirectories(dsFolder);

                Regex regex1 = new Regex(@"^\w\w-\w\w$");
                Regex regex2 = new Regex(@"^\w\w$");

                foreach (string folder in folders)
                {
                    string folderName = System.IO.Path.GetFileName(folder);
                    if (folderName.Equals("Vault", StringComparison.InvariantCultureIgnoreCase))
                        container.DeploymentItems.Add(new DeploymentFolder(folder, "Vault"));
                    else if (folderName.Equals("CAD", StringComparison.InvariantCultureIgnoreCase))
                        container.DeploymentItems.Add(new DeploymentFolder(folder, "CAD"));
                    else if (regex1.IsMatch(folderName) || regex2.IsMatch(folderName))
                        container.DeploymentItems.Add(new DeploymentFolder(folder, folderName + " (localization strings)"));
                }

            }
            catch
            { }

            return container;
        }

        public override void SetMoveOperations(string folder, UtilSettings utilSettings)
        {
            try
            {
                string dsFolder = ExtensionLoader.DefaultExtensionsFolder;
                dsFolder = System.IO.Path.Combine(dsFolder, "DataStandard");

                string[] tempFolders = Directory.GetDirectories(folder);

                if (tempFolders == null)
                    return;

                foreach (string tempFolder in tempFolders)
                {
                    string folderName = Path.GetFileName(tempFolder);
                    string localFolder = Path.Combine(dsFolder, folderName);

                    SetFolderMergeMoveOperations(tempFolder, localFolder, utilSettings, true);
                    
                }
            }
            catch
            { }
        }
    }

    //public class VLogicController : DeploymentContainerController
    //{
    //    public VLogicController(Connection conn, string vaultName)
    //        : base(conn, vaultName, "Autodesk.vLogic")
    //    { }

    //    public override DeploymentContainer DetectItems()
    //    {
    //        DeploymentContainer container = new DeploymentContainer("vLogic Scripts", Key);
    //        try
    //        {
    //            string vLogicFolder = ExtensionLoader.DefaultExtensionsFolder;
    //            vLogicFolder = System.IO.Path.Combine(vLogicFolder, "vLogic");

    //            string vLogicCommandPath = System.IO.Path.Combine(vLogicFolder, "Command Scripts");
    //            if (System.IO.Directory.Exists(vLogicCommandPath))
    //            {
    //                string[] commandPaths = System.IO.Directory.GetFiles(vLogicCommandPath);
    //                if (commandPaths != null)
    //                {
    //                    foreach (string commandPath in commandPaths)
    //                    {
    //                        string name = "Command Script: " + System.IO.Path.GetFileName(commandPath);
    //                        container.DeploymentItems.Add(new DeploymentFile(commandPath, name, "Command Scripts"));
    //                    }
    //                }
    //            }


    //            string vLogicEventPath = System.IO.Path.Combine(vLogicFolder, "Event Scripts");
    //            if (System.IO.Directory.Exists(vLogicEventPath))
    //            {
    //                string[] eventPaths = System.IO.Directory.GetFiles(vLogicEventPath);
    //                if (eventPaths != null)
    //                {
    //                    foreach (string eventPath in eventPaths)
    //                    {
    //                        string fileName = System.IO.Path.GetFileName(eventPath);
    //                        if (fileName.StartsWith("_"))
    //                            continue;

    //                        string name = "Event Script: " + fileName;
    //                        container.DeploymentItems.Add(new DeploymentFile(eventPath, name, "Event Scripts"));
    //                    }
    //                }
    //            }
    //        }
    //        catch
    //        { }

    //        return container;
    //    }


    //    public override void  SetMoveOperations(string folder, UtilSettings utilSettings)
    //    {
    //        try
    //        {
    //            string vLogicFolder = ExtensionLoader.DefaultExtensionsFolder;
    //            vLogicFolder = Path.Combine(vLogicFolder, "vLogic");

    //            string vLogicCommandPath = Path.Combine(vLogicFolder, "Command Scripts");
    //            string tempCommandPath = Path.Combine(folder, "Command Scripts");
    //            SetFolderMergeMoveOperations(tempCommandPath, vLogicCommandPath, utilSettings);

    //            string vLogicEventPath = System.IO.Path.Combine(vLogicFolder, "Event Scripts");
    //            string tempEventPath = System.IO.Path.Combine(folder, "Event Scripts");
    //            SetFolderMergeMoveOperations(tempEventPath, vLogicEventPath, utilSettings);
    //        }
    //        catch
    //        { }

    //    }
    //}

    public class FileDataSource : ICSharpCode.SharpZipLib.Zip.IStaticDataSource
    {
        private string m_path;

        public FileDataSource(string path)
        {
            m_path = path;
        }

        public System.IO.Stream GetSource()
        {
            System.IO.FileStream stream = new System.IO.FileStream(m_path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            return stream;
        }
    }
}
