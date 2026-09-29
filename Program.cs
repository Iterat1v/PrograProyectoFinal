using System.Globalization;

namespace PrograProyectoFinal
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Para que el dinero siempre salga como $98,000
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-MX");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es-MX");
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-MX");

            // Imágenes y fuentes se cargan una sola vez
            Recursos.Cargar();

            Application.Run(new FormTitulo());
        }
    }
}
