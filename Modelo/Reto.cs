namespace PrograProyectoFinal
{
    // Reto del día: la fecha (aaaammdd) es la semilla del Random de la partida,
    // así todos los que juegan hoy tienen la misma zona, cartas, autos chocados y eventos.
    // Archivo: %AppData%\Concesionario\retos.txt   (fecha;nombre;patrimonio)
    internal static class Reto
    {
        public const int TOP = 5;

        // Rivales fijos: un bot MEDIO y un bot PRO
        public static readonly string[] Rivales = { "Bot Chayo", "Pro Tiburón" };
        public static readonly int[] NivelRivales = { Bot.MEDIO, Bot.PRO };

        private static string Ruta => Path.Combine(Path.GetDirectoryName(Perfiles.Ruta), "retos.txt");

        public static int Fecha(DateTime dia) => dia.Year * 10000 + dia.Month * 100 + dia.Day;
        public static int Hoy => Fecha(DateTime.Today);
        public static int Ayer => Fecha(DateTime.Today.AddDays(-1));
        public static int Zona => Hoy % Partida.Zonas.Length;

        public static Partida CrearPartida(string nombre, int vendedor)
        {
            Partida p = new Partida(Zona, null, Hoy);
            Concesionario tu = new Concesionario(nombre, vendedor);
            tu.Perfil = Perfiles.ObtenerOCrear(nombre);
            if (tu.Perfil != null && tu.Perfil.Personalizado) tu.Cara = tu.Perfil.Avatar.Dibujar();
            p.AgregarJugador(tu);
            for (int i = 0; i < Rivales.Length; i++)
                p.AgregarJugador(new Concesionario(Rivales[i], (vendedor + 1 + i) % 4, NivelRivales[i]));
            return p;
        }

        public static void Registrar(string nombre, double patrimonio)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Ruta));
                using (StreamWriter escritor = new StreamWriter(Ruta, true))      // true = agrega al final
                    escritor.WriteLine($"{Hoy};{nombre};{Math.Round(patrimonio)}");
            }
            catch (IOException)
            {
            }
        }

        // La mejor partida de hoy de cada persona, ordenadas con burbuja.
        // Regresa cuántas hay (máximo TOP).
        public static int MejoresDeHoy(string[] nombres, double[] puntos)
        {
            int n = 0;
            string[] todos = new string[200];
            double[] mejor = new double[200];
            if (File.Exists(Ruta))
            {
                try
                {
                    using (StreamReader lector = new StreamReader(Ruta))
                    {
                        string linea;
                        while ((linea = lector.ReadLine()) != null)
                        {
                            string[] c = linea.Split(';');
                            if (c.Length < 3 || c[0] != Hoy.ToString()) continue;
                            double.TryParse(c[2], out double valor);
                            int k = 0;
                            while (k < n && todos[k].ToLower() != c[1].ToLower()) k++;
                            if (k == n)
                            {
                                if (n == todos.Length) continue;
                                todos[n] = c[1];
                                mejor[n] = valor;
                                n++;
                            }
                            else if (valor > mejor[k])
                                mejor[k] = valor;
                        }
                    }
                }
                catch (IOException)
                {
                }
            }

            for (int i = 0; i < n - 1; i++)
                for (int j = 0; j < n - 1 - i; j++)
                    if (mejor[j] < mejor[j + 1])
                    {
                        double t = mejor[j]; mejor[j] = mejor[j + 1]; mejor[j + 1] = t;
                        string s = todos[j]; todos[j] = todos[j + 1]; todos[j + 1] = s;
                    }

            int cuantos = Math.Min(n, Math.Min(TOP, nombres.Length));
            for (int i = 0; i < cuantos; i++)
            {
                nombres[i] = todos[i];
                puntos[i] = mejor[i];
            }
            return cuantos;
        }

        // Racha: si jugaste ayer sube, si ya jugaste hoy se queda, si no vuelve a 1.
        // Regresa las monedas de premio (solo la primera partida del día).
        public static int ActualizarRacha(Perfil p)
        {
            if (p.UltimoReto == Hoy) return 0;
            p.Racha = p.UltimoReto == Ayer ? p.Racha + 1 : 1;
            p.UltimoReto = Hoy;
            if (p.Racha >= 3) Logros.Dar(p, Logros.RACHA_3);
            if (p.Racha >= 7) Logros.Dar(p, Logros.RACHA_7);
            int monedas = 20 * Math.Min(p.Racha, 7);
            p.Ganar(0, monedas);
            return monedas;
        }
    }
}
