namespace Thunderdome
{
    partial class FolderBrowseControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_folderTreeView = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // m_folderTreeView
            // 
            this.m_folderTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_folderTreeView.Location = new System.Drawing.Point(0, 0);
            this.m_folderTreeView.Name = "m_folderTreeView";
            this.m_folderTreeView.Size = new System.Drawing.Size(260, 183);
            this.m_folderTreeView.TabIndex = 0;
            this.m_folderTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.m_folderTreeView_BeforeExpand);
            // 
            // FolderBrowseControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_folderTreeView);
            this.Name = "FolderBrowseControl";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView m_folderTreeView;
    }
}
