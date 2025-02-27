using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

public partial class TiposApuestas : Form
{
    public TiposApuestas()
    {
        InitializeComponent();
    }

    private async void TiposApuestas_Load(object sender, EventArgs e)
    {
        try
        {
            string userDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "aciertalaapp");
            var envOptions = new CoreWebView2EnvironmentOptions();
            var environment = await CoreWebView2Environment.CreateAsync(null, userDataFolder, envOptions);

            await browser.EnsureCoreWebView2Async(environment);

            string url = "https://peru.aciertala.com/definiciones-de-apuestas";
            if (!string.IsNullOrEmpty(url) && Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                browser.CoreWebView2.Navigate(url);

                // Suscribir al evento NavigationCompleted para ejecutar el script
                browser.CoreWebView2.NavigationCompleted += Browser_NavigationCompleted;
            }
            else
            {
                MessageBox.Show("URL no válida o vacía.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al inicializar WebView2: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void Browser_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        if (e.IsSuccess)
        {
            try
            {
                // Script para eliminar el banner de cookies con la clase "cc_banner cc_container cc_container--open"
                // y otros elementos que quieras ocultar/eliminar.
                string script = @"
                (function() {
                    var result = '';

                    // Ejemplo para eliminar por clase:
                    var cookieBanner = document.querySelector('.cc_banner.cc_container.cc_container--open');
                    if (cookieBanner) {
                        cookieBanner.parentNode.removeChild(cookieBanner);
                        result += 'Cookie banner eliminado. ';
                    } else {
                        result += 'Cookie banner no encontrado. ';
                    }

                    // (Opcional) Ocultar/Eliminar otros elementos que ya tenías:
                    var row5 = document.getElementById('u_row_5');
                    if (row5) {
                        row5.style.display = 'none';
                        result += 'Oculto u_row_5. ';
                    }

                    var row11 = document.getElementById('u_row_11');
                    if (row11) {
                        row11.style.display = 'none';
                        result += 'Oculto u_row_11. ';
                    }

                    var column4 = document.getElementById('u_column_4');
                    if (column4) {
                        column4.style.display = 'none';
                        result += 'Oculto u_column_4. ';
                    }

                    var st2 = document.getElementById('st-2');
                    if (st2) {
                        st2.style.display = 'none';
                        result += 'Oculto st-2. ';
                    }

                    return result;
                })();
            ";

                // Ejecutar el script en la página
                string result = await browser.CoreWebView2.ExecuteScriptAsync(script);
                // Opcional: si quieres ver en un MessageBox lo que devolvió
                // MessageBox.Show(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al ejecutar el script: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        else
        {
            MessageBox.Show("La navegación no se completó correctamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
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

    //private void Tiposapuesta_Deactivate(object sender, EventArgs e)
    //{
    //    this.Close(); 
    //}


}
