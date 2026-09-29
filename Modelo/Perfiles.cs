namespace PrograProyectoFinal
{
    // Todos los perfiles guardados en un archivo de texto en la compu:
    // %AppData%\Concesionario\perfiles.txt (una línea por jugador)
    internal static class Perfiles
    {
        public const int MAX_PERFILES = 100;

        private static Perfil[] perfiles = new Perfil[MAX_PERFILES];
        private static int cantidad = 0;

        public static string Ruta => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Concesionario", "perfiles.txt");

        // Lee el archivo línea por línea con StreamReader
        public static void Cargar()
        {
            cantidad = 0;
            if (!File.Exists(Ruta)) return;
            try
            {
                using (StreamReader lector = new StreamReader(Ruta))
                {
                    string linea;
                    while ((linea = lector.ReadLine()) != null && cantidad < MAX_PERFILES)
                    {
                        Perfil p = Perfil.DesdeLinea(linea);
                        if (p != null)
                            perfiles[cantidad++] = p;
                    }
                }
            }
            catch (IOException)
            {
                cantidad = 0;      // si el archivo está dañado, se empieza de cero
            }
        }

        // Escribe todos los perfiles con StreamWriter
        public static void Guardar()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Ruta));
                using (StreamWriter escritor = new StreamWriter(Ruta))
                {
                    for (int i = 0; i < cantidad; i++)
                        escritor.WriteLine(perfiles[i].ALinea());
                }
            }
            catch (IOException)
            {
                // si no se pudo guardar, el juego sigue
            }
        }

        // Búsqueda lineal por nombre (sin importar mayúsculas)
        public static Perfil Buscar(string nombre)
        {
            for (int i = 0; i < cantidad; i++)
                if (perfiles[i].Nombre.ToLower() == nombre.ToLower())
                    return perfiles[i];
            return null;
        }

        public static Perfil ObtenerOCrear(string nombre)
        {
            Perfil p = Buscar(nombre);
            if (p == null && cantidad < MAX_PERFILES)
            {
                p = new Perfil(nombre.Replace(";", ","));
                perfiles[cantidad++] = p;
                Guardar();
            }
            return p;
        }
    }
}
