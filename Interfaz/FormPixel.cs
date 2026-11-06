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
            Text = "Car Cash";
            KeyPreview = true;
        }

        // ── Pantalla completa ────────────────────────────────────────────────────────
        // Todo el juego está pensado en 1280 x 720. Con Pixel.Esc > 1 la ventana, los controles
        // y el dibujo se agrandan por ese factor y el mouse se devuelve a coordenadas del diseño.
        private bool escalado = false;

        // Ancho y alto de esta ventana en medidas del diseño (lo que usa el dibujo)
        protected int AnchoD { get { return escalado ? (int)Math.Round(Width / Pixel.Esc) : Width; } }
        protected int AltoD { get { return escalado ? (int)Math.Round(Height / Pixel.Esc) : Height; } }

        // Convierte un punto del diseño (1280 x 720) a pixeles reales de esta ventana
        protected Point Aqui(int x, int y)
        {
            float s = escalado ? Pixel.Esc : 1f;
            return new Point((int)Math.Round(x * s), (int)Math.Round(y * s));
        }

        // Al revés: el rectángulo real de un control, en coordenadas del diseño
        protected Rectangle Diseno(Rectangle r)
        {
            float s = escalado ? Pixel.Esc : 1f;
            return new Rectangle((int)Math.Round(r.X / s), (int)Math.Round(r.Y / s), (int)Math.Round(r.Width / s), (int)Math.Round(r.Height / s));
        }

        private void EscalarControl(Control c, float s)
        {
            c.Bounds = new Rectangle((int)Math.Round(c.Left * s), (int)Math.Round(c.Top * s),
                                     (int)Math.Round(c.Width * s), (int)Math.Round(c.Height * s));
            if (!(c is BotonPixel))
                c.Font = new Font(c.Font.FontFamily, c.Font.Size * s, c.Font.Style, c.Font.Unit);
            foreach (Control hijo in c.Controls)
                EscalarControl(hijo, s);
        }

        private void AplicarEscala()
        {
            float s = Pixel.Esc;
            if (s == 1f || escalado || DesignMode) return;
            escalado = true;
            Size cs = ClientSize;
            ClientSize = new Size((int)Math.Round(cs.Width * s), (int)Math.Round(cs.Height * s));
            foreach (Control c in Controls)
                EscalarControl(c, s);
            if (Pixel.Captura) return;
            Rectangle pantalla = Screen.FromControl(Owner ?? this).Bounds;
            Location = new Point(pantalla.X + (pantalla.Width - Width) / 2, pantalla.Y + (pantalla.Height - Height) / 2);
        }

        protected override bool ShowWithoutActivation { get { return Pixel.Captura; } }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AplicarEscala();
        }

        // Los controles que se agregan con la ventana ya abierta también se agrandan
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (escalado)
                EscalarControl(e.Control, Pixel.Esc);
        }

        // El mouse sobre el fondo de la ventana llega en pixeles reales: se devuelve al diseño
        protected override void WndProc(ref Message m)
        {
            if (escalado && m.Msg >= 0x0200 && m.Msg <= 0x020D && m.Msg != 0x020A)
            {
                long lp = m.LParam.ToInt64();
                int x = (short)(lp & 0xFFFF);
                int y = (short)((lp >> 16) & 0xFFFF);
                x = (int)(x / Pixel.Esc);
                y = (int)(y / Pixel.Esc);
                m.LParam = (IntPtr)(((y & 0xFFFF) << 16) | (x & 0xFFFF));
            }
            base.WndProc(ref m);
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

        // Botón "< REGRESAR" en la barra de arriba (todas las pantallas lo tienen en el mismo lugar)
        protected BotonPixel BotonRegresar(EventHandler clic)
        {
            return Boton("< REGRESAR", 900, 6, 210, 44, Paleta.Crema, Paleta.Crema2, clic, 10);
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
            if (escalado && !Pixel.Captura)
            {
                TopMost = true;   // para tapar la barra de tareas mientras se juega
                Activate();
            }
        }

        // Si el jugador pasa a otra aplicación, el juego deja de tapar todo; al volver, lo vuelve a hacer
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (escalado && !Pixel.Captura && !TopMost)
                TopMost = true;
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            if (escalado && !Pixel.Captura && TopMost)
                TopMost = false;
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
            Pixel.Escalar(e.Graphics);
            Pixel.Fondo(e.Graphics);
            if (Titulo != "")
                Pixel.Barra(e.Graphics, Titulo);
            base.OnPaint(e);
        }

        // La barra se puede arrastrar; la X cierra y el guion minimiza
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (IsDisposed || !Visible) return;
            if (Titulo == "" || e.Y > 54) return;

            if (e.X >= 1220)
                Close();
            else if (e.X >= 1130 && e.X < 1170 && !Modal)
                WindowState = FormWindowState.Minimized;
            else
            {
                arrastrando = !escalado;   // en pantalla completa la barra no se arrastra
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
