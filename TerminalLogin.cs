using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json.Linq;
using QRCoder;
using ZXing;
using ZXing.Common;

namespace AciertalaV3
{
    public partial class TerminalLogin : Form
    {
        private string ticketContent;
        private Timer keyListenerTimer = new Timer();

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
        private const int VK_F2 = 0x71;

        public TerminalLogin()
        {
            InitializeComponent();
        }

        private async void TerminalLogin_Load(object sender, EventArgs e)
        {
            try
            {
                string userDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "aciertalaapp");
                var envOptions = new CoreWebView2EnvironmentOptions();
                var environment = await CoreWebView2Environment.CreateAsync(null, userDataFolder, envOptions);

                await browser.EnsureCoreWebView2Async(environment);

                browser.ZoomFactor = 1.25;
                browser.CoreWebView2.Settings.AreDevToolsEnabled = true;

                browser.CoreWebView2.NavigationCompleted += OnNavigationCompleted;
                browser.CoreWebView2.WebMessageReceived += OnWebMessage;
                browser.CoreWebView2.WebMessageReceived += OnWebMessageSave2;
                browser.CoreWebView2.NewWindowRequested += HandleNewWindowRequested;

                await InjectInterceptionScriptAsync();
                await InjectInterceptionScriptForSave2Async();

                string url = "https://www.pe.aciertala.com/sport";
                if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    browser.CoreWebView2.Navigate(url);
                }
                else
                {
                    MessageBox.Show("URL no válida o vacía.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                keyListenerTimer.Interval = 100;
                keyListenerTimer.Tick += KeyListenerTimer_Tick;
                keyListenerTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar WebView2: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task InjectInterceptionScriptAsync()
        {
            string interceptionScript = @"
            (function() {
                const originalFetch = window.fetch;
                const originalXHR = window.XMLHttpRequest;

                window.fetch = async function(...args) {
                    const response = await originalFetch.apply(this, args);
                    const clonedResponse = response.clone();
                    const requestUrl = args[0];
                    const requestInit = args[1] || {};
                    const requestBody = requestInit.body || null;

                    if (requestUrl.includes('/set-print') && requestInit.method === 'POST') {
                        clonedResponse.text().then(body => {
                            window.chrome.webview.postMessage({
                                type: 'fetch-intercept',
                                url: requestUrl,
                                method: requestInit.method,
                                requestBody: requestBody,
                                responseBody: body
                            });
                        });
                    }
                    return response;
                };

                const xhrOpen = originalXHR.prototype.open;
                const xhrSend = originalXHR.prototype.send;

                originalXHR.prototype.open = function(method, url, ...rest) {
                    this._url = url;
                    this._method = method;
                    return xhrOpen.apply(this, [method, url, ...rest]);
                };

                originalXHR.prototype.send = function(body) {
                    this.addEventListener('load', function() {
                        if (this._url.includes('/set-print') && this._method === 'POST') {
                            window.chrome.webview.postMessage({
                                type: 'xhr-intercept',
                                url: this._url,
                                method: this._method,
                                requestBody: body,
                                responseBody: this.responseText
                            });
                        }
                    });
                    return xhrSend.apply(this, [body]);
                };

                console.log('Interceptores de fetch y XHR inyectados');
            })();";
            await browser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(interceptionScript);
            Debug.WriteLine("[InjectInterceptionScriptAsync] Script inyectado correctamente.");
        }

        private async Task InjectInterceptionScriptForSave2Async()
        {
            string interceptionScript = @"
            (function() {
                const originalXHR = window.XMLHttpRequest;
                const xhrOpen = originalXHR.prototype.open;
                const xhrSend = originalXHR.prototype.send;

                originalXHR.prototype.open = function(method, url) {
                    this._url = url;
                    this._method = method;
                    return xhrOpen.apply(this, arguments);
                };
                originalXHR.prototype.send = function(body) {
                    this.addEventListener('load', function() {
                        if (this._url.includes('/ticket/save2') && this._method.toUpperCase() === 'POST') {
                            window.chrome.webview.postMessage({
                                type: 'xhr-save2',
                                url: this._url,
                                method: this._method,
                                requestBody: body,
                                responseBody: this.responseText
                            });
                        }
                    });
                    return xhrSend.apply(this, arguments);
                };
            })();";
            await browser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(interceptionScript);
            Debug.WriteLine("[InjectInterceptionScriptForSave2Async] Script inyectado para interceptar /ticket/save2.");
        }

        private void HandleNewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.Handled = true;
            Debug.WriteLine($"Ventana emergente bloqueada: {e.Uri}");
        }

        private async void OnNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {

                string removeScript = @"
            (function() {
                function removeElements() {
                    const selectorsToRemove = [
                        'nvscore-dynamic-element.ng-tns-c151-4.ng-tns-c435-2.visibility-true.nvs-menu-item.ng-star-inserted',
                        'nvscore-carousel.ng-star-inserted',
                        'div.nvscore-carousel.multiple',
                        'div.nvscore-carousel.full-width',
                        'div#footer',
                        'div.tawk-min-container',
                        'iframe[src=""about:blank""][style*=""position:fixed""]',
                        'div.login-img',
                        'bs-modal-backdrop',
                        'nvscore-editable-popup'
                    ];
                    // Eliminar solo los elementos innecesarios
                    selectorsToRemove.forEach(selector => {
                        document.querySelectorAll(selector).forEach(el => el.remove());
                    });
                    // Si existe `nvscore-editable-popup`, quitar solo la clase 'fade'
                    document.querySelectorAll('nvscore-editable-popup').forEach(popup => {
                        if (popup.classList.contains('fade')) {
                            popup.classList.remove('fade');
                            console.log('Se quitó la clase fade del popup.');
                        }
                    });
                }
                removeElements();
                const observer = new MutationObserver(() => { removeElements(); });
                observer.observe(document.body, { childList: true, subtree: true });
                console.log('Observer activo: Eliminación de elementos y ajuste de clase fade.');
            })();
        ";
                await browser.CoreWebView2.ExecuteScriptAsync(removeScript);
                Debug.WriteLine("[OnNavigationCompleted] removeScript ejecutado.");


                string mutationObserverScript = @"
            (function() {
                function addHideListener() {
                    const btn = document.querySelector('.btn.btn-primary.ticket-action-btn.save.ng-star-inserted');
                    if (btn) {
                        if (!btn.dataset.hideListenerAdded) {
                            btn.dataset.hideListenerAdded = 'true';

                            btn.addEventListener('click', function() {
                                btn.style.display = 'none';
                                console.log('Botón ocultado al primer clic');
                            }, { once: true }); 

                            console.log('Listener de clic para ocultar el botón añadido.');
                        }
                    }
                }

                addHideListener();

 
                const observer = new MutationObserver((mutations, obs) => {
        
                    addHideListener();
                });

                observer.observe(document.body, { childList: true, subtree: true });
                console.log('MutationObserver inyectado. El botón se ocultará al primer clic cada vez que reaparezca.');
            })();
            ";
                await browser.CoreWebView2.ExecuteScriptAsync(mutationObserverScript);
                Debug.WriteLine("[OnNavigationCompleted] MutationObserver inyectado para ocultar el botón al primer clic.");
            }
            else
            {
                Debug.WriteLine("La navegación falló o fue cancelada.");
            }
        }

        private void KeyListenerTimer_Tick(object sender, EventArgs e)
        {
            if (GetAsyncKeyState(VK_F2) != 0)
            {
                OpenSettingsForm();
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

        private void OpenSettingsForm()
        {
            var settingsForm = Application.OpenForms["FormPrintSettings"];
            if (settingsForm == null)
            {
                settingsForm = new FormPrintSettings();
                settingsForm.Show();
            }
            else
            {
                settingsForm.BringToFront();
                settingsForm.Activate();
            }
        }

        private void OnWebMessage(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var rawJson = e.WebMessageAsJson;
                var msgObj = JObject.Parse(rawJson);
                var msgType = msgObj?["type"]?.ToString();

                switch (msgType)
                {
                    case "fetch-intercept":
                        ProcessTicketResponse(msgObj?["responseBody"]?.ToString());
                        break;
                    case "xhr-intercept":
                        ProcessTicketResponse(msgObj?["responseBody"]?.ToString());
                        break;
                    default:
                        Debug.WriteLine($"[JS MSG] Tipo desconocido: {rawJson}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[OnWebMessage ERROR] {ex.Message}");
            }
        }

        private async void OnWebMessageSave2(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var rawJson = e.WebMessageAsJson;
                Debug.WriteLine($"[OnWebMessageSave2] Received message: {rawJson}");
                var msgObj = JObject.Parse(rawJson);
                var msgType = msgObj?["type"]?.ToString();

                if (msgType == "xhr-save2")
                {
                    var responseBody = msgObj?["responseBody"]?.ToString();

                    try
                    {

                        ProcessTicketSave2WithDesign(responseBody);


                    }
                    catch (Exception exPrint)
                    {
                        Debug.WriteLine($"[OnWebMessageSave2] Error al imprimir: {exPrint.Message}");


                    }
                    finally
                    {

                        string showButtonScript = @"
                            (function() {
                                const btn = document.querySelector('.btn.btn-primary.ticket-action-btn.save.ng-star-inserted');
                                if (btn) {
                                    btn.style.display = 'block';
                                    console.log('Botón de impresión vuelto a mostrar tras la impresión.');
                                }
                            })();";
                        await browser.CoreWebView2.ExecuteScriptAsync(showButtonScript);
                        Debug.WriteLine("[OnWebMessageSave2] Botón vuelto a mostrar.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[OnWebMessageSave2 ERROR] {ex.Message}");
                string exceptionMessageScript = @"
                    (function() {
                        alert('Ocurrió un error al procesar la solicitud de impresión.');
                    })();";
                await browser.CoreWebView2.ExecuteScriptAsync(exceptionMessageScript);
            }
        }

        public void ProcessTicketSave2WithDesign(string responseBody)
        {
            Task.Run(() =>
            {
                try
                {
                    JObject parsed = JObject.Parse(responseBody);

                    string voucherCode = parsed["data"]?["code"]?.ToString();
                    string fechaStr = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                    if (string.IsNullOrEmpty(voucherCode))
                    {
                        Debug.WriteLine("[ProcessTicketSave2WithDesign] El código del voucher no está presente en la respuesta.");
                        return;
                    }

                    PrintDocument printDoc = new PrintDocument();
                    printDoc.PrinterSettings.PrinterName = Properties.Settings.Default.PrinterName;
                    printDoc.PrintController = new StandardPrintController();

                    if (!printDoc.PrinterSettings.IsValid)
                    {
                        MessageBox.Show($"La impresora '{Properties.Settings.Default.PrinterName}' no es válida o no está instalada.",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    PaperSize customPaperSize = new PaperSize("Custom", 300, 1000);
                    printDoc.DefaultPageSettings.PaperSize = customPaperSize;

                    printDoc.PrintPage += (senderObj, eArgs) =>
                    {
                        Graphics g = eArgs.Graphics;
                        g.Clear(Color.White);

                        int marginLeft = 10;
                        int marginTop = 10;
                        int yPos = marginTop;

                        int ticketWidth = 280;

                        Font fontDate = new Font("Arial", 8, FontStyle.Regular);
                        Font fontSubtitle = new Font("Arial", 10, FontStyle.Bold);
                        Font fontVoucher = new Font("Arial", 14, FontStyle.Bold);
                        Font fontFooter = new Font("Arial", 7, FontStyle.Regular);


                        if (!string.IsNullOrEmpty(Properties.Settings.Default.LogoPath) && System.IO.File.Exists(Properties.Settings.Default.LogoPath))
                        {
                            using (Image logoImg = Image.FromFile(Properties.Settings.Default.LogoPath))
                            {
                                int logoWidth = 200;
                                int logoHeight = 100;
                                int logoX = marginLeft + (ticketWidth - logoWidth) / 2;
                                g.DrawImage(logoImg, new Rectangle(logoX, yPos, logoWidth, logoHeight));
                                yPos += logoHeight + 10;
                            }
                        }
                        else
                        {
                            Font fontTitle = new Font("Arial", 16, FontStyle.Bold);
                            string textLogo = "LOGO";
                            SizeF sizeLogo = g.MeasureString(textLogo, fontTitle);
                            float logoX = marginLeft + (ticketWidth - sizeLogo.Width) / 2;
                            g.DrawString(textLogo, fontTitle, Brushes.Black, logoX, yPos);
                            yPos += (int)sizeLogo.Height + 10;
                        }

                        string TerminalTexto = $"{Properties.Settings.Default.Terminal}";
                        SizeF sizeVoucher2 = g.MeasureString(voucherCode, fontVoucher);
                        float fechaT = marginLeft + (ticketWidth - sizeVoucher2.Width) / 2;
                        g.DrawString(TerminalTexto, fontVoucher, Brushes.Black, fechaT - 50, yPos);
                        yPos += (int)sizeVoucher2.Height + 10;

                        string fechaTexto = $"Fecha: {fechaStr}";
                        SizeF sizeDate = g.MeasureString(fechaTexto, fontDate);
                        float fechaX = marginLeft + (ticketWidth - sizeDate.Width) / 2;
                        g.DrawString(fechaTexto, fontDate, Brushes.Black, fechaX, yPos);
                        yPos += (int)sizeDate.Height + 10;


                        string tituloVoucher = "CÓDIGO DE VOUCHER DE RESERVA";
                        SizeF sizeSub = g.MeasureString(tituloVoucher, fontSubtitle);
                        float subX = marginLeft + (ticketWidth - sizeSub.Width) / 2;
                        g.DrawString(tituloVoucher, fontSubtitle, Brushes.Black, subX, yPos);
                        yPos += (int)sizeSub.Height + 10;


                        SizeF sizeVoucher = g.MeasureString(voucherCode, fontVoucher);
                        float voucherX = marginLeft + (ticketWidth - sizeVoucher.Width) / 2;
                        g.DrawString(voucherCode, fontVoucher, Brushes.Black, voucherX, yPos);
                        yPos += (int)sizeVoucher.Height + 10;


                        ZXing.OneD.Code128Writer writer = new ZXing.OneD.Code128Writer();
                        BitMatrix matrix = writer.encode(voucherCode, BarcodeFormat.CODE_128, 200, 50);
                        Bitmap barcodeBitmap = new Bitmap(matrix.Width, matrix.Height);
                        for (int yy = 0; yy < matrix.Height; yy++)
                        {
                            for (int xx = 0; xx < matrix.Width; xx++)
                            {
                                barcodeBitmap.SetPixel(xx, yy, matrix[xx, yy] ? Color.Black : Color.White);
                            }
                        }

                        int barcodeX = marginLeft + (ticketWidth - matrix.Width) / 2;
                        g.DrawImage(barcodeBitmap, new Point(barcodeX, yPos));
                        yPos += barcodeBitmap.Height + 10;


                        string footerText = "La apuesta será válida únicamente al ser autorizada en caja.\n" +
                            "Si las probabilidades seleccionadas cambian, este Voucher perderá su validez automáticamente.";

                        RectangleF footerRect = new RectangleF(marginLeft, yPos, ticketWidth, 200);
                        StringFormat sf = new StringFormat(StringFormatFlags.LineLimit)
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Near
                        };

                        g.DrawString(footerText, fontFooter, Brushes.Black, footerRect, sf);

                        SizeF measuredFooterSize = g.MeasureString(footerText, fontFooter, ticketWidth, sf);
                        yPos += (int)measuredFooterSize.Height + 10;


                        int finalContentHeight = yPos - marginTop;
                        Rectangle ticketRect = new Rectangle(marginLeft - 7, marginTop, ticketWidth, finalContentHeight);
                        g.DrawRectangle(Pens.Black, ticketRect);
                    };

                    printDoc.Print();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("[ProcessTicketSave2WithDesign ERROR] " + ex.Message);
                }
            });
        }

        public void ProcessTicketResponse(string responseBody)
        {
            Task.Run(() =>
            {
                try
                {
                    Debug.WriteLine("Response Body (set-print):");
                    Debug.WriteLine(responseBody);

                    JObject parsedResponse = JObject.Parse(responseBody);
                    JObject ticketData = parsedResponse["data"]?["ticket"] as JObject;

                    PrintDocument printDoc = new PrintDocument();
                    printDoc.PrinterSettings.PrinterName = Properties.Settings.Default.PrinterName;

                    printDoc.PrintController = new StandardPrintController();

                    if (!printDoc.PrinterSettings.IsValid)
                    {
                        MessageBox.Show($"La impresora '{Properties.Settings.Default.PrinterName}' no es válida o no está instalada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    Debug.WriteLine($"Impresora en Settings: {Properties.Settings.Default.PrinterName}");

                    // Configuración del tamaño del papel
                    int paperWidth = Properties.Settings.Default.PaperSize == "58 mm" ? 200 : 300;
                    // Altura del papel establecida en 3000 para permitir suficiente espacio
                    PaperSize customPaperSize = new PaperSize("Custom", paperWidth, 3000);
                    printDoc.DefaultPageSettings.PaperSize = customPaperSize;

                    printDoc.PrintPage += (sender, e) =>
                    {
                        Graphics g = e.Graphics;
                        g.Clear(Color.White);

                        int y = 5;
                        int marginLeft = Properties.Settings.Default.PaperSize == "58 mm" ? 5 : 10;
                        int pageWidthCalc = e.PageBounds.Width - marginLeft * 2;

                        // Definir fuentes
                        float fontNormalSize = Properties.Settings.Default.PaperSize == "58 mm" ? 7.0f : 10.0f;
                        Font fontNormal = new Font("Arial", fontNormalSize, FontStyle.Regular);
                        Font fontBold = new Font("Arial", fontNormalSize, FontStyle.Bold);
                        Font fontBigBold = new Font(fontBold.FontFamily, fontBold.Size + 3, fontBold.Style);

                        // Dibujar logo si existe
                        int logoWidth = Properties.Settings.Default.PaperSize == "58 mm" ? 100 : 160;
                        int logoHeight = Properties.Settings.Default.PaperSize == "58 mm" ? 40 : 80;
                        Rectangle logoRect = new Rectangle(marginLeft, y, logoWidth, logoHeight);

                        // Dibujar QR Code
                        int qrSize = Properties.Settings.Default.PaperSize == "58 mm" ? 80 : 120;
                        int spaceBetween = 10;
                        Rectangle qrRect = new Rectangle(marginLeft + logoWidth + spaceBetween, y, qrSize, qrSize);

                        int rowHeight = 0;

                        if (!string.IsNullOrEmpty(Properties.Settings.Default.LogoPath) && System.IO.File.Exists(Properties.Settings.Default.LogoPath))
                        {
                            using (Image logoImg = Image.FromFile(Properties.Settings.Default.LogoPath))
                            {
                                g.DrawImage(logoImg, logoRect);
                            }
                            rowHeight = Math.Max(rowHeight, logoRect.Height);
                        }

                        JToken qrToken = ticketData["qr_code_url"];
                        if (qrToken != null)
                        {
                            string qrLink = qrToken.ToString();
                            if (!string.IsNullOrEmpty(qrLink))
                            {
                                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                                {
                                    QRCodeData qrData = qrGenerator.CreateQrCode(qrLink, QRCodeGenerator.ECCLevel.Q);
                                    using (QRCode qrCode = new QRCode(qrData))
                                    {
                                        int scale = Properties.Settings.Default.PaperSize == "58 mm" ? 4 : 6;
                                        Bitmap qrBmp = qrCode.GetGraphic(scale);

                                        g.DrawImage(qrBmp, qrRect);
                                        rowHeight = Math.Max(rowHeight, qrRect.Height);
                                    }
                                }
                            }
                        }

                        int usedHeight = rowHeight > 0 ? rowHeight : 0;
                        usedHeight += 10;
                        y += usedHeight;

                        // Configuración del pen para líneas divisorias
                        Pen dashPen = new Pen(Color.Black, Properties.Settings.Default.PaperSize == "58 mm" ? 1 : 3);
                        dashPen.DashPattern = new float[] { 1, Properties.Settings.Default.PaperSize == "58 mm" ? 1 : 2 };

                        // Definir el pen para las líneas horizontales como sólido
                        Pen solidPen = new Pen(Color.Black, Properties.Settings.Default.PaperSize == "58 mm" ? 1 : 3);

                        List<string> sections = new List<string>();

                        JObject operatorObj = ticketData["operator"] as JObject;
                        if (operatorObj != null)
                        {
                            string operatorId = operatorObj["id"]?.ToString();
                            string operatorUser = operatorObj["username"]?.ToString();
                            string operatorText = $"Operador: {operatorUser} - ID: {operatorId}";
                            sections.Add(operatorText);
                        }

                        JObject userObj = ticketData["user"] as JObject;

                        if (userObj != null)
                        {
                            string userId = userObj["id"]?.ToString();
                            string userUsername = userObj["username"]?.ToString();
                            string userText = $"Usuario: {userUsername} - ID: {userId}";
                            sections.Add(userText);
                        }



                        string cuponText = $"Cupón: {ticketData["code"]?.ToString()}";
                        sections.Add(cuponText);

                        // Definir fuentes para las secciones
                        Font fontBox = new Font(fontBold.FontFamily, fontBold.Size + 4, fontBold.Style);

                        // Configuración del cuadro grande
                        int boxMargin = 10;
                        int boxPadding = 7;  // Espacio interno para evitar que el texto toque los bordes
                        int boxWidth = pageWidthCalc - (2 * boxMargin);
                        int textAreaWidth = boxWidth - (2 * boxPadding);

                        // Envolver el texto de las secciones
                        List<string> wrappedSections = new List<string>();
                        foreach (string section in sections)
                        {
                            wrappedSections.AddRange(WrapText(section, textAreaWidth, fontBox, g));
                        }

                        // Calcular alturas
                        int lineHeightBox = (int)g.MeasureString("A", fontBox).Height;
                        int lineSpacingBox = 3; // Espaciado entre líneas dentro de las secciones
                        int sectionSpacing = 5;  // Espaciado entre secciones

                        // Calcular la altura de las secciones
                        int sectionsHeightBox = wrappedSections.Count * (lineHeightBox + lineSpacingBox) + (sections.Count - 1) * sectionSpacing;

                        // Calcular la altura total del cuadro grande
                        int totalBoxHeight = sectionsHeightBox + (2 * boxPadding);

                        Rectangle infoBoxRect = new Rectangle(marginLeft, y, boxWidth, totalBoxHeight);
                        Pen penBox = new Pen(Color.Black, 1);
                        g.DrawRectangle(penBox, infoBoxRect);

                        float sectionY = infoBoxRect.Y + boxPadding;

                        for (int i = 0; i < sections.Count; i++)
                        {
                            string section = sections[i];

                            List<string> wrappedLines = WrapText(section, textAreaWidth, fontBox, g);

                            foreach (string line in wrappedLines)
                            {
                                g.DrawString(line, fontBox, Brushes.Black,
                                    new RectangleF(infoBoxRect.X + boxPadding, sectionY, textAreaWidth, lineHeightBox),
                                    new StringFormat { Alignment = StringAlignment.Near }
                                );
                                sectionY += lineHeightBox + lineSpacingBox;
                            }

                            if (i < sections.Count - 1)
                            {
                                g.DrawLine(solidPen, infoBoxRect.X + boxPadding, sectionY, infoBoxRect.Right - boxPadding, sectionY);
                                sectionY += sectionSpacing;
                            }
                        }

                        y += totalBoxHeight + 10;



                        // Fecha de emisión
                        Font fontApuestas = new Font(fontNormal.FontFamily, fontNormal.Size - 1, fontNormal.Style);
                        string checkoutTime = ticketData["checkout_time"]?.ToString();
                        DateTime parsedDate = DateTime.Parse(checkoutTime);
                        g.DrawString($"Fecha emisión: {parsedDate:dd/MM/yyyy HH:mm:ss}", fontApuestas, Brushes.Black, marginLeft, y);
                        y += (int)(fontNormal.Size * 2.5f);

                        // Línea divisoria
                        g.DrawLine(dashPen, marginLeft, y, pageWidthCalc - marginLeft, y);
                        y += (int)(fontNormal.Size * 1.5f);

                        // Tipo de apuesta
                        string betType = ticketData["bets_type"]?.ToString();
                        string Type = ticketData["type"]?.ToString();
                        g.DrawString($"{Type}  {betType}", fontBold, Brushes.Black, marginLeft, y);
                        y += (int)(fontBold.Size * 2.5f);

                        // Otra línea divisoria
                        g.DrawLine(dashPen, marginLeft, y, pageWidthCalc - marginLeft, y);
                        y += (int)(fontNormal.Size * 1.5f);

                        // Lista de apuestas
                        JToken items = ticketData["items"];
                        if (items != null)
                        {
                            foreach (JObject bet in items)
                            {
                                string gameStartDate = bet["event_date"]?.ToString();
                                string league = bet["tournament"]?["name"]?.ToString();

                                List<string> lines1 = WrapText($"{gameStartDate} - {league}", pageWidthCalc - 10, fontBold, g);
                                foreach (string line in lines1)
                                {
                                    g.DrawString(line, fontBold, Brushes.Black, marginLeft, y);
                                    y += (int)(fontBold.Size * 2.5f);
                                }

                                List<string> lines2 = WrapText($"Match: {bet["event_name"]}", pageWidthCalc - 10, fontApuestas, g);
                                foreach (string line in lines2)
                                {
                                    g.DrawString(line, fontApuestas, Brushes.Black, marginLeft, y);
                                    y += (int)(fontNormal.Size * 2.5f);
                                }

                                string line3 = $"{bet["market_name"]} - {bet["odds_name"]} ({bet["odds_value"]})";
                                List<string> lines3 = WrapText(line3, pageWidthCalc - 10, fontApuestas, g);
                                foreach (string l in lines3)
                                {
                                    g.DrawString(l, fontApuestas, Brushes.Black, marginLeft, y);
                                    y += (int)(fontNormal.Size * 3.0f);
                                }
                            }
                        }

                        // Línea divisoria final
                        g.DrawLine(dashPen, marginLeft, y, pageWidthCalc - marginLeft, y);
                        y += (int)(fontNormal.Size * 1.5f);

                        // Información de cuotas, importe, impuestos y ganancias
                        string stakeVal = ticketData["stake"]?.ToString();
                        string totalOdds = ticketData["total_odds"]?.ToString();
                        string maxWin = ticketData["max_win"]?.ToString();
                        string currencyCode = ticketData["ticket_currency"]?["code"]?.ToString();


                        g.DrawString($"Cuotas Totales: {totalOdds}", fontBold, Brushes.Black, marginLeft, y);
                        y += (int)(fontBold.Size * 2.5f);

                        g.DrawString($"Importe: {stakeVal} {currencyCode}", fontBold, Brushes.Black, marginLeft, y);
                        y += (int)(fontBold.Size * 2.5f);

                        // Otra línea divisoria
                        g.DrawLine(dashPen, marginLeft, y, pageWidthCalc - marginLeft, y);
                        y += (int)(fontBold.Size * 2.0f);



                        // Última línea divisoria
                        g.DrawLine(dashPen, marginLeft, y, pageWidthCalc - marginLeft, y);
                        y += (int)(fontBold.Size * 2.0f);

                        g.DrawString($"Ganancias: {maxWin} {currencyCode}", fontBold, Brushes.Black, marginLeft, y);
                        y += (int)(fontBold.Size * 2.5f);

                        // Última línea divisoria
                        g.DrawLine(dashPen, marginLeft, y, pageWidthCalc - marginLeft, y);
                        y += (int)(fontBold.Size * 2.0f);


                        string createdTimeFinal = "No disponible";
                        if (ticketData["checkout_time"] != null && DateTime.TryParse(ticketData["checkout_time"].ToString(), out DateTime tempParsedDateFinal))
                        {
                            createdTimeFinal = tempParsedDateFinal.ToString("dd/MM/yyyy HH:mm:ss");
                        }
                        string printedTimeFinal = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                        // Definir el cuadro inferior para "Creada" e "Impresa"
                        int infoBoxMarginFinal = 10;
                        int infoBoxPaddingFinal = 7; // Espacio interno
                        int infoBoxWidthFinal = pageWidthCalc - (2 * infoBoxMarginFinal);
                        // Calcular la altura necesaria para el texto de "Creada" e "Impresa"
                        SizeF createdSizeFinal = g.MeasureString($"Creada: {createdTimeFinal}", fontBold);
                        SizeF printedSizeFinal = g.MeasureString($"Impresa: {printedTimeFinal}", fontBold);
                        int infoBoxHeightFinal = (int)(createdSizeFinal.Height + printedSizeFinal.Height) + (2 * infoBoxPaddingFinal);

                        // Definir el rectángulo del cuadro inferior, ubicado justo debajo de "Ganancias"
                        Rectangle infoBoxRect2Final = new Rectangle(marginLeft, y, infoBoxWidthFinal, infoBoxHeightFinal);
                        g.DrawRectangle(solidPen, infoBoxRect2Final);

                        // Dibujar "Creada" e "Impresa" dentro del cuadro inferior
                        float infoYFinal = infoBoxRect2Final.Y + infoBoxPaddingFinal;
                        string headerTextFinal = $"Creada: {createdTimeFinal}    Impresa: {printedTimeFinal}";

                        // Envolver el texto si es necesario
                        List<string> headerLinesFinal = WrapText(headerTextFinal, infoBoxWidthFinal - (2 * infoBoxPaddingFinal), fontBold, g);

                        foreach (string line in headerLinesFinal)
                        {
                            g.DrawString(line, fontBold, Brushes.Black,
                                new RectangleF(infoBoxRect2Final.X + infoBoxPaddingFinal, infoYFinal, infoBoxWidthFinal - (2 * infoBoxPaddingFinal), createdSizeFinal.Height),
                                new StringFormat { Alignment = StringAlignment.Near }
                            );
                            infoYFinal += createdSizeFinal.Height + lineSpacingBox;
                        }

                        y += infoBoxHeightFinal + 10;


                        // Dibujar la línea divisoria después del cuadro
                        g.DrawLine(dashPen, marginLeft, y, pageWidthCalc - marginLeft, y);
                        y += (int)(fontBold.Size * 2.0f);

                        string rawPrintId = ticketData["print_id"]?.ToString();

                        if (string.IsNullOrEmpty(rawPrintId) || rawPrintId == "No disponible")
                        {
                            string mensajeReimpresion = "Esto es una reimpresión";

                            float reimpresionX = (pageWidthCalc - g.MeasureString(mensajeReimpresion, fontBold).Width) / 2;
                            g.DrawString(mensajeReimpresion, fontBold, Brushes.Black, reimpresionX, y);
                            y += 30;
                        }
                        else
                        {
                            string[] parts = rawPrintId.Split('-');

                            string codeToPrint = rawPrintId;
                            if (parts.Length >= 3)
                            {
                                codeToPrint = parts[1].Trim();
                            }
                            else if (parts.Length == 2)
                            {
                                codeToPrint = parts[1].Trim();
                            }

                            ZXing.OneD.Code128Writer writerBarcode = new ZXing.OneD.Code128Writer();
                            BitMatrix matrixBarcode = writerBarcode.encode(
                                codeToPrint,
                                BarcodeFormat.CODE_128,
                                Properties.Settings.Default.PaperSize == "58 mm" ? 150 : 200,
                                Properties.Settings.Default.PaperSize == "58 mm" ? 60 : 100
                            );

                            Bitmap barcodeBitmapFinal = new Bitmap(matrixBarcode.Width, matrixBarcode.Height);
                            for (int yPos = 0; yPos < matrixBarcode.Height; yPos++)
                            {
                                for (int xPos = 0; xPos < matrixBarcode.Width; xPos++)
                                {
                                    barcodeBitmapFinal.SetPixel(xPos, yPos, matrixBarcode[xPos, yPos] ? Color.Black : Color.White);
                                }
                            }

                            int xBarcode = (pageWidthCalc - matrixBarcode.Width) / 2;
                            g.DrawImage(barcodeBitmapFinal, new Rectangle(xBarcode, y, matrixBarcode.Width, matrixBarcode.Height));
                            y += matrixBarcode.Height + 10;
                        }
                        string line1 = "Acepto los términos y condiciones";
                        string line2 = "ec.aciertala.com";
                        string line4 = "Visítanos en:";

                        Font fontCentered = new Font("Arial", fontNormalSize, FontStyle.Regular);

                        SizeF sizeLine1 = g.MeasureString(line1, fontCentered);
                        SizeF sizeLine4 = g.MeasureString(line4, fontCentered);
                        SizeF sizeLine2 = g.MeasureString(line2, fontCentered);

                        float xLine1 = (pageWidthCalc - sizeLine1.Width) / 2 + marginLeft;
                        float xLine2 = (pageWidthCalc - sizeLine2.Width) / 2 + marginLeft;
                        float xLine4 = (pageWidthCalc - sizeLine4.Width) / 2 + marginLeft;

                        g.DrawString(line1, fontCentered, Brushes.Black, xLine1, y);
                        y += (int)(fontCentered.Size * 1.5f);
                        g.DrawString(line4, fontCentered, Brushes.Black, xLine4, y);
                        y += (int)(fontCentered.Size * 2.0f);
                        g.DrawString(line2, fontCentered, Brushes.Black, xLine2, y);
                        y += (int)(fontCentered.Size * 2.5f);
                    };

                    printDoc.Print();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ProcessTicketResponse ERROR] {ex.Message}");
                }
            });
        }

        private void DrawBox(Graphics g, string text, Font font, int marginLeft, int y, int pageWidth, string tipoImpresora)
        {
            int boxWidth = pageWidth - (2 * marginLeft);
            SizeF textSize = g.MeasureString(text, font);

            if (tipoImpresora == "58 mm")

            {
                Pen dashPen = new Pen(Color.Black, 1);
                dashPen.DashPattern = new float[] { 2, 2 };

                int topY = y;
                int bottomY = y + (int)textSize.Height + 10;
                int leftX = marginLeft;
                int rightX = leftX + boxWidth;

                g.DrawLine(dashPen, leftX, topY, rightX, topY);
                g.DrawLine(dashPen, leftX, topY, leftX, bottomY);
                g.DrawLine(dashPen, rightX, topY, rightX, bottomY);
                g.DrawLine(dashPen, leftX, bottomY, rightX, bottomY);

                float textX = leftX + (boxWidth - textSize.Width) / 2;
                float textY = topY + ((bottomY - topY - textSize.Height) / 2);
                g.DrawString(text, font, Brushes.Black, textX, textY);
            }
            else
            {
                int rectX = marginLeft;
                int rectY = y;
                int rectWidth = boxWidth;
                int rectHeight = (int)textSize.Height + 15;

                g.FillRectangle(Brushes.Black, rectX, rectY, rectWidth, rectHeight);
                g.DrawString(text, font, Brushes.White, rectX + (rectWidth - textSize.Width) / 2, rectY + 5);
            }
        }

        private List<string> WrapText(string input, int maxWidth, Font font, Graphics g)
        {
            string[] words = input.Split(' ');
            List<string> lines = new List<string>();
            string currentLine = "";

            foreach (string word in words)
            {
                string testLine = string.IsNullOrEmpty(currentLine) ? word : $"{currentLine} {word}";
                SizeF size = g.MeasureString(testLine, font);

                if (size.Width > maxWidth)
                {
                    if (!string.IsNullOrEmpty(currentLine))
                    {
                        lines.Add(currentLine);
                    }
                    currentLine = word;
                }
                else
                {
                    currentLine = testLine;
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
            {
                lines.Add(currentLine);
            }

            return lines;
        }

        private void TerminalLogin_Deactivate(object sender, EventArgs e)
        {
            this.Close(); // Cierra el formulario al quedar en segundo plano
        }

    }
}
