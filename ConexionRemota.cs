using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public partial class ConexionRemota : Form
{
    public ConexionRemota()
    {
        InitializeComponent();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        using (var brush = new SolidBrush(Color.FromArgb(240, ColorTranslator.FromHtml("#1A24B1"))))
        {
            e.Graphics.FillRectangle(brush, this.ClientRectangle);
        }

        using (var font = new Font("Segoe UI", 14, FontStyle.Bold))
        using (var brush = new SolidBrush(Color.White))
        {
            string closeText = "X";
            SizeF size = e.Graphics.MeasureString(closeText, font);
            float x = this.ClientSize.Width - size.Width - 10;
            float y = 10;
            e.Graphics.DrawString(closeText, font, brush, x, y);
        }
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);

        using (var font = new Font("Segoe UI", 14, FontStyle.Bold))
        {
            string closeText = "X";
            SizeF size = this.CreateGraphics().MeasureString(closeText, font);
            float x = this.ClientSize.Width - size.Width - 10;
            float y = 10;

            if (e.X >= x && e.X <= x + size.Width && e.Y >= y && e.Y <= y + size.Height)
            {
                this.Close();
            }
        }
    }

    private void SetRoundedCorners(int radius)
    {
        System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
        path.StartFigure();

        path.AddArc(new Rectangle(0, 0, radius, radius), 180, 90);
        path.AddArc(new Rectangle(this.Width - radius, 0, radius, radius), 270, 90);
        path.AddArc(new Rectangle(this.Width - radius, this.Height - radius, radius, radius), 0, 90);
        path.AddArc(new Rectangle(0, this.Height - radius, radius, radius), 90, 90);

        path.CloseFigure();
        this.Region = new Region(path);
    }

    private void ConexionRemota_Load(object sender, EventArgs e)
    {
        SetRoundedCorners(20);

        Label lblTitulo = new Label()
        {
            Text = "Escoge una de las opciones",
            Width = this.ClientSize.Width,
            Height = 40,
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.Transparent,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
        };

        lblTitulo.Top = (this.ClientSize.Height - (40 + 3 * 50 + 2 * 20)) / 2;
        lblTitulo.Left = 0;

        int buttonWidth = 200;
        int buttonHeight = 50;
        int spacing = 20;
        int startX = (this.ClientSize.Width - buttonWidth) / 2;
        int startY = lblTitulo.Bottom + spacing;

        // Botón TeamViewer
        Button btnTeamViewer = new Button()
        {
            Text = "TEAMVIEWER",
            Width = buttonWidth,
            Height = buttonHeight,
            Left = startX,
            Top = startY,
            BackColor = ColorTranslator.FromHtml("#0078FF"), // Azul TeamViewer
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
        };

        // Botón AnyDesk
        Button btnAnyDesk = new Button()
        {
            Text = "ANYDESK",
            Width = buttonWidth,
            Height = buttonHeight,
            Left = startX,
            Top = startY + buttonHeight + spacing,
            BackColor = ColorTranslator.FromHtml("#D02828"), // Rojo AnyDesk
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
        };

        // Eventos de clic para abrir los programas
        btnTeamViewer.Click += (s, args) => { AbrirAplicacionTeamViewer(); };
        btnAnyDesk.Click += (s, args) => { AbrirAplicacionAnyDesk(); };

        this.Controls.Add(lblTitulo);
        this.Controls.Add(btnTeamViewer);
        this.Controls.Add(btnAnyDesk);
    }

    private void ConexionRemota_Deactivate(object sender, EventArgs e)
    {
        this.Close();
    }

    // Método para abrir TeamViewer
    private void AbrirAplicacionTeamViewer()
    {
        string[] rutas = {
            @"C:\Program Files\TeamViewer\TeamViewer.exe",
            @"C:\Program Files (x86)\TeamViewer\TeamViewer.exe"
        };

        foreach (var ruta in rutas)
        {
            if (File.Exists(ruta))
            {
                Process.Start(ruta);
                return;
            }
        }

        MessageBox.Show("No se encontró TeamViewer en las rutas especificadas.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    // Método para abrir AnyDesk
    private void AbrirAplicacionAnyDesk()
    {
        string[] rutas = {
            @"C:\Program Files\AnyDesk\AnyDesk.exe",
            @"C:\Program Files (x86)\AnyDesk\AnyDesk.exe"
        };

        foreach (var ruta in rutas)
        {
            if (File.Exists(ruta))
            {
                Process.Start(ruta);
                return;
            }
        }

        MessageBox.Show("No se encontró AnyDesk en las rutas especificadas.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
