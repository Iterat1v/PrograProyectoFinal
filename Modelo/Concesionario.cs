namespace PrograProyectoFinal
{
    // Cada jugador administra un concesionario
    internal class Concesionario
    {
        public const double PRESUPUESTO_INICIAL = 500000;
        public const int MAX_AUTOS = 10;

        private string jugador;
        private int vendedor;                        // 0 Licenciado, 1 Doña, 2 Junior, 3 Don Chuy
        private double dinero = PRESUPUESTO_INICIAL;
        private Auto[] autos = new Auto[MAX_AUTOS];
        private int numAutos = 0;
        // Las cartas (PowerUp[3]) las agrega el equipo de las cartas

        public Concesionario(string jugador, int vendedor)
        {
            this.jugador = jugador;
            this.vendedor = vendedor;
        }

        public string Jugador => jugador;
        public int Vendedor => vendedor;
        public int NumAutos => numAutos;

        public double Dinero
        {
            get => dinero;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("El dinero no puede quedar en negativo");
                else
                    dinero = value;
            }
        }

        public Auto ObtenerAuto(int indice)
        {
            return autos[indice];
        }

        // Regresa un mensaje con lo que pasó; null si sí se pudo
        public string Comprar(Auto auto)
        {
            if (!auto.Disponible)
                return "Ese auto ya lo compró alguien más.";
            if (numAutos == MAX_AUTOS)
                return $"Tu lote está lleno ({MAX_AUTOS} autos).";
            if (auto.Precio > dinero)
                return $"No te alcanza: cuesta ${auto.Precio:N0}.";

            Dinero -= auto.Precio;
            autos[numAutos] = auto;
            numAutos++;
            auto.Disponible = false;
            return null;
        }

        // Intenta venderle el auto al cliente. Regresa lo que contesta el cliente
        public string Vender(int indice, Cliente cliente, out bool vendido)
        {
            Auto auto = autos[indice];
            vendido = false;

            if (auto.Tipo != cliente.TipoBuscado)
                return $"Mmm no. Yo busco un {Recursos.Tipos[cliente.TipoBuscado].ToLower()}.";
            if (auto.PrecioVenta() > cliente.Presupuesto)
                return $"¡Muy caro! Nada más traigo ${cliente.Presupuesto:N0}.";

            Dinero += auto.PrecioVenta();
            QuitarAuto(indice);
            vendido = true;
            return $"¡Trato hecho! Me llevo el {auto.Modelo}.";
        }

        // Recorre el arreglo hacia la izquierda para no dejar huecos
        private void QuitarAuto(int indice)
        {
            for (int i = indice; i < numAutos - 1; i++)
                autos[i] = autos[i + 1];
            numAutos--;
            autos[numAutos] = null;
        }

        // Ganancia = dinero + valor de los autos que le quedan
        public double Patrimonio()
        {
            double total = dinero;
            for (int i = 0; i < numAutos; i++)
                total += autos[i].Precio;
            return total;
        }
    }
}
