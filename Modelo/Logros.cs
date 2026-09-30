namespace PrograProyectoFinal
{
    // 18 logros: se revisan al terminar cada partida y se guardan en el perfil
    internal static class Logros
    {
        public const int CANTIDAD = 18;
        public const int MONEDAS = 50;

        public const int PRIMERA_VENTA = 0, VENDEDOR_DEL_MES = 1, MILLONARIO = 2, CAMPEON = 3, SIN_DEUDAS = 4,
                         CHATARRERO = 5, TUNERO = 6, ESTAFADOR = 7, SANTO = 8, FAMA = 9, MATABOTS = 10,
                         RACHA_3 = 11, RACHA_7 = 12, CAMPEON_TEMPORADA = 13, REY_CDMX = 14, BRO = 15,
                         REGATEADOR = 16, COLECCIONISTA = 17;

        public static readonly string[] Nombres =
        {
            "PRIMERA VENTA", "VENDEDOR DEL MES", "MILLONARIO", "CAMPEÓN", "SIN DEUDAS",
            "CHATARRERO", "TUNERO", "ESTAFADOR", "SANTO", "FAMA", "MATABOTS",
            "RACHA DE 3", "RACHA DE 7", "CAMPEÓN DE LIGA", "REY DE LA CDMX", "AMIGO DEL BRO",
            "REGATEADOR", "COLECCIONISTA"
        };
        public static readonly string[] Descripciones =
        {
            "Vende tu primer auto.",
            "Vende 6 autos en una partida.",
            "Termina con $1,000,000 o más.",
            "Gana una partida.",
            "Gana sin pedir préstamos.",
            "Vende 2 autos chocados en una partida.",
            "Vende un auto tuneado.",
            "Usa 3 estafas en una partida.",
            "Gana sin usar estafas.",
            "Termina con 90 de reputación.",
            "Gánale a un bot SSJ DIOS 1000.",
            "Juega el reto 3 días seguidos.",
            "Juega el reto 7 días seguidos.",
            "Gana una temporada de liga.",
            "Termina el modo historia.",
            "Véndele un auto a El Bro.",
            "Vende 3 autos arriba del precio sugerido.",
            "Gana una subasta."
        };

        // Regresa true si es nuevo (y paga las monedas)
        public static bool Dar(Perfil p, int logro)
        {
            if (p == null || p.TieneLogro(logro)) return false;
            p.MarcarLogro(logro);
            p.Ganar(0, MONEDAS);
            return true;
        }

        // Revisa los logros de una persona al final de la partida.
        // Regresa los nombres de los nuevos separados por coma ("" si no hubo).
        public static string Revisar(Concesionario j, Partida partida, bool gano)
        {
            Perfil p = j.Perfil;
            if (p == null || j.Rendido) return "";

            bool hayDios = false;
            for (int i = 0; i < partida.NumJugadores; i++)
                if (partida.ObtenerJugador(i).NivelBot == Bot.DIOS) hayDios = true;

            bool[] cumple = new bool[CANTIDAD];
            cumple[PRIMERA_VENTA] = j.Ventas >= 1;
            cumple[VENDEDOR_DEL_MES] = j.Ventas >= 6;
            cumple[MILLONARIO] = j.Patrimonio() >= 1000000;
            cumple[CAMPEON] = gano;
            cumple[SIN_DEUDAS] = gano && !j.PidioPrestamo;
            cumple[CHATARRERO] = j.ChocadosVendidos >= 2;
            cumple[TUNERO] = j.TuneadosVendidos >= 1;
            cumple[ESTAFADOR] = j.EstafasUsadas >= 3;
            cumple[SANTO] = gano && j.EstafasUsadas == 0;
            cumple[FAMA] = j.Reputacion >= 90;
            cumple[MATABOTS] = gano && hayDios;
            cumple[BRO] = j.VentasBro >= 1;
            cumple[REGATEADOR] = j.VentasCaras >= 3;
            cumple[COLECCIONISTA] = j.SubastasGanadas >= 1;

            string nuevos = "";
            for (int i = 0; i < CANTIDAD; i++)
                if (cumple[i] && Dar(p, i))
                    nuevos += (nuevos == "" ? "" : ", ") + Nombres[i];
            return nuevos;
        }
    }
}
