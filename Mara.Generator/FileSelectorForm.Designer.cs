
namespace Mara.Generator
{
    partial class FileSelectorForm
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
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.modifiedButton = new System.Windows.Forms.Button();
            this.oriButton = new System.Windows.Forms.Button();
            this.modifiedTextBox = new System.Windows.Forms.TextBox();
            this.originalTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SaveButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(131, 264);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 21);
            this.label3.TabIndex = 36;
            this.label3.Text = "Modified files";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(134, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 21);
            this.label2.TabIndex = 35;
            this.label2.Text = "Original files";
            // 
            // modifiedButton
            // 
            this.modifiedButton.Location = new System.Drawing.Point(301, 288);
            this.modifiedButton.Name = "modifiedButton";
            this.modifiedButton.Size = new System.Drawing.Size(27, 23);
            this.modifiedButton.TabIndex = 32;
            this.modifiedButton.Text = "...";
            this.modifiedButton.UseVisualStyleBackColor = true;
            // 
            // oriButton
            // 
            this.oriButton.Location = new System.Drawing.Point(301, 118);
            this.oriButton.Name = "oriButton";
            this.oriButton.Size = new System.Drawing.Size(27, 23);
            this.oriButton.TabIndex = 31;
            this.oriButton.Text = "...";
            this.oriButton.UseVisualStyleBackColor = true;
            // 
            // modifiedTextBox
            // 
            this.modifiedTextBox.Location = new System.Drawing.Point(47, 288);
            this.modifiedTextBox.Name = "modifiedTextBox";
            this.modifiedTextBox.Size = new System.Drawing.Size(248, 23);
            this.modifiedTextBox.TabIndex = 30;
            // 
            // originalTextBox
            // 
            this.originalTextBox.Location = new System.Drawing.Point(47, 118);
            this.originalTextBox.Name = "originalTextBox";
            this.originalTextBox.Size = new System.Drawing.Size(248, 23);
            this.originalTextBox.TabIndex = 29;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(96, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(175, 37);
            this.label1.TabIndex = 40;
            this.label1.Text = "PATCH FILES";
            // 
            // SaveButton
            // 
            this.SaveButton.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SaveButton.Location = new System.Drawing.Point(12, 390);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(342, 48);
            this.SaveButton.TabIndex = 41;
            this.SaveButton.Text = "SAVE INFORMATION";
            this.SaveButton.UseVisualStyleBackColor = true;
            // 
            // FileSelectorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 450);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.modifiedButton);
            this.Controls.Add(this.oriButton);
            this.Controls.Add(this.modifiedTextBox);
            this.Controls.Add(this.originalTextBox);
            this.Name = "FileSelectorForm";
            this.Text = "Mara Generator - Patch Files";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button modifiedButton;
        private System.Windows.Forms.Button oriButton;
        private System.Windows.Forms.TextBox modifiedTextBox;
        private System.Windows.Forms.TextBox originalTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SaveButton;
    }
}