namespace PrograProyectoFinal
{
    // Guarda todo lo de una partida y se pasa de una pantalla a otra
    internal class Partida
    {
        public const int MAX_JUGADORES = 4;
        public const int MIN_JUGADORES = 2;
        public const int RONDAS = 10;

        private static Random azar = new Random();

        private Concesionario[] jugadores = new Concesionario[MAX_JUGADORES];
        private int numJugadores = 0;
        private Auto[] catalogo = new Auto[30];
        private int ronda = 1;
        private int turno = 0;
        private Cliente cliente;
        private bool yaVendio = false;

        // Draft: 16 cartas de la baraja en una cuadrícula de 4 x 4
        private Baraja baraja = new Baraja();
        private PowerUp[,] draft = new PowerUp[4, 4];
        private int[,] duenoDraft = new int[4, 4];   // índice del jugador que la tomó, -1 = libre
        private int rondaDraft = 0;                   // 0, 1, 2: cada jugador escoge 3
        private int posicionDraft = 0;

        private static string[] nombresClientes =
            { "Don Beto", "Doña Lupe", "Kevin", "Sofi", "El Ingeniero", "Don Pancho", "La Güera", "El Profe" };

        // Frases por tipo de auto: [tipo, frase]
        private static string[,] frases =
        {
            { "Busco algo para Uber.", "Quiero un sedán para la familia.", "Necesito carro para la oficina." },
            { "Busco algo chiquito para la escuela.", "Algo que no gaste gasolina.", "Es mi primer carro, algo compacto." },
            { "Necesito una camioneta para la obra.", "Busco pickup para el rancho.", "Algo que aguante carga pesada." }
        };

        public Partida()
        {
            LlenarCatalogo();
        }

        public int NumJugadores => numJugadores;
        public int Ronda => ronda;
        public int Turno => turno;
        public Cliente ClienteActual => cliente;
        public bool YaVendio => yaVendio;
        public Concesionario JugadorActual => jugadores[turno];
        public Auto[] Catalogo => catalogo;
        public bool Terminada => ronda > RONDAS;

        public Concesionario ObtenerJugador(int indice)
        {
            return jugadores[indice];
        }

        public void AgregarJugador(Concesionario jugador)
        {
            if (numJugadores == MAX_JUGADORES)
                throw new InvalidOperationException("Ya hay 4 jugadores");
            jugadores[numJugadores] = jugador;
            numJugadores++;
        }

        // 30 autos: 10 sedanes, 10 compactos y 10 pickups (tipo, color del sprite)
        private void LlenarCatalogo()
        {
            catalogo[0] = new Auto("Volkswagen", "Jetta", 2015, 98000, 0, 0);
            catalogo[1] = new Auto("Nissan", "Tsuru", 2012, 55000, 0, 5);
            catalogo[2] = new Auto("Nissan", "Sentra", 2018, 150000, 0, 7);
            catalogo[3] = new Auto("Chevrolet", "Aveo", 2016, 72000, 0, 3);
            catalogo[4] = new Auto("Nissan", "Versa", 2019, 135000, 0, 6);
            catalogo[5] = new Auto("Toyota", "Corolla", 2017, 165000, 0, 4);
            catalogo[6] = new Auto("Honda", "Civic", 2016, 175000, 0, 1);
            catalogo[7] = new Auto("Mazda", "Mazda 3", 2018, 190000, 0, 0);
            catalogo[8] = new Auto("Volkswagen", "Vento", 2017, 110000, 0, 2);
            catalogo[9] = new Auto("Hyundai", "Accent", 2019, 125000, 0, 3);

            catalogo[10] = new Auto("Volkswagen", "Vocho", 1998, 45000, 1, 1);
            catalogo[11] = new Auto("Chevrolet", "Chevy", 2008, 40000, 1, 2);
            catalogo[12] = new Auto("Nissan", "March", 2017, 85000, 1, 4);
            catalogo[13] = new Auto("Chevrolet", "Spark", 2016, 60000, 1, 6);
            catalogo[14] = new Auto("Dodge", "Atos", 2010, 38000, 1, 5);
            catalogo[15] = new Auto("Volkswagen", "Beetle", 2014, 140000, 1, 0);
            catalogo[16] = new Auto("Mini", "Cooper", 2013, 180000, 1, 3);
            catalogo[17] = new Auto("Fiat", "500", 2015, 95000, 1, 4);
            catalogo[18] = new Auto("Hyundai", "i10", 2019, 105000, 1, 7);
            catalogo[19] = new Auto("Renault", "Kwid", 2020, 120000, 1, 2);

            catalogo[20] = new Auto("Toyota", "Hilux", 2019, 160000, 2, 2);
            catalogo[21] = new Auto("Ford", "Lobo", 2014, 190000, 2, 0);
            catalogo[22] = new Auto("Toyota", "Tacoma", 2016, 250000, 2, 7);
            catalogo[23] = new Auto("Ford", "Ranger", 2015, 170000, 2, 3);
            catalogo[24] = new Auto("Nissan", "NP300", 2018, 145000, 2, 5);
            catalogo[25] = new Auto("Mitsubishi", "L200", 2017, 155000, 2, 6);
            catalogo[26] = new Auto("RAM", "RAM 700", 2019, 175000, 2, 1);
            catalogo[27] = new Auto("Chevrolet", "Tornado", 2016, 90000, 2, 0);
            catalogo[28] = new Auto("Nissan", "Frontier", 2020, 260000, 2, 7);
            catalogo[29] = new Auto("Chevrolet", "Silverado", 2012, 200000, 2, 3);
        }

        // Búsqueda lineal: empieza en un lugar al azar del catálogo y da la vuelta
        // hasta encontrar un auto del tipo que se pide
        public Auto BuscarAuto(int tipo)
        {
            int inicio = azar.Next(catalogo.Length);
            for (int i = 0; i < catalogo.Length; i++)
            {
                Auto auto = catalogo[(inicio + i) % catalogo.Length];
                if (auto.Tipo == tipo)
                    return auto;
            }
            return null;
        }

        // Llega un cliente con el tipo de auto que busca y cuánto trae
        public Cliente NuevoCliente()
        {
            int tipo = azar.Next(3);
            Auto referencia = BuscarAuto(tipo);
            // El presupuesto varía: entre 90 % y 140 % del precio de venta de un auto de ese tipo
            double presupuesto = referencia.PrecioVenta() * (0.9 + azar.NextDouble() * 0.5);
            if (JugadorActual.Vendedor == Concesionario.DONA)
                presupuesto *= 1.10;      // clientela fiel
            presupuesto = Math.Round(presupuesto / 5000) * 5000;

            int retrato = azar.Next(nombresClientes.Length);
            string frase = frases[tipo, azar.Next(3)];
            cliente = new Cliente(nombresClientes[retrato], presupuesto, tipo, frase, retrato);
            yaVendio = false;
            return cliente;
        }

        public string Vender(int indice, out bool vendido)
        {
            string respuesta = JugadorActual.Vender(indice, cliente, out vendido);
            if (vendido)
                yaVendio = true;
            return respuesta;
        }

        // turno = (turno + 1) % n; cuando se da la vuelta empieza otra ronda
        public void SiguienteTurno()
        {
            JugadorActual.FinDeTurno();
            turno = (turno + 1) % numJugadores;
            if (turno == 0)
                ronda++;
            if (!Terminada)
                NuevoCliente();
        }

        // ----- Draft de cartas -----

        public void PrepararDraft()
        {
            baraja.Barajar();
            for (int f = 0; f < 4; f++)
                for (int c = 0; c < 4; c++)
                {
                    draft[f, c] = baraja.Sacar();
                    duenoDraft[f, c] = -1;
                }
            rondaDraft = 0;
            posicionDraft = 0;
        }

        public PowerUp CartaDraft(int fila, int columna) => draft[fila, columna];
        public int DuenoDraft(int fila, int columna) => duenoDraft[fila, columna];
        public int RondaDraft => rondaDraft + 1;
        public bool DraftTerminado => rondaDraft == Concesionario.MAX_CARTAS;

        // Orden en serpiente: 1-2-3-4, luego 4-3-2-1, luego 1-2-3-4.
        // Así el último no se queda siempre con lo que sobra.
        public int OrdenDraft(int posicion)
        {
            return rondaDraft % 2 == 0 ? posicion : numJugadores - 1 - posicion;
        }

        public int JugadorDraft => OrdenDraft(posicionDraft);
        public int PosicionDraft => posicionDraft;

        public bool EscogerCarta(int fila, int columna)
        {
            if (DraftTerminado || duenoDraft[fila, columna] != -1)
                return false;

            duenoDraft[fila, columna] = JugadorDraft;
            jugadores[JugadorDraft].AgregarCarta(draft[fila, columna]);
            posicionDraft++;
            if (posicionDraft == numJugadores)
            {
                posicionDraft = 0;
                rondaDraft++;
            }
            return true;
        }

        // Ranking con el método de la burbuja, de mayor a menor patrimonio
        public Concesionario[] Ranking()
        {
            Concesionario[] orden = new Concesionario[numJugadores];
            for (int i = 0; i < numJugadores; i++)
                orden[i] = jugadores[i];

            for (int i = 0; i < numJugadores - 1; i++)
            {
                for (int j = 0; j < numJugadores - 1 - i; j++)
                {
                    if (orden[j].Patrimonio() < orden[j + 1].Patrimonio())
                    {
                        Concesionario temporal = orden[j];
                        orden[j] = orden[j + 1];
                        orden[j + 1] = temporal;
                    }
                }
            }
            return orden;
        }
    }
}
