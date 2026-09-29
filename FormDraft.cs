namespace PrograProyectoFinal
{
    // Pantalla 2: 16 cartas boca abajo sacadas de las 25.
    // Por turnos (en serpiente) cada jugador voltea una hasta tener 3.
    public partial class FormDraft : FormPixel
    {
        private const int PASOS = 16;         // pasos de la animación de voltear

        private Partida partida;
        private PowerUp ultima;               // la carta que se muestra en "SALIÓ"
        private int volteando = -1;           // casilla que se está volteando
        private int paso = 0;

        internal FormDraft(Partida partida)
        {
            InitializeComponent();
            this.partida = partida;
            partida.PrepararDraft();
            btnIniciar.Colores(Paleta.Verde, Paleta.Verde2);
            btnIniciar.Enabled = false;
        }

        private Rectangle Casilla(int fila, int columna)
        {
            return new Rectangle(40 + columna * 184, 132 + fila * 144, 168, 132);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            int n = partida.NumJugadores;

            if (partida.DraftTerminado)
                Pixel.Texto(g, "¡Listo! Todos tienen sus 3 cartas", 40, 96, 15, Paleta.Oro);
            else
                Pixel.Texto(g, $"Ronda {partida.RondaDraft} · Turno de: {partida.ObtenerJugador(partida.JugadorDraft).Jugador.ToUpper()}", 40, 96, 15, Paleta.Oro);

            // ----- cuadrícula 4 x 4 -----
            for (int f = 0; f < 4; f++)
            {
                for (int c = 0; c < 4; c++)
                {
                    Rectangle r = Casilla(f, c);
                    int dueno = partida.DuenoDraft(f, c);
                    bool esLaQueGira = volteando == f * 4 + c;

                    if (esLaQueGira)
                    {
                        // Se angosta hasta 0 y se vuelve a abrir ya volteada
                        int ancho = paso <= PASOS / 2 ? r.Width * (PASOS / 2 - paso) / (PASOS / 2)
                                                      : r.Width * (paso - PASOS / 2) / (PASOS / 2);
                        ancho = Math.Max(ancho, 16);
                        int x = r.X + (r.Width - ancho) / 2;
                        if (paso <= PASOS / 2)
                            Pixel.CartaBocaAbajo(g, x, r.Y, ancho, r.Height);
                        else
                            Pixel.Panel(g, x, r.Y, ancho, r.Height, Paleta.Crema);
                    }
                    else if (dueno == -1)
                        Pixel.CartaBocaAbajo(g, r.X, r.Y, r.Width, r.Height);
                    else
                        Pixel.Carta(g, r.X, r.Y, r.Width, r.Height, partida.CartaDraft(f, c), partida.ObtenerJugador(dueno).Jugador);
                }
            }

            // ----- panel "SALIÓ" -----
            Pixel.Panel(g, 800, 124, 448, 300, Paleta.Crema);
            Pixel.Texto(g, "SALIÓ:", 828, 160, 12, Paleta.Morado2);
            if (ultima == null)
            {
                Pixel.CartaBocaAbajo(g, 828, 184, 140, 200);
                Pixel.Parrafo(g, "Escoge una carta para voltearla.", new Rectangle(988, 196, 236, 200), 12, Paleta.Noche);
            }
            else
            {
                Pixel.Carta(g, 828, 184, 140, 200, ultima, null);
                Pixel.Parrafo(g, ultima.Nombre.ToUpper(), new Rectangle(988, 186, 240, 90), 15, Paleta.Noche);
                Pixel.Parrafo(g, $"{ultima.Descripcion} Se usa 1 vez.", new Rectangle(988, 280, 240, 140), 11, Paleta.Noche);
            }

            // ----- orden del turno -----
            Pixel.Texto(g, "ORDEN DEL TURNO", 800, 460, 12, Paleta.Agua);
            string orden = "";
            for (int k = 0; k < n; k++)
            {
                int j = partida.OrdenDraft(k);
                int x = 800 + k * 112;
                bool actual = !partida.DraftTerminado && k == partida.PosicionDraft;
                Pixel.R(g, x, 488, 88, 88, actual ? Paleta.Oro : Paleta.Morado2);
                Pixel.R(g, x + 4, 492, 80, 80, Paleta.Morado2);
                Pixel.Sprite(g, Recursos.Vendedores[partida.ObtenerJugador(j).Vendedor], x + 4, 492, 5);
                if (k < n - 1)
                    Pixel.Texto(g, "›", x + 100, 530, 16, Paleta.Crema, StringAlignment.Center);

                string nombre = partida.ObtenerJugador(j).Jugador;
                orden += actual ? nombre.ToUpper() : nombre;
                if (k < n - 1) orden += " → ";
            }
            Pixel.Texto(g, orden, 800, 608, 13, Paleta.Crema);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (volteando != -1 || partida.DraftTerminado) return;

            for (int f = 0; f < 4; f++)
                for (int c = 0; c < 4; c++)
                    if (Casilla(f, c).Contains(e.Location) && partida.DuenoDraft(f, c) == -1)
                    {
                        volteando = f * 4 + c;
                        paso = 0;
                        Recursos.Sonar("carta");
                        timerVoltear.Start();
                        return;
                    }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            bool sobreCarta = false;
            for (int f = 0; f < 4; f++)
                for (int c = 0; c < 4; c++)
                    if (Casilla(f, c).Contains(e.Location) && partida.DuenoDraft(f, c) == -1 && !partida.DraftTerminado)
                        sobreCarta = true;
            Cursor = sobreCarta ? Cursors.Hand : Cursors.Default;
        }

        private void timerVoltear_Tick(object sender, EventArgs e)
        {
            paso++;
            if (paso == PASOS)
            {
                timerVoltear.Stop();
                int f = volteando / 4, c = volteando % 4;
                ultima = partida.CartaDraft(f, c);
                partida.EscogerCarta(f, c);
                volteando = -1;
                if (partida.DraftTerminado)
                {
                    btnIniciar.Enabled = true;
                    Recursos.Sonar("turno");
                }
            }
            Invalidate();
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            IrA(new FormTablero(partida));
        }
    }
}
