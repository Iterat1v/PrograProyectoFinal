namespace PrograProyectoFinal
{
    // El cliente que llega en cada turno
    internal class Cliente
    {
        private string nombre;
        private double presupuesto;
        private int tipoBuscado;
        private string frase;
        private int retrato;

        public Cliente(string nombre, double presupuesto, int tipoBuscado, string frase, int retrato)
        {
            this.nombre = nombre;
            Presupuesto = presupuesto;
            this.tipoBuscado = tipoBuscado;
            this.frase = frase;
            this.retrato = retrato;
        }

        public string Nombre => nombre;
        public int TipoBuscado => tipoBuscado;
        public int Retrato => retrato;

        public double Presupuesto
        {
            get => presupuesto;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("El presupuesto no puede ser negativo");
                else
                    presupuesto = value;
            }
        }

        // Lo que dice el cliente al llegar
        public string Saludo => $"{frase} Traigo ${presupuesto:N0}.";
    }
}
