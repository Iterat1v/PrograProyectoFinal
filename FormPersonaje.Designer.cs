namespace PrograProyectoFinal
{
    partial class FormPersonaje
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
            btnAzar = new BotonPixel();
            btnListo = new BotonPixel();
            SuspendLayout();
            //
            // btnAzar
            //
            btnAzar.Location = new Point(724, 632);
            btnAzar.Name = "btnAzar";
            btnAzar.Size = new Size(240, 72);
            btnAzar.TabIndex = 0;
            btnAzar.Text = "AL AZAR";
            btnAzar.Click += btnAzar_Click;
            //
            // btnListo
            //
            btnListo.Location = new Point(980, 632);
            btnListo.Name = "btnListo";
            btnListo.Size = new Size(260, 72);
            btnListo.TabIndex = 1;
            btnListo.Text = "LISTO";
            btnListo.Click += btnListo_Click;
            //
            // FormPersonaje
            //
            AcceptButton = btnListo;
            CancelButton = btnListo;
            ClientSize = new Size(1280, 720);
            Controls.Add(btnAzar);
            Controls.Add(btnListo);
            Name = "FormPersonaje";
            StartPosition = FormStartPosition.CenterParent;
            Titulo = "TU PERSONAJE · TIENDA";
            ResumeLayout(false);
        }

        #endregion

        private BotonPixel btnAzar;
        private BotonPixel btnListo;
    }
}
