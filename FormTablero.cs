namespace PrograProyectoFinal
{
    // Pantalla 3: tu lote a la izquierda, el cliente al centro y el ranking a la derecha
    public partial class FormTablero : FormPixel
    {
        private const int FILAS = 3;               // autos que caben a la vista en MI LOTE

        private Partida partida;
        private int seleccionado = -1;             // índice del auto escogido en el lote
        private int pagina = 0;
        private string dialogo;                    // lo que dice el cliente en el globo
        private bool esperando = false;            // ya vendió o juega el bot: los botones no se usan
        private int pasoBot = 0;                   // en qué va el bot en su turno
        private BotonPixel btnOficina;

        // Texto que sube flotando cuando entra dinero ("+$127,000")
        private string flotante = "";
        private int flotanteY = 0;

        internal FormTablero(Partida partida)
        {
            InitializeComponent();
            this.partida = partida;
            btnMenu.Colores(Paleta.LilaClaro, Paleta.Crema2);
            btnMercado.Colores(Paleta.Oro, Paleta.Oro2);
            btnVender.Colores(Paleta.Verde, Paleta.Verde2);
            btnUsarCarta.Colores(Paleta.Rosa, Paleta.Rosa2);
            btnPasar.Colores(Paleta.Crema, Paleta.Crema2);
            btnAnterior.Colores(Paleta.LilaClaro, Paleta.Crema2);
            btnSiguiente.Colores(Paleta.LilaClaro, Paleta.Crema2);
            btnOficina = Boton("OFICINA", 874, 12, 116, 62, Paleta.Agua, Paleta.Agua2, btnOficina_Click, 9);

            partida.NuevoCliente();
            NuevoTurno();
            // Los avisos se enseñan cuando ya se ve la ventana
            Shown += (s, e) => Avisos();
        }

        private void NuevoTurno()
        {
            seleccionado = -1;
            pagina = 0;
            esperando = false;
            if (partida.JugadorActual.PierdeTurno)
                dialogo = "¡¿Por qué tan caro?! Mejor me voy a otro lado...";
            else if (partida.ClienteActual == null)
                dialogo = "...";
            else
                dialogo = partida.ClienteActual.Saludo;
            Actualizar();
        }

        // Al empezar un turno: el evento del día y lo que le hicieron al jugador
        private void Avisos()
        {
            Concesionario jugador = partida.JugadorActual;

            // Una vez por partida: todos votan el evento del día
            if (partida.VotacionPendiente)
                using (FormVotacion votacion = new FormVotacion(partida))
                    votacion.ShowDialog(this);

            bool empezoDia = partida.DiaNuevo;
            if (partida.DiaNuevo)
            {
                partida.DiaNuevo = false;
                int ev = partida.Evento;
                string titulo = $"DÍA {partida.Dia} · {partida.NombreDia}";
                if (ev == -1)
                {
                    Recursos.Sonar("turno");
                    string texto = partida.Ronda == 1 ? "¡Abrimos el lote! Cada día tiene 2 turnos: mañana y tarde. Compren autos en el MERCADO y a vender."
                                                      : "Día normal. Ni terremotos ni zombies: puro vender.";
                    FormAviso.Mostrar(this, titulo, texto, Paleta.Agua);
                }
                else
                {
                    Recursos.Sonar("evento");
                    if (ev == Partida.TERREMOTO) Sacudir();
                    FormAviso.Mostrar(this, $"{titulo}: {Partida.NombresEvento[ev]}", Partida.DescripcionesEvento[ev], ColorEvento(ev));
                }
            }
            else
                Recursos.Sonar("turno");

            // Subasta del clásico (todos ofrecen en secreto)
            if (partida.AutoSubasta != null)
                using (FormSubasta subasta = new FormSubasta(partida))
                {
                    subasta.ShowDialog(this);
                    dialogo = subasta.Resultado;
                }

            if (empezoDia && !jugador.EsBot)
                DecisionDeHistoria(jugador);
            Guardado.Guardar(partida);

            if (jugador.EsBot)
            {
                jugador.Noticia = "";
                EmpezarBot();
                return;
            }

            if (jugador.Noticia != "")
            {
                Recursos.Sonar("estafa");
                Sacudir();
                FormAviso.Mostrar(this, $"¡AGUAS, {jugador.Jugador.ToUpper()}!", jugador.Noticia, Paleta.Rosa);
                jugador.Noticia = "";
            }
            // Decisión: un señor te ofrece su coche barato
            if (!jugador.PierdeTurno && partida.HayOferta())
            {
                Auto oferta = partida.OfertaAuto;
                Recursos.Sonar("bro");
                string texto = $"{partida.OfertaSenor} te ofrece su {oferta.Nombre} ({oferta.NombreTipo}) en {Pixel.Dinero(partida.OfertaPrecio)}. " +
                               $"Vale {Pixel.Dinero(oferta.Precio)}. ¿Se lo compras?";
                if (FormAviso.Preguntar(this, "¡TE VENDEN UN COCHE!", texto, Paleta.Oro, Recursos.Autos[oferta.Tipo, oferta.Color]))
                {
                    string error = jugador.CompraDirecta(oferta, partida.OfertaPrecio);
                    if (error != null)
                        FormAviso.Mostrar(this, "NO SE PUDO", error, Paleta.Rosa);
                    else
                    {
                        Recursos.Sonar("compra");
                        dialogo = $"¡Buena compra! El {oferta.Nombre} ya está en tu lote.";
                    }
                }
            }

            if (jugador.PierdeTurno)
                FormAviso.Mostrar(this, "PIERDES EL TURNO", "Alguien te lanzó \"¿Por qué tan caro?\" y tu cliente se fue. Solo puedes PASAR TURNO.",
                                  Paleta.Rosa, Recursos.Clientes[partida.ClienteActual?.Retrato ?? 0]);
            Actualizar();
        }

        private static Color ColorEvento(int evento)
        {
            Color[] colores = { Paleta.Rosa, Paleta.Agua, Paleta.Oro, Paleta.Verde, Paleta.Rosa, Paleta.Crema2, Paleta.Verde, Paleta.Lila };
            return colores[evento];
        }

        // Modo historia: en ciertos días hay que decidir, y eso cambia el final
        private void DecisionDeHistoria(Concesionario jugador)
        {
            Historia h = partida.Historia;
            if (h == null) return;
            int decision = h.Decision(partida.Dia);
            if (decision == Historia.COYOTE)
            {
                Auto robado = new Auto("Volkswagen", "Jetta", 2018, 140000, 0, partida.Azar.Next(8));
                if (FormAviso.Preguntar(this, "EL COYOTE", "Un tipo te ofrece un Jetta 2018 \"sin papeles\" en $25,000. Vale $140,000. ¿Se lo compras?",
                                        Paleta.Gris, Recursos.Autos[0, robado.Color]))
                {
                    if (jugador.CompraDirecta(robado, 25000) == null)
                    {
                        h.Coyote = true;
                        jugador.CambiarReputacion(-10);
                        dialogo = "Ya tienes el Jetta del coyote. Ojalá nadie pregunte...";
                    }
                }
            }
            else if (decision == Historia.INFLUENCER)
            {
                if (FormAviso.Preguntar(this, "UN INFLUENCER", "@AutosChidosMx quiere hacerte promoción por $30,000. Los clientes traerían 20 % más toda la semana.", Paleta.Rosa)
                    && jugador.Dinero >= 30000)
                {
                    jugador.Dinero -= 30000;
                    h.Influencer = true;
                    jugador.CambiarReputacion(5);
                    dialogo = "¡Saliste en TikTok! Los clientes vienen con más ganas.";
                }
            }
            else if (decision == Historia.MORDIDA)
            {
                string texto = "El inspector del SAT se acerca: \"Con $50,000 hago como que no vi nada\". ¿Le das?" +
                               (h.Coyote ? " (Ojo: todavía tienes el Jetta del coyote.)" : "");
                if (FormAviso.Preguntar(this, "EL INSPECTOR", texto, Paleta.Crema2, Recursos.Clientes[7]) && jugador.Dinero >= 50000)
                {
                    jugador.Dinero -= 50000;
                    h.Mordida = true;
                    jugador.CambiarReputacion(-15);
                    dialogo = "El inspector se fue silbando.";
                }
            }
            if (decision != -1)
                h.Guardar();
        }

        private void btnOficina_Click(object sender, EventArgs e)
        {
            using (FormOficina oficina = new FormOficina(partida.JugadorActual))
                oficina.ShowDialog(this);
            Actualizar();
        }

        private void Actualizar()
        {
            Concesionario jugador = partida.JugadorActual;
            int paginas = Paginas();
            if (pagina >= paginas) pagina = paginas - 1;
            btnAnterior.Visible = jugador.NumAutos > FILAS;
            btnSiguiente.Visible = jugador.NumAutos > FILAS;
            btnAnterior.Enabled = pagina > 0;
            btnSiguiente.Enabled = pagina < paginas - 1;

            bool castigado = jugador.PierdeTurno;
            bool libre = !esperando && !castigado;
            btnVender.Enabled = libre && !partida.YaVendio && partida.ClienteActual != null;
            btnMercado.Enabled = libre;
            btnOficina.Enabled = libre;
            btnUsarCarta.Enabled = libre && jugador.NumCartas > 0;
            btnPasar.Enabled = !esperando;
            btnPasar.Text = jugador.EsBot ? "JUGANDO EL BOT..." : esperando ? "SIGUIENTE TURNO..." : "PASAR TURNO";

            // TALLER aparece cuando escoges un auto del lote
            int fila = seleccionado - pagina * FILAS;
            btnRematar.Visible = libre && seleccionado != -1 && fila >= 0 && fila < FILAS;
            btnRematar.Location = new Point(236, 498);
            btnRematar.Text = "TALLER";
            Invalidate();
        }

        private int Paginas()
        {
            int n = partida.JugadorActual.NumAutos;
            return Math.Max(1, (n + FILAS - 1) / FILAS);
        }

        private Rectangle Fila(int k)
        {
            return new Rectangle(40, 164 + k * 116, 340, 100);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            Concesionario jugador = partida.JugadorActual;
            Cliente cliente = partida.ClienteActual;

            // ----- barra de arriba -----
            Pixel.R(g, 0, 0, Pixel.ANCHO, 84, Paleta.Negro);
            Pixel.R(g, 0, 0, Pixel.ANCHO, 78, Paleta.Morado2);
            Pixel.Texto(g, $"DÍA {partida.Dia}/{Partida.DIAS}", 128, 28, 12, Paleta.Agua);
            Pixel.Texto(g, $"{partida.NombreDia} · {partida.Momento}", 128, 56, 9, Paleta.Tenue);
            Pixel.R(g, 464, 8, 64, 64, Paleta.Morado3);
            Pixel.Sprite(g, Recursos.Cara(jugador), 464, 8, 4);
            Pixel.Texto(g, $"Turno de {jugador.Jugador.ToUpper()}", 544, 30, 13, Paleta.Crema);
            // reputación del lote
            Pixel.Texto(g, "REP", 544, 62, 8, Paleta.Tenue);
            Pixel.R(g, 580, 56, 144, 12, Paleta.Negro);
            Color colorRep = jugador.Reputacion >= 60 ? Paleta.Verde : jugador.Reputacion >= 30 ? Paleta.Oro : Paleta.Rosa;
            Pixel.R(g, 582, 58, 140 * jugador.Reputacion / 100, 8, colorRep);
            Pixel.Texto(g, $"{jugador.Reputacion}", 732, 62, 8, colorRep);
            if (jugador.DeudaBanco + jugador.DeudaCompadre > 0)
                Pixel.Texto(g, $"debes {Pixel.DineroCorto(jugador.DeudaBanco + jugador.DeudaCompadre)}", 772, 62, 8, Paleta.Rosa);
            Pixel.Sprite(g, Recursos.Moneda, 1002, 22, 6);
            Pixel.Texto(g, Pixel.Dinero(jugador.Dinero), 1046, 40, 15, Paleta.Oro);

            // ----- mi lote -----
            Pixel.Panel(g, 20, 104, 380, 512, Paleta.Crema);
            Pixel.Texto(g, "MI LOTE", 44, 136, 13, Paleta.Morado2);
            if (jugador.NumAutos > FILAS)
                Pixel.Texto(g, $"{pagina + 1}/{Paginas()}", 290, 136, 12, Paleta.Gris, StringAlignment.Far);

            if (jugador.NumAutos == 0)
            {
                Pixel.Parrafo(g, "Tu lote está vacío. Compra autos en el MERCADO para tener qué vender.",
                              new Rectangle(44, 176, 330, 200), 12, Paleta.Gris);
            }
            for (int k = 0; k < FILAS; k++)
            {
                int indice = pagina * FILAS + k;
                if (indice >= jugador.NumAutos) break;
                Auto auto = jugador.ObtenerAuto(indice);
                Rectangle f = Fila(k);

                if (indice == seleccionado)
                    Pixel.R(g, f.X - 6, f.Y - 6, f.Width + 12, f.Height + 12, Paleta.Oro);
                Pixel.R(g, f.X, f.Y, f.Width, f.Height, Paleta.LilaClaro);
                Pixel.Sprite(g, Recursos.Autos[auto.Tipo, auto.Color], f.X + 8, f.Y + 18, 4);
                Pixel.Texto(g, auto.Nombre, f.X + 148, f.Y + 34, 12, Paleta.Noche);
                Pixel.Texto(g, Pixel.Dinero(jugador.PrecioVenta(auto)), f.X + 148, f.Y + 70, 12, Paleta.Verde2);
                if (auto.Estado != Auto.NORMAL)
                    Pixel.Texto(g, Auto.NombresEstado[auto.Estado], f.Right - 8, f.Y + 14, 7,
                                auto.Estado == Auto.CHOCADO ? Paleta.Rosa2 : Paleta.Agua2, StringAlignment.Far);
            }

            Pixel.Texto(g, "MIS CARTAS", 44, 524, 12, Paleta.Morado2);
            // Boca abajo para que los rivales no las vean; se ven al darle USAR CARTA
            for (int c = 0; c < jugador.NumCartas; c++)
                Pixel.CartaBocaAbajo(g, 44 + c * 88, 544, 80, 60);
            if (jugador.NumCartas == 0)
                Pixel.Texto(g, "sin cartas", 44, 574, 11, Paleta.Gris);

            // ----- showroom con el cliente -----
            Pixel.R(g, 420, 104, 500, 380, Paleta.Showroom);
            Escenario.Dibujar(g, partida.Zona, new Rectangle(420, 112, 500, 300));
            Pixel.R(g, 420, 412, 500, 72, Paleta.Showroom);
            Pixel.R(g, 420, 104, 500, 8, Paleta.Negro);
            Pixel.Texto(g, $"LOTE EN {Partida.Zonas[partida.Zona]}", 432, 474, 8, Paleta.Tenue);
            for (int x = 428; x < 920; x += 48)
                Pixel.R(g, x, 420, 24, 4, Paleta.Morado3);

            // Evento del día
            if (partida.Evento != -1)
            {
                Pixel.R(g, 428, 118, 484, 44, ColorEvento(partida.Evento));
                Pixel.Texto(g, $"HOY: {Partida.NombresEvento[partida.Evento]}", 670, 140, 11, Paleta.Noche, StringAlignment.Center);
            }

            Pixel.R(g, 472, 176, 192, 240, Paleta.Negro);
            Pixel.R(g, 480, 184, 176, 224, Paleta.Morado3);
            if (cliente != null && !jugador.PierdeTurno)
            {
                Pixel.Sprite(g, Recursos.Clientes[cliente.Retrato], 480, 200, 11);
                Pixel.Texto(g, cliente.Nombre, 568, 452, 13, Paleta.Crema, StringAlignment.Center);
            }
            else
            {
                Pixel.Texto(g, "?", 568, 296, 30, Paleta.Morado2, StringAlignment.Center);
                Pixel.Texto(g, "(nadie)", 568, 452, 13, Paleta.Tenue, StringAlignment.Center);
            }

            Pixel.Globo(g, 688, 176, 216, 192);
            DibujarDialogo(g, new Rectangle(712, 194, 180, 166));

            // ----- ranking -----
            Pixel.Panel(g, 940, 104, 320, 512, Paleta.Crema);
            Pixel.Texto(g, "RANKING", 964, 136, 13, Paleta.Morado2);
            Concesionario[] ranking = partida.Ranking();
            for (int i = 0; i < ranking.Length; i++)
            {
                int y = 168 + i * 104;
                Concesionario r = ranking[i];
                Pixel.R(g, 960, y, 280, 88, r.Rendido ? Paleta.Crema2 : i == 0 ? Paleta.OroClaro : Paleta.LilaClaro);
                Pixel.Texto(g, $"{i + 1}", 980, y + 44, 15, Paleta.Morado2);
                Pixel.Sprite(g, Recursos.Cara(r), 1008, y + 12, 4);
                Pixel.Texto(g, r.Jugador, 1084, y + 28, 12, r.Rendido ? Paleta.Gris : Paleta.Noche);
                if (r.Rendido)
                    Pixel.Texto(g, "se rindió", 1084, y + 62, 11, Paleta.Rosa2);
                else
                    Pixel.Texto(g, Pixel.DineroCorto(r.Patrimonio()), 1084, y + 62, 12, Paleta.Verde2);
                if (r.EsBot)
                {
                    string[] etiqueta = { "", "BOTARDO", "MEDIO", "PRO", "DIOS" };
                    Pixel.Texto(g, etiqueta[r.NivelBot], 1232, y + 76, 7, r.NivelBot == 4 ? Paleta.Oro2 : Paleta.Lila, StringAlignment.Far);
                }
            }
            DibujarFlotante(g);
        }

        private void DibujarFlotante(Graphics g)
        {
            if (flotante == "") return;
            Color color = flotante.StartsWith("+") ? Paleta.Verde : Paleta.Rosa;
            Pixel.Texto(g, flotante, 1052, 100 + flotanteY + 3, 17, Paleta.Negro);
            Pixel.Texto(g, flotante, 1048, 100 + flotanteY, 17, color);
        }

        private void MostrarFlotante(double cantidad)
        {
            flotante = cantidad >= 0 ? $"+{Pixel.Dinero(cantidad)}" : $"-{Pixel.Dinero(-cantidad)}";
            flotanteY = 40;
            timerFlotante.Start();
        }

        private void timerFlotante_Tick(object sender, EventArgs e)
        {
            flotanteY -= 2;
            if (flotanteY < -24)
            {
                flotante = "";
                timerFlotante.Stop();
            }
            Invalidate(new Rectangle(1000, 60, 280, 100));
        }

        // Si el texto no cabe en el globo, se hace más chiquita la letra
        private void DibujarDialogo(Graphics g, Rectangle area)
        {
            float[] tamanos = { 13, 12, 11, 10 };
            foreach (float tam in tamanos)
            {
                SizeF medida = g.MeasureString(dialogo, Recursos.Letra(tam), area.Width);
                if (medida.Height <= area.Height || tam == 10)
                {
                    Pixel.Parrafo(g, dialogo, area, tam, Paleta.Noche);
                    return;
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (esperando) return;
            for (int k = 0; k < FILAS; k++)
            {
                int indice = pagina * FILAS + k;
                if (indice < partida.JugadorActual.NumAutos && Fila(k).Contains(e.Location))
                {
                    seleccionado = indice == seleccionado ? -1 : indice;
                    Invalidate();
                }
            }
            // Clic en las cartas chiquitas = USAR CARTA
            if (btnUsarCarta.Enabled && new Rectangle(44, 544, 264, 60).Contains(e.Location))
                btnUsarCarta_Click(this, EventArgs.Empty);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            bool sobreFila = false;
            for (int k = 0; k < FILAS; k++)
                if (pagina * FILAS + k < partida.JugadorActual.NumAutos && Fila(k).Contains(e.Location))
                    sobreFila = true;
            Cursor = sobreFila ? Cursors.Hand : Cursors.Default;
        }

        private void btnVender_Click(object sender, EventArgs e)
        {
            if (seleccionado == -1)
            {
                dialogo = "¿Qué me enseñas? Escoge un auto de tu lote.";
                Recursos.Sonar("error");
                Actualizar();
                return;
            }

            bool vendido;
            using (FormNegociar negociar = new FormNegociar(partida, seleccionado))
            {
                vendido = negociar.ShowDialog(this) == DialogResult.OK;
                dialogo = negociar.Resultado == "" ? dialogo : negociar.Resultado;
                if (vendido)
                    MostrarFlotante(negociar.Recibido);
            }
            if (!vendido)
            {
                Actualizar();
                return;
            }
            seleccionado = -1;
            DespuesDeVender();
        }

        // Después de vender: a veces llega El Bro; si no, se pasa el turno solo
        private void DespuesDeVender()
        {
            if (partida.TalVezLlegaBro())
            {
                Actualizar();
                Recursos.Sonar("bro");
                if (!partida.JugadorActual.EsBot)
                    FormAviso.Mostrar(this, "¡LLEGÓ UN BRO!", "De la nada se metió El Bro con cash. Te compra el auto que sea y le puedes pedir hasta 50 % más de lo que te costó.",
                                      Paleta.Lila, Recursos.Clientes[8]);
                dialogo = partida.ClienteActual.Saludo;
            }
            else if (Recursos.PasarAlVender && !partida.JugadorActual.EsBot)
            {
                esperando = true;
                timerPasar.Start();
            }
            Actualizar();
        }

        private void btnRematar_Click(object sender, EventArgs e)
        {
            using (FormTaller taller = new FormTaller(partida.JugadorActual, seleccionado))
            {
                if (taller.ShowDialog(this) == DialogResult.OK)
                {
                    dialogo = taller.Resultado;
                    if (taller.Rematado > 0)
                    {
                        MostrarFlotante(taller.Rematado);
                        seleccionado = -1;
                    }
                }
            }
            Actualizar();
        }

        // ----- turno del bot: 4 pasos con pausa para que se vea lo que hace -----
        private void EmpezarBot()
        {
            esperando = true;
            pasoBot = 0;
            Actualizar();
            timerBot.Start();
        }

        private void timerBot_Tick(object sender, EventArgs e)
        {
            Concesionario bot = partida.JugadorActual;
            string quien = bot.Jugador;
            string hizo = null;

            switch (pasoBot)
            {
                case 0:
                    if (bot.PierdeTurno)
                    {
                        dialogo = $"{quien} perdió el turno: su cliente se fue.";
                        pasoBot = 3;
                    }
                    else if (Bot.SeDistrajo(partida))
                    {
                        dialogo = $"{quien} se quedó viendo TikTok y no hizo nada.";
                        pasoBot = 3;
                    }
                    else
                        hizo = Bot.UsarCarta(partida);
                    if (hizo != null) Recursos.Sonar(hizo.Contains("\"") ? "carta" : "turno");
                    break;
                case 1:
                    if (partida.HayOferta() && Bot.AceptaOferta(partida) &&
                        bot.CompraDirecta(partida.OfertaAuto, partida.OfertaPrecio) == null)
                        hizo = $"le compró su {partida.OfertaAuto.Nombre} a {partida.OfertaSenor}. ";
                    string compras = Bot.Comprar(partida);
                    if (compras != null)
                    {
                        hizo = (hizo ?? "") + compras;
                        Recursos.Sonar("compra");
                    }
                    break;
                case 2:
                    hizo = Bot.Vender(partida, out double recibido);
                    if (recibido > 0)
                    {
                        MostrarFlotante(recibido);
                        Recursos.Sonar("venta");
                        if (partida.TalVezLlegaBro())
                        {
                            Recursos.Sonar("bro");
                            string otra = Bot.Vender(partida, out double otroRecibido);
                            if (otroRecibido > 0) hizo += $" ¡Llegó El Bro y también le {otra}";
                        }
                    }
                    break;
                default:
                    timerBot.Stop();
                    PasarTurno();
                    return;
            }

            if (hizo != null)
                dialogo = $"{quien} {hizo}";
            pasoBot++;
            Actualizar();
        }

        private void timerPasar_Tick(object sender, EventArgs e)
        {
            timerPasar.Stop();
            PasarTurno();
        }

        private void btnUsarCarta_Click(object sender, EventArgs e)
        {
            using (FormUsarCarta cartas = new FormUsarCarta(partida))
            {
                if (cartas.ShowDialog(this) == DialogResult.OK)
                    dialogo = cartas.Resultado;
            }
            Actualizar();
        }

        private void btnPasar_Click(object sender, EventArgs e)
        {
            PasarTurno();
        }

        private void PasarTurno()
        {
            timerPasar.Stop();
            timerBot.Stop();
            partida.SiguienteTurno();
            if (partida.Terminada)
            {
                Recursos.Sonar("victoria");
                IrA(new FormResultados(partida));
            }
            else
            {
                NuevoTurno();
                Avisos();
            }
        }

        private void btnMercado_Click(object sender, EventArgs e)
        {
            using (FormMercado mercado = new FormMercado(partida))
                mercado.ShowDialog(this);
            Actualizar();
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            pagina--;
            Actualizar();
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            pagina++;
            Actualizar();
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            AbrirMenu();
        }

        private void AbrirMenu()
        {
            bool estabaEsperando = timerPasar.Enabled;
            bool botJugando = timerBot.Enabled;
            timerPasar.Stop();
            timerBot.Stop();

            int eleccion;
            using (FormMenu menu = new FormMenu())
            {
                menu.ShowDialog(this);
                eleccion = menu.Eleccion;
            }

            Concesionario jugador = partida.JugadorActual;
            if (eleccion == FormMenu.REVANCHA &&
                FormAviso.Preguntar(this, "¿REVANCHA?", "Se empieza de cero con los mismos jugadores y vendedores. Esta partida se pierde.", Paleta.Oro))
            {
                IrA(new FormDraft(partida.Revancha()));
                return;
            }
            if (eleccion == FormMenu.RENDIRSE &&
                FormAviso.Preguntar(this, $"¿{jugador.Jugador.ToUpper()} SE RINDE?", "Te sales de la partida y quedas hasta abajo del ranking. Los demás siguen jugando.", Paleta.Rosa))
            {
                timerPasar.Stop();
                partida.Rendirse();
                if (partida.Terminada)
                {
                    Recursos.Sonar("victoria");
                    IrA(new FormResultados(partida));
                }
                else
                {
                    NuevoTurno();
                    Avisos();
                }
                return;
            }
            if (eleccion == FormMenu.TITULO &&
                FormAviso.Preguntar(this, "¿SALIR AL TÍTULO?", "Se pierde la partida.", Paleta.Crema2))
            {
                VolverAlTitulo();
                return;
            }

            if (estabaEsperando)
                timerPasar.Start();
            if (botJugando)
                timerBot.Start();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Escape)
                AbrirMenu();
        }
    }
}
