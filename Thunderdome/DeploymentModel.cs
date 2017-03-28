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

using ICSharpCode.SharpZipLib.Zip;

namespace Thunderdome
{
    /// <summary>
    /// A description of things in the deployment package
    /// </summary>
    [XmlRoot]
    public class DeploymentModel
    {

        [XmlElement]
        public List<DeploymentContainer> Containers;

        public DeploymentModel()
        { }

        public string ToXml()
        {
            try
            {
                using (StringWriter writer = new StringWriter())
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(DeploymentModel));
                    serializer.Serialize(writer, this);

                    return writer.ToString();
                }
            }
            catch
            { }

            return string.Empty;
        }

        public static DeploymentModel Load(string xml)
        {
            DeploymentModel retVal = null;

            try
            {
                using (StringReader reader = new StringReader(xml))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(DeploymentModel));
                    retVal = serializer.Deserialize(reader) as DeploymentModel;
                }
            }
            catch
            { }

            if (retVal == null)
                retVal = new DeploymentModel();
            return retVal;
        }

    }

    /// <summary>
    /// A collection of similar DeploymentItems
    /// </summary>
    [XmlRoot]
    [XmlInclude(typeof(DeploymentFile)), XmlInclude(typeof(DeploymentFolder))]
    public class DeploymentContainer
    {
        [XmlAttribute]
        public string Key;

        [XmlAttribute]
        public string DisplayName;

        [XmlElement]
        public List<DeploymentItem> DeploymentItems;

        /// <summary>
        /// for XML Serialization
        /// </summary>
        public DeploymentContainer()
        { }

        public DeploymentContainer(string displayName, string key)
        {
            this.DisplayName = displayName;
            this.Key = key;
            this.DeploymentItems = new List<DeploymentItem>();
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }


    /// <summary>
    /// Something that gets packaged and deployed
    /// </summary>
    [XmlRoot]
    public abstract class DeploymentItem
    {
        [XmlAttribute]
        public string DisplayName;

        public override string ToString()
        {
            return DisplayName;
        }

        public abstract void Zip(ZipFile zip, DeploymentContainer container);
    }

    
    [XmlRoot]
    public class DeploymentFile : DeploymentItem
    {
        [XmlIgnore]
        public string Path { get; private set; }

        [XmlIgnore]
        public string SubFolder { get; private set; }

        /// <summary>
        /// for XML Serialization
        /// </summary>
        public DeploymentFile()
        { }

        public DeploymentFile(string path, string displayName, string subFolder = null)
        {
            this.Path = path;
            this.DisplayName = displayName;
            this.SubFolder = subFolder;
        }

        public override void Zip(ZipFile zip, DeploymentContainer container)
        {
            string zipPath;
            if (SubFolder == null)
                zipPath = container.Key + "/" + System.IO.Path.GetFileName(Path);
            else
                zipPath = container.Key + "/" + SubFolder + "/" + System.IO.Path.GetFileName(Path);

            zip.Add(new FileDataSource(Path), zipPath);
        }
    }

    [XmlRoot]
    public class DeploymentFolder : DeploymentItem
    {
        [XmlIgnore]
        public string Path { get; private set; }

        /// <summary>
        /// for XML Serialization
        /// </summary>
        public DeploymentFolder()
        { }

        public DeploymentFolder(string path, string displayName)
        {
            this.Path = path;
            this.DisplayName = displayName;
        }

        public override void Zip(ZipFile zip, DeploymentContainer container)
        {
            ZipFolder(zip, container, Path);
        }

        private void ZipFolder(ZipFile zip, DeploymentContainer container, string currentPath)
        {
            string [] files = Directory.GetFiles(currentPath);
            string folderName = System.IO.Path.GetFileName(Path);

            if (files != null)
            {
                foreach (string file in files)
                {
                    string subPath = System.IO.Path.GetDirectoryName(file);
                    subPath = subPath.Remove(0, Path.Length);
                    subPath = subPath.Replace('\\', '/');
                    subPath = subPath.TrimStart('/'.ToSingleArray());

                    string zipPath;

                    if (subPath.Length > 0)
                        zipPath = container.Key + "/" + folderName + "/" + subPath + "/" + System.IO.Path.GetFileName(file);
                    else
                        zipPath = container.Key + "/" + folderName + "/" + System.IO.Path.GetFileName(file);

                    zip.Add(new FileDataSource(file), zipPath);
                }
            }

            string[] folders = Directory.GetDirectories(currentPath);
            if (folders != null)
            {
                foreach (string folder in folders)
                {
                    ZipFolder(zip, container, folder);
                }
            }
        }
    }
    
}
