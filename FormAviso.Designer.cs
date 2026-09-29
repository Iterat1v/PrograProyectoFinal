namespace PrograProyectoFinal
{
    partial class FormAviso
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
            btnSi = new BotonPixel();
            btnNo = new BotonPixel();
            SuspendLayout();
            //
            // btnSi
            //
            btnSi.Location = new Point(100, 296);
            btnSi.Name = "btnSi";
            btnSi.Size = new Size(204, 76);
            btnSi.TabIndex = 0;
            btnSi.Text = "¡VA!";
            btnSi.Click += btnSi_Click;
            //
            // btnNo
            //
            btnNo.Location = new Point(336, 296);
            btnNo.Name = "btnNo";
            btnNo.Size = new Size(204, 76);
            btnNo.TabIndex = 1;
            btnNo.Text = "NO";
            btnNo.Click += btnNo_Click;
            //
            // FormAviso
            //
            AcceptButton = btnSi;
            CancelButton = btnNo;
            ClientSize = new Size(640, 400);
            Controls.Add(btnSi);
            Controls.Add(btnNo);
            Name = "FormAviso";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            ResumeLayout(false);
        }

        #endregion

        private BotonPixel btnSi;
        private BotonPixel btnNo;
    }
}
