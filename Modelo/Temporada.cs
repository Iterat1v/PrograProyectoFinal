namespace PrograProyectoFinal
{
    // Liga de temporada: cada 4 partidas libres se cierra la tabla, se premia al campeón
    // y cambia el tema, que mueve la economía (clientes, mercado y qué tan seguido llega El Bro).
    // Archivo: %AppData%\Concesionario\temporada.txt
    //   1a línea: numero;partidasJugadas;ultimoCampeon
    //   las demás: nombre;puntos;ganadas
    internal static class Temporada
    {
        public const int PARTIDAS = 4;
        public const int MAX_TABLA = 50;

        public static readonly string[] Nombres = { "TEMPORADA NORMAL", "HOT SALE", "BUEN FIN", "CUESTA DE ENERO", "MUNDIAL" };
        public static readonly string[] Descripciones =
        {
            "Nada raro: puro vender.",
            "Los clientes andan gastalones: traen 15 % más.",
            "El mercado está 15 % más barato, pero los clientes también regatean: traen 15 % menos.",
            "Nadie tiene dinero: los clientes traen 20 % menos y el mercado cuesta lo mismo.",
            "Todos andan de fiesta: El Bro llega el doble de seguido y los clientes traen 5 % más."
        };
        public static readonly double[] FactorClientes = { 1.00, 1.15, 0.85, 0.80, 1.05 };
        public static readonly double[] FactorMercado = { 1.00, 1.00, 0.85, 1.00, 1.00 };
        public static readonly int[] ProbabilidadBro = { 25, 25, 25, 25, 50 };

        public static readonly int[] PuntosPorLugar = { 10, 6, 3, 1 };
        public static readonly int[] PremioMonedas = { 300, 150, 75 };

        private static int numero = 1;
        private static int partidas = 0;
        private static string ultimoCampeon = "";
        private static string[] nombres = new string[MAX_TABLA];
        private static int[] puntos = new int[MAX_TABLA];
        private static int[] ganadas = new int[MAX_TABLA];
        private static int cantidad = 0;

        private static string Ruta => Path.Combine(Path.GetDirectoryName(Perfiles.Ruta), "temporada.txt");

        public static int Numero => numero;
        public static int Partidas => partidas;
        public static string UltimoCampeon => ultimoCampeon;
        public static int Cantidad => cantidad;
        public static int TemaActual => numero % Nombres.Length;     // la 1 es HOT SALE, la 5 es normal
        public static int TemaSiguiente => (numero + 1) % Nombres.Length;

        public static string NombreEn(int i) => nombres[i];
        public static int PuntosEn(int i) => puntos[i];
        public static int GanadasEn(int i) => ganadas[i];

        public static void Cargar()
        {
            numero = 1;
            partidas = 0;
            ultimoCampeon = "";
            cantidad = 0;
            if (!File.Exists(Ruta)) return;
            try
            {
                using (StreamReader lector = new StreamReader(Ruta))
                {
                    string[] cabeza = (lector.ReadLine() ?? "").Split(';');
                    if (cabeza.Length >= 3)
                    {
                        int.TryParse(cabeza[0], out numero);
                        int.TryParse(cabeza[1], out partidas);
                        ultimoCampeon = cabeza[2];
                    }
                    if (numero < 1) numero = 1;
                    string linea;
                    while ((linea = lector.ReadLine()) != null && cantidad < MAX_TABLA)
                    {
                        string[] c = linea.Split(';');
                        if (c.Length < 3) continue;
                        nombres[cantidad] = c[0];
                        int.TryParse(c[1], out puntos[cantidad]);
                        int.TryParse(c[2], out ganadas[cantidad]);
                        cantidad++;
                    }
                }
            }
            catch (IOException)
            {
                cantidad = 0;
            }
        }

        private static void Guardar()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Ruta));
                using (StreamWriter escritor = new StreamWriter(Ruta))
                {
                    escritor.WriteLine($"{numero};{partidas};{ultimoCampeon}");
                    for (int i = 0; i < cantidad; i++)
                        escritor.WriteLine($"{nombres[i]};{puntos[i]};{ganadas[i]}");
                }
            }
            catch (IOException)
            {
            }
        }

        // Búsqueda lineal; si no está, lo agrega al final
        private static int Indice(string nombre)
        {
            for (int i = 0; i < cantidad; i++)
                if (nombres[i].ToLower() == nombre.ToLower())
                    return i;
            if (cantidad == MAX_TABLA) return -1;
            nombres[cantidad] = nombre;
            puntos[cantidad] = 0;
            ganadas[cantidad] = 0;
            return cantidad++;
        }

        // Burbuja de mayor a menor puntos (los tres arreglos se mueven juntos)
        public static void Ordenar()
        {
            for (int i = 0; i < cantidad - 1; i++)
                for (int j = 0; j < cantidad - 1 - i; j++)
                    if (puntos[j] < puntos[j + 1])
                    {
                        (puntos[j], puntos[j + 1]) = (puntos[j + 1], puntos[j]);
                        (ganadas[j], ganadas[j + 1]) = (ganadas[j + 1], ganadas[j]);
                        (nombres[j], nombres[j + 1]) = (nombres[j + 1], nombres[j]);
                    }
        }

        // Suma los puntos de una partida libre. Regresa el aviso que se enseña (o null).
        public static string RegistrarPartida(Concesionario[] ranking)
        {
            Cargar();
            bool hayPersonas = false;
            for (int lugar = 0; lugar < ranking.Length; lugar++)
            {
                Concesionario j = ranking[lugar];
                if (j.Perfil == null) continue;
                hayPersonas = true;
                int i = Indice(j.Perfil.Nombre);
                if (i == -1 || j.Rendido) continue;
                puntos[i] += PuntosPorLugar[lugar];
                if (lugar == 0) ganadas[i]++;
            }
            if (!hayPersonas) return null;

            partidas++;
            string aviso;
            if (partidas < PARTIDAS)
                aviso = $"Partida {partidas} de {PARTIDAS} de la temporada {numero} ({Nombres[TemaActual]}). " +
                        $"Puntos: 1o +{PuntosPorLugar[0]}, 2o +{PuntosPorLugar[1]}, 3o +{PuntosPorLugar[2]}, 4o +{PuntosPorLugar[3]}.";
            else
                aviso = Cerrar();
            Guardar();
            return aviso;
        }

        // Se acaba la temporada: premios a los 3 primeros y empieza la siguiente con otro tema
        private static string Cerrar()
        {
            Ordenar();
            string aviso = $"¡SE ACABÓ LA TEMPORADA {numero}! ";
            for (int k = 0; k < PremioMonedas.Length && k < cantidad; k++)
            {
                Perfil p = Perfiles.Buscar(nombres[k]);
                if (p == null) continue;
                p.Ganar(0, PremioMonedas[k]);
                if (k == 0) Logros.Dar(p, Logros.CAMPEON_TEMPORADA);
                aviso += $"{k + 1}o {nombres[k]} ({puntos[k]} pts, +{PremioMonedas[k]} monedas). ";
            }
            Perfiles.Guardar();
            ultimoCampeon = cantidad > 0 ? nombres[0] : "";
            numero++;
            partidas = 0;
            cantidad = 0;
            aviso += $"Ahora empieza {Nombres[TemaActual]}.";
            return aviso;
        }
    }
}
