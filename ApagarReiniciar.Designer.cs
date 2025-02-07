using System;
using System.Windows.Forms;

partial class ApagarReiniciar : Form
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
        this.Name = "ApagarReiniciar";
        this.Text = "Apagar Reiniciar";
        this.Load += new System.EventHandler(this.ApagarReiniciar_Load);
        //this.Deactivate += new System.EventHandler(this.ApagarReiniciar_Deactivate);
        this.ResumeLayout(false);

        // Manejo de cierre con Escape
        this.KeyDown += ApagarReiniciar_KeyDown;
    }

    private void ApagarReiniciar_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            this.Close(); // Cierra el popup al presionar Escape
        }
    }
}
