using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace PrograProyectoFinal
{
    partial class FormDraft : Form
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

        #region Código generado por el Diseñador de Windows Forms

        private void InitializeComponent()
        {
            mainLayout = new TableLayoutPanel();
            lbTitulo = new Label();
            lbTurno = new Label();
            panelCuadricula = new TableLayoutPanel();
            btnContinuarTablero = new Button();

            mainLayout.SuspendLayout();
            SuspendLayout();

            // 
            // mainLayout
            // 
            mainLayout.ColumnCount = 1;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayout.Controls.Add(lbTitulo, 0, 0);
            mainLayout.Controls.Add(lbTurno, 0, 1);
            mainLayout.Controls.Add(panelCuadricula, 0, 2);
            mainLayout.Controls.Add(btnContinuarTablero, 0, 3);
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Location = new System.Drawing.Point(0, 0);
            mainLayout.Name = "mainLayout";
            mainLayout.Padding = new Padding(20);
            mainLayout.RowCount = 4;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            mainLayout.Size = new System.Drawing.Size(800, 600);
            mainLayout.TabIndex = 0;

            // 
            // lbTitulo
            // 
            lbTitulo.Dock = DockStyle.Fill;
            lbTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            lbTitulo.ForeColor = System.Drawing.Color.DarkSlateBlue;
            lbTitulo.Location = new System.Drawing.Point(23, 20);
            lbTitulo.Name = "lbTitulo";
            lbTitulo.Size = new System.Drawing.Size(754, 50);
            lbTitulo.TabIndex = 0;
            lbTitulo.Text = "FASE DE DRAFT: SELECCIÓN DE CARTAS";
            lbTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // lbTurno
            // 
            lbTurno.Dock = DockStyle.Fill;
            lbTurno.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            lbTurno.ForeColor = System.Drawing.Color.DimGray;
            lbTurno.Location = new System.Drawing.Point(23, 70);
            lbTurno.Name = "lbTurno";
            lbTurno.Size = new System.Drawing.Size(754, 40);
            lbTurno.TabIndex = 1;
            lbTurno.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // panelCuadricula
            // 
            panelCuadricula.ColumnCount = 4;
            panelCuadricula.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            panelCuadricula.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            panelCuadricula.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            panelCuadricula.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            panelCuadricula.Dock = DockStyle.Fill;
            panelCuadricula.Location = new System.Drawing.Point(23, 113);
            panelCuadricula.Name = "panelCuadricula";
            panelCuadricula.Padding = new Padding(10);
            panelCuadricula.RowCount = 4;
            panelCuadricula.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            panelCuadricula.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            panelCuadricula.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            panelCuadricula.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            panelCuadricula.Size = new System.Drawing.Size(754, 404);
            panelCuadricula.TabIndex = 2;

            // 
            // btnContinuarTablero
            // 
            btnContinuarTablero.Anchor = AnchorStyles.None;
            btnContinuarTablero.BackColor = System.Drawing.Color.DarkSeaGreen;
            btnContinuarTablero.Enabled = false;
            btnContinuarTablero.FlatStyle = FlatStyle.Flat;
            btnContinuarTablero.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            btnContinuarTablero.Location = new System.Drawing.Point(250, 527);
            btnContinuarTablero.Name = "btnContinuarTablero";
            btnContinuarTablero.Size = new System.Drawing.Size(300, 45);
            btnContinuarTablero.TabIndex = 3;
            btnContinuarTablero.Text = "Iniciar Partida (Ir al Tablero) ➔";
            btnContinuarTablero.UseVisualStyleBackColor = false;
            btnContinuarTablero.Click += new System.EventHandler(BtnContinuarTablero_Click);

            // 
            // FormDraft
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(245, 245, 250);
            ClientSize = new System.Drawing.Size(800, 600);
            Controls.Add(mainLayout);
            Name = "FormDraft";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Fase de Draft - Selección de Cartas";
            WindowState = FormWindowState.Maximized;

            mainLayout.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel mainLayout;
        private Label lbTitulo;
        private Label lbTurno;
        private TableLayoutPanel panelCuadricula;
        private Button btnContinuarTablero;
    }
}