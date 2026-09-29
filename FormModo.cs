namespace PrograProyectoFinal
{
    // ¿Cómo quieres jugar? Partida libre (con zona), modo historia o continuar la guardada
    public class FormModo : FormPixel
    {
        private int zona = 0;
        private BotonPixel btnContinuar;
        private string aviso = "";

        public FormModo()
        {
            Titulo = "¿CÓMO QUIERES JUGAR?";
            Boton("JUGAR LIBRE", 70, 530, 520, 72, Paleta.Verde, Paleta.Verde2, (s, e) => IrA(new FormRegistro(zona)));
            Boton("MODO HISTORIA", 690, 530, 520, 72, Paleta.Rosa, Paleta.Rosa2, (s, e) => IrA(new FormHistoria()));
            btnContinuar = Boton("CONTINUAR PARTIDA", 40, 636, 420, 72, Paleta.Oro, Paleta.Oro2, Continuar);
            btnContinuar.Enabled = Guardado.Existe;
            Boton("VOLVER", 1000, 636, 240, 72, Paleta.Crema, Paleta.Crema2, (s, e) => VolverAlTitulo());
        }

        private Rectangle Zona(int i) => new Rectangle(66 + i * 106, 250, 100, 76);

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // ----- partida libre -----
            Pixel.Panel(g, 40, 84, 580, 540, Paleta.Crema);
            Pixel.Texto(g, "PARTIDA LIBRE", 330, 124, 17, Paleta.Morado2, StringAlignment.Center);
            Pixel.Parrafo(g, "De 2 a 4 jugadores en la misma compu o contra bots. Escoge dónde está tu lote:",
                          new Rectangle(70, 152, 520, 80), 11, Paleta.Noche, StringAlignment.Center);
            for (int i = 0; i < Partida.Zonas.Length; i++)
            {
                Rectangle r = Zona(i);
                Pixel.R(g, r.X - 6, r.Y - 6, r.Width + 12, r.Height + 12, i == zona ? Paleta.Oro : Paleta.Negro);
                Escenario.Dibujar(g, i, r);
                Pixel.Texto(g, Partida.Zonas[i], r.X + r.Width / 2, r.Bottom + 22, 8, i == zona ? Paleta.Rosa2 : Paleta.Morado2, StringAlignment.Center);
            }
            Pixel.Parrafo(g, Escenario.Descripciones[zona], new Rectangle(70, 380, 520, 60), 11, Paleta.Gris, StringAlignment.Center);
            Pixel.Texto(g, $"Los clientes traen x{Partida.FactorZona[zona]:0.00}", 330, 460, 11, Paleta.Verde2, StringAlignment.Center);

            // ----- modo historia -----
            Pixel.Panel(g, 660, 84, 580, 540, Paleta.Crema);
            Pixel.Texto(g, "MODO HISTORIA", 950, 124, 17, Paleta.Morado2, StringAlignment.Center);
            Pixel.Parrafo(g, "Un jugador. Cinco semanas, cinco zonas y cinco jefes: de un lote de tierra en Ecatepec hasta Polanco. Tus decisiones cambian el final.",
                          new Rectangle(690, 152, 520, 120), 11, Paleta.Noche, StringAlignment.Center);
            for (int i = 0; i < Historia.SEMANAS; i++)
            {
                int x = 700 + i * 106;
                if (i > 0) Pixel.R(g, x - 26, 330, 26, 6, Paleta.Morado2);      // camino (línea recta)
                Pixel.R(g, x, 296, 80, 80, Paleta.Negro);
                Pixel.R(g, x + 4, 300, 72, 72, Paleta.Morado3);
                Pixel.Sprite(g, Recursos.Clientes[Historia.RetratoJefe[i]], x + 8, 304, 4);
                Pixel.Texto(g, Partida.Zonas[i], x + 40, 398, 8, Paleta.Morado2, StringAlignment.Center);
            }
            Pixel.Parrafo(g, "Jefes: El Chato, La Chayo, Lic. Garza, el Inspector del SAT y El Bro.",
                          new Rectangle(690, 430, 520, 70), 10, Paleta.Gris, StringAlignment.Center);

            if (aviso != "")
                Pixel.Texto(g, aviso, 480, 672, 11, Paleta.Rosa);
            else if (Guardado.Existe)
                Pixel.Texto(g, "Hay una partida guardada.", 480, 672, 11, Paleta.Tenue);
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
