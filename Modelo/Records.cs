namespace PrograProyectoFinal
{
    // Top 10 histórico de las mejores partidas (solo personas).
    // Archivo: %AppData%\Concesionario\records.txt   (nombre;patrimonio;fecha;modo)
    internal static class Records
    {
        public const int TOP = 10;

        private static string[] nombres = new string[TOP + 1];
        private static double[] patrimonios = new double[TOP + 1];
        private static string[] fechas = new string[TOP + 1];
        private static string[] modos = new string[TOP + 1];
        private static int cantidad = 0;

        private static string Ruta => Path.Combine(Path.GetDirectoryName(Perfiles.Ruta), "records.txt");

        public static int Cantidad => cantidad;
        public static string Nombre(int i) => nombres[i];
        public static double Patrimonio(int i) => patrimonios[i];
        public static string Fecha(int i) => fechas[i];
        public static string Modo(int i) => modos[i];

        public static void Cargar()
        {
            cantidad = 0;
            if (!File.Exists(Ruta)) return;
            try
            {
                using (StreamReader lector = new StreamReader(Ruta))
                {
                    string linea;
                    while ((linea = lector.ReadLine()) != null && cantidad < TOP)
                    {
                        string[] c = linea.Split(';');
                        if (c.Length < 4) continue;
                        nombres[cantidad] = c[0];
                        double.TryParse(c[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out patrimonios[cantidad]);
                        fechas[cantidad] = c[2];
                        modos[cantidad] = c[3];
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
                    for (int i = 0; i < cantidad; i++)
                        escritor.WriteLine($"{nombres[i]};{patrimonios[i].ToString(System.Globalization.CultureInfo.InvariantCulture)};{fechas[i]};{modos[i]}");
            }
            catch (IOException)
            {
            }
        }

        // Agrega la partida, ordena con burbuja y se queda con las 10 mejores.
        // Regresa el lugar que ocupó (1 a 10) o 0 si no entró.
        public static int Agregar(string nombre, double patrimonio, string modo)
        {
            Cargar();
            nombres[cantidad] = nombre;
            patrimonios[cantidad] = patrimonio;
            fechas[cantidad] = DateTime.Today.ToString("dd/MM/yyyy");
            modos[cantidad] = modo;
            int nuevo = cantidad;
            cantidad++;

            for (int i = 0; i < cantidad - 1; i++)
                for (int j = 0; j < cantidad - 1 - i; j++)
                    if (patrimonios[j] < patrimonios[j + 1])
                    {
                        double p = patrimonios[j]; patrimonios[j] = patrimonios[j + 1]; patrimonios[j + 1] = p;
                        string n = nombres[j]; nombres[j] = nombres[j + 1]; nombres[j + 1] = n;
                        string f = fechas[j]; fechas[j] = fechas[j + 1]; fechas[j + 1] = f;
                        string m = modos[j]; modos[j] = modos[j + 1]; modos[j + 1] = m;
                        if (nuevo == j) nuevo = j + 1;
                        else if (nuevo == j + 1) nuevo = j;
                    }

            if (cantidad > TOP) cantidad = TOP;
            Guardar();
            return nuevo < TOP ? nuevo + 1 : 0;
        }
    }
}
