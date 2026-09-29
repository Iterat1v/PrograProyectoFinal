namespace PrograProyectoFinal
{
    partial class FormTablero
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
            btnMercado = new BotonPixel();
            btnVender = new BotonPixel();
            btnUsarCarta = new BotonPixel();
            btnPasar = new BotonPixel();
            btnAnterior = new BotonPixel();
            btnSiguiente = new BotonPixel();
            SuspendLayout();
            //
            // btnMercado
            //
            btnMercado.Location = new Point(290, 12);
            btnMercado.Name = "btnMercado";
            btnMercado.Size = new Size(158, 62);
            btnMercado.TabIndex = 0;
            btnMercado.TamLetra = 12F;
            btnMercado.Text = "MERCADO";
            btnMercado.Click += btnMercado_Click;
            //
            // btnVender
            //
            btnVender.Location = new Point(420, 504);
            btnVender.Name = "btnVender";
            btnVender.Size = new Size(244, 82);
            btnVender.TabIndex = 1;
            btnVender.Text = "VENDER";
            btnVender.Click += btnVender_Click;
            //
            // btnUsarCarta
            //
            btnUsarCarta.Location = new Point(680, 504);
            btnUsarCarta.Name = "btnUsarCarta";
            btnUsarCarta.Size = new Size(244, 82);
            btnUsarCarta.TabIndex = 2;
            btnUsarCarta.Text = "USAR CARTA";
            btnUsarCarta.Click += btnUsarCarta_Click;
            //
            // btnPasar
            //
            btnPasar.Location = new Point(420, 600);
            btnPasar.Name = "btnPasar";
            btnPasar.Size = new Size(504, 74);
            btnPasar.TabIndex = 3;
            btnPasar.Text = "PASAR TURNO";
            btnPasar.Click += btnPasar_Click;
            //
            // btnAnterior
            //
            btnAnterior.Location = new Point(300, 116);
            btnAnterior.Name = "btnAnterior";
            btnAnterior.Size = new Size(40, 42);
            btnAnterior.TabIndex = 4;
            btnAnterior.TamLetra = 11F;
            btnAnterior.Text = "<";
            btnAnterior.Click += btnAnterior_Click;
            //
            // btnSiguiente
            //
            btnSiguiente.Location = new Point(344, 116);
            btnSiguiente.Name = "btnSiguiente";
            btnSiguiente.Size = new Size(40, 42);
            btnSiguiente.TabIndex = 5;
            btnSiguiente.TamLetra = 11F;
            btnSiguiente.Text = ">";
            btnSiguiente.Click += btnSiguiente_Click;
            //
            // FormTablero
            //
            ClientSize = new Size(1280, 720);
            Controls.Add(btnMercado);
            Controls.Add(btnVender);
            Controls.Add(btnUsarCarta);
            Controls.Add(btnPasar);
            Controls.Add(btnAnterior);
            Controls.Add(btnSiguiente);
            Name = "FormTablero";
            ResumeLayout(false);
        }

        #endregion

        private BotonPixel btnMercado;
        private BotonPixel btnVender;
        private BotonPixel btnUsarCarta;
        private BotonPixel btnPasar;
        private BotonPixel btnAnterior;
        private BotonPixel btnSiguiente;
    }
}
