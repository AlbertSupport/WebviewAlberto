using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

public partial class Registro : Form
{
    public Registro()
    {
        InitializeComponent();
    }

    private async void Registro_Load(object sender, EventArgs e)
    {
        try
        {
            // Definir la ruta del archivo JSON
            string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "aciertalaapp", "Config.json");

            // Leer la URL desde el archivo JSON
            string url = LeerUrlDesdeJson(configPath);

            if (string.IsNullOrEmpty(url) || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                MessageBox.Show("URL no válida o no encontrada en el archivo de configuración.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string userDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "aciertalaapp");
            var envOptions = new CoreWebView2EnvironmentOptions();
            var environment = await CoreWebView2Environment.CreateAsync(null, userDataFolder, envOptions);

            await browser.EnsureCoreWebView2Async(environment);

            // Navegar a la URL obtenida
            browser.CoreWebView2.Navigate(url);
            browser.ZoomFactor = 1.0; // Ajustar el zoom al 100%
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al inicializar WebView2: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private string LeerUrlDesdeJson(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                string jsonContent = File.ReadAllText(filePath);
                using (JsonDocument doc = JsonDocument.Parse(jsonContent))
                {
                    JsonElement root = doc.RootElement;
                    if (root.TryGetProperty("UrlRegistro", out JsonElement urlElement))
                    {
                        return urlElement.GetString();
                    }
                }
            }
            else
            {
                MessageBox.Show("El archivo Config.json no existe.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al leer Config.json: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        return null;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        var currentScreen = Screen.FromPoint(Cursor.Position);
        int screenWidth = currentScreen.Bounds.Width;
        int fixedHeight = 1000;
        this.ClientSize = new Size(screenWidth, fixedHeight);
        this.Location = new Point(currentScreen.Bounds.X, currentScreen.Bounds.Y + 80);
    }

    private void Registro_Deactivate(object sender, EventArgs e)
    {
        this.Close();
    }
}
