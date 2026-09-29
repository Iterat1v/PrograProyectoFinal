namespace PrograProyectoFinal
{
    // Cada jugador administra un concesionario
    internal class Concesionario
    {
        public const double PRESUPUESTO_INICIAL = 500000;
        public const int MAX_AUTOS = 10;
        public const int MAX_CARTAS = 3;
        public const double PINTURA = 10000;   // pintar un auto del color que pide el cliente

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
        private bool rendido = false;
        private bool esBot = false;

        // Estadísticas para el podio
        private int ventas = 0;
        private double mejorVenta = 0;
        private string mejorVentaAuto = "";

        public Concesionario(string jugador, int vendedor, bool esBot) : this(jugador, vendedor)
        {
            this.esBot = esBot;
        }

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
        public bool Rendido { get => rendido; set => rendido = value; }
        public bool EsBot => esBot;
        public int Ventas => ventas;
        public double MejorVenta => mejorVenta;
        public string MejorVentaAuto => mejorVentaAuto;

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
        // factorEvento: 0.7 en Black Friday, 1 en un día normal
        public double PrecioCompra(Auto auto, double factorEvento = 1)
        {
            double precio = auto.Precio * (1 - descuento) * factorEvento;
            if (sobreprecio)
                precio *= 1.20;
            return Math.Round(precio / 1000) * 1000;
        }

        // Regresa un mensaje con lo que pasó; null si sí se pudo
        public string Comprar(Auto auto, double factorEvento = 1)
        {
            double precio = PrecioCompra(auto, factorEvento);
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

        // Compra fuera del mercado (el señor que vende su coche)
        public string CompraDirecta(Auto auto, double precio)
        {
            if (numAutos == MAX_AUTOS)
                return "Tu lote está lleno.";
            if (precio > dinero)
                return "No te alcanza.";
            Dinero -= precio;
            RecibirAuto(auto);
            return null;
        }

        // Los zombies se llevan una carta; si no tiene, dinero
        public string PerderPorZombies(Random azar)
        {
            if (numCartas > 0)
            {
                int i = azar.Next(numCartas);
                string nombre = cartas[i].Nombre;
                QuitarCarta(i);
                return $"Los zombies se llevaron tu carta \"{nombre}\".";
            }
            double perdida = Math.Min(20000, dinero);
            Dinero -= perdida;
            return $"Los zombies se llevaron ${perdida:N0} de la caja.";
        }

        public void RecibirAuto(Auto auto)
        {
            autos[numAutos] = auto;
            numAutos++;
        }

        // ----- ventas -----

        // Precio de lista que se sugiere (30 % sobre lo que costó; el Licenciado 5 % más)
        public double PrecioVenta(Auto auto)
        {
            double precio = auto.PrecioVenta();
            if (vendedor == LICENCIADO)
                precio *= 1.05;
            return Math.Round(precio / 1000) * 1000;
        }

        // Cierra la venta al precio que se negoció. Regresa lo que ganó de verdad.
        public double CerrarVenta(int indice, Cliente cliente, double precio, out string detalle)
        {
            Auto auto = autos[indice];
            double recibido = precio;
            detalle = "";

            if (errorDeCuentas)
            {
                recibido *= 0.80;
                detalle += " Error de cuentas: -20 %.";
            }
            // Si lo quiere de otro color, hay que pintarlo
            if (cliente.ColorBuscado != -1 && auto.Color != cliente.ColorBuscado)
            {
                recibido -= PINTURA;
                detalle += $" Lo pintaste de {Cliente.NombresColor[cliente.ColorBuscado]}: -${PINTURA:N0}.";
            }
            if (bonoVenta > 0)
            {
                recibido += bonoVenta;
                detalle += $" Bono: +${bonoVenta:N0}.";
            }

            Dinero = Math.Max(0, dinero + recibido);
            QuitarAuto(indice);
            bonoVenta = 0;
            errorDeCuentas = false;
            ventaSegura = false;

            ventas++;
            if (precio > mejorVenta)
            {
                mejorVenta = precio;
                mejorVentaAuto = auto.Nombre;
            }
            return recibido;
        }

        // Rematar: se lo vendes al lote de enfrente al 70 % y regresa al mercado
        public const double REMATE = 0.70;

        public double PrecioRemate(Auto auto)
        {
            return Math.Round(auto.Precio * REMATE / 1000) * 1000;
        }

        public double Rematar(int indice)
        {
            Auto auto = QuitarAuto(indice);
            double precio = PrecioRemate(auto);
            Dinero += precio;
            auto.Disponible = true;
            return precio;
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
