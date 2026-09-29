namespace PrograProyectoFinal
{
    // Subasta en secreto: cada jugador ofrece sin que los demás vean; gana la oferta más alta
    public class FormSubasta : FormPixel
    {
        private Partida partida;
        private Auto clasico;
        private double[] ofertas = new double[Partida.MAX_JUGADORES];
        private int postor = -1;
        private double oferta = 0;
        private bool terminada = false;
        private string resultado = "";
        private BotonPixel[] controles;
        private BotonPixel btnListo;

        public string Resultado => resultado;

        internal FormSubasta(Partida partida)
        {
            this.partida = partida;
            clasico = partida.AutoSubasta;
            Titulo = "¡SUBASTA! OFERTAS EN SECRETO";
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            controles = new[]
            {
                Boton("-50K", 240, 480, 120, 62, Paleta.LilaClaro, Paleta.Crema2, (s, e) => Cambiar(-50000), 11),
                Boton("-10K", 370, 480, 120, 62, Paleta.LilaClaro, Paleta.Crema2, (s, e) => Cambiar(-10000), 11),
                Boton("+10K", 790, 480, 120, 62, Paleta.LilaClaro, Paleta.Crema2, (s, e) => Cambiar(10000), 11),
                Boton("+50K", 920, 480, 120, 62, Paleta.LilaClaro, Paleta.Crema2, (s, e) => Cambiar(50000), 11),
                Boton("OFRECER", 330, 600, 300, 76, Paleta.Verde, Paleta.Verde2, (s, e) => Ofrecer(oferta)),
                Boton("NO QUIERO", 650, 600, 300, 76, Paleta.Crema, Paleta.Crema2, (s, e) => Ofrecer(0))
            };
            btnListo = Boton("¡VA!", 490, 620, 300, 76, Paleta.Verde, Paleta.Verde2, (s, e) => Close());
            btnListo.Visible = false;
            Siguiente();
        }

        private void Cambiar(double cuanto)
        {
            oferta = Math.Max(0, Math.Min(partida.ObtenerJugador(postor).Dinero, oferta + cuanto));
            Invalidate();
        }

        private void Ofrecer(double cantidad)
        {
            ofertas[postor] = cantidad;
            Recursos.Sonar("carta");
            Siguiente();
        }

        private void Siguiente()
        {
            // Los bots ofrecen solos; se pasa a la siguiente persona
            do
            {
                postor++;
                if (postor < partida.NumJugadores && partida.ObtenerJugador(postor).EsBot && !partida.ObtenerJugador(postor).Rendido)
                    ofertas[postor] = partida.OfertaBot(partida.ObtenerJugador(postor));
            }
            while (postor < partida.NumJugadores && (partida.ObtenerJugador(postor).EsBot || partida.ObtenerJugador(postor).Rendido));

            if (postor >= partida.NumJugadores)
            {
                Concesionario ganador = partida.ResolverSubasta(ofertas, out double pago);
                resultado = ganador == null ? "Nadie ofreció lo suficiente. El clásico se fue a otra subasta."
                                            : $"¡{ganador.Jugador} se llevó el {clasico.Nombre} por {Pixel.Dinero(pago)}!";
                terminada = true;
                foreach (BotonPixel b in controles) b.Visible = false;
                btnListo.Visible = true;
                Recursos.Sonar(ganador == null ? "error" : "venta");
            }
            else
                oferta = Math.Round(clasico.ValorActual() * 0.5 / 10000) * 10000;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            Pixel.Panel(g, 40, 80, 1200, 300, Paleta.Crema);
            Pixel.R(g, 70, 110, 380, 240, Paleta.Morado3);
            Pixel.Sprite(g, Recursos.Autos[clasico.Tipo, clasico.Color], 100, 160, 10);
            Pixel.Texto(g, $"{clasico.Nombre} · CLÁSICO", 490, 140, 18, Paleta.Rosa2);
            Pixel.Texto(g, $"{clasico.NombreTipo} · tuneado de fábrica", 490, 186, 12, Paleta.Morado2);
            Pixel.Texto(g, $"Vale {Pixel.Dinero(clasico.ValorActual())}", 490, 230, 16, Paleta.Verde2);
            Pixel.Parrafo(g, "Cada quien ofrece en secreto. Gana la oferta más alta y paga lo que ofreció. Los clientes pagan muy bien por un clásico.",
                          new Rectangle(490, 262, 720, 100), 11, Paleta.Noche);

            if (!terminada)
            {
                Concesionario j = partida.ObtenerJugador(postor);
                Pixel.Texto(g, $"LE TOCA A {j.Jugador.ToUpper()} (que nadie vea) · tienes {Pixel.Dinero(j.Dinero)}", 640, 420, 13, Paleta.Oro, StringAlignment.Center);
                Pixel.Texto(g, Pixel.Dinero(oferta), 640, 510, 22, Paleta.Crema, StringAlignment.Center);
            }
            else
            {
                Pixel.Texto(g, resultado, 640, 420, 14, Paleta.Oro, StringAlignment.Center);
                string todas = "";
                for (int i = 0; i < partida.NumJugadores; i++)
                    todas += $"{partida.ObtenerJugador(i).Jugador}: {Pixel.DineroCorto(ofertas[i])}    ";
                Pixel.Texto(g, todas.Trim(), 640, 480, 11, Paleta.Tenue, StringAlignment.Center);
            }
        }
    }
}
