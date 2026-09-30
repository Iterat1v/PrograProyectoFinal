namespace PrograProyectoFinal
{
    // La tienda del día (como la de Fortnite): cada día a las 00:00 cambian los artículos.
    // Se escogen con la fecha como semilla, así a todos les sale la misma tienda.
    // 2 DESTACADOS con 25 % de descuento + 8 del DIARIO.
    internal static class Tienda
    {
        public const int DESTACADOS = 2;
        public const int DIARIOS = 8;
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

        // Artículo = parte del avatar + opción
        public static int[] Parte = new int[DESTACADOS + DIARIOS];
        public static int[] Opcion = new int[DESTACADOS + DIARIOS];
        private static int fechaArmada = 0;

        public static int Rareza(int parte, int opcion)
        {
            int precio = Perfil.Precios[parte][opcion];
            return precio <= 60 ? COMUN : precio <= 150 ? RARO : precio <= 350 ? EPICO : LEGENDARIO;
        }

        public static int Precio(int articulo)
        {
            int normal = Perfil.Precios[Parte[articulo]][Opcion[articulo]];
            return articulo < DESTACADOS ? (int)(Math.Round(normal * (1 - DESCUENTO) / 5) * 5) : normal;
        }

        public static int PrecioNormal(int articulo) => Perfil.Precios[Parte[articulo]][Opcion[articulo]];

        public static string Nombre(int articulo) => Avatar.Opciones[Parte[articulo]][Opcion[articulo]];

        // Tiempo que falta para que cambie la tienda
        public static TimeSpan Falta => DateTime.Today.AddDays(1) - DateTime.Now;

        // Arma la tienda de hoy: destacados de lo caro (épico o legendario), el diario de lo demás.
        // Todo con búsqueda lineal en la lista de artículos que se venden.
        public static void Armar()
        {
            if (fechaArmada == Reto.Hoy) return;
            fechaArmada = Reto.Hoy;

            // todos los artículos que cuestan algo
            int total = 0;
            int[] partes = new int[80], opciones = new int[80];
            for (int p = 0; p < Avatar.PARTES; p++)
                for (int o = 0; o < Avatar.Opciones[p].Length; o++)
                    if (Perfil.Precios[p][o] > 0)
                    {
                        partes[total] = p;
                        opciones[total] = o;
                        total++;
                    }

            Random r = new Random(Reto.Hoy * 13);
            bool[] usado = new bool[total];
            for (int k = 0; k < DESTACADOS + DIARIOS; k++)
            {
                int i;
                int intentos = 0;
                do
                {
                    i = r.Next(total);
                    intentos++;
                } while (usado[i] || (k < DESTACADOS && Rareza(partes[i], opciones[i]) < EPICO && intentos < 200));
                usado[i] = true;
                Parte[k] = partes[i];
                Opcion[k] = opciones[i];
            }
        }
    }
}
