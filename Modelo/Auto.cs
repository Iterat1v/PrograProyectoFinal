namespace PrograProyectoFinal
{
    // Un auto del mercado o del lote de un jugador
    internal class Auto
    {
        // Lo que gana el concesionario al vender: 30 % sobre lo que pagó
        private const double MARGEN = 1.30;

        private string marca;
        private string modelo;
        private int anio;
        private double precio;
        private int tipo;      // 0 = Sedán, 1 = Compacto, 2 = Pickup
        private int color;     // índice del sprite en Recursos.Autos
        private bool disponible = true;

        public Auto(string marca, string modelo, int anio, double precio, int tipo, int color)
        {
            this.marca = marca;
            this.modelo = modelo;
            this.anio = anio;
            Precio = precio;
            this.tipo = tipo;
            this.color = color;
        }

        public string Marca => marca;
        public string Modelo => modelo;
        public int Anio => anio;
        public int Tipo => tipo;
        public int Color => color;
        public string Nombre => $"{modelo} {anio}";
        public string NombreTipo => Recursos.Tipos[tipo];

        public double Precio
        {
            get => precio;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("El precio tiene que ser mayor a cero");
                else
                    precio = value;
            }
        }

        // En el mercado: false cuando ya lo compró alguien
        public bool Disponible
        {
            get => disponible;
            set => disponible = value;
        }

        // Precio al que se le vende al cliente
        public double PrecioVenta()
        {
            return Math.Round(precio * MARGEN / 1000) * 1000;
        }
    }
}
