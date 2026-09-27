using System;
using System.Windows.Forms;

namespace PrograProyectoFinal
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Llama directamente a FormSaludo que ahora vive en PrograProyectoFinal
            Application.Run(new FormSaludo());
        }
    }
}