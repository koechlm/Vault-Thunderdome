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

using Autodesk.Connectivity.Extensibility.Framework;
using Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.WebServicesTools;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;

namespace Thunderdome
{
    public partial class Configure : Form
    {
        private Connection m_conn;
        private bool m_init;

        private string FormatPath(string str)
        {
            string retVal = str.Trim();
            if (retVal.Length > 0 && !retVal.EndsWith("/"))
                retVal += "/";
            return retVal;
        }

        public Folder DeploymentFolder = null;

        private DeploymentModel m_deploymentModel;

        public Configure(string deployFolder, DeploymentModel deploymentModel, string vaultName, Connection conn)
        {
            InitializeComponent();

            m_init = true;
            m_conn = conn;
            m_deployFolderTextBox.Text = deployFolder;
            m_deploymentModel = deploymentModel;

            if (m_deploymentModel == null)
                m_deploymentModel = new DeploymentModel();
            if (m_deploymentModel.Containers == null)
                m_deploymentModel.Containers = new List<DeploymentContainer>();

            InitTree(vaultName);
            m_init = false;
        }

        private void InitTree(string vaultName)
        {
            MasterController mc = new MasterController(m_conn, vaultName);

            foreach (DeploymentContainerController controller in mc.ContainerControllers)
            {
                DeploymentContainer container = controller.DetectItems();
                DeploymentContainer selectedContainer = m_deploymentModel.Containers.FirstOrDefault(
                    n => n.Key == container.Key);

                if (selectedContainer == null)
                    selectedContainer = new DeploymentContainer() { DeploymentItems = new List<DeploymentItem>() };

                if (container.DeploymentItems.Any())
                {
                    TreeNode node = m_deploymentTreeView.Nodes.Add(container.DisplayName);
                    node.Tag = container;

                    foreach (DeploymentItem item in container.DeploymentItems)
                    {
                        TreeNode subNode = node.Nodes.Add(item.DisplayName);
                        subNode.Tag = item;

                        if (selectedContainer.DeploymentItems.Any(n => n.DisplayName == item.DisplayName))
                        {
                            subNode.Checked = true;
                            node.Checked = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Build a data model based on the selected nodes
        /// </summary>
        public DeploymentModel GetSelectedDataModel()
        {
            DeploymentModel model = new DeploymentModel();
            model.Containers = new List<DeploymentContainer>();

            foreach (TreeNode containerNode in m_deploymentTreeView.Nodes)
            {
                DeploymentContainer container = containerNode.Tag as DeploymentContainer;
                if (container == null)
                    continue;

                DeploymentContainer newContainer = new DeploymentContainer(container.DisplayName, container.Key);

                foreach (TreeNode itemNode in containerNode.Nodes)
                {
                    if (!itemNode.Checked)
                        continue;

                    DeploymentItem item = itemNode.Tag as DeploymentItem;
                    if (item == null)
                        continue;

                    newContainer.DeploymentItems.Add(item);
                }

                if (newContainer.DeploymentItems.Any())
                    model.Containers.Add(newContainer);
            }

            return model;
        }


        private void m_allUsersButton_Click(object sender, EventArgs e)
        {
            SelectFolder(m_deployFolderTextBox);
        }

        private void SelectFolder(TextBox textbox)
        {
            FolderBrowseDialog dialog = new FolderBrowseDialog(m_conn);
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                textbox.Text = dialog.SelectedFolder.FullName;
            }
        }

        private void m_cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void m_okButton_Click(object sender, EventArgs e)
        {
            string deployFolder = m_deployFolderTextBox.Text;

            if (deployFolder.Length == 0)
            {
                MessageBox.Show("You must provide a value for the deployment Folder");
                return;
            }

            Folder[] results = m_conn.WebServiceManager.DocumentService.FindFoldersByPaths(deployFolder.ToSingleArray());
            if (results == null || results.Length == 0 || results[0] == null || results[0].Cloaked || results[0].Id < 0)
            {
                MessageBox.Show("Invalid deployment folder");
                return;
            }

            this.DeploymentFolder = results[0];
            
            DialogResult = DialogResult.OK;
        }

        private void m_deploymentTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null || m_init)
                return;

            foreach (TreeNode node in e.Node.Nodes)
            {
                node.Checked = e.Node.Checked;
            }
        } 
    }
}
