using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mara.Lib;
using Mara.Lib.Configs;

namespace Mara.Generator
{
    public partial class FormInfo : Form
    {
        private MaraConfig configPassed;
        private bool savedChanges;
        public FormInfo(MaraConfig config)
        {
            InitializeComponent();
            configPassed = config;

            if (configPassed.Info != null)
            {
                ChangelogTextBox.Text = configPassed.Info.Changelog;
                PatchIdNumeric.Value = configPassed.Info.PatchId;
                PatchVersionTextBox.Text = configPassed.Info.PatchVersion;
            }
        }

        public (bool, MaraConfig) UpdateConfig()
        {
            ShowDialog();

            return (savedChanges, configPassed);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            configPassed.Info ??= new PatchInfo();

            configPassed.Info.PatchId = (int) PatchIdNumeric.Value;
            configPassed.Info.Changelog = ChangelogTextBox.Text;
            configPassed.Info.PatchVersion = PatchVersionTextBox.Text;
            savedChanges = true;
            Close();
        }
    }
}
