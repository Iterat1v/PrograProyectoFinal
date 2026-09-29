namespace PrograProyectoFinal
{
    // La "inteligencia" del bot: decide qué comprar, a qué precio vender
    // y a quién lanzarle una Estafa. Cada método regresa lo que hizo (o null).
    internal static class Bot
    {
        public static readonly string[] Nombres = { "Bot Rulo", "Bot Chayo", "Bot Neto", "Bot Pepa" };
        private const double RESERVA = 30000;     // dinero que siempre se guarda

        // Draft: escoge una casilla libre al azar
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

        public static string UsarCarta(Partida partida)
        {
            Concesionario bot = partida.JugadorActual;
            for (int i = 0; i < bot.NumCartas; i++)
            {
                PowerUp carta = bot.ObtenerCarta(i);
                bool conviene = false;
                Concesionario rival = null;

                if (carta is Regateo)
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
                else if (carta is Estafa && partida.Azar.Next(100) < 45)
                {
                    rival = Lider(partida, bot);
                    conviene = rival != null;
                }

                if (!conviene) continue;
                string resultado = carta.Aplicar(bot, rival, out bool usada);
                if (usada)
                {
                    bot.QuitarCarta(i);
                    return $"usó \"{carta.Nombre}\". {resultado}";
                }
            }
            return null;
        }

        public static bool AceptaOferta(Partida partida)
        {
            Concesionario bot = partida.JugadorActual;
            return partida.OfertaPrecio <= partida.OfertaAuto.Precio * 0.65 &&
                   bot.Dinero - partida.OfertaPrecio > RESERVA && bot.NumAutos < 8;
        }

        // Compra hasta 2 autos: primero del tipo que busca el cliente, luego de los que no tenga
        public static string Comprar(Partida partida)
        {
            Concesionario bot = partida.JugadorActual;
            string compras = "";
            for (int k = 0; k < 2 && bot.NumAutos < 5; k++)
            {
                int tipo = partida.ClienteActual != null && !partida.ClienteActual.AceptaCualquiera && k == 0
                    ? partida.ClienteActual.TipoBuscado
                    : TipoQueFalta(bot, partida.Azar);

                // Búsqueda lineal del más barato de ese tipo que le alcance
                Auto mejor = null;
                foreach (Auto auto in partida.Catalogo)
                {
                    double precio = bot.PrecioCompra(auto, partida.FactorMercado);
                    if (auto.Disponible && auto.Tipo == tipo && precio <= bot.Dinero - RESERVA &&
                        (mejor == null || auto.Precio < mejor.Precio))
                        mejor = auto;
                }
                if (mejor != null && bot.Comprar(mejor, partida.FactorMercado) == null)
                    compras += compras == "" ? mejor.Nombre : $" y {mejor.Nombre}";
            }
            return compras == "" ? null : $"compró {compras}.";
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

        // Regatea: pide el precio de lista y, si le contraofertan algo que deje ganancia, lo acepta
        public static string Vender(Partida partida, out double recibido)
        {
            recibido = 0;
            Concesionario bot = partida.JugadorActual;
            Cliente cliente = partida.ClienteActual;
            if (cliente == null || partida.YaVendio || bot.NumAutos == 0)
                return null;

            // Búsqueda lineal del auto que busca el cliente
            int indice = -1;
            for (int i = 0; i < bot.NumAutos && indice == -1; i++)
                if (cliente.AceptaCualquiera || bot.VentaSegura || bot.ObtenerAuto(i).Tipo == cliente.TipoBuscado)
                    indice = i;
            if (indice == -1)
                return "no tenía lo que buscaba el cliente.";

            Auto auto = bot.ObtenerAuto(indice);
            double pedido = cliente.AceptaCualquiera || bot.VentaSegura ? auto.Precio * 1.5 : bot.PrecioVenta(auto);
            string nombre = auto.Nombre;
            for (int intento = 0; intento < Partida.MAX_INTENTOS; intento++)
            {
                if (partida.OfrecerPrecio(indice, pedido, out string respuesta, out double contra, out recibido))
                    return $"vendió el {nombre} en ${pedido:N0}.";
                if (partida.YaVendio || contra < auto.Precio * 1.05)
                    return $"no se puso de acuerdo con el cliente por el {nombre}.";
                pedido = contra;
            }
            return null;
        }
    }
}
