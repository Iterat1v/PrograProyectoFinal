using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace PrograProyectoFinal
{
    public partial class Tablero : Form
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
            lblJugadorTurno = new Label();
            lblTitulo = new Label();
            lbltxt2 = new Label();
            chkListAutos = new CheckedListBox();
            lbltxt1 = new Label();
            lstSobrePromedio = new ListBox();
            lblPromedio = new Label();
            lblConteo = new Label();
            btnCalcular = new Button();

            SuspendLayout();

            // 
            // lblJugadorTurno
            // 
            lblJugadorTurno.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            lblJugadorTurno.Location = new System.Drawing.Point(20, 10);
            lblJugadorTurno.Name = "lblJugadorTurno";
            lblJugadorTurno.Size = new System.Drawing.Size(550, 25);
            lblJugadorTurno.TabIndex = 0;
            lblJugadorTurno.Text = "Jugador en turno: -";

            // 
            // lblTitulo
            // 
            lblTitulo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Italic);
            lblTitulo.Location = new System.Drawing.Point(20, 38);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new System.Drawing.Size(550, 22);
            lblTitulo.TabIndex = 1;
            lblTitulo.Text = "Tablero Principal de Comercio y Decisiones";

            // 
            // lbltxt2
            // 
            lbltxt2.Location = new System.Drawing.Point(20, 70);
            lbltxt2.Name = "lbltxt2";
            lbltxt2.Size = new System.Drawing.Size(250, 20);
            lbltxt2.TabIndex = 2;
            lbltxt2.Text = "Catálogo / Autos en Posesión:";

            // 
            // chkListAutos
            // 
            chkListAutos.FormattingEnabled = true;
            chkListAutos.Location = new System.Drawing.Point(20, 95);
            chkListAutos.Name = "chkListAutos";
            chkListAutos.Size = new System.Drawing.Size(260, 180);
            chkListAutos.TabIndex = 3;

            // 
            // lbltxt1
            // 
            lbltxt1.Location = new System.Drawing.Point(300, 70);
            lbltxt1.Name = "lbltxt1";
            lbltxt1.Size = new System.Drawing.Size(260, 20);
            lbltxt1.TabIndex = 4;
            lbltxt1.Text = "Modelos con mayor comisión:";

            // 
            // lstSobrePromedio
            // 
            lstSobrePromedio.FormattingEnabled = true;
            lstSobrePromedio.Location = new System.Drawing.Point(300, 95);
            lstSobrePromedio.Name = "lstSobrePromedio";
            lstSobrePromedio.Size = new System.Drawing.Size(260, 180);
            lstSobrePromedio.TabIndex = 5;

            // 
            // lblPromedio
            // 
            lblPromedio.Location = new System.Drawing.Point(20, 290);
            lblPromedio.Name = "lblPromedio";
            lblPromedio.Size = new System.Drawing.Size(540, 20);
            lblPromedio.TabIndex = 6;
            lblPromedio.Text = "Precio promedio (0 autos): $0.00";

            // 
            // lblConteo
            // 
            lblConteo.Location = new System.Drawing.Point(20, 315);
            lblConteo.Name = "lblConteo";
            lblConteo.Size = new System.Drawing.Size(540, 20);
            lblConteo.TabIndex = 7;
            lblConteo.Text = "Autos arriba del promedio y con mayor comisión: 0";

            // 
            // btnCalcular
            // 
            btnCalcular.Location = new System.Drawing.Point(20, 350);
            btnCalcular.Name = "btnCalcular";
            btnCalcular.Size = new System.Drawing.Size(120, 35);
            btnCalcular.TabIndex = 8;
            btnCalcular.Text = "Calcular";
            btnCalcular.UseVisualStyleBackColor = true;
            btnCalcular.Click += new System.EventHandler(btnCalcular_Click);

            // 
            // Tablero
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(580, 405);
            Controls.Add(lblJugadorTurno);
            Controls.Add(lblTitulo);
            Controls.Add(lbltxt2);
            Controls.Add(chkListAutos);
            Controls.Add(lbltxt1);
            Controls.Add(lstSobrePromedio);
            Controls.Add(lblPromedio);
            Controls.Add(lblConteo);
            Controls.Add(btnCalcular);

            Name = "Tablero";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Tablero Principal de Juego";
            Load += new System.EventHandler(Tablero_Load);

            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblJugadorTurno;
        private Button btnCalcular;
        private Label lblPromedio;
        private Label lblConteo;
        private CheckedListBox chkListAutos;
        private ListBox lstSobrePromedio;
        private Label lbltxt1;
        private Label lbltxt2;
        private Label lblTitulo;
    }
}