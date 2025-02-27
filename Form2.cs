using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

namespace AciertalaV3
{
    public partial class Form2 : Form
    {
        private Panel panelLateral;
        private Panel panelBotonesContainer;
        private FlowLayoutPanel panelBotones;
        private Button btnHome;
        private Button btnCerrar;
        private bool botonesVisibles = false;
        private int alturaBotones = 0;
        private static ApagarReiniciar formApagarReiniciar = null;
        private static AciertalaWeb formaciertalaWeb = null;
        private bool posicionActualizada = false;

        public Form2()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Black; 
            this.Size = new Size(9, 60);
          
            this.DoubleBuffered = true;
            this.MouseDown += Form2_MouseDown;
            this.KeyPreview = true;

            var workingArea = Screen.PrimaryScreen.WorkingArea;


            int x = workingArea.X + 1000;
            int y = workingArea.Y + 5;
            this.Location = new Point(x, y);


            GenerarInterfaz();
            GenerarBotones();
            CalcularAlturaBotones();

            PosicionarArribaDerecha();


            SystemEvents.DisplaySettingsChanged += (s, e) => PosicionarArribaDerecha();
            SystemEvents.UserPreferenceChanged += (s, e) => PosicionarArribaDerecha();

        }


        private void PosicionarArribaDerecha()
        {
            if (posicionActualizada) return; // Evita recálculos innecesarios

            Rectangle wa = Screen.PrimaryScreen.WorkingArea;
            int screenWidth = wa.Width;

            int offsetDerecha = 480;
            int offsetArriba = 10;

            if (screenWidth <= 1280)
            {
                offsetDerecha = 300;
            }
            if (screenWidth <= 1440)
            {
                offsetDerecha = 405;
            }

            int x = wa.Right - this.Width - offsetDerecha;
            int y = wa.Top + offsetArriba;

            // Asegura que no se salga de los límites
            x = Math.Max(x, wa.Left);
            y = Math.Max(y, wa.Top);

            this.Location = new Point(x, y);
            posicionActualizada = true; // Evita actualizaciones futuras innecesarias
        }



        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Form2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 0xA1, 0x2, 0);
            }
        }

        private void GenerarInterfaz()
        {

            panelLateral = new Panel
            {
                Width = 175,
                BackColor = ColorTranslator.FromHtml("#313439"),
                Dock = DockStyle.Left,
                Padding = new Padding(0)
            };
            this.Controls.Add(panelLateral);


            panelBotonesContainer = new Panel
            {
                Width = 0,
                Height = 0,
                Dock = DockStyle.Top,
                Padding = new Padding(0),
                Margin = new Padding(0),
                BackColor = ColorTranslator.FromHtml("#313439"),
                Visible = botonesVisibles
            };
            panelLateral.Controls.Add(panelBotonesContainer);


            panelBotones = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = ColorTranslator.FromHtml("#313439"),
                AutoScroll = false,
                Padding = new Padding(0),
                Margin = new Padding(0),
                Dock = DockStyle.Fill
            };
            panelBotonesContainer.Controls.Add(panelBotones);


            btnHome = new Button
            {
                Text = "",
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = ColorTranslator.FromHtml("#1A24B1"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ImageAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 0, 0, 0),
                FlatAppearance = { BorderSize = 0 },
                Margin = new Padding(0, 0, 0, 0)
            };

            if (File.Exists("icons/home.png"))
            {
                btnHome.Image = new Bitmap("icons/home.png");
                btnHome.ImageAlign = ContentAlignment.MiddleLeft;
            }


            btnHome.Click += (s, e) =>
            {
                TogglePanelBotones();
            };

            panelLateral.Controls.Add(btnHome);
            btnHome.Paint += BtnHome_Paint;

        }

        private void BtnHome_Paint(object sender, PaintEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            string textMain = "HOME";
            string textVersion = " V3.0";

            Font fontMain = new Font("Segoe UI", 14, FontStyle.Bold);
            Font fontVersion = new Font("Segoe UI", 7, FontStyle.Regular);

            SizeF sizeMain = e.Graphics.MeasureString(textMain, fontMain);
            SizeF sizeVersion = e.Graphics.MeasureString(textVersion, fontVersion);

            float totalWidth = sizeMain.Width + sizeVersion.Width;
            float startX = (btn.Width - totalWidth) / 2; // Centrar el texto

            using (SolidBrush brush = new SolidBrush(btn.ForeColor))
            {
                // Dibujamos "HOME" con fuente grande
                e.Graphics.DrawString(textMain, fontMain, brush, startX, (btn.Height - sizeMain.Height) / 2);
                // Dibujamos "V1.0" con fuente más pequeña
                e.Graphics.DrawString(textVersion, fontVersion, brush, startX + sizeMain.Width, (btn.Height - sizeVersion.Height) / 2 + 4);
            }
        }

        private void GenerarBotones()
        {
            ButtonConfig[] buttonConfigs = new ButtonConfig[]
            {
                 new ButtonConfig("CABALLOS", "icons/Caballos.png", (s, e) => AbrirCaballos()),
                 new ButtonConfig("JUEGOS VIRTUALES", "icons/Caballos.png", (s, e) => AbrirJuegosVirtuales()),
                 new ButtonConfig("ADMIN GOLDEN", "icons/lives.png", (s, e) => AbrirAdminGolden()),
                 new ButtonConfig("SHEETS", "icons/scores.png", (s, e) => AbrirSheetsGoogle()),
                 new ButtonConfig("CASHIER ONLINE", "icons/register.png", (s, e) => AbrirAciertalaWeb()),
                 new ButtonConfig("CHROME", "icons/browser.png", (s, e) => AbrirChrome()),
                 new ButtonConfig("WHATSAPP", "icons/wsp.png", (s, e) => AbrirWhatsapp()),
                 new ButtonConfig("CHAT SOPORTE", "icons/chat.png", (s, e) => AbrirTawk()),
                 new ButtonConfig("TIPOS DE APUESTAS", "icons/bets.png", (s, e) => AbrirTiposApuestas()),
                 new ButtonConfig("UTILITARIOS", "icons/utilitarios.png", (s, e) => AbrirUtilitarios()),
                 new ButtonConfig("CONEXIÓN REMOTA", "icons/remote.png", (s, e) => AbrirConexionRemota()),
                 new ButtonConfig("ACTUALIZAR", "icons/update.png", (s, e) => RestartAciertala()),
                 new ButtonConfig("APAGAR / REINICIAR", "icons/power.png", (s, e) => AbrirApagarReiniciar())
            };

            foreach (var btnConfig in buttonConfigs)
            {
                // Verificar el valor de BotonesVirtuales en el archivo JSON
                string configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AciertalaApp", "config.json");
                string botonesVirtualesStatus = "Desactivar"; // Valor por defecto

                if (File.Exists(configFilePath))
                {
                    try
                    {
                        string json = File.ReadAllText(configFilePath);
                        var jsonObj = JObject.Parse(json);
                        botonesVirtualesStatus = jsonObj["BotonesVirtuales"]?.ToString() ?? "Desactivar";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al leer el archivo de configuración: {ex.Message}");
                    }
                }

                // Solo mostrar el botón "JUEGOS VIRTUALES" si "BotonesVirtuales" está en "Activar"
                if (btnConfig.Texto == "JUEGOS VIRTUALES" && botonesVirtualesStatus == "Activar")
                {
                    Button btn = new Button
                    {
                        Text = " " + btnConfig.Texto,
                        Width = panelLateral.Width - 40,
                        Height = 50,
                        BackColor = ColorTranslator.FromHtml("#313439"),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        TextAlign = ContentAlignment.MiddleLeft,
                        TextImageRelation = TextImageRelation.ImageBeforeText,
                        ImageAlign = ContentAlignment.MiddleLeft,
                        Padding = new Padding(5, 0, 0, 0),
                        Font = new Font("Segoe UI", 10, FontStyle.Bold),
                        Margin = new Padding(0, 0, 0, 0),
                        FlatAppearance = { BorderSize = 2, BorderColor = Color.Blue }
                    };

                    if (File.Exists(btnConfig.Icono))
                    {
                        btn.Image = new Bitmap(btnConfig.Icono);
                        btn.ImageAlign = ContentAlignment.MiddleLeft;
                    }

                    // Usar la función ConfigurarEventoClickBoton
                    ConfigurarEventoClickBoton(btn, btnConfig);
                    panelBotones.Controls.Add(btn);
                }
                else if (btnConfig.Texto != "JUEGOS VIRTUALES")
                {
                    // Agregar el resto de los botones (excepto "JUEGOS VIRTUALES")
                    Button btn = new Button
                    {
                        Text = " " + btnConfig.Texto,
                        Width = panelLateral.Width - 40,
                        Height = 50,
                        BackColor = ColorTranslator.FromHtml("#313439"),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        TextAlign = ContentAlignment.MiddleLeft,
                        TextImageRelation = TextImageRelation.ImageBeforeText,
                        ImageAlign = ContentAlignment.MiddleLeft,
                        Padding = new Padding(5, 0, 0, 0),
                        Font = new Font("Segoe UI", 10, FontStyle.Bold),
                        Margin = new Padding(0, 0, 0, 0),
                        FlatAppearance = { BorderSize = 2, BorderColor = Color.Blue }
                    };

                    if (File.Exists(btnConfig.Icono))
                    {
                        btn.Image = new Bitmap(btnConfig.Icono);
                        btn.ImageAlign = ContentAlignment.MiddleLeft;
                    }

                    // Usar la función ConfigurarEventoClickBoton
                    ConfigurarEventoClickBoton(btn, btnConfig);
                    panelBotones.Controls.Add(btn);
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.F11:
                    EjecutarPrograma(@"C:\BotonesAciertala\BotonF11.exe");
                    return true;

                case Keys.F6:
                    EjecutarPrograma(@"C:\BotonesAciertala\BotonF6.exe");
                    return true;

                case Keys.F5:
                    ReiniciarAciertala();
                    return true;

                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        private void EjecutarPrograma(string ruta)
        {
            if (File.Exists(ruta))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = ruta,
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show($"No se encontró el archivo:\n{ruta}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReiniciarAciertala()
        {
            string aciertalaPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Aciertala", "Aciertala.exe");

            if (File.Exists(aciertalaPath))
            {
                try
                {
             
                    foreach (var proceso in Process.GetProcessesByName("Aciertala"))
                    {
                        proceso.Kill();
                    }

                    System.Threading.Thread.Sleep(1000);

                    Process.Start(new ProcessStartInfo
                    {
                        FileName = aciertalaPath,
                        UseShellExecute = true
                    });


                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al reiniciar Aciértala:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No se encontró Aciértala.exe.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DibujarBotonHome(object sender, PaintEventArgs e)
        {
            Button btn = sender as Button;
            Graphics g = e.Graphics;
            Rectangle rect = new Rectangle(0, 0, btn.Width, btn.Height);
            LinearGradientBrush gradient = new LinearGradientBrush(rect, Color.Blue, Color.DarkBlue, LinearGradientMode.Vertical);

            g.FillRectangle(gradient, rect);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawRectangle(new Pen(ColorTranslator.FromHtml("#1A24B1"), 5), rect);

        }

        private void TogglePanelBotones()
        {
            botonesVisibles = !botonesVisibles;
            panelBotonesContainer.Visible = botonesVisibles;

            if (botonesVisibles)
            {
                panelBotonesContainer.Height = alturaBotones;
                this.Height += alturaBotones;
            }
            else
            {
                panelBotonesContainer.Height = 0;
                this.Height -= alturaBotones;
            }
        }

        private void ConfigurarEventoClickBoton(Button btn, ButtonConfig config)
        {
            btn.Click += (s, e) =>
            {
                config.EventoClick(s, e);

                TogglePanelBotones();
            };
        }

        private void AbrirCaballos()
        {
            Caballos form = new Caballos();
            form.Show();
        }

        private void AbrirJuegosVirtuales()
        {
            Juegosvirtuales form = new Juegosvirtuales();
            form.Show();
        }

        private void AbrirAdminGolden()
        {
            AdminGolden form = new AdminGolden();
            form.Show();
        }

        private void AbrirSheetsGoogle()
        {
            WebSheets form = new WebSheets();
            form.Show();
        }

        private void AbrirAciertalaWeb()
        {
            if (formaciertalaWeb == null || formaciertalaWeb.IsDisposed)
            {
                formaciertalaWeb = new AciertalaWeb();
                formaciertalaWeb.Show();
            }
            else
            {
                formaciertalaWeb.Focus();
            }
        }

        private void AbrirChrome()
        {
            WebChrome form = new WebChrome();
            form.Show();
        }

        private void AbrirWhatsapp()
        {
            WhatsappWeb form = new WhatsappWeb();
            form.Show();
        }

        private void AbrirTawk()
        {
            Tawk form = new Tawk();
            form.Show();
        }

        private void AbrirTiposApuestas()
        {
            TiposApuestas form = new TiposApuestas();
            form.Show();
        }

        private void AbrirUtilitarios()
        {
            Utilitarios form = new Utilitarios();
            form.Show();
        }

        private void AbrirConexionRemota()
        {
            ConexionRemota form = new ConexionRemota();
            form.Show();
        }

        private void RestartAciertala()
        {
            string processName = "Aciertala";

            try
            {
                // Buscar y cerrar todas las instancias del proceso
                Process[] procesos = Process.GetProcessesByName(processName);
                foreach (var proceso in procesos)
                {
                    try
                    {
                        proceso.Kill();
                        proceso.WaitForExit(6000);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error al cerrar {processName}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cerrar Aciertala: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AbrirApagarReiniciar()
        {
            if (formApagarReiniciar == null || formApagarReiniciar.IsDisposed)
            {
                formApagarReiniciar = new ApagarReiniciar();
                formApagarReiniciar.Show();
            }
            else
            {
                formApagarReiniciar.Focus(); // Lleva la ventana al frente si ya está abierta
            }
        }

        private void CalcularAlturaBotones()
        {
            alturaBotones = 0;
            foreach (Control control in panelBotones.Controls)
            {
                alturaBotones += control.Height + control.Margin.Bottom;
            }
        }
    }
}
