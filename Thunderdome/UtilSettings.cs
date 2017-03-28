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
using System.Xml.Serialization;

namespace Thunderdome
{
    [XmlRoot("FileMove")]
    public class FileMove
    {
        public string From;
        public string To;

        public FileMove()
        {   }

        public FileMove(string from, string to)
        {
            this.From = from;
            this.To = to;
        }
    }

    [XmlRoot("FolderMove")]
    public class FolderMove
    {
        public string From;
        public string To;

        public FolderMove()
        { }

        public FolderMove(string from, string to)
        {
            this.From = from;
            this.To = to;
        }
    }

    /// <summary>
    /// Settings for the Vault client to pass to the utility.
    /// </summary>
    [XmlRoot("UtilSettings")]
    public class UtilSettings
    {
        [XmlElement("VaultClient")]
        public string VaultClient;

        [XmlElement("TempFolder")]
        public string TempFolder;

        [XmlElement("FileMoveOperations")]
        public List<FileMove> FileMoveOperations;

        [XmlElement("FolderMoveOperations")]
        public List<FolderMove> FolderMoveOperations;

        [XmlElement("DeleteOperations")]
        public List<string> DeleteOperations;

        [XmlIgnore]
        public static string FILE_NAME = "utilSettings.xml";

        public UtilSettings()
        {
            FileMoveOperations = new List<FileMove>();
            FolderMoveOperations = new List<FolderMove>();
            DeleteOperations = new List<string>();
        }

        public void Save()
        {
            try
            {
                string codeFolder = Util.GetAssemblyPath();
                string xmlPath = Path.Combine(codeFolder, FILE_NAME);

                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(xmlPath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(UtilSettings));
                    serializer.Serialize(writer, this);
                }
            }
            catch
            { }
        }

        public static UtilSettings Load()
        {
            UtilSettings retVal = new UtilSettings();

            try
            {
                string codeFolder = Util.GetAssemblyPath();
                string xmlPath = Path.Combine(codeFolder, FILE_NAME);

                using (System.IO.StreamReader reader = new System.IO.StreamReader(xmlPath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(UtilSettings));
                    retVal = (UtilSettings)serializer.Deserialize(reader);
                }
            }
            catch
            { }

            return retVal;
        }
    }
}
