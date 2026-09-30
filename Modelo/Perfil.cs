namespace PrograProyectoFinal
{
    // Lo que se guarda de cada jugador en la compu: nivel, monedas, personaje y lo que compró
    internal class Perfil
    {
        public const int XP_POR_NIVEL = 300;

        // Precio y nivel de cada opción de la tienda: [parte][opción]
        public static readonly int[][] Precios =
        {
            new[] { 0, 0, 0, 0, 0, 300, 300 },
            new[] { 0, 0, 0, 0, 80, 120, 150, 800 },
            new[] { 0, 0, 0, 0, 40, 40, 100, 100, 100, 300 },
            new[] { 0, 0, 0, 0, 0, 50, 50, 50, 80, 250 },
            new[] { 0, 0, 80, 120, 150, 200, 500 },
            new[] { 0, 60, 100, 150, 200, 250, 400, 1000 },
            new[] { 0, 40, 200, 300, 300, 250, 450 },
            new[] { 0, 30, 50, 60, 80, 40, 350, 150 }
        };
        public static readonly int[][] Niveles =
        {
            new[] { 1, 1, 1, 1, 1, 3, 3 },
            new[] { 1, 1, 1, 1, 1, 2, 2, 5 },
            new[] { 1, 1, 1, 1, 1, 1, 2, 2, 2, 4 },
            new[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 3 },
            new[] { 1, 1, 1, 1, 2, 2, 4 },
            new[] { 1, 1, 1, 2, 2, 2, 3, 6 },
            new[] { 1, 1, 2, 2, 2, 2, 3 },
            new[] { 1, 1, 1, 1, 1, 1, 3, 2 }
        };

        private string nombre;
        private int xp = 0;
        private int monedas = 100;       // para que puedas comprar algo desde el inicio
        private bool personalizado = false;
        private Avatar avatar = new Avatar();
        private bool[,] comprado = new bool[Avatar.PARTES, 10];

        // Logros, misiones del día, racha del reto y si ya vio el tutorial
        private bool[] logros = new bool[Logros.CANTIDAD];
        private int fechaMisiones = 0;
        private int[] progreso = new int[Misiones.POR_DIA];
        private bool[] cobrada = new bool[Misiones.POR_DIA];
        private int ultimoReto = 0;
        private int racha = 0;
        private bool tutorialVisto = false;

        public Perfil(string nombre)
        {
            this.nombre = nombre;
        }

        public string Nombre => nombre;
        public int Xp => xp;
        public int Monedas => monedas;
        public int Nivel => 1 + xp / XP_POR_NIVEL;
        public int XpEnNivel => xp % XP_POR_NIVEL;
        public Avatar Avatar => avatar;

        // Si nunca lo editó, en el juego sale el dibujo del vendedor
        public bool Personalizado
        {
            get => personalizado;
            set => personalizado = value;
        }

        public bool TieneLogro(int i) => logros[i];
        public void MarcarLogro(int i) => logros[i] = true;

        public int CuantosLogros()
        {
            int n = 0;
            for (int i = 0; i < logros.Length; i++)
                if (logros[i]) n++;
            return n;
        }

        // Si cambió el día, las misiones empiezan de cero
        public void PrepararMisiones(int hoy)
        {
            if (fechaMisiones == hoy) return;
            fechaMisiones = hoy;
            for (int k = 0; k < Misiones.POR_DIA; k++)
            {
                progreso[k] = 0;
                cobrada[k] = false;
            }
        }

        public int Progreso(int k) => progreso[k];
        public bool Cobrada(int k) => cobrada[k];
        public void Avanzar(int k, int cuanto) => progreso[k] += cuanto;
        public void Cobrar(int k) => cobrada[k] = true;

        public int UltimoReto { get => ultimoReto; set => ultimoReto = value; }
        public int Racha { get => racha; set => racha = value; }
        public bool TutorialVisto { get => tutorialVisto; set => tutorialVisto = value; }

        public bool LoTiene(int parte, int opcion)
        {
            return Precios[parte][opcion] == 0 || comprado[parte, opcion];
        }

        // Regresa null si se compró; si no, por qué no
        public string Comprar(int parte, int opcion)
        {
            if (LoTiene(parte, opcion))
                return "Ya lo tienes.";
            if (Nivel < Niveles[parte][opcion])
                return $"Se desbloquea en el nivel {Niveles[parte][opcion]}.";
            if (monedas < Precios[parte][opcion])
                return $"Te faltan {Precios[parte][opcion] - monedas} monedas.";
            monedas -= Precios[parte][opcion];
            comprado[parte, opcion] = true;
            return null;
        }

        // Compra en la tienda del día (puede venir con descuento)
        public string ComprarA(int parte, int opcion, int precio)
        {
            if (LoTiene(parte, opcion))
                return "Ya lo tienes.";
            if (Nivel < Niveles[parte][opcion])
                return $"Se desbloquea en el nivel {Niveles[parte][opcion]}.";
            if (monedas < precio)
                return $"Te faltan {precio - monedas} monedas.";
            monedas -= precio;
            comprado[parte, opcion] = true;
            return null;
        }

        // Premio al terminar una partida. Regresa true si subió de nivel.
        public bool Ganar(int xpGanada, int monedasGanadas)
        {
            int antes = Nivel;
            xp += xpGanada;
            monedas += monedasGanadas;
            return Nivel > antes;
        }

        // ----- archivo: una línea por perfil -----
        // nombre;xp;monedas;personalizado;avatar;comprados;logros;fechaMisiones;progreso;cobradas;ultimoReto;racha;tutorial
        // (los archivos viejos solo tienen los 6 primeros campos y también se leen)
        public string ALinea()
        {
            string compras = "";
            for (int p = 0; p < Avatar.PARTES; p++)
            {
                if (p > 0) compras += "|";
                for (int o = 0; o < Avatar.Opciones[p].Length; o++)
                    compras += comprado[p, o] ? "1" : "0";
            }
            string textoLogros = "";
            for (int i = 0; i < logros.Length; i++)
                textoLogros += logros[i] ? "1" : "0";
            string cobradas = "";
            for (int k = 0; k < cobrada.Length; k++)
                cobradas += cobrada[k] ? "1" : "0";
            return $"{nombre};{xp};{monedas};{(personalizado ? 1 : 0)};{avatar};{compras};" +
                   $"{textoLogros};{fechaMisiones};{progreso[0]},{progreso[1]},{progreso[2]};{cobradas};{ultimoReto};{racha};{(tutorialVisto ? 1 : 0)}";
        }

        public static Perfil DesdeLinea(string linea)
        {
            string[] campos = linea.Split(';');
            if (campos.Length < 6) return null;
            Perfil p = new Perfil(campos[0]);
            int.TryParse(campos[1], out p.xp);
            int.TryParse(campos[2], out p.monedas);
            p.personalizado = campos[3] == "1";
            p.avatar = Avatar.DesdeTexto(campos[4]);
            string[] partes = campos[5].Split('|');
            for (int i = 0; i < Avatar.PARTES && i < partes.Length; i++)
                for (int o = 0; o < partes[i].Length && o < 10; o++)
                    p.comprado[i, o] = partes[i][o] == '1';

            if (campos.Length >= 13)
            {
                for (int i = 0; i < campos[6].Length && i < p.logros.Length; i++)
                    p.logros[i] = campos[6][i] == '1';
                int.TryParse(campos[7], out p.fechaMisiones);
                string[] prog = campos[8].Split(',');
                for (int k = 0; k < prog.Length && k < p.progreso.Length; k++)
                    int.TryParse(prog[k], out p.progreso[k]);
                for (int k = 0; k < campos[9].Length && k < p.cobrada.Length; k++)
                    p.cobrada[k] = campos[9][k] == '1';
                int.TryParse(campos[10], out p.ultimoReto);
                int.TryParse(campos[11], out p.racha);
                p.tutorialVisto = campos[12] == "1";
            }
            return p;
        }
    }
}
