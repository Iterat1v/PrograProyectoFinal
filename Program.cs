using System.Globalization;

namespace PrograProyectoFinal
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();

            // Pantalla completa: se agranda todo para llenar el monitor (con --ventana se juega en 1280 x 720)
            if (!args.Contains("--ventana"))
            {
                Rectangle monitor = Screen.PrimaryScreen.Bounds;
                float escala = Math.Min(monitor.Width / (float)Pixel.ANCHO, monitor.Height / (float)Pixel.ALTO);
                if (escala >= 1.1f)
                    Pixel.Esc = (float)Math.Floor(escala * 100) / 100f;
            }

            // Si una ventana ya cerrada recibe un evento tardío (por ejemplo un doble clic), no se muestra el cuadro de error:
            // se anota en errores.txt (junto a los perfiles) y el juego sigue. Cualquier otro error sí se avisa.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) =>
            {
                try { File.AppendAllText(Path.Combine(Path.GetDirectoryName(Perfiles.Ruta) ?? ".", "errores.txt"), DateTime.Now + "\n" + e.Exception + "\n\n"); } catch (IOException) { }
                if (e.Exception is ObjectDisposedException) return;
                MessageBox.Show(e.Exception.Message, "Car Cash", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            // Para que el dinero siempre salga como $98,000
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-MX");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es-MX");
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-MX");

            // Imágenes y fuentes se cargan una sola vez
            Recursos.Cargar();
            Perfiles.Cargar();
            Temporada.Cargar();
            int ic = Array.IndexOf(args, "--captura");
            if (ic >= 0 && ic + 1 < args.Length)
            {
                Capturar(args[ic + 1]);
                return;
            }

            Recursos.Musica(Recursos.MusicaActiva);

            Application.Run(new FormTitulo());
        }

        // Pruebas: guarda una foto de varias pantallas sin mostrarlas en el monitor (no se usa al jugar)
        static void Capturar(string carpeta)
        {
            Pixel.Captura = true;
            Directory.CreateDirectory(carpeta);
            Form[] formas = { new FormTitulo(), new FormModo(), new FormLobby(), new FormRegistro(), new FormReglas(), new FormCartas(), new FormRecords(), new FormHistoria() };
            int n = 0;
            foreach (Form f in formas)
            {
                f.StartPosition = FormStartPosition.Manual;
                f.Location = new Point(-30000, -30000);
                f.Show();
                for (int i = 0; i < 10; i++) { Application.DoEvents(); Thread.Sleep(40); }
                using (Bitmap foto = new Bitmap(f.ClientSize.Width, f.ClientSize.Height))
                {
                    f.DrawToBitmap(foto, new Rectangle(0, 0, foto.Width, foto.Height));
                    foto.Save(Path.Combine(carpeta, (++n) + "_" + f.GetType().Name + ".png"));
                }
                f.Hide();
            }
            Environment.Exit(0);
        }
    }
}
