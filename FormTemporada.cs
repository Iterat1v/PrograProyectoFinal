namespace PrograProyectoFinal
{
    // La liga: tema de la temporada, cuántas partidas van y la tabla de puntos
    public class FormTemporada : FormPixel
    {
        public FormTemporada()
        {
            Temporada.Cargar();
            Temporada.Ordenar();
            Titulo = $"TEMPORADA {Temporada.Numero}";
            BotonRegresar((s, e) => IrA(new FormModo()));
            Boton("JUGAR PARTIDA LIBRE", 40, 632, 500, 70, Paleta.Verde, Paleta.Verde2, (s, e) => IrA(new FormModo()), 14);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            int tema = Temporada.TemaActual;

            // ----- tema -----
            Pixel.Panel(g, 40, 76, 560, 540, Paleta.Crema);
            Pixel.R(g, 58, 94, 524, 90, Paleta.Oro);
            Pixel.Texto(g, "TEMA DE ESTA TEMPORADA", 320, 116, 10, Paleta.Morado2, StringAlignment.Center);
            Pixel.Texto(g, Temporada.Nombres[tema], 320, 156, 22, Paleta.Noche, StringAlignment.Center);
            Pixel.Parrafo(g, Temporada.Descripciones[tema], new Rectangle(70, 200, 500, 70), 12, Paleta.Noche, StringAlignment.Center);

            Pixel.Texto(g, $"PARTIDA {Math.Min(Temporada.Partidas + 1, Temporada.PARTIDAS)} DE {Temporada.PARTIDAS}", 70, 296, 12, Paleta.Morado2);
            for (int i = 0; i < Temporada.PARTIDAS; i++)
            {
                Pixel.R(g, 70 + i * 124, 318, 114, 30, Paleta.Negro);
                Pixel.R(g, 74 + i * 124, 322, 106, 22, i < Temporada.Partidas ? Paleta.Verde : Paleta.LilaClaro);
            }
            Pixel.Parrafo(g, "Cada partida libre suma puntos a las personas: 1o 10, 2o 6, 3o 3 y 4o 1. " +
                             "Al terminar las 4 partidas se cierra la tabla.", new Rectangle(70, 360, 500, 80), 9, Paleta.Gris);

            Pixel.Texto(g, "PREMIOS", 70, 450, 12, Paleta.Rosa2);
            string[] lugar = { "1er lugar", "2o lugar", "3er lugar" };
            for (int i = 0; i < Temporada.PremioMonedas.Length; i++)
            {
                Pixel.Sprite(g, Recursos.Moneda, 70, 468 + i * 34, 3);
                Pixel.Texto(g, $"{lugar[i]}: {Temporada.PremioMonedas[i]} monedas", 104, 480 + i * 34, 11, Paleta.Noche);
            }
            Pixel.Texto(g, "(el campeón también gana el logro CAMPEÓN DE LIGA)", 70, 586, 8, Paleta.Gris);
            Pixel.Texto(g, $"Siguiente: {Temporada.Nombres[Temporada.TemaSiguiente]}", 640, 667, 11, Paleta.Tenue);
            if (Temporada.UltimoCampeon != "")
                Pixel.Texto(g, $"Campeón pasado: {Temporada.UltimoCampeon}", 640, 640, 11, Paleta.Oro);

            // ----- tabla -----
            Pixel.Panel(g, 620, 76, 620, 540, Paleta.Crema);
            Pixel.Texto(g, "TABLA DE LA LIGA", 930, 110, 15, Paleta.Morado2, StringAlignment.Center);
            Pixel.Texto(g, "PTS", 1120, 140, 9, Paleta.Gris, StringAlignment.Far);
            Pixel.Texto(g, "GANÓ", 1214, 140, 9, Paleta.Gris, StringAlignment.Far);
            if (Temporada.Cantidad == 0)
                Pixel.Parrafo(g, "Todavía nadie tiene puntos. Jueguen una partida libre.", new Rectangle(650, 200, 560, 80), 12, Paleta.Gris, StringAlignment.Center);
            for (int i = 0; i < Temporada.Cantidad && i < 9; i++)
            {
                int y = 156 + i * 50;
                Pixel.R(g, 644, y, 572, 44, i == 0 ? Paleta.OroClaro : Paleta.LilaClaro);
                Pixel.Texto(g, $"{i + 1}", 664, y + 22, 13, Paleta.Morado2);
                Pixel.Texto(g, Temporada.NombreEn(i), 704, y + 22, 12, Paleta.Noche);
                Pixel.Texto(g, $"{Temporada.PuntosEn(i)}", 1120, y + 22, 13, Paleta.Verde2, StringAlignment.Far);
                Pixel.Texto(g, $"{Temporada.GanadasEn(i)}", 1200, y + 22, 12, Paleta.Gris, StringAlignment.Far);
            }
        }
    }
}
