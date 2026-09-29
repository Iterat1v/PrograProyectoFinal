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
        private double factorPago = 1;      // El Bro paga más
        private int colorBuscado = -1;      // -1 = le da igual el color

        public static readonly string[] NombresColor = { "rojo", "aqua", "amarillo", "azul", "rosa", "blanco", "verde", "gris" };

        public Cliente(string nombre, double presupuesto, int tipoBuscado, string frase, int retrato, double factorPago)
            : this(nombre, presupuesto, tipoBuscado, frase, retrato)
        {
            this.factorPago = factorPago;
        }

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
        public double FactorPago => factorPago;
        public bool AceptaCualquiera => tipoBuscado == -1;   // -1 = le da igual el tipo

        public int ColorBuscado
        {
            get => colorBuscado;
            set => colorBuscado = value;
        }

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
        public string Saludo
        {
            get
            {
                if (AceptaCualquiera) return frase;
                string color = colorBuscado == -1 ? "" : $" Y lo quiero {NombresColor[colorBuscado]}.";
                return $"{frase} Traigo ${presupuesto:N0}.{color}";
            }
        }
    }
}
