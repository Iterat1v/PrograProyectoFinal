namespace PrograProyectoFinal
{
    // Ventanita de aviso con el estilo del juego (en lugar de MessageBox):
    // eventos del día, estafas, el bro, y preguntas de sí / no.
    public partial class FormAviso : FormPixel
    {
        private string encabezado;
        private string texto;
        private Color color;
        private Image sprite;

        private FormAviso(string encabezado, string texto, Color color, Image sprite, bool pregunta)
        {
            InitializeComponent();
            ClientSize = new Size(640, 400);
            this.encabezado = encabezado;
            this.texto = texto;
            this.color = color;
            this.sprite = sprite;

            btnSi.Colores(Paleta.Verde, Paleta.Verde2);
            btnNo.Colores(Paleta.Crema, Paleta.Crema2);
            btnNo.Visible = pregunta;
            if (pregunta)
                btnSi.Text = "SÍ";
            else
                btnSi.Left = (640 - btnSi.Width) / 2;
        }

        // Solo avisa
        public static void Mostrar(IWin32Window dueno, string encabezado, string texto, Color color, Image sprite = null)
        {
            using (FormAviso aviso = new FormAviso(encabezado, texto, color, sprite, false))
                aviso.ShowDialog(dueno);
        }

        // Pregunta y regresa true si dijo que sí
        public static bool Preguntar(IWin32Window dueno, string encabezado, string texto, Color color, Image sprite = null)
        {
            using (FormAviso aviso = new FormAviso(encabezado, texto, color, sprite, true))
                return aviso.ShowDialog(dueno) == DialogResult.OK;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pixel.Preparar(g);
            Pixel.R(g, 0, 0, Width, Height, Paleta.Morado);
            Pixel.Panel(g, 0, 0, Width, Height, Paleta.Crema);
            Pixel.R(g, 14, 14, Width - 28, 64, color);
            Pixel.Texto(g, encabezado, Width / 2, 46, 17, Paleta.Noche, StringAlignment.Center);

            int x = 40;
            if (sprite != null)
            {
                Pixel.R(g, 36, 106, 136, 136, Paleta.Morado3);
                Pixel.Sprite(g, sprite, 40, 110, 128f / Math.Max(sprite.Width, sprite.Height));
                x = 196;
            }
            Pixel.Parrafo(g, texto, new Rectangle(x, 104, Width - x - 36, 180), 13, Paleta.Noche);
        }

        private void btnSi_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
