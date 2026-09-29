namespace PrograProyectoFinal
{
    // TIPO 3 · ASEGURADORA: te protegen. No se usan con el botón,
    // se activan solas cuando alguien te lanza una Estafa (ver Concesionario.Defender).
    internal class Aseguradora : PowerUp
    {
        public const int SEGURO_COMERCIAL = 0;
        public const int CONTRA_ESTAFAS = 1;
        public const int COLCHONCITO = 2;

        public Aseguradora(string nombre, string descripcion, int efecto) : base(nombre, descripcion, efecto)
        {
        }

        public override string Tipo => "ASEGURADORA";
        public override Color Color => Paleta.Agua;
        public override int Icono => 2;

        public override string Aplicar(Concesionario usuario, Concesionario rival, out bool usada)
        {
            usada = false;
            return "Esta carta se activa sola cuando alguien te lanza una Estafa.";
        }

        // ¿Esta carta detiene a esa estafa?
        public bool Protege(Estafa estafa, double perdida)
        {
            switch (efecto)
            {
                case CONTRA_ESTAFAS:
                    return estafa.Efecto == Estafa.MANIPULACION;
                case COLCHONCITO:
                    return perdida > 0 && perdida <= 50000;
                default:
                    return true;      // el Seguro comercial bloquea cualquier Estafa
            }
        }
    }
}
