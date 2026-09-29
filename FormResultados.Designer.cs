namespace PrograProyectoFinal
{
    partial class FormResultados
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
            btnRevancha = new BotonPixel();
            btnNueva = new BotonPixel();
            btnSalir = new BotonPixel();
            SuspendLayout();
            //
            // btnNueva
            //
            btnRevancha.Location = new Point(150, 628);
            btnRevancha.Name = "btnRevancha";
            btnRevancha.Size = new Size(304, 78);
            btnRevancha.TabIndex = 2;
            btnRevancha.Text = "REVANCHA";
            btnRevancha.Click += btnRevancha_Click;
            btnNueva.Location = new Point(488, 628);
            btnNueva.Name = "btnNueva";
            btnNueva.Size = new Size(304, 78);
            btnNueva.TabIndex = 0;
            btnNueva.Text = "NUEVA PARTIDA";
            btnNueva.Click += btnNueva_Click;
            //
            // btnSalir
            //
            btnSalir.Location = new Point(826, 628);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(304, 78);
            btnSalir.TabIndex = 1;
            btnSalir.Text = "SALIR";
            btnSalir.Click += btnSalir_Click;
            //
            // FormResultados
            //
            AcceptButton = btnRevancha;
            ClientSize = new Size(1280, 720);
            Controls.Add(btnRevancha);
            Controls.Add(btnNueva);
            Controls.Add(btnSalir);
            Name = "FormResultados";
            ResumeLayout(false);
        }

        #endregion

        private BotonPixel btnRevancha;
        private BotonPixel btnNueva;
        private BotonPixel btnSalir;
    }
}
