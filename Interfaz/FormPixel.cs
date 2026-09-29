using System.ComponentModel;

namespace PrograProyectoFinal
{
    // Base de todas las pantallas: 1280 x 720, sin borde de Windows,
    // fondo morado rayado y (si tiene Titulo) la barra aqua de arriba.
    public class FormPixel : Form
    {
        private bool navegando = false;
        private bool arrastrando = false;
        private Point inicioArrastre;

        [Category("Pixel"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] public string Titulo { get; set; } = "";

        public FormPixel()
        {
            FormBorderStyle = FormBorderStyle.None;
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(Pixel.ANCHO, Pixel.ALTO);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Paleta.Morado;
            DoubleBuffered = true;
            Text = "Concesionario";
            KeyPreview = true;
        }

        // Cambia de pantalla: abre la siguiente en el mismo lugar y cierra esta
        protected void IrA(Form siguiente)
        {
            siguiente.StartPosition = FormStartPosition.Manual;
            siguiente.Location = Location;
            siguiente.Show();

            if (Application.OpenForms[0] == this)
                Hide();          // la pantalla de título es la principal: solo se esconde
            else
            {
                navegando = true;
                Close();
            }
        }

        // Regresa a la pantalla de título (que está escondida) y cierra esta
        protected void VolverAlTitulo()
        {
            Form titulo = Application.OpenForms[0];
            titulo.Location = Location;
            titulo.Show();
            navegando = true;
            Close();
        }

        // Crea un botón de juego con una sola línea (para las ventanas nuevas)
        protected BotonPixel Boton(string texto, int x, int y, int w, int h, Color fondo, Color sombra, EventHandler clic, float tam = 13)
        {
            BotonPixel b = new BotonPixel();
            b.Text = texto;
            b.Location = new Point(x, y);
            b.Size = new Size(w, h);
            b.TamLetra = tam;
            b.Colores(fondo, sombra);
            b.Click += clic;
            Controls.Add(b);
            return b;
        }

        // La ventana tiembla (terremoto, estafas): un Timer la mueve de un lado a otro
        public void Sacudir()
        {
            Point origen = Location;
            int paso = 0;
            Random r = new Random();
            System.Windows.Forms.Timer temblor = new System.Windows.Forms.Timer { Interval = 25 };
            temblor.Tick += (s, e) =>
            {
                paso++;
                int fuerza = Math.Max(1, 12 - paso / 2);
                Location = paso < 22 ? new Point(origen.X + r.Next(-fuerza, fuerza + 1), origen.Y + r.Next(-fuerza, fuerza + 1)) : origen;
                if (paso >= 22)
                {
                    temblor.Stop();
                    temblor.Dispose();
                }
            };
            temblor.Start();
        }

        // Las pantallas grandes entran con un fundido (sube Opacity con un Timer)
        protected override void OnShown(EventArgs e)
        {
            if (!Modal && Width >= Pixel.ANCHO)
            {
                Opacity = 0;
                System.Windows.Forms.Timer fundido = new System.Windows.Forms.Timer { Interval = 15 };
                fundido.Tick += (s, ev) =>
                {
                    Opacity = Math.Min(1, Opacity + 0.12);
                    if (Opacity >= 1)
                    {
                        fundido.Stop();
                        fundido.Dispose();
                    }
                };
                fundido.Start();
            }
            base.OnShown(e);
        }

        // Si el jugador cierra la ventana (y no es un cambio de pantalla), se acaba el juego
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            if (!navegando && !Modal)
                Application.Exit();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Pixel.Preparar(e.Graphics);
            Pixel.Fondo(e.Graphics);
            if (Titulo != "")
                Pixel.Barra(e.Graphics, Titulo);
            base.OnPaint(e);
        }

        // La barra se puede arrastrar; la X cierra y el guion minimiza
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (Titulo == "" || e.Y > 54) return;

            if (e.X >= 1220)
                Close();
            else if (e.X >= 1130 && e.X < 1170 && !Modal)
                WindowState = FormWindowState.Minimized;
            else
            {
                arrastrando = true;
                inicioArrastre = e.Location;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (arrastrando)
                Location = new Point(Location.X + e.X - inicioArrastre.X, Location.Y + e.Y - inicioArrastre.Y);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            arrastrando = false;
        }
    }
}
