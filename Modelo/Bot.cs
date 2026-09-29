namespace PrograProyectoFinal
{
    // La "inteligencia" del bot, por niveles:
    // 1 BOTARDO (se distrae y regatea mal), 2 MEDIO, 3 PRO y
    // 4 SSJ ULTRA INSTINTO FASE DIOS 1000: El Bro, que sabe cuánto trae cada cliente.
    internal static class Bot
    {
        public const int BOTARDO = 1, MEDIO = 2, PRO = 3, DIOS = 4;

        public static readonly string[] NombresNivel = { "", "BOTARDO", "MEDIO", "PRO", "SSJ DIOS 1000" };
        public static readonly string[][] Nombres =
        {
            new string[0],
            new[] { "Botardo", "Botardín", "Botardito", "Botardazo" },
            new[] { "Bot Chayo", "Bot Rulo", "Bot Pepa", "Bot Neto" },
            new[] { "Pro Tiburón", "Pro Galán", "Pro Max", "Pro Lobo" },
            new[] { "EL BRO", "EL BRO 2", "EL BRO 3", "EL BRO 4" }
        };

        private static double Reserva(int nivel) => nivel == BOTARDO ? 0 : 30000;

        // Draft: el Dios prefiere Estafas y Regateos; los demás, al azar
        public static void EscogerDraft(Partida partida, out int fila, out int columna)
        {
            do
            {
                fila = partida.Azar.Next(4);
                columna = partida.Azar.Next(4);
            } while (partida.DuenoDraft(fila, columna) != -1);
        }

        // El que va ganando (sin contar al bot) es a quien se le lanzan las Estafas
        private static Concesionario Lider(Partida partida, Concesionario bot)
        {
            Concesionario lider = null;
            for (int i = 0; i < partida.NumJugadores; i++)
            {
                Concesionario j = partida.ObtenerJugador(i);
                if (j == bot || j.Rendido) continue;
                if (lider == null || j.Patrimonio() > lider.Patrimonio())
                    lider = j;
            }
            return lider;
        }

        private static Concesionario RivalAlAzar(Partida partida, Concesionario bot)
        {
            Concesionario rival;
            do rival = partida.ObtenerJugador(partida.Azar.Next(partida.NumJugadores));
            while (rival == bot);
            return rival;
        }

        // El botardo a veces no hace nada
        public static bool SeDistrajo(Partida partida)
        {
            return partida.JugadorActual.NivelBot == BOTARDO && partida.Azar.Next(100) < 30;
        }

        public static string UsarCarta(Partida partida)
        {
            Concesionario bot = partida.JugadorActual;
            int nivel = bot.NivelBot;
            for (int i = 0; i < bot.NumCartas; i++)
            {
                PowerUp carta = bot.ObtenerCarta(i);
                bool conviene = false;
                Concesionario rival = null;

                if (nivel == BOTARDO)
                {
                    // usa lo que sea, cuando sea
                    conviene = !(carta is Aseguradora) && partida.Azar.Next(100) < 35;
                    if (carta.NecesitaRival) rival = RivalAlAzar(partida, bot);
                }
                else if (carta is Regateo)
                {
                    switch (carta.Efecto)
                    {
                        case Regateo.BONO_NAVIDENO:
                        case Regateo.EVASION_FISCAL:
                            conviene = true; break;
                        case Regateo.DESCUENTO_PROVEEDOR:
                        case Regateo.AMIGO_MERCADO:
                        case Regateo.REMATE:
                            conviene = bot.Descuento == 0 && bot.NumAutos < 6 && bot.Dinero > 60000; break;
                        case Regateo.ENCHILAME:
                        case Regateo.SEMINUEVO:
                            conviene = bot.NumAutos > 0 && bot.BonoVenta == 0; break;
                        default:  // ¡¡Toma mi dinero!!
                            conviene = partida.ClienteActual != null && !partida.YaVendio && bot.NumAutos > 0; break;
                    }
                }
                else if (carta is Estafa)
                {
                    int ganas = nivel == DIOS ? 100 : nivel == PRO ? 50 : 30;
                    rival = nivel == MEDIO ? RivalAlAzar(partida, bot) : Lider(partida, bot);
                    conviene = rival != null && partida.Azar.Next(100) < ganas;
                }

                if (!conviene) continue;
                double antes = rival?.Patrimonio() ?? 0;
                string resultado = carta.Aplicar(bot, rival, out bool usada);
                if (usada)
                {
                    bot.QuitarCarta(i);
                    if (carta is Estafa)
                        partida.RegistrarEstafa(bot, rival, carta, Math.Max(0, antes - rival.Patrimonio()));
                    return $"usó \"{carta.Nombre}\". {resultado}";
                }
            }
            return null;
        }

        public static bool AceptaOferta(Partida partida)
        {
            Concesionario bot = partida.JugadorActual;
            double tope = bot.NivelBot == DIOS ? 0.80 : bot.NivelBot == BOTARDO ? 1.2 : 0.65;
            return partida.OfertaPrecio <= partida.OfertaAuto.Precio * tope &&
                   bot.Dinero - partida.OfertaPrecio >= Reserva(bot.NivelBot) && bot.NumAutos < 8;
        }

        // Compra autos: el botardo al azar, los demás buscan el tipo que les falta
        public static string Comprar(Partida partida)
        {
            Concesionario bot = partida.JugadorActual;
            int nivel = bot.NivelBot;
            int cuantos = nivel == BOTARDO ? 1 : nivel == MEDIO ? 1 : 2;
            int lote = nivel == DIOS ? 6 : 5;
            string compras = "";

            for (int k = 0; k < cuantos && bot.NumAutos < lote; k++)
            {
                Auto elegido = null;
                if (nivel == BOTARDO)
                {
                    Auto auto = partida.Catalogo[partida.Azar.Next(partida.Catalogo.Length)];
                    if (auto.Disponible && bot.PrecioCompra(auto, partida.FactorMercado) <= bot.Dinero)
                        elegido = auto;
                }
                else
                {
                    int tipo = partida.ClienteActual != null && !partida.ClienteActual.AceptaCualquiera && k == 0
                        ? partida.ClienteActual.TipoBuscado
                        : TipoQueFalta(bot, partida.Azar);

                    // Búsqueda lineal del más barato de ese tipo que le alcance
                    foreach (Auto auto in partida.Catalogo)
                    {
                        double precio = bot.PrecioCompra(auto, partida.FactorMercado);
                        if (auto.Disponible && auto.Tipo == tipo && precio <= bot.Dinero - Reserva(nivel) &&
                            (elegido == null || auto.Precio < elegido.Precio))
                            elegido = auto;
                    }
                }
                if (elegido != null && bot.Comprar(elegido, partida.FactorMercado) == null)
                    compras += compras == "" ? elegido.Nombre : $" y {elegido.Nombre}";
            }
            // Menos el botardo, todos mandan al taller lo que compraron chocado
            string taller = "";
            if (nivel != BOTARDO)
                for (int i = 0; i < bot.NumAutos; i++)
                    if (bot.ObtenerAuto(i).Estado == Auto.CHOCADO && bot.Reparar(i) == null)
                        taller += $" Reparó el {bot.ObtenerAuto(i).Nombre}.";
            if (compras == "" && taller == "") return null;
            return (compras == "" ? "" : $"compró {compras}.") + taller;
        }

        private static int TipoQueFalta(Concesionario bot, Random azar)
        {
            int[] cuantos = new int[3];
            for (int i = 0; i < bot.NumAutos; i++)
                cuantos[bot.ObtenerAuto(i).Tipo]++;
            int menor = azar.Next(3);
            for (int t = 0; t < 3; t++)
                if (cuantos[t] < cuantos[menor])
                    menor = t;
            return menor;
        }

        public static string Vender(Partida partida, out double recibido)
        {
            recibido = 0;
            Concesionario bot = partida.JugadorActual;
            Cliente cliente = partida.ClienteActual;
            if (cliente == null || partida.YaVendio || bot.NumAutos == 0)
                return null;

            if (bot.NivelBot == DIOS)
                return VenderDios(partida, out recibido);

            // Búsqueda lineal del auto que busca el cliente (el botardo agarra el primero)
            int indice = -1;
            for (int i = 0; i < bot.NumAutos && indice == -1; i++)
                if (bot.NivelBot == BOTARDO || cliente.AceptaCualquiera || bot.VentaSegura || bot.ObtenerAuto(i).Tipo == cliente.TipoBuscado)
                    indice = i;
            if (indice == -1)
                return "no tenía lo que buscaba el cliente.";

            Auto auto = bot.ObtenerAuto(indice);
            string nombre = auto.Nombre;
            double pedido;
            if (cliente.AceptaCualquiera || bot.VentaSegura) pedido = auto.ValorActual() * 1.5;
            else if (bot.NivelBot == BOTARDO) pedido = bot.PrecioVenta(auto) * (1.0 + partida.Azar.NextDouble() * 0.8);
            else if (bot.NivelBot == PRO) pedido = bot.PrecioVenta(auto) * 1.25;    // el Pro pide alto y luego baja
            else pedido = bot.PrecioVenta(auto);
            pedido = Math.Round(pedido / 1000) * 1000;

            for (int intento = 0; intento < Partida.MAX_INTENTOS; intento++)
            {
                if (partida.OfrecerPrecio(indice, pedido, out string respuesta, out double contra, out recibido))
                    return $"vendió el {nombre} en ${pedido:N0}.";
                bool acepta = bot.NivelBot == BOTARDO ? partida.Azar.Next(2) == 0 : contra >= auto.Costo * 1.05;
                if (partida.YaVendio || !acepta)
                    return $"no se puso de acuerdo con el cliente por el {nombre}.";
                pedido = contra;
            }
            return null;
        }

        // Nivel Dios: sabe cuánto trae el cliente, escoge el auto que más deja y pide lo justo
        private static string VenderDios(Partida partida, out double recibido)
        {
            recibido = 0;
            Concesionario bot = partida.JugadorActual;
            Cliente cliente = partida.ClienteActual;
            bool lic = bot.Vendedor == Concesionario.LICENCIADO;

            int mejor = -1;
            double mejorGanancia = 0, mejorPrecio = 0;
            for (int i = 0; i < bot.NumAutos; i++)
            {
                Auto a = bot.ObtenerAuto(i);
                double limite = cliente.Limite(a, lic, bot.VentaSegura);
                double ganancia = limite - a.Costo;
                if (ganancia > mejorGanancia)
                {
                    mejor = i;
                    mejorGanancia = ganancia;
                    mejorPrecio = limite;
                }
            }
            if (mejor == -1)
                return "leyó al cliente y decidió no venderle nada. Frío y calculador.";

            string nombre = bot.ObtenerAuto(mejor).Nombre;
            if (partida.OfrecerPrecio(mejor, mejorPrecio, out string respuesta, out double contra, out recibido))
                return $"leyó la mente del cliente y le vendió el {nombre} en ${mejorPrecio:N0}.";
            return null;
        }
    }
}
