namespace PrograProyectoFinal
{
    // Lo que se guarda de cada jugador en la compu: nivel, monedas, personaje, lo que tiene,
    // logros, misiones, racha del reto y su avance en el pase de temporada
    internal class Perfil
    {
        public const int XP_POR_NIVEL = 300;

        private string nombre;
        private int xp = 0;
        private int monedas = 100;       // para que puedas comprar algo desde el inicio
        private bool personalizado = false;
        private Avatar avatar = new Avatar();
        private bool[,] comprado = new bool[Avatar.PARTES, Cosmeticos.MAX];      // lo que tiene en su casillero

        // Logros, misiones del día, racha del reto y si ya vio el tutorial
        private bool[] logros = new bool[Logros.CANTIDAD];
        private int fechaMisiones = 0;
        private int[] progreso = new int[Misiones.POR_DIA];
        private bool[] cobrada = new bool[Misiones.POR_DIA];
        private int ultimoReto = 0;
        private int racha = 0;
        private bool tutorialVisto = false;

        // Pase de temporada
        private int paseXp = 0;
        private bool pasePremium = false;
        private bool[] reclamadoGratis = new bool[Pase.NIVELES + 1];
        private bool[] reclamadoPremium = new bool[Pase.NIVELES + 1];

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
            return Cosmeticos.Origen(parte, opcion) == Cosmeticos.BASICO || comprado[parte, opcion];
        }

        // Lo que se gana en el pase entra directo al casillero
        public void Dar(int parte, int opcion) => comprado[parte, opcion] = true;

        public int CuantosTiene(int parte)
        {
            int n = 0;
            for (int o = 0; o < Cosmeticos.Cantidad(parte); o++)
                if (LoTiene(parte, o)) n++;
            return n;
        }

        // Compra en la tienda del día (puede venir con descuento). Regresa null si se compró.
        public string ComprarA(int parte, int opcion, int precio)
        {
            if (LoTiene(parte, opcion))
                return "Ya lo tienes.";
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
            paseXp += xpGanada;          // la misma XP sube el pase
            monedas += monedasGanadas;
            return Nivel > antes;
        }

        // ----- pase -----
        public int PaseXp => paseXp;
        public int NivelPase => Math.Min(Pase.NIVELES, paseXp / Pase.XP_POR_NIVEL);
        public bool PasePremium => pasePremium;
        public bool Reclamado(bool premium, int nivel) => premium ? reclamadoPremium[nivel] : reclamadoGratis[nivel];
        public void MarcarReclamado(bool premium, int nivel)
        {
            if (premium) reclamadoPremium[nivel] = true; else reclamadoGratis[nivel] = true;
        }

        public string ComprarPremium()
        {
            if (pasePremium) return "Ya tienes el pase premium.";
            if (monedas < Pase.PRECIO_PREMIUM) return $"Te faltan {Pase.PRECIO_PREMIUM - monedas} monedas.";
            monedas -= Pase.PRECIO_PREMIUM;
            pasePremium = true;
            return null;
        }

        public void GastarMonedas(int cuantas) => monedas -= cuantas;

        // ----- archivo: una línea por perfil -----
        // nombre;xp;monedas;personalizado;avatar;comprados;logros;fechaMisiones;progreso;cobradas;ultimoReto;racha;tutorial;
        // paseXp;premium;reclamadosGratis;reclamadosPremium
        // (los archivos viejos solo tienen los 6 primeros campos y también se leen)
        public string ALinea()
        {
            string compras = "";
            for (int p = 0; p < Avatar.PARTES; p++)
            {
                if (p > 0) compras += "|";
                for (int o = 0; o < Cosmeticos.Cantidad(p); o++)
                    compras += comprado[p, o] ? "1" : "0";
            }
            string textoLogros = "";
            for (int i = 0; i < logros.Length; i++)
                textoLogros += logros[i] ? "1" : "0";
            string cobradas = "";
            for (int k = 0; k < cobrada.Length; k++)
                cobradas += cobrada[k] ? "1" : "0";
            string gratis = "", premium = "";
            for (int n = 0; n <= Pase.NIVELES; n++)
            {
                gratis += reclamadoGratis[n] ? "1" : "0";
                premium += reclamadoPremium[n] ? "1" : "0";
            }
            return $"{nombre};{xp};{monedas};{(personalizado ? 1 : 0)};{avatar};{compras};" +
                   $"{textoLogros};{fechaMisiones};{progreso[0]},{progreso[1]},{progreso[2]};{cobradas};{ultimoReto};{racha};{(tutorialVisto ? 1 : 0)};" +
                   $"{paseXp};{(pasePremium ? 1 : 0)};{gratis};{premium}";
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
                for (int o = 0; o < partes[i].Length && o < Cosmeticos.Cantidad(i); o++)
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
            if (campos.Length >= 17)
            {
                int.TryParse(campos[13], out p.paseXp);
                p.pasePremium = campos[14] == "1";
                for (int n = 0; n < campos[15].Length && n < p.reclamadoGratis.Length; n++)
                    p.reclamadoGratis[n] = campos[15][n] == '1';
                for (int n = 0; n < campos[16].Length && n < p.reclamadoPremium.Length; n++)
                    p.reclamadoPremium[n] = campos[16][n] == '1';
            }
            return p;
        }
    }
}
