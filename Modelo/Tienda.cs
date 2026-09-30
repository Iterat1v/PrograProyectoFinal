namespace PrograProyectoFinal
{
    // La tienda del día (como la de Fortnite): cada día a las 00:00 cambian los artículos.
    // Se escogen con la fecha como semilla, así a todos les sale la misma tienda.
    // 2 DESTACADOS con 25 % de descuento + 8 del DIARIO. Si hay una temporada activa
    // (Fiestas Patrias, Halloween, Muertos, Navidad...) hasta 4 de esos 8 son DE TEMPORADA.
    // Solo se venden artículos de la tienda: los del pase no salen aquí.
    internal static class Tienda
    {
        public const int DESTACADOS = 2;
        public const int DIARIOS = 8;
        public const int MAX_TEMPORADA = 4;
        public const double DESCUENTO = 0.25;

        public const int COMUN = 0, RARO = 1, EPICO = 2, LEGENDARIO = 3;
        public static readonly string[] NombresRareza = { "COMÚN", "RARO", "ÉPICO", "LEGENDARIO" };
        public static readonly Color[] ColorRareza =
        {
            ColorTranslator.FromHtml("#5fb24a"), ColorTranslator.FromHtml("#3f8fe8"),
            ColorTranslator.FromHtml("#a24cf0"), ColorTranslator.FromHtml("#f08a24")
        };
        public static readonly Color[] ColorRarezaOscuro =
        {
            ColorTranslator.FromHtml("#2f6b25"), ColorTranslator.FromHtml("#1d4f96"),
            ColorTranslator.FromHtml("#5b1f96"), ColorTranslator.FromHtml("#9a4d0c")
        };

        // Artículo = parte del avatar + opción. Orden: destacados, diario, de temporada.
        public static int[] Parte = new int[DESTACADOS + DIARIOS];
        public static int[] Opcion = new int[DESTACADOS + DIARIOS];
        private static int fechaArmada = 0;

        public static int Temporada { get; private set; } = Cosmeticos.NINGUNA;
        public static int NumTemporada { get; private set; } = 0;       // cuántos de los 8 son de temporada
        public static int PrimeroTemporada => DESTACADOS + DIARIOS - NumTemporada;

        public static int Rareza(int parte, int opcion) => Cosmeticos.Rareza(parte, opcion);

        public static int Precio(int articulo)
        {
            int normal = PrecioNormal(articulo);
            return articulo < DESTACADOS ? (int)(Math.Round(normal * (1 - DESCUENTO) / 5) * 5) : normal;
        }

        public static int PrecioNormal(int articulo) => Cosmeticos.Precio(Parte[articulo], Opcion[articulo]);
        public static string Nombre(int articulo) => Cosmeticos.Nombre(Parte[articulo], Opcion[articulo]);
        public static bool EsDeTemporada(int articulo) => articulo >= PrimeroTemporada;

        // Tiempo que falta para que cambie la tienda
        public static TimeSpan Falta => DateTime.Today.AddDays(1) - DateTime.Now;

        public static void Armar()
        {
            if (fechaArmada == Reto.Hoy) return;
            fechaArmada = Reto.Hoy;
            Temporada = Cosmeticos.TemporadaHoy;

            // lista de lo que se vende: normales y de la temporada de hoy
            int[] pn = new int[300], on = new int[300], pt = new int[60], ot = new int[60];
            int normales = 0, deTemporada = 0;
            for (int p = 0; p < Avatar.PARTES; p++)
                for (int o = 0; o < Cosmeticos.Cantidad(p); o++)
                {
                    if (Cosmeticos.Origen(p, o) != Cosmeticos.TIENDA) continue;
                    int t = Cosmeticos.Temporada(p, o);
                    if (t == Cosmeticos.NINGUNA) { pn[normales] = p; on[normales] = o; normales++; }
                    else if (t == Temporada) { pt[deTemporada] = p; ot[deTemporada] = o; deTemporada++; }
                }

            Random r = new Random(Reto.Hoy * 13);
            NumTemporada = Math.Min(MAX_TEMPORADA, deTemporada);
            bool[] usadoN = new bool[normales], usadoT = new bool[deTemporada];

            for (int k = 0; k < DESTACADOS + DIARIOS; k++)
            {
                if (k >= PrimeroTemporada)
                {
                    int t;
                    do t = r.Next(deTemporada); while (usadoT[t]);
                    usadoT[t] = true;
                    Parte[k] = pt[t];
                    Opcion[k] = ot[t];
                    continue;
                }
                // los destacados buscan algo épico o legendario
                int i, intentos = 0;
                do
                {
                    i = r.Next(normales);
                    intentos++;
                } while (usadoN[i] || (k < DESTACADOS && Rareza(pn[i], on[i]) < EPICO && intentos < 300));
                usadoN[i] = true;
                Parte[k] = pn[i];
                Opcion[k] = on[i];
            }
        }
    }
}
