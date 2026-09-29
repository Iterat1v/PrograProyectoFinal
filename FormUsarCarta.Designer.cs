namespace PrograProyectoFinal
{
    partial class FormUsarCarta
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
            btnUsar = new BotonPixel();
            btnCerrar = new BotonPixel();
            SuspendLayout();
            //
            // btnUsar
            //
            btnUsar.Location = new Point(692, 620);
            btnUsar.Name = "btnUsar";
            btnUsar.Size = new Size(268, 82);
            btnUsar.TabIndex = 0;
            btnUsar.Text = "USAR";
            btnUsar.Click += btnUsar_Click;
            //
            // btnCerrar
            //
            btnCerrar.Location = new Point(976, 620);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(268, 82);
            btnCerrar.TabIndex = 1;
            btnCerrar.Text = "CERRAR";
            btnCerrar.Click += btnCerrar_Click;
            //
            // FormUsarCarta
            //
            AcceptButton = btnUsar;
            CancelButton = btnCerrar;
            ClientSize = new Size(1280, 720);
            Controls.Add(btnUsar);
            Controls.Add(btnCerrar);
            Name = "FormUsarCarta";
            StartPosition = FormStartPosition.CenterParent;
            Titulo = "TUS ABILICARDS";
            ResumeLayout(false);
        }

        #endregion

        private BotonPixel btnUsar;
        private BotonPixel btnCerrar;
    }
}
