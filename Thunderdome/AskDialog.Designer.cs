namespace Thunderdome
{
    partial class AskDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AskDialog));
            this.m_neverButton = new System.Windows.Forms.Button();
            this.m_noButton = new System.Windows.Forms.Button();
            this.m_yesButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.m_updateListTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // m_neverButton
            // 
            this.m_neverButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_neverButton.Location = new System.Drawing.Point(332, 240);
            this.m_neverButton.Name = "m_neverButton";
            this.m_neverButton.Size = new System.Drawing.Size(170, 23);
            this.m_neverButton.TabIndex = 2;
            this.m_neverButton.Text = "No, and never ask me again";
            this.m_neverButton.UseVisualStyleBackColor = true;
            this.m_neverButton.Click += new System.EventHandler(this.m_neverButton_Click);
            // 
            // m_noButton
            // 
            this.m_noButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_noButton.Location = new System.Drawing.Point(251, 240);
            this.m_noButton.Name = "m_noButton";
            this.m_noButton.Size = new System.Drawing.Size(75, 23);
            this.m_noButton.TabIndex = 1;
            this.m_noButton.Text = "No";
            this.m_noButton.UseVisualStyleBackColor = true;
            this.m_noButton.Click += new System.EventHandler(this.m_noButton_Click);
            // 
            // m_yesButton
            // 
            this.m_yesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_yesButton.Location = new System.Drawing.Point(170, 240);
            this.m_yesButton.Name = "m_yesButton";
            this.m_yesButton.Size = new System.Drawing.Size(75, 23);
            this.m_yesButton.TabIndex = 0;
            this.m_yesButton.Text = "Yes";
            this.m_yesButton.UseVisualStyleBackColor = true;
            this.m_yesButton.Click += new System.EventHandler(this.m_yesButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(267, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "There are Vault updates, would you like to install them?";
            // 
            // m_updateListTextBox
            // 
            this.m_updateListTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_updateListTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.m_updateListTextBox.CausesValidation = false;
            this.m_updateListTextBox.Location = new System.Drawing.Point(15, 25);
            this.m_updateListTextBox.Multiline = true;
            this.m_updateListTextBox.Name = "m_updateListTextBox";
            this.m_updateListTextBox.ReadOnly = true;
            this.m_updateListTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.m_updateListTextBox.Size = new System.Drawing.Size(487, 209);
            this.m_updateListTextBox.TabIndex = 4;
            // 
            // AskDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 275);
            this.Controls.Add(this.m_updateListTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.m_yesButton);
            this.Controls.Add(this.m_noButton);
            this.Controls.Add(this.m_neverButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AskDialog";
            this.Text = "Updates Found";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_neverButton;
        private System.Windows.Forms.Button m_noButton;
        private System.Windows.Forms.Button m_yesButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox m_updateListTextBox;
    }
}