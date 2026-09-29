namespace PrograProyectoFinal
{
    // Pantalla 3: tu lote a la izquierda, el cliente al centro y el ranking a la derecha
    public partial class FormTablero : FormPixel
    {
        private const int FILAS = 3;               // autos que caben a la vista en MI LOTE

        private Partida partida;
        private int seleccionado = -1;             // índice del auto escogido en el lote
        private int pagina = 0;
        private string dialogo;                    // lo que dice el cliente en el globo

        internal FormTablero(Partida partida)
        {
            InitializeComponent();
            this.partida = partida;
            btnMercado.Colores(Paleta.Oro, Paleta.Oro2);
            btnVender.Colores(Paleta.Verde, Paleta.Verde2);
            btnUsarCarta.Colores(Paleta.Rosa, Paleta.Rosa2);
            btnPasar.Colores(Paleta.Crema, Paleta.Crema2);
            btnAnterior.Colores(Paleta.LilaClaro, Paleta.Crema2);
            btnSiguiente.Colores(Paleta.LilaClaro, Paleta.Crema2);

            partida.NuevoCliente();
            NuevoTurno();
        }

        private void NuevoTurno()
        {
            seleccionado = -1;
            pagina = 0;
            dialogo = partida.ClienteActual.Saludo;
            Actualizar();
        }

        private void Actualizar()
        {
            Concesionario jugador = partida.JugadorActual;
            int paginas = Paginas();
            if (pagina >= paginas) pagina = paginas - 1;
            btnAnterior.Visible = jugador.NumAutos > FILAS;
            btnSiguiente.Visible = jugador.NumAutos > FILAS;
            btnAnterior.Enabled = pagina > 0;
            btnSiguiente.Enabled = pagina < paginas - 1;
            btnVender.Enabled = !partida.YaVendio;
            Invalidate();
        }

        private int Paginas()
        {
            int n = partida.JugadorActual.NumAutos;
            return Math.Max(1, (n + FILAS - 1) / FILAS);
        }

        private Rectangle Fila(int k)
        {
            return new Rectangle(40, 164 + k * 116, 340, 100);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            Concesionario jugador = partida.JugadorActual;
            Cliente cliente = partida.ClienteActual;

            // ----- barra de arriba -----
            Pixel.R(g, 0, 0, Pixel.ANCHO, 84, Paleta.Negro);
            Pixel.R(g, 0, 0, Pixel.ANCHO, 78, Paleta.Morado2);
            Pixel.Texto(g, $"RONDA {partida.Ronda}/{Partida.RONDAS}", 28, 40, 15, Paleta.Agua);
            Pixel.R(g, 464, 8, 64, 64, Paleta.Morado3);
            Pixel.Sprite(g, Recursos.Vendedores[jugador.Vendedor], 464, 8, 4);
            Pixel.Texto(g, $"Turno de {jugador.Jugador.ToUpper()}", 544, 40, 15, Paleta.Crema);
            Pixel.Sprite(g, Recursos.Moneda, 1000, 22, 6);
            Pixel.Texto(g, Pixel.Dinero(jugador.Dinero), 1048, 40, 17, Paleta.Oro);

            // ----- mi lote -----
            Pixel.Panel(g, 20, 104, 380, 512, Paleta.Crema);
            Pixel.Texto(g, "MI LOTE", 44, 136, 13, Paleta.Morado2);
            if (jugador.NumAutos > FILAS)
                Pixel.Texto(g, $"{pagina + 1}/{Paginas()}", 290, 136, 12, Paleta.Gris, StringAlignment.Far);

            if (jugador.NumAutos == 0)
            {
                Pixel.Parrafo(g, "Tu lote está vacío. Compra autos en el MERCADO para tener qué vender.",
                              new Rectangle(44, 176, 330, 200), 12, Paleta.Gris);
            }
            for (int k = 0; k < FILAS; k++)
            {
                int indice = pagina * FILAS + k;
                if (indice >= jugador.NumAutos) break;
                Auto auto = jugador.ObtenerAuto(indice);
                Rectangle f = Fila(k);

                if (indice == seleccionado)
                    Pixel.R(g, f.X - 6, f.Y - 6, f.Width + 12, f.Height + 12, Paleta.Oro);
                Pixel.R(g, f.X, f.Y, f.Width, f.Height, Paleta.LilaClaro);
                Pixel.Sprite(g, Recursos.Autos[auto.Tipo, auto.Color], f.X + 8, f.Y + 18, 4);
                Pixel.Texto(g, auto.Nombre, f.X + 148, f.Y + 34, 12, Paleta.Noche);
                Pixel.Texto(g, Pixel.Dinero(auto.PrecioVenta()), f.X + 148, f.Y + 70, 12, Paleta.Verde2);
            }

            Pixel.Texto(g, "MIS CARTAS", 44, 524, 12, Paleta.Morado2);
            // Aquí se dibujan las cartas del jugador (las pone el equipo de las cartas)
            for (int c = 0; c < 3; c++)
                Pixel.CartaBocaAbajo(g, 44 + c * 88, 544, 80, 60);

            // ----- showroom con el cliente -----
            Pixel.R(g, 420, 104, 500, 380, Paleta.Showroom);
            Pixel.R(g, 420, 104, 500, 8, Paleta.Negro);
            for (int x = 428; x < 920; x += 48)
                Pixel.R(g, x, 420, 24, 4, Paleta.Morado3);
            Pixel.R(g, 472, 176, 192, 240, Paleta.Negro);
            Pixel.R(g, 480, 184, 176, 224, Paleta.Morado3);
            Pixel.Sprite(g, Recursos.Clientes[cliente.Retrato], 480, 200, 11);
            Pixel.Texto(g, cliente.Nombre, 568, 452, 13, Paleta.Crema, StringAlignment.Center);

            Pixel.Globo(g, 688, 140, 216, 192);
            DibujarDialogo(g, new Rectangle(712, 158, 180, 166));

            // ----- ranking -----
            Pixel.Panel(g, 940, 104, 320, 512, Paleta.Crema);
            Pixel.Texto(g, "RANKING", 964, 136, 13, Paleta.Morado2);
            Concesionario[] ranking = partida.Ranking();
            for (int i = 0; i < ranking.Length; i++)
            {
                int y = 168 + i * 104;
                Pixel.R(g, 960, y, 280, 88, i == 0 ? Paleta.OroClaro : Paleta.LilaClaro);
                Pixel.Texto(g, $"{i + 1}", 980, y + 44, 15, Paleta.Morado2);
                Pixel.Sprite(g, Recursos.Vendedores[ranking[i].Vendedor], 1008, y + 12, 4);
                Pixel.Texto(g, ranking[i].Jugador, 1084, y + 28, 12, Paleta.Noche);
                Pixel.Texto(g, Pixel.DineroCorto(ranking[i].Patrimonio()), 1084, y + 62, 12, Paleta.Verde2);
            }
        }

        // Si el texto no cabe en el globo, se hace más chiquita la letra
        private void DibujarDialogo(Graphics g, Rectangle area)
        {
            float[] tamanos = { 13, 12, 11, 10 };
            foreach (float tam in tamanos)
            {
                SizeF medida = g.MeasureString(dialogo, Recursos.Letra(tam), area.Width);
                if (medida.Height <= area.Height || tam == 10)
                {
                    Pixel.Parrafo(g, dialogo, area, tam, Paleta.Noche);
                    return;
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            for (int k = 0; k < FILAS; k++)
            {
                int indice = pagina * FILAS + k;
                if (indice < partida.JugadorActual.NumAutos && Fila(k).Contains(e.Location))
                {
                    seleccionado = indice == seleccionado ? -1 : indice;
                    Invalidate();
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            bool sobreFila = false;
            for (int k = 0; k < FILAS; k++)
                if (pagina * FILAS + k < partida.JugadorActual.NumAutos && Fila(k).Contains(e.Location))
                    sobreFila = true;
            Cursor = sobreFila ? Cursors.Hand : Cursors.Default;
        }

        private void btnVender_Click(object sender, EventArgs e)
        {
            if (seleccionado == -1)
            {
                dialogo = "¿Qué me enseñas? Escoge un auto de tu lote.";
            }
            else
            {
                dialogo = partida.Vender(seleccionado, out bool vendido);
                if (vendido)
                    seleccionado = -1;
            }
            Actualizar();
        }

        private void btnUsarCarta_Click(object sender, EventArgs e)
        {
            // Lo conecta el equipo de las cartas
            dialogo = "Las ABILICARDS llegan en la siguiente versión.";
            Invalidate();
        }

        private void btnPasar_Click(object sender, EventArgs e)
        {
            partida.SiguienteTurno();
            if (partida.Terminada)
                IrA(new FormResultados(partida));
            else
                NuevoTurno();
        }

        private void btnMercado_Click(object sender, EventArgs e)
        {
            using (FormMercado mercado = new FormMercado(partida.JugadorActual, partida.Catalogo))
                mercado.ShowDialog(this);
            Actualizar();
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            pagina--;
            Actualizar();
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            pagina++;
            Actualizar();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Escape &&
                MessageBox.Show("¿Salir de la partida?", "Concesionario", MessageBoxButtons.YesNo) == DialogResult.Yes)
                Application.Exit();
        }
    }
}
