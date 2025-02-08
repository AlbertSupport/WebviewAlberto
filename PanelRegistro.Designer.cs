using System;
using System.Windows.Forms;

partial class PanelRegistro : Form
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();
        // 
        // PanelTransmisiones
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(500, 500);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None; // Sin bordes
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen; // Centrado
        this.Opacity = 0.95; // Opcional: efecto translúcido
        this.Name = "PanelRegistro";
        this.Text = "Panel Registro";
        this.Load += new System.EventHandler(this.PanelRegistro_Load);
        this.Deactivate += new System.EventHandler(this.PanelRegistro_Deactivate);
        this.ResumeLayout(false);

        // Manejo de cierre con Escape
        this.KeyDown += PanelRegistro_KeyDown;
    }

    private void PanelRegistro_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            this.Close(); // Cierra el popup al presionar Escape
        }
    }
}
