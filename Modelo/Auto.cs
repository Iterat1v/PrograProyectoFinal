namespace PrograProyectoFinal
{
    // Un auto del mercado o del lote de un jugador
    internal class Auto
    {
        // Lo que gana el concesionario al vender: 30 % sobre lo que vale
        private const double MARGEN = 1.30;

        public const int NORMAL = 0, CHOCADO = 1, TUNEADO = 2;
        public static readonly string[] NombresEstado = { "", "CHOCADO", "TUNEADO" };

        // Fallas escondidas: no se ven en el mercado; aparecen al comprarlo (o antes, si tienes mecánico)
        public static readonly string[] NombresFalla =
        {
            "Transmisión que patina", "Motor desvielado", "Falla eléctrica", "Frenos gastados", "Kilometraje alterado",
            "Llantas lisas", "Adeudo de tenencia", "Sin verificación", "Choque escondido con masilla", "Fuga de aceite"
        };
        public static readonly double[] BajaFalla = { 0.25, 0.40, 0.10, 0.08, 0.20, 0.05, 0.12, 0.07, 0.18, 0.10 };
        // Lo que cuesta arreglarla (fracción del precio). 0 = no tiene arreglo
        public static readonly double[] CostoFalla = { 0.15, 0.30, 0.06, 0.04, 0, 0.03, 0.10, 0.05, 0.12, 0.06 };

        private string marca;
        private string modelo;
        private int anio;
        private double precio;       // lo que vale en buen estado
        private int tipo;            // 0 = Sedán, 1 = Compacto, 2 = Pickup
        private int color;           // índice del sprite en Recursos.Autos
        private bool disponible = true;
        private int estado = NORMAL;
        private double costo;        // lo que le ha costado a su dueño (compra + taller)
        private int indiceCatalogo = -1;
        private int km = 0;
        private int falla = -1;          // -1 = sin falla
        private bool fallaConocida = false;

        public Auto(string marca, string modelo, int anio, double precio, int tipo, int color)
        {
            this.marca = marca;
            this.modelo = modelo;
            this.anio = anio;
            Precio = precio;
            this.tipo = tipo;
            this.color = color;
            costo = precio;
        }

        public string Marca => marca;
        public string Modelo => modelo;
        public int Anio => anio;
        public int Tipo => tipo;
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

        // Se puede escoger al comprarlo
        public int Color
        {
            get => color;
            set
            {
                if (value < 0 || value >= Cliente.NombresColor.Length)
                    throw new ArgumentOutOfRangeException("Ese color no existe");
                color = value;
            }
        }

        public int Estado { get => estado; set => estado = value; }
        public double Costo { get => costo; set => costo = value; }
        public int IndiceCatalogo { get => indiceCatalogo; set => indiceCatalogo = value; }
        public int Km { get => km; set => km = value; }
        public int Falla { get => falla; set => falla = value; }
        public bool FallaConocida { get => fallaConocida; set => fallaConocida = value; }
        public bool TieneFalla => falla != -1;
        public bool FallaTieneArreglo => falla != -1 && CostoFalla[falla] > 0;
        public string NombreFalla => falla == -1 ? "" : NombresFalla[falla];

        // En el mercado: false cuando ya lo compró alguien
        public bool Disponible
        {
            get => disponible;
            set => disponible = value;
        }

        // Chocado vale la mitad; tuneado, 30 % más; una falla le baja lo suyo
        public double ValorActual()
        {
            double factor = estado == CHOCADO ? 0.5 : estado == TUNEADO ? 1.3 : 1;
            if (falla != -1) factor *= 1 - BajaFalla[falla];
            return Math.Round(precio * factor / 1000) * 1000;
        }

        // Lo que pide el vendedor en el mercado: si la falla no se ha descubierto, cobra como si no la tuviera
        public double ValorAnunciado()
        {
            if (falla == -1 || fallaConocida) return ValorActual();
            double factor = estado == CHOCADO ? 0.5 : estado == TUNEADO ? 1.3 : 1;
            return Math.Round(precio * factor / 1000) * 1000;
        }

        // Precio de lista al que se le ofrece al cliente
        public double PrecioVenta()
        {
            return Math.Round(ValorActual() * MARGEN / 1000) * 1000;
        }

        // Lo que cobra el taller (el mecánico propio cobra la mitad)
        public double CostoReparar(bool mecanico) => Math.Round(precio * (mecanico ? 0.12 : 0.25) / 1000) * 1000;
        public double CostoArreglarFalla(bool mecanico) =>
            falla == -1 ? 0 : Math.Round(precio * CostoFalla[falla] * (mecanico ? 0.5 : 1) / 1000) * 1000;
        public double CostoTunear(bool mecanico) => Math.Round(precio * (mecanico ? 0.10 : 0.20) / 1000) * 1000;
    }
}
