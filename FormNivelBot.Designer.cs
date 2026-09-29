namespace PrograProyectoFinal
{
    partial class FormNivelBot
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
            btnBotardo = new BotonPixel();
            btnMedio = new BotonPixel();
            btnPro = new BotonPixel();
            btnDios = new BotonPixel();
            btnCancelar = new BotonPixel();
            SuspendLayout();
            //
            // btnBotardo
            //
            btnBotardo.Location = new Point(110, 80);
            btnBotardo.Name = "btnBotardo";
            btnBotardo.Size = new Size(484, 56);
            btnBotardo.TabIndex = 0;
            btnBotardo.Text = "BOTARDO";
            btnBotardo.Click += btnBotardo_Click;
            //
            // btnMedio
            //
            btnMedio.Location = new Point(110, 184);
            btnMedio.Name = "btnMedio";
            btnMedio.Size = new Size(484, 56);
            btnMedio.TabIndex = 1;
            btnMedio.Text = "MEDIO";
            btnMedio.Click += btnMedio_Click;
            //
            // btnPro
            //
            btnPro.Location = new Point(110, 288);
            btnPro.Name = "btnPro";
            btnPro.Size = new Size(484, 56);
            btnPro.TabIndex = 2;
            btnPro.Text = "PRO";
            btnPro.Click += btnPro_Click;
            //
            // btnDios
            //
            btnDios.Location = new Point(110, 392);
            btnDios.Name = "btnDios";
            btnDios.Size = new Size(484, 56);
            btnDios.TabIndex = 3;
            btnDios.TamLetra = 11F;
            btnDios.Text = "SSJ ULTRA INSTINTO FASE DIOS 1000";
            btnDios.Click += btnDios_Click;
            //
            // btnCancelar
            //
            btnCancelar.Location = new Point(230, 516);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(244, 62);
            btnCancelar.TabIndex = 4;
            btnCancelar.Text = "CANCELAR";
            btnCancelar.Click += btnCancelar_Click;
            //
            // FormNivelBot
            //
            CancelButton = btnCancelar;
            ClientSize = new Size(700, 600);
            Controls.Add(btnBotardo);
            Controls.Add(btnMedio);
            Controls.Add(btnPro);
            Controls.Add(btnDios);
            Controls.Add(btnCancelar);
            Name = "FormNivelBot";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            ResumeLayout(false);
        }

        #endregion

        private BotonPixel btnBotardo;
        private BotonPixel btnMedio;
        private BotonPixel btnPro;
        private BotonPixel btnDios;
        private BotonPixel btnCancelar;
    }
}
