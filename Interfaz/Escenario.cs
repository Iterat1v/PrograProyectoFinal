namespace PrograProyectoFinal
{
    // Fondos de cada zona de la ciudad (de Ecatepec a Polanco), dibujados con rectángulos.
    // Todo se da en fracciones del área (0 a 1) para que sirva en grande o en chico.
    internal static class Escenario
    {
        private static Color H(string hex) => ColorTranslator.FromHtml(hex);

        public static readonly string[] Descripciones =
        {
            "Lote de tierra en el cerro. Clientes con poco dinero.",
            "Calles con puestos y mucho movimiento.",
            "Colonial, cafecitos y clientes exigentes.",
            "Torres de vidrio. Aquí sí traen dinero.",
            "Lo más caro de la ciudad. Clientes de lujo."
        };

        public static void Dibujar(Graphics g, int zona, Rectangle a)
        {
            // Rectángulo en fracciones del área
            void F(double fx, double fy, double fw, double fh, Color c)
            {
                int x = a.X + (int)(fx * a.Width), y = a.Y + (int)(fy * a.Height);
                int w = Math.Max(1, (int)(fw * a.Width)), h = Math.Max(1, (int)(fh * a.Height));
                Pixel.R(g, x, y, w, h, c);
            }

            switch (zona)
            {
                case 0: // Ecatepec: atardecer, cerro lleno de casitas, tinacos y tierra
                    F(0, 0, 1, 0.25, H("#f28c28")); F(0, 0.25, 1, 0.2, H("#ffb86b")); F(0, 0.45, 1, 0.2, H("#ffd9a0"));
                    F(0, 0.35, 1, 0.4, H("#8a6a4a"));
                    Color[] casas = { H("#d9534f"), H("#5bc0de"), H("#f0ad4e"), H("#a78bfa"), H("#5fd068"), H("#fff4e8") };
                    for (int i = 0; i < 16; i++)
                    {
                        double x = i * 0.063, y = 0.36 + (i % 3) * 0.08 + Math.Abs(i - 8) * 0.012;
                        F(x, y, 0.05, 0.07, casas[i % casas.Length]);
                        if (i % 2 == 0) F(x + 0.01, y - 0.03, 0.02, 0.03, Paleta.Negro);   // tinaco
                    }
                    F(0, 0.30, 1, 0.006, Paleta.Negro); F(0, 0.33, 1, 0.006, Paleta.Negro);     // cables
                    F(0, 0.75, 1, 0.25, H("#7a5230")); F(0, 0.75, 1, 0.02, H("#5c3d22"));
                    break;

                case 1: // Neza: cielo claro, casas de dos pisos y puestos con lonas
                    F(0, 0, 1, 0.5, H("#9fd8ff")); F(0, 0.5, 1, 0.25, H("#cfeeff"));
                    Color[] fachadas = { H("#ff8fab"), H("#ffd447"), H("#7fb8e6"), H("#9be89b"), H("#e8d9c4") };
                    for (int i = 0; i < 7; i++)
                    {
                        F(i * 0.145, 0.28 + (i % 2) * 0.06, 0.14, 0.47, fachadas[i % fachadas.Length]);
                        F(i * 0.145 + 0.03, 0.4 + (i % 2) * 0.06, 0.04, 0.07, H("#3d1670"));
                        F(i * 0.145 + 0.08, 0.4 + (i % 2) * 0.06, 0.04, 0.07, H("#3d1670"));
                    }
                    Color[] lonas = { Paleta.Rosa, Paleta.Azul, Paleta.Rosa, Paleta.Agua };
                    for (int i = 0; i < 4; i++) F(0.05 + i * 0.25, 0.62, 0.18, 0.05, lonas[i]);
                    F(0, 0.75, 1, 0.25, H("#6b6480")); F(0, 0.86, 1, 0.015, H("#ffd447"));
                    break;

                case 2: // Coyoacán: fachadas coloniales, arcos y árboles
                    F(0, 0, 1, 0.75, H("#bfe3ff"));
                    Color[] colonial = { H("#e07a3f"), H("#f2c14e"), H("#c0392b"), H("#f2c14e") };
                    for (int i = 0; i < 4; i++)
                    {
                        F(i * 0.25, 0.25, 0.25, 0.5, colonial[i]);
                        for (int k = 0; k < 3; k++) F(i * 0.25 + 0.03 + k * 0.075, 0.5, 0.05, 0.25, H("#5a2d1a"));   // arcos
                        F(i * 0.25, 0.25, 0.25, 0.03, H("#fff4e8"));
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        F(0.1 + i * 0.35, 0.3, 0.12, 0.2, H("#3a9a44")); F(0.15 + i * 0.35, 0.5, 0.02, 0.25, H("#5a2d1a"));
                    }
                    F(0, 0.75, 1, 0.25, H("#b5a48c"));
                    for (int i = 0; i < 20; i++) F(i * 0.05, 0.8 + (i % 2) * 0.08, 0.04, 0.03, H("#9c8b73"));   // empedrado
                    break;

                case 3: // Santa Fe: torres de vidrio y autopista
                    F(0, 0, 1, 0.75, H("#7fb8e6"));
                    double[] alturas = { 0.55, 0.7, 0.45, 0.65, 0.5, 0.72 };
                    for (int i = 0; i < 6; i++)
                    {
                        double alto = alturas[i], x = i * 0.17;
                        F(x, 0.75 - alto, 0.15, alto, i % 2 == 0 ? H("#3f6fd8") : H("#9fd8ff"));
                        for (int fila = 0; fila < (int)(alto * 20); fila++)
                            F(x + 0.02, 0.75 - alto + 0.02 + fila * 0.045, 0.11, 0.015, H("#e8f6ff"));
                    }
                    F(0, 0.75, 1, 0.25, H("#3a3450"));
                    for (int i = 0; i < 8; i++) F(i * 0.13, 0.86, 0.07, 0.015, Paleta.Crema);
                    break;

                default: // Polanco: noche de lujo, boutiques doradas y palmeras
                    F(0, 0, 1, 0.75, H("#2a0f4a"));
                    for (int i = 0; i < 12; i++) F((i * 37 % 100) / 100.0, (i * 13 % 30) / 100.0, 0.01, 0.015, Paleta.Oro);
                    for (int i = 0; i < 3; i++)
                    {
                        F(0.05 + i * 0.33, 0.3, 0.26, 0.45, H("#fff4e8"));
                        F(0.05 + i * 0.33, 0.3, 0.26, 0.06, Paleta.Oro);
                        F(0.09 + i * 0.33, 0.45, 0.18, 0.2, H("#ffe9a8"));
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        double x = 0.3 + i * 0.33;
                        F(x, 0.25, 0.015, 0.5, H("#8a5a2b"));
                        F(x - 0.04, 0.22, 0.1, 0.03, H("#3a9a44")); F(x - 0.02, 0.19, 0.06, 0.03, H("#5fd068"));
                    }
                    F(0, 0.75, 1, 0.25, H("#e8e8e8")); F(0, 0.75, 1, 0.02, Paleta.Oro);
                    break;
            }
        }
    }
}
