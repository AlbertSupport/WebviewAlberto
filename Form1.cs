using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AciertalaV3
{
    public partial class Form1 : Form
    {
        private Panel panelLateral;
        private Panel panelBotonesContainer;
        private FlowLayoutPanel panelBotones;
        private Button btnHome;
        private Button btnCerrar;
        private bool botonesVisibles = false;
        private int alturaBotones = 0;

        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.BackColor = Color.Black;
            this.Size = new Size(182, 60);
            this.DoubleBuffered = true;
            this.MouseDown += Form1_MouseDown;
            this.KeyPreview = true;

            var workingArea = Screen.PrimaryScreen.WorkingArea;


            GenerarInterfaz();
            GenerarBotones();
            CalcularAlturaBotones();
            PosicionarArribaDerecha();


            SystemEvents.DisplaySettingsChanged += (s, e) => PosicionarArribaDerecha();
            SystemEvents.UserPreferenceChanged += (s, e) => PosicionarArribaDerecha();
        }

        private void PosicionarArribaDerecha()
        {
            Rectangle wa = Screen.PrimaryScreen.WorkingArea;
            int screenWidth = wa.Width;


            int offsetDerecha = 360;
            int offsetArriba = 10;


            if (screenWidth <= 1280)
            {
                offsetDerecha = 300;
            }
            if (screenWidth <= 1440)
            {
                offsetDerecha = 280;
            }

            int x = wa.Right - this.Width - offsetDerecha;
            int y = wa.Top + offsetArriba;

            // Asegura que no se salga de los límites
            x = Math.Max(x, wa.Left);
            y = Math.Max(y, wa.Top);

            this.Location = new Point(x, y);
        }



        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Form1_MouseDown(object sender, MouseEventArgs e)
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
                Width = 220,
                BackColor = ColorTranslator.FromHtml("#FFC0CB"),
                Dock = DockStyle.Left,
                Padding = new Padding(0)
            };
            this.Controls.Add(panelLateral);

            panelBotonesContainer = new Panel
            {
                Width = 200,
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
                Padding = new Padding(10, 0, 0, 0),
                FlatAppearance = { BorderSize = 0 },
                Margin = new Padding(0, 0, 0, 5)
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
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            // Definir tamaños predeterminados
            int buttonWidth = panelLateral.Width - 40;
            int buttonHeight = 50;
            int fontSize = 10;
            int paddingLeft = 20;

            // Ajustar panel para resolución 800x600
            if (screenWidth <= 800 && screenHeight <= 600)
            {
                buttonWidth = 200; // Ancho fijo para pantallas pequeñas
                buttonHeight = 35; // Reducir altura
                fontSize = 8; // Reducir tamaño de fuente
                paddingLeft = 10; // Reducir padding

                // Ajustar el panel lateral para que se superponga a la barra de tareas
                panelLateral.Width = buttonWidth + 40; // Ajustar el panel lateral
                panelLateral.Location = new Point(screenWidth - panelLateral.Width, 0); // Mover el panel hacia la izquierda
            }
            // Ajustar para resolución 1280x600
            else if (screenWidth <= 1280 && screenHeight <= 600)
            {
                buttonWidth = 250; // Ancho fijo para esta resolución
                buttonHeight = 40; // Ajustar altura de los botones
                fontSize = 9; // Tamaño de fuente ligeramente mayor
                paddingLeft = 15; // Ajustar padding

                // Ajustar el panel lateral para que se superponga a la barra de tareas
                panelLateral.Width = buttonWidth + 40; // Ajustar el panel lateral
                panelLateral.Location = new Point(screenWidth - panelLateral.Width, 0); // Mover el panel hacia la izquierda
            }
            // Ajustar para resoluciones mayores
            else
            {
                panelLateral.Width = buttonWidth + 40; // Para resoluciones mayores
                panelLateral.Location = new Point(screenWidth - panelLateral.Width, 0); // Mover el panel hacia la izquierda
            }

            ButtonConfig[] buttonConfigs = new ButtonConfig[]
            {
        new ButtonConfig("TERMINAL LOGIN", "icons/WEB.png", (s, e) => AbrirTerminalLogin()),
        new ButtonConfig("CABALLOS", "icons/Caballos.png", (s, e) => AbrirCaballos()),
        new ButtonConfig("JUEGOS VIRTUALES", "icons/Caballos.png", (s, e) => AbrirJuegosVirtuales()),
        new ButtonConfig("RESULTADO EN VIVO", "icons/lives.png", (s, e) => AbrirResultadoEnVivo()),
        new ButtonConfig("MARCADORES EN VIVO", "icons/scores.png", (s, e) => AbrirMarcadoresEnVivo()),
        new ButtonConfig("ESTADISTICA", "icons/stats.png", (s, e) => AbrirEstadistica()),
        new ButtonConfig("TRANSMISIÓN", "icons/stream.png", (s, e) => AbrirPanelTransmisiones()),
        new ButtonConfig("CHROME", "icons/browser.png", (s, e) => AbrirChrome()),
        new ButtonConfig("REGISTRO", "icons/register.png", (s, e) => AbrirPanelRegistro()),
        new ButtonConfig("TIPOS DE APUESTAS", "icons/bets.png", (s, e) => AbrirTiposApuestas()),
        new ButtonConfig("ACTUALIZAR", "icons/update.png", (s, e) => RestartAciertala()),
        new ButtonConfig("CONEXIÓN REMOTA", "icons/remote.png", (s, e) => AbrirConexionRemota()),
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
                        Width = buttonWidth,
                        Height = buttonHeight,
                        BackColor = ColorTranslator.FromHtml("#313439"),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        TextAlign = ContentAlignment.MiddleLeft,
                        TextImageRelation = TextImageRelation.ImageBeforeText,
                        ImageAlign = ContentAlignment.MiddleLeft,
                        Padding = new Padding(paddingLeft, 0, 0, 0),
                        Font = new Font("Segoe UI", fontSize, FontStyle.Bold),
                        Margin = new Padding(0, 0, 0, 5),
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

        private void AbrirTerminalLogin()
        {
            TerminalLogin form = new TerminalLogin();
            form.Show();
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

        private void AbrirResultadoEnVivo()
        {
            ResultadoEnvivo form = new ResultadoEnvivo();
            form.Show();
        }

        private void AbrirMarcadoresEnVivo()
        {
            MarcadoresEnVivo form = new MarcadoresEnVivo();
            form.Show();
        }

        private void AbrirEstadistica()
        {
            Estadistica form = new Estadistica();
            form.Show();
        }

        private void AbrirPanelTransmisiones()
        {
            PanelTransmisiones form = new PanelTransmisiones();
            form.Show();
        }

        private void AbrirChrome()
        {
            WebChrome form = new WebChrome();
            form.Show();
        }

        private void AbrirPanelRegistro()
        {
            PanelRegistro form = new PanelRegistro();
            form.Show();
        }

        private void AbrirTiposApuestas()
        {
            TiposApuestas form = new TiposApuestas();
            form.Show();
        }

        private void RestartAciertala()
        {
            string processName = "Aciertala";
            string exePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Aciertala", "Aciertala.exe");

            try
            {
                // Buscar y cerrar todas las instancias del proceso
                Process[] procesos = Process.GetProcessesByName(processName);
                foreach (var proceso in procesos)
                {
                    try
                    {
                        proceso.Kill();
                        proceso.WaitForExit(5000);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error al cerrar {processName}: {ex.Message}");
                    }
                }

                // Verificar si el ejecutable existe antes de intentar abrirlo
                if (File.Exists(exePath))
                {
                    Thread.Sleep(2000);

                    // Iniciar nuevamente el programa
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = exePath,
                        UseShellExecute = true,
                        WorkingDirectory = Path.GetDirectoryName(exePath)
                    });
                }
                else
                {
                    MessageBox.Show("No se encontró 'Aciertala.exe' en AppData\\Local.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al reiniciar Aciertala: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AbrirConexionRemota()
        {
            ConexionRemota form = new ConexionRemota();
            form.Show();
        }

        private void AbrirApagarReiniciar()
        {
            ApagarReiniciar form = new ApagarReiniciar();
            form.Show();
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

