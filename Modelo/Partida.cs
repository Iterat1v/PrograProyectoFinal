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

        // ----- Eventos del día -----
        public const int TERREMOTO = 0;
        public const int HURACAN = 1;
        public const int BLACK_FRIDAY = 2;
        public const int NAVIDAD = 3;
        public const int HOT_SALE = 4;
        public const int NO_HAY_NADIE = 5;
        public const int ZOMBIES = 6;

        public static readonly string[] NombresEvento =
            { "¡TERREMOTO!", "¡HURACÁN!", "BLACK FRIDAY", "¡NAVIDAD!", "HOT SALE", "NO HAY NADIEEE", "¡HORDA DE ZOMBIES!" };
        public static readonly string[] DescripcionesEvento =
        {
            "Se abrió la tierra: cada quien pierde un auto al azar de su lote.",
            "Todos quieren salir del agua: hoy solo buscan pickup y pagan 20 % más.",
            "Todo el mercado está 30 % más barato hoy.",
            "Aguinaldo de $30,000 para todos y los clientes traen 15 % más.",
            "Los clientes andan gastalones: hoy traen 25 % más.",
            "Hoy no llegó ni un cliente. Solo se puede comprar y usar cartas.",
            "Es Halloween y llegaron los zombies: a cada quien le roban una carta (o $20,000 si no tiene)."
        };
        private static readonly string[] dias = { "LUNES", "MARTES", "MIÉRCOLES", "JUEVES", "VIERNES", "SÁBADO", "DOMINGO" };

        private int evento = -1;          // -1 = día normal
        public const int MAX_INTENTOS = 3;
        private int intentos = 0;          // ofertas rechazadas por el cliente de este turno
        private bool diaNuevo = true;     // para que la pantalla anuncie el día

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
        public bool Terminada => ronda > RONDAS || (numJugadores >= MIN_JUGADORES && Activos() <= 1);
        public int Evento => evento;
        // Cada día tiene 2 turnos: mañana y tarde
        public const int DIAS = RONDAS / 2;
        public int Dia => (ronda + 1) / 2;
        public string NombreDia => dias[(Dia - 1) % dias.Length];
        public string Momento => ronda % 2 == 1 ? "MAÑANA" : "TARDE";
        public double FactorMercado => evento == BLACK_FRIDAY ? 0.70 : 1;

        // La pantalla lo pone en false cuando ya enseñó el aviso del día
        public bool DiaNuevo
        {
            get => diaNuevo;
            set => diaNuevo = value;
        }

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
        public int Intentos => intentos;
        public Random Azar => azar;

        public Cliente NuevoCliente()
        {
            yaVendio = false;
            intentos = 0;
            if (evento == NO_HAY_NADIE)
            {
                cliente = null;
                return null;
            }

            int tipo = evento == HURACAN ? 2 : azar.Next(3);
            Auto referencia = BuscarAuto(tipo);
            // El presupuesto varía: entre 90 % y 140 % del precio de venta de un auto de ese tipo
            double presupuesto = referencia.PrecioVenta() * (0.9 + azar.NextDouble() * 0.5);
            if (JugadorActual.Vendedor == Concesionario.DONA)
                presupuesto *= 1.10;      // clientela fiel
            if (evento == HURACAN) presupuesto *= 1.20;
            if (evento == NAVIDAD) presupuesto *= 1.15;
            if (evento == HOT_SALE) presupuesto *= 1.25;
            presupuesto = Math.Round(presupuesto / 5000) * 5000;

            int retrato = azar.Next(nombresClientes.Length);
            string frase = frases[tipo, azar.Next(3)];
            cliente = new Cliente(nombresClientes[retrato], presupuesto, tipo, frase, retrato);
            // 1 de cada 3 lo quiere de un color: si no lo tienes, lo pintas
            if (azar.Next(3) == 0)
                cliente.ColorBuscado = azar.Next(Cliente.NombresColor.Length);
            return cliente;
        }

        // ----- Decisión: un señor te ofrece su coche -----
        private static string[] senores = { "Don Goyo", "Doña Chela", "El Tío Beto", "Don Ramiro" };
        private static string[,] cochesSenor =
        {
            { "Tsuru", "2005" }, { "Golf", "2009" }, { "Datsun", "1994" }, { "Pointer", "2007" },
            { "Explorer", "2011" }, { "Cavalier", "2004" }, { "F-150", "2008" }, { "Platina", "2006" }
        };

        public Auto OfertaAuto { get; private set; }
        public double OfertaPrecio { get; private set; }
        public string OfertaSenor { get; private set; }

        // 1 de cada 5 turnos llega un señor que quiere vender su coche barato
        public bool HayOferta()
        {
            if (azar.Next(5) != 0) return false;
            int c = azar.Next(cochesSenor.GetLength(0));
            int tipo = azar.Next(3);
            double valor = Math.Round((50000 + azar.Next(150000)) / 5000.0) * 5000;
            OfertaAuto = new Auto("", cochesSenor[c, 0], int.Parse(cochesSenor[c, 1]), valor, tipo, azar.Next(8));
            OfertaPrecio = Math.Round(valor * (0.5 + azar.NextDouble() * 0.2) / 1000) * 1000;
            OfertaSenor = senores[azar.Next(senores.Length)];
            return true;
        }

        // Después de vender, a veces llega de la nada un bro con cash
        public bool TalVezLlegaBro()
        {
            if (cliente == null || azar.Next(100) >= 25)
                return false;
            cliente = new Cliente("El Bro", 9999999, -1, "¡Qué onda, carnal! Traigo cash. Véndeme lo que sea y ponle tú el precio.", 8, 1.25);
            yaVendio = false;
            intentos = 0;
            return true;
        }

        // El jugador le pide un precio al cliente por el auto "indice" de su lote.
        // true = se vendió. Si no, "contraoferta" es lo que daría el cliente.
        public bool OfrecerPrecio(int indice, double precio, out string respuesta, out double contraoferta, out double recibido)
        {
            recibido = 0;
            contraoferta = 0;
            if (cliente == null || yaVendio)
            {
                respuesta = "No hay nadie...";
                return false;
            }

            Concesionario j = JugadorActual;
            Auto auto = j.ObtenerAuto(indice);
            int decision = cliente.Evaluar(auto, precio, j.Vendedor == Concesionario.LICENCIADO, j.VentaSegura, azar,
                                           out respuesta, out contraoferta);
            if (decision == Cliente.ACEPTA)
            {
                recibido = j.CerrarVenta(indice, cliente, precio, out string detalle);
                respuesta += detalle;
                yaVendio = true;
                return true;
            }

            intentos++;
            if (intentos >= MAX_INTENTOS)
            {
                respuesta = "¡Ya me cansé de regatear! Me voy a otro lado.";
                yaVendio = true;      // el cliente se fue: ya no se puede vender este turno
            }
            return false;
        }

        // Se salta a los que se rindieron. Cuando se da la vuelta empieza otro día.
        public void SiguienteTurno()
        {
            JugadorActual.FinDeTurno();
            do
            {
                turno = (turno + 1) % numJugadores;
                if (turno == 0)
                {
                    ronda++;
                    if (!Terminada && ronda % 2 == 1)
                        IniciarDia();
                }
            } while (jugadores[turno].Rendido && !Terminada);

            if (!Terminada)
                NuevoCliente();
        }

        // Sortea el evento del día (desde el día 2, 45 % de probabilidad, sin repetir el de ayer)
        private void IniciarDia()
        {
            int anterior = evento;
            evento = -1;
            if (azar.Next(100) < 45)
            {
                do evento = azar.Next(NombresEvento.Length);
                while (evento == anterior);
            }
            diaNuevo = true;

            for (int i = 0; i < numJugadores; i++)
            {
                Concesionario j = jugadores[i];
                if (j.Rendido) continue;
                if (evento == NAVIDAD)
                    j.Dinero += 30000;
                if (evento == TERREMOTO && j.NumAutos > 0)
                {
                    Auto perdido = j.QuitarAuto(azar.Next(j.NumAutos));
                    j.Noticia = $"El terremoto se tragó tu {perdido.Nombre}.";
                }
                if (evento == ZOMBIES)
                    j.Noticia = j.PerderPorZombies(azar);
            }
        }

        public int Activos()
        {
            int activos = 0;
            for (int i = 0; i < numJugadores; i++)
                if (!jugadores[i].Rendido)
                    activos++;
            return activos;
        }

        // El jugador en turno abandona (forfeit). Queda al final del ranking.
        public void Rendirse()
        {
            JugadorActual.Rendido = true;
            if (!Terminada)
                SiguienteTurno();
        }

        // Misma gente con los mismos vendedores, todo de cero
        public Partida Revancha()
        {
            Partida nueva = new Partida();
            for (int i = 0; i < numJugadores; i++)
            {
                Concesionario j = new Concesionario(jugadores[i].Jugador, jugadores[i].Vendedor, jugadores[i].NivelBot);
                j.Perfil = jugadores[i].Perfil;
                j.Cara = jugadores[i].Cara;
                nueva.AgregarJugador(j);
            }
            return nueva;
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

        // El que se rindió queda hasta abajo
        private static double Puntos(Concesionario c)
        {
            return c.Rendido ? -1 : c.Patrimonio();
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
                    if (Puntos(orden[j]) < Puntos(orden[j + 1]))
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
