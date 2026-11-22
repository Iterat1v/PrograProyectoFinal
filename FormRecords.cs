namespace PrograProyectoFinal
{
    // Top 10 histórico (archivo records.txt) y los mejores del reto de hoy
    public class FormRecords : FormPixel
    {
        private string[] nombresReto = new string[Reto.TOP];
        private double[] puntosReto = new double[Reto.TOP];
        private int cuantosReto;

        public FormRecords()
        {
            Titulo = "RÉCORDS";
            BotonRegresar((s, e) => IrA(new FormModo()));
            Records.Cargar();
            cuantosReto = Reto.MejoresDeHoy(nombresReto, puntosReto);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            Pixel.Panel(g, 40, 76, 820, 626, Paleta.Crema);
            Pixel.Texto(g, "TOP 10 DE TODOS LOS TIEMPOS", 450, 110, 15, Paleta.Morado2, StringAlignment.Center);
            if (Records.Cantidad == 0)
                Pixel.Parrafo(g, "Todavía no hay récords. Termina una partida para aparecer aquí.", new Rectangle(80, 200, 740, 80), 12, Paleta.Gris, StringAlignment.Center);
            Color[] medalla = { Paleta.Oro, Paleta.Crema2, Paleta.Bronce };
            for (int i = 0; i < Records.Cantidad; i++)
            {
                int y = 140 + i * 55;
                Pixel.R(g, 64, y, 772, 48, i < 3 ? Pixel.Aclarar(medalla[i], 0.5f) : Paleta.LilaClaro);
                if (i < 3) Pixel.R(g, 64, y, 10, 48, medalla[i]);
                Pixel.Texto(g, $"{i + 1}", 100, y + 24, 14, Paleta.Morado2, StringAlignment.Center);
                Pixel.Texto(g, Records.Nombre(i), 140, y + 24, 13, Paleta.Noche);
                Pixel.Texto(g, Records.Modo(i), 330, y + 24, 9, Paleta.Gris);
                Pixel.Texto(g, Records.Fecha(i), 715, y + 24, 9, Paleta.Gris, StringAlignment.Far);
                Pixel.Texto(g, Pixel.Dinero(Records.Patrimonio(i)), 840, y + 24, 12, Paleta.Verde2, StringAlignment.Far);
            }

            Pixel.Panel(g, 880, 76, 360, 626, Paleta.Crema);
            Pixel.Texto(g, "RETO DE HOY", 1060, 110, 15, Paleta.Morado2, StringAlignment.Center);
            if (cuantosReto == 0)
                Pixel.Parrafo(g, "Nadie ha jugado el reto de hoy.", new Rectangle(910, 170, 300, 80), 11, Paleta.Gris, StringAlignment.Center);
            for (int i = 0; i < cuantosReto; i++)
            {
                int y = 146 + i * 70;
                Pixel.R(g, 904, y, 312, 60, i == 0 ? Paleta.OroClaro : Paleta.LilaClaro);
                Pixel.Texto(g, $"{i + 1}. {nombresReto[i]}", 920, y + 20, 12, Paleta.Noche);
                Pixel.Texto(g, Pixel.Dinero(puntosReto[i]), 920, y + 44, 11, Paleta.Verde2);
            }
        }
    }
}
