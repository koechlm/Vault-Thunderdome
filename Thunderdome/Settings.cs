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
using System.Linq;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Thunderdome
{
    [XmlRoot("Settings")]
    public class Settings
    {
        [XmlElement("NeverDownload")]
        public bool NeverDownload;

        [XmlElement("VaultEntry")]
        public List<VaultEntry> VaultEntries;

        public Settings()
        {
            this.NeverDownload = false;
            this.VaultEntries = new List<VaultEntry>();
        }

        public VaultEntry GetOrCreateEntry(string serverName, string vaultName)
        {
            VaultEntry entry = VaultEntries.FirstOrDefault(n =>
                string.Equals(n.ServerName, serverName, StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(n.VaultName, vaultName, StringComparison.InvariantCultureIgnoreCase));
            if (entry == null)
            {
                entry = new VaultEntry()
                {
                    ServerName = serverName,
                    VaultName = vaultName,
                    LastUpdate = DateTime.MinValue
                };
                VaultEntries.Add(entry);
            }

            return entry;
        }

        public void Save()
        {
            try
            {
                string codeFolder = Util.GetAssemblyPath();
                string xmlPath = Path.Combine(codeFolder, "settings.xml");

                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(xmlPath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                    serializer.Serialize(writer, this);
                }
            }
            catch
            { }
        }

        public static Settings Load()
        {
            Settings retVal = new Settings();

            try
            {
                string codeFolder = Util.GetAssemblyPath();
                string xmlPath = Path.Combine(codeFolder, "settings.xml");

                using (System.IO.StreamReader reader = new System.IO.StreamReader(xmlPath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                    retVal = (Settings)serializer.Deserialize(reader);
                }
            }
            catch
            { }

            return retVal;
        }
    }

    public class VaultEntry
    {
        [XmlElement("LastUpdate")]
        public DateTime LastUpdate;

        [XmlElement("ServerName")]
        public string ServerName;

        [XmlElement("VaultName")]
        public string VaultName;

    }
}
