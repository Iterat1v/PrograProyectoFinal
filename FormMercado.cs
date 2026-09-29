namespace PrograProyectoFinal
{
    // Mercado: los 30 autos en una cuadrícula de 5 x 6. Cada uno lo compra un solo jugador.
    public partial class FormMercado : FormPixel
    {
        private const int COLUMNAS = 6;

        private Concesionario jugador;
        private Auto[] catalogo;
        private int seleccionado = -1;
        private string aviso = "";

        internal FormMercado(Concesionario jugador, Auto[] catalogo)
        {
            InitializeComponent();
            this.jugador = jugador;
            this.catalogo = catalogo;
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

                Pixel.Sprite(g, Recursos.Autos[auto.Tipo, auto.Color], c.X + 50, c.Y + 8, 3);
                Pixel.Texto(g, auto.Nombre, c.X + 98, c.Y + 66, 9, Paleta.Noche, StringAlignment.Center);
                if (auto.Disponible)
                    Pixel.Texto(g, Pixel.Dinero(jugador.PrecioCompra(auto)), c.X + 98, c.Y + 86, 9, Paleta.Verde2, StringAlignment.Center);
                else
                    Pixel.Texto(g, "VENDIDO", c.X + 98, c.Y + 86, 9, Paleta.Rosa2, StringAlignment.Center);
            }

            Pixel.Texto(g, $"Tienes {Pixel.Dinero(jugador.Dinero)}  ·  lote {jugador.NumAutos}/{Concesionario.MAX_AUTOS}", 40, 652, 13, Paleta.Oro);
            if (aviso != "")
                Pixel.Texto(g, aviso, 40, 692, 11, Paleta.Rosa);
            else if (seleccionado != -1)
            {
                Auto auto = catalogo[seleccionado];
                Pixel.Texto(g, $"{auto.Nombre} · {auto.NombreTipo} · lo vendes en {Pixel.Dinero(jugador.PrecioVenta(auto))}", 40, 692, 11, Paleta.Crema);
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
            int i = CasillaEn(e.Location);
            if (i == -1) return;

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
            string error = jugador.Comprar(catalogo[seleccionado]);
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
