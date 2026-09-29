namespace PrograProyectoFinal
{
    partial class FormRegistro
    {
        private System.ComponentModel.IContainer components = null;

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
            txtNombre = new TextBox();
            btnAgregar = new BotonPixel();
            btnLimpiar = new BotonPixel();
            btnJugar = new BotonPixel();
            SuspendLayout();
            //
            // txtNombre
            //
            txtNombre.BackColor = Color.FromArgb(255, 244, 232);
            txtNombre.BorderStyle = BorderStyle.None;
            txtNombre.ForeColor = Color.FromArgb(27, 10, 46);
            txtNombre.Location = new Point(68, 506);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(560, 44);
            txtNombre.TabIndex = 0;
            txtNombre.TextChanged += txtNombre_TextChanged;
            txtNombre.KeyUp += txtNombre_KeyUp;
            //
            // btnAgregar
            //
            btnAgregar.Location = new Point(40, 600);
            btnAgregar.Name = "btnAgregar";
            btnAgregar.Size = new Size(364, 86);
            btnAgregar.TabIndex = 1;
            btnAgregar.Text = "AGREGAR JUGADOR";
            btnAgregar.Click += btnAgregar_Click;
            //
            // btnLimpiar
            //
            btnLimpiar.Location = new Point(424, 600);
            btnLimpiar.Name = "btnLimpiar";
            btnLimpiar.Size = new Size(244, 86);
            btnLimpiar.TabIndex = 2;
            btnLimpiar.Text = "LIMPIAR";
            btnLimpiar.Click += btnLimpiar_Click;
            //
            // btnJugar
            //
            btnJugar.Location = new Point(880, 600);
            btnJugar.Name = "btnJugar";
            btnJugar.Size = new Size(364, 86);
            btnJugar.TabIndex = 3;
            btnJugar.Text = "¡A JUGAR!";
            btnJugar.Click += btnJugar_Click;
            //
            // FormRegistro
            //
            AcceptButton = btnAgregar;
            ClientSize = new Size(1280, 720);
            Controls.Add(txtNombre);
            Controls.Add(btnAgregar);
            Controls.Add(btnLimpiar);
            Controls.Add(btnJugar);
            Name = "FormRegistro";
            Titulo = "¡BIENVENIDO! · ELIGE A TU VENDEDOR";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtNombre;
        private BotonPixel btnAgregar;
        private BotonPixel btnLimpiar;
        private BotonPixel btnJugar;
    }
}
