namespace PrograProyectoFinal
{
    // TIPO 2 · ESTAFA: afectan a un rival
    internal class Estafa : PowerUp
    {
        public const int YA_TE_CAYO_LA_LEY = 0;
        public const int POR_QUE_TAN_CARO = 1;
        public const int MANIPULACION = 2;
        public const int ERROR_DE_CUENTAS = 3;
        public const int MATANGA = 4;
        public const int COMBUSTION = 5;

        private static Random azar = new Random();

        public Estafa(string nombre, string descripcion, int efecto) : base(nombre, descripcion, efecto)
        {
        }

        public override string Tipo => "ESTAFA";
        public override Color Color => Paleta.Rosa;
        public override int Icono => 1;
        public override bool NecesitaRival => true;

        public override string Aplicar(Concesionario usuario, Concesionario rival, out bool usada)
        {
            usada = false;

            // Primero se revisa que la estafa tenga sentido
            int indiceAuto = -1;
            double perdida = 0;
            if (efecto == MATANGA || efecto == COMBUSTION)
            {
                if (rival.NumAutos == 0)
                    return $"{rival.Jugador} no tiene autos en su lote.";
                if (efecto == MATANGA && usuario.NumAutos == Concesionario.MAX_AUTOS)
                    return "Tu lote está lleno, no tienes dónde meter el coche.";
                indiceAuto = azar.Next(rival.NumAutos);
                perdida = rival.ObtenerAuto(indiceAuto).Precio;
            }
            if (efecto == YA_TE_CAYO_LA_LEY)
                perdida = Math.Min(30000, rival.Dinero);

            usada = true;

            // El rival se puede defender con una Aseguradora
            string defensa = rival.Defender(this, perdida);
            if (defensa != null)
                return defensa;

            switch (efecto)
            {
                case YA_TE_CAYO_LA_LEY:
                    rival.Dinero -= perdida;
                    rival.Noticia = $"Te cayó la ley: pagaste ${perdida:N0} al SAT.";
                    return $"¡Ya le cayó la ley a {rival.Jugador}! Pierde ${perdida:N0}.";
                case POR_QUE_TAN_CARO:
                    rival.PierdeTurno = true;
                    return $"{rival.Jugador} asustó a su cliente: pierde su siguiente turno.";
                case MANIPULACION:
                    rival.Sobreprecio = true;
                    rival.Noticia = "Te manipularon el mercado: +20 % en tus compras este turno.";
                    return $"A {rival.Jugador} le suben 20 % los precios en su siguiente turno.";
                case ERROR_DE_CUENTAS:
                    rival.ErrorDeCuentas = true;
                    rival.Noticia = "Error de cuentas: tu próxima venta sale 20 % más barata.";
                    return $"{rival.Jugador} va a regatear mal: -20 % en su próxima venta.";
                case MATANGA:
                    Auto robado = rival.QuitarAuto(indiceAuto);
                    usuario.RecibirAuto(robado);
                    rival.Noticia = $"¡{usuario.Jugador} te robó el {robado.Nombre}!";
                    return $"¡¡Matanga!! Le volaste el {robado.Nombre} a {rival.Jugador}.";
                default:
                    Auto quemado = rival.QuitarAuto(indiceAuto);
                    rival.Noticia = $"Tu {quemado.Nombre} se prendió solito...";
                    return $"El {quemado.Nombre} de {rival.Jugador} se prendió en llamas.";
            }
        }
    }
}
