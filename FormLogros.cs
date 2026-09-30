namespace PrograProyectoFinal
{
    // Misiones del día y los 18 logros con su medalla
    public class FormLogros : FormPixel
    {
        private const int COLUMNAS = 6;
        private Perfil perfil;

        internal FormLogros(Perfil perfil)
        {
            this.perfil = perfil;
            perfil.PrepararMisiones(Reto.Hoy);
            Titulo = $"LOGROS Y MISIONES · {perfil.Nombre.ToUpper()}";
            BotonRegresar((s, e) => IrA(new FormLobby()));
        }

        private Rectangle Casilla(int i) => new Rectangle(40 + (i % COLUMNAS) * 202, 292 + (i / COLUMNAS) * 142, 194, 134);

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // ----- misiones -----
            Pixel.Panel(g, 40, 72, 1200, 200, Paleta.Crema);
            Pixel.Texto(g, "MISIONES DE HOY", 64, 102, 14, Paleta.Rosa2);
            Pixel.Texto(g, $"+{Misiones.MONEDAS} monedas y +{Misiones.XP} XP cada una · las 3: +{Misiones.BONO_TODAS} extra", 1216, 102, 9, Paleta.Gris, StringAlignment.Far);
            int[] hoy = Misiones.DelDia(Reto.Hoy);
            for (int k = 0; k < Misiones.POR_DIA; k++)
            {
                int x = 64 + k * 392;
                int meta = Misiones.Metas[hoy[k]];
                int avance = Math.Min(meta, perfil.Progreso(k));
                bool lista = perfil.Cobrada(k);
                Pixel.R(g, x, 128, 372, 126, lista ? Pixel.Aclarar(Paleta.Verde, 0.6f) : Paleta.LilaClaro);
                Pixel.Parrafo(g, Misiones.Textos[hoy[k]], new Rectangle(x + 14, 140, 344, 56), 12, Paleta.Noche);
                Pixel.R(g, x + 14, 206, 280, 20, Paleta.Negro);
                Pixel.R(g, x + 18, 210, 272 * avance / meta, 12, lista ? Paleta.Verde : Paleta.Oro);
                Pixel.Texto(g, lista ? "¡LISTA!" : $"{avance}/{meta}", x + 356, 216, 11, lista ? Paleta.Verde2 : Paleta.Morado2, StringAlignment.Far);
            }

            // ----- logros -----
            Pixel.Texto(g, $"LOGROS  {perfil.CuantosLogros()}/{Logros.CANTIDAD}", 40, 282, 12, Paleta.Oro);
            Pixel.Texto(g, $"+{Logros.MONEDAS} monedas cada uno", 1240, 282, 9, Paleta.Tenue, StringAlignment.Far);
            for (int i = 0; i < Logros.CANTIDAD; i++)
            {
                Rectangle c = Casilla(i);
                bool tiene = perfil.TieneLogro(i);
                Pixel.Panel(g, c.X, c.Y + 10, c.Width, c.Height - 10, tiene ? Paleta.Crema : Paleta.Crema2);
                DibujarMedalla(g, c.X + 14, c.Y + 26, tiene, i);
                Pixel.Parrafo(g, Logros.Nombres[i], new Rectangle(c.X + 68, c.Y + 22, c.Width - 76, 40), 8, tiene ? Paleta.Noche : Paleta.Gris);
                Pixel.Parrafo(g, Logros.Descripciones[i], new Rectangle(c.X + 12, c.Y + 74, c.Width - 20, 56), 7, tiene ? Paleta.Morado2 : Paleta.Gris);
            }
        }

        // Medalla pixel: listón y círculo (cuadros) dorado si ya lo tienes, gris si no
        private static void DibujarMedalla(Graphics g, int x, int y, bool tiene, int numero)
        {
            Color liston = tiene ? Paleta.Rosa : Paleta.Gris;
            Color metal = tiene ? Paleta.Oro : Paleta.Tenue;
            Color borde = tiene ? Paleta.Oro2 : Paleta.Gris;
            Pixel.R(g, x + 12, y, 8, 14, liston);
            Pixel.R(g, x + 24, y, 8, 14, liston);
            Pixel.R(g, x + 8, y + 14, 28, 32, borde);
            Pixel.R(g, x + 4, y + 18, 36, 24, borde);
            Pixel.R(g, x + 12, y + 18, 20, 24, metal);
            Pixel.R(g, x + 8, y + 22, 28, 16, metal);
            Pixel.Texto(g, tiene ? $"{numero + 1}" : "?", x + 22, y + 30, 9, Paleta.Noche, StringAlignment.Center);
        }
    }
}
