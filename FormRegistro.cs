namespace PrograProyectoFinal
{
    // Pantalla 1: cada jugador escribe su nombre y escoge a su vendedor
    public partial class FormRegistro : FormPixel
    {
        private const int MAX_CARACTERES = 12;

        private Partida partida = new Partida();
        private string[] duenos = new string[4];   // quién tiene a cada vendedor (null = libre)
        private int elegido = -1;
        private string aviso = "";                 // mensaje de error en rosa, en vez de MessageBox

        public FormRegistro()
        {
            InitializeComponent();
            txtNombre.Font = Recursos.Letra(17);
            txtNombre.MaxLength = MAX_CARACTERES;
            btnAgregar.Colores(Paleta.Crema, Paleta.Crema2);
            btnLimpiar.Colores(Paleta.Crema, Paleta.Crema2);
            btnJugar.Colores(Paleta.Verde, Paleta.Verde2);
            ElegirPrimeroLibre();
            ActualizarBotones();
        }

        // Rectángulo de la tarjeta de cada vendedor
        private Rectangle Tarjeta(int i)
        {
            return new Rectangle(40 + i * 304, 124, 280, 300);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            if (aviso != "")
                Pixel.Texto(g, aviso, 1252, 96, 13, Paleta.Rosa, StringAlignment.Far);
            else if (partida.NumJugadores < Partida.MAX_JUGADORES)
                Pixel.Texto(g, $"Registrando jugador {partida.NumJugadores + 1} de {Partida.MAX_JUGADORES}", 1252, 96, 13, Paleta.Oro, StringAlignment.Far);
            else
                Pixel.Texto(g, "¡Ya están los 4! Denle a jugar", 1252, 96, 13, Paleta.Oro, StringAlignment.Far);

            for (int i = 0; i < 4; i++)
            {
                Rectangle t = Tarjeta(i);
                bool seleccionado = i == elegido;
                bool tomado = duenos[i] != null;

                Pixel.Panel(g, t.X, t.Y, t.Width, t.Height, seleccionado ? Paleta.Oro : Paleta.Crema);
                if (seleccionado)
                    Pixel.R(g, t.X + 12, t.Y + 12, t.Width - 24, t.Height - 24, Paleta.Crema);
                Pixel.R(g, t.X + 60, t.Y + 28, 160, 160, tomado ? Paleta.Crema2 : Paleta.LilaClaro);
                Pixel.Sprite(g, Recursos.Vendedores[i], t.X + 60, t.Y + 28, 10);
                Pixel.Texto(g, Recursos.NombresVendedor[i], t.X + 140, t.Y + 216, 13, Paleta.Noche, StringAlignment.Center);

                string estado = tomado ? $"lo tiene {duenos[i]}" : seleccionado ? "ELEGIDO" : "libre";
                Color colorEstado = tomado ? Paleta.Gris : seleccionado ? Paleta.Rosa2 : Paleta.Agua2;
                Pixel.Texto(g, estado, t.X + 140, t.Y + 256, 11, colorEstado, StringAlignment.Center);
            }

            // caja del nombre (el TextBox va encima)
            Pixel.Panel(g, 40, 452, 760, 112, Paleta.Crema);
            Pixel.Texto(g, "Tu nombre:", 68, 488, 12, Paleta.Morado2);
            Pixel.Texto(g, $"{txtNombre.Text.Length} / {MAX_CARACTERES}", 776, 532, 11, Paleta.Gris, StringAlignment.Far);

            // quiénes ya están en la partida
            Pixel.Texto(g, "En la partida:", 832, 472, 12, Paleta.Tenue);
            for (int k = 0; k < partida.NumJugadores; k++)
            {
                Concesionario j = partida.ObtenerJugador(k);
                int x = 832 + (k % 2) * 208;
                int y = 494 + (k / 2) * 50;
                Pixel.R(g, x, y, 44, 44, Paleta.Morado2);
                Pixel.Sprite(g, Recursos.Vendedores[j.Vendedor], x + 6, y + 6, 2);
                Pixel.Texto(g, j.Jugador, x + 54, y + 22, 12, Paleta.Crema);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            bool sobreTarjeta = false;
            for (int i = 0; i < 4; i++)
                if (Tarjeta(i).Contains(e.Location) && duenos[i] == null)
                    sobreTarjeta = true;
            Cursor = sobreTarjeta ? Cursors.Hand : Cursors.Default;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            for (int i = 0; i < 4; i++)
            {
                if (Tarjeta(i).Contains(e.Location))
                {
                    if (duenos[i] != null)
                        aviso = $"{Recursos.NombresVendedor[i]} ya lo tiene {duenos[i]}";
                    else
                    {
                        elegido = i;
                        aviso = "";
                    }
                    txtNombre.Focus();
                    Invalidate();
                }
            }
        }

        private void txtNombre_TextChanged(object sender, EventArgs e)
        {
            aviso = "";
            Invalidate(new Rectangle(600, 70, 680, 480));
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            Agregar();
        }

        private bool Agregar()
        {
            string nombre = txtNombre.Text.Trim();

            if (partida.NumJugadores == Partida.MAX_JUGADORES)
                aviso = "Ya no caben: máximo 4 jugadores";
            else if (nombre == "")
                aviso = "Escribe tu nombre";
            else if (NombreRepetido(nombre))
                aviso = "Ese nombre ya está registrado";
            else if (elegido == -1)
                aviso = "Escoge a tu vendedor";
            else
            {
                partida.AgregarJugador(new Concesionario(nombre, elegido));
                duenos[elegido] = nombre;
                txtNombre.Text = "";
                aviso = "";
                ElegirPrimeroLibre();
            }

            ActualizarBotones();
            Invalidate();
            txtNombre.Focus();
            return aviso == "";
        }

        // Búsqueda lineal en los jugadores que ya se registraron
        private bool NombreRepetido(string nombre)
        {
            for (int i = 0; i < partida.NumJugadores; i++)
                if (partida.ObtenerJugador(i).Jugador.ToLower() == nombre.ToLower())
                    return true;
            return false;
        }

        private void ElegirPrimeroLibre()
        {
            elegido = -1;
            for (int i = 0; i < 4 && elegido == -1; i++)
                if (duenos[i] == null)
                    elegido = i;
        }

        private void ActualizarBotones()
        {
            btnAgregar.Enabled = partida.NumJugadores < Partida.MAX_JUGADORES;
            txtNombre.Enabled = partida.NumJugadores < Partida.MAX_JUGADORES;
            btnJugar.Enabled = partida.NumJugadores >= Partida.MIN_JUGADORES ||
                               (partida.NumJugadores == Partida.MIN_JUGADORES - 1 && txtNombre.Text.Trim() != "");
        }

        // LIMPIAR: empieza el registro desde cero
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            partida = new Partida();
            duenos = new string[4];
            txtNombre.Text = "";
            aviso = "";
            ElegirPrimeroLibre();
            ActualizarBotones();
            Invalidate();
            txtNombre.Focus();
        }

        private void btnJugar_Click(object sender, EventArgs e)
        {
            // Si dejó un nombre escrito, se registra antes de empezar
            if (txtNombre.Text.Trim() != "" && !Agregar())
                return;

            if (partida.NumJugadores < Partida.MIN_JUGADORES)
            {
                aviso = $"Se necesitan mínimo {Partida.MIN_JUGADORES} jugadores";
                Invalidate();
                return;
            }

            // Aquí iría el draft de cartas cuando esté listo:
            // IrA(new FormDraft(partida));
            IrA(new FormTablero(partida));
        }

        private void txtNombre_KeyUp(object sender, KeyEventArgs e)
        {
            ActualizarBotones();
        }
    }
}
