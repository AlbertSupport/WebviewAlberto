using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using WebviewAlberto;

public partial class PanelTransmisiones : Form
{
    public PanelTransmisiones()
    {
        InitializeComponent();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        // Dibujar un fondo sólido con color personalizado y la misma transparencia
        using (var brush = new SolidBrush(Color.FromArgb(240, ColorTranslator.FromHtml("#1A24B1")))) // 150 es la transparencia
        {
            e.Graphics.FillRectangle(brush, this.ClientRectangle);
        }

        // Dibujar la "X" de cierre en la parte superior derecha
        using (var font = new Font("Segoe UI", 14, FontStyle.Bold))
        using (var brush = new SolidBrush(Color.White))  // Color blanco para la X
        {
            string closeText = "X";
            SizeF size = e.Graphics.MeasureString(closeText, font);
            float x = this.ClientSize.Width - size.Width - 10; // Posición X en la parte derecha
            float y = 10; // Posición Y en la parte superior
            e.Graphics.DrawString(closeText, font, brush, x, y);
        }
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);

        // Comprobar si el clic está dentro del área de la "X"
        using (var font = new Font("Segoe UI", 14, FontStyle.Bold))
        {
            string closeText = "X";
            SizeF size = this.CreateGraphics().MeasureString(closeText, font);
            float x = this.ClientSize.Width - size.Width - 10;
            float y = 10;

            // Si el clic está dentro de la zona de la "X"
            if (e.X >= x && e.X <= x + size.Width && e.Y >= y && e.Y <= y + size.Height)
            {
                this.Close(); // Cerrar el formulario
            }
        }
    }

    private void SetRoundedCorners(int radius)
    {
        System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
        path.StartFigure();

        // Esquinas superiores
        path.AddArc(new Rectangle(0, 0, radius, radius), 180, 90); // Esquina superior izquierda
        path.AddArc(new Rectangle(this.Width - radius, 0, radius, radius), 270, 90); // Esquina superior derecha

        // Lados y esquinas inferiores
        path.AddArc(new Rectangle(this.Width - radius, this.Height - radius, radius, radius), 0, 90); // Esquina inferior derecha
        path.AddArc(new Rectangle(0, this.Height - radius, radius, radius), 90, 90); // Esquina inferior izquierda

        path.CloseFigure();

        // Asignar la región al formulario
        this.Region = new Region(path);
    }

    private void PanelTransmisiones_Load(object sender, EventArgs e)
    {
        SetRoundedCorners(20);

        // Crear y configurar el título
        Label lblTitulo = new Label()
        {
            Text = "Escoge una de las opciones",  // Texto del título
            Width = this.ClientSize.Width,       // Ancho igual al tamaño del formulario
            Height = 40,                         // Altura del título
            TextAlign = ContentAlignment.MiddleCenter,  // Centrar texto horizontal y vertical
            BackColor = Color.Transparent,       // Fondo transparente
            ForeColor = Color.White,             // Color blanco para el texto
            Font = new Font("Segoe UI", 16, FontStyle.Bold),  // Fuente más grande para el título
        };

        // Centrar el título en la parte superior del formulario
        lblTitulo.Top = (this.ClientSize.Height - (40 + 3 * 50 + 2 * 20)) / 2; // Ajustar al centro vertical
        lblTitulo.Left = 0;

        // Crear y configurar los botones
        int buttonWidth = 200;  // Ancho de los botones
        int buttonHeight = 50;  // Altura de los botones
        int spacing = 20;       // Espacio entre botones
        int startX = (this.ClientSize.Width - buttonWidth) / 2;  // Centrado horizontalmente
        int startY = lblTitulo.Bottom + spacing;  // Los botones inician justo debajo del título

        // Botón Transmisión 1
        Button btnTransmision1 = new Button()
        {
            Text = "TRANSMISION 1",
            Width = buttonWidth,
            Height = buttonHeight,
            Left = startX,
            Top = startY,
            BackColor = Color.White,
            ForeColor = ColorTranslator.FromHtml("#1A24B1"),
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
        };

        // Botón Transmisión 3
        //Button btnTransmision3 = new Button()
        //{
        //    Text = "TRANSMISION 3",
        //    Width = buttonWidth,
        //    Height = buttonHeight,
        //    Left = startX,
        //    Top = startY + buttonHeight + spacing,  // Espaciado entre botones
        //    BackColor = Color.White,
        //    ForeColor = ColorTranslator.FromHtml("#1A24B1"),
        //    Font = new Font("Segoe UI", 12, FontStyle.Bold),
        //};

        // Ruta de la imagen en el sistema local
        string iconPath = "icons/tv.png";

        // Intentamos cargar el icono desde la ruta local
        try
        {
            if (File.Exists(iconPath))
            {
                Image transmisionIcon = Image.FromFile(iconPath);

                // Configurar el botón Transmisión 1
                btnTransmision1.Image = transmisionIcon;
                btnTransmision1.ImageAlign = ContentAlignment.MiddleLeft; // Alineación de la imagen en el botón
                btnTransmision1.TextAlign = ContentAlignment.MiddleRight; // Alineación del texto a la derecha del icono
                btnTransmision1.Padding = new Padding(15, 0, 15, 0); // Ajusta el padding (espaciado) entre el icono y el texto

                //// Configurar el botón Transmisión 3
                //btnTransmision3.Image = transmisionIcon;
                //btnTransmision3.ImageAlign = ContentAlignment.MiddleLeft; // Alineación de la imagen en el botón
                //btnTransmision3.TextAlign = ContentAlignment.MiddleRight; // Alineación del texto a la derecha del icono
                //btnTransmision3.Padding = new Padding(15, 0, 15, 0); // Ajusta el padding (espaciado) entre el icono y el texto
            }
            else
            {
                MessageBox.Show("No se encontró el archivo de imagen en la ruta especificada: " + iconPath);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al cargar el icono: " + ex.Message);
        }


        // Manejo de clic para el botón Transmisión 1
        btnTransmision1.Click += (s, args) =>
        {
            MessageBox.Show("Botón Transmisión 1 presionado");
            Transmision1 transmision1Form = new Transmision1();
            transmision1Form.Show();
        };

        // Manejo de clic para el botón Transmisión 3
        //btnTransmision3.Click += (s, args) =>
        //{
        //    MessageBox.Show("Botón Transmisión 3 presionado");
        //    Transmision3 transmision3Form = new Transmision3();
        //    transmision3Form.Show();
        //};

        //// Agregar el título y los botones al formulario
        this.Controls.Add(lblTitulo);  // Agregar el título
        this.Controls.Add(btnTransmision1);  // Agregar Botón Transmisión 1
        //this.Controls.Add(btnTransmision3);  // Agregar Botón Transmisión 3
    }

    private void PanelTransmisiones_Deactivate(object sender, EventArgs e)
    {
        this.Close(); // Cierra el formulario al quedar en segundo plano
    }
}
