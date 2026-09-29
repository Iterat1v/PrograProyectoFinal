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

        public const int ACEPTA = 0;
        public const int CONTRAOFERTA = 1;

        // Lo máximo que pagaría por ese auto (el bot nivel Dios lo "adivina")
        public double Limite(Auto auto, bool licenciado, bool ventaSegura)
        {
            double limite;
            if (AceptaCualquiera || ventaSegura)
                limite = auto.Precio * 1.6;                 // El Bro o "¡¡Toma mi dinero!!"
            else
            {
                limite = presupuesto;
                if (licenciado) limite *= 1.05;             // el Licenciado convence mejor
                if (auto.Tipo != tipoBuscado) limite *= 0.60;   // no es lo que busca: solo muy barato
            }
            return Math.Floor(limite / 1000) * 1000;
        }

        // El jugador pide un precio y el cliente decide.
        // Si no acepta, "contraoferta" es lo máximo que daría.
        public int Evaluar(Auto auto, double precio, bool licenciado, bool ventaSegura, Random azar,
                           out string respuesta, out double contraoferta)
        {
            bool otroTipo = !AceptaCualquiera && auto.Tipo != tipoBuscado;
            double limite = Limite(auto, licenciado, ventaSegura);
            contraoferta = limite;

            if (precio <= limite)
            {
                respuesta = otroTipo ? "No es lo que buscaba... pero a ese precio me lo llevo."
                                     : "¡Trato hecho! Me lo llevo.";
                return ACEPTA;
            }
            if (precio <= limite * 1.15 && azar.Next(100) < 40)
            {
                respuesta = "Mmm... ándale pues, me convenciste.";
                return ACEPTA;
            }

            if (otroTipo)
                respuesta = $"Yo busco un {Recursos.Tipos[tipoBuscado].ToLower()}... pero si me lo dejas en ${limite:N0} lo pienso.";
            else if (precio > limite * 1.4)
                respuesta = $"¡Estás loco! Te doy ${limite:N0} y ya.";
            else
                respuesta = $"Está caro. Te doy ${limite:N0}.";
            return CONTRAOFERTA;
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
