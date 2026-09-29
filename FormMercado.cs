namespace PrograProyectoFinal
{
    // Mercado: los 30 autos en una cuadrícula de 5 x 6. Cada uno lo compra un solo jugador.
    public partial class FormMercado : FormPixel
    {
        private const int COLUMNAS = 6;

        private Concesionario jugador;
        private Auto[] catalogo;
        private double factor;      // 0.7 en Black Friday
        private int seleccionado = -1;
        private int colorElegido = 0;     // el color con el que te lo llevas
        private string aviso = "";

        internal FormMercado(Partida partida)
        {
            InitializeComponent();
            jugador = partida.JugadorActual;
            catalogo = partida.Catalogo;
            factor = partida.FactorMercado;
            if (factor < 1)
                Titulo = "MERCADO · ¡BLACK FRIDAY! 30 % MENOS";
            btnComprar.Colores(Paleta.Verde, Paleta.Verde2);
            btnCerrar.Colores(Paleta.Crema, Paleta.Crema2);
            btnComprar.Enabled = false;
        }

        // [fila, columna] -> índice del arreglo: fila * 6 + columna
        private Rectangle Casilla(int indice)
        {
            int fila = indice / COLUMNAS;
            int columna = indice % COLUMNAS;
            return new Rectangle(40 + columna * 204, 76 + fila * 110, 196, 102);
        }

        private Rectangle Muestra(int color) => new Rectangle(470 + color * 38, 632, 30, 30);

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            for (int i = 0; i < catalogo.Length; i++)
            {
                Auto auto = catalogo[i];
                Rectangle c = Casilla(i);

                if (i == seleccionado)
                {
                    Pixel.Panel(g, c.X, c.Y, c.Width, c.Height, Paleta.Oro);
                    Pixel.R(g, c.X + 10, c.Y + 10, c.Width - 20, c.Height - 20, Paleta.Crema);
                }
                else
                    Pixel.Panel(g, c.X, c.Y, c.Width, c.Height, auto.Disponible ? Paleta.Crema : Paleta.Crema2);

                int color = i == seleccionado ? colorElegido : auto.Color;
                Pixel.Sprite(g, Recursos.Autos[auto.Tipo, color], c.X + 50, c.Y + 8, 3);
                if (auto.Disponible && auto.Estado == Auto.CHOCADO)
                    Pixel.Texto(g, "CHOCADO", c.Right - 10, c.Y + 16, 7, Paleta.Rosa2, StringAlignment.Far);
                Pixel.Texto(g, auto.Nombre, c.X + 98, c.Y + 66, 9, Paleta.Noche, StringAlignment.Center);
                if (auto.Disponible)
                    Pixel.Texto(g, Pixel.Dinero(jugador.PrecioCompra(auto, factor)), c.X + 98, c.Y + 86, 9, Paleta.Verde2, StringAlignment.Center);
                else
                    Pixel.Texto(g, "VENDIDO", c.X + 98, c.Y + 86, 9, Paleta.Rosa2, StringAlignment.Center);
            }

            Pixel.Texto(g, $"Tienes {Pixel.Dinero(jugador.Dinero)}  ·  lote {jugador.NumAutos}/{Concesionario.MAX_AUTOS}", 40, 652, 13, Paleta.Oro);

            // Escoger el color con el que te lo llevas
            if (seleccionado != -1)
                for (int k = 0; k < Cliente.NombresColor.Length; k++)
                {
                    Rectangle m = Muestra(k);
                    Pixel.R(g, m.X - 3, m.Y - 3, m.Width + 6, m.Height + 6, k == colorElegido ? Paleta.Oro : Paleta.Negro);
                    Color[] pintura = { Paleta.Rojo, Paleta.Agua, Paleta.Oro, Paleta.Azul, Paleta.Rosa, Paleta.Crema, Paleta.Verde, Paleta.Gris };
                    Pixel.R(g, m.X, m.Y, m.Width, m.Height, pintura[k]);
                }
            if (aviso != "")
                Pixel.Texto(g, aviso, 40, 692, 11, Paleta.Rosa);
            else if (seleccionado != -1)
            {
                Auto auto = catalogo[seleccionado];
                string extra = auto.Estado == Auto.CHOCADO ? " · CHOCADO: repáralo en el TALLER" : "";
                Pixel.Texto(g, $"{auto.Nombre} {Cliente.NombresColor[colorElegido]} · {auto.NombreTipo} · lista {Pixel.Dinero(jugador.PrecioVenta(auto))}{extra}", 40, 692, 10, Paleta.Crema);
            }
            else if (jugador.Sobreprecio)
                Pixel.Texto(g, "¡Te manipularon el mercado! Todo está 20 % más caro este turno.", 40, 692, 11, Paleta.Rosa);
            else if (jugador.Descuento > 0)
                Pixel.Texto(g, $"Tienes {jugador.Descuento * 100:0} % de descuento en esta compra (ya aplicado).", 40, 692, 11, Paleta.Agua);
            else
                Pixel.Texto(g, "Escoge un auto. Lo vendes 30 % más caro de lo que te costó.", 40, 692, 11, Paleta.Tenue);
        }

        private int CasillaEn(Point punto)
        {
            for (int i = 0; i < catalogo.Length; i++)
                if (Casilla(i).Contains(punto))
                    return i;
            return -1;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (seleccionado != -1)
                for (int k = 0; k < Cliente.NombresColor.Length; k++)
                    if (Muestra(k).Contains(e.Location))
                    {
                        colorElegido = k;
                        Recursos.Sonar("menu");
                        Invalidate();
                        return;
                    }
            int i = CasillaEn(e.Location);
            if (i == -1) return;
            colorElegido = catalogo[i].Color;

            if (!catalogo[i].Disponible)
            {
                aviso = "Ese auto ya lo compró alguien.";
                seleccionado = -1;
            }
            else
            {
                aviso = "";
                seleccionado = i;
            }
            btnComprar.Enabled = seleccionado != -1;
            Invalidate();
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (CasillaEn(e.Location) == seleccionado && seleccionado != -1)
                Comprar();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int i = CasillaEn(e.Location);
            Cursor = i != -1 && catalogo[i].Disponible ? Cursors.Hand : Cursors.Default;
        }

        private void btnComprar_Click(object sender, EventArgs e)
        {
            Comprar();
        }

        private void Comprar()
        {
            if (seleccionado == -1) return;
            if (catalogo[seleccionado].Disponible)
                catalogo[seleccionado].Color = colorElegido;
            string error = jugador.Comprar(catalogo[seleccionado], factor);
            if (error != null)
            {
                aviso = error;
                Recursos.Sonar("error");
            }
            else
            {
                Recursos.Sonar("compra");
                aviso = "";
                seleccionado = -1;
                btnComprar.Enabled = false;
            }
            Invalidate();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
