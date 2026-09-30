namespace PrograProyectoFinal
{
    // Tu personaje + la tienda: escoges tez, peinado, lentes, gorros, máscaras...
    // Lo que no tienes se compra con monedas; algunas cosas piden nivel.
    public partial class FormPersonaje : FormPixel
    {
        private const int COLUMNAS = 5;

        private Perfil perfil;
        private int parte = Avatar.PEINADO;           // categoría que se está viendo
        private BotonPixel[] pestanas = new BotonPixel[Avatar.PARTES];
        private Image[] vistas = new Image[10];        // cómo se vería con cada opción
        private Image cara;
        private string aviso = "";

        internal FormPersonaje(Perfil perfil)
        {
            InitializeComponent();
            this.perfil = perfil;
            Titulo = $"CASILLERO · {perfil.Nombre.ToUpper()}";
            btnAzar.Colores(Paleta.Agua, Paleta.Agua2);
            btnListo.Colores(Paleta.Verde, Paleta.Verde2);

            // Las 8 pestañas se crean con un arreglo de botones
            for (int i = 0; i < Avatar.PARTES; i++)
            {
                BotonPixel b = new BotonPixel();
                b.Text = Avatar.NombresParte[i];
                b.TamLetra = 10;
                b.Size = new Size(200, 52);
                b.Location = new Point(420 + (i % 4) * 206, 72 + (i / 4) * 58);
                b.Tag = i;
                b.Click += Pestana_Click;
                pestanas[i] = b;
                Controls.Add(b);
            }
            Refrescar();
        }

        private void Refrescar()
        {
            cara = perfil.Avatar.Dibujar();
            for (int o = 0; o < vistas.Length; o++)
                vistas[o] = null;
            for (int o = 0; o < Avatar.Opciones[parte].Length; o++)
            {
                Avatar prueba = perfil.Avatar.Copia();
                prueba.Cambiar(parte, o);
                vistas[o] = prueba.Dibujar();
            }
            for (int i = 0; i < Avatar.PARTES; i++)
                pestanas[i].Colores(i == parte ? Paleta.Oro : Paleta.LilaClaro, i == parte ? Paleta.Oro2 : Paleta.Crema2);
            Invalidate();
        }

        private Rectangle Casilla(int opcion)
        {
            return new Rectangle(420 + (opcion % COLUMNAS) * 166, 200 + (opcion / COLUMNAS) * 204, 158, 196);
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
            Pixel.R(g, 70, 488, 300, 22, Paleta.Negro);
            Pixel.R(g, 74, 492, 292, 14, Paleta.LilaClaro);
            Pixel.R(g, 74, 492, 292 * perfil.XpEnNivel / Perfil.XP_POR_NIVEL, 14, Paleta.Agua);
            Pixel.Texto(g, $"{perfil.XpEnNivel} / {Perfil.XP_POR_NIVEL} XP", 220, 530, 10, Paleta.Gris, StringAlignment.Center);
            Pixel.Sprite(g, Recursos.Moneda, 110, 562, 6);
            Pixel.Texto(g, $"{perfil.Monedas} monedas", 160, 580, 14, Paleta.Oro2);
            Pixel.Parrafo(g, "Ganas monedas y XP en cada partida.", new Rectangle(64, 620, 312, 60), 9, Paleta.Gris, StringAlignment.Center);

            // ----- artículos de la categoría -----
            for (int o = 0; o < Avatar.Opciones[parte].Length; o++)
            {
                Rectangle c = Casilla(o);
                bool puesto = perfil.Avatar.Parte(parte) == o;
                bool tuyo = perfil.LoTiene(parte, o);
                int nivel = Perfil.Niveles[parte][o];
                bool bloqueado = !tuyo && perfil.Nivel < nivel;

                if (puesto)
                {
                    Pixel.Panel(g, c.X, c.Y, c.Width, c.Height, Paleta.Oro);
                    Pixel.R(g, c.X + 10, c.Y + 10, c.Width - 20, c.Height - 20, Paleta.Crema);
                }
                else
                    Pixel.Panel(g, c.X, c.Y, c.Width, c.Height, bloqueado ? Paleta.Crema2 : Paleta.Crema);

                Pixel.R(g, c.X + 27, c.Y + 14, 104, 104, bloqueado ? Paleta.Gris : Paleta.Morado3);
                Pixel.Sprite(g, vistas[o], c.X + 31, c.Y + 18, 6);
                Pixel.Texto(g, Avatar.Opciones[parte][o], c.X + 79, c.Y + 140, 9, Paleta.Noche, StringAlignment.Center);

                if (puesto)
                    Pixel.Texto(g, "PUESTO", c.X + 79, c.Y + 170, 10, Paleta.Verde2, StringAlignment.Center);
                else if (tuyo)
                    Pixel.Texto(g, "TUYO", c.X + 79, c.Y + 170, 10, Paleta.Gris, StringAlignment.Center);
                else if (bloqueado)
                    Pixel.Texto(g, $"NIVEL {nivel}", c.X + 79, c.Y + 170, 10, Paleta.Rosa2, StringAlignment.Center);
                else
                {
                    Pixel.Sprite(g, Recursos.Moneda, c.X + 40, c.Y + 162, 3);
                    Pixel.Texto(g, $"{Perfil.Precios[parte][o]}", c.X + 64, c.Y + 170, 10, Paleta.Oro2);
                }
            }

            if (aviso != "")
                Pixel.Parrafo(g, aviso, new Rectangle(420, 626, 290, 80), 10, Paleta.Rosa);
            else
                Pixel.Parrafo(g, "Clic para ponértelo. Si no lo tienes, se compra.", new Rectangle(420, 626, 290, 80), 10, Paleta.Tenue);
        }

        private void Pestana_Click(object sender, EventArgs e)
        {
            parte = (int)((BotonPixel)sender).Tag;
            aviso = "";
            Recursos.Sonar("menu");
            Refrescar();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            for (int o = 0; o < Avatar.Opciones[parte].Length; o++)
            {
                if (!Casilla(o).Contains(e.Location)) continue;

                if (!perfil.LoTiene(parte, o))
                {
                    string razon = perfil.Nivel < Perfil.Niveles[parte][o] || perfil.Monedas < Perfil.Precios[parte][o]
                        ? perfil.Comprar(parte, o) : null;
                    if (razon != null)
                    {
                        aviso = razon;
                        Recursos.Sonar("error");
                        Invalidate();
                        return;
                    }
                    if (!FormAviso.Preguntar(this, "¿LO COMPRAS?", $"{Avatar.Opciones[parte][o]} cuesta {Perfil.Precios[parte][o]} monedas. Tienes {perfil.Monedas}.",
                                             Paleta.Oro, vistas[o]))
                        return;
                    perfil.Comprar(parte, o);
                    Recursos.Sonar("compra");
                }
                else
                    Recursos.Sonar("carta");

                perfil.Avatar.Cambiar(parte, o);
                perfil.Personalizado = true;
                aviso = "";
                Perfiles.Guardar();
                Refrescar();
                return;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            bool sobre = false;
            for (int o = 0; o < Avatar.Opciones[parte].Length; o++)
                if (Casilla(o).Contains(e.Location)) sobre = true;
            Cursor = sobre ? Cursors.Hand : Cursors.Default;
        }

        // AL AZAR: solo con lo que ya tienes
        private void btnAzar_Click(object sender, EventArgs e)
        {
            Random azar = new Random();
            for (int p = 0; p < Avatar.PARTES; p++)
            {
                int o;
                do o = azar.Next(Avatar.Opciones[p].Length);
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
