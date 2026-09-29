namespace PrograProyectoFinal
{
    partial class FormCartas
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
            btnLista = new BotonPixel();
            btnVolver = new BotonPixel();
            SuspendLayout();
            //
            // btnLista
            //
            btnLista.Location = new Point(720, 628);
            btnLista.Name = "btnLista";
            btnLista.Size = new Size(260, 76);
            btnLista.TabIndex = 0;
            btnLista.Text = "VER LAS 25";
            btnLista.Click += btnLista_Click;
            //
            // btnVolver
            //
            btnVolver.Location = new Point(1000, 628);
            btnVolver.Name = "btnVolver";
            btnVolver.Size = new Size(244, 76);
            btnVolver.TabIndex = 1;
            btnVolver.Text = "VOLVER";
            btnVolver.Click += btnVolver_Click;
            //
            // FormCartas
            //
            CancelButton = btnVolver;
            ClientSize = new Size(1280, 720);
            Controls.Add(btnLista);
            Controls.Add(btnVolver);
            Name = "FormCartas";
            StartPosition = FormStartPosition.CenterParent;
            Titulo = "LAS CARTAS · 25 EN LA BARAJA";
            ResumeLayout(false);
        }

        #endregion

        private BotonPixel btnLista;
        private BotonPixel btnVolver;
    }
}
