namespace PrograProyectoFinal
{
    // Modo historia: 5 semanas, una por zona de la ciudad (de Ecatepec a Polanco).
    // Cada semana es una partida contra un jefe. Las decisiones cambian el final.
    internal class Historia
    {
        public const int SEMANAS = 5;

        // Decisiones que aparecen en ciertos días
        public const int COYOTE = 0, INFLUENCER = 1, MORDIDA = 2;

        public static readonly string[] Jefes = { "El Chato", "La Chayo", "Lic. Garza", "Inspector SAT", "EL BRO" };
        public static readonly int[] NivelJefe = { Bot.BOTARDO, Bot.MEDIO, Bot.PRO, Bot.PRO, Bot.DIOS };
        public static readonly int[] RetratoJefe = { 5, 1, 4, 7, 8 };     // índice en Recursos.Clientes
        public static readonly string[] Intros =
        {
            "Empiezas con un lote de tierra, tres llantas y un letrero pintado a mano. El Chato dice que en Ecatepec el que vende es él.",
            "En Neza te topas con La Chayo: vende hasta los tapetes. Cuidado, que por aquí también anda un coyote ofreciendo autos \"baratos\".",
            "Coyoacán, café de especialidad y clientes exigentes. El Lic. Garza te ve como competencia. Un influencer quiere hacerte promoción.",
            "Santa Fe: torres de vidrio y un inspector del SAT que revisa todo. Si tu reputación está baja, te multa. Y si le compraste al coyote...",
            "Polanco. El último jefe es El Bro: sabe cuánto trae cada cliente. Si le ganas, eres el rey de la CDMX."
        };

        private string nombre;
        private int vendedor;
        private int semana = 0;          // la que sigue por jugar (5 = terminó)
        private bool coyote = false;     // le compró un auto robado al coyote
        private bool influencer = false;
        private bool mordida = false;    // le dio mordida al inspector
        private int reputacion = 50;     // con la que terminó la última semana

        public Historia(string nombre, int vendedor)
        {
            this.nombre = nombre;
            this.vendedor = vendedor;
        }

        public string Nombre => nombre;
        public int Vendedor { get => vendedor; set => vendedor = value; }
        public int Semana => semana;
        public bool Terminada => semana >= SEMANAS;
        public bool Coyote { get => coyote; set => coyote = value; }
        public bool Influencer { get => influencer; set => influencer = value; }
        public bool Mordida { get => mordida; set => mordida = value; }
        public int Reputacion { get => reputacion; set => reputacion = value; }

        public void GanoSemana()
        {
            semana++;
        }

        // Qué decisión toca en este día de esta semana (-1 = ninguna)
        public int Decision(int dia)
        {
            if (semana == 1 && dia == 2) return COYOTE;
            if (semana == 2 && dia == 2) return INFLUENCER;
            if (semana == 3 && dia == 2) return MORDIDA;
            return -1;
        }

        // Final según las decisiones y la reputación con la que terminó
        public string[] Final(int reputacion)
        {
            if (!coyote && !mordida && reputacion >= 60)
                return new[] { "EL LOTE MÁS HONESTO DE POLANCO",
                    "Nunca le compraste al coyote ni le diste mordida a nadie. Don Beto te recomienda con toda su familia y tu lote sale en el periódico." };
            if (coyote || mordida)
                return new[] { "EL REY DE LA TRANZA",
                    "Tienes el lote más grande de Polanco... y una carpeta con tu nombre en el SAT. Fin. ¿O no?" };
            return new[] { "DE ECATEPEC A POLANCO",
                "Llegaste. Con tierra en los zapatos y deudas con tu compadre, pero llegaste." };
        }

        // ----- archivo: %AppData%\Concesionario\historias.txt (una línea por jugador) -----
        private static string Ruta => Path.Combine(Path.GetDirectoryName(Perfiles.Ruta), "historias.txt");

        public string ALinea() => $"{nombre};{vendedor};{semana};{(coyote ? 1 : 0)};{(influencer ? 1 : 0)};{(mordida ? 1 : 0)};{reputacion}";

        public static Historia Cargar(string nombre)
        {
            if (!File.Exists(Ruta)) return null;
            using (StreamReader lector = new StreamReader(Ruta))
            {
                string linea;
                while ((linea = lector.ReadLine()) != null)
                {
                    string[] c = linea.Split(';');
                    if (c.Length < 6 || c[0].ToLower() != nombre.ToLower()) continue;
                    Historia h = new Historia(c[0], int.Parse(c[1]));
                    h.semana = int.Parse(c[2]);
                    h.coyote = c[3] == "1";
                    h.influencer = c[4] == "1";
                    h.mordida = c[5] == "1";
                    if (c.Length > 6) int.TryParse(c[6], out h.reputacion);
                    return h;
                }
            }
            return null;
        }

        // Reescribe el archivo cambiando solo la línea de este jugador
        public void Guardar()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Ruta));
                string[] lineas = File.Exists(Ruta) ? File.ReadAllLines(Ruta) : new string[0];
                using (StreamWriter escritor = new StreamWriter(Ruta))
                {
                    foreach (string l in lineas)
                        if (!l.ToLower().StartsWith(nombre.ToLower() + ";"))
                            escritor.WriteLine(l);
                    escritor.WriteLine(ALinea());
                }
            }
            catch (IOException)
            {
            }
        }
    }
}
