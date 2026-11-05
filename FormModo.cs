namespace PrograProyectoFinal
{
    // ¿Cómo quieres jugar? Partida libre (con zona y dificultad), modo historia,
    // reto del día, temporada, y continuar la partida guardada
    public class FormModo : FormPixel
    {
        private int zona = 0;
        private int dificultad = Partida.NORMAL;
        private BotonPixel btnContinuar;
        private string aviso = "";

        public FormModo()
        {
            Titulo = "¿CÓMO QUIERES JUGAR?";
            Boton("JUGAR LIBRE", 70, 418, 520, 66, Paleta.Verde, Paleta.Verde2, (s, e) => IrA(new FormRegistro(zona, dificultad)));
            Boton("MODO HISTORIA", 690, 418, 520, 66, Paleta.Rosa, Paleta.Rosa2, (s, e) => IrA(new FormHistoria()));

            Boton("RETO DEL DÍA", 40, 524, 390, 80, Paleta.Agua, Paleta.Agua2, (s, e) => IrA(new FormReto()), 14);
            Boton("TEMPORADA", 445, 524, 390, 80, Paleta.Oro, Paleta.Oro2, (s, e) => IrA(new FormTemporada()), 14);
            Boton("RÉCORDS", 850, 524, 390, 80, Paleta.LilaClaro, Paleta.Crema2, (s, e) => IrA(new FormRecords()), 14);

            btnContinuar = Boton("CONTINUAR PARTIDA", 40, 632, 420, 70, Paleta.Oro, Paleta.Oro2, Continuar);
            btnContinuar.Enabled = Guardado.Existe;
            Boton("REGRESAR", 1000, 632, 240, 70, Paleta.Crema, Paleta.Crema2, (s, e) => IrA(new FormLobby()));
        }

        private Rectangle Zona(int i) => new Rectangle(66 + i * 106, 196, 100, 72);
        private Rectangle Dificultad(int i) => new Rectangle(70 + i * 176, 318, 164, 40);

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // ----- partida libre -----
            Pixel.Panel(g, 40, 76, 580, 428, Paleta.Crema);
            Pixel.Texto(g, "PARTIDA LIBRE", 330, 108, 16, Paleta.Morado2, StringAlignment.Center);
            Pixel.Texto(g, "De 1 a 4 jugadores: solo contra bots o con amigos. Escoge la zona:", 330, 146, 10, Paleta.Noche, StringAlignment.Center);
            for (int i = 0; i < Partida.Zonas.Length; i++)
            {
                Rectangle r = Zona(i);
                Pixel.R(g, r.X - 6, r.Y - 6, r.Width + 12, r.Height + 12, i == zona ? Paleta.Oro : Paleta.Negro);
                Escenario.Dibujar(g, i, r);
                Pixel.Texto(g, Partida.Zonas[i], r.X + r.Width / 2, r.Bottom + 18, 8, i == zona ? Paleta.Rosa2 : Paleta.Morado2, StringAlignment.Center);
            }
            for (int i = 0; i < 3; i++)
            {
                Rectangle r = Dificultad(i);
                Color[] fondo = { Paleta.Verde, Paleta.Oro, Paleta.Rosa };
                Pixel.R(g, r.X - 4, r.Y - 4, r.Width + 8, r.Height + 8, i == dificultad ? Paleta.Negro : Paleta.Crema2);
                Pixel.R(g, r.X, r.Y, r.Width, r.Height, i == dificultad ? fondo[i] : Paleta.LilaClaro);
                Pixel.Texto(g, Partida.NombresDificultad[i], r.X + r.Width / 2, r.Y + r.Height / 2, 11, Paleta.Noche, StringAlignment.Center);
            }
            Pixel.Texto(g, Partida.DescripcionesDificultad[dificultad], 330, 378, 8, Paleta.Gris, StringAlignment.Center);
            Pixel.Texto(g, $"Clientes x{Partida.FactorZona[zona]:0.00} · Temporada: {Temporada.Nombres[Temporada.TemaActual]}", 330, 400, 9, Paleta.Verde2, StringAlignment.Center);

            // ----- modo historia -----
            Pixel.Panel(g, 660, 76, 580, 428, Paleta.Crema);
            Pixel.Texto(g, "MODO HISTORIA", 950, 108, 16, Paleta.Morado2, StringAlignment.Center);
            Pixel.Parrafo(g, "Un jugador. Cinco semanas, cinco zonas y cinco jefes: de un lote de tierra en Ecatepec hasta Polanco. Tus decisiones cambian el final.",
                          new Rectangle(690, 132, 520, 80), 10, Paleta.Noche, StringAlignment.Center);
            for (int i = 0; i < Historia.SEMANAS; i++)
            {
                int x = 700 + i * 106;
                if (i > 0) Pixel.R(g, x - 26, 262, 26, 6, Paleta.Morado2);      // camino (línea recta)
                Pixel.R(g, x, 226, 80, 80, Paleta.Negro);
                Pixel.R(g, x + 4, 230, 72, 72, Paleta.Morado3);
                Pixel.Sprite(g, Recursos.Clientes[Historia.RetratoJefe[i]], x + 8, 234, 4);
                Pixel.Texto(g, Partida.Zonas[i], x + 40, 326, 8, Paleta.Morado2, StringAlignment.Center);
            }
            Pixel.Parrafo(g, "Jefes: El Chato, La Chayo, Lic. Garza, el Inspector del SAT y El Bro.",
                          new Rectangle(690, 350, 520, 50), 9, Paleta.Gris, StringAlignment.Center);

            if (aviso != "")
                Pixel.Texto(g, aviso, 480, 667, 11, Paleta.Rosa);
            else if (Guardado.Existe)
                Pixel.Texto(g, "Hay una partida guardada.", 480, 667, 11, Paleta.Tenue);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            for (int i = 0; i < Partida.Zonas.Length; i++)
                if (Zona(i).Contains(e.Location))
                {
                    zona = i;
                    Recursos.Sonar("menu");
                    Invalidate();
                }
            for (int i = 0; i < 3; i++)
                if (Dificultad(i).Contains(e.Location))
                {
                    dificultad = i;
                    Recursos.Sonar("menu");
                    Invalidate();
                }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            bool mano = false;
            for (int i = 0; i < Partida.Zonas.Length; i++) if (Zona(i).Contains(e.Location)) mano = true;
            for (int i = 0; i < 3; i++) if (Dificultad(i).Contains(e.Location)) mano = true;
            Cursor = mano ? Cursors.Hand : Cursors.Default;
        }

        private void Continuar(object sender, EventArgs e)
        {
            Partida p = Guardado.Cargar();
            if (p == null)
            {
                aviso = "No se pudo abrir la partida guardada.";
                btnContinuar.Enabled = false;
                Invalidate();
                return;
            }
            IrA(new FormTablero(p));
        }
    }
}
