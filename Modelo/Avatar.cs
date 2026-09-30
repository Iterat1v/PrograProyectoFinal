namespace PrograProyectoFinal
{
    // El personaje del jugador: 9 partes (tez, peinado, color de pelo, ropa, lentes, gorro,
    // máscara, extra y fondo). Cada parte guarda el número del artículo del catálogo (Cosmeticos).
    // Dibujar() arma un sprite de 16 x 16 con rectángulos según la "forma" y los colores del artículo.
    internal class Avatar
    {
        public const int TEZ = 0, PEINADO = 1, COLOR_PELO = 2, ROPA = 3, LENTES = 4, GORRO = 5, MASCARA = 6, EXTRA = 7, FONDO = 8;
        public const int PARTES = 9;

        public static readonly string[] NombresParte = { "TEZ", "PEINADO", "COLOR DE PELO", "ROPA", "LENTES", "GORROS", "MÁSCARAS", "EXTRAS", "FONDOS" };

        private int[] partes = new int[PARTES];

        public Avatar()
        {
            partes[TEZ] = 1;
            partes[COLOR_PELO] = 1;
        }

        public int Parte(int cual) => partes[cual];

        public void Cambiar(int cual, int opcion)
        {
            if (opcion < 0 || opcion >= Cosmeticos.Cantidad(cual))
                throw new ArgumentOutOfRangeException("Esa opción no existe");
            partes[cual] = opcion;
        }

        // Para guardarlo en el archivo: "1,0,1,0,0,0,0,0,0"
        public override string ToString()
        {
            return string.Join(",", partes);
        }

        public static Avatar DesdeTexto(string texto)
        {
            Avatar a = new Avatar();
            string[] numeros = texto.Split(',');
            for (int i = 0; i < PARTES && i < numeros.Length; i++)
                if (int.TryParse(numeros[i], out int n) && n >= 0 && n < Cosmeticos.Cantidad(i))
                    a.partes[i] = n;
            return a;
        }

        public Avatar Copia()
        {
            return DesdeTexto(ToString());
        }

        public Bitmap Dibujar()
        {
            Bitmap b = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(b))
            {
                Color N = Paleta.Negro;
                Color piel = Cosmeticos.A(TEZ, partes[TEZ]);
                Color cab = Cosmeticos.A(COLOR_PELO, partes[COLOR_PELO]);
                Color cab2 = Cosmeticos.Forma(COLOR_PELO, partes[COLOR_PELO]) == 1 ? Cosmeticos.B(COLOR_PELO, partes[COLOR_PELO]) : cab;
                void P(int x, int y, int w, int h, Color c)
                {
                    using (SolidBrush brocha = new SolidBrush(c))
                        g.FillRectangle(brocha, x, y, w, h);
                }

                DibujarFondo(P);

                // ----- cuerpo con la ropa -----
                int ro = partes[ROPA];
                Color ra = Cosmeticos.A(ROPA, ro), rb = Cosmeticos.B(ROPA, ro);
                P(2, 11, 12, 5, N); P(3, 11, 10, 5, ra);
                switch (Cosmeticos.Forma(ROPA, ro))
                {
                    case 1: P(3, 12, 10, 1, rb); P(3, 14, 10, 1, rb); break;                                     // rayas
                    case 2: P(7, 11, 2, 5, rb); P(3, 11, 1, 5, rb); P(12, 11, 1, 5, rb); break;                 // jersey
                    case 3: for (int i = 3; i < 13; i += 2) { P(i, 12, 1, 1, rb); P(i + 1, 14, 1, 1, rb); } break; // franela
                    case 4: P(6, 11, 4, 1, rb); P(7, 12, 2, 1, rb); break;                                      // camisa con cuello
                    case 5: P(7, 11, 1, 5, rb); P(4, 11, 2, 1, rb); P(10, 11, 2, 1, rb); break;                 // sudadera con cierre
                    case 6: P(4, 12, 1, 1, rb); P(7, 13, 1, 1, rb); P(10, 12, 1, 1, rb); P(5, 14, 1, 1, rb); P(11, 14, 1, 1, rb); P(3, 15, 10, 1, rb); break; // suéter
                    case 7: P(3, 11, 10, 5, ra); P(5, 12, 1, 1, rb); P(10, 12, 1, 1, rb); P(5, 14, 1, 1, rb); P(10, 14, 1, 1, rb); P(7, 11, 2, 1, rb); break; // traje de charro
                    case 8: P(4, 12, 2, 2, rb); P(9, 13, 2, 2, rb); P(6, 15, 2, 1, rb); break;                  // vestido con flores
                }

                // extras que van sobre el pecho
                int ex = partes[EXTRA], fex = Cosmeticos.Forma(EXTRA, ex);
                Color xa = Cosmeticos.A(EXTRA, ex), xb = Cosmeticos.B(EXTRA, ex);
                if (fex == 1) { P(7, 11, 2, 4, xa); P(6, 11, 1, 1, xb); P(9, 11, 1, 1, xb); }                    // corbata
                if (fex == 6) { P(5, 11, 6, 1, xa); P(7, 12, 2, 1, xa); }                                        // collar
                if (fex == 10) { P(3, 10, 10, 2, xa); P(4, 11, 1, 1, xb); P(8, 11, 1, 1, xb); P(10, 12, 2, 3, xa); } // bufanda
                if (fex == 11) { P(7, 11, 2, 1, xb); P(7, 12, 2, 2, xa); }                                       // medalla
                if (fex == 12) { for (int i = 4; i < 12; i += 2) P(i, 11, 2, 1, i % 4 == 0 ? xa : xb); }          // collar de flores
                if (fex == 13) { P(6, 11, 1, 2, xa); P(9, 11, 1, 2, xa); P(7, 11, 2, 1, xa); }                   // moño

                // ----- cabeza -----
                P(3, 3, 10, 9, N); P(4, 4, 8, 7, piel);

                // ----- pelo -----
                switch (Cosmeticos.Forma(PEINADO, partes[PEINADO]))
                {
                    case 0: P(3, 2, 10, 3, N); P(4, 3, 8, 2, cab); P(4, 5, 1, 2, cab2); P(11, 5, 1, 2, cab2); break;
                    case 1: P(3, 2, 10, 3, N); P(4, 3, 8, 2, cab); P(2, 4, 2, 7, cab); P(12, 4, 2, 7, cab2); break;
                    case 2: P(5, 0, 6, 3, N); P(6, 0, 4, 2, cab2); P(3, 2, 10, 3, N); P(4, 3, 8, 2, cab); P(4, 5, 1, 4, cab); P(11, 5, 1, 4, cab); break;
                    case 3: P(6, 4, 3, 1, Pixel.Aclarar(piel, 0.35f)); break;
                    case 4: P(3, 2, 10, 3, N); P(4, 3, 8, 2, cab); P(12, 5, 3, 5, N); P(13, 6, 1, 3, cab2); break;
                    case 5: P(1, 0, 14, 5, N); P(2, 1, 12, 4, cab); P(1, 5, 3, 5, N); P(2, 5, 2, 4, cab2); P(12, 5, 3, 5, N); P(12, 5, 2, 4, cab2); break;
                    case 6: P(6, 0, 4, 4, N); P(7, 0, 2, 4, cab); P(3, 3, 10, 1, N); break;
                    case 7:
                        P(2, 1, 12, 4, N); P(3, 2, 10, 2, cab);
                        P(1, 0, 3, 2, cab); P(5, 0, 2, 2, cab2); P(9, 0, 2, 2, cab); P(12, 0, 3, 2, cab2); P(0, 2, 2, 3, cab); P(14, 2, 2, 3, cab);
                        P(4, 4, 1, 2, cab); P(11, 4, 1, 2, cab);
                        break;
                    case 8: P(3, 2, 10, 3, N); P(4, 3, 8, 2, cab); P(11, 5, 3, 7, N); P(12, 5, 2, 6, cab2); break;                          // mullet
                    case 9: P(3, 2, 10, 3, N); P(4, 3, 8, 2, cab); P(2, 5, 2, 8, N); P(2, 6, 1, 6, cab); P(12, 5, 2, 8, N); P(13, 6, 1, 6, cab2); break; // trenzas
                    case 10: P(4, 0, 9, 4, N); P(5, 0, 7, 3, cab); P(3, 3, 10, 2, N); P(4, 3, 8, 1, cab2); break;                            // copete
                    case 11: P(3, 2, 10, 4, N); P(4, 3, 8, 2, cab); P(4, 5, 5, 1, cab2); break;                                             // flequillo
                }

                // ----- cara -----
                P(6, 7, 1, 1, N); P(9, 7, 1, 1, N);
                if (fex == 3) P(5, 9, 6, 1, cab);
                else if (fex == 7) { P(4, 9, 8, 2, cab); P(6, 11, 4, 1, cab); P(7, 9, 2, 1, Paleta.Rosa2); }
                else if (fex == 8) { P(4, 9, 8, 1, cab); P(4, 10, 1, 1, cab); P(11, 10, 1, 1, cab); }       // bigote de charro
                else P(7, 9, 2, 1, Paleta.Rosa2);
                if (fex == 4) { P(3, 8, 1, 2, xa); P(12, 8, 1, 2, xa); }
                if (fex == 5) { P(5, 8, 1, 1, ColorTranslator.FromHtml("#f59aa0")); P(10, 8, 1, 1, ColorTranslator.FromHtml("#f59aa0")); }
                if (fex == 9) { P(8, 9, 3, 1, xb); P(11, 8, 2, 2, xa); }                                    // rosa en la boca

                DibujarMascara(P, piel);
                DibujarLentes(P);
                DibujarGorro(P);
            }
            return b;
        }

        private void DibujarFondo(Action<int, int, int, int, Color> P)
        {
            int i = partes[FONDO];
            Color a = Cosmeticos.A(FONDO, i), c = Cosmeticos.B(FONDO, i);
            switch (Cosmeticos.Forma(FONDO, i))
            {
                case 1: P(0, 0, 16, 16, a); break;
                case 2: P(0, 0, 16, 16, a); for (int y = 0; y < 16; y += 4) P(0, y, 16, 2, c); break;
                case 3: P(0, 0, 16, 16, a); for (int y = 0; y < 16; y += 2) for (int x = (y / 2) % 2; x < 16; x += 2) P(x, y, 1, 1, c); break;
                case 4: P(0, 0, 16, 16, a); P(1, 1, 1, 1, c); P(13, 2, 1, 1, c); P(2, 9, 1, 1, c); P(14, 11, 1, 1, c); P(12, 7, 1, 1, c); P(3, 14, 1, 1, c); break;
                case 5: P(0, 0, 16, 16, a); P(0, 1, 16, 1, c); for (int x = 0; x < 16; x += 4) { P(x, 2, 3, 3, c); P(x + 1, 5, 1, 1, c); } break;   // papel picado
                case 6: P(0, 0, 16, 16, a); P(1, 2, 1, 1, c); P(5, 0, 1, 1, c); P(12, 3, 1, 1, c); P(14, 8, 1, 1, c); P(1, 10, 1, 1, c); P(0, 15, 16, 1, c); break; // nieve
                case 7: P(0, 0, 5, 16, a); P(5, 0, 6, 16, Cosmeticos.Colores[Cosmeticos.BLANCO]); P(11, 0, 5, 16, c); break;                     // bandera
                case 8: P(0, 0, 16, 16, a); P(0, 8, 16, 1, c); P(0, 0, 1, 16, c); P(15, 0, 1, 16, c); break;                                     // cancha
                case 9: P(0, 0, 16, 16, a); P(1, 1, 2, 1, c); P(1, 2, 1, 1, c); P(3, 2, 0, 0, c); P(12, 2, 1, 1, c); P(14, 2, 1, 1, c); P(12, 3, 3, 1, c); P(13, 4, 1, 1, c); P(1, 13, 3, 1, c); P(2, 14, 1, 1, c); break; // corazones
                case 10: P(0, 0, 16, 16, a); P(0, 0, 1, 6, c); P(0, 0, 6, 1, c); P(1, 1, 1, 1, c); P(2, 2, 1, 1, c); P(3, 3, 1, 1, c); P(15, 0, 1, 5, c); P(11, 0, 5, 1, c); break; // telaraña
                case 11: P(0, 0, 16, 16, Cosmeticos.Colores[Cosmeticos.NEGRO]); P(0, 10, 16, 6, a); P(1, 7, 2, 3, c); P(6, 8, 3, 2, c); P(12, 6, 2, 4, c); P(0, 13, 16, 3, c); break; // fuego
                case 12:
                    Color[] arco = { Cosmeticos.Colores[0], Cosmeticos.Colores[6], Cosmeticos.Colores[3], Cosmeticos.Colores[2], Cosmeticos.Colores[1], Cosmeticos.Colores[5] };
                    for (int k = 0; k < 6; k++) P(0, k * 3, 16, 3, arco[k]);
                    break;
                case 13: P(0, 0, 16, 16, a); P(0, 12, 16, 4, c); P(2, 1, 1, 1, Paleta.Crema); P(13, 3, 1, 1, Paleta.Oro); P(1, 8, 1, 1, Paleta.Crema); P(14, 12, 1, 1, Paleta.Crema); break; // galaxia
            }
        }

        private void DibujarMascara(Action<int, int, int, int, Color> P, Color piel)
        {
            Color N = Paleta.Negro;
            int i = partes[MASCARA];
            Color a = Cosmeticos.A(MASCARA, i), c = Cosmeticos.B(MASCARA, i);
            switch (Cosmeticos.Forma(MASCARA, i))
            {
                case 1: P(4, 8, 8, 3, a); P(3, 8, 1, 1, Paleta.Crema); P(12, 8, 1, 1, Paleta.Crema); break;
                case 2: P(4, 4, 8, 7, N); P(4, 6, 8, 2, piel); P(6, 7, 1, 1, N); P(9, 7, 1, 1, N); P(3, 5, 10, 1, a); P(13, 5, 2, 1, a); break;
                case 3:
                    P(3, 2, 10, 2, N); P(4, 3, 8, 1, a); P(4, 4, 8, 7, a);
                    P(5, 6, 2, 2, Paleta.Crema); P(9, 6, 2, 2, Paleta.Crema); P(6, 7, 1, 1, N); P(9, 7, 1, 1, N);
                    P(7, 9, 2, 1, N); P(7, 4, 2, 2, c); P(4, 5, 1, 1, c); P(11, 5, 1, 1, c);
                    break;
                case 4:
                    P(4, 4, 8, 7, a); P(5, 6, 2, 2, N); P(9, 6, 2, 2, N); P(7, 8, 2, 1, N);
                    P(5, 9, 6, 1, N); P(6, 9, 1, 1, a); P(8, 9, 1, 1, a); P(10, 9, 1, 1, a);
                    P(4, 4, 2, 1, c); P(10, 4, 2, 1, Paleta.Agua);
                    break;
                case 5: P(4, 4, 8, 7, ColorTranslator.FromHtml("#7fb069")); P(6, 7, 1, 1, Paleta.Rojo); P(9, 7, 1, 1, Paleta.Rojo); P(5, 5, 3, 1, N); P(6, 9, 4, 1, N); P(7, 10, 1, 1, Paleta.Crema); break;
                case 6: P(4, 4, 8, 7, a); P(5, 6, 2, 2, c); P(9, 6, 2, 2, c); P(5, 9, 6, 1, N); P(6, 9, 1, 1, a); P(8, 9, 1, 1, a); P(7, 2, 2, 2, N); P(7, 1, 2, 1, Paleta.Rojo); break;
                case 7:   // catrina: cara blanca, ojos con flores y boca cosida
                    P(4, 4, 8, 7, a); P(5, 6, 2, 2, c); P(9, 6, 2, 2, c); P(6, 7, 1, 1, N); P(9, 7, 1, 1, N);
                    P(7, 8, 2, 1, N); P(5, 10, 6, 1, N); P(6, 10, 1, 1, a); P(8, 10, 1, 1, a); P(7, 4, 2, 1, c);
                    break;
                case 8: P(4, 4, 8, 7, a); P(5, 6, 2, 2, c); P(9, 6, 2, 2, c); P(5, 9, 6, 1, c); P(6, 10, 1, 1, c); P(9, 10, 1, 1, c); break;   // calabaza
                case 9: P(4, 4, 8, 7, a); P(4, 5, 8, 1, c); P(4, 8, 8, 1, c); P(4, 10, 8, 1, c); P(6, 6, 1, 1, N); P(9, 6, 1, 1, N); break; // momia
                case 10: P(7, 8, 2, 2, a); break;                                                                                       // nariz de reno
                case 11: P(3, 3, 10, 9, a); P(5, 6, 2, 2, c); P(9, 6, 2, 2, c); P(7, 9, 2, 2, c); break;                                  // fantasma
                case 12: P(4, 6, 8, 3, a); P(5, 7, 1, 1, N); P(10, 7, 1, 1, N); P(3, 6, 1, 1, c); P(12, 6, 1, 1, c); break;               // antifaz
            }
        }

        private void DibujarLentes(Action<int, int, int, int, Color> P)
        {
            Color N = Paleta.Negro;
            int i = partes[LENTES];
            Color a = Cosmeticos.A(LENTES, i);
            switch (Cosmeticos.Forma(LENTES, i))
            {
                case 1: P(4, 6, 3, 3, N); P(9, 6, 3, 3, N); P(7, 7, 2, 1, N); P(5, 7, 1, 1, Paleta.Cielo); P(10, 7, 1, 1, Paleta.Cielo); break;
                case 2: P(4, 6, 8, 2, N); P(5, 6, 2, 1, Paleta.Gris); P(9, 6, 2, 1, Paleta.Gris); break;
                case 3: P(4, 6, 8, 2, N); P(5, 6, 2, 1, Paleta.Rojo); P(9, 6, 2, 1, Paleta.Cielo); break;
                case 4: P(4, 6, 3, 2, Paleta.Rosa); P(9, 6, 3, 2, Paleta.Rosa); P(5, 8, 1, 1, Paleta.Rosa); P(10, 8, 1, 1, Paleta.Rosa); P(7, 6, 2, 1, N); break;
                case 5: P(9, 6, 3, 3, Paleta.Oro); P(10, 7, 1, 1, Paleta.Cielo); P(11, 9, 1, 2, Paleta.Oro); break;
                case 6: P(10, 5, 4, 4, N); P(10, 6, 3, 2, Paleta.Verde); P(13, 3, 1, 3, N); break;
                case 7: P(4, 6, 8, 2, N); P(5, 6, 2, 1, a); P(9, 6, 2, 1, a); break;                                          // oscuros de color
                case 8: P(4, 6, 3, 3, a); P(9, 6, 3, 3, a); P(5, 5, 1, 1, a); P(10, 5, 1, 1, a); P(7, 7, 2, 1, N); break;       // estrella
                case 9: P(3, 5, 10, 3, N); P(4, 6, 8, 1, a); P(2, 6, 1, 1, N); P(13, 6, 1, 1, N); break;                      // goggles
            }
        }

        private void DibujarGorro(Action<int, int, int, int, Color> P)
        {
            Color N = Paleta.Negro;
            int i = partes[GORRO];
            Color a = Cosmeticos.A(GORRO, i), c = Cosmeticos.B(GORRO, i);
            switch (Cosmeticos.Forma(GORRO, i))
            {
                case 1: P(3, 1, 10, 4, N); P(4, 2, 8, 2, a); P(10, 4, 5, 1, N); P(10, 3, 4, 1, a); break;
                case 2: P(5, 0, 6, 3, N); P(6, 1, 4, 2, a); P(1, 3, 14, 2, N); P(2, 3, 12, 1, a); break;
                case 3: P(3, 1, 10, 3, N); P(4, 1, 8, 2, a); P(10, 0, 4, 2, a); P(3, 3, 10, 1, c); P(13, 0, 2, 2, c); break;
                case 4: P(2, 1, 12, 6, N); P(3, 2, 10, 4, a); P(4, 5, 8, 3, N); P(5, 6, 6, 1, c); break;
                case 5: P(3, 0, 2, 3, N); P(3, 0, 1, 2, a); P(11, 0, 2, 3, N); P(12, 0, 1, 2, a); break;
                case 6: P(4, 0, 8, 1, a); P(3, 1, 1, 1, a); P(12, 1, 1, 1, a); break;
                case 7: P(3, 1, 10, 3, N); P(4, 1, 8, 2, a); P(4, 0, 1, 1, a); P(7, 0, 2, 1, a); P(11, 0, 1, 1, a); P(7, 2, 2, 1, c); break;
                case 8: P(5, 0, 6, 3, N); P(6, 0, 4, 3, a); P(0, 3, 16, 2, N); P(1, 3, 14, 1, a); P(6, 2, 4, 1, c); P(1, 4, 14, 1, c); break;   // charro
                case 9: P(7, 0, 3, 1, a); P(6, 1, 4, 1, a); P(5, 2, 6, 1, a); P(1, 3, 14, 2, N); P(2, 3, 12, 1, a); P(5, 2, 6, 1, c); break;   // bruja
                case 10: P(2, 0, 1, 4, a); P(1, 1, 1, 1, a); P(3, 1, 1, 1, a); P(13, 0, 1, 4, a); P(12, 1, 1, 1, a); P(14, 1, 1, 1, a); P(7, 8, 2, 2, c); break; // reno
                case 11: for (int k = 3; k < 13; k += 3) { P(k, 1, 3, 3, a); P(k + 1, 2, 1, 1, c); } break;                                    // cempasúchil
                case 12: P(3, 0, 10, 12, a); P(7, 0, 2, 1, Paleta.Verde); P(5, 5, 2, 2, c); P(9, 5, 2, 2, c); P(5, 9, 6, 1, c); break;          // calabaza
                case 13: P(3, 2, 10, 1, N); P(6, 0, 1, 2, a); P(9, 0, 1, 2, a); P(5, 0, 1, 1, c); P(10, 0, 1, 1, c); break;                   // diadema de corazón
                case 14: P(3, 2, 10, 2, a); P(4, 2, 1, 1, c); P(8, 3, 1, 1, c); P(12, 3, 3, 1, a); break;                                      // bandana
                case 15: P(3, 1, 10, 4, N); P(4, 1, 8, 3, a); P(3, 3, 10, 1, c); P(7, 0, 2, 1, c); break;                                     // gorro con pompón
                case 16: P(5, 0, 6, 3, a); P(1, 3, 14, 1, a); P(5, 2, 6, 1, c); break;                                                        // paja
                case 17: P(3, 0, 10, 3, a); P(3, 0, 1, 1, Color.Transparent); P(5, 1, 1, 1, c); P(8, 1, 1, 1, Paleta.Rojo); P(11, 1, 1, 1, c); break; // corona de Reyes
                case 18: P(4, 0, 2, 4, N); P(4, 0, 1, 3, a); P(5, 1, 1, 2, c); P(10, 0, 2, 4, N); P(10, 0, 1, 3, a); P(11, 1, 1, 2, c); break;  // conejo
                case 19: P(3, 1, 10, 1, a); P(2, 2, 1, 5, a); P(13, 2, 1, 5, a); P(1, 6, 3, 3, c); P(12, 6, 3, 3, c); break;                    // audífonos
            }
        }
    }
}
