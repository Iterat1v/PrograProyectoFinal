namespace PrograProyectoFinal
{
    partial class FormNegociar
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
            btnMenos10 = new BotonPixel();
            btnMenos1 = new BotonPixel();
            btnMas1 = new BotonPixel();
            btnMas10 = new BotonPixel();
            btnOfrecer = new BotonPixel();
            btnSuOferta = new BotonPixel();
            btnYaNo = new BotonPixel();
            SuspendLayout();
            //
            // btnMenos10
            //
            btnMenos10.Location = new Point(150, 350);
            btnMenos10.Name = "btnMenos10";
            btnMenos10.Size = new Size(110, 64);
            btnMenos10.TabIndex = 0;
            btnMenos10.TamLetra = 11F;
            btnMenos10.Text = "-10K";
            btnMenos10.Click += btnMenos10_Click;
            //
            // btnMenos1
            //
            btnMenos1.Location = new Point(270, 350);
            btnMenos1.Name = "btnMenos1";
            btnMenos1.Size = new Size(96, 64);
            btnMenos1.TabIndex = 1;
            btnMenos1.TamLetra = 11F;
            btnMenos1.Text = "-1K";
            btnMenos1.Click += btnMenos1_Click;
            //
            // btnMas1
            //
            btnMas1.Location = new Point(634, 350);
            btnMas1.Name = "btnMas1";
            btnMas1.Size = new Size(96, 64);
            btnMas1.TabIndex = 2;
            btnMas1.TamLetra = 11F;
            btnMas1.Text = "+1K";
            btnMas1.Click += btnMas1_Click;
            //
            // btnMas10
            //
            btnMas10.Location = new Point(740, 350);
            btnMas10.Name = "btnMas10";
            btnMas10.Size = new Size(110, 64);
            btnMas10.TabIndex = 3;
            btnMas10.TamLetra = 11F;
            btnMas10.Text = "+10K";
            btnMas10.Click += btnMas10_Click;
            //
            // btnOfrecer
            //
            btnOfrecer.Location = new Point(40, 470);
            btnOfrecer.Name = "btnOfrecer";
            btnOfrecer.Size = new Size(300, 76);
            btnOfrecer.TabIndex = 4;
            btnOfrecer.Text = "OFRECER";
            btnOfrecer.Click += btnOfrecer_Click;
            //
            // btnSuOferta
            //
            btnSuOferta.Location = new Point(356, 470);
            btnSuOferta.Name = "btnSuOferta";
            btnSuOferta.Size = new Size(340, 76);
            btnSuOferta.TabIndex = 5;
            btnSuOferta.TamLetra = 12F;
            btnSuOferta.Text = "ACEPTAR SU OFERTA";
            btnSuOferta.Click += btnSuOferta_Click;
            //
            // btnYaNo
            //
            btnYaNo.Location = new Point(712, 470);
            btnYaNo.Name = "btnYaNo";
            btnYaNo.Size = new Size(248, 76);
            btnYaNo.TabIndex = 6;
            btnYaNo.Text = "YA NO";
            btnYaNo.Click += btnYaNo_Click;
            //
            // FormNegociar
            //
            AcceptButton = btnOfrecer;
            CancelButton = btnYaNo;
            ClientSize = new Size(1000, 600);
            Controls.Add(btnMenos10);
            Controls.Add(btnMenos1);
            Controls.Add(btnMas1);
            Controls.Add(btnMas10);
            Controls.Add(btnOfrecer);
            Controls.Add(btnSuOferta);
            Controls.Add(btnYaNo);
            Name = "FormNegociar";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            ResumeLayout(false);
        }

        #endregion

        private BotonPixel btnMenos10;
        private BotonPixel btnMenos1;
        private BotonPixel btnMas1;
        private BotonPixel btnMas10;
        private BotonPixel btnOfrecer;
        private BotonPixel btnSuOferta;
        private BotonPixel btnYaNo;
    }
}
