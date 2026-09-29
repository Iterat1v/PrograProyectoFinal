namespace PrograProyectoFinal
{
    partial class FormDraft
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
            components = new System.ComponentModel.Container();
            btnIniciar = new BotonPixel();
            timerVoltear = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            //
            // btnIniciar
            //
            btnIniciar.Location = new Point(800, 636);
            btnIniciar.Name = "btnIniciar";
            btnIniciar.Size = new Size(452, 70);
            btnIniciar.TabIndex = 0;
            btnIniciar.Text = "INICIAR PARTIDA";
            btnIniciar.Click += btnIniciar_Click;
            //
            // timerVoltear
            //
            timerVoltear.Interval = 15;
            timerVoltear.Tick += timerVoltear_Tick;
            //
            // FormDraft
            //
            AcceptButton = btnIniciar;
            ClientSize = new Size(1280, 720);
            Controls.Add(btnIniciar);
            Name = "FormDraft";
            Titulo = "DRAFT DE CARTAS";
            ResumeLayout(false);
        }

        #endregion

        private BotonPixel btnIniciar;
        private System.Windows.Forms.Timer timerVoltear;
    }
}
