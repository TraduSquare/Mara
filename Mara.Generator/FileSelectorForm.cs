using System;
using System.Windows.Forms;

namespace Mara.Generator
{
    public partial class FileSelectorForm : Form
    {
        private bool savedChanges;

        public FileSelectorForm(PatchGenerator config)
        {
            InitializeComponent();

            if (config == null) return;

            originalTextBox.Text = config.OriPath;
            modifiedTextBox.Text = config.ModPath;
            outTextBox.Text = config.OutPath;
        }

        public (bool, PatchGenerator) UpdateConfig()
        {
            ShowDialog();

            return (savedChanges, new PatchGenerator(originalTextBox.Text, modifiedTextBox.Text, outTextBox.Text));
        }

        private string SearchFolder(string title)
        {
            using var fbd = new FolderBrowserDialog()
            {
                Description = title
            };
            var result = fbd.ShowDialog();

            return result == DialogResult.OK ? fbd.SelectedPath : string.Empty;
        }

        private void oriButton_Click(object sender, EventArgs e)
        {
            var result = SearchFolder("Select the folder with original files.");
            if (!string.IsNullOrWhiteSpace(originalTextBox.Text) && string.IsNullOrWhiteSpace(result))
                return;
            originalTextBox.Text = result;
        }

        private void modifiedButton_Click(object sender, EventArgs e)
        {
            var result = SearchFolder("Select the folder with modified files.");
            if (!string.IsNullOrWhiteSpace(modifiedTextBox.Text) && string.IsNullOrWhiteSpace(result))
                return;
            modifiedTextBox.Text = result;
        }

        private void outButton_Click(object sender, EventArgs e)
        {
            var result = SearchFolder("Select the out folder.");
            if (!string.IsNullOrWhiteSpace(outTextBox.Text) && string.IsNullOrWhiteSpace(result))
                return;
            outTextBox.Text = result;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(originalTextBox.Text))
            {
                MessageBox.Show("Please select the folder with original files.",
                    "Folder with original files not selected.",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(modifiedTextBox.Text))
            {
                MessageBox.Show("Please select the folder with modified files.",
                    "Folder with modified files not selected.",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(outTextBox.Text))
            {
                MessageBox.Show("Please select the out folder.",
                    "Out folder not selected.",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            savedChanges = true;
            Close();
        }
    }
}
