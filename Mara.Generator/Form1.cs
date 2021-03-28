using System;
using System.Windows.Forms;

namespace Mara.Generator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
