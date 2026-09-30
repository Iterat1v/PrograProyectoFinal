namespace PrograProyectoFinal
{
    // Tutorial de la primera partida: una foto del tablero oscurecida donde se ilumina
    // una parte a la vez con un globo que explica para qué sirve.
    public class FormTutorial : FormPixel
    {
        private const int GLOBO_W = 500, GLOBO_H = 230;

        private Bitmap foto;
        private Rectangle[] zonas;
        private string[] titulos;
        private string[] textos;
        private int paso = 0;
        private BotonPixel btnSiguiente, btnSaltar;

        internal FormTutorial(Bitmap foto, Rectangle[] zonas, string[] titulos, string[] textos)
        {
            this.foto = foto;
            this.zonas = zonas;
            this.titulos = titulos;
            this.textos = textos;
            btnSiguiente = Boton("SIGUIENTE", 0, 0, 200, 56, Paleta.Verde, Paleta.Verde2, (s, e) => Avanzar(), 11);
            btnSaltar = Boton("SALTAR", 0, 0, 160, 56, Paleta.Crema, Paleta.Crema2, (s, e) => Close(), 11);
            Acomodar();
        }

        // El globo va abajo de la zona si está arriba de la pantalla, y arriba si está abajo
        private Rectangle Globo()
        {
            Rectangle z = zonas[paso];
            int y = z.Bottom + 20 + GLOBO_H <= Pixel.ALTO ? z.Bottom + 20 : z.Top - 20 - GLOBO_H;
            if (y < 10) y = 10;
            int x = Math.Max(10, Math.Min(Pixel.ANCHO - GLOBO_W - 10, z.X + z.Width / 2 - GLOBO_W / 2));
            return new Rectangle(x, y, GLOBO_W, GLOBO_H);
        }

        private void Acomodar()
        {
            Rectangle gl = Globo();
            btnSiguiente.Location = new Point(gl.Right - 220, gl.Bottom - 72);
            btnSaltar.Location = new Point(gl.Right - 400, gl.Bottom - 72);
            btnSiguiente.Text = paso == zonas.Length - 1 ? "¡A JUGAR!" : "SIGUIENTE";
            btnSaltar.Visible = paso < zonas.Length - 1;
            Invalidate();
        }

        private void Avanzar()
        {
            Recursos.Sonar("menu");
            if (paso == zonas.Length - 1)
            {
                Close();
                return;
            }
            paso++;
            Acomodar();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Escape) Close();
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space || e.KeyCode == Keys.Right) Avanzar();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pixel.Preparar(g);
            g.DrawImage(foto, 0, 0, Pixel.ANCHO, Pixel.ALTO);
            using (SolidBrush sombra = new SolidBrush(Color.FromArgb(185, 10, 4, 20)))
                g.FillRectangle(sombra, 0, 0, Pixel.ANCHO, Pixel.ALTO);

            // la parte que se explica se ve con su color normal, con marco dorado
            Rectangle z = zonas[paso];
            Pixel.R(g, z.X - 6, z.Y - 6, z.Width + 12, z.Height + 12, Paleta.Oro);
            g.DrawImage(foto, z, z, GraphicsUnit.Pixel);

            Rectangle gl = Globo();
            Pixel.Panel(g, gl.X, gl.Y, gl.Width, gl.Height, Paleta.Crema);
            Pixel.Texto(g, titulos[paso], gl.X + 24, gl.Y + 30, 14, Paleta.Rosa2);
            Pixel.Texto(g, $"{paso + 1}/{zonas.Length}", gl.Right - 24, gl.Y + 30, 10, Paleta.Gris, StringAlignment.Far);
            Pixel.Parrafo(g, textos[paso], new Rectangle(gl.X + 24, gl.Y + 54, gl.Width - 48, 100), 11, Paleta.Noche);
        }
    }
}
