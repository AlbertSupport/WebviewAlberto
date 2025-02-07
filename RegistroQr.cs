using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

public partial class RegistroQr : Form
{
    public RegistroQr()
    {
        InitializeComponent();
    }

    private async void RegistroQr_Load(object sender, EventArgs e)
    {
        try
        {
            // Ruta al archivo config.json
            string storageFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "aciertalaapp", "config.json");

            // Leer la URL desde el archivo JSON
            string LinkQR = ReadQrUrlFromStorage(storageFilePath);

            // Validar la URL leída
            if (!string.IsNullOrEmpty(LinkQR) && Uri.IsWellFormedUriString(LinkQR, UriKind.Absolute))
            {
                string userDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "aciertalaapp");
                var envOptions = new CoreWebView2EnvironmentOptions();
                var environment = await CoreWebView2Environment.CreateAsync(null, userDataFolder, envOptions);

                await browser.EnsureCoreWebView2Async(environment);

                // Navegar a la URL leída
                browser.CoreWebView2.Navigate(LinkQR);
            }
            else
            {
                MessageBox.Show("URL no válida o vacía en el archivo de configuración.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al inicializar WebView2: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Lee la URL asociada a "LinkQR" desde el archivo config.json.
    /// </summary>
    /// <param name="filePath">Ruta completa al archivo config.json.</param>
    /// <returns>El valor de "LinkQR" si existe; de lo contrario, cadena vacía.</returns>
    private string ReadQrUrlFromStorage(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return string.Empty;
        }

        try
        {
            // Leer todo el contenido del archivo JSON
            string jsonContent = File.ReadAllText(filePath);

            // Deserializar el JSON a un diccionario
            var configData = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);

            // Verificar si existe la clave "LinkQR"
            if (configData != null && configData.ContainsKey("LinkQR"))
            {
                return configData["LinkQR"];
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al leer el archivo de configuración: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return string.Empty;
    }

    private void RegistroQr_Deactivate(object sender, EventArgs e)
    {
        this.Close(); // Cierra el formulario al quedar en segundo plano
    }
}
