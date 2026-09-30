namespace PrograProyectoFinal
{
    // Casillero: solo aparecen los artículos que YA tienes (los básicos, lo que compraste en la
    // tienda y lo que ganaste en el pase). Aquí no se compra nada: solo te lo pones.
    public partial class FormPersonaje : FormPixel
    {
        private const int COLUMNAS = 5;
        private const int POR_PAGINA = 10;

        private Perfil perfil;
        private int parte = Avatar.PEINADO;           // categoría que se está viendo
        private int pagina = 0;
        private BotonPixel[] pestanas = new BotonPixel[Avatar.PARTES];
        private int[] tuyos = new int[Cosmeticos.MAX];  // los artículos que tienes de esta parte
        private int numTuyos = 0;
        private Image[] vistas = new Image[POR_PAGINA];
        private Image cara;

        internal FormPersonaje(Perfil perfil)
        {
            InitializeComponent();
            this.perfil = perfil;
            Titulo = $"CASILLERO · {perfil.Nombre.ToUpper()}";
            btnAzar.Colores(Paleta.Agua, Paleta.Agua2);
            btnListo.Colores(Paleta.Verde, Paleta.Verde2);
            Boton("<", 420, 632, 64, 64, Paleta.LilaClaro, Paleta.Crema2, (s, e) => CambiarPagina(-1), 14);
            Boton(">", 492, 632, 64, 64, Paleta.LilaClaro, Paleta.Crema2, (s, e) => CambiarPagina(1), 14);

            // Las 9 pestañas se crean con un arreglo de botones
            for (int i = 0; i < Avatar.PARTES; i++)
            {
                BotonPixel b = new BotonPixel();
                b.Text = Avatar.NombresParte[i];
                b.TamLetra = 9;
                b.Size = new Size(158, 52);
                b.Location = new Point(420 + (i % 5) * 164, 72 + (i / 5) * 58);
                b.Tag = i;
                b.Click += Pestana_Click;
                pestanas[i] = b;
                Controls.Add(b);
            }
            Refrescar();
        }

        private int Paginas => Math.Max(1, (numTuyos + POR_PAGINA - 1) / POR_PAGINA);

        // Búsqueda lineal de lo que tienes en esta parte
        private void Refrescar()
        {
            numTuyos = 0;
            for (int o = 0; o < Cosmeticos.Cantidad(parte); o++)
                if (perfil.LoTiene(parte, o))
                    tuyos[numTuyos++] = o;
            if (pagina >= Paginas) pagina = Paginas - 1;

            cara = perfil.Avatar.Dibujar();
            for (int k = 0; k < POR_PAGINA; k++)
            {
                vistas[k] = null;
                int n = pagina * POR_PAGINA + k;
                if (n >= numTuyos) continue;
                Avatar prueba = perfil.Avatar.Copia();
                prueba.Cambiar(parte, tuyos[n]);
                vistas[k] = prueba.Dibujar();
            }
            for (int i = 0; i < Avatar.PARTES; i++)
                pestanas[i].Colores(i == parte ? Paleta.Oro : Paleta.LilaClaro, i == parte ? Paleta.Oro2 : Paleta.Crema2);
            Invalidate();
        }

        private void CambiarPagina(int cuanto)
        {
            pagina = Math.Max(0, Math.Min(Paginas - 1, pagina + cuanto));
            Recursos.Sonar("menu");
            Refrescar();
        }

        private Rectangle Casilla(int k)
        {
            return new Rectangle(420 + (k % COLUMNAS) * 166, 196 + (k / COLUMNAS) * 214, 158, 206);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // ----- tu personaje -----
            Pixel.Panel(g, 40, 76, 360, 624, Paleta.Crema);
            Pixel.Texto(g, perfil.Nombre, 220, 112, 15, Paleta.Noche, StringAlignment.Center);
            Pixel.R(g, 76, 140, 288, 288, Paleta.Negro);
            Pixel.R(g, 80, 144, 280, 280, Paleta.Morado3);
            Pixel.Sprite(g, cara, 92, 156, 16);
            Pixel.Texto(g, $"NIVEL {perfil.Nivel}", 220, 460, 14, Paleta.Morado2, StringAlignment.Center);
            int total = 0, tiene = 0;
            for (int p = 0; p < Avatar.PARTES; p++)
            {
                total += Cosmeticos.Cantidad(p);
                tiene += perfil.CuantosTiene(p);
            }
            Pixel.Texto(g, $"Colección: {tiene} de {total}", 220, 494, 11, Paleta.Gris, StringAlignment.Center);
            Pixel.R(g, 70, 512, 300, 16, Paleta.Negro);
            Pixel.R(g, 74, 516, 292 * tiene / total, 8, Paleta.Agua);
            Pixel.Sprite(g, Recursos.Moneda, 110, 548, 6);
            Pixel.Texto(g, $"{perfil.Monedas} monedas", 160, 566, 14, Paleta.Oro2);
            Pixel.Parrafo(g, "Consigue más en la TIENDA del día y en el PASE de temporada.", new Rectangle(64, 604, 312, 60), 9, Paleta.Gris, StringAlignment.Center);

            // ----- lo que tienes de esta parte -----
            if (numTuyos == 0)
                Pixel.Parrafo(g, "Todavía no tienes nada de esta parte.", new Rectangle(440, 300, 780, 60), 13, Paleta.Tenue, StringAlignment.Center);
            for (int k = 0; k < POR_PAGINA; k++)
            {
                int n = pagina * POR_PAGINA + k;
                if (n >= numTuyos) break;
                int o = tuyos[n];
                Rectangle c = Casilla(k);
                bool puesto = perfil.Avatar.Parte(parte) == o;
                int rareza = Cosmeticos.Rareza(parte, o);
                Color colorRareza = Cosmeticos.Origen(parte, o) == Cosmeticos.BASICO ? Paleta.Gris : Tienda.ColorRareza[rareza];

                Pixel.Panel(g, c.X, c.Y, c.Width, c.Height, puesto ? Paleta.Oro : Paleta.Crema);
                if (puesto) Pixel.R(g, c.X + 10, c.Y + 10, c.Width - 20, c.Height - 20, Paleta.Crema);
                Pixel.R(g, c.X + 27, c.Y + 14, 104, 104, colorRareza);
                Pixel.Sprite(g, vistas[k], c.X + 31, c.Y + 18, 6);
                Pixel.Parrafo(g, Cosmeticos.Nombre(parte, o), new Rectangle(c.X + 8, c.Y + 126, c.Width - 16, 40), 8, Paleta.Noche, StringAlignment.Center);
                string origen = Cosmeticos.NombresOrigen[Cosmeticos.Origen(parte, o)];
                Pixel.Texto(g, puesto ? "PUESTO" : origen, c.X + 79, c.Y + 184, 8, puesto ? Paleta.Verde2 : colorRareza, StringAlignment.Center);
            }
            Pixel.Texto(g, $"{pagina + 1}/{Paginas}", 574, 650, 12, Paleta.Crema);
            Pixel.Texto(g, $"{numTuyos}/{Cosmeticos.Cantidad(parte)}", 574, 682, 9, Paleta.Tenue);
        }

        private void Pestana_Click(object sender, EventArgs e)
        {
            parte = (int)((BotonPixel)sender).Tag;
            pagina = 0;
            Recursos.Sonar("menu");
            Refrescar();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            for (int k = 0; k < POR_PAGINA; k++)
            {
                int n = pagina * POR_PAGINA + k;
                if (n >= numTuyos || !Casilla(k).Contains(e.Location)) continue;
                perfil.Avatar.Cambiar(parte, tuyos[n]);
                perfil.Personalizado = true;
                Recursos.Sonar("carta");
                Perfiles.Guardar();
                Refrescar();
                return;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            bool sobre = false;
            for (int k = 0; k < POR_PAGINA; k++)
                if (pagina * POR_PAGINA + k < numTuyos && Casilla(k).Contains(e.Location)) sobre = true;
            Cursor = sobre ? Cursors.Hand : Cursors.Default;
        }

        // AL AZAR: solo con lo que ya tienes
        private void btnAzar_Click(object sender, EventArgs e)
        {
            Random azar = new Random();
            for (int p = 0; p < Avatar.PARTES; p++)
            {
                int o;
                do o = azar.Next(Cosmeticos.Cantidad(p));
                while (!perfil.LoTiene(p, o));
                perfil.Avatar.Cambiar(p, o);
            }
            perfil.Personalizado = true;
            Recursos.Sonar("carta");
            Refrescar();
        }

        private void btnListo_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Perfiles.Guardar();
            base.OnFormClosed(e);
        }
    }
}
