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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.WebServicesTools;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;

namespace Thunderdome
{
    /// <summary>
    /// A dialog which allows the user to select a Vault folder.
    /// </summary>
    public partial class FolderBrowseDialog : Form
    {
        /// <summary>
        /// Gets the currently selected folder.  The value will be null if no folder is selected.
        /// </summary>
        public Folder SelectedFolder
        {
            get
            {
                return m_folderBrowseControl.SelectedFolder;
            }
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="docSvc">The DocumentService object to use when populating the control.</param>
        public FolderBrowseDialog(Connection conn)
        {
            InitializeComponent();

            m_folderBrowseControl.VaultConnection = conn;
            m_folderBrowseControl.InitControl();
        }

        private void m_cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void m_okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

       
    }
}
