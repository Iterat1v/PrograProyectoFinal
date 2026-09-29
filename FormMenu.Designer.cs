namespace PrograProyectoFinal
{
    partial class FormMenu
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
            btnContinuar = new BotonPixel();
            btnRevancha = new BotonPixel();
            btnRendirse = new BotonPixel();
            btnMusica = new BotonPixel();
            btnSonidos = new BotonPixel();
            btnPasar = new BotonPixel();
            btnTitulo = new BotonPixel();
            SuspendLayout();
            //
            // btnContinuar
            //
            btnContinuar.Location = new Point(60, 110);
            btnContinuar.Name = "btnContinuar";
            btnContinuar.Size = new Size(444, 70);
            btnContinuar.TabIndex = 0;
            btnContinuar.Text = "CONTINUAR";
            btnContinuar.Click += btnContinuar_Click;
            //
            // btnRevancha
            //
            btnRevancha.Location = new Point(60, 188);
            btnRevancha.Name = "btnRevancha";
            btnRevancha.Size = new Size(444, 70);
            btnRevancha.TabIndex = 1;
            btnRevancha.Text = "REVANCHA";
            btnRevancha.Click += btnRevancha_Click;
            //
            // btnRendirse
            //
            btnRendirse.Location = new Point(60, 266);
            btnRendirse.Name = "btnRendirse";
            btnRendirse.Size = new Size(444, 70);
            btnRendirse.TabIndex = 2;
            btnRendirse.Text = "RENDIRSE";
            btnRendirse.Click += btnRendirse_Click;
            //
            // btnMusica
            //
            btnMusica.Location = new Point(60, 344);
            btnMusica.Name = "btnMusica";
            btnMusica.Size = new Size(444, 70);
            btnMusica.TabIndex = 3;
            btnMusica.TamLetra = 12F;
            btnMusica.Text = "MÚSICA";
            btnMusica.Click += btnMusica_Click;
            //
            // btnSonidos
            //
            btnSonidos.Location = new Point(60, 422);
            btnSonidos.Name = "btnSonidos";
            btnSonidos.Size = new Size(444, 70);
            btnSonidos.TabIndex = 4;
            btnSonidos.TamLetra = 12F;
            btnSonidos.Text = "SONIDOS";
            btnSonidos.Click += btnSonidos_Click;
            //
            // btnPasar
            //
            btnPasar.Location = new Point(60, 500);
            btnPasar.Name = "btnPasar";
            btnPasar.Size = new Size(444, 70);
            btnPasar.TabIndex = 5;
            btnPasar.TamLetra = 12F;
            btnPasar.Text = "PASAR SOLO AL VENDER";
            btnPasar.Click += btnPasar_Click;
            //
            // btnTitulo
            //
            btnTitulo.Location = new Point(60, 578);
            btnTitulo.Name = "btnTitulo";
            btnTitulo.Size = new Size(444, 70);
            btnTitulo.TabIndex = 6;
            btnTitulo.Text = "SALIR AL TÍTULO";
            btnTitulo.Click += btnTitulo_Click;
            //
            // FormMenu
            //
            CancelButton = btnContinuar;
            ClientSize = new Size(560, 660);
            Controls.Add(btnContinuar);
            Controls.Add(btnRevancha);
            Controls.Add(btnRendirse);
            Controls.Add(btnMusica);
            Controls.Add(btnSonidos);
            Controls.Add(btnPasar);
            Controls.Add(btnTitulo);
            Name = "FormMenu";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            ResumeLayout(false);
        }

        #endregion

        private BotonPixel btnContinuar;
        private BotonPixel btnRevancha;
        private BotonPixel btnRendirse;
        private BotonPixel btnMusica;
        private BotonPixel btnSonidos;
        private BotonPixel btnPasar;
        private BotonPixel btnTitulo;
    }
}
