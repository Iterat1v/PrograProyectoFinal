namespace PrograProyectoFinal
{
    // Votación (una vez por partida): cada jugador escoge qué evento pasa hoy.
    // Los bots votan solos. Gana la opción con más votos.
    public class FormVotacion : FormPixel
    {
        private Partida partida;
        private int[] votos = new int[3];
        private int votante = -1;          // índice del jugador que está votando
        private bool terminada = false;
        private int ganador = -1;
        private System.Windows.Forms.Timer timerBot = new System.Windows.Forms.Timer { Interval = 800 };
        private BotonPixel btnListo;

        internal FormVotacion(Partida partida)
        {
            this.partida = partida;
            Titulo = "¡VOTACIÓN! HOY TODOS ESCOGEN EL EVENTO";
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            btnListo = Boton("¡VA!", 490, 620, 300, 76, Paleta.Verde, Paleta.Verde2, (s, e) => Close());
            btnListo.Visible = false;
            timerBot.Tick += (s, e) => { timerBot.Stop(); VotaBot(); };
            Siguiente();
        }

        private Rectangle Opcion(int k) => new Rectangle(40 + k * 410, 150, 380, 300);

        private void Siguiente()
        {
            do votante++;
            while (votante < partida.NumJugadores && partida.ObtenerJugador(votante).Rendido);

            if (votante >= partida.NumJugadores)
            {
                terminada = true;
                ganador = -1;
                int evento = partida.ResolverVotacion(votos);
                for (int k = 0; k < 3; k++)
                    if (partida.OpcionesVoto[k] == evento) ganador = k;
                Recursos.Sonar("evento");
                btnListo.Visible = true;
            }
            else if (partida.ObtenerJugador(votante).EsBot)
                timerBot.Start();
            Invalidate();
        }

        private void VotaBot()
        {
            votos[partida.Azar.Next(3)]++;
            Recursos.Sonar("carta");
            Siguiente();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (terminada || votante >= partida.NumJugadores || partida.ObtenerJugador(votante).EsBot) return;
            for (int k = 0; k < 3; k++)
                if (Opcion(k).Contains(e.Location))
                {
                    votos[k]++;
                    Recursos.Sonar("carta");
                    Siguiente();
                    return;
                }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            if (!terminada)
            {
                Concesionario j = partida.ObtenerJugador(votante);
                Pixel.R(g, 40, 76, 64, 64, Paleta.Morado3);
                Pixel.Sprite(g, Recursos.Cara(j), 40, 76, 4);
                Pixel.Texto(g, j.EsBot ? $"{j.Jugador} está votando..." : $"VOTA {j.Jugador.ToUpper()} (que nadie vea)", 120, 108, 15, Paleta.Oro);
            }
            else
                Pixel.Texto(g, $"GANÓ: {Partida.NombresEvento[partida.Evento]}", 640, 108, 18, Paleta.Oro, StringAlignment.Center);

            for (int k = 0; k < 3; k++)
            {
                Rectangle r = Opcion(k);
                int ev = partida.OpcionesVoto[k];
                bool gano = terminada && k == ganador;
                Pixel.Panel(g, r.X, r.Y, r.Width, r.Height, gano ? Paleta.Oro : Paleta.Crema);
                if (gano) Pixel.R(g, r.X + 10, r.Y + 10, r.Width - 20, r.Height - 20, Paleta.Crema);
                Pixel.Texto(g, $"{k + 1}", r.X + 36, r.Y + 44, 20, Paleta.Rosa2, StringAlignment.Center);
                Pixel.Texto(g, Partida.NombresEvento[ev], r.X + r.Width / 2 + 20, r.Y + 44, 13, Paleta.Noche, StringAlignment.Center);
                Pixel.Parrafo(g, Partida.DescripcionesEvento[ev], new Rectangle(r.X + 24, r.Y + 86, r.Width - 48, 150), 11, Paleta.Morado2, StringAlignment.Center);
                if (terminada)
                {
                    Pixel.R(g, r.X + 24, r.Bottom - 50, (r.Width - 48) * votos[k] / Math.Max(1, partida.Activos()), 24, Paleta.Agua);
                    Pixel.Texto(g, $"{votos[k]} voto{(votos[k] == 1 ? "" : "s")}", r.X + r.Width / 2, r.Bottom - 38, 11, Paleta.Noche, StringAlignment.Center);
                }
            }
            if (!terminada)
                Pixel.Texto(g, "Toca el evento que quieres. Los votos se ven al final.", 640, 520, 12, Paleta.Tenue, StringAlignment.Center);
        }
    }
}
