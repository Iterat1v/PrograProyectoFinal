namespace PrograProyectoFinal
{
    // Pantalla 0: el lote de noche con los autos estacionados
    public partial class FormTitulo : FormPixel
    {
        public FormTitulo()
        {
            InitializeComponent();
            btnJugar.Colores(Paleta.Verde, Paleta.Verde2);
            btnComoSeJuega.Colores(Paleta.Crema, Paleta.Crema2);
            btnSalir.Colores(Paleta.Rosa, Paleta.Rosa2);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pixel.Preparar(g);

            // cielo en tres franjas
            for (int y = 0; y < 400; y += 16)
            {
                Color franja = y < 120 ? ColorTranslator.FromHtml("#140722") : y < 240 ? Paleta.Noche : Paleta.Showroom;
                Pixel.R(g, 0, y, Pixel.ANCHO, 16, franja);
            }
            Pixel.Estrellas(g, 70, 7);

            // edificio con sus ventanas
            Pixel.R(g, 300, 140, 680, 220, Paleta.Negro);
            Pixel.R(g, 312, 152, 656, 196, Paleta.Morado2);
            for (int i = 0; i < 6; i++)
                Pixel.R(g, 344 + i * 104, 240, 68, 80, i % 2 == 1 ? Paleta.Cielo : Paleta.Cielo2);

            Pixel.SombraTexto(g, "CONCESIONARIO", 640, 104, 80, Paleta.Oro, Paleta.Rosa);
            Pixel.Texto(g, "EQUIPO 4 · LOTE DE AUTOS", 640, 190, 13, Paleta.Agua, StringAlignment.Center);

            // piso del estacionamiento
            Pixel.R(g, 0, 360, Pixel.ANCHO, 360, Paleta.Piso);
            for (int x = 40; x < Pixel.ANCHO; x += 220)
                Pixel.R(g, x, 430, 8, 120, Paleta.Oro2);
            Pixel.R(g, 0, 360, Pixel.ANCHO, 8, Paleta.Negro);

            Pixel.Sprite(g, Recursos.Autos[0, 0], 80, 440, 6);    // sedán rojo
            Pixel.Sprite(g, Recursos.Autos[1, 1], 500, 444, 6);   // vocho aqua
            Pixel.Sprite(g, Recursos.Autos[2, 2], 900, 440, 6);   // pickup amarilla
        }

        private void btnJugar_Click(object sender, EventArgs e)
        {
            IrA(new FormRegistro());
        }

        private void btnComoSeJuega_Click(object sender, EventArgs e)
        {
            using (FormReglas reglas = new FormReglas())
                reglas.ShowDialog(this);
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
