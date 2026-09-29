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
