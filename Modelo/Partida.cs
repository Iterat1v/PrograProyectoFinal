namespace PrograProyectoFinal
{
    // Guarda todo lo de una partida y se pasa de una pantalla a otra
    internal class Partida
    {
        public const int MAX_JUGADORES = 4;
        public const int MIN_JUGADORES = 2;
        public const int RONDAS = 10;

        // Cada partida tiene su propio Random. En el reto del día se crea con la fecha
        // como semilla, así a todos les salen las mismas cartas, autos y eventos.
        private Random azar;
        private int semilla = 0;          // 0 = partida normal; aaaammdd = reto del día

        // ----- Dificultad (solo partida libre) -----
        public const int FACIL = 0, NORMAL = 1, DIFICIL = 2;
        public static readonly string[] NombresDificultad = { "FÁCIL", "NORMAL", "DIFÍCIL" };
        public static readonly string[] DescripcionesDificultad =
        {
            "Clientes con 15 % más, 4 intentos para regatear y $50,000 extra.",
            "Como se diseñó el juego: 3 intentos para regatear.",
            "Clientes con 12 % menos, solo 2 intentos y $50,000 menos."
        };
        private static readonly double[] clientesDificultad = { 1.15, 1.0, 0.88 };
        private static readonly int[] intentosDificultad = { 4, 3, 2 };
        private static readonly double[] dineroDificultad = { 50000, 0, -50000 };
        private int dificultad = NORMAL;
        private int tema = 0;             // tema de la temporada (cambia la economía)

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

        // ----- Zonas de la ciudad (de Ecatepec a Polanco) -----
        public static readonly string[] Zonas = { "ECATEPEC", "NEZA", "COYOACÁN", "SANTA FE", "POLANCO" };
        public static readonly double[] FactorZona = { 0.85, 0.95, 1.10, 1.25, 1.45 };   // cuánto traen los clientes
        private int zona = 0;
        private Historia historia;       // null = partida libre

        // ----- Eventos del día -----
        public const int TERREMOTO = 0;
        public const int HURACAN = 1;
        public const int BLACK_FRIDAY = 2;
        public const int NAVIDAD = 3;
        public const int HOT_SALE = 4;
        public const int NO_HAY_NADIE = 5;
        public const int ZOMBIES = 6;
        public const int SUBASTA = 7;

        public static readonly string[] NombresEvento =
            { "¡TERREMOTO!", "¡HURACÁN!", "BLACK FRIDAY", "¡NAVIDAD!", "HOT SALE", "NO HAY NADIEEE", "¡HORDA DE ZOMBIES!", "¡SUBASTA!" };
        public static readonly string[] DescripcionesEvento =
        {
            "Se abrió la tierra: cada quien pierde un auto. Al que va ganando se le va el más caro.",
            "Todos quieren salir del agua: hoy solo buscan pickup y pagan 20 % más.",
            "Todo el mercado está 30 % más barato hoy.",
            "Aguinaldo de $30,000 para todos y los clientes traen 15 % más.",
            "Los clientes andan gastalones: hoy traen 25 % más.",
            "Hoy no llegó ni un cliente. Solo se puede comprar y usar cartas.",
            "Es Halloween y llegaron los zombies: a cada quien le roban una carta (o $20,000 si no tiene).",
            "Llegó un auto clásico y se subasta: cada quien ofrece en secreto y se lo lleva el que dé más."
        };
        private static readonly string[] dias = { "LUNES", "MARTES", "MIÉRCOLES", "JUEVES", "VIERNES", "SÁBADO", "DOMINGO" };

        private int evento = -1;          // -1 = día normal
        private int intentos = 0;          // ofertas rechazadas por el cliente de este turno
        private bool diaNuevo = true;      // para que la pantalla anuncie el día

        // Votación: una vez por partida todos votan el evento del día
        private int diaVotacion;
        private bool votacionPendiente = false;
        private int[] opcionesVoto = new int[3];

        // Subasta
        private static string[,] clasicos = { { "Mustang", "1967" }, { "Impala", "1964" }, { "Combi", "1975" }, { "Caribe", "1987" }, { "Vocho Oro", "1970" } };
        private Auto autoSubasta;

        // ----- Clientes que regresan -----
        private static string[] nombresClientes =
            { "Don Beto", "Doña Lupe", "Kevin", "Sofi", "El Ingeniero", "Don Pancho", "La Güera", "El Profe" };

        // [cliente, visita]: lo que busca y dice en su 1a, 2a y 3a visita
        private static int[,] tipoHistoria =
        {
            { 0, 1, 0 }, { 1, 0, 2 }, { 1, 1, 0 }, { 1, 0, 1 }, { 0, 2, 2 }, { 2, 2, 0 }, { 0, 1, 1 }, { 0, 0, 1 }
        };
        private static string[,] frasesHistoria =
        {
            { "Busco algo para Uber.", "¡Otra vez yo! Ahora quiero uno para mi hija.", "Mi hija ya choca menos. Ahora uno para mí, pa' jubilarme." },
            { "Busco algo chiquito para ir al mercado.", "Mi nieto ya entró a la prepa, quiere su nave.", "Ahora sí quiero una troca para la fonda." },
            { "Mi primer carro, carnal. Algo compacto.", "Ya tengo novia, quiero algo con más estilo.", "Me corrieron del trabajo, pero algo barato sí." },
            { "Para ir a la uni, algo chiquito.", "Me gradué. Quiero uno de adulta, un sedán.", "Me mudo a Querétaro, algo que aguante." },
            { "Algo serio para la oficina.", "Según mis cálculos, necesito una pickup para la obra.", "La obra creció. Otra pickup." },
            { "Una camioneta para el rancho.", "Se me murió la troca vieja. Otra, joven.", "Ahora mi señora quiere un sedán." },
            { "Algo bonito, que se vea.", "Ya me aburrí del otro. Otro compacto.", "Algo que combine con mi perrito." },
            { "Para ir a dar clases.", "Voy a llevar a mis alumnos de excursión... bueno, a tres.", "Me jubilo. Algo compacto y tranquilo." }
        };
        private int[] visitas = new int[8];
        private string[] ultimaCompra = new string[8];
        private int[] recuerdo = new int[8];          // 0 nada, 1 le fue bien, 2 le salió malo

        // Frases por tipo de auto (para los que no son de historia): [tipo, frase]
        private static string[,] frases =
        {
            { "Busco algo para Uber.", "Quiero un sedán para la familia.", "Necesito carro para la oficina." },
            { "Busco algo chiquito para la escuela.", "Algo que no gaste gasolina.", "Es mi primer carro, algo compacto." },
            { "Necesito una camioneta para la obra.", "Busco pickup para el rancho.", "Algo que aguante carga pesada." }
        };

        // ----- Estafas: combos y la mejor de la partida -----
        private int[] estafasHoy = new int[MAX_JUGADORES];
        public string MejorEstafaTexto { get; private set; } = "";
        public double MejorEstafaPerdida { get; private set; } = 0;
        public Concesionario MejorEstafaUsuario { get; private set; }
        public Concesionario MejorEstafaRival { get; private set; }
        public string MejorEstafaCarta { get; private set; } = "";

        public Partida() : this(0, null)
        {
        }

        public Partida(int zona, Historia historia) : this(zona, historia, 0)
        {
        }

        public Partida(int zona, Historia historia, int semilla)
        {
            this.zona = zona;
            this.historia = historia;
            this.semilla = semilla;
            azar = semilla == 0 ? new Random() : new Random(semilla);
            // El tema de la temporada solo aplica en la partida libre
            if (historia == null && semilla == 0)
                tema = Temporada.TemaActual;
            LlenarCatalogo();
            diaVotacion = 2 + azar.Next(3);       // la votación cae el día 2, 3 o 4
        }

        public int Semilla => semilla;
        public bool EsReto => semilla != 0;
        public bool EsLibre => historia == null && semilla == 0;
        public int Tema => tema;
        public int IntentosMaximos => intentosDificultad[dificultad];

        public int Dificultad
        {
            get => dificultad;
            set => dificultad = value;
        }

        // ----- Registro del patrimonio de cada día (para la gráfica del final) -----
        private int diasRegistrados = 0;
        public int DiasRegistrados => diasRegistrados;

        public void RegistrarPatrimonios(int dia)
        {
            if (dia > DIAS) return;
            for (int i = 0; i < numJugadores; i++)
                jugadores[i].RegistrarDia(dia);
            diasRegistrados = Math.Max(diasRegistrados, dia + 1);
        }

        // Si la partida terminó antes (todos se rindieron), se apunta cómo quedó
        public void CerrarRegistro()
        {
            int dia = Math.Min(Dia, DIAS);
            if (diasRegistrados <= dia)
                RegistrarPatrimonios(dia);
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
        public int Zona => zona;
        public Historia Historia => historia;
        public bool VotacionPendiente => votacionPendiente;
        public int[] OpcionesVoto => opcionesVoto;
        public Auto AutoSubasta => autoSubasta;

        // Cada día tiene 2 turnos: mañana y tarde
        public const int DIAS = RONDAS / 2;
        public int Dia => (ronda + 1) / 2;
        public string NombreDia => dias[(Dia - 1) % dias.Length];
        public string Momento => ronda % 2 == 1 ? "MAÑANA" : "TARDE";
        public double FactorMercado => (evento == BLACK_FRIDAY ? 0.70 : 1) * Temporada.FactorMercado[tema];

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

        public int IndiceDe(Concesionario c)
        {
            for (int i = 0; i < numJugadores; i++)
                if (jugadores[i] == c) return i;
            return -1;
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

            for (int i = 0; i < catalogo.Length; i++)
                catalogo[i].IndiceCatalogo = i;

            // 6 autos llegan chocados: salen a mitad de precio, pero hay que repararlos
            for (int k = 0; k < 6; k++)
                catalogo[azar.Next(catalogo.Length)].Estado = Auto.CHOCADO;
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

        public int Intentos => intentos;
        public Random Azar => azar;

        // Llega un cliente. Si ya había venido, regresa con su historia.
        public Cliente NuevoCliente()
        {
            yaVendio = false;
            intentos = 0;
            Concesionario j = JugadorActual;
            if (evento == NO_HAY_NADIE)
            {
                cliente = null;
                return null;
            }
            // Con mala fama, a veces no llega nadie
            if (j.Reputacion < 25 && azar.Next(100) < 35)
            {
                cliente = null;
                j.Noticia = "Nadie quiso entrar a tu lote: tienes mala fama. Sube tu reputación.";
                return null;
            }

            int retrato = azar.Next(nombresClientes.Length);
            int visita = Math.Min(visitas[retrato], 2);
            int tipo = evento == HURACAN ? 2 : tipoHistoria[retrato, visita];
            string frase = frasesHistoria[retrato, visita];
            if (visitas[retrato] > 2)
            {
                tipo = evento == HURACAN ? 2 : azar.Next(3);
                frase = frases[tipo, azar.Next(3)];
            }
            visitas[retrato]++;

            Auto referencia = BuscarAuto(tipo);
            // El presupuesto varía: entre 90 % y 140 % del precio de venta de un auto de ese tipo
            double presupuesto = referencia.Precio * 1.3 * (0.9 + azar.NextDouble() * 0.5);
            if (j.Vendedor == Concesionario.DONA) presupuesto *= 1.10;      // clientela fiel
            if (evento == HURACAN) presupuesto *= 1.20;
            if (evento == NAVIDAD) presupuesto *= 1.15;
            if (evento == HOT_SALE) presupuesto *= 1.25;
            if (historia != null && historia.Influencer && historia.Semana == 2) presupuesto *= 1.20;
            presupuesto *= FactorZona[zona];
            presupuesto *= Temporada.FactorClientes[tema] * clientesDificultad[dificultad];
            presupuesto *= 0.85 + j.Reputacion * 0.003;       // buena fama = clientes con más dinero
            presupuesto = Math.Round(presupuesto / 5000) * 5000;

            // Si ya te compró antes, se acuerda
            double factorRecuerdo = 1;
            if (recuerdo[retrato] == 1)
            {
                frase = $"¡El {ultimaCompra[retrato]} que me vendiste salió buenísimo! {frase}";
                factorRecuerdo = 1.15;
            }
            else if (recuerdo[retrato] == 2)
            {
                frase = $"¡El {ultimaCompra[retrato]} que me vendiste se desbarató! {frase}";
                factorRecuerdo = 0.80;
            }

            cliente = new Cliente(nombresClientes[retrato], presupuesto, tipo, frase, retrato);
            cliente.FactorRecuerdo = factorRecuerdo;
            int favorito = azar.Next(Cliente.NombresColor.Length);
            int odiado;
            do odiado = azar.Next(Cliente.NombresColor.Length); while (odiado == favorito);
            cliente.Gustos(favorito, odiado);
            // 1 de cada 3 lo quiere de un color: si no lo tienes, lo pintas
            if (azar.Next(3) == 0)
                cliente.ColorBuscado = favorito;
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

        // 1 de cada 5 turnos llega un señor que quiere vender su coche barato.
        // Si tu lote está vacío es más seguido (3 de cada 5), para que no te quedes sin nada.
        public bool HayOferta()
        {
            int probabilidad = JugadorActual.NumAutos == 0 ? 60 : 20;
            if (azar.Next(100) >= probabilidad) return false;
            int c = azar.Next(cochesSenor.GetLength(0));
            int tipo = azar.Next(3);
            double valor = Math.Round((50000 + azar.Next(150000)) / 5000.0) * 5000;
            OfertaAuto = new Auto("", cochesSenor[c, 0], int.Parse(cochesSenor[c, 1]), valor, tipo, azar.Next(8));
            if (azar.Next(3) == 0) OfertaAuto.Estado = Auto.CHOCADO;
            OfertaPrecio = Math.Round(OfertaAuto.ValorActual() * (0.5 + azar.NextDouble() * 0.2) / 1000) * 1000;
            OfertaSenor = senores[azar.Next(senores.Length)];
            return true;
        }

        // Después de vender, a veces llega de la nada un bro con cash
        public bool TalVezLlegaBro()
        {
            if (cliente == null || azar.Next(100) >= Temporada.ProbabilidadBro[tema])
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
                // El cliente se acuerda de cómo le fue
                if (cliente.Retrato < 8)
                {
                    ultimaCompra[cliente.Retrato] = auto.Modelo;
                    recuerdo[cliente.Retrato] = auto.Estado == Auto.CHOCADO ? 2 : 1;
                }
                recibido = j.CerrarVenta(indice, cliente, precio, out string detalle);
                respuesta += detalle;
                yaVendio = true;
                return true;
            }

            intentos++;
            if (intentos >= IntentosMaximos)
            {
                respuesta = "¡Ya me cansé de regatear! Me voy a otro lado.";
                j.CambiarReputacion(-4);
                yaVendio = true;      // el cliente se fue: ya no se puede vender este turno
            }
            return false;
        }

        // Si el jugador pasa sin vender y tiene vendedor, el empleado atiende al cliente
        private string AtiendeElEmpleado()
        {
            Concesionario j = JugadorActual;
            if (!j.TieneVendedor || yaVendio || cliente == null || j.NumAutos == 0) return null;

            for (int i = 0; i < j.NumAutos; i++)
            {
                Auto auto = j.ObtenerAuto(i);
                if (!cliente.AceptaCualquiera && auto.Tipo != cliente.TipoBuscado) continue;
                double pedido = j.PrecioVenta(auto);
                string nombre = auto.Nombre;
                if (OfrecerPrecio(i, pedido, out _, out double contra, out double recibido) ||
                    (contra >= auto.Costo * 1.05 && OfrecerPrecio(i, contra, out _, out _, out recibido)))
                    return $"Mientras no estabas, tu vendedor le vendió el {nombre} a {cliente.Nombre} (+${recibido:N0}).";
                return null;
            }
            return null;
        }

        // Se salta a los que se rindieron. Cuando se da la vuelta empieza otro día.
        public void SiguienteTurno()
        {
            Concesionario anterior = JugadorActual;
            string delEmpleado = AtiendeElEmpleado();
            anterior.FinDeTurno();
            if (delEmpleado != null)
                anterior.Noticia = delEmpleado;      // lo ve en su siguiente turno
            do
            {
                turno = (turno + 1) % numJugadores;
                if (turno == 0)
                {
                    ronda++;
                    if (ronda % 2 == 1)
                        RegistrarPatrimonios((ronda - 1) / 2);     // terminó un día
                    if (!Terminada && ronda % 2 == 1)
                        IniciarDia();
                }
            } while (jugadores[turno].Rendido && !Terminada);

            if (!Terminada)
                NuevoCliente();
        }

        // Sortea el evento del día (45 % de probabilidad, sin repetir el de ayer).
        // Una vez por partida, en lugar de sortearlo, lo votan todos.
        private void IniciarDia()
        {
            int anterior = evento;
            evento = -1;
            diaNuevo = true;
            // Reto del día: cada día arranca desde la misma semilla para todos
            if (semilla != 0)
                azar = new Random(semilla * 7 + Dia);
            SurtirMercado();
            for (int i = 0; i < numJugadores; i++)
                estafasHoy[i] = 0;

            if (Dia == diaVotacion)
            {
                // tres opciones distintas al azar
                for (int k = 0; k < 3; k++)
                {
                    int e;
                    bool repetido;
                    do
                    {
                        e = azar.Next(NombresEvento.Length);
                        repetido = false;
                        for (int m = 0; m < k; m++)
                            if (opcionesVoto[m] == e) repetido = true;
                    } while (repetido);
                    opcionesVoto[k] = e;
                }
                votacionPendiente = true;
                return;          // el evento se aplica cuando terminen de votar
            }

            if (azar.Next(100) < 45)
            {
                do evento = azar.Next(NombresEvento.Length);
                while (evento == anterior);
            }
            AplicarEvento();
        }

        // ----- El mercado se surte cada mañana -----
        // Los autos que ya se vendieron a clientes (no están en ningún lote) regresan
        // al mercado como seminuevos, con otro color y a veces chocados.
        public int AutosNuevos { get; private set; }

        private bool EnAlgunLote(Auto auto)
        {
            for (int i = 0; i < numJugadores; i++)
                for (int k = 0; k < jugadores[i].NumAutos; k++)
                    if (jugadores[i].ObtenerAuto(k) == auto) return true;
            return false;
        }

        private void SurtirMercado()
        {
            AutosNuevos = 0;
            for (int i = 0; i < catalogo.Length; i++)
            {
                Auto auto = catalogo[i];
                if (auto.Disponible || EnAlgunLote(auto) || azar.Next(100) >= 70) continue;
                auto.Disponible = true;
                auto.Color = azar.Next(8);
                auto.Estado = azar.Next(4) == 0 ? Auto.CHOCADO : Auto.NORMAL;
                auto.Costo = 0;
                AutosNuevos++;
            }
        }

        // Cuenta los votos (arreglo con cuántos votos tuvo cada opción) y aplica el ganador
        public int ResolverVotacion(int[] votos)
        {
            int ganador = 0;
            for (int k = 1; k < 3; k++)
                if (votos[k] > votos[ganador] || (votos[k] == votos[ganador] && azar.Next(2) == 0))
                    ganador = k;
            evento = opcionesVoto[ganador];
            votacionPendiente = false;
            AplicarEvento();
            if (!Terminada) NuevoCliente();
            return evento;
        }

        private void AplicarEvento()
        {
            Concesionario lider = Ranking()[0];
            for (int i = 0; i < numJugadores; i++)
            {
                Concesionario j = jugadores[i];
                if (j.Rendido) continue;
                j.NuevoDia();
                if (evento == NAVIDAD)
                    j.Dinero += 30000;
                if (evento == TERREMOTO && j.NumAutos > 0)
                {
                    // Al que va ganando se le va el más caro (búsqueda del máximo)
                    int cual = azar.Next(j.NumAutos);
                    if (j == lider)
                        for (int k = 0; k < j.NumAutos; k++)
                            if (j.ObtenerAuto(k).ValorActual() > j.ObtenerAuto(cual).ValorActual()) cual = k;
                    Auto perdido = j.QuitarAuto(cual);
                    j.Noticia = j == lider ? $"¡Justo cuando ibas ganando! El terremoto se tragó tu {perdido.Nombre}."
                                           : $"El terremoto se tragó tu {perdido.Nombre}.";
                }
                if (evento == ZOMBIES)
                    j.Noticia = j.PerderPorZombies(azar);
                ReglasDeHistoria(j);
            }

            autoSubasta = null;
            if (evento == SUBASTA)
            {
                int c = azar.Next(clasicos.GetLength(0));
                autoSubasta = new Auto("Clásico", clasicos[c, 0], int.Parse(clasicos[c, 1]), 180000 + azar.Next(12) * 10000, azar.Next(3), azar.Next(8));
                autoSubasta.Estado = Auto.TUNEADO;
            }
        }

        // Semana del SAT: multa si tu reputación está baja y confisca el auto del coyote
        private void ReglasDeHistoria(Concesionario j)
        {
            if (historia == null || historia.Semana != 3 || j.EsBot || historia.Mordida) return;
            if (j.Reputacion < 40)
            {
                double multa = Math.Min(40000, j.Dinero);
                j.Dinero -= multa;
                j.Noticia = $"El inspector del SAT te multó con ${multa:N0} por tu mala reputación.";
            }
            if (historia.Coyote && Dia == 3)
            {
                double multa = Math.Min(80000, j.Dinero);
                j.Dinero -= multa;
                j.Noticia = $"¡El SAT encontró el auto robado del coyote! Multa de ${multa:N0}.";
            }
        }

        // Subasta en secreto: gana la oferta más alta (búsqueda del máximo)
        public Concesionario ResolverSubasta(double[] ofertas, out double pago)
        {
            int ganador = -1;
            pago = 0;
            for (int i = 0; i < numJugadores; i++)
                if (!jugadores[i].Rendido && ofertas[i] > 0 && ofertas[i] <= jugadores[i].Dinero &&
                    (ganador == -1 || ofertas[i] > ofertas[ganador]))
                    ganador = i;
            if (ganador == -1 || jugadores[ganador].NumAutos == Concesionario.MAX_AUTOS)
            {
                autoSubasta = null;
                return null;
            }
            pago = ofertas[ganador];
            jugadores[ganador].CompraDirecta(autoSubasta, pago);
            jugadores[ganador].GanoSubasta();
            autoSubasta = null;
            return jugadores[ganador];
        }

        // Lo que ofrece un bot en la subasta
        public double OfertaBot(Concesionario bot)
        {
            double[] factor = { 0, 0.55, 0.75, 0.9, 1.1 };
            double oferta = autoSubasta.ValorActual() * factor[bot.NivelBot] * (0.9 + azar.NextDouble() * 0.2);
            oferta = Math.Min(oferta, bot.Dinero - 20000);
            return Math.Max(0, Math.Round(oferta / 5000) * 5000);
        }

        // ----- Estafas -----
        // Se llama cuando alguien usa una Estafa. "perdida" = cuánto bajó el patrimonio del rival.
        public void RegistrarEstafa(Concesionario usuario, Concesionario rival, PowerUp carta, double perdida)
        {
            usuario.CambiarReputacion(-8);
            usuario.UsoEstafa();
            int r = IndiceDe(rival);
            if (r >= 0)
            {
                estafasHoy[r]++;
                if (estafasHoy[r] >= 2)
                    rival.Noticia = $"¡COMBO DE ESTAFAS! Ya van {estafasHoy[r]} hoy.";
            }
            if (perdida > MejorEstafaPerdida)
            {
                MejorEstafaPerdida = perdida;
                MejorEstafaUsuario = usuario;
                MejorEstafaRival = rival;
                MejorEstafaCarta = carta.Nombre;
                MejorEstafaTexto = $"{usuario.Jugador} le lanzó \"{carta.Nombre}\" a {rival.Jugador} el día {Dia} y le bajó ${perdida:N0}.";
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
            Partida nueva = new Partida(zona, historia, semilla);
            nueva.dificultad = dificultad;
            nueva.tema = tema;
            for (int i = 0; i < numJugadores; i++)
            {
                Concesionario j = new Concesionario(jugadores[i].Jugador, jugadores[i].Vendedor, jugadores[i].NivelBot);
                j.Perfil = jugadores[i].Perfil;
                j.Cara = jugadores[i].Cara;
                nueva.AgregarJugador(j);
            }
            return nueva;
        }

        // Para cargar una partida guardada
        public void RestaurarExtras(int dificultad, int tema, int diasRegistrados)
        {
            this.dificultad = dificultad;
            this.tema = tema;
            this.diasRegistrados = diasRegistrados;
        }

        public void Restaurar(int ronda, int turno, int evento, int diaVotacion, bool votacionPendiente, int[] opciones)
        {
            this.ronda = ronda;
            this.turno = turno;
            this.evento = evento;
            this.diaVotacion = diaVotacion;
            this.votacionPendiente = votacionPendiente;
            opcionesVoto = opciones;
            rondaDraft = Concesionario.MAX_CARTAS;     // el draft ya se hizo
            diaNuevo = false;
        }

        public int DiaVotacion => diaVotacion;
        public int[] Visitas => visitas;
        public string[] UltimaCompra => ultimaCompra;
        public int[] Recuerdo => recuerdo;

        // ----- Draft de cartas -----

        public void PrepararDraft()
        {
            baraja.Barajar(semilla != 0 ? azar : null);

            // Dinero según la dificultad (solo a las personas) y el día 0 de la gráfica
            for (int i = 0; i < numJugadores; i++)
                if (!jugadores[i].EsBot)
                    jugadores[i].Dinero += dineroDificultad[dificultad];
            RegistrarPatrimonios(0);

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
            return c.Rendido ? double.MinValue : c.Patrimonio();
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
