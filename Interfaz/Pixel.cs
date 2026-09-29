using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace PrograProyectoFinal
{
    // Piezas de dibujo del estilo pixel: rectángulos, paneles, texto, sprites.
    // Todas las pantallas miden 1280 x 720 (el mockup era 640 x 360 agrandado x2).
    internal static class Pixel
    {
        public const int ANCHO = 1280;
        public const int ALTO = 720;

        public static void Preparar(Graphics g)
        {
            g.InterpolationMode = InterpolationMode.NearestNeighbor;   // pixel nítido, no borroso
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.SmoothingMode = SmoothingMode.None;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        }

        public static void R(Graphics g, int x, int y, int w, int h, Color color)
        {
            if (w <= 0 || h <= 0) return;
            using (SolidBrush brocha = new SolidBrush(color))
                g.FillRectangle(brocha, x, y, w, h);
        }

        // Panel con las esquinas "mordidas" de pixel art
        public static void Panel(Graphics g, int x, int y, int w, int h, Color relleno, Color borde)
        {
            R(g, x + 8, y, w - 16, h, borde);
            R(g, x, y + 8, w, h - 16, borde);
            R(g, x + 4, y + 4, w - 8, h - 8, borde);
            R(g, x + 8, y + 6, w - 16, h - 12, relleno);
            R(g, x + 6, y + 8, w - 12, h - 16, relleno);
        }

        public static void Panel(Graphics g, int x, int y, int w, int h, Color relleno)
        {
            Panel(g, x, y, w, h, relleno, Paleta.Negro);
        }

        // Texto centrado en "y" (como textBaseline = middle del mockup)
        public static void Texto(Graphics g, string texto, int x, int y, float tam, Color color,
                                 StringAlignment alineacion = StringAlignment.Near)
        {
            Escribir(g, texto, Recursos.Letra(tam), x, y, color, alineacion);
        }

        public static void Escribir(Graphics g, string texto, Font letra, int x, int y, Color color,
                                    StringAlignment alineacion)
        {
            using (StringFormat formato = new StringFormat(StringFormat.GenericTypographic))
            using (SolidBrush brocha = new SolidBrush(color))
            {
                formato.Alignment = alineacion;
                formato.LineAlignment = StringAlignment.Center;
                formato.FormatFlags |= StringFormatFlags.NoWrap;
                g.DrawString(texto, letra, brocha, x, y, formato);
            }
        }

        // Texto que se acomoda en varias líneas dentro de un rectángulo
        public static void Parrafo(Graphics g, string texto, Rectangle area, float tam, Color color,
                                   StringAlignment alineacion = StringAlignment.Near)
        {
            using (StringFormat formato = new StringFormat())
            using (SolidBrush brocha = new SolidBrush(color))
            {
                formato.Alignment = alineacion;
                formato.LineAlignment = StringAlignment.Near;
                g.DrawString(texto, Recursos.Letra(tam), brocha, area, formato);
            }
        }

        // Título grande con sombra (CONCESIONARIO, ¡GANÓ!)
        public static void SombraTexto(Graphics g, string texto, int x, int y, float pixeles, Color color, Color sombra)
        {
            Font letra = Recursos.Titulo(pixeles);
            Escribir(g, texto, letra, x + 6, y + 6, sombra, StringAlignment.Center);
            Escribir(g, texto, letra, x, y, color, StringAlignment.Center);
        }

        // Dibuja un sprite agrandado "escala" veces, sin suavizar
        public static void Sprite(Graphics g, Image imagen, int x, int y, float escala)
        {
            if (imagen == null) return;
            g.DrawImage(imagen, x, y, imagen.Width * escala, imagen.Height * escala);
        }

        // Fondo morado con rayitas horizontales
        public static void Fondo(Graphics g)
        {
            R(g, 0, 0, ANCHO, ALTO, Paleta.Morado);
            for (int y = 0; y < ALTO; y += 8)
                if ((y / 8) % 2 == 1)
                    R(g, 0, y, ANCHO, 2, Paleta.Raya);
        }

        // Barra de título aqua con — ▢ ✕
        public static void Barra(Graphics g, string titulo)
        {
            R(g, 0, 0, ANCHO, 60, Paleta.Agua2);
            R(g, 0, 0, ANCHO, 54, Paleta.Agua);
            R(g, 0, 54, ANCHO, 6, Paleta.Negro);
            Texto(g, titulo, 28, 28, 15, Paleta.Noche);

            // minimizar
            R(g, 1140, 30, 22, 4, Paleta.Noche);
            // maximizar (solo de adorno)
            R(g, 1182, 16, 22, 4, Paleta.Noche); R(g, 1182, 36, 22, 4, Paleta.Noche);
            R(g, 1182, 16, 4, 24, Paleta.Noche); R(g, 1200, 16, 4, 24, Paleta.Noche);
            // cerrar
            for (int i = 0; i < 22; i += 2)
            {
                R(g, 1228 + i, 16 + i, 4, 4, Paleta.Noche);
                R(g, 1248 - i, 16 + i, 4, 4, Paleta.Noche);
            }
        }

        // Estrellas "al azar" pero siempre en el mismo lugar (misma semilla que el mockup)
        public static void Estrellas(Graphics g, int cuantas, int semilla)
        {
            double s = semilla;
            for (int i = 0; i < cuantas; i++)
            {
                s = (s * 9301 + 49297) % 233280; int x = (int)(s / 233280 * 640) * 2;
                s = (s * 9301 + 49297) % 233280; int y = (int)(s / 233280 * 170) * 2;
                s = (s * 9301 + 49297) % 233280; Color c = s / 233280 > 0.7 ? Paleta.Oro : Paleta.Crema;
                R(g, x, y, 4, 4, c);
            }
        }

        // Carta boca abajo con el "?"
        public static void CartaBocaAbajo(Graphics g, int x, int y, int w, int h)
        {
            Panel(g, x, y, w, h, Paleta.Morado3);
            R(g, x + 12, y + 12, w - 24, h - 24, Paleta.Morado2);
            Color punto = ColorTranslator.FromHtml("#4a1c86");
            for (int i = 0; i < w - 24; i += 12)
                for (int j = 0; j < h - 24; j += 12)
                    if (((i + j) / 12) % 2 == 0)
                        R(g, x + 12 + i, y + 12 + j, 6, 6, punto);
            Texto(g, "?", x + w / 2, y + h / 2, h < 100 ? 14 : 22, Paleta.Oro, StringAlignment.Center);
        }

        // Globo de diálogo con piquito a la izquierda
        public static void Globo(Graphics g, int x, int y, int w, int h)
        {
            Panel(g, x, y, w, h, Paleta.Crema);
            R(g, x - 16, y + h / 2 - 8, 20, 16, Paleta.Negro);
            R(g, x - 10, y + h / 2 - 4, 16, 8, Paleta.Crema);
        }

        public static Color Aclarar(Color c, float cuanto)
        {
            return Color.FromArgb(c.R + (int)((255 - c.R) * cuanto),
                                  c.G + (int)((255 - c.G) * cuanto),
                                  c.B + (int)((255 - c.B) * cuanto));
        }

        public static string Dinero(double cantidad)
        {
            return $"${cantidad:N0}";
        }

        public static string DineroCorto(double cantidad)
        {
            return $"${cantidad / 1000:N0}k";
        }
    }
}
