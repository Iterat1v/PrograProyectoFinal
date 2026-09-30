namespace PrograProyectoFinal
{
    // Pase de temporada (como el Rocket Pass): 30 niveles que se suben con la XP de las partidas.
    // Fila GRATIS: premio cada 2 niveles. Fila PREMIUM (1,000 monedas): premio en cada nivel.
    // Los artículos del pase son exclusivos: no salen en la tienda.
    internal static class Pase
    {
        public const int NIVELES = 30;
        public const int XP_POR_NIVEL = 150;
        public const int PRECIO_PREMIUM = 1000;
        public const string NOMBRE = "PASE CONCESIONARIO · TEMPORADA 1";

        public const int NADA = 0, MONEDAS = 1, ARTICULO = 2;

        // [0] = fila gratis, [1] = fila premium; índice = nivel (1 a 30)
        private static int[,] tipo = new int[2, NIVELES + 1];
        private static int[,] parte = new int[2, NIVELES + 1];
        private static int[,] opcion = new int[2, NIVELES + 1];
        private static int[,] monedas = new int[2, NIVELES + 1];

        public static int Tipo(bool premium, int nivel) => tipo[premium ? 1 : 0, nivel];
        public static int Parte(bool premium, int nivel) => parte[premium ? 1 : 0, nivel];
        public static int Opcion(bool premium, int nivel) => opcion[premium ? 1 : 0, nivel];
        public static int Monedas(bool premium, int nivel) => monedas[premium ? 1 : 0, nivel];

        private static void Articulo(int fila, int nivel, int p, string nombre)
        {
            tipo[fila, nivel] = ARTICULO;
            parte[fila, nivel] = p;
            opcion[fila, nivel] = Cosmeticos.Buscar(p, nombre);
        }

        private static void Dinero(int fila, int nivel, int cuanto)
        {
            tipo[fila, nivel] = MONEDAS;
            monedas[fila, nivel] = cuanto;
        }

        static Pase()
        {
            // ----- fila gratis: niveles pares -----
            Dinero(0, 2, 50); Articulo(0, 4, Avatar.MASCARA, "Antifaz del pase"); Dinero(0, 6, 75);
            Articulo(0, 8, Avatar.COLOR_PELO, "Mechas rosas"); Dinero(0, 10, 100); Articulo(0, 12, Avatar.LENTES, "Goggles del pase");
            Dinero(0, 14, 100); Articulo(0, 16, Avatar.GORRO, "Audífonos aqua"); Dinero(0, 18, 125);
            Articulo(0, 20, Avatar.ROPA, "Sudadera del pase"); Dinero(0, 22, 125); Articulo(0, 24, Avatar.EXTRA, "Medalla de campeón");
            Dinero(0, 26, 150); Dinero(0, 28, 150); Articulo(0, 30, Avatar.FONDO, "Arcoíris");

            // ----- fila premium: todos los niveles -----
            int[] dineroPremium = { 0, 80, 0, 100, 0, 120, 0, 80, 0, 100, 0, 150, 0, 100, 0, 120, 0, 150, 0, 200, 0, 150, 0, 150, 0, 200, 0, 200, 0, 250, 0 };
            for (int n = 1; n <= NIVELES; n++)
                if (dineroPremium[n] > 0) Dinero(1, n, dineroPremium[n]);
            Articulo(1, 2, Avatar.COLOR_PELO, "Arcoíris");
            Articulo(1, 4, Avatar.EXTRA, "Collar de oro");
            Articulo(1, 6, Avatar.LENTES, "Scouter");
            Articulo(1, 8, Avatar.MASCARA, "Robot");
            Articulo(1, 10, Avatar.TEZ, "Plata");
            Articulo(1, 12, Avatar.FONDO, "Fuego");
            Articulo(1, 14, Avatar.MASCARA, "Luchador oro");
            Articulo(1, 16, Avatar.ROPA, "Traje de oro");
            Articulo(1, 18, Avatar.FONDO, "Galaxia");
            Articulo(1, 20, Avatar.GORRO, "Corona de rey");
            Articulo(1, 25, Avatar.TEZ, "Oro puro");
            Articulo(1, 30, Avatar.PEINADO, "Súper Saiyajin");
            // los niveles premium que quedaron sin nada (22, 24, 26, 28 ya tienen monedas; 21, 23... también)
            for (int n = 1; n <= NIVELES; n++)
                if (tipo[1, n] == NADA) Dinero(1, n, 100);
        }

        public static string Descripcion(bool premium, int nivel)
        {
            int t = Tipo(premium, nivel);
            if (t == MONEDAS) return $"{Monedas(premium, nivel)} monedas";
            if (t == ARTICULO) return Cosmeticos.Nombre(Parte(premium, nivel), Opcion(premium, nivel));
            return "";
        }

        public static bool SePuedeReclamar(Perfil p, bool premium, int nivel) =>
            Tipo(premium, nivel) != NADA && nivel <= p.NivelPase && !p.Reclamado(premium, nivel) && (!premium || p.PasePremium);

        // Regresa null si se reclamó; si no, por qué no
        public static string Reclamar(Perfil p, bool premium, int nivel)
        {
            if (Tipo(premium, nivel) == NADA) return "Ese nivel no tiene premio en esta fila.";
            if (p.Reclamado(premium, nivel)) return "Ya lo reclamaste.";
            if (nivel > p.NivelPase) return $"Llega al nivel {nivel} del pase para reclamarlo.";
            if (premium && !p.PasePremium) return $"Es de la fila PREMIUM: desbloquéala con {PRECIO_PREMIUM} monedas.";
            if (Tipo(premium, nivel) == MONEDAS)
                p.Ganar(0, Monedas(premium, nivel));
            else
                p.Dar(Parte(premium, nivel), Opcion(premium, nivel));
            p.MarcarReclamado(premium, nivel);
            return null;
        }
    }
}
