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

namespace Thunderdome
{
    public partial class AskDialog : Form
    {
        public enum AskResultEnum
        {
            Yes,
            No,
            Never
        }

        public AskResultEnum AskResult { get; private set; }

        public AskDialog(string updateList)
        {
            InitializeComponent();

            m_updateListTextBox.Text = updateList;
            AskResult = AskResultEnum.No;
        }

        private void m_yesButton_Click(object sender, EventArgs e)
        {
            AskResult = AskResultEnum.Yes;
            DialogResult = DialogResult.OK;
        }

        private void m_noButton_Click(object sender, EventArgs e)
        {
            AskResult = AskResultEnum.No;
            DialogResult = DialogResult.OK;
        }

        private void m_neverButton_Click(object sender, EventArgs e)
        {
            AskResult = AskResultEnum.Never;
            DialogResult = DialogResult.OK;
        }


    }
}
