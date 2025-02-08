using System;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;
using AciertalaV3;

namespace AciertalaV3
{
    public partial class FormPrintSettings : Form
    {
        private readonly string DefaultLogoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logo", "LogoAciertala.png");

        public FormPrintSettings()
        {
            InitializeComponent();
        }

        private void FormPrintSettings_Load(object sender, EventArgs e)
        {
            cmbPrinters.Items.Clear();
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                cmbPrinters.Items.Add(printer);
            }

            if (!string.IsNullOrEmpty(Properties.Settings.Default.PrinterName))
            {
                cmbPrinters.SelectedItem = Properties.Settings.Default.PrinterName;
            }

            if (Properties.Settings.Default.PaperSize == "58mm")
            {
                rb58mm.Checked = true;
            }
            else if (Properties.Settings.Default.PaperSize == "80mm")
            {
                rb80mm.Checked = true;
            }

            txtTerminal.Text = Properties.Settings.Default.Terminal;


        }



        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cmbPrinters.SelectedItem != null)
            {
                Properties.Settings.Default.PrinterName = cmbPrinters.SelectedItem.ToString();
            }

            if (rb58mm.Checked)
            {
                Properties.Settings.Default.PaperSize = "58mm";
            }
            else if (rb80mm.Checked)
            {
                Properties.Settings.Default.PaperSize = "80mm";
            }

            Properties.Settings.Default.Terminal = txtTerminal.Text;

  

            Properties.Settings.Default.Save();


            MessageBox.Show("Configuraciones guardadas correctamente.", "Configuración de Impresión", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Close();
        }

 

        private void Button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.PrinterName = string.Empty;
            Properties.Settings.Default.PaperSize = "58mm";
            Properties.Settings.Default.LogoPath = DefaultLogoPath;



            Properties.Settings.Default.Save();

            cmbPrinters.SelectedItem = null;
            rb58mm.Checked = true;


            MessageBox.Show("Las configuraciones se han restablecido a los valores predeterminados.", "Configuración de Impresión", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
