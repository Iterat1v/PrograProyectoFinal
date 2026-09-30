using System.Globalization;

namespace PrograProyectoFinal
{
    // Autoguardado de la partida en curso: %AppData%\Concesionario\partida.txt
    // Se escribe al empezar cada turno y se borra cuando termina la partida.
    // Cada línea empieza con una letra que dice qué es: P partida, V visitas,
    // C catálogo, J jugador, E contadores e historial del jugador, A auto, K carta.
    internal static class Guardado
    {
        private const char S = '|';
        private static CultureInfo inv = CultureInfo.InvariantCulture;

        public static string Ruta => Path.Combine(Path.GetDirectoryName(Perfiles.Ruta), "partida.txt");
        public static bool Existe => File.Exists(Ruta);

        public static void Borrar()
        {
            try { if (Existe) File.Delete(Ruta); } catch (IOException) { }
        }

        private static string N(double d) => d.ToString(inv);
        private static string B(bool b) => b ? "1" : "0";
        private static string Limpio(string t) => (t ?? "").Replace("|", "/");

        public static void Guardar(Partida p)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Ruta));
                using (StreamWriter w = new StreamWriter(Ruta))
                {
                    int[] op = p.OpcionesVoto;
                    w.WriteLine(string.Join(S, "P", p.Zona, p.Historia?.Nombre ?? "", p.Ronda, p.Turno, p.Evento,
                                            p.DiaVotacion, B(p.VotacionPendiente), $"{op[0]},{op[1]},{op[2]}",
                                            p.Semilla, p.Dificultad, p.Tema, p.DiasRegistrados));
                    w.WriteLine(string.Join(S, "V", string.Join(",", p.Visitas),
                                            string.Join(",", Array.ConvertAll(p.UltimaCompra, u => Limpio(u).Replace(",", " "))),
                                            string.Join(",", p.Recuerdo)));
                    for (int i = 0; i < p.Catalogo.Length; i++)
                    {
                        Auto a = p.Catalogo[i];
                        w.WriteLine(string.Join(S, "C", i, a.Estado, a.Color, B(a.Disponible), N(a.Costo),
                                                Limpio(a.Marca), Limpio(a.Modelo), a.Anio, N(a.Precio), a.Tipo, a.Km, a.Falla, B(a.FallaConocida)));
                    }
                    for (int j = 0; j < p.NumJugadores; j++)
                    {
                        Concesionario c = p.ObtenerJugador(j);
                        w.WriteLine(string.Join(S, "J", Limpio(c.Jugador), c.Vendedor, c.NivelBot, N(c.Dinero), c.Reputacion,
                                                N(c.DeudaBanco), N(c.DeudaCompadre), c.DiasCompadre, B(c.TieneVendedor), B(c.TieneMecanico),
                                                c.Ventas, N(c.MejorVenta), Limpio(c.MejorVentaAuto), B(c.Rendido),
                                                N(c.Descuento), N(c.BonoVenta), B(c.VentaSegura), B(c.ErrorDeCuentas), B(c.Sobreprecio), B(c.PierdeTurno)));
                        w.WriteLine(string.Join(S, "E", j, c.ChocadosVendidos, c.TuneadosVendidos, c.VentasBro, c.VentasCaras,
                                                c.EstafasUsadas, c.SubastasGanadas, B(c.PidioPrestamo),
                                                string.Join(",", Array.ConvertAll(c.Historial, N))));
                        for (int k = 0; k < c.NumAutos; k++)
                        {
                            Auto a = c.ObtenerAuto(k);
                            w.WriteLine(string.Join(S, "A", j, a.IndiceCatalogo, Limpio(a.Marca), Limpio(a.Modelo), a.Anio, N(a.Precio),
                                                    a.Tipo, a.Color, a.Estado, N(a.Costo), a.Km, a.Falla, B(a.FallaConocida)));
                        }
                        for (int k = 0; k < c.NumCartas; k++)
                        {
                            PowerUp carta = c.ObtenerCarta(k);
                            w.WriteLine(string.Join(S, "K", j, carta.Tipo, carta.Efecto, Limpio(carta.Nombre), Limpio(carta.Descripcion)));
                        }
                    }
                }
            }
            catch (IOException)
            {
                // si no se pudo guardar, el juego sigue
            }
        }

        public static Partida Cargar()
        {
            try
            {
                string[] lineas = File.ReadAllLines(Ruta);
                Partida p = null;
                Concesionario[] jugadores = new Concesionario[Partida.MAX_JUGADORES];
                int nj = 0;

                foreach (string linea in lineas)
                {
                    string[] c = linea.Split(S);
                    switch (c[0])
                    {
                        case "P":
                            Historia h = c[2] == "" ? null : Historia.Cargar(c[2]);
                            int semilla = c.Length > 12 ? int.Parse(c[9]) : 0;
                            p = new Partida(int.Parse(c[1]), h, semilla);
                            string[] op = c[8].Split(',');
                            p.Restaurar(int.Parse(c[3]), int.Parse(c[4]), int.Parse(c[5]), int.Parse(c[6]), c[7] == "1",
                                        new[] { int.Parse(op[0]), int.Parse(op[1]), int.Parse(op[2]) });
                            if (c.Length > 12)
                                p.RestaurarExtras(int.Parse(c[10]), int.Parse(c[11]), int.Parse(c[12]));
                            break;
                        case "V":
                            string[] vis = c[1].Split(','), ult = c[2].Split(','), rec = c[3].Split(',');
                            for (int i = 0; i < p.Visitas.Length; i++)
                            {
                                p.Visitas[i] = int.Parse(vis[i]);
                                p.UltimaCompra[i] = ult[i] == "" ? null : ult[i];
                                p.Recuerdo[i] = int.Parse(rec[i]);
                            }
                            break;
                        case "C":
                            Auto a = p.Catalogo[int.Parse(c[1])];
                            if (c.Length >= 14)
                            {
                                a = new Auto(c[6], c[7], int.Parse(c[8]), double.Parse(c[9], inv), int.Parse(c[10]), int.Parse(c[3]));
                                a.Km = int.Parse(c[11]);
                                a.Falla = int.Parse(c[12]);
                                a.FallaConocida = c[13] == "1";
                                p.PonerEnCatalogo(int.Parse(c[1]), a);
                            }
                            a.Estado = int.Parse(c[2]);
                            a.Color = int.Parse(c[3]);
                            a.Disponible = c[4] == "1";
                            a.Costo = double.Parse(c[5], inv);
                            break;
                        case "J":
                            Concesionario j = new Concesionario(c[1], int.Parse(c[2]), int.Parse(c[3]));
                            j.Restaurar(double.Parse(c[4], inv), int.Parse(c[5]), double.Parse(c[6], inv), double.Parse(c[7], inv),
                                        int.Parse(c[8]), c[9] == "1", c[10] == "1", int.Parse(c[11]), double.Parse(c[12], inv), c[13], c[14] == "1");
                            double desc = double.Parse(c[15], inv);
                            if (desc > 0) j.DarDescuento(desc);
                            j.BonoVenta = double.Parse(c[16], inv);
                            j.VentaSegura = c[17] == "1";
                            j.ErrorDeCuentas = c[18] == "1";
                            j.Sobreprecio = c[19] == "1";
                            j.PierdeTurno = c[20] == "1";
                            if (!j.EsBot)
                            {
                                j.Perfil = Perfiles.Buscar(j.Jugador);
                                if (j.Perfil != null && j.Perfil.Personalizado) j.Cara = j.Perfil.Avatar.Dibujar();
                            }
                            else if (p.Historia != null)
                                j.Cara = Recursos.Clientes[Historia.RetratoJefe[p.Historia.Semana]];
                            jugadores[nj++] = j;
                            p.AgregarJugador(j);
                            break;
                        case "E":
                            double[] hist = Array.ConvertAll(c[9].Split(','), x => double.Parse(x, inv));
                            jugadores[int.Parse(c[1])].RestaurarContadores(int.Parse(c[2]), int.Parse(c[3]), int.Parse(c[4]),
                                int.Parse(c[5]), int.Parse(c[6]), int.Parse(c[7]), c[8] == "1", hist);
                            break;
                        case "A":
                            int dueno = int.Parse(c[1]);
                            Auto auto = new Auto(c[3], c[4], int.Parse(c[5]), double.Parse(c[6], inv), int.Parse(c[7]), int.Parse(c[8]));
                            if (c.Length >= 14)
                            {
                                auto.Km = int.Parse(c[11]);
                                auto.Falla = int.Parse(c[12]);
                                auto.FallaConocida = c[13] == "1";
                            }
                            auto.Estado = int.Parse(c[9]);
                            auto.Color = int.Parse(c[8]);
                            auto.Costo = double.Parse(c[10], inv);
                            jugadores[dueno].RecibirAuto(auto);
                            break;
                        case "K":
                            int d = int.Parse(c[1]), efecto = int.Parse(c[3]);
                            PowerUp carta = c[2] == "REGATEO" ? new Regateo(c[4], c[5], efecto)
                                          : c[2] == "ESTAFA" ? new Estafa(c[4], c[5], efecto)
                                          : (PowerUp)new Aseguradora(c[4], c[5], efecto);
                            jugadores[d].AgregarCarta(carta);
                            break;
                    }
                }
                if (p == null || p.NumJugadores < Partida.MIN_JUGADORES || p.Terminada) return null;
                p.NuevoCliente();
                return p;
            }
            catch (Exception)
            {
                return null;      // archivo dañado: no se puede continuar
            }
        }
    }
}
