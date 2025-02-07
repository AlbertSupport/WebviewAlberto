using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public partial class ApagarReiniciar : Form
{
    public ApagarReiniciar()
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

    private void ApagarReiniciar_Load(object sender, EventArgs e)
    {
        SetRoundedCorners(20);

        Label lblTitulo = new Label()
        {
            Text = "Escoge una opción",
            Width = this.ClientSize.Width,
            Height = 40,
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.Transparent,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
        };

        lblTitulo.Top = (this.ClientSize.Height - (40 + 2 * 50 + 20)) / 2;
        lblTitulo.Left = 0;

        int buttonWidth = 200;
        int buttonHeight = 50;
        int spacing = 20;
        int startX = (this.ClientSize.Width - buttonWidth) / 2;
        int startY = lblTitulo.Bottom + spacing;

        // Botón Apagar
        Button btnApagar = new Button()
        {
            Text = "APAGAR",
            Width = buttonWidth,
            Height = buttonHeight,
            Left = startX,
            Top = startY,
            BackColor = ColorTranslator.FromHtml("#D02828"), // Rojo
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
        };

        // Botón Reiniciar
        Button btnReiniciar = new Button()
        {
            Text = "REINICIAR",
            Width = buttonWidth,
            Height = buttonHeight,
            Left = startX,
            Top = startY + buttonHeight + spacing,
            BackColor = ColorTranslator.FromHtml("#0078FF"), // Azul
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
        };

        // Eventos de clic para apagar y reiniciar
        btnApagar.Click += (s, args) => { ConfirmarApagar(); };
        btnReiniciar.Click += (s, args) => { ConfirmarReiniciar(); };

        this.Controls.Add(lblTitulo);
        this.Controls.Add(btnApagar);
        this.Controls.Add(btnReiniciar);
    }

    //private void ApagarReiniciar_Deactivate(object sender, EventArgs e)
    //{
    //    this.Close();
    //}

    // Método para confirmar y apagar la PC
    private void ConfirmarApagar()
    {
        DialogResult result = MessageBox.Show("¿Estás seguro de que deseas apagar la PC?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (result == DialogResult.Yes)
        {
            Process.Start("shutdown", "/s /t 0");
        }
    }

    // Método para confirmar y reiniciar la PC
    private void ConfirmarReiniciar()
    {
        DialogResult result = MessageBox.Show("¿Estás seguro de que deseas reiniciar la PC?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (result == DialogResult.Yes)
        {
            Process.Start("shutdown", "/r /t 0");
        }
    }
}
