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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.WebServicesTools;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;

namespace Thunderdome
{
    /// <summary>
    /// A control that displays a tree view of the Vault folder structure.
    /// </summary>
    /// <remarks>The control dynamicly builds the tree level-by-level as the user expands the tree.
    /// This way only the needed Folder objects are downloaded from the server.</remarks>
    public partial class FolderBrowseControl : UserControl
    {
        /// <summary>
        /// Gets the currently selected folder.  The value will be null if no folder is selected.
        /// </summary>
        public Folder SelectedFolder
        {
            get
            {
                TreeNode node = m_folderTreeView.SelectedNode;
                if (node == null)
                    return null;
                else
                {
                    return (Folder)node.Tag;
                }
            }
        }

        public Connection VaultConnection;


        /// <summary>
        /// Default constructor.
        /// </summary>
        public FolderBrowseControl()
        {
            InitializeComponent();

            this.VaultConnection = null;
        }

        /// <summary>
        /// Initializes the control with Vault data.
        /// Must be called after DocumentService is set.
        /// </summary>
        public void InitControl()
        {
            if (this.VaultConnection == null)
                throw new Exception("Error FolderBrowseControl does not have a Vault Connection object");

            Folder root = this.VaultConnection.WebServiceManager.DocumentService.GetFolderRoot();
            TreeNode rootNode = new TreeNode(root.FullName);
            rootNode.Tag = root;

            m_folderTreeView.Nodes.Add(rootNode);
            AddChildFolders(rootNode);
        }

        private void m_folderTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            // get the next level in the tree
            m_folderTreeView.BeginUpdate();
            foreach (TreeNode node in e.Node.Nodes)
                AddChildFolders(node);
            m_folderTreeView.EndUpdate();
        }


        /// <summary>
        /// Make a server call and populate the folder tree 1 level deep
        /// if the folders are already there, no call to the server is made.
        /// </summary>
        private void AddChildFolders(TreeNode parentNode)
        {
            Folder parentFolder = (Folder)parentNode.Tag;

            if (parentFolder.NumClds == parentNode.Nodes.Count)
                return;  // we already have the child nodes

            parentNode.Nodes.Clear();

            Folder[] childFolders = this.VaultConnection.WebServiceManager.DocumentService.GetFoldersByParentId(parentFolder.Id, false);
            foreach (Folder folder in childFolders)
            {
                TreeNode childNode = new TreeNode(folder.Name);
                childNode.Tag = folder;
                parentNode.Nodes.Add(childNode);
            }
        }
    }
}
