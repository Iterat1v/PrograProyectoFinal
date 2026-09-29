namespace PrograProyectoFinal
{
    // Pantalla 4: el podio con el ranking final (ordenado con burbuja)
    public partial class FormResultados : FormPixel
    {
        private Partida partida;
        private Concesionario[] ranking;
        private string[] recompensas = new string[4];
        private bool[] subio = new bool[4];
        private string titulo;
        private bool ganoSemana = false;        // modo historia

        internal FormResultados(Partida partida)
        {
            InitializeComponent();
            this.partida = partida;
            ranking = partida.Ranking();
            DarRecompensas();
            Guardado.Borrar();          // la partida ya terminó: ya no hay qué continuar
            btnRevancha.Colores(Paleta.Oro, Paleta.Oro2);
            btnNueva.Colores(Paleta.Verde, Paleta.Verde2);
            btnSalir.Colores(Paleta.Rosa, Paleta.Rosa2);
            titulo = $"¡GANÓ {ranking[0].Jugador.ToUpper()}!";

            // Modo historia: ¿le ganaste al jefe?
            Historia h = partida.Historia;
            if (h != null)
            {
                Concesionario tu = null;
                for (int i = 0; i < partida.NumJugadores; i++)
                    if (!partida.ObtenerJugador(i).EsBot) tu = partida.ObtenerJugador(i);
                ganoSemana = tu != null && !tu.Rendido && ranking[0] == tu;
                if (ganoSemana)
                {
                    h.Reputacion = tu.Reputacion;
                    h.GanoSemana();
                    h.Guardar();
                    titulo = h.Terminada ? "¡ERES EL REY DE LA CDMX!" : $"¡GANASTE LA SEMANA {h.Semana}!";
                    btnRevancha.Text = h.Terminada ? "VER EL FINAL" : "SIGUIENTE SEMANA";
                }
                else
                {
                    titulo = "PERDISTE LA SEMANA";
                    btnRevancha.Text = "REINTENTAR";
                }
                btnNueva.Text = "VER EL MAPA";
            }

            // La mejor estafa de la partida se puede volver a ver
            if (partida.MejorEstafaUsuario != null)
                Boton("REPETICIÓN", 40, 440, 250, 60, Paleta.Rosa, Paleta.Rosa2,
                      (s, e) => { using (FormRepeticion r = new FormRepeticion(partida)) r.ShowDialog(this); }, 11);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pixel.Preparar(g);

            Pixel.R(g, 0, 0, Pixel.ANCHO, Pixel.ALTO, Paleta.Noche);
            Pixel.Estrellas(g, 50, 3);

            // confeti
            Color[] colores = { Paleta.Rosa, Paleta.Agua, Paleta.Oro, Paleta.Verde };
            double s = 11;
            for (int i = 0; i < 60; i++)
            {
                s = (s * 9301 + 49297) % 233280; int x = (int)(s / 233280 * 640) * 2;
                s = (s * 9301 + 49297) % 233280; int y = (int)(s / 233280 * 240) * 2;
                Pixel.R(g, x, y, 8, 8, colores[i % 4]);
            }

            Pixel.SombraTexto(g, titulo, 640, 80, titulo.Length > 16 ? 48 : 64, Paleta.Oro, Paleta.Rosa);
            if (partida.MejorEstafaUsuario != null)
                Pixel.Texto(g, $"Mejor estafa: {partida.MejorEstafaUsuario.Jugador} ({Pixel.DineroCorto(partida.MejorEstafaPerdida)})", 40, 420, 9, Paleta.Rosa);

            // podio: {lugar, x, alto}
            int[,] podio = { { 1, 380, 220 }, { 0, 540, 300 }, { 2, 700, 160 } };
            Color[] bloque = { Paleta.Oro, Paleta.Crema2, Paleta.Bronce };
            for (int p = 0; p < 3; p++)
            {
                int lugar = podio[p, 0], x = podio[p, 1], h = podio[p, 2];
                if (lugar >= ranking.Length) continue;
                int y = 600 - h;
                Concesionario j = ranking[lugar];

                Pixel.R(g, x, y, 160, h, Paleta.Negro);
                Pixel.R(g, x + 6, y + 6, 148, h - 6, bloque[lugar]);
                Pixel.Texto(g, $"{lugar + 1}", x + 80, y + 52, 24, Paleta.Noche, StringAlignment.Center);
                Pixel.Sprite(g, Recursos.Cara(j), x + 16, y - 132, 8);
                Pixel.Texto(g, j.Jugador, x + 80, y + 104, 12, Paleta.Noche, StringAlignment.Center);
                Pixel.Texto(g, j.Rendido ? "se rindió" : Pixel.Dinero(j.Patrimonio()), x + 80, y + 140, 11, Paleta.Noche, StringAlignment.Center);
            }
            if (ranking.Length == 4)
                Pixel.Texto(g, $"4. {ranking[3].Jugador}  {(ranking[3].Rendido ? "se rindió" : Pixel.Dinero(ranking[3].Patrimonio()))}", 40, 560, 11, Paleta.Tenue);

            // Recompensas de cada persona
            int fila = 0;
            for (int i = 0; i < ranking.Length; i++)
            {
                if (recompensas[i] == null) continue;
                if (fila == 0)
                    Pixel.Texto(g, "RECOMPENSAS", 890, 200, 12, Paleta.Agua);
                int y = 244 + fila * 84;
                Pixel.Texto(g, ranking[i].Jugador, 890, y, 11, Paleta.Crema);
                Pixel.Texto(g, recompensas[i], 890, y + 28, 10, Paleta.Oro);
                if (subio[i])
                    Pixel.Texto(g, $"¡SUBISTE A NIVEL {ranking[i].Perfil.Nivel}!", 890, y + 54, 10, Paleta.Rosa);
                fila++;
            }

            // Estadísticas: búsqueda lineal del mejor en cada cosa
            Concesionario masVentas = ranking[0], mejorVenta = ranking[0];
            for (int i = 1; i < ranking.Length; i++)
            {
                if (ranking[i].Ventas > masVentas.Ventas) masVentas = ranking[i];
                if (ranking[i].MejorVenta > mejorVenta.MejorVenta) mejorVenta = ranking[i];
            }
            if (masVentas.Ventas > 0)
            {
                Pixel.Texto(g, "ESTADÍSTICAS", 40, 200, 12, Paleta.Agua);
                Pixel.Texto(g, "Más ventas:", 40, 244, 10, Paleta.Tenue);
                Pixel.Texto(g, $"{masVentas.Jugador} ({masVentas.Ventas})", 40, 274, 11, Paleta.Crema);
                Pixel.Texto(g, "Mejor venta:", 40, 318, 10, Paleta.Tenue);
                Pixel.Texto(g, mejorVenta.Jugador, 40, 348, 11, Paleta.Crema);
                Pixel.Texto(g, $"{mejorVenta.MejorVentaAuto} {Pixel.DineroCorto(mejorVenta.MejorVenta)}", 40, 376, 10, Paleta.Oro);
            }

            Pixel.R(g, 0, 600, Pixel.ANCHO, 120, Paleta.Piso);
            Pixel.R(g, 0, 600, Pixel.ANCHO, 6, Paleta.Negro);
        }

        // XP y monedas para cada persona (los bots no tienen perfil). Se guarda en el archivo.
        private void DarRecompensas()
        {
            int[] porLugar = { 60, 35, 20, 10 };
            for (int i = 0; i < ranking.Length; i++)
            {
                Concesionario j = ranking[i];
                if (j.Perfil == null) continue;
                int xp, monedas;
                if (j.Rendido)
                {
                    xp = 10;
                    monedas = 5;
                }
                else
                {
                    xp = 40 + j.Ventas * 15 + (i == 0 ? 100 : 0);
                    monedas = 25 + j.Ventas * 10 + porLugar[i] + (int)Math.Max(0, (j.Patrimonio() - Concesionario.PRESUPUESTO_INICIAL) / 10000);
                }
                subio[i] = j.Perfil.Ganar(xp, monedas);
                recompensas[i] = $"+{xp} XP  +{monedas} monedas";
            }
            Perfiles.Guardar();
        }

        private void btnRevancha_Click(object sender, EventArgs e)
        {
            Historia h = partida.Historia;
            if (h == null)
                IrA(new FormDraft(partida.Revancha()));
            else if (h.Terminada)
                IrA(new FormFinal(h));
            else if (ganoSemana)
                IrA(new FormHistoria(h));
            else
                IrA(new FormDraft(partida.Revancha()));     // reintentar la misma semana
        }

        private void btnNueva_Click(object sender, EventArgs e)
        {
            if (partida.Historia != null)
            {
                IrA(new FormHistoria(partida.Historia));
                return;
            }
            IrA(new FormRegistro());
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
