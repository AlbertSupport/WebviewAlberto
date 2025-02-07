using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public partial class Utilitarios : Form
{
    public Utilitarios()
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

    private void Utilitarios_Load(object sender, EventArgs e)
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

        lblTitulo.Top = 20;
        lblTitulo.Left = 0;

        int groupWidth = 250;
        int groupHeight = 140;
        int buttonWidth = 200;
        int buttonHeight = 40;
        int spacing = 10;

        // Grupo: Herramientas Office
        GroupBox grpOffice = new GroupBox()
        {
            Text = "Herramientas Office",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Width = groupWidth,
            Height = groupHeight,
            Left = (this.ClientSize.Width - groupWidth) / 2,
            Top = lblTitulo.Bottom + 20,
            BackColor = Color.Transparent,
        };

        Button btnExcel = new Button()
        {
            Text = "Excel",
            Width = buttonWidth,
            Height = buttonHeight,
            Left = (grpOffice.Width - buttonWidth) / 2,
            Top = 30,
            BackColor = ColorTranslator.FromHtml("#217346"), // Verde Excel
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
        };
        btnExcel.Click += (s, args) => { AbrirExcel(); };

        Button btnWord = new Button()
        {
            Text = "Word",
            Width = buttonWidth,
            Height = buttonHeight,
            Left = (grpOffice.Width - buttonWidth) / 2,
            Top = btnExcel.Bottom + spacing,
            BackColor = ColorTranslator.FromHtml("#2B579A"), // Azul Word
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
        };
        btnWord.Click += (s, args) => { AbrirWord(); };

        grpOffice.Controls.Add(btnExcel);
        grpOffice.Controls.Add(btnWord);

        // Grupo: Herramientas
        GroupBox grpHerramientas = new GroupBox()
        {
            Text = "Herramientas",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Width = groupWidth,
            Height = groupHeight - 50,
            Left = (this.ClientSize.Width - groupWidth) / 2,
            Top = grpOffice.Bottom + 20,
            BackColor = Color.Transparent,
        };

        Button btnCalculadora = new Button()
        {
            Text = "Calculadora",
            Width = buttonWidth,
            Height = buttonHeight,
            Left = (grpHerramientas.Width - buttonWidth) / 2,
            Top = 30,
            BackColor = ColorTranslator.FromHtml("#4A90E2"), // Azul claro
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
        };
        btnCalculadora.Click += (s, args) => { AbrirCalculadora(); };

        grpHerramientas.Controls.Add(btnCalculadora);

        // Agregar controles al formulario
        this.Controls.Add(lblTitulo);
        this.Controls.Add(grpOffice);
        this.Controls.Add(grpHerramientas);
    }

    private void AbrirExcel()
    {
        try
        {
            Process.Start("excel");
        }
        catch
        {
            MessageBox.Show("No se pudo abrir Microsoft Excel.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void AbrirWord()
    {
        try
        {
            Process.Start("winword");
        }
        catch
        {
            MessageBox.Show("No se pudo abrir Microsoft Word.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void AbrirCalculadora()
    {
        try
        {
            Process.Start("calc");
        }
        catch
        {
            MessageBox.Show("No se pudo abrir la Calculadora.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void Utilitarios_Deactivate(object sender, EventArgs e)
    {
        this.Close(); // Cierra el formulario al quedar en segundo plano
    }
}
