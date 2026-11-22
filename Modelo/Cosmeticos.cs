namespace PrograProyectoFinal
{
    // Catálogo de los artículos para tu personaje (más de 250), repartidos en 9 partes.
    // Cada artículo tiene: nombre, forma (cómo se dibuja), dos colores, precio, de dónde se
    // consigue (básico, tienda o pase) y, si es de temporada, en qué temporada sale en la tienda.
    // Se guarda todo en arreglos paralelos [parte][artículo], igual que una tabla.
    internal static class Cosmeticos
    {
        public const int MAX = 80;           // artículos máximos por parte

        public const int BASICO = 0, TIENDA = 1, PASE_GRATIS = 2, PASE_PREMIUM = 3;
        public static readonly string[] NombresOrigen = { "BÁSICO", "TIENDA", "PASE GRATIS", "PASE PREMIUM" };

        // ----- temporadas del año (fechas reales) -----
        public const int NINGUNA = -1, PATRIAS = 0, HALLOWEEN = 1, MUERTOS = 2, NAVIDAD = 3, REYES = 4, SAN_VALENTIN = 5, PRIMAVERA = 6, MUNDIAL = 7;
        public static readonly string[] NombresTemporada =
            { "FIESTAS PATRIAS", "HALLOWEEN", "DÍA DE MUERTOS", "NAVIDAD", "DÍA DE REYES", "SAN VALENTÍN", "PRIMAVERA", "MUNDIAL 2026" };

        // Qué temporada hay hoy (búsqueda por rangos de fecha)
        public static int TemporadaDe(DateTime d)
        {
            int md = d.Month * 100 + d.Day;
            if (md >= 901 && md <= 930) return PATRIAS;
            if (md >= 1001 && md <= 1024) return HALLOWEEN;
            if (md >= 1025 && md <= 1105) return MUERTOS;
            if (md >= 1201 && md <= 1231) return NAVIDAD;
            if (md >= 101 && md <= 115) return REYES;
            if (md >= 201 && md <= 220) return SAN_VALENTIN;
            if (md >= 321 && md <= 430) return PRIMAVERA;
            if (md >= 601 && md <= 719) return MUNDIAL;
            return NINGUNA;
        }
        public static int TemporadaHoy => TemporadaDe(DateTime.Today);

        // ----- colores con nombre -----
        public static readonly string[] NombresColor =
            { "Rojo", "Azul", "Verde", "Amarillo", "Rosa", "Morado", "Naranja", "Negro", "Blanco", "Gris", "Aqua", "Café", "Guinda", "Oro", "Plata", "Verde bandera" };
        public static readonly Color[] Colores =
        {
            Hex("#e8413c"), Hex("#3f6fd8"), Hex("#4c9a5b"), Hex("#ffd447"), Hex("#ff5fa2"), Hex("#8b5cf6"), Hex("#f28c28"), Hex("#2a2438"),
            Hex("#fff4e8"), Hex("#8a8499"), Hex("#2ee6c8"), Hex("#7a4a2a"), Hex("#8c1d40"), Hex("#e6b422"), Hex("#c9ccd6"), Hex("#006847")
        };
        public const int ROJO = 0, AZUL = 1, VERDE = 2, AMARILLO = 3, ROSA = 4, MORADO = 5, NARANJA = 6, NEGRO = 7, BLANCO = 8, GRIS = 9,
                         AQUA = 10, CAFE = 11, GUINDA = 12, ORO = 13, PLATA = 14, BANDERA = 15;

        private static Color Hex(string h) => ColorTranslator.FromHtml(h);

        // ----- la tabla -----
        private static string[,] nombre = new string[Avatar.PARTES, MAX];
        private static int[,] forma = new int[Avatar.PARTES, MAX];
        private static Color[,] colorA = new Color[Avatar.PARTES, MAX];
        private static Color[,] colorB = new Color[Avatar.PARTES, MAX];
        private static int[,] precio = new int[Avatar.PARTES, MAX];
        private static int[,] origen = new int[Avatar.PARTES, MAX];
        private static int[,] temporada = new int[Avatar.PARTES, MAX];
        private static int[] cantidad = new int[Avatar.PARTES];

        public static int Cantidad(int parte) => cantidad[parte];
        public static string Nombre(int parte, int i) => nombre[parte, i];
        public static int Forma(int parte, int i) => forma[parte, i];
        public static Color A(int parte, int i) => colorA[parte, i];
        public static Color B(int parte, int i) => colorB[parte, i];
        public static int Precio(int parte, int i) => precio[parte, i];
        public static int Origen(int parte, int i) => origen[parte, i];
        public static int Temporada(int parte, int i) => temporada[parte, i];

        public static int Total()
        {
            int t = 0;
            for (int p = 0; p < Avatar.PARTES; p++) t += cantidad[p];
            return t;
        }

        // Rareza según el precio (como en los juegos: común, raro, épico, legendario)
        public static int Rareza(int parte, int i)
        {
            int pr = precio[parte, i];
            if (origen[parte, i] == PASE_PREMIUM) return pr >= 700 ? 3 : 2;
            return pr <= 80 ? 0 : pr <= 180 ? 1 : pr <= 450 ? 2 : 3;
        }

        // Búsqueda lineal por nombre (para armar el pase)
        public static int Buscar(int parte, string texto)
        {
            for (int i = 0; i < cantidad[parte]; i++)
                if (nombre[parte, i] == texto) return i;
            throw new ArgumentException($"No existe el artículo {texto}");
        }

        private static void Agregar(int parte, string n, int f, int a, int b, int pr, int org = TIENDA, int temp = NINGUNA)
        {
            int i = cantidad[parte]++;
            nombre[parte, i] = n;
            forma[parte, i] = f;
            colorA[parte, i] = a >= 0 ? Colores[a] : Color.Transparent;
            colorB[parte, i] = b >= 0 ? Colores[b] : Color.Transparent;
            precio[parte, i] = org == BASICO ? 0 : pr;
            origen[parte, i] = org;
            temporada[parte, i] = temp;
        }

        // Tez con color propio (no de la lista de colores)
        private static void AgregarTez(string n, string hex, int pr, int org = TIENDA, int temp = NINGUNA)
        {
            Agregar(Avatar.TEZ, n, 0, -1, -1, pr, org, temp);
            colorA[Avatar.TEZ, cantidad[Avatar.TEZ] - 1] = Hex(hex);
        }

        static Cosmeticos()
        {
            int PE = Avatar.PEINADO, CP = Avatar.COLOR_PELO, R = Avatar.ROPA, L = Avatar.LENTES,
                G = Avatar.GORRO, M = Avatar.MASCARA, X = Avatar.EXTRA, F = Avatar.FONDO;

            // ===== TEZ (12) =====
            AgregarTez("Clara", "#ffe0c4", 0, BASICO); AgregarTez("Media", "#f2c29b", 0, BASICO); AgregarTez("Morena", "#c98e62", 0, BASICO);
            AgregarTez("Canela", "#a86b45", 0, BASICO); AgregarTez("Oscura", "#8d5a3b", 0, BASICO);
            AgregarTez("Alien", "#7fb069", 300); AgregarTez("Avatar", "#6fa8dc", 300); AgregarTez("Zombie", "#9bb58a", 250, TIENDA, HALLOWEEN);
            AgregarTez("Calavera", "#f4efe6", 350, TIENDA, MUERTOS); AgregarTez("Hulk", "#5aa04a", 400);
            AgregarTez("Plata", "#c9ccd6", 0, PASE_PREMIUM); AgregarTez("Oro puro", "#e6b422", 0, PASE_PREMIUM);

            // ===== PEINADO (12) =====
            string[] peinados = { "Corto", "Largo", "Chongo", "Pelón", "Coleta", "Afro", "Mohicano", "Súper Saiyajin", "Mullet", "Trenzas", "Copete", "Flequillo" };
            int[] precioPeinado = { 0, 0, 0, 0, 80, 120, 150, 0, 120, 140, 180, 90 };
            for (int i = 0; i < peinados.Length; i++)
                Agregar(PE, peinados[i], i, -1, -1, precioPeinado[i], i < 4 ? BASICO : i == 7 ? PASE_PREMIUM : TIENDA);

            // ===== COLOR DE PELO (22) =====
            string[] pelos = { "Negro", "Café", "Castaño", "Rubio", "Pelirrojo", "Canoso" };
            string[] hexPelo = { "#1c1020", "#3b2416", "#5a2d1a", "#d9a441", "#c2410c", "#dcdcdc" };
            for (int i = 0; i < pelos.Length; i++)
            {
                Agregar(CP, pelos[i], 0, -1, -1, 0, BASICO);
                colorA[CP, cantidad[CP] - 1] = Hex(hexPelo[i]);
            }
            Agregar(CP, "Rosa", 0, ROSA, -1, 40); Agregar(CP, "Azul", 0, AZUL, -1, 40); Agregar(CP, "Verde", 0, VERDE, -1, 40);
            Agregar(CP, "Morado", 0, MORADO, -1, 60); Agregar(CP, "Aqua", 0, AQUA, -1, 60); Agregar(CP, "Naranja", 0, NARANJA, -1, 60);
            Agregar(CP, "Blanco", 0, BLANCO, -1, 80); Agregar(CP, "Rojo fuego", 0, ROJO, -1, 100); Agregar(CP, "Guinda", 0, GUINDA, -1, 100);
            Agregar(CP, "Plata", 0, PLATA, -1, 150); Agregar(CP, "Dorado", 0, ORO, -1, 300);
            Agregar(CP, "Calabaza", 0, NARANJA, -1, 120, TIENDA, HALLOWEEN); Agregar(CP, "Verde bandera", 0, BANDERA, -1, 120, TIENDA, PATRIAS);
            Agregar(CP, "Arcoíris", 1, ROJO, AZUL, 0, PASE_PREMIUM);
            Agregar(CP, "Mechas rosas", 1, NEGRO, ROSA, 0, PASE_GRATIS); Agregar(CP, "Mechas azules", 1, NEGRO, AQUA, 180);

            // ===== ROPA (57) =====
            int[] colRopa = { AZUL, ROSA, AQUA, ROJO, VERDE, MORADO, NEGRO, BLANCO, NARANJA, ORO, GUINDA, GRIS };
            for (int i = 0; i < colRopa.Length; i++)       // playeras lisas
                Agregar(R, $"Playera {NombresColor[colRopa[i]].ToLower()}", 0, colRopa[i], -1, i < 4 ? 0 : i == 9 ? 250 : 40, i < 4 ? BASICO : TIENDA);
            int[,] rayas = { { BLANCO, AZUL }, { BLANCO, ROJO }, { NEGRO, AMARILLO }, { AZUL, BLANCO }, { VERDE, BLANCO }, { ROSA, BLANCO }, { NEGRO, BLANCO }, { MORADO, AMARILLO } };
            for (int i = 0; i < rayas.GetLength(0); i++)
                Agregar(R, $"Rayas {NombresColor[rayas[i, 0]].ToLower()} y {NombresColor[rayas[i, 1]].ToLower()}", 1, rayas[i, 0], rayas[i, 1], 70);
            int[,] jersey = { { ROJO, BLANCO }, { AZUL, AMARILLO }, { AMARILLO, AZUL }, { NEGRO, ROJO }, { BLANCO, AZUL }, { MORADO, ORO }, { NARANJA, NEGRO }, { AQUA, BLANCO } };
            for (int i = 0; i < jersey.GetLength(0); i++)
                Agregar(R, $"Jersey {NombresColor[jersey[i, 0]].ToLower()}", 2, jersey[i, 0], jersey[i, 1], 120);
            int[,] cuadros = { { ROJO, NEGRO }, { AZUL, BLANCO }, { VERDE, NEGRO }, { AMARILLO, NEGRO }, { ROSA, BLANCO }, { GRIS, NEGRO } };
            for (int i = 0; i < cuadros.GetLength(0); i++)
                Agregar(R, $"Franela {NombresColor[cuadros[i, 0]].ToLower()}", 3, cuadros[i, 0], cuadros[i, 1], 90);
            int[] camisas = { BLANCO, AZUL, ROSA, GRIS, NEGRO, GUINDA };
            for (int i = 0; i < camisas.Length; i++)
                Agregar(R, $"Camisa {NombresColor[camisas[i]].ToLower()}", 4, camisas[i], BLANCO, 110);
            int[,] sudaderas = { { GRIS, BLANCO }, { NEGRO, ROJO }, { AZUL, BLANCO }, { VERDE, AMARILLO }, { ROSA, BLANCO }, { MORADO, AQUA }, { ROJO, NEGRO }, { AMARILLO, NEGRO } };
            for (int i = 0; i < sudaderas.GetLength(0); i++)
                Agregar(R, $"Sudadera {NombresColor[sudaderas[i, 0]].ToLower()}", 5, sudaderas[i, 0], sudaderas[i, 1], 150);
            Agregar(R, "Suéter navideño", 6, ROJO, VERDE, 200, TIENDA, NAVIDAD); Agregar(R, "Suéter del reno", 6, VERDE, BLANCO, 200, TIENDA, NAVIDAD);
            Agregar(R, "Traje de charro", 7, NEGRO, ORO, 450, TIENDA, PATRIAS);
            Agregar(R, "Vestido de Catrina", 8, MORADO, NARANJA, 400, TIENDA, MUERTOS); Agregar(R, "Rebozo de cempasúchil", 8, NEGRO, NARANJA, 300, TIENDA, MUERTOS);
            Agregar(R, "Jersey de México", 2, BANDERA, BLANCO, 300, TIENDA, MUNDIAL);
            Agregar(R, "Disfraz de calabaza", 1, NARANJA, NEGRO, 180, TIENDA, HALLOWEEN);
            Agregar(R, "Suéter de corazones", 6, ROSA, ROJO, 180, TIENDA, SAN_VALENTIN);
            Agregar(R, "Traje de oro", 7, ORO, NEGRO, 0, PASE_PREMIUM);
            Agregar(R, "Sudadera del pase", 5, AQUA, MORADO, 0, PASE_GRATIS);

            // ===== LENTES (19) =====
            string[] lentes = { "Sin lentes", "Lentes", "Oscuros", "3D", "Corazón", "Monóculo", "Scouter" };
            int[] precioLentes = { 0, 0, 0, 80, 120, 250, 0 };
            for (int i = 0; i < lentes.Length; i++)
                Agregar(L, lentes[i], i, -1, -1, precioLentes[i], i < 3 ? BASICO : i == 6 ? PASE_PREMIUM : TIENDA, i == 4 ? SAN_VALENTIN : NINGUNA);
            int[] colLente = { ROJO, AZUL, VERDE, ROSA, ORO, AQUA };
            for (int i = 0; i < colLente.Length; i++)
                Agregar(L, $"Oscuros {NombresColor[colLente[i]].ToLower()}", 7, colLente[i], -1, colLente[i] == ORO ? 300 : 90);
            int[] colEstrella = { AMARILLO, ROSA, AQUA };
            for (int i = 0; i < colEstrella.Length; i++)
                Agregar(L, $"Estrella {NombresColor[colEstrella[i]].ToLower()}", 8, colEstrella[i], -1, 160);
            Agregar(L, "Goggles de nieve", 9, AQUA, -1, 200, TIENDA, NAVIDAD); Agregar(L, "Goggles naranjas", 9, NARANJA, -1, 200);
            Agregar(L, "Goggles del pase", 9, MORADO, -1, 0, PASE_GRATIS);

            // ===== GORROS (44) =====
            Agregar(G, "Sin gorro", 0, -1, -1, 0, BASICO);
            int[] gorras = { ROJO, AZUL, NEGRO, BLANCO, VERDE, ROSA, AMARILLO, MORADO, GRIS, NARANJA };
            for (int i = 0; i < gorras.Length; i++)
                Agregar(G, $"Gorra {NombresColor[gorras[i]].ToLower()}", 1, gorras[i], -1, i < 1 ? 0 : 60, i < 1 ? BASICO : TIENDA);
            Agregar(G, "Sombrero vaquero", 2, CAFE, -1, 120);
            Agregar(G, "Gorro navideño", 3, ROJO, BLANCO, 150, TIENDA, NAVIDAD);
            int[] cascos = { AZUL, ROJO, NEGRO, ORO };
            for (int i = 0; i < cascos.Length; i++)
                Agregar(G, $"Casco {NombresColor[cascos[i]].ToLower()}", 4, cascos[i], AQUA, cascos[i] == ORO ? 500 : 150);
            Agregar(G, "Cuernos de diablito", 5, ROJO, -1, 200, TIENDA, HALLOWEEN);
            Agregar(G, "Aureola", 6, ORO, -1, 250);
            Agregar(G, "Corona de rey", 7, ORO, ROSA, 0, PASE_PREMIUM);
            Agregar(G, "Sombrero de charro", 8, NEGRO, ORO, 350, TIENDA, PATRIAS); Agregar(G, "Sombrero de charro blanco", 8, BLANCO, ORO, 350, TIENDA, PATRIAS);
            Agregar(G, "Sombrero de bruja", 9, NEGRO, MORADO, 250, TIENDA, HALLOWEEN); Agregar(G, "Sombrero de bruja verde", 9, VERDE, NEGRO, 250, TIENDA, HALLOWEEN);
            Agregar(G, "Cuernos de reno", 10, CAFE, ROJO, 200, TIENDA, NAVIDAD);
            Agregar(G, "Corona de cempasúchil", 11, NARANJA, AMARILLO, 300, TIENDA, MUERTOS);
            Agregar(G, "Cabeza de calabaza", 12, NARANJA, NEGRO, 350, TIENDA, HALLOWEEN);
            Agregar(G, "Diadema de corazón", 13, ROJO, ROSA, 150, TIENDA, SAN_VALENTIN);
            int[] bandanas = { ROJO, AZUL, NEGRO, VERDE, MORADO };
            for (int i = 0; i < bandanas.Length; i++)
                Agregar(G, $"Bandana {NombresColor[bandanas[i]].ToLower()}", 14, bandanas[i], BLANCO, 80);
            int[,] pompones = { { ROJO, BLANCO }, { AZUL, BLANCO }, { VERDE, ROJO }, { ROSA, BLANCO } };
            for (int i = 0; i < pompones.GetLength(0); i++)
                Agregar(G, $"Gorro de invierno {NombresColor[pompones[i, 0]].ToLower()}", 15, pompones[i, 0], pompones[i, 1], 100);
            Agregar(G, "Sombrero de paja", 16, AMARILLO, ROJO, 90, TIENDA, PRIMAVERA);
            Agregar(G, "Corona de Reyes", 17, ORO, AZUL, 400, TIENDA, REYES);
            Agregar(G, "Orejas de conejo", 18, BLANCO, ROSA, 150, TIENDA, PRIMAVERA);
            int[] audifonos = { NEGRO, BLANCO, ROSA, AQUA };
            for (int i = 0; i < audifonos.Length; i++)
                Agregar(G, $"Audífonos {NombresColor[audifonos[i]].ToLower()}", 19, audifonos[i], GRIS, audifonos[i] == AQUA ? 0 : 130, audifonos[i] == AQUA ? PASE_GRATIS : TIENDA);

            // ===== MÁSCARAS (31) =====
            Agregar(M, "Sin máscara", 0, -1, -1, 0, BASICO);
            int[] cubrebocas = { AQUA, BLANCO, NEGRO, ROSA };
            for (int i = 0; i < cubrebocas.Length; i++)
                Agregar(M, $"Cubrebocas {NombresColor[cubrebocas[i]].ToLower()}", 1, cubrebocas[i], -1, 30);
            int[] ninjas = { ROJO, AZUL, NARANJA, MORADO };
            for (int i = 0; i < ninjas.Length; i++)
                Agregar(M, $"Ninja {NombresColor[ninjas[i]].ToLower()}", 2, ninjas[i], -1, 120);
            int[,] luchas = { { ROJO, ORO }, { AZUL, PLATA }, { VERDE, AMARILLO }, { NEGRO, ORO }, { ROSA, BLANCO }, { MORADO, AMARILLO },
                              { BLANCO, AZUL }, { NARANJA, NEGRO }, { AQUA, ROSA }, { ORO, NEGRO } };
            for (int i = 0; i < luchas.GetLength(0); i++)
                Agregar(M, $"Luchador {NombresColor[luchas[i, 0]].ToLower()}", 3, luchas[i, 0], luchas[i, 1],
                        luchas[i, 0] == ORO ? 0 : 200, luchas[i, 0] == ORO ? PASE_PREMIUM : TIENDA, i == 0 ? PATRIAS : NINGUNA);
            Agregar(M, "Calaverita", 4, BLANCO, ROSA, 200, TIENDA, MUERTOS);
            Agregar(M, "Zombie", 5, -1, -1, 250, TIENDA, HALLOWEEN);
            Agregar(M, "Robot", 6, GRIS, AQUA, 0, PASE_PREMIUM);
            Agregar(M, "Catrina", 7, BLANCO, NARANJA, 350, TIENDA, MUERTOS); Agregar(M, "Catrina azul", 7, BLANCO, AZUL, 350, TIENDA, MUERTOS);
            Agregar(M, "Cara de calabaza", 8, NARANJA, NEGRO, 220, TIENDA, HALLOWEEN);
            Agregar(M, "Momia", 9, BLANCO, GRIS, 220, TIENDA, HALLOWEEN);
            Agregar(M, "Nariz de reno", 10, ROJO, -1, 100, TIENDA, NAVIDAD);
            Agregar(M, "Fantasma", 11, BLANCO, NEGRO, 180, TIENDA, HALLOWEEN);
            Agregar(M, "Antifaz dorado", 12, ORO, ROSA, 300); Agregar(M, "Antifaz morado", 12, MORADO, ORO, 150); Agregar(M, "Antifaz del pase", 12, AQUA, BLANCO, 0, PASE_GRATIS);

            // ===== EXTRAS (26) =====
            Agregar(X, "Nada", 0, -1, -1, 0, BASICO);
            int[] corbatas = { ROJO, AZUL, NEGRO, VERDE, MORADO, ORO };
            for (int i = 0; i < corbatas.Length; i++)
                Agregar(X, $"Corbata {NombresColor[corbatas[i]].ToLower()}", 1, corbatas[i], BLANCO, i == 0 ? 0 : 40, i == 0 ? BASICO : TIENDA);
            Agregar(X, "Bigote", 3, -1, -1, 50);
            int[] aretes = { ORO, PLATA, ROSA };
            for (int i = 0; i < aretes.Length; i++)
                Agregar(X, $"Aretes {NombresColor[aretes[i]].ToLower()}", 4, aretes[i], -1, 60);
            Agregar(X, "Chapitas", 5, -1, -1, 50);
            Agregar(X, "Collar de oro", 6, ORO, -1, 0, PASE_PREMIUM);
            Agregar(X, "Barba", 7, -1, -1, 250);
            Agregar(X, "Bigote de charro", 8, -1, -1, 180, TIENDA, PATRIAS);
            int[,] bufandas = { { ROJO, VERDE }, { BLANCO, ROJO }, { AZUL, BLANCO }, { MORADO, AQUA } };
            for (int i = 0; i < bufandas.GetLength(0); i++)
                Agregar(X, $"Bufanda {NombresColor[bufandas[i, 0]].ToLower()}", 10, bufandas[i, 0], bufandas[i, 1], 110, TIENDA, i < 2 ? NAVIDAD : NINGUNA);
            Agregar(X, "Medalla de campeón", 11, ORO, ROJO, 0, PASE_GRATIS);
            Agregar(X, "Collar de cempasúchil", 12, NARANJA, AMARILLO, 250, TIENDA, MUERTOS);
            int[] monos = { ROJO, NEGRO, ROSA, AZUL };
            for (int i = 0; i < monos.Length; i++)
                Agregar(X, $"Moño {NombresColor[monos[i]].ToLower()}", 13, monos[i], -1, 70);
            Agregar(X, "Rosa en la boca", 9, ROJO, VERDE, 200, TIENDA, SAN_VALENTIN);

            // ===== FONDOS (38) =====
            Agregar(F, "Sin fondo", 0, -1, -1, 0, BASICO);
            int[] lisos = { MORADO, AZUL, ROSA, VERDE, NARANJA, AQUA, NEGRO, ROJO, AMARILLO, GRIS, GUINDA, ORO };
            for (int i = 0; i < lisos.Length; i++)
                Agregar(F, $"Fondo {NombresColor[lisos[i]].ToLower()}", 1, lisos[i], -1, lisos[i] == ORO ? 300 : 40);
            int[,] fondoRayas = { { MORADO, ROSA }, { AZUL, AQUA }, { NEGRO, AMARILLO }, { ROJO, BLANCO } };
            for (int i = 0; i < fondoRayas.GetLength(0); i++)
                Agregar(F, $"Rayas {NombresColor[fondoRayas[i, 0]].ToLower()}", 2, fondoRayas[i, 0], fondoRayas[i, 1], 90);
            int[,] fondoCuadros = { { NEGRO, BLANCO }, { ROSA, BLANCO }, { AZUL, BLANCO }, { VERDE, AMARILLO } };
            for (int i = 0; i < fondoCuadros.GetLength(0); i++)
                Agregar(F, $"Cuadritos {NombresColor[fondoCuadros[i, 0]].ToLower()}", 3, fondoCuadros[i, 0], fondoCuadros[i, 1], 90);
            int[,] fondoEstrellas = { { NEGRO, AMARILLO }, { AZUL, BLANCO }, { MORADO, ROSA } };
            for (int i = 0; i < fondoEstrellas.GetLength(0); i++)
                Agregar(F, $"Estrellas {NombresColor[fondoEstrellas[i, 0]].ToLower()}", 4, fondoEstrellas[i, 0], fondoEstrellas[i, 1], 140);
            int[,] picado = { { MORADO, ROSA }, { NEGRO, NARANJA }, { AZUL, AMARILLO } };
            for (int i = 0; i < picado.GetLength(0); i++)
                Agregar(F, $"Papel picado {NombresColor[picado[i, 1]].ToLower()}", 5, picado[i, 0], picado[i, 1], 200, TIENDA, MUERTOS);
            Agregar(F, "Nevada", 6, AZUL, BLANCO, 180, TIENDA, NAVIDAD); Agregar(F, "Noche de paz", 6, NEGRO, AMARILLO, 180, TIENDA, NAVIDAD);
            Agregar(F, "Bandera de México", 7, BANDERA, ROJO, 300, TIENDA, PATRIAS);
            Agregar(F, "Cancha del Mundial", 8, VERDE, BLANCO, 300, TIENDA, MUNDIAL);
            Agregar(F, "Corazones", 9, ROSA, ROJO, 150, TIENDA, SAN_VALENTIN); Agregar(F, "Corazones morados", 9, MORADO, ROSA, 150, TIENDA, SAN_VALENTIN);
            Agregar(F, "Telaraña", 10, NEGRO, BLANCO, 200, TIENDA, HALLOWEEN);
            Agregar(F, "Fuego", 11, ROJO, NARANJA, 0, PASE_PREMIUM);
            Agregar(F, "Arcoíris", 12, ROJO, AZUL, 0, PASE_GRATIS);
            Agregar(F, "Galaxia", 13, NEGRO, MORADO, 0, PASE_PREMIUM);
        }
    }
}
