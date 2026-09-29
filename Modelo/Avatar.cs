namespace PrograProyectoFinal
{
    // El personaje del jugador: 8 partes que se escogen en la tienda.
    // Dibujar() arma un sprite de 16 x 16 con rectángulos, igual que los mockups.
    internal class Avatar
    {
        public const int TEZ = 0, PEINADO = 1, COLOR_PELO = 2, ROPA = 3, LENTES = 4, GORRO = 5, MASCARA = 6, EXTRA = 7;
        public const int PARTES = 8;

        public static readonly string[] NombresParte = { "TEZ", "PEINADO", "COLOR DE PELO", "ROPA", "LENTES", "GORROS", "MÁSCARAS", "EXTRAS" };

        public static readonly string[][] Opciones =
        {
            new[] { "Clara", "Media", "Morena", "Canela", "Oscura", "Alien", "Avatar" },
            new[] { "Corto", "Largo", "Chongo", "Pelón", "Coleta", "Afro", "Mohicano", "Súper Saiyajin" },
            new[] { "Negro", "Café", "Castaño", "Rubio", "Pelirrojo", "Canoso", "Rosa", "Azul", "Verde", "Dorado" },
            new[] { "Azul", "Rosa", "Aqua", "Rojo", "Verde", "Morado", "Negro", "Blanco", "Naranja", "Oro" },
            new[] { "Sin lentes", "Lentes", "Oscuros", "3D", "Corazón", "Monóculo", "Scouter" },
            new[] { "Sin gorro", "Gorra", "Sombrero", "Navideño", "Casco", "Cuernos", "Aureola", "Corona" },
            new[] { "Sin máscara", "Cubrebocas", "Ninja", "Luchador", "Calaverita", "Zombie", "Robot" },
            new[] { "Nada", "Corbata", "Cuadros", "Bigote", "Aretes", "Chapitas", "Collar de oro", "Barba" }
        };

        private static readonly Color[] tez = { Hex("#ffe0c4"), Hex("#f2c29b"), Hex("#c98e62"), Hex("#a86b45"), Hex("#8d5a3b"), Hex("#7fb069"), Hex("#6fa8dc") };
        private static readonly Color[] pelo = { Hex("#1c1020"), Hex("#3b2416"), Hex("#5a2d1a"), Hex("#d9a441"), Hex("#c2410c"), Hex("#dcdcdc"), Hex("#ff5fa2"), Hex("#3f6fd8"), Hex("#5fd068"), Hex("#ffd447") };
        private static readonly Color[] ropa = { Hex("#3f6fd8"), Hex("#ff5fa2"), Hex("#2ee6c8"), Hex("#ff4d4d"), Hex("#4c9a5b"), Hex("#8b5cf6"), Hex("#2a2438"), Hex("#fff4e8"), Hex("#f28c28"), Hex("#ffd447") };

        private int[] partes = new int[PARTES];

        public Avatar()
        {
            partes[TEZ] = 1;
            partes[COLOR_PELO] = 1;
        }

        public int Parte(int cual) => partes[cual];

        public void Cambiar(int cual, int opcion)
        {
            if (opcion < 0 || opcion >= Opciones[cual].Length)
                throw new ArgumentOutOfRangeException("Esa opción no existe");
            partes[cual] = opcion;
        }

        // Para guardarlo en el archivo: "1,0,1,0,0,0,0,0"
        public override string ToString()
        {
            return string.Join(",", partes);
        }

        public static Avatar DesdeTexto(string texto)
        {
            Avatar a = new Avatar();
            string[] numeros = texto.Split(',');
            for (int i = 0; i < PARTES && i < numeros.Length; i++)
                if (int.TryParse(numeros[i], out int n) && n >= 0 && n < Opciones[i].Length)
                    a.partes[i] = n;
            return a;
        }

        public Avatar Copia()
        {
            return DesdeTexto(ToString());
        }

        private static Color Hex(string h) => ColorTranslator.FromHtml(h);

        public Bitmap Dibujar()
        {
            Bitmap b = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(b))
            {
                Color N = Paleta.Negro;
                Color piel = tez[partes[TEZ]];
                Color cab = pelo[partes[COLOR_PELO]];
                Color camisa = ropa[partes[ROPA]];
                void P(int x, int y, int w, int h, Color c)
                {
                    using (SolidBrush brocha = new SolidBrush(c))
                        g.FillRectangle(brocha, x, y, w, h);
                }

                // cuerpo
                P(2, 11, 12, 5, N); P(3, 11, 10, 5, camisa);
                switch (partes[EXTRA])
                {
                    case 1: P(7, 11, 2, 4, Paleta.Rojo); P(6, 11, 1, 1, Paleta.Crema); P(9, 11, 1, 1, Paleta.Crema); break;
                    case 2: for (int i = 3; i < 13; i += 2) { P(i, 12, 1, 1, Paleta.Negro); P(i + 1, 14, 1, 1, Paleta.Negro); } break;
                    case 6: P(5, 11, 6, 1, Paleta.Oro); P(7, 12, 2, 1, Paleta.Oro); break;
                }

                // cabeza
                P(3, 3, 10, 9, N); P(4, 4, 8, 7, piel);

                // pelo
                switch (partes[PEINADO])
                {
                    case 0: P(3, 2, 10, 3, N); P(4, 3, 8, 2, cab); P(4, 5, 1, 2, cab); P(11, 5, 1, 2, cab); break;
                    case 1: P(3, 2, 10, 3, N); P(4, 3, 8, 2, cab); P(2, 4, 2, 7, cab); P(12, 4, 2, 7, cab); break;
                    case 2: P(5, 0, 6, 3, N); P(6, 0, 4, 2, cab); P(3, 2, 10, 3, N); P(4, 3, 8, 2, cab); P(4, 5, 1, 4, cab); P(11, 5, 1, 4, cab); break;
                    case 3: P(6, 4, 3, 1, Pixel.Aclarar(piel, 0.35f)); break;
                    case 4: P(3, 2, 10, 3, N); P(4, 3, 8, 2, cab); P(12, 5, 3, 5, N); P(13, 6, 1, 3, cab); break;
                    case 5: P(1, 0, 14, 5, N); P(2, 1, 12, 4, cab); P(1, 5, 3, 5, N); P(2, 5, 2, 4, cab); P(12, 5, 3, 5, N); P(12, 5, 2, 4, cab); break;
                    case 6: P(6, 0, 4, 4, N); P(7, 0, 2, 4, cab); P(3, 3, 10, 1, N); break;
                    default:
                        P(2, 1, 12, 4, N); P(3, 2, 10, 2, cab);
                        P(1, 0, 3, 2, cab); P(5, 0, 2, 2, cab); P(9, 0, 2, 2, cab); P(12, 0, 3, 2, cab); P(0, 2, 2, 3, cab); P(14, 2, 2, 3, cab);
                        P(4, 4, 1, 2, cab); P(11, 4, 1, 2, cab);
                        break;
                }

                // cara
                P(6, 7, 1, 1, N); P(9, 7, 1, 1, N);
                if (partes[EXTRA] == 3) P(5, 9, 6, 1, cab);
                else if (partes[EXTRA] == 7) { P(4, 9, 8, 2, cab); P(6, 11, 4, 1, cab); P(7, 9, 2, 1, Paleta.Rosa2); }
                else P(7, 9, 2, 1, Paleta.Rosa2);
                if (partes[EXTRA] == 4) { P(3, 8, 1, 2, Paleta.Oro); P(12, 8, 1, 2, Paleta.Oro); }
                if (partes[EXTRA] == 5) { P(5, 8, 1, 1, Hex("#f59aa0")); P(10, 8, 1, 1, Hex("#f59aa0")); }

                // máscara
                switch (partes[MASCARA])
                {
                    case 1: P(4, 8, 8, 3, Paleta.Cielo); P(3, 8, 1, 1, Paleta.Crema); P(12, 8, 1, 1, Paleta.Crema); break;
                    case 2: P(4, 4, 8, 7, N); P(4, 6, 8, 2, piel); P(6, 7, 1, 1, N); P(9, 7, 1, 1, N); P(3, 5, 10, 1, Paleta.Rojo); P(13, 5, 2, 1, Paleta.Rojo); break;
                    case 3:
                        Color lucha = Paleta.Rojo;
                        P(3, 2, 10, 2, N); P(4, 3, 8, 1, lucha); P(4, 4, 8, 7, lucha);
                        P(5, 6, 2, 2, Paleta.Crema); P(9, 6, 2, 2, Paleta.Crema); P(6, 7, 1, 1, N); P(9, 7, 1, 1, N);
                        P(7, 9, 2, 1, N); P(7, 4, 2, 2, Paleta.Oro); P(4, 5, 1, 1, Paleta.Oro); P(11, 5, 1, 1, Paleta.Oro);
                        break;
                    case 4:
                        P(4, 4, 8, 7, Paleta.Crema); P(5, 6, 2, 2, N); P(9, 6, 2, 2, N); P(7, 8, 2, 1, N);
                        P(5, 9, 6, 1, N); P(6, 9, 1, 1, Paleta.Crema); P(8, 9, 1, 1, Paleta.Crema); P(10, 9, 1, 1, Paleta.Crema);
                        P(4, 4, 2, 1, Paleta.Rosa); P(10, 4, 2, 1, Paleta.Agua);
                        break;
                    case 5: P(4, 4, 8, 7, Hex("#7fb069")); P(6, 7, 1, 1, Paleta.Rojo); P(9, 7, 1, 1, Paleta.Rojo); P(5, 5, 3, 1, N); P(6, 9, 4, 1, N); P(7, 10, 1, 1, Paleta.Crema); break;
                    case 6: P(4, 4, 8, 7, Paleta.Gris); P(5, 6, 2, 2, Paleta.Cielo); P(9, 6, 2, 2, Paleta.Cielo); P(5, 9, 6, 1, N); P(6, 9, 1, 1, Paleta.Gris); P(8, 9, 1, 1, Paleta.Gris); P(7, 2, 2, 2, N); P(7, 1, 2, 1, Paleta.Rojo); break;
                }

                // lentes
                switch (partes[LENTES])
                {
                    case 1: P(4, 6, 3, 3, N); P(9, 6, 3, 3, N); P(7, 7, 2, 1, N); P(5, 7, 1, 1, Paleta.Cielo); P(10, 7, 1, 1, Paleta.Cielo); break;
                    case 2: P(4, 6, 8, 2, N); P(5, 6, 2, 1, Paleta.Gris); P(9, 6, 2, 1, Paleta.Gris); break;
                    case 3: P(4, 6, 8, 2, N); P(5, 6, 2, 1, Paleta.Rojo); P(9, 6, 2, 1, Paleta.Cielo); break;
                    case 4: P(4, 6, 3, 2, Paleta.Rosa); P(9, 6, 3, 2, Paleta.Rosa); P(5, 8, 1, 1, Paleta.Rosa); P(10, 8, 1, 1, Paleta.Rosa); P(7, 6, 2, 1, N); break;
                    case 5: P(9, 6, 3, 3, Paleta.Oro); P(10, 7, 1, 1, Paleta.Cielo); P(11, 9, 1, 2, Paleta.Oro); break;
                    case 6: P(10, 5, 4, 4, N); P(10, 6, 3, 2, Paleta.Verde); P(13, 3, 1, 3, N); break;
                }

                // gorro
                switch (partes[GORRO])
                {
                    case 1: P(3, 1, 10, 4, N); P(4, 2, 8, 2, Paleta.Rojo); P(10, 4, 5, 1, N); P(10, 3, 4, 1, Paleta.Rojo); break;
                    case 2: P(5, 0, 6, 3, N); P(6, 1, 4, 2, Hex("#d8b46a")); P(1, 3, 14, 2, N); P(2, 3, 12, 1, Hex("#d8b46a")); break;
                    case 3: P(3, 1, 10, 3, N); P(4, 1, 8, 2, Paleta.Rojo); P(10, 0, 4, 2, Paleta.Rojo); P(3, 3, 10, 1, Paleta.Crema); P(13, 0, 2, 2, Paleta.Crema); break;
                    case 4: P(2, 1, 12, 6, N); P(3, 2, 10, 4, Paleta.Azul); P(4, 5, 8, 3, N); P(5, 6, 6, 1, Paleta.Cielo); break;
                    case 5: P(3, 0, 2, 3, N); P(3, 0, 1, 2, Paleta.Rojo); P(11, 0, 2, 3, N); P(12, 0, 1, 2, Paleta.Rojo); break;
                    case 6: P(4, 0, 8, 1, Paleta.Oro); P(3, 1, 1, 1, Paleta.Oro); P(12, 1, 1, 1, Paleta.Oro); break;
                    case 7: P(3, 1, 10, 3, N); P(4, 1, 8, 2, Paleta.Oro); P(4, 0, 1, 1, Paleta.Oro); P(7, 0, 2, 1, Paleta.Oro); P(11, 0, 1, 1, Paleta.Oro); P(7, 2, 2, 1, Paleta.Rosa); break;
                }
            }
            return b;
        }
    }
}
