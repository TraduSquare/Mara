
namespace Mara.Generator
{
    partial class FormInfo
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
            this.label12 = new System.Windows.Forms.Label();
            this.ChangelogTextBox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.PatchVersionTextBox = new System.Windows.Forms.TextBox();
            this.PatchIdNumeric = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.LoadInfoButton = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.SaveButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PatchIdNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label12.Location = new System.Drawing.Point(301, 191);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(85, 21);
            this.label12.TabIndex = 36;
            this.label12.Text = "Changelog";
            // 
            // ChangelogTextBox
            // 
            this.ChangelogTextBox.Location = new System.Drawing.Point(12, 215);
            this.ChangelogTextBox.Multiline = true;
            this.ChangelogTextBox.Name = "ChangelogTextBox";
            this.ChangelogTextBox.Size = new System.Drawing.Size(662, 297);
            this.ChangelogTextBox.TabIndex = 35;
            this.ChangelogTextBox.Text = "Initial release";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label11.Location = new System.Drawing.Point(480, 88);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(102, 21);
            this.label11.TabIndex = 34;
            this.label11.Text = "Patch version";
            // 
            // PatchVersionTextBox
            // 
            this.PatchVersionTextBox.Location = new System.Drawing.Point(440, 112);
            this.PatchVersionTextBox.Name = "PatchVersionTextBox";
            this.PatchVersionTextBox.Size = new System.Drawing.Size(182, 23);
            this.PatchVersionTextBox.TabIndex = 33;
            this.PatchVersionTextBox.Text = "1.0";
            // 
            // PatchIdNumeric
            // 
            this.PatchIdNumeric.Location = new System.Drawing.Point(66, 112);
            this.PatchIdNumeric.Name = "PatchIdNumeric";
            this.PatchIdNumeric.Size = new System.Drawing.Size(177, 23);
            this.PatchIdNumeric.TabIndex = 32;
            this.PatchIdNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label10.Location = new System.Drawing.Point(12, 138);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(285, 30);
            this.label10.TabIndex = 31;
            this.label10.Text = "If you want to upload an update, make sure to give it\r\na higher number than the o" +
    "riginal";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LoadInfoButton
            // 
            this.LoadInfoButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LoadInfoButton.Location = new System.Drawing.Point(12, 12);
            this.LoadInfoButton.Name = "LoadInfoButton";
            this.LoadInfoButton.Size = new System.Drawing.Size(153, 32);
            this.LoadInfoButton.TabIndex = 30;
            this.LoadInfoButton.Text = "Load previous info";
            this.LoadInfoButton.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label9.Location = new System.Drawing.Point(122, 88);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(64, 21);
            this.label9.TabIndex = 29;
            this.label9.Text = "Patch Id";
            // 
            // SaveButton
            // 
            this.SaveButton.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SaveButton.Location = new System.Drawing.Point(12, 539);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(662, 48);
            this.SaveButton.TabIndex = 37;
            this.SaveButton.Text = "SAVE INFORMATION";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(193, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(300, 37);
            this.label1.TabIndex = 38;
            this.label1.Text = "PATCH INFORMATION";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(389, 138);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(285, 30);
            this.label2.TabIndex = 39;
            this.label2.Text = "If you want to upload an update, make sure to give it\r\na different version than t" +
    "he original";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 599);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.ChangelogTextBox);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.PatchVersionTextBox);
            this.Controls.Add(this.PatchIdNumeric);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.LoadInfoButton);
            this.Controls.Add(this.label9);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormInfo";
            this.Text = "Mara Generator - Patch Information";
            ((System.ComponentModel.ISupportInitialize)(this.PatchIdNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox ChangelogTextBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox PatchVersionTextBox;
        private System.Windows.Forms.NumericUpDown PatchIdNumeric;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button LoadInfoButton;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}