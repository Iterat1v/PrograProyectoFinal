namespace PrograProyectoFinal
{
    partial class FormTitulo
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
            btnJugar = new BotonPixel();
            btnComoSeJuega = new BotonPixel();
            btnSalir = new BotonPixel();
            SuspendLayout();
            //
            // btnJugar
            //
            btnJugar.Location = new Point(180, 600);
            btnJugar.Name = "btnJugar";
            btnJugar.Size = new Size(284, 82);
            btnJugar.TabIndex = 0;
            btnJugar.Text = "JUGAR";
            btnJugar.Click += btnJugar_Click;
            //
            // btnComoSeJuega
            //
            btnComoSeJuega.Location = new Point(500, 600);
            btnComoSeJuega.Name = "btnComoSeJuega";
            btnComoSeJuega.Size = new Size(284, 82);
            btnComoSeJuega.TabIndex = 1;
            btnComoSeJuega.Text = "CÓMO SE JUEGA";
            btnComoSeJuega.Click += btnComoSeJuega_Click;
            //
            // btnSalir
            //
            btnSalir.Location = new Point(820, 600);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(284, 82);
            btnSalir.TabIndex = 2;
            btnSalir.Text = "SALIR";
            btnSalir.Click += btnSalir_Click;
            //
            // FormTitulo
            //
            AcceptButton = btnJugar;
            ClientSize = new Size(1280, 720);
            Controls.Add(btnJugar);
            Controls.Add(btnComoSeJuega);
            Controls.Add(btnSalir);
            Name = "FormTitulo";
            Text = "Concesionario";
            ResumeLayout(false);
        }

        #endregion

        private BotonPixel btnJugar;
        private BotonPixel btnComoSeJuega;
        private BotonPixel btnSalir;
    }
}
