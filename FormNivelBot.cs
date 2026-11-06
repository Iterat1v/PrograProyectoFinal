namespace PrograProyectoFinal
{
    // Escoger qué tan bueno es el bot
    public partial class FormNivelBot : FormPixel
    {
        public int Nivel { get; private set; } = 0;

        public FormNivelBot()
        {
            InitializeComponent();
            ClientSize = new Size(700, 600);
            btnBotardo.Colores(Paleta.LilaClaro, Paleta.Crema2);
            btnMedio.Colores(Paleta.Agua, Paleta.Agua2);
            btnPro.Colores(Paleta.Rosa, Paleta.Rosa2);
            btnDios.Colores(Paleta.Oro, Paleta.Oro2);
            btnCancelar.Colores(Paleta.Crema, Paleta.Crema2);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pixel.Preparar(g); Pixel.Escalar(g);
            Pixel.R(g, 0, 0, AnchoD, AltoD, Paleta.Morado);
            Pixel.Panel(g, 0, 0, AnchoD, AltoD, Paleta.Morado2, Paleta.Negro);
            Pixel.Texto(g, "¿QUÉ TAN BUENO ES EL BOT?", AnchoD / 2, 44, 16, Paleta.Oro, StringAlignment.Center);

            string[] textos =
            {
                "Se distrae con el celular y regatea como sea.",
                "Juega normal: compra lo que le falta.",
                "Regatea bien y le tira Estafas al que va ganando.",
                "Es El Bro. Sabe cuánto trae cada cliente. Suerte."
            };
            for (int i = 0; i < 4; i++)
                Pixel.Texto(g, textos[i], 350, 150 + i * 104, 10, Paleta.Tenue, StringAlignment.Center);
            Pixel.Sprite(g, Recursos.Clientes[8], 600, 382, 3);
        }

        private void Elegir(int nivel)
        {
            Nivel = nivel;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnBotardo_Click(object sender, EventArgs e) => Elegir(Bot.BOTARDO);
        private void btnMedio_Click(object sender, EventArgs e) => Elegir(Bot.MEDIO);
        private void btnPro_Click(object sender, EventArgs e) => Elegir(Bot.PRO);
        private void btnDios_Click(object sender, EventArgs e) => Elegir(Bot.DIOS);
        private void btnCancelar_Click(object sender, EventArgs e) => Close();
    }
}
