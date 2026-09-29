namespace PrograProyectoFinal
{
    // "Las cartas": los 3 tipos y las 25 de la baraja
    public partial class FormCartas : FormPixel
    {
        private Baraja baraja = new Baraja();
        private int pagina = 0;       // 0 = los tipos, 1 = las 25 cartas

        public FormCartas()
        {
            InitializeComponent();
            btnVolver.Colores(Paleta.Verde, Paleta.Verde2);
            btnLista.Colores(Paleta.Crema, Paleta.Crema2);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            if (pagina == 0)
                DibujarTipos(g);
            else
                DibujarLista(g);
        }

        // Igual que el mockup: boca abajo + un ejemplo de cada tipo
        private void DibujarTipos(Graphics g)
        {
            Pixel.CartaBocaAbajo(g, 80, 120, 240, 340);
            Pixel.Texto(g, "Boca abajo", 200, 500, 13, Paleta.Crema, StringAlignment.Center);

            string[] nombres = { "REGATEO", "ESTAFA", "ASEGURADORA" };
            Color[] colores = { Paleta.Oro, Paleta.Rosa, Paleta.Agua };
            string[,] textos = { { "Te dan ventaja", "a ti." }, { "Afectan a", "un rival." }, { "Te protegen", "de una estafa." } };
            for (int i = 0; i < 3; i++)
            {
                int x = 380 + i * 292;
                Pixel.Panel(g, x, 120, 260, 340, Paleta.Crema);
                Pixel.R(g, x + 8, 128, 244, 40, colores[i]);
                Pixel.Texto(g, nombres[i], x + 130, 148, 12, Paleta.Noche, StringAlignment.Center);
                Pixel.Sprite(g, Recursos.Iconos[i], x + 58, 192, 12);
                Pixel.Texto(g, textos[i, 0], x + 130, 392, 11, Paleta.Noche, StringAlignment.Center);
                Pixel.Texto(g, textos[i, 1], x + 130, 424, 11, Paleta.Noche, StringAlignment.Center);
            }
            Pixel.Texto(g, "Cada tipo tiene su color y su dibujo. De las 25 salen 16 al azar para el draft.", 640, 560, 13, Paleta.Crema, StringAlignment.Center);
            Pixel.Texto(g, "Regateo te ayuda, Estafa ataca a un rival y Aseguradora te defiende.", 640, 600, 12, Paleta.Tenue, StringAlignment.Center);
        }

        // Las 25 cartas en 3 columnas (una por tipo)
        private void DibujarLista(Graphics g)
        {
            string[] tipos = { "REGATEO", "ESTAFA", "ASEGURADORA" };
            for (int t = 0; t < 3; t++)
            {
                int x = 40 + t * 404;
                int y = 84;
                Pixel.Panel(g, x, y, 388, 536, Paleta.Crema);
                string anterior = "";
                int fila = 0;
                for (int i = 0; i < baraja.Cantidad; i++)
                {
                    PowerUp carta = baraja.ObtenerCarta(i);
                    if (carta.Tipo != tipos[t] || carta.Nombre == anterior) continue;
                    anterior = carta.Nombre;
                    int cuantas = 0;
                    for (int j = 0; j < baraja.Cantidad; j++)
                        if (baraja.ObtenerCarta(j).Nombre == carta.Nombre) cuantas++;

                    int yy = y + 16 + fila * 64;
                    Pixel.Sprite(g, Recursos.Iconos[carta.Icono], x + 20, yy + 6, 3);
                    string nombre = cuantas > 1 ? $"{carta.Nombre} (x{cuantas})" : carta.Nombre;
                    Pixel.Texto(g, nombre, x + 70, yy + 10, 10, Paleta.Noche);
                    Pixel.Parrafo(g, carta.Descripcion, new Rectangle(x + 70, yy + 22, 310, 44), 7, Paleta.Morado2);
                    fila++;
                }
            }
        }

        private void btnLista_Click(object sender, EventArgs e)
        {
            pagina = 1 - pagina;
            btnLista.Text = pagina == 0 ? "VER LAS 25" : "VER LOS TIPOS";
            Titulo = pagina == 0 ? "LAS CARTAS · 25 EN LA BARAJA" : "LAS 25 ABILICARDS";
            Invalidate();
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
