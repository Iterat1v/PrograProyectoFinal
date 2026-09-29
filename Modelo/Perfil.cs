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

        // Premio al terminar una partida. Regresa true si subió de nivel.
        public bool Ganar(int xpGanada, int monedasGanadas)
        {
            int antes = Nivel;
            xp += xpGanada;
            monedas += monedasGanadas;
            return Nivel > antes;
        }

        // ----- archivo: una línea por perfil -----
        // nombre;xp;monedas;personalizado;avatar;comprados
        public string ALinea()
        {
            string compras = "";
            for (int p = 0; p < Avatar.PARTES; p++)
            {
                if (p > 0) compras += "|";
                for (int o = 0; o < Avatar.Opciones[p].Length; o++)
                    compras += comprado[p, o] ? "1" : "0";
            }
            return $"{nombre};{xp};{monedas};{(personalizado ? 1 : 0)};{avatar};{compras}";
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
            return p;
        }
    }
}
