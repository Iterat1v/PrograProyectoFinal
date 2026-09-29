namespace PrograProyectoFinal
{
    // Carta ABILICARD. Es abstracta: cada tipo (Regateo, Estafa, Aseguradora)
    // sobreescribe Aplicar() con lo que hace.
    internal abstract class PowerUp
    {
        private string nombre;
        private string descripcion;
        protected int efecto;

        internal PowerUp(string nombre, string descripcion, int efecto)
        {
            this.nombre = nombre;
            this.descripcion = descripcion;
            this.efecto = efecto;
        }

        public string Nombre => nombre;
        public string Descripcion => descripcion;
        public int Efecto => efecto;

        public abstract string Tipo { get; }        // REGATEO, ESTAFA o ASEGURADORA
        public abstract Color Color { get; }        // color de la franja de arriba
        public abstract int Icono { get; }          // índice en Recursos.Iconos

        // Las de Estafa necesitan escoger a un rival
        public virtual bool NecesitaRival => false;

        // Regresa lo que pasó. "usada" es false si no se pudo usar (y la carta se queda)
        public abstract string Aplicar(Concesionario usuario, Concesionario rival, out bool usada);
    }
}
