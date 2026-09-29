namespace PrograProyectoFinal
{
    // Cada jugador administra un concesionario
    internal class Concesionario
    {
        public const double PRESUPUESTO_INICIAL = 500000;
        public const int MAX_AUTOS = 10;
        public const int MAX_CARTAS = 3;

        // Vendedores (cada uno con su ventaja)
        public const int LICENCIADO = 0;   // vende 5 % más caro
        public const int DONA = 1;         // sus clientes traen 10 % más
        public const int JUNIOR = 2;       // empieza con $20,000 extra
        public const int DON_CHUY = 3;     // sus autos valen 10 % más al final

        private string jugador;
        private int vendedor;
        private double dinero = PRESUPUESTO_INICIAL;
        private Auto[] autos = new Auto[MAX_AUTOS];
        private int numAutos = 0;
        private PowerUp[] cartas = new PowerUp[MAX_CARTAS];
        private int numCartas = 0;

        // Efectos de las cartas que están activos
        private double descuento = 0;        // en la próxima compra (0.2 = 20 %)
        private double bonoVenta = 0;        // se suma a la próxima venta
        private bool ventaSegura = false;    // ¡¡Toma mi dinero!!
        private bool errorDeCuentas = false; // -20 % en la próxima venta
        private bool sobreprecio = false;    // +20 % en compras este turno
        private bool pierdeTurno = false;
        private string noticia = "";         // lo que le hicieron los rivales

        public Concesionario(string jugador, int vendedor)
        {
            this.jugador = jugador;
            this.vendedor = vendedor;
            if (vendedor == JUNIOR)
                dinero += 20000;
        }

        public string Jugador => jugador;
        public int Vendedor => vendedor;
        public int NumAutos => numAutos;
        public int NumCartas => numCartas;
        public double Descuento => descuento;

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

        public double BonoVenta { get => bonoVenta; set => bonoVenta = value; }
        public bool VentaSegura { get => ventaSegura; set => ventaSegura = value; }
        public bool ErrorDeCuentas { get => errorDeCuentas; set => errorDeCuentas = value; }
        public bool Sobreprecio { get => sobreprecio; set => sobreprecio = value; }
        public bool PierdeTurno { get => pierdeTurno; set => pierdeTurno = value; }

        // Si ya había una noticia, la nueva se agrega
        public string Noticia
        {
            get => noticia;
            set => noticia = noticia == "" || value == "" ? value : $"{noticia} {value}";
        }

        public Auto ObtenerAuto(int indice)
        {
            return autos[indice];
        }

        public PowerUp ObtenerCarta(int indice)
        {
            return cartas[indice];
        }

        // ----- compras -----

        public void DarDescuento(double porcentaje)
        {
            descuento = Math.Max(descuento, porcentaje);
        }

        // Lo que le cuesta a ESTE jugador, con descuentos o sobreprecio
        public double PrecioCompra(Auto auto)
        {
            double precio = auto.Precio * (1 - descuento);
            if (sobreprecio)
                precio *= 1.20;
            return Math.Round(precio / 1000) * 1000;
        }

        // Regresa un mensaje con lo que pasó; null si sí se pudo
        public string Comprar(Auto auto)
        {
            double precio = PrecioCompra(auto);
            if (!auto.Disponible)
                return "Ese auto ya lo compró alguien más.";
            if (numAutos == MAX_AUTOS)
                return $"Tu lote está lleno ({MAX_AUTOS} autos).";
            if (precio > dinero)
                return $"No te alcanza: cuesta ${precio:N0}.";

            Dinero -= precio;
            RecibirAuto(auto);
            auto.Disponible = false;
            descuento = 0;              // el descuento es solo para una compra
            return null;
        }

        public void RecibirAuto(Auto auto)
        {
            autos[numAutos] = auto;
            numAutos++;
        }

        // ----- ventas -----

        // Lo que pagaría el cliente por ese auto
        public double PrecioVenta(Auto auto)
        {
            double precio = auto.PrecioVenta();
            if (vendedor == LICENCIADO)
                precio *= 1.05;
            if (errorDeCuentas)
                precio *= 0.80;
            return Math.Round(precio / 1000) * 1000;
        }

        // Intenta venderle el auto al cliente. Regresa lo que contesta el cliente
        public string Vender(int indice, Cliente cliente, out bool vendido)
        {
            Auto auto = autos[indice];
            double precio = PrecioVenta(auto);
            vendido = false;

            if (!ventaSegura)
            {
                if (auto.Tipo != cliente.TipoBuscado)
                    return $"Mmm no. Yo busco un {Recursos.Tipos[cliente.TipoBuscado].ToLower()}.";
                if (precio > cliente.Presupuesto)
                    return $"¡Muy caro! Nada más traigo ${cliente.Presupuesto:N0}.";
            }

            Dinero += precio + bonoVenta;
            string extra = bonoVenta > 0 ? $" +${bonoVenta:N0} de bono." : "";
            QuitarAuto(indice);
            bonoVenta = 0;
            errorDeCuentas = false;
            ventaSegura = false;
            vendido = true;
            return $"¡Trato hecho! Me llevo el {auto.Modelo}.{extra}";
        }

        // Recorre el arreglo hacia la izquierda para no dejar huecos
        public Auto QuitarAuto(int indice)
        {
            Auto auto = autos[indice];
            for (int i = indice; i < numAutos - 1; i++)
                autos[i] = autos[i + 1];
            numAutos--;
            autos[numAutos] = null;
            return auto;
        }

        // ----- cartas -----

        public void AgregarCarta(PowerUp carta)
        {
            if (numCartas < MAX_CARTAS)
            {
                cartas[numCartas] = carta;
                numCartas++;
            }
        }

        public void QuitarCarta(int indice)
        {
            for (int i = indice; i < numCartas - 1; i++)
                cartas[i] = cartas[i + 1];
            numCartas--;
            cartas[numCartas] = null;
        }

        // Busca (búsqueda lineal) una Aseguradora que detenga la estafa.
        // Si la encuentra la gasta y regresa el mensaje; si no, null.
        public string Defender(Estafa estafa, double perdida)
        {
            for (int i = 0; i < numCartas; i++)
            {
                if (cartas[i] is Aseguradora seguro && seguro.Protege(estafa, perdida))
                {
                    QuitarCarta(i);
                    Noticia = $"Tu {seguro.Nombre} te salvó de \"{estafa.Nombre}\".";
                    return $"¡{jugador} se salvó con su {seguro.Nombre}!";
                }
            }
            return null;
        }

        // Al terminar su turno se acaban los efectos de "1 turno"
        public void FinDeTurno()
        {
            sobreprecio = false;
            pierdeTurno = false;
            ventaSegura = false;
            noticia = "";
        }

        // Ganancia = dinero + valor de los autos que le quedan
        public double Patrimonio()
        {
            double valorAutos = 0;
            for (int i = 0; i < numAutos; i++)
                valorAutos += autos[i].Precio;
            if (vendedor == DON_CHUY)
                valorAutos *= 1.10;
            return dinero + valorAutos;
        }
    }
}
