namespace PrograProyectoFinal
{
    // Misiones del día: 3 de una lista de 10, escogidas con la fecha como semilla
    // (a todos les tocan las mismas). El avance se suma en todas las partidas del día.
    internal static class Misiones
    {
        public const int POR_DIA = 3;
        public const int MONEDAS = 40, XP = 30, BONO_TODAS = 60;

        public static readonly string[] Textos =
        {
            "Vende 5 autos", "Vende 2 autos chocados", "Vende 1 auto tuneado", "Gana 1 partida",
            "Juega 2 partidas", "Véndele a El Bro", "Vende 3 autos arriba del precio sugerido",
            "Termina con 70 de reputación", "Gana una subasta", "Juega el reto del día"
        };
        public static readonly int[] Metas = { 5, 2, 1, 1, 2, 1, 3, 1, 1, 1 };

        // Las 3 de hoy, sin repetir
        public static int[] DelDia(int fecha)
        {
            Random r = new Random(fecha);
            int[] hoy = new int[POR_DIA];
            for (int k = 0; k < POR_DIA; k++)
            {
                bool repetida;
                do
                {
                    hoy[k] = r.Next(Textos.Length);
                    repetida = false;
                    for (int m = 0; m < k; m++)
                        if (hoy[m] == hoy[k]) repetida = true;
                } while (repetida);
            }
            return hoy;
        }

        // Cuánto avanza cada tipo de misión con una partida
        private static int Avance(int mision, Concesionario j, bool gano, bool esReto)
        {
            switch (mision)
            {
                case 0: return j.Ventas;
                case 1: return j.ChocadosVendidos;
                case 2: return j.TuneadosVendidos;
                case 3: return gano ? 1 : 0;
                case 4: return 1;
                case 5: return j.VentasBro;
                case 6: return j.VentasCaras;
                case 7: return j.Reputacion >= 70 ? 1 : 0;
                case 8: return j.SubastasGanadas;
                default: return esReto ? 1 : 0;
            }
        }

        // Suma el avance y cobra las que se cumplieron. Regresa el aviso ("" si nada).
        public static string Contar(Concesionario j, bool gano, bool esReto)
        {
            Perfil p = j.Perfil;
            if (p == null) return "";
            p.PrepararMisiones(Reto.Hoy);
            int[] hoy = DelDia(Reto.Hoy);

            string aviso = "";
            int nuevas = 0;
            for (int k = 0; k < POR_DIA; k++)
            {
                if (p.Cobrada(k)) continue;
                int mision = hoy[k];
                if (!j.Rendido)
                    p.Avanzar(k, Avance(mision, j, gano, esReto));
                if (p.Progreso(k) >= Metas[mision])
                {
                    p.Cobrar(k);
                    p.Ganar(XP, MONEDAS);
                    aviso += $"\"{Textos[mision]}\" (+{MONEDAS} monedas). ";
                    nuevas++;
                }
            }
            if (nuevas > 0 && p.Cobrada(0) && p.Cobrada(1) && p.Cobrada(2))
            {
                p.Ganar(0, BONO_TODAS);
                aviso += $"¡Las 3 del día! Bono de +{BONO_TODAS} monedas.";
            }
            return aviso;
        }
    }
}
