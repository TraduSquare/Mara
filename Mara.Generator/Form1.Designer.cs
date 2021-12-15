﻿
namespace Mara.Generator
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.GenerateButton = new System.Windows.Forms.Button();
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonGeneric = new System.Windows.Forms.RadioButton();
            this.Button3ds = new System.Windows.Forms.RadioButton();
            this.ButtonSwitch = new System.Windows.Forms.RadioButton();
            this.ButtonPsvita = new System.Windows.Forms.RadioButton();
            this.ButtonPs4 = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.FilesButton = new System.Windows.Forms.Button();
            this.informationButton = new System.Windows.Forms.Button();
            this.patcherButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(280, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(262, 37);
            this.label1.TabIndex = 8;
            this.label1.Text = "MARA GENERATOR";
            // 
            // GenerateButton
            // 
            this.GenerateButton.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.GenerateButton.Location = new System.Drawing.Point(12, 465);
            this.GenerateButton.Name = "GenerateButton";
            this.GenerateButton.Size = new System.Drawing.Size(798, 48);
            this.GenerateButton.TabIndex = 12;
            this.GenerateButton.Text = "GENERATE PATCH";
            this.GenerateButton.UseVisualStyleBackColor = true;
            this.GenerateButton.Click += new System.EventHandler(this.GenerateButton_Click);
            // 
            // logTextBox
            // 
            this.logTextBox.Location = new System.Drawing.Point(13, 554);
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logTextBox.Size = new System.Drawing.Size(797, 102);
            this.logTextBox.TabIndex = 16;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 536);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(27, 15);
            this.label7.TabIndex = 17;
            this.label7.Text = "Log";
            // 
            // buttonGeneric
            // 
            this.buttonGeneric.AutoSize = true;
            this.buttonGeneric.Checked = true;
            this.buttonGeneric.Location = new System.Drawing.Point(41, 31);
            this.buttonGeneric.Name = "buttonGeneric";
            this.buttonGeneric.Size = new System.Drawing.Size(91, 19);
            this.buttonGeneric.TabIndex = 18;
            this.buttonGeneric.TabStop = true;
            this.buttonGeneric.Text = "Generic (PC)";
            this.buttonGeneric.UseVisualStyleBackColor = true;
            this.buttonGeneric.CheckedChanged += new System.EventHandler(this.buttonGeneric_CheckedChanged);
            // 
            // Button3ds
            // 
            this.Button3ds.AutoSize = true;
            this.Button3ds.Location = new System.Drawing.Point(185, 31);
            this.Button3ds.Name = "Button3ds";
            this.Button3ds.Size = new System.Drawing.Size(98, 19);
            this.Button3ds.TabIndex = 20;
            this.Button3ds.Text = "Nintendo 3DS";
            this.Button3ds.UseVisualStyleBackColor = true;
            this.Button3ds.CheckedChanged += new System.EventHandler(this.Button3ds_CheckedChanged);
            // 
            // ButtonSwitch
            // 
            this.ButtonSwitch.AutoSize = true;
            this.ButtonSwitch.Location = new System.Drawing.Point(336, 31);
            this.ButtonSwitch.Name = "ButtonSwitch";
            this.ButtonSwitch.Size = new System.Drawing.Size(113, 19);
            this.ButtonSwitch.TabIndex = 21;
            this.ButtonSwitch.Text = "Nintendo Switch";
            this.ButtonSwitch.UseVisualStyleBackColor = true;
            this.ButtonSwitch.CheckedChanged += new System.EventHandler(this.ButtonSwitch_CheckedChanged);
            // 
            // ButtonPsvita
            // 
            this.ButtonPsvita.AutoSize = true;
            this.ButtonPsvita.Location = new System.Drawing.Point(502, 31);
            this.ButtonPsvita.Name = "ButtonPsvita";
            this.ButtonPsvita.Size = new System.Drawing.Size(107, 19);
            this.ButtonPsvita.TabIndex = 22;
            this.ButtonPsvita.Text = "PlayStation Vita";
            this.ButtonPsvita.UseVisualStyleBackColor = true;
            this.ButtonPsvita.CheckedChanged += new System.EventHandler(this.ButtonPsvita_CheckedChanged);
            // 
            // ButtonPs4
            // 
            this.ButtonPs4.AutoSize = true;
            this.ButtonPs4.Location = new System.Drawing.Point(662, 31);
            this.ButtonPs4.Name = "ButtonPs4";
            this.ButtonPs4.Size = new System.Drawing.Size(93, 19);
            this.ButtonPs4.TabIndex = 23;
            this.ButtonPs4.Text = "PlayStation 4";
            this.ButtonPs4.UseVisualStyleBackColor = true;
            this.ButtonPs4.CheckedChanged += new System.EventHandler(this.ButtonPs4_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ButtonPs4);
            this.groupBox1.Controls.Add(this.ButtonPsvita);
            this.groupBox1.Controls.Add(this.ButtonSwitch);
            this.groupBox1.Controls.Add(this.Button3ds);
            this.groupBox1.Controls.Add(this.buttonGeneric);
            this.groupBox1.Location = new System.Drawing.Point(12, 89);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(798, 67);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Platform";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.button1.Location = new System.Drawing.Point(13, 9);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(222, 40);
            this.button1.TabIndex = 25;
            this.button1.Text = "LOAD PREVIOUS INFO";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // FilesButton
            // 
            this.FilesButton.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.FilesButton.Location = new System.Drawing.Point(214, 286);
            this.FilesButton.Name = "FilesButton";
            this.FilesButton.Size = new System.Drawing.Size(394, 48);
            this.FilesButton.TabIndex = 26;
            this.FilesButton.Text = "FILES WINDOW";
            this.FilesButton.UseVisualStyleBackColor = true;
            this.FilesButton.Click += new System.EventHandler(this.FilesButton_Click);
            // 
            // informationButton
            // 
            this.informationButton.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.informationButton.Location = new System.Drawing.Point(214, 197);
            this.informationButton.Name = "informationButton";
            this.informationButton.Size = new System.Drawing.Size(394, 48);
            this.informationButton.TabIndex = 27;
            this.informationButton.Text = "INFORMATION WINDOW";
            this.informationButton.UseVisualStyleBackColor = true;
            this.informationButton.Click += new System.EventHandler(this.informationButton_Click);
            // 
            // patcherButton
            // 
            this.patcherButton.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.patcherButton.Location = new System.Drawing.Point(214, 381);
            this.patcherButton.Name = "patcherButton";
            this.patcherButton.Size = new System.Drawing.Size(394, 48);
            this.patcherButton.TabIndex = 28;
            this.patcherButton.Text = "PATCHER WINDOW";
            this.patcherButton.UseVisualStyleBackColor = true;
            this.patcherButton.Click += new System.EventHandler(this.patcherButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(144, 173);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(535, 21);
            this.label2.TabIndex = 29;
            this.label2.Text = "2º Open the information window for fill the neccesary info about your patch";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(175, 262);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(472, 21);
            this.label3.TabIndex = 30;
            this.label3.Text = "3º Open the files window and select your original and patched files";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(164, 357);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(495, 21);
            this.label4.TabIndex = 31;
            this.label4.Text = "4º Open the patcher window and fill the neccesary info for the patcher";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(303, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(216, 21);
            this.label5.TabIndex = 32;
            this.label5.Text = "1º Select your platform target";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label6.Location = new System.Drawing.Point(285, 441);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(253, 21);
            this.label6.TabIndex = 33;
            this.label6.Text = "5º Press this for generate the patch";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(766, 528);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(44, 23);
            this.button2.TabIndex = 34;
            this.button2.Text = "Clear";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(822, 668);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.patcherButton);
            this.Controls.Add(this.informationButton);
            this.Controls.Add(this.FilesButton);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.logTextBox);
            this.Controls.Add(this.GenerateButton);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Mara Generator";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button GenerateButton;
        private System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RadioButton buttonGeneric;
        private System.Windows.Forms.RadioButton Button3ds;
        private System.Windows.Forms.RadioButton ButtonSwitch;
        private System.Windows.Forms.RadioButton ButtonPsvita;
        private System.Windows.Forms.RadioButton ButtonPs4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button FilesButton;
        private System.Windows.Forms.Button informationButton;
        private System.Windows.Forms.Button patcherButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button2;
    }
}

