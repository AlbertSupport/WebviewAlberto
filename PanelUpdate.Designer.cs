namespace WebviewAlberto
{
    partial class PanelUpdate
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblMensaje;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.Button btnRecordarMasTarde;
        private System.Windows.Forms.Label lblContador;
        private System.Windows.Forms.Timer timerActualizacion;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.btnRecordarMasTarde = new System.Windows.Forms.Button();
            this.lblContador = new System.Windows.Forms.Label();
            this.timerActualizacion = new System.Windows.Forms.Timer();

            // Configuración del formulario
            this.ClientSize = new System.Drawing.Size(450, 280);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Actualización Disponible";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.ControlBox = false;

            // lblTitulo
            this.lblTitulo.Text = "¡Nueva versión disponible!";
            this.lblTitulo.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTitulo.Location = new System.Drawing.Point(50, 20);
            this.lblTitulo.Size = new System.Drawing.Size(350, 30);

            // lblMensaje
            this.lblMensaje.Text = "Se ha detectado una nueva versión de la aplicación.\nEs recomendable actualizar para acceder a las mejoras.";
            this.lblMensaje.Font = new System.Drawing.Font("Arial", 10F);
            this.lblMensaje.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMensaje.Location = new System.Drawing.Point(30, 60);
            this.lblMensaje.Size = new System.Drawing.Size(390, 50);

            // btnActualizar
            this.btnActualizar.Text = "Actualizar ahora";
            this.btnActualizar.Location = new System.Drawing.Point(60, 140);
            this.btnActualizar.Size = new System.Drawing.Size(150, 50);
            this.btnActualizar.BackColor = System.Drawing.Color.Green;
            this.btnActualizar.ForeColor = System.Drawing.Color.White;
            this.btnActualizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnActualizar.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnActualizar.TextAlign = System.Drawing.ContentAlignment.MiddleCenter; // Centrar texto
            this.btnActualizar.Padding = new System.Windows.Forms.Padding(0);
            this.btnActualizar.FlatAppearance.BorderSize = 0; // Eliminar bordes
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);

            // btnRecordarMasTarde
            this.btnRecordarMasTarde.Text = "Recordar más tarde";
            this.btnRecordarMasTarde.Location = new System.Drawing.Point(240, 140);
            this.btnRecordarMasTarde.Size = new System.Drawing.Size(150, 50);
            this.btnRecordarMasTarde.BackColor = System.Drawing.Color.Orange;
            this.btnRecordarMasTarde.ForeColor = System.Drawing.Color.White;
            this.btnRecordarMasTarde.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRecordarMasTarde.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnRecordarMasTarde.TextAlign = System.Drawing.ContentAlignment.MiddleCenter; // Centrar texto
            this.btnRecordarMasTarde.Padding = new System.Windows.Forms.Padding(0);
            this.btnRecordarMasTarde.FlatAppearance.BorderSize = 0; // Eliminar bordes
            this.btnRecordarMasTarde.Click += new System.EventHandler(this.btnRecordarMasTarde_Click);

            // lblContador
            this.lblContador.Text = "La actualización comenzará en 50 segundos...";
            this.lblContador.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblContador.ForeColor = System.Drawing.Color.Red;
            this.lblContador.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblContador.Location = new System.Drawing.Point(50, 210);
            this.lblContador.Size = new System.Drawing.Size(350, 30);

            // Timer Config
            this.timerActualizacion.Interval = 1000;
            this.timerActualizacion.Tick += new System.EventHandler(this.timerActualizacion_Tick);

            // Agregar controles al formulario
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.btnActualizar);
            this.Controls.Add(this.btnRecordarMasTarde);
            this.Controls.Add(this.lblContador);
        }

        #endregion
    }
}
