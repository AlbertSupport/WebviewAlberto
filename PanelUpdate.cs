using SharpCompress.Archives;
using SharpCompress.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Windows.Forms;

namespace WebviewAlberto
{
    public partial class PanelUpdate : Form
    {
        private static readonly string downloadUrl = "https://apk.solutions/prueba/jsonaciertala/AciertalaApp.rar";
        private static readonly string downloadPath = @"C:\Actualizaciones\AciertalaApp.rar";
        private static readonly string extractPath = @"C:\Actualizaciones\Descomprimido";
        private static readonly string versionFilePath = Path.Combine(extractPath, "version.json");
        private static readonly string localVersionPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AciertalaApp", "version.json");

        private int tiempoRestante = 50; // Contador en segundos
        public bool HayActualizacion { get; private set; } = false;

        public PanelUpdate()
        {
            InitializeComponent();

            // Verificar actualización
            HayActualizacion = VerificarActualizacion();

            if (HayActualizacion)
            {
                timerActualizacion.Start(); // Inicia la cuenta regresiva si hay actualización
            }
            else
            {
                this.DialogResult = DialogResult.Cancel; // Se cierra sin error
                this.Close();
            }
        }

        private bool VerificarActualizacion()
        {
            try
            {
                // Descargar el archivo
                DownloadFile(downloadUrl, downloadPath);
                ExtractRar(downloadPath, extractPath);

                // Leer la versión del archivo descargado
                string nuevaVersion = LeerVersion(versionFilePath);
                string versionLocal = LeerVersion(localVersionPath);

                // Comparar versiones
                if (nuevaVersion != versionLocal)
                {
                    GuardarVersionLocal(versionFilePath, localVersionPath);
                    return true; // Hay actualización
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al verificar actualización: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false; // No hay actualización
        }

        private static void DownloadFile(string url, string filePath)
        {
            string folderPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(url, filePath);
            }
        }

        private static void ExtractRar(string rarFilePath, string destinationPath)
        {
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            using (var archive = ArchiveFactory.Open(rarFilePath))
            {
                foreach (var entry in archive.Entries)
                {
                    if (!entry.IsDirectory)
                    {
                        entry.WriteToDirectory(destinationPath, new ExtractionOptions
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        });
                    }
                }
            }
        }

        private string LeerVersion(string rutaArchivo)
        {
            if (File.Exists(rutaArchivo))
            {
                string json = File.ReadAllText(rutaArchivo);
                var doc = JsonSerializer.Deserialize<JsonElement>(json);
                if (doc.TryGetProperty("version", out JsonElement version))
                {
                    return version.GetString();
                }
            }
            return "0.0"; // Versión por defecto si no existe
        }

        private void GuardarVersionLocal(string origen, string destino)
        {
            try
            {
                string directorioDestino = Path.GetDirectoryName(destino);
                if (!Directory.Exists(directorioDestino))
                {
                    Directory.CreateDirectory(directorioDestino);
                }
                File.Copy(origen, destino, true); // Sobrescribe si ya existe
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la versión local: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            IniciarActualizacion();
        }

        private void btnRecordarMasTarde_Click(object sender, EventArgs e)
        {
            timerActualizacion.Stop(); // Detiene el contador
            this.Close(); // Cierra el formulario
        }

        private void timerActualizacion_Tick(object sender, EventArgs e)
        {
            tiempoRestante--;
            lblContador.Text = $"La actualización comenzará en {tiempoRestante} segundos...";

            if (tiempoRestante <= 0)
            {
                timerActualizacion.Stop();
                IniciarActualizacion();
            }
        }

        private void IniciarActualizacion()
        {
            MessageBox.Show("Iniciando actualización...", "Actualización", MessageBoxButtons.OK, MessageBoxIcon.Information);

            string setupPath = @"C:\Actualizaciones\Descomprimido\SetupAciertala.exe";

            if (File.Exists(setupPath))
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = setupPath,
                        Arguments = "/verysilent /norestart", // Ejecutar en modo silencioso
                        UseShellExecute = false, // No usar shell
                        CreateNoWindow = true // No crear ventana de consola
                    };

                    using (Process installer = Process.Start(psi)) // Inicia el proceso
                    {
                        if (installer != null)
                        {
                            installer.WaitForExit(); // Espera a que el proceso de instalación termine
                        }
                    }

                    // Una vez que la instalación termine, cierra la ventana
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al ejecutar la actualización: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No se encontró el archivo de instalación.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



    }
}
