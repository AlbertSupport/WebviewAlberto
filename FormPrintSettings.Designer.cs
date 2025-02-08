namespace AciertalaV3
{
    partial class FormPrintSettings
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
            this.cmbPrinters = new System.Windows.Forms.ComboBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.rb58mm = new System.Windows.Forms.RadioButton();
            this.rb80mm = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.Button1 = new System.Windows.Forms.Button();
            this.lblTerminal = new System.Windows.Forms.Label();
            this.txtTerminal = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // cmbPrinters
            // 
            this.cmbPrinters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrinters.FormattingEnabled = true;
            this.cmbPrinters.Location = new System.Drawing.Point(88, 29);
            this.cmbPrinters.Margin = new System.Windows.Forms.Padding(2);
            this.cmbPrinters.Name = "cmbPrinters";
            this.cmbPrinters.Size = new System.Drawing.Size(114, 21);
            this.cmbPrinters.TabIndex = 0;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(22, 32);
            this.Label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(53, 13);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "Impresora";
            // 
            // rb58mm
            // 
            this.rb58mm.AutoSize = true;
            this.rb58mm.Location = new System.Drawing.Point(22, 66);
            this.rb58mm.Margin = new System.Windows.Forms.Padding(2);
            this.rb58mm.Name = "rb58mm";
            this.rb58mm.Size = new System.Drawing.Size(101, 17);
            this.rb58mm.TabIndex = 2;
            this.rb58mm.TabStop = true;
            this.rb58mm.Text = "Impresión 58mm";
            this.rb58mm.UseVisualStyleBackColor = true;
            // 
            // rb80mm
            // 
            this.rb80mm.AutoSize = true;
            this.rb80mm.Location = new System.Drawing.Point(22, 96);
            this.rb80mm.Margin = new System.Windows.Forms.Padding(2);
            this.rb80mm.Name = "rb80mm";
            this.rb80mm.Size = new System.Drawing.Size(101, 17);
            this.rb80mm.TabIndex = 3;
            this.rb80mm.TabStop = true;
            this.rb80mm.Text = "Impresión 80mm";
            this.rb80mm.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(22, 233);
            this.btnOK.Margin = new System.Windows.Forms.Padding(2);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(166, 28);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "Guardar";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // Button1
            // 
            this.Button1.Location = new System.Drawing.Point(442, 233);
            this.Button1.Margin = new System.Windows.Forms.Padding(2);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(166, 28);
            this.Button1.TabIndex = 9;
            this.Button1.Text = "Restablecer Configuración";
            this.Button1.UseVisualStyleBackColor = true;
            this.Button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // lblTerminal
            // 
            this.lblTerminal.AutoSize = true;
            this.lblTerminal.Location = new System.Drawing.Point(403, 37);
            this.lblTerminal.Name = "lblTerminal";
            this.lblTerminal.Size = new System.Drawing.Size(47, 13);
            this.lblTerminal.TabIndex = 10;
            this.lblTerminal.Text = "Terminal";
            // 
            // txtTerminal
            // 
            this.txtTerminal.Location = new System.Drawing.Point(456, 32);
            this.txtTerminal.Name = "txtTerminal";
            this.txtTerminal.Size = new System.Drawing.Size(100, 20);
            this.txtTerminal.TabIndex = 11;
            // 
            // FormPrintSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 305);
            this.Controls.Add(this.txtTerminal);
            this.Controls.Add(this.lblTerminal);
            this.Controls.Add(this.Button1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.rb80mm);
            this.Controls.Add(this.rb58mm);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.cmbPrinters);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormPrintSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormPrintSettings";
            this.Load += new System.EventHandler(this.FormPrintSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbPrinters;
        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.RadioButton rb58mm;
        private System.Windows.Forms.RadioButton rb80mm;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button Button1;
        private System.Windows.Forms.Label lblTerminal;
        private System.Windows.Forms.TextBox txtTerminal;
    }


}