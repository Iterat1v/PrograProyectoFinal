namespace PrograProyectoFinal
{
    // Repetición de la mejor estafa: la carta vuela del estafador al estafado
    public class FormRepeticion : FormPixel
    {
        private Partida partida;
        private int paso = 0;
        private System.Windows.Forms.Timer animacion = new System.Windows.Forms.Timer { Interval = 30 };

        internal FormRepeticion(Partida partida)
        {
            this.partida = partida;
            ClientSize = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            Boton("OTRA VEZ", 250, 506, 240, 70, Paleta.Agua, Paleta.Agua2, (s, e) => Empezar());
            Boton("CERRAR", 510, 506, 240, 70, Paleta.Crema, Paleta.Crema2, (s, e) => Close());
            animacion.Tick += (s, e) =>
            {
                paso++;
                if (paso == 40) Recursos.Sonar("estafa");
                if (paso >= 90) animacion.Stop();
                Invalidate();
            };
            Empezar();
        }

        private void Empezar()
        {
            paso = 0;
            Recursos.Sonar("carta");
            animacion.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pixel.Preparar(g);
            Pixel.R(g, 0, 0, Width, Height, Paleta.Noche);
            Pixel.Panel(g, 0, 0, Width, Height, Paleta.Noche, Paleta.Rosa);
            Pixel.SombraTexto(g, "REPETICIÓN", 500, 50, 40, Paleta.Oro, Paleta.Rosa);
            Pixel.Texto(g, "LA MEJOR ESTAFA DE LA PARTIDA", 500, 106, 12, Paleta.Agua, StringAlignment.Center);

            Concesionario a = partida.MejorEstafaUsuario, b = partida.MejorEstafaRival;
            // el estafador a la izquierda, el estafado a la derecha (tiembla cuando le pega)
            int temblor = paso > 40 && paso < 60 ? (paso % 2 == 0 ? 8 : -8) : 0;
            Pixel.R(g, 80, 150, 200, 200, Paleta.Morado3);
            Pixel.Sprite(g, Recursos.Cara(a), 84, 154, 12);
            Pixel.Texto(g, a.Jugador, 180, 380, 13, Paleta.Crema, StringAlignment.Center);
            Pixel.R(g, 720 + temblor, 150, 200, 200, paso > 40 ? Paleta.Rosa2 : Paleta.Morado3);
            Pixel.Sprite(g, Recursos.Cara(b), 724 + temblor, 154, 12);
            Pixel.Texto(g, b.Jugador, 820, 380, 13, Paleta.Crema, StringAlignment.Center);

            // la carta vuela en línea recta
            if (paso <= 40)
            {
                int x = 300 + (380 * paso / 40);
                Pixel.R(g, x, 200, 100, 140, Paleta.Negro);
                Pixel.R(g, x + 4, 204, 92, 132, Paleta.Crema);
                Pixel.R(g, x + 4, 204, 92, 22, Paleta.Rosa);
                Pixel.Sprite(g, Recursos.Iconos[1], x + 26, 240, 4);
            }
            else
            {
                int subida = Math.Min(40, (paso - 40) * 2);
                Pixel.Texto(g, $"-{Pixel.Dinero(partida.MejorEstafaPerdida)}", 820, 130 - subida, 16, Paleta.Rosa, StringAlignment.Center);
                Pixel.Texto(g, $"\"{partida.MejorEstafaCarta}\"", 500, 250, 15, Paleta.Oro, StringAlignment.Center);
            }
            if (paso >= 60)
                Pixel.Parrafo(g, partida.MejorEstafaTexto, new Rectangle(80, 420, 840, 70), 11, Paleta.Crema, StringAlignment.Center);
        }
    }
}
