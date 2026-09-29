using System.ComponentModel;

namespace PrograProyectoFinal
{
    // Botón de juego: plano, con sombra negra y franja oscura abajo.
    // Después de compilar aparece en el Cuadro de herramientas.
    public class BotonPixel : Button
    {
        private bool encima = false;
        private bool presionado = false;

        [Category("Pixel"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] public Color ColorFondo { get; set; } = Paleta.Crema;
        [Category("Pixel"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] public Color ColorSombra { get; set; } = Paleta.Crema2;
        [Category("Pixel"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] public Color ColorTexto { get; set; } = Paleta.Negro;
        [Category("Pixel"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] public float TamLetra { get; set; } = 13;

        public BotonPixel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.ResizeRedraw, true);
            BackColor = Color.Transparent;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Cursor = Cursors.Hand;
        }

        // Pone los tres colores de una vez
        public void Colores(Color fondo, Color sombra)
        {
            ColorFondo = fondo;
            ColorSombra = sombra;
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e) { encima = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { encima = false; presionado = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { presionado = true; Invalidate(); base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { presionado = false; Invalidate(); base.OnMouseUp(e); }
        protected override void OnEnabledChanged(EventArgs e) { Invalidate(); base.OnEnabledChanged(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pixel.Preparar(g);

            // El control incluye la sombra: 4 px a la derecha y 6 px abajo
            int w = Width - 4;
            int h = Height - 6;
            int d = presionado ? 3 : 0;

            Color fondo = ColorFondo, sombra = ColorSombra, texto = ColorTexto;
            if (!Enabled)
            {
                fondo = Paleta.Gris; sombra = Paleta.Gris2; texto = Paleta.Crema2;
            }
            else if (encima)
            {
                fondo = Pixel.Aclarar(fondo, 0.18f);
            }

            if (!presionado)
                Pixel.R(g, 4, 6, w, h, Paleta.Negro);
            Pixel.Panel(g, d, d, w, h, fondo);
            Pixel.R(g, d + 8, d + h - 14, w - 16, 6, sombra);
            Pixel.Texto(g, Text, d + w / 2, d + h / 2 - 1, TamLetra, texto, StringAlignment.Center);
        }
    }
}
