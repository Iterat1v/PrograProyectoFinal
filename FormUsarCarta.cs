namespace PrograProyectoFinal
{
    // Las cartas del jugador en turno. Si es Estafa, se escoge a quién.
    public partial class FormUsarCarta : FormPixel
    {
        private Partida partida;
        private Concesionario jugador;
        private Concesionario[] rivales;
        private int seleccionada = -1;
        private int rival = -1;
        private string aviso = "";

        // Lo que pasó, para que el tablero lo muestre en el globo
        public string Resultado { get; private set; } = "";

        internal FormUsarCarta(Partida partida)
        {
            InitializeComponent();
            this.partida = partida;
            jugador = partida.JugadorActual;
            Titulo = $"TUS ABILICARDS · {jugador.Jugador.ToUpper()}";
            btnUsar.Colores(Paleta.Rosa, Paleta.Rosa2);
            btnCerrar.Colores(Paleta.Crema, Paleta.Crema2);

            // Los rivales son todos menos el que está jugando
            rivales = new Concesionario[partida.NumJugadores - 1];
            int k = 0;
            for (int i = 0; i < partida.NumJugadores; i++)
                if (partida.ObtenerJugador(i) != jugador)
                    rivales[k++] = partida.ObtenerJugador(i);

            if (jugador.NumCartas > 0)
                seleccionada = 0;
            if (rivales.Length == 1)
                rival = 0;
            btnUsar.Enabled = jugador.NumCartas > 0;
        }

        private Rectangle Carta(int i)
        {
            return new Rectangle(40 + i * 292, 88, 272, 420);
        }

        private Rectangle Rival(int k)
        {
            return new Rectangle(944, 168 + k * 84, 272, 72);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            if (jugador.NumCartas == 0)
                Pixel.Parrafo(g, "Ya no tienes cartas. Las gastaste todas.", new Rectangle(40, 100, 840, 100), 15, Paleta.Crema);

            for (int i = 0; i < jugador.NumCartas; i++)
            {
                Rectangle r = Carta(i);
                if (i == seleccionada)
                    Pixel.R(g, r.X - 8, r.Y - 8, r.Width + 16, r.Height + 16, Paleta.Oro);
                Pixel.CartaGrande(g, r.X, r.Y, r.Width, r.Height, jugador.ObtenerCarta(i));
            }

            // Panel derecho: a quién o qué hace
            Pixel.Panel(g, 920, 88, 320, 420, Paleta.Crema);
            if (seleccionada != -1)
            {
                PowerUp carta = jugador.ObtenerCarta(seleccionada);
                if (carta.NecesitaRival)
                {
                    Pixel.Texto(g, "¿A QUIÉN?", 944, 128, 13, Paleta.Rosa2);
                    for (int k = 0; k < rivales.Length; k++)
                    {
                        Rectangle r = Rival(k);
                        Pixel.R(g, r.X, r.Y, r.Width, r.Height, k == rival ? Paleta.OroClaro : Paleta.LilaClaro);
                        if (k == rival)
                            Pixel.R(g, r.X, r.Y, 6, r.Height, Paleta.Rosa);
                        Pixel.Sprite(g, Recursos.Vendedores[rivales[k].Vendedor], r.X + 12, r.Y + 12, 3);
                        Pixel.Texto(g, rivales[k].Jugador, r.X + 76, r.Y + 22, 12, Paleta.Noche);
                        Pixel.Texto(g, $"{rivales[k].NumAutos} autos · {rivales[k].NumCartas} cartas", r.X + 76, r.Y + 52, 10, Paleta.Gris);
                    }
                }
                else if (carta is Aseguradora)
                    Pixel.Parrafo(g, "Esta no se usa con el botón: se activa sola cuando un rival te lanza una Estafa.", new Rectangle(948, 120, 268, 360), 12, Paleta.Noche);
                else
                    Pixel.Parrafo(g, "Esta te ayuda a ti. Dale USAR y se aplica al instante.", new Rectangle(948, 120, 268, 360), 12, Paleta.Noche);
            }

            if (aviso != "")
                Pixel.Parrafo(g, aviso, new Rectangle(40, 540, 620, 160), 12, Paleta.Rosa);
            else
                Pixel.Parrafo(g, $"Tienes {jugador.NumCartas} de {Concesionario.MAX_CARTAS} cartas. Cada una se usa 1 vez.", new Rectangle(40, 540, 620, 160), 12, Paleta.Tenue);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            for (int i = 0; i < jugador.NumCartas; i++)
                if (Carta(i).Contains(e.Location))
                {
                    seleccionada = i;
                    aviso = "";
                }
            if (seleccionada != -1 && jugador.ObtenerCarta(seleccionada).NecesitaRival)
                for (int k = 0; k < rivales.Length; k++)
                    if (Rival(k).Contains(e.Location))
                    {
                        rival = k;
                        aviso = "";
                    }
            Invalidate();
        }

        private void btnUsar_Click(object sender, EventArgs e)
        {
            if (seleccionada == -1) return;
            PowerUp carta = jugador.ObtenerCarta(seleccionada);

            if (carta.NecesitaRival && rival == -1)
                aviso = "Escoge a qué rival se la vas a aplicar.";
            else if (carta.Efecto == Regateo.TOMA_MI_DINERO && carta is Regateo && partida.YaVendio)
                aviso = "Ya vendiste en este turno. Guárdala para el siguiente cliente.";
            else
            {
                Concesionario objetivo = carta.NecesitaRival ? rivales[rival] : null;
                string resultado = carta.Aplicar(jugador, objetivo, out bool usada);
                if (usada)
                {
                    jugador.QuitarCarta(seleccionada);
                    Resultado = resultado;
                    Recursos.Sonar(carta is Estafa ? "estafa" : "venta");
                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }
                aviso = resultado;
            }
            Recursos.Sonar("error");
            Invalidate();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
