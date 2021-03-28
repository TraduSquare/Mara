
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.originalTextBox = new System.Windows.Forms.TextBox();
            this.modifiedTextBox = new System.Windows.Forms.TextBox();
            this.oriButton = new System.Windows.Forms.Button();
            this.modifiedButton = new System.Windows.Forms.Button();
            this.outButton = new System.Windows.Forms.Button();
            this.outTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // originalTextBox
            // 
            this.originalTextBox.Location = new System.Drawing.Point(18, 143);
            this.originalTextBox.Name = "originalTextBox";
            this.originalTextBox.Size = new System.Drawing.Size(169, 23);
            this.originalTextBox.TabIndex = 0;
            // 
            // modifiedTextBox
            // 
            this.modifiedTextBox.Location = new System.Drawing.Point(244, 142);
            this.modifiedTextBox.Name = "modifiedTextBox";
            this.modifiedTextBox.Size = new System.Drawing.Size(169, 23);
            this.modifiedTextBox.TabIndex = 1;
            // 
            // oriButton
            // 
            this.oriButton.Location = new System.Drawing.Point(193, 143);
            this.oriButton.Name = "oriButton";
            this.oriButton.Size = new System.Drawing.Size(27, 23);
            this.oriButton.TabIndex = 3;
            this.oriButton.Text = "...";
            this.oriButton.UseVisualStyleBackColor = true;
            this.oriButton.Click += new System.EventHandler(this.oriButton_Click);
            // 
            // modifiedButton
            // 
            this.modifiedButton.Location = new System.Drawing.Point(419, 142);
            this.modifiedButton.Name = "modifiedButton";
            this.modifiedButton.Size = new System.Drawing.Size(27, 23);
            this.modifiedButton.TabIndex = 4;
            this.modifiedButton.Text = "...";
            this.modifiedButton.UseVisualStyleBackColor = true;
            this.modifiedButton.Click += new System.EventHandler(this.modifiedButton_Click);
            // 
            // outButton
            // 
            this.outButton.Location = new System.Drawing.Point(314, 200);
            this.outButton.Name = "outButton";
            this.outButton.Size = new System.Drawing.Size(27, 23);
            this.outButton.TabIndex = 7;
            this.outButton.Text = "...";
            this.outButton.UseVisualStyleBackColor = true;
            this.outButton.Click += new System.EventHandler(this.outButton_Click);
            // 
            // outTextBox
            // 
            this.outTextBox.Location = new System.Drawing.Point(139, 200);
            this.outTextBox.Name = "outTextBox";
            this.outTextBox.Size = new System.Drawing.Size(169, 23);
            this.outTextBox.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(284, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(262, 37);
            this.label1.TabIndex = 8;
            this.label1.Text = "MARA GENERATOR";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(61, 119);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 21);
            this.label2.TabIndex = 9;
            this.label2.Text = "Original files";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(284, 118);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 21);
            this.label3.TabIndex = 10;
            this.label3.Text = "Modified files";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(192, 176);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 21);
            this.label4.TabIndex = 11;
            this.label4.Text = "Out folder";
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.button3.Location = new System.Drawing.Point(12, 465);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(798, 48);
            this.button3.TabIndex = 12;
            this.button3.Text = "GENERATE PATCH";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(19, 282);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(421, 150);
            this.label5.TabIndex = 13;
            this.label5.Text = resources.GetString("label5.Text");
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label6.Location = new System.Drawing.Point(187, 246);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 30);
            this.label6.TabIndex = 15;
            this.label6.Text = "GUIDE";
            // 
            // logTextBox
            // 
            this.logTextBox.Location = new System.Drawing.Point(13, 541);
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.Size = new System.Drawing.Size(797, 115);
            this.logTextBox.TabIndex = 16;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 523);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(27, 15);
            this.label7.TabIndex = 17;
            this.label7.Text = "Log";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label13.Location = new System.Drawing.Point(164, 59);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(113, 30);
            this.label13.TabIndex = 28;
            this.label13.Text = "Patch files";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(822, 668);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.logTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.outButton);
            this.Controls.Add(this.outTextBox);
            this.Controls.Add(this.modifiedButton);
            this.Controls.Add(this.oriButton);
            this.Controls.Add(this.modifiedTextBox);
            this.Controls.Add(this.originalTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Quick Xdelta Generator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox originalTextBox;
        private System.Windows.Forms.TextBox modifiedTextBox;
        private System.Windows.Forms.Button oriButton;
        private System.Windows.Forms.Button modifiedButton;
        private System.Windows.Forms.Button outButton;
        private System.Windows.Forms.TextBox outTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label13;
    }
}

