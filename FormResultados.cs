namespace PrograProyectoFinal
{
    // Pantalla 4: el podio con el ranking final (ordenado con burbuja)
    public partial class FormResultados : FormPixel
    {
        private Concesionario[] ranking;

        internal FormResultados(Partida partida)
        {
            InitializeComponent();
            ranking = partida.Ranking();
            btnNueva.Colores(Paleta.Verde, Paleta.Verde2);
            btnSalir.Colores(Paleta.Rosa, Paleta.Rosa2);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pixel.Preparar(g);

            Pixel.R(g, 0, 0, Pixel.ANCHO, Pixel.ALTO, Paleta.Noche);
            Pixel.Estrellas(g, 50, 3);

            // confeti
            Color[] colores = { Paleta.Rosa, Paleta.Agua, Paleta.Oro, Paleta.Verde };
            double s = 11;
            for (int i = 0; i < 60; i++)
            {
                s = (s * 9301 + 49297) % 233280; int x = (int)(s / 233280 * 640) * 2;
                s = (s * 9301 + 49297) % 233280; int y = (int)(s / 233280 * 240) * 2;
                Pixel.R(g, x, y, 8, 8, colores[i % 4]);
            }

            Pixel.SombraTexto(g, $"¡GANÓ {ranking[0].Jugador.ToUpper()}!", 640, 80, 64, Paleta.Oro, Paleta.Rosa);

            // podio: {lugar, x, alto}
            int[,] podio = { { 1, 380, 220 }, { 0, 540, 300 }, { 2, 700, 160 } };
            Color[] bloque = { Paleta.Oro, Paleta.Crema2, Paleta.Bronce };
            for (int p = 0; p < 3; p++)
            {
                int lugar = podio[p, 0], x = podio[p, 1], h = podio[p, 2];
                if (lugar >= ranking.Length) continue;
                int y = 600 - h;
                Concesionario j = ranking[lugar];

                Pixel.R(g, x, y, 160, h, Paleta.Negro);
                Pixel.R(g, x + 6, y + 6, 148, h - 6, bloque[lugar]);
                Pixel.Texto(g, $"{lugar + 1}", x + 80, y + 52, 24, Paleta.Noche, StringAlignment.Center);
                Pixel.Sprite(g, Recursos.Vendedores[j.Vendedor], x + 16, y - 132, 8);
                Pixel.Texto(g, j.Jugador, x + 80, y + 104, 12, Paleta.Noche, StringAlignment.Center);
                Pixel.Texto(g, Pixel.Dinero(j.Patrimonio()), x + 80, y + 140, 11, Paleta.Noche, StringAlignment.Center);
            }
            if (ranking.Length == 4)
                Pixel.Texto(g, $"4. {ranking[3].Jugador}  {Pixel.Dinero(ranking[3].Patrimonio())}", 1240, 576, 11, Paleta.Tenue, StringAlignment.Far);

            Pixel.R(g, 0, 600, Pixel.ANCHO, 120, Paleta.Piso);
            Pixel.R(g, 0, 600, Pixel.ANCHO, 6, Paleta.Negro);
        }

        private void btnNueva_Click(object sender, EventArgs e)
        {
            IrA(new FormRegistro());
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
