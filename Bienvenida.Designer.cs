using System.Drawing;
using System.Windows.Forms;

namespace PrograProyectoFinal
{
    partial class FormSaludo
    {
        private Label lbTntroName;
        private Label lbContador;
        private Label lbCharCount;
        private TextBox txtbName;
        private Button btnRegistrarOtro;
        private Button btnAceptarFinal;
        private Button btnLimpiar;

        private void InitializeComponent()
        {
            lbTntroName = new Label();
            lbContador = new Label();
            lbCharCount = new Label();
            txtbName = new TextBox();
            btnRegistrarOtro = new Button();
            btnAceptarFinal = new Button();
            btnLimpiar = new Button();
            SuspendLayout();
            // 
            // lbTntroName
            // 
            lbTntroName.Location = new Point(20, 20);
            lbTntroName.Name = "lbTntroName";
            lbTntroName.Size = new Size(259, 25);
            lbTntroName.TabIndex = 0;
            lbTntroName.Text = "Ingresa tu nombre de usuario:";
            // 
            // lbContador
            // 
            lbContador.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lbContador.Location = new Point(366, 20);
            lbContador.Name = "lbContador";
            lbContador.Size = new Size(220, 25);
            lbContador.TabIndex = 1;
            lbContador.Text = "Registrando Jugador 1 (máximo 4)";
            lbContador.TextAlign = ContentAlignment.TopRight;
            // 
            // lbCharCount
            // 
            lbCharCount.ForeColor = Color.Gray;
            lbCharCount.Location = new Point(386, 85);
            lbCharCount.Name = "lbCharCount";
            lbCharCount.Size = new Size(200, 20);
            lbCharCount.TabIndex = 3;
            lbCharCount.Text = "0 / 200 caracteres";
            lbCharCount.TextAlign = ContentAlignment.TopRight;
            // 
            // txtbName
            // 
            txtbName.Location = new Point(20, 55);
            txtbName.MaxLength = 200;
            txtbName.Name = "txtbName";
            txtbName.Size = new Size(566, 27);
            txtbName.TabIndex = 2;
            txtbName.TextChanged += txtbName_TextChanged;
            // 
            // btnRegistrarOtro
            // 
            btnRegistrarOtro.Location = new Point(20, 120);
            btnRegistrarOtro.Name = "btnRegistrarOtro";
            btnRegistrarOtro.Size = new Size(200, 50);
            btnRegistrarOtro.TabIndex = 4;
            btnRegistrarOtro.Text = "Aceptar y registrar otro jugador";
            btnRegistrarOtro.UseVisualStyleBackColor = true;
            btnRegistrarOtro.Click += btnRegistrarOtro_Click;
            // 
            // btnAceptarFinal
            // 
            btnAceptarFinal.Location = new Point(245, 120);
            btnAceptarFinal.Name = "btnAceptarFinal";
            btnAceptarFinal.Size = new Size(160, 50);
            btnAceptarFinal.TabIndex = 5;
            btnAceptarFinal.Text = "Aceptar y finalizar registro";
            btnAceptarFinal.UseVisualStyleBackColor = true;
            btnAceptarFinal.Click += btnAceptarFinal2_Click;
            // 
            // btnLimpiar
            // 
            btnLimpiar.Location = new Point(435, 120);
            btnLimpiar.Name = "btnLimpiar";
            btnLimpiar.Size = new Size(151, 50);
            btnLimpiar.TabIndex = 6;
            btnLimpiar.Text = "Limpiar";
            btnLimpiar.UseVisualStyleBackColor = true;
            btnLimpiar.Click += btnLimpiar_Click;
            // 
            // FormSaludo
            // 
            ClientSize = new Size(598, 256);
            Controls.Add(lbTntroName);
            Controls.Add(lbContador);
            Controls.Add(txtbName);
            Controls.Add(lbCharCount);
            Controls.Add(btnRegistrarOtro);
            Controls.Add(btnAceptarFinal);
            Controls.Add(btnLimpiar);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "FormSaludo";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "¡BIENVENIDO - REGISTRO!";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}