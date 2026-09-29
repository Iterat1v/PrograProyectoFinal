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
            btnNueva = new BotonPixel();
            btnSalir = new BotonPixel();
            SuspendLayout();
            //
            // btnNueva
            //
            btnNueva.Location = new Point(320, 628);
            btnNueva.Name = "btnNueva";
            btnNueva.Size = new Size(304, 78);
            btnNueva.TabIndex = 0;
            btnNueva.Text = "NUEVA PARTIDA";
            btnNueva.Click += btnNueva_Click;
            //
            // btnSalir
            //
            btnSalir.Location = new Point(660, 628);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(304, 78);
            btnSalir.TabIndex = 1;
            btnSalir.Text = "SALIR";
            btnSalir.Click += btnSalir_Click;
            //
            // FormResultados
            //
            AcceptButton = btnNueva;
            ClientSize = new Size(1280, 720);
            Controls.Add(btnNueva);
            Controls.Add(btnSalir);
            Name = "FormResultados";
            ResumeLayout(false);
        }

        #endregion

        private BotonPixel btnNueva;
        private BotonPixel btnSalir;
    }
}
