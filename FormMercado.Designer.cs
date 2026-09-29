namespace PrograProyectoFinal
{
    partial class FormMercado
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
            btnComprar = new BotonPixel();
            btnCerrar = new BotonPixel();
            SuspendLayout();
            //
            // btnComprar
            //
            btnComprar.Location = new Point(800, 632);
            btnComprar.Name = "btnComprar";
            btnComprar.Size = new Size(208, 76);
            btnComprar.TabIndex = 0;
            btnComprar.Text = "COMPRAR";
            btnComprar.Click += btnComprar_Click;
            //
            // btnCerrar
            //
            btnCerrar.Location = new Point(1032, 632);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(208, 76);
            btnCerrar.TabIndex = 1;
            btnCerrar.Text = "CERRAR";
            btnCerrar.Click += btnCerrar_Click;
            //
            // FormMercado
            //
            AcceptButton = btnComprar;
            CancelButton = btnCerrar;
            ClientSize = new Size(1280, 720);
            Controls.Add(btnComprar);
            Controls.Add(btnCerrar);
            Name = "FormMercado";
            StartPosition = FormStartPosition.CenterParent;
            Titulo = "MERCADO · 30 AUTOS DISPONIBLES";
            ResumeLayout(false);
        }

        #endregion

        private BotonPixel btnComprar;
        private BotonPixel btnCerrar;
    }
}
