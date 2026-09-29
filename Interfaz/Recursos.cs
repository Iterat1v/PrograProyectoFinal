using System.Drawing.Text;
using System.Media;
using System.Runtime.InteropServices;

namespace PrograProyectoFinal
{
    // Carga las imágenes y las fuentes UNA sola vez al arrancar (en Program.cs).
    // Así cada pantalla abre al instante.
    internal static class Recursos
    {
        public static readonly string[] Tipos = { "Sedán", "Compacto", "Pickup" };
        public static readonly string[] NombresVendedor = { "El Licenciado", "La Doña", "El Junior", "Don Chuy" };

        public static Image[] Vendedores = new Image[4];
        public static Image[] Clientes = new Image[8];
        public static Image[,] Autos = new Image[3, 8];   // [tipo, color]
        public static readonly string[] VentajasVendedor =
        {
            "Negociador: vende 5 % más caro.",
            "Clientela fiel: sus clientes traen 10 % más.",
            "Herencia: empieza con $20,000 extra.",
            "Mecánico: sus autos valen 10 % más al final."
        };

        public static Image Moneda;
        public static Image[] Iconos = new Image[3];     // Regateo, Estafa, Aseguradora

        private static string[] nombresSonidos = { "venta", "compra", "carta", "error", "turno", "estafa", "victoria" };
        private static SoundPlayer[] sonidos = new SoundPlayer[7];

        private static PrivateFontCollection fuentes = new PrivateFontCollection();
        private static FontFamily jersey = FontFamily.GenericMonospace;
        private static FontFamily silkscreen = FontFamily.GenericMonospace;
        private static Dictionary<float, Font> letras = new Dictionary<float, Font>();
        private static Dictionary<float, Font> titulos = new Dictionary<float, Font>();

        // Para que también el TextBox (que dibuja con GDI) encuentre la fuente
        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        private static extern int AddFontResourceEx(string archivo, uint flags, IntPtr reservado);
        private const uint FR_PRIVATE = 0x10;

        public static bool Cargado { get; private set; }

        public static void Cargar()
        {
            string assets = Path.Combine(Application.StartupPath, "Assets");
            string sprites = Path.Combine(assets, "Sprites");
            string[] carpetaTipo = { "sedan", "vocho", "pickup" };

            for (int i = 0; i < Vendedores.Length; i++)
                Vendedores[i] = Image.FromFile(Path.Combine(sprites, $"vendedor{i}.png"));
            for (int i = 0; i < Clientes.Length; i++)
                Clientes[i] = Image.FromFile(Path.Combine(sprites, $"cliente{i}.png"));
            for (int t = 0; t < 3; t++)
                for (int c = 0; c < 8; c++)
                    Autos[t, c] = Image.FromFile(Path.Combine(sprites, $"auto_{carpetaTipo[t]}{c}.png"));
            Moneda = Image.FromFile(Path.Combine(sprites, "moneda.png"));
            string[] iconos = { "regateo", "estafa", "aseguradora" };
            for (int i = 0; i < Iconos.Length; i++)
                Iconos[i] = Image.FromFile(Path.Combine(sprites, $"icono_{iconos[i]}.png"));

            for (int i = 0; i < sonidos.Length; i++)
            {
                sonidos[i] = new SoundPlayer(Path.Combine(assets, "Sonidos", $"{nombresSonidos[i]}.wav"));
                sonidos[i].Load();
            }

            string jerseyTtf = Path.Combine(assets, "Fuentes", "Jersey10-Regular.ttf");
            string silkTtf = Path.Combine(assets, "Fuentes", "Silkscreen-Bold.ttf");
            fuentes.AddFontFile(jerseyTtf);
            fuentes.AddFontFile(silkTtf);
            AddFontResourceEx(jerseyTtf, FR_PRIVATE, IntPtr.Zero);
            foreach (FontFamily familia in fuentes.Families)
            {
                if (familia.Name.StartsWith("Jersey")) jersey = familia;
                if (familia.Name.StartsWith("Silkscreen")) silkscreen = familia;
            }
            Cargado = true;
        }

        // Toca un efecto: Recursos.Sonar("venta"). Play() no congela la pantalla.
        public static void Sonar(string nombre)
        {
            for (int i = 0; i < nombresSonidos.Length; i++)
                if (nombresSonidos[i] == nombre && sonidos[i] != null)
                    sonidos[i].Play();
        }

        // Letra normal. "tam" es el mismo número que se usó en el mockup
        public static Font Letra(float tam)
        {
            if (!letras.ContainsKey(tam))
                letras[tam] = new Font(jersey, (float)Math.Round(tam * 1.35) * 2, GraphicsUnit.Pixel);
            return letras[tam];
        }

        // Letra gruesa de los títulos grandes (CONCESIONARIO, ¡GANÓ!)
        public static Font Titulo(float pixeles)
        {
            if (!titulos.ContainsKey(pixeles))
                titulos[pixeles] = new Font(silkscreen, pixeles, FontStyle.Bold, GraphicsUnit.Pixel);
            return titulos[pixeles];
        }
    }
}
