using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

public partial class Estadistica : Form
{
    public Estadistica()
    {
        InitializeComponent();
        this.Deactivate += Estadistica_Deactivate; // Para detectar cuando el formulario pierde el foco
    }

    private async void Estadistica_Load(object sender, EventArgs e)
    {
        try
        {
            string userDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "aciertalaapp");
            var envOptions = new CoreWebView2EnvironmentOptions();
            var environment = await CoreWebView2Environment.CreateAsync(null, userDataFolder, envOptions);

            await browser.EnsureCoreWebView2Async(environment);

            // Suscribir al evento NewWindowRequested para manejar ventanas emergentes
            browser.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;

            string url = "https://statsinfo.co/stats/1/c/26/";
            if (!string.IsNullOrEmpty(url) && Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                browser.CoreWebView2.Navigate(url);

                // Ajustar el factor de zoom en el control WebView2
                browser.ZoomFactor = 1.0; // Escala al 100% (puedes cambiar este valor para ajustar el zoom)
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

    // Evento que maneja las ventanas emergentes
    private void CoreWebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
    {
        if (browser.CoreWebView2 != null) // Verifica si CoreWebView2 está disponible
        {
            // Detenemos la acción predeterminada de abrir la nueva ventana
            e.Handled = true;

            // Navegamos a la URL solicitada en el mismo WebView2
            browser.CoreWebView2.Navigate(e.Uri);  // Cargar la URL de la ventana emergente en el mismo WebView2
        }
        else
        {
            MessageBox.Show("No se pudo inicializar el WebView2", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Método para cerrar el formulario cuando se desenfoca
    private void Estadistica_Deactivate(object sender, EventArgs e)
    {
        this.Close(); // Cierra el formulario si se desenfoca (pierde el enfoque)
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Obtén la pantalla actual
        var currentScreen = Screen.FromPoint(Cursor.Position);
        int screenWidth = currentScreen.Bounds.Width;
        int fixedHeight = 1000;

        // Configura el tamaño y posición del formulario antes de que sea visible
        this.ClientSize = new Size(screenWidth, fixedHeight);
        this.Location = new Point(currentScreen.Bounds.X, currentScreen.Bounds.Y + 80); // Respeta el desplazamiento vertical de 80 píxeles
    }
}
