using System;
using System.Windows.Forms;

namespace Mara.Generator
{
    public partial class PatcherForm : Form
    {
        private bool save;
        private string[] oriData;
        public PatcherForm(string[] data)
        {
            InitializeComponent();
            if (data == null)
                return;

            titleTextBox.Text = data[0];
            creditsTextBox.Text = data[1];
            bgTextBox.Text = data[2];
            oriData = data;
        }

        public string[] GetData()
        {
            this.ShowDialog();
            if (save)
                return new[]
                {
                    titleTextBox.Text,
                    creditsTextBox.Text,
                    bgTextBox.Text
                };

            return oriData;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            save = true;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using var fbd = new OpenFileDialog()
            {
                Filter = "Image files (*.png, *.jpg)|*.jpg;*.png"
            };
            var result = fbd.ShowDialog();

            bgTextBox.Text = result == DialogResult.OK ? fbd.FileName : string.Empty;
        }
    }
}
