namespace PrograProyectoFinal
{
    // Menú de pausa del tablero (se abre con MENÚ o con Esc)
    public partial class FormMenu : FormPixel
    {
        public const int CONTINUAR = 0;
        public const int REVANCHA = 1;
        public const int RENDIRSE = 2;
        public const int TITULO = 3;

        public int Eleccion { get; private set; } = CONTINUAR;

        public FormMenu()
        {
            InitializeComponent();
            ClientSize = new Size(560, 660);
            btnContinuar.Colores(Paleta.Verde, Paleta.Verde2);
            btnRevancha.Colores(Paleta.Oro, Paleta.Oro2);
            btnRendirse.Colores(Paleta.Rosa, Paleta.Rosa2);
            btnMusica.Colores(Paleta.LilaClaro, Paleta.Crema2);
            btnSonidos.Colores(Paleta.LilaClaro, Paleta.Crema2);
            btnPasar.Colores(Paleta.LilaClaro, Paleta.Crema2);
            btnTitulo.Colores(Paleta.Crema, Paleta.Crema2);
            ActualizarTextos();
            Recursos.Sonar("menu");
        }

        private void ActualizarTextos()
        {
            btnMusica.Text = $"MÚSICA: {(Recursos.MusicaActiva ? "SÍ" : "NO")}";
            btnSonidos.Text = $"SONIDOS: {(Recursos.SonidosActivos ? "SÍ" : "NO")}";
            btnPasar.Text = $"PASAR SOLO AL VENDER: {(Recursos.PasarAlVender ? "SÍ" : "NO")}";
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pixel.Preparar(g); Pixel.Escalar(g);
            Pixel.R(g, 0, 0, AnchoD, AltoD, Paleta.Morado);
            Pixel.Panel(g, 0, 0, AnchoD, AltoD, Paleta.Morado2, Paleta.Negro);
            Pixel.SombraTexto(g, "PAUSA", AnchoD / 2, 56, 48, Paleta.Oro, Paleta.Rosa);
        }

        private void Elegir(int opcion)
        {
            Eleccion = opcion;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnContinuar_Click(object sender, EventArgs e) => Elegir(CONTINUAR);
        private void btnRevancha_Click(object sender, EventArgs e) => Elegir(REVANCHA);
        private void btnRendirse_Click(object sender, EventArgs e) => Elegir(RENDIRSE);
        private void btnTitulo_Click(object sender, EventArgs e) => Elegir(TITULO);

        private void btnMusica_Click(object sender, EventArgs e)
        {
            Recursos.MusicaActiva = !Recursos.MusicaActiva;
            Recursos.Musica(Recursos.MusicaActiva);
            ActualizarTextos();
        }

        private void btnSonidos_Click(object sender, EventArgs e)
        {
            Recursos.SonidosActivos = !Recursos.SonidosActivos;
            ActualizarTextos();
            Recursos.Sonar("menu");
        }

        private void btnPasar_Click(object sender, EventArgs e)
        {
            Recursos.PasarAlVender = !Recursos.PasarAlVender;
            ActualizarTextos();
        }
    }
}
