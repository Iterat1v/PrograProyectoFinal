using System.Globalization;

namespace PrograProyectoFinal
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

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
            Recursos.Musica(Recursos.MusicaActiva);

            Application.Run(new FormTitulo());
        }
    }
}
