using System;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Wpf;
using Newtonsoft.Json;

namespace AciertalaV3
{
    public partial class Juegosvirtuales : Form
    {
        private System.Windows.Forms.Timer _checkPrintTimer;

        public string TicketData { get; set; }

        public event EventHandler<string> TicketDataReceived;

        public Juegosvirtuales()
        {
            InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;

            try
            {
                Debug.WriteLine("🟡 Iniciando WebView2...");

                if (webView2 == null)
                {
                    Debug.WriteLine("❌ WebView2 no está inicializado.");
                    return;
                }

                await webView2.EnsureCoreWebView2Async();

                if (webView2.CoreWebView2 == null)
                {
                    Debug.WriteLine("❌ WebView2 no se inicializó correctamente.");
                    return;
                }

                Debug.WriteLine("✅ WebView2 inicializado correctamente.");

                webView2.CoreWebView2.NewWindowRequested += WebView2_NewWindowRequested;
                webView2.CoreWebView2.WebMessageReceived += WebView2_WebMessageReceived;

                string interceptScript = @"
     (function() {
    console.log('🚀 Interceptor de iframe activado.');

    function interceptIframe() {
        console.log('✅ Observador de iframe iniciado.');
        const observer = new MutationObserver((mutations) => {
            mutations.forEach((mutation) => {
                mutation.addedNodes.forEach((node) => {
                    if (node.tagName === 'IFRAME') {
                        console.log('📌 Iframe detectado:', node);
                        handleIframe(node);
                    }
                });
            });
        });
        observer.observe(document.body, { childList: true, subtree: true });
    }

    function handleIframe(iframe) {
        console.log('📄 Entraste papito:', iframe);
        
        // Observar cambios en el documento del iframe
        const iframeObserver = new MutationObserver(() => {
            try {
                const iframeDoc = iframe.contentDocument || iframe.contentWindow.document;
                if (iframeDoc && iframeDoc.body) {
                    console.log('🔍 Detectado cambio en iframe');
                    const script = iframeDoc.createElement('script');
                    script.innerHTML = `
                        window.parent.postMessage({
                            type: ""ticketData"",
                            data: document.documentElement.innerHTML
                        }, ""*"");
                    `;
                    iframeDoc.body.appendChild(script);
                }
            } catch (error) {
                console.error('Error:', error);
            }
        });

        // Iniciar observación cuando el iframe esté listo
        iframe.onload = () => {
            try {
                const iframeDoc = iframe.contentDocument || iframe.contentWindow.document;
                iframeObserver.observe(iframeDoc, {
                    childList: true,
                    subtree: true,
                    characterData: true
                });
            } catch (error) {
                console.error('Error al iniciar observador:', error);
            }
        };
    }

    window.addEventListener(""message"", (event) => {
        if (event.data?.type === ""ticketData"" && window.chrome?.webview) {
            window.chrome.webview.postMessage(JSON.stringify(event.data));
        }
    });

    if (document.readyState === 'complete') {
        interceptIframe();
    } else {
        window.addEventListener('DOMContentLoaded', interceptIframe);
    }
})();
        "
                ;

                await webView2.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(interceptScript);

                webView2.Source = new Uri("https://v2.betssons.net/");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al inicializar WebView2: {ex.Message}");
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


        private void WebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            string popupUrl = e.Uri;
            Debug.WriteLine($"🔹 Detectado intento de abrir un popup: {popupUrl}");
            e.Handled = true;
            if (!string.IsNullOrEmpty(popupUrl))
            {
                webView2.CoreWebView2.Navigate(popupUrl);
                Debug.WriteLine($"✅ Redirigiendo WebView2 a: {popupUrl}");
            }
        }


        private void WebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string message = e.TryGetWebMessageAsString();
                if (string.IsNullOrEmpty(message))
                {
                    Debug.WriteLine("⚠️ WebView2 recibió un mensaje vacío.");
                    return;
                }
                Debug.WriteLine("📩 Datos recibidos desde WebView2: " + message);
                dynamic json = JsonConvert.DeserializeObject(message);
                if (json.type == "ticketData")
                {
                    TicketData = json.data;
                    Debug.WriteLine("✅ TicketData guardado: " + TicketData);
                    TicketDataReceived?.Invoke(this, TicketData);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("❌ Error al procesar mensaje: " + ex.Message);
            }
        }


        private void StartAutoScraping()
        {
            _checkPrintTimer = new System.Windows.Forms.Timer();
            _checkPrintTimer.Interval = 2000;
            _checkPrintTimer.Tick += async (s, e) =>
            {
                // Antes de llamar, comprobamos que webView2.CoreWebView2 no sea null
                if (webView2 == null || webView2.CoreWebView2 == null)
                {
                    Debug.WriteLine("⚠️ webView2 o CoreWebView2 son null. No se realizará el scraping.");
                    return;
                }
                string capturedHtml = await GetCapturedPrintContentAsync();
                if (!string.IsNullOrEmpty(capturedHtml))
                {
                    Debug.WriteLine("📩 Contenido capturado automáticamente: " +
                        (capturedHtml.Length > 500 ? capturedHtml.Substring(0, 500) : capturedHtml) + " ... [truncado]");
                }
            };
            _checkPrintTimer.Start();
        }


        private async Task<string> GetCapturedPrintContentAsync()
        {
            // Comprobación adicional para asegurar que webView2 y su Core están inicializados.
            if (webView2 == null || webView2.CoreWebView2 == null)
            {
                Debug.WriteLine("⚠️ webView2 o CoreWebView2 son null en GetCapturedPrintContentAsync().");
                return string.Empty;
            }

            try
            {
                await Task.Delay(3000); // Espera para asegurarse de que los datos estén listos
                string result = await webView2.CoreWebView2.ExecuteScriptAsync("window._capturedPrintContent || ''");
                if (string.IsNullOrEmpty(result) || result == "\"\"")
                {
                    Debug.WriteLine("⚠️ No se encontró contenido capturado del iframe aún.");
                    return string.Empty;
                }
                result = Regex.Unescape(result.Trim('"'));
                Debug.WriteLine("📩 Contenido capturado correctamente: " + result);
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("❌ Error en GetCapturedPrintContentAsync(): " + ex.Message);
                return string.Empty;
            }
        }

        private void JuegosVirtuales_Deactivate(object sender, EventArgs e)
        {
            this.Close(); // Cierra el formulario al quedar en segundo plano
        }
    }
}
