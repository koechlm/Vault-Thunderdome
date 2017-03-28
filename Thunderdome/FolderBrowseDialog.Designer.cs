namespace Thunderdome
{
    partial class FolderBrowseDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FolderBrowseDialog));
            this.m_folderBrowseControl = new FolderBrowseControl();
            this.m_okButton = new System.Windows.Forms.Button();
            this.m_cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_folderBrowseControl
            // 
            resources.ApplyResources(this.m_folderBrowseControl, "m_folderBrowseControl");
            this.m_folderBrowseControl.Name = "m_folderBrowseControl";
            // 
            // m_okButton
            // 
            resources.ApplyResources(this.m_okButton, "m_okButton");
            this.m_okButton.Name = "m_okButton";
            this.m_okButton.UseVisualStyleBackColor = true;
            this.m_okButton.Click += new System.EventHandler(this.m_okButton_Click);
            // 
            // m_cancelButton
            // 
            resources.ApplyResources(this.m_cancelButton, "m_cancelButton");
            this.m_cancelButton.Name = "m_cancelButton";
            this.m_cancelButton.UseVisualStyleBackColor = true;
            this.m_cancelButton.Click += new System.EventHandler(this.m_cancelButton_Click);
            // 
            // FolderBrowseDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_okButton);
            this.Controls.Add(this.m_cancelButton);
            this.Controls.Add(this.m_folderBrowseControl);
            this.Name = "FolderBrowseDialog";
            this.ResumeLayout(false);

        }

        #endregion

        private FolderBrowseControl m_folderBrowseControl;
        private System.Windows.Forms.Button m_okButton;
        private System.Windows.Forms.Button m_cancelButton;
    }
}