namespace Thunderdome
{
    partial class Configure
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Configure));
            this.m_allUsersButton = new System.Windows.Forms.Button();
            this.m_deployFolderTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.m_cancelButton = new System.Windows.Forms.Button();
            this.m_okButton = new System.Windows.Forms.Button();
            this.m_deploymentTreeView = new System.Windows.Forms.TreeView();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_allUsersButton
            // 
            this.m_allUsersButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_allUsersButton.Location = new System.Drawing.Point(366, 12);
            this.m_allUsersButton.Name = "m_allUsersButton";
            this.m_allUsersButton.Size = new System.Drawing.Size(26, 20);
            this.m_allUsersButton.TabIndex = 5;
            this.m_allUsersButton.Text = "...";
            this.m_allUsersButton.UseVisualStyleBackColor = true;
            this.m_allUsersButton.Click += new System.EventHandler(this.m_allUsersButton_Click);
            // 
            // m_deployFolderTextBox
            // 
            this.m_deployFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_deployFolderTextBox.Location = new System.Drawing.Point(114, 12);
            this.m_deployFolderTextBox.Name = "m_deployFolderTextBox";
            this.m_deployFolderTextBox.Size = new System.Drawing.Size(253, 20);
            this.m_deployFolderTextBox.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Deployment folder:";
            // 
            // m_cancelButton
            // 
            this.m_cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_cancelButton.Location = new System.Drawing.Point(312, 319);
            this.m_cancelButton.Name = "m_cancelButton";
            this.m_cancelButton.Size = new System.Drawing.Size(75, 23);
            this.m_cancelButton.TabIndex = 6;
            this.m_cancelButton.Text = "Cancel";
            this.m_cancelButton.UseVisualStyleBackColor = true;
            this.m_cancelButton.Click += new System.EventHandler(this.m_cancelButton_Click);
            // 
            // m_okButton
            // 
            this.m_okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_okButton.Location = new System.Drawing.Point(231, 319);
            this.m_okButton.Name = "m_okButton";
            this.m_okButton.Size = new System.Drawing.Size(75, 23);
            this.m_okButton.TabIndex = 7;
            this.m_okButton.Text = "OK";
            this.m_okButton.UseVisualStyleBackColor = true;
            this.m_okButton.Click += new System.EventHandler(this.m_okButton_Click);
            // 
            // m_deploymentTreeView
            // 
            this.m_deploymentTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_deploymentTreeView.CheckBoxes = true;
            this.m_deploymentTreeView.Location = new System.Drawing.Point(16, 67);
            this.m_deploymentTreeView.Name = "m_deploymentTreeView";
            this.m_deploymentTreeView.Size = new System.Drawing.Size(371, 246);
            this.m_deploymentTreeView.TabIndex = 9;
            this.m_deploymentTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.m_deploymentTreeView_AfterCheck);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Deployment Contents:";
            // 
            // Configure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 354);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.m_deploymentTreeView);
            this.Controls.Add(this.m_okButton);
            this.Controls.Add(this.m_cancelButton);
            this.Controls.Add(this.m_allUsersButton);
            this.Controls.Add(this.m_deployFolderTextBox);
            this.Controls.Add(this.label2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Configure";
            this.Text = "Create Deployment";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_allUsersButton;
        private System.Windows.Forms.TextBox m_deployFolderTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button m_cancelButton;
        private System.Windows.Forms.Button m_okButton;
        private System.Windows.Forms.TreeView m_deploymentTreeView;
        private System.Windows.Forms.Label label3;
    }
}