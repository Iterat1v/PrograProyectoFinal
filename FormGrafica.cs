namespace PrograProyectoFinal
{
    // Gráfica del patrimonio de cada jugador al terminar cada día
    public class FormGrafica : FormPixel
    {
        private static readonly Color[] colores = { Paleta.Rosa, Paleta.Agua, Paleta.Oro, Paleta.Verde };
        private static readonly Rectangle area = new Rectangle(160, 110, 820, 470);

        private Partida partida;

        internal FormGrafica(Partida partida)
        {
            this.partida = partida;
            Titulo = "¿CÓMO LE FUE A CADA QUIEN?";
            Boton("CERRAR", 1000, 632, 240, 70, Paleta.Verde, Paleta.Verde2, (s, e) => Close());
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            int dias = partida.DiasRegistrados;

            Pixel.Panel(g, 40, 76, 980, 546, Paleta.Crema);

            // búsqueda del mínimo y el máximo para la escala
            double min = double.MaxValue, max = double.MinValue;
            for (int j = 0; j < partida.NumJugadores; j++)
                for (int d = 0; d < dias; d++)
                {
                    double v = partida.ObtenerJugador(j).Historial[d];
                    if (v < min) min = v;
                    if (v > max) max = v;
                }
            if (dias == 0) { min = 0; max = 1; }
            if (max - min < 50000) { max += 25000; min -= 25000; }

            // rejilla: 5 líneas horizontales con su cantidad
            for (int k = 0; k <= 4; k++)
            {
                int y = area.Bottom - k * area.Height / 4;
                Pixel.R(g, area.X, y, area.Width, 2, Paleta.Crema2);
                Pixel.Texto(g, Pixel.DineroCorto(min + (max - min) * k / 4), area.X - 12, y, 9, Paleta.Gris, StringAlignment.Far);
            }
            for (int d = 0; d <= Partida.DIAS; d++)
            {
                int x = X(d);
                Pixel.R(g, x, area.Y, 2, area.Height, Paleta.Crema2);
                Pixel.Texto(g, d == 0 ? "INICIO" : $"DÍA {d}", x, area.Bottom + 22, 9, Paleta.Morado2, StringAlignment.Center);
            }

            // una línea por jugador con cuadritos en cada día
            for (int j = 0; j < partida.NumJugadores; j++)
            {
                Concesionario c = partida.ObtenerJugador(j);
                using (Pen pluma = new Pen(colores[j], 5))
                    for (int d = 1; d < dias; d++)
                        g.DrawLine(pluma, X(d - 1), Y(c.Historial[d - 1], min, max), X(d), Y(c.Historial[d], min, max));
                for (int d = 0; d < dias; d++)
                {
                    int x = X(d), y = Y(c.Historial[d], min, max);
                    Pixel.R(g, x - 7, y - 7, 14, 14, Paleta.Negro);
                    Pixel.R(g, x - 5, y - 5, 10, 10, colores[j]);
                }
            }

            // leyenda
            Pixel.Panel(g, 1040, 76, 200, 546, Paleta.Morado2, Paleta.Negro);
            Pixel.Texto(g, "JUGADORES", 1060, 108, 11, Paleta.Agua);
            for (int j = 0; j < partida.NumJugadores; j++)
            {
                Concesionario c = partida.ObtenerJugador(j);
                int y = 140 + j * 110;
                Pixel.R(g, 1060, y, 20, 20, colores[j]);
                Pixel.Texto(g, c.Jugador, 1090, y + 10, 11, Paleta.Crema);
                Pixel.Sprite(g, Recursos.Cara(c), 1060, y + 30, 3);
                Pixel.Texto(g, dias > 0 ? Pixel.DineroCorto(c.Historial[dias - 1]) : "-", 1114, y + 54, 11, Paleta.Oro);
            }
            Pixel.Texto(g, "Patrimonio = dinero + autos − deudas, al terminar cada día.", 40, 667, 10, Paleta.Tenue);
        }

        private static int X(int dia) => area.X + dia * area.Width / Partida.DIAS;

        private static int Y(double valor, double min, double max) =>
            area.Bottom - (int)((valor - min) / (max - min) * area.Height);
    }
}
