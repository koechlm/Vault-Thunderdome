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

using Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.WebServicesTools;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using Autodesk.DataManagement.Client.Framework.Vault.Settings;
using Autodesk.DataManagement.Client.Framework.Vault.Results;
using VDF = Autodesk.DataManagement.Client.Framework;

namespace Thunderdome
{
    public class Util
    {
        private static string COMMON_FOLDER_1 = "Services_Security_1_22_2014";
        private static string COMMON_FOLDER_2 = "Services_Security_6_29_2011";
        private static string VAULT_PRO_FOLDER_NAME = "Autodesk Vault Professional 2018";
        private static string VAULT_WG_FOLDER_NAME = "Autodesk Vault Workgroup 2018";

        public static void DoAction(Action a)
        {
            try
            {
                a();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public static string GetAssemblyPath()
        {
            string prefix = "file:///";
            string codebase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            if (codebase.StartsWith(prefix))
                codebase = codebase.Substring(prefix.Length);

            return Path.GetDirectoryName(codebase);
        }

        /// <summary>
        /// Tells if the logged in user is an admin or not.
        /// </summary>
        /// <returns>True if the user is an administrator. Otherwise false.</returns>
        public static bool IsAdmin(Connection conn)
        {
            long userId = conn.WebServiceManager.SecurityService.SecurityHeader.UserId;
            if (userId > 0)
            {
                Permis[] permissions = conn.WebServiceManager.AdminService.GetPermissionsByUserId(userId);

                // assume that if the current user has the AdminUserRead permission,
                // then they are an admin.
                long adminUserRead = 82;

                foreach (Permis p in permissions)
                {
                    if (p.Id == adminUserRead)
                        return true;
                }
            }
            return false;
        }

        public static Autodesk.Connectivity.WebServices.File AddOrUpdateFile(
            string localPath, string fileName, Folder vaultFolder, Connection conn)
        {
            string vaultPath = vaultFolder.FullName + "/" + fileName;

            Autodesk.Connectivity.WebServices.File[] result = conn.WebServiceManager.DocumentService.FindLatestFilesByPaths(
                vaultPath.ToSingleArray());

            Autodesk.Connectivity.WebServices.File retVal = null;
            if (result == null || result.Length == 0 || result[0].Id < 0)
            {
                VDF.Vault.Currency.Entities.Folder vdfFolder = new VDF.Vault.Currency.Entities.Folder(
                    conn, vaultFolder);

                // using a stream so that we can set a different file name
                using (FileStream stream = new FileStream(localPath, FileMode.Open, FileAccess.Read))
                {
                    VDF.Vault.Currency.Entities.FileIteration newFile = conn.FileManager.AddFile(
                        vdfFolder, fileName, "Thunderdome deployment", 
                        System.IO.File.GetLastWriteTime(localPath), null, null,
                        FileClassification.None, false, stream);

                    retVal = newFile;
                }
            }
            else
            {
                VDF.Vault.Currency.Entities.FileIteration vdfFile = new VDF.Vault.Currency.Entities.FileIteration(conn, result[0]);

                AcquireFilesSettings settings = new AcquireFilesSettings(conn);
                settings.AddEntityToAcquire(vdfFile);
                settings.DefaultAcquisitionOption = AcquireFilesSettings.AcquisitionOption.Checkout;
                AcquireFilesResults results = conn.FileManager.AcquireFiles(settings);

                if (results.FileResults.First().Exception != null)
                    throw results.FileResults.First().Exception;

                vdfFile = results.FileResults.First().File;
                vdfFile = conn.FileManager.CheckinFile(vdfFile, "Thunderdome deployment", false,
                    null, null, false, null, FileClassification.None, false, localPath.ToVDFPath());

                retVal = vdfFile;
            }

            return retVal;
        }

        public static string GetLocalVaultCommonFolder()
        {
            string retval = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            retval = Path.Combine(retval, "Autodesk", "VaultCommon");
            return retval;
        }

        public static string GetCurrentVaultCommonFolder(string server, string vault)
        {
            string commonRoot = GetLocalVaultCommonFolder();
            string retval = Path.Combine(commonRoot, "Servers", COMMON_FOLDER_1, server, "Vaults", vault);
            return retval;
        }

        public static string GetCurrentVaultCommonFolder2(string server, string vault)
        {
            string commonRoot = GetLocalVaultCommonFolder();
            string retval = Path.Combine(commonRoot, "Servers", COMMON_FOLDER_2, server, "Vaults", vault);
            return retval;
        }

        public static string GetLocalVaultSettingsFolder()
        {
            // locate the folder with the local Vault settings
            string settingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string exeName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            string dirName = null;

            if (exeName == "Connectivity.VaultPro")
                dirName = VAULT_PRO_FOLDER_NAME;
            else if (exeName == "Connectivity.VaultWkg")
                dirName = VAULT_WG_FOLDER_NAME;
            else
                return null;

            return Path.Combine(settingsPath, "Autodesk", dirName);
        }

        public static string GetCurrentVaultSettingsFolder(string server, string vault)
        {
            string root = GetLocalVaultSettingsFolder();
            string retval = Path.Combine(root, "Servers", server, "Vaults", vault);
            return retval;
        }

        public static bool BinCompare(byte [] arrayA, byte [] arrayB)
        {
            if (arrayA == null && arrayB == null)
                return true;
            if (arrayA == null || arrayB == null)
                return false;

            if (arrayA.Length != arrayB.Length)
                return false;

            for (int i = 0; i < arrayA.Length; i++)
            {
                if (arrayA[i] != arrayB[i])
                    return false;
            }
            return true;
        }
        
    }

    internal static class ExtensionMethods
    {
        internal static T[] ToSingleArray<T>(this T obj)
        {
            return new T[] { obj };
        }

        internal static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || collection.Count() == 0;
        }

        internal static List<T> ShallowCopy<T>(this List<T> origList)
        {
            List<T> newList = new List<T>();
            newList.AddRange(origList);
            return newList;
        }

        internal static VDF.Currency.FilePathAbsolute ToVDFPath(this string localPath)
        {
            return new VDF.Currency.FilePathAbsolute(localPath);
        }
    }
}
