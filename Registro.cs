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

            // Asignar el manejador de eventos para cuando la navegación haya terminado
            browser.CoreWebView2.NavigationCompleted += InjectScriptWithInterval;

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

    private void InjectScriptWithInterval(object sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        var scriptToModifyElements = @"(function() {
    const hideElements = () => {
        const selectors = [
            '.sc-bwsPYA.hCUxjX', 
            'a.header__logo', 
            '.sc-hLirLb.sc-hbqYmb.jGPdMe.fpmbEs',
            '.sc-hLirLb.sc-hbqYmb.jGPdMe.cafran',
            '.sc-hLirLb.sc-hbqYmb.jGPdMe.cafran img[alt=""Subscription""]',
            '.sc-hLirLb.sc-hbqYmb.jGPdMe.kszbty',
            'div.search.false', 
            'a.button.tg.undefined',
            'button.sc-jOiSOi.iFwTcU.sc-nTrUm.eDSTMV',
            'section.sc-enHPVx.icbqGE', 
            'a.j6075e732.__web-inspector-hide-shortcut__',
            'a[href*=""ads.adfox.ru/699683/clickURL""]',
            'jdiv.button__bEFyn',
            'jdiv.wrap__mwjDj._orientationRight__FZyz2._show__HwnHL._hoverMenu__NHxTH.__jivoDesktopButton.__web-inspector-hide-shortcut__',
            'div.share.__web-inspector-hide-shortcut__',
            'div.ya-share2.ya-share2_inited',
            'jdiv.iconWrap__SceC7',
            'jdiv.button__bEFyn[style*=""background: linear-gradient(95deg, rgb(211, 55, 55)""]',
            'div.share__text',
            '.sc-GKYbw.lkPYer', // Highlights elemento
            '.sc-itMJkM.jkTSKs', 

            
            // Elementos que deseas ocultar por su id
            '#Promociones',
            '#Escríbenos',
            '#Recarga\\ al\\ toque', // Escapamos el espacio con doble barra invertida
            '#Habilidad',
            
            // Nuevos elementos a ocultar
            '#Casino', 
            '#Live\\ Casino', // Escapamos el espacio con doble barra invertida
            
            // Nuevo elemento a ocultar: nvscore-carousel
            'nvscore-carousel' 
            


        ];

        // Seleccionamos los elementos completos que contienen los enlaces y ocultamos todo su contenedor <li>
        selectors.forEach(selector => {
            document.querySelectorAll(selector).forEach(el => {
                let li = el.closest('li'); // Buscar el <li> más cercano al <a> con el id
                if (li) li.style.display = 'none'; // Ocultar el <li>
                else el.style.display = 'none'; // Si no es <li>, ocultar directamente el elemento
            });
        });
        
        // Intentamos acceder al contenedor con la clase ""tawk-min-container"" cada 500ms, hasta un máximo de 5 segundos
        let attempts = 0;
        const maxAttempts = 10; // Intentos de 500ms (500ms * 10 = 5 segundos)

        const intervalId = setInterval(() => {
            const tawkContainer = document.querySelector('.tawk-min-container');
            if (tawkContainer) {
                tawkContainer.style.display = 'none'; // Ocultar el elemento estableciendo display: none
                clearInterval(intervalId); // Detener el intervalo una vez que encontramos el elemento
            }
            attempts++;
            if (attempts >= maxAttempts) {
                clearInterval(intervalId); // Detener el intervalo si se alcanzan los intentos máximos
            }
        }, 500); // Cada 500ms
    };

    document.addEventListener('DOMContentLoaded', hideElements);

    const observer = new MutationObserver(hideElements);
    observer.observe(document.body, { childList: true, subtree: true });
})();";

        browser.CoreWebView2.ExecuteScriptAsync(scriptToModifyElements);
    }

    private void Registro_Deactivate(object sender, EventArgs e)
    {
        this.Close();
    }
}
