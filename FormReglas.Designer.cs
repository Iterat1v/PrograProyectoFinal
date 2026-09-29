namespace PrograProyectoFinal
{
    partial class FormReglas
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
            btnVolver = new BotonPixel();
            SuspendLayout();
            //
            // btnVolver
            //
            btnVolver.Location = new Point(1000, 612);
            btnVolver.Name = "btnVolver";
            btnVolver.Size = new Size(244, 82);
            btnVolver.TabIndex = 0;
            btnVolver.Text = "VOLVER";
            btnVolver.Click += btnVolver_Click;
            //
            // FormReglas
            //
            AcceptButton = btnVolver;
            CancelButton = btnVolver;
            ClientSize = new Size(1280, 720);
            Controls.Add(btnVolver);
            Name = "FormReglas";
            StartPosition = FormStartPosition.CenterParent;
            Titulo = "CÓMO SE JUEGA";
            ResumeLayout(false);
        }

        #endregion

        private BotonPixel btnVolver;
    }
}
