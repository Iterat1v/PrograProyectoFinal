namespace PrograProyectoFinal
{
    // Tienda del día estilo Fortnite: 2 destacados grandes con descuento y 8 del diario.
    // Cada tarjeta tiene el color de su rareza, tu personaje con el artículo puesto y su precio.
    public class FormTienda : FormPixel
    {
        private const int TOTAL = Tienda.DESTACADOS + Tienda.DIARIOS;

        private Perfil perfil;
        private Image[] vistas = new Image[TOTAL];
        private int encima = -1;                    // tarjeta con el mouse encima
        private string aviso = "";
        private System.Windows.Forms.Timer reloj = new System.Windows.Forms.Timer { Interval = 1000 };

        // comoVentana = se abrió encima de otra pantalla (desde el registro): REGRESAR solo la cierra
        internal FormTienda(Perfil perfil, bool comoVentana = false)
        {
            this.perfil = perfil;
            Titulo = $"TIENDA DEL DÍA · {perfil.Nombre.ToUpper()}";
            BotonRegresar((s, e) => { if (comoVentana) Close(); else IrA(new FormLobby()); });
            Boton("CASILLERO", 1000, 648, 240, 60, Paleta.Lila, Paleta.Morado3, (s, e) =>
            {
                using (FormPersonaje casillero = new FormPersonaje(perfil)) casillero.ShowDialog(this);
                Refrescar();
            }, 12);
            Tienda.Armar();
            Refrescar();
            reloj.Tick += (s, e) =>
            {
                if (Tienda.Falta.TotalSeconds < 1) { Tienda.Armar(); Refrescar(); }
                Invalidate(new Rectangle(0, 60, 640, 44));
            };
            reloj.Start();
            FormClosed += (s, e) => reloj.Stop();
        }

        // Cómo se vería tu personaje con cada artículo
        private void Refrescar()
        {
            for (int k = 0; k < TOTAL; k++)
            {
                Avatar prueba = perfil.Avatar.Copia();
                prueba.Cambiar(Tienda.Parte[k], Tienda.Opcion[k]);
                vistas[k] = prueba.Dibujar();
            }
            Invalidate();
        }

        private Rectangle Tarjeta(int k)
        {
            if (k < Tienda.DESTACADOS)
                return new Rectangle(40 + k * 304, 138, 290, 496);
            int d = k - Tienda.DESTACADOS;
            return new Rectangle(660 + (d % 4) * 148, 138 + (d / 4) * 254, 140, 242);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // franja de arriba: contador y tus monedas
            Pixel.R(g, 0, 60, Pixel.ANCHO, 44, Paleta.Noche);
            TimeSpan falta = Tienda.Falta;
            Pixel.Texto(g, $"SE RENUEVA EN {falta.Hours:00}:{falta.Minutes:00}:{falta.Seconds:00}", 40, 82, 11, Paleta.Oro);
            Pixel.Sprite(g, Recursos.Moneda, 1110, 66, 4);
            Pixel.Texto(g, $"{perfil.Monedas}", 1148, 82, 14, Paleta.Oro);
            Pixel.Texto(g, $"NIVEL {perfil.Nivel}", 1090, 82, 10, Paleta.Agua, StringAlignment.Far);

            Pixel.Texto(g, "DESTACADO", 40, 120, 14, Paleta.Crema);
            Pixel.Texto(g, $"-{Tienda.DESCUENTO * 100:0} % SOLO HOY", 620, 120, 10, Paleta.Rosa, StringAlignment.Far);
            Pixel.Texto(g, "DIARIO", 660, 120, 14, Paleta.Crema);

            for (int k = 0; k < TOTAL; k++)
                DibujarTarjeta(g, k, Tarjeta(k), k < Tienda.DESTACADOS);

            Pixel.Parrafo(g, aviso != "" ? aviso : "Clic en un artículo para comprarlo. Lo que ya tienes te lo pones desde el CASILLERO.",
                          new Rectangle(40, 648, 940, 60), 11, aviso != "" ? Paleta.Rosa : Paleta.Tenue);
        }

        private void DibujarTarjeta(Graphics g, int k, Rectangle t, bool grande)
        {
            int parte = Tienda.Parte[k], opcion = Tienda.Opcion[k];
            int rareza = Tienda.Rareza(parte, opcion);
            Color color = Tienda.ColorRareza[rareza];
            Color oscuro = Tienda.ColorRarezaOscuro[rareza];
            bool tuyo = perfil.LoTiene(parte, opcion);
            int nivel = Perfil.Niveles[parte][opcion];
            bool bloqueado = !tuyo && perfil.Nivel < nivel;

            // marco (dorado si el mouse está encima)
            Pixel.R(g, t.X - 4, t.Y - 4, t.Width + 8, t.Height + 8, k == encima ? Paleta.Oro : Paleta.Negro);

            // fondo: degradado de claro (arriba) a su color (abajo), en franjas
            int franjas = 12;
            for (int i = 0; i < franjas; i++)
            {
                int y = t.Y + i * t.Height / franjas;
                Pixel.R(g, t.X, y, t.Width, t.Height / franjas + 1, Pixel.Aclarar(color, 0.45f - i * 0.035f));
            }
            // rayos diagonales de fondo, como en la tienda de Fortnite
            using (SolidBrush rayo = new SolidBrush(Color.FromArgb(40, 255, 255, 255)))
                for (int i = 0; i < 5; i++)
                {
                    int x0 = t.X + i * t.Width / 4 - t.Width / 4;
                    for (int y = 0; y < t.Height - (grande ? 130 : 92); y += 8)
                        g.FillRectangle(rayo, x0 + y / 2, t.Y + y, grande ? 18 : 10, 8);
                }

            // el personaje con el artículo puesto
            float escala = grande ? 15 : 7;
            int lado = (int)(16 * escala);
            Pixel.Sprite(g, vistas[k], t.X + (t.Width - lado) / 2, t.Y + (grande ? 40 : 20), escala);

            // banda de abajo con nombre, rareza y precio
            int alto = grande ? 130 : 92;
            int yb = t.Bottom - alto;
            Pixel.R(g, t.X, yb, t.Width, alto, oscuro);
            Pixel.R(g, t.X, yb, t.Width, 4, color);
            Pixel.Texto(g, Tienda.Nombre(k).ToUpper(), t.X + 12, yb + (grande ? 26 : 18), grande ? 16 : (Tienda.Nombre(k).Length > 10 ? 7 : 9), Paleta.Crema);
            Pixel.Texto(g, $"{Tienda.NombresRareza[rareza]} · {Avatar.NombresParte[parte]}", t.X + 12, yb + (grande ? 58 : 38), grande ? 9 : 6, Pixel.Aclarar(color, 0.5f));

            int yp = yb + (grande ? 100 : 70);
            if (tuyo)
                Pixel.Texto(g, "¡YA ES TUYO!", t.X + 12, yp, grande ? 13 : 8, Paleta.Verde);
            else if (bloqueado)
                Pixel.Texto(g, $"NIVEL {nivel}", t.X + 12, yp, grande ? 13 : 9, Paleta.Tenue);
            else
            {
                Pixel.Sprite(g, Recursos.Moneda, t.X + 10, yp - (grande ? 14 : 9), grande ? 3.5f : 2.2f);
                Pixel.Texto(g, $"{Tienda.Precio(k)}", t.X + (grande ? 56 : 40), yp, grande ? 15 : 10, Paleta.Oro);
                if (grande)
                {
                    // precio de antes tachado
                    string antes = $"{Tienda.PrecioNormal(k)}";
                    Pixel.Texto(g, antes, t.Right - 16, yp, 10, Paleta.Tenue, StringAlignment.Far);
                    Pixel.R(g, t.Right - 16 - antes.Length * 12, yp, antes.Length * 12, 2, Paleta.Tenue);
                }
            }

            // etiquetas de las esquinas
            if (grande)
            {
                Pixel.R(g, t.X, t.Y, 96, 34, Paleta.Rosa);
                Pixel.Texto(g, $"-{Tienda.DESCUENTO * 100:0} %", t.X + 48, t.Y + 17, 11, Paleta.Noche, StringAlignment.Center);
            }
            if (tuyo)
            {
                // palomita dibujada con cuadritos
                int s = grande ? 4 : 3, cx = t.Right - (grande ? 42 : 30), cy = t.Y + 6;
                Pixel.R(g, cx, cy, s * 9, s * 9, Paleta.Verde);
                int[,] palomita = { { 1, 5 }, { 2, 6 }, { 3, 7 }, { 4, 6 }, { 5, 5 }, { 6, 4 }, { 7, 3 } };
                for (int i = 0; i < palomita.GetLength(0); i++)
                    Pixel.R(g, cx + palomita[i, 0] * s, cy + palomita[i, 1] * s - s, s, s * 2, Paleta.Noche);
            }
            if (bloqueado)
                using (SolidBrush velo = new SolidBrush(Color.FromArgb(90, 18, 8, 24)))
                    g.FillRectangle(velo, t.X, t.Y, t.Width, yb - t.Y);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int antes = encima;
            encima = -1;
            for (int k = 0; k < TOTAL; k++)
                if (Tarjeta(k).Contains(e.Location)) encima = k;
            Cursor = encima != -1 ? Cursors.Hand : Cursors.Default;
            if (antes != encima) Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            for (int k = 0; k < TOTAL; k++)
                if (Tarjeta(k).Contains(e.Location))
                    Comprar(k);
        }

        private void Comprar(int k)
        {
            int parte = Tienda.Parte[k], opcion = Tienda.Opcion[k];
            string nombre = Tienda.Nombre(k);
            if (perfil.LoTiene(parte, opcion))
            {
                if (FormAviso.Preguntar(this, "YA ES TUYO", $"¿Te pones {nombre}?", Paleta.Verde, vistas[k]))
                    Ponerselo(parte, opcion);
                return;
            }
            int precio = Tienda.Precio(k);
            string razon = perfil.Nivel < Perfil.Niveles[parte][opcion] || perfil.Monedas < precio ? perfil.ComprarA(parte, opcion, precio) : null;
            if (razon != null)
            {
                aviso = razon;
                Recursos.Sonar("error");
                Invalidate();
                return;
            }
            int rareza = Tienda.Rareza(parte, opcion);
            if (!FormAviso.Preguntar(this, $"{Tienda.NombresRareza[rareza]}: {nombre.ToUpper()}",
                                     $"Cuesta {precio} monedas y tienes {perfil.Monedas}. ¿Lo compras?", Tienda.ColorRareza[rareza], vistas[k]))
                return;
            perfil.ComprarA(parte, opcion, precio);
            Recursos.Sonar("compra");
            aviso = $"¡Compraste {nombre}!";
            Perfiles.Guardar();
            if (FormAviso.Preguntar(this, "¡ES TUYO!", $"¿Te pones {nombre} de una vez?", Paleta.Verde, vistas[k]))
                Ponerselo(parte, opcion);
            Invalidate();
        }

        private void Ponerselo(int parte, int opcion)
        {
            perfil.Avatar.Cambiar(parte, opcion);
            perfil.Personalizado = true;
            Perfiles.Guardar();
            Recursos.Sonar("carta");
            Refrescar();
        }
    }
}
