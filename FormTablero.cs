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

        // ----- Reloj del cliente: se impacienta y, si tardas mucho, se va enojado -----
        private System.Windows.Forms.Timer timerTiempo = new System.Windows.Forms.Timer { Interval = 100 };
        private int tiempoMs, tiempoTotalMs;       // 0 = sin reloj (turno del bot o sin cliente)
        private int humor = 0;                     // 0 tranquilo, 1 impaciente, 2 enojado
        private int vibrar = 0;                    // cuenta para que tiemble el globo cuando esta enojado
        private bool sePaso = false;               // ya se fue enojado

        // ----- Animaciones del cliente: entra deslizandose y su frase se escribe letra por letra -----
        private System.Windows.Forms.Timer timerAnim = new System.Windows.Forms.Timer { Interval = 30 };
        private int entrada = 0;                   // pixeles que le faltan para llegar a su lugar
        private string ultimoDialogo = "";
        private int letras = 0;                    // cuantas letras de la frase se ven ya

        // ----- Turno del bot: dura unos 5 s (4 pasos) con su linea de tiempo; RAPIDO lo acelera -----
        private const int BOT_PASO_MS = 1250, BOT_PASO_RAPIDO_MS = 350, BOT_PASOS = 4;
        private static bool botRapido = false;     // se queda para toda la partida
        private System.Diagnostics.Stopwatch swPaso = new System.Diagnostics.Stopwatch();
        private BotonPixel btnRapido;

        // ----- Pausa: congela el reloj del cliente, el turno del bot y las animaciones -----
        private bool pausado = false;
        private BotonPixel btnPausa;
        private bool botEstabaJugando, pasarEstaba;
        private System.Collections.Generic.List<Control> bloqueados = new System.Collections.Generic.List<Control>();
        private string ventaTitulo = "¡VENDISTE!";

        // ----- Efecto de venta: el auto sale manejando, sello de VENDIDO y lluvia de monedas -----
        private const int VENTA_VIDA = 60;         // ticks de 30 ms
        private int ventaVida = 0;
        private Auto autoVendido = null;
        private double ventaDinero = 0;

        // Letrero grande de "TURNO DE ..." que aparece al empezar cada turno
        private string letrero = "";
        private int letreroVida = 0;
        private System.Windows.Forms.Timer timerLetrero = new System.Windows.Forms.Timer { Interval = 40 };

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
            Boton("?", 800, 12, 62, 62, Paleta.LilaClaro, Paleta.Crema2, (s, e) => MostrarTutorial(), 15);
            btnPausa = Boton("PAUSA", 940, 636, 150, 60, Paleta.Oro, Paleta.Oro2, (s, e) => AlternarPausa(), 11);
            btnRapido = Boton(botRapido ? "NORMAL" : "RÁPIDO", 1100, 636, 160, 60, Paleta.Agua, Paleta.Agua2, (s, e) => AlternarRapido(), 11);
            btnRapido.Visible = false;
            timerLetrero.Tick += (s, e) =>
            {
                letreroVida--;
                if (letreroVida <= 0) timerLetrero.Stop();
                Repintar(new Rectangle(0, 250, Pixel.ANCHO, 180));
            };

            timerTiempo.Tick += (s, e) => TickTiempo();
            timerAnim.Tick += (s, e) => TickAnim();
            FormClosed += (s, e) => { timerTiempo.Stop(); timerAnim.Stop(); };
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
            ReiniciarTiempo();
            entrada = partida.ClienteActual != null && !partida.JugadorActual.PierdeTurno ? 260 : 0;
            if (entrada > 0) { timerAnim.Start(); Recursos.Sonar("menu"); }
            Actualizar();
        }

        private void TickAnim()
        {
            if (pausado) return;
            if (entrada > 0) entrada = Math.Max(0, entrada - (entrada > 80 ? 22 : 9));   // llega rapido y frena al final
            if (letras < ultimoDialogo.Length) letras += 2;
            if (ventaVida > 0) ventaVida--;
            if (entrada == 0 && letras >= ultimoDialogo.Length && !timerBot.Enabled && ventaVida == 0) timerAnim.Stop();
            Repintar(new Rectangle(420, 104, 500, 380));
        }

        // Repinta una zona dada en medidas del diseno (1280 x 720). Con pantalla completa el dibujo va escalado
        // (x2 a 2K) y Invalidate(rect) usa pixeles reales: sin esta conversion solo se repintaba una esquina.
        private void Repintar(Rectangle zonaDiseno)
        {
            Point a = Aqui(zonaDiseno.X, zonaDiseno.Y);
            Point b2 = Aqui(zonaDiseno.Right, zonaDiseno.Bottom);
            Invalidate(new Rectangle(a.X, a.Y, b2.X - a.X, b2.Y - a.Y));
        }

        private void IniciarVenta(Auto auto, double dinero, string titulo = "¡VENDISTE!")
        {
            ventaTitulo = titulo;
            autoVendido = auto;
            ventaDinero = dinero;
            ventaVida = VENTA_VIDA;
            timerAnim.Start();
        }

        private void DibujarVenta(Graphics g)
        {
            if (ventaVida <= 0) return;
            int t = VENTA_VIDA - ventaVida;                       // 0 .. 59
            Region recorte = g.Clip;
            g.SetClip(new Rectangle(420, 104, 500, 380));

            // el auto vendido sale manejando por el piso, de izquierda a derecha, acelerando
            if (autoVendido != null)
            {
                double avance = Math.Min(1.0, t / 44.0);
                int x = 440 + (int)(avance * avance * 520);
                Pixel.Sprite(g, Recursos.Autos[autoVendido.Tipo, autoVendido.Color], x, 330, 4);
                if (t < 40) Pixel.R(g, x - 30, 388, 24, 6, Paleta.Tenue);       // humito
            }

            // lluvia de monedas desde el cliente
            for (int i = 0; i < 16; i++)
            {
                double ang = i * 2 * Math.PI / 16 + 0.3;
                double vx = Math.Cos(ang) * (3 + i % 4), vy = Math.Sin(ang) * (3 + i % 3) - 7;
                int mx = 568 + (int)(vx * t), my = 300 + (int)(vy * t + 0.35 * t * t);
                if (my < 470) Pixel.R(g, mx, my, 10, 10, i % 3 == 0 ? Paleta.Oro2 : Paleta.Oro);
            }

            g.Clip = recorte;
        }

        // Banner grande y al frente: cruza toda la pantalla cuando se vende
        private void DibujarBannerVenta(Graphics g)
        {
            if (ventaVida <= 0) return;
            int t = VENTA_VIDA - ventaVida;
            int x = 0;
            if (t < 8) x = -Pixel.ANCHO + t * Pixel.ANCHO / 8;
            else if (ventaVida < 8) x = (8 - ventaVida) * Pixel.ANCHO / 8;
            float tam = 56 + Math.Max(0, 10 - t) * 2 + (float)(Math.Sin(t / 3.0) * 2);
            Pixel.R(g, x, 262, Pixel.ANCHO, 176, Paleta.Negro);
            Pixel.R(g, x, 270, Pixel.ANCHO, 160, ((t / 4) & 1) == 0 ? Paleta.Verde : Paleta.Verde2);
            Pixel.SombraTexto(g, ventaTitulo, x + 640, 336, tam, Paleta.Crema, Paleta.Morado3);
            string detalle = ventaDinero > 0 ? "+" + Pixel.Dinero(ventaDinero) : "";
            if (autoVendido != null) detalle = autoVendido.Nombre + "   " + detalle;
            Pixel.Texto(g, detalle, x + 640, 404, 20, Paleta.Oro, StringAlignment.Center);
        }

        private void AlternarPausa()
        {
            pausado = !pausado;
            if (pausado)
            {
                botEstabaJugando = timerBot.Enabled;
                pasarEstaba = timerPasar.Enabled;
                timerBot.Stop();
                timerPasar.Stop();
                bloqueados.Clear();
                foreach (Control c in Controls)
                    if (c != btnPausa && c.Enabled) { c.Enabled = false; bloqueados.Add(c); }
                btnPausa.Text = "REANUDAR";
            }
            else
            {
                foreach (Control c in bloqueados) c.Enabled = true;
                bloqueados.Clear();
                btnPausa.Text = "PAUSA";
                if (botEstabaJugando) { timerBot.Start(); swPaso.Restart(); }
                if (pasarEstaba) timerPasar.Start();
            }
            Recursos.Sonar("menu");
            Invalidate();
        }

        private void AlternarRapido()
        {
            botRapido = !botRapido;
            btnRapido.Text = botRapido ? "NORMAL" : "RÁPIDO";
            if (timerBot.Enabled) timerBot.Interval = botRapido ? BOT_PASO_RAPIDO_MS : BOT_PASO_MS;
            Recursos.Sonar("menu");
            Invalidate();
        }

        private void ReiniciarTiempo()
        {
            timerTiempo.Stop();
            humor = 0; vibrar = 0; sePaso = false;
            Concesionario j = partida.JugadorActual;
            if (j.EsBot || j.PierdeTurno || partida.ClienteActual == null) { tiempoTotalMs = 0; return; }
            tiempoTotalMs = partida.TiempoTurno * 1000;
            tiempoMs = tiempoTotalMs;
            timerTiempo.Start();
        }

        // Cada 0.1 s. El reloj solo corre con esta ventana al frente: se pausa solo con el menu, el mercado o el regateo.
        private void TickTiempo()
        {
            if (pausado) return;
            if (tiempoTotalMs == 0 || esperando || partida.YaVendio || partida.ClienteActual == null) return;
            if (Form.ActiveForm != this) return;
            tiempoMs -= timerTiempo.Interval;
            Cliente cliente = partida.ClienteActual;
            double resta = (double)tiempoMs / tiempoTotalMs;
            int fase = resta <= 0 ? 3 : resta < 0.2 ? 2 : resta < 0.5 ? 1 : 0;
            if (fase >= 1 && humor < 1) { humor = 1; dialogo = cliente.FraseImpaciente(partida.Azar); Recursos.Sonar("turno"); }
            if (fase >= 2 && humor < 2) { humor = 2; dialogo = cliente.FraseImpaciente(partida.Azar); Recursos.Sonar("error"); }
            if (humor == 2) vibrar++;
            if (fase == 3) { SeFueEnojado(); return; }
            Repintar(new Rectangle(420, 104, 500, 380));
        }

        // Se acabo la paciencia: el cliente se va enojado, baja la reputacion y pasa el turno
        private void SeFueEnojado()
        {
            timerTiempo.Stop();
            sePaso = true;
            esperando = true;
            dialogo = partida.ClienteActual.FraseEnojado(partida.Azar) + "  (-8 de reputación)";
            partida.JugadorActual.CambiarReputacion(-8);
            Recursos.Sonar("error");
            Actualizar();
            timerPasar.Start();
        }

        // Mientras juega el bot: una linea de tiempo que se llena en ~5 s con los 4 pasos de su turno
        private void DibujarTiempoBot(Graphics g)
        {
            int intervalo = Math.Max(1, timerBot.Interval);
            double avance = (pasoBot + Math.Min(1.0, swPaso.ElapsedMilliseconds / (double)intervalo)) / BOT_PASOS;
            avance = Math.Max(0, Math.Min(1, avance));
            int x0 = 432, y0 = 426, ancho = 340, alto = 11;
            Pixel.R(g, x0 - 3, y0 - 3, ancho + 6, alto + 6, Paleta.Negro);
            Pixel.R(g, x0, y0, ancho, alto, Paleta.Morado3);
            Pixel.R(g, x0, y0, (int)(ancho * avance), alto, Paleta.Agua);
            string[] pasos = { "CARTA", "COMPRA", "VENDE", "FIN" };
            for (int i = 0; i < BOT_PASOS; i++)
            {
                int cx = x0 + ancho * i / BOT_PASOS + ancho / (BOT_PASOS * 2);
                bool hecho = i < pasoBot || (i == pasoBot && avance * BOT_PASOS - i > 0.4);
                Pixel.Texto(g, pasos[i], cx, y0 + alto / 2, 7, hecho ? Paleta.Noche : Paleta.Crema, StringAlignment.Center);
            }
            Pixel.Texto(g, "TURNO DE " + partida.JugadorActual.Jugador.ToUpper(), 912, y0 + alto / 2, 9, Paleta.Agua, StringAlignment.Far);
        }

        // La linea de tiempo: tres tramos (tranquilo, impaciente, enojado) y una marca que avanza
        private void DibujarTiempo(Graphics g)
        {
            if (partida.JugadorActual.EsBot && timerBot.Enabled)
            {
                DibujarTiempoBot(g);
                return;
            }
            if (tiempoTotalMs == 0 || partida.JugadorActual.EsBot || partida.JugadorActual.PierdeTurno) return;
            if (esperando && !sePaso) return;   // ya vendio: la barra de paciencia se esconde
            double usado = sePaso ? 1 : 1 - (double)tiempoMs / tiempoTotalMs;
            usado = Math.Max(0, Math.Min(1, usado));
            int x0 = 432, y0 = 426, ancho = 340, alto = 11;
            int t1 = ancho * 50 / 100, t2 = ancho * 30 / 100, t3 = ancho - t1 - t2;
            Pixel.R(g, x0 - 3, y0 - 3, ancho + 6, alto + 6, Paleta.Negro);
            Pixel.R(g, x0, y0, t1, alto, Paleta.Verde2);
            Pixel.R(g, x0 + t1, y0, t2, alto, Paleta.Oro2);
            Pixel.R(g, x0 + t1 + t2, y0, t3, alto, Paleta.Rosa2);
            // lo ya gastado se oscurece y una marca clara va avanzando
            int gastado = (int)(ancho * usado);
            Pixel.R(g, x0, y0, gastado, alto, Color.FromArgb(150, 20, 10, 30));
            Pixel.Texto(g, "TRANQUILO", x0 + t1 / 2, y0 + alto / 2, 7, Paleta.Crema, StringAlignment.Center);
            Pixel.Texto(g, "IMPACIENTE", x0 + t1 + t2 / 2, y0 + alto / 2, 7, Paleta.Noche, StringAlignment.Center);
            Pixel.Texto(g, "ENOJADO", x0 + t1 + t2 + t3 / 2, y0 + alto / 2, 7, Paleta.Crema, StringAlignment.Center);
            Pixel.R(g, x0 + gastado - 3, y0 - 6, 6, alto + 12, Paleta.Crema);
            Pixel.R(g, x0 + gastado - 5, y0 - 8, 10, 4, Paleta.Crema);
            int seg = (int)Math.Ceiling(Math.Max(0, tiempoMs) / 1000.0);
            Color cs = humor == 2 ? ((vibrar / 3 & 1) == 0 ? Paleta.Rosa : Paleta.Crema) : humor == 1 ? Paleta.Oro : Paleta.Verde;
            Pixel.Texto(g, sePaso ? "¡SE FUE!" : $"{seg} s", 912, y0 + alto / 2, sePaso ? 10 : 12, cs, StringAlignment.Far);
        }

        // Al empezar un turno: el evento del día y lo que le hicieron al jugador
        private void Avisos()
        {
            Concesionario jugador = partida.JugadorActual;
            CambioDeTurno(jugador);

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
                    if (partida.AutosNuevos > 0) texto += $" Llegaron {partida.AutosNuevos} autos al mercado.";
                    FormAviso.Mostrar(this, titulo, texto, Paleta.Agua);
                }
                else
                {
                    Recursos.Sonar("evento");
                    if (ev == Partida.TERREMOTO) Sacudir();
                    string extra = partida.AutosNuevos > 0 ? $" Además llegaron {partida.AutosNuevos} autos al mercado." : "";
                    FormAviso.Mostrar(this, $"{titulo}: {Partida.NombresEvento[ev]}", Partida.DescripcionesEvento[ev] + extra, ColorEvento(ev));
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
                        if (oferta.TieneFalla)
                            FormAviso.Mostrar(this, "¡POR ALGO ESTABA BARATO!", $"Al revisar el {oferta.Nombre} salió: {oferta.NombreFalla}. " +
                                              $"Ahora vale {Pixel.Dinero(oferta.ValorActual())}." +
                                              (oferta.FallaTieneArreglo ? " Arréglalo en el TALLER." : " No tiene arreglo."), Paleta.Rosa,
                                              Recursos.Autos[oferta.Tipo, oferta.Color]);
                    }
                }
            }

            if (jugador.PierdeTurno)
                FormAviso.Mostrar(this, "PIERDES EL TURNO", "Alguien te lanzó \"¿Por qué tan caro?\" y tu cliente se fue. Solo puedes PASAR TURNO.",
                                  Paleta.Rosa, Recursos.Clientes[partida.ClienteActual?.Retrato ?? 0]);

            // La primera vez que una persona juega, se le enseña el tutorial
            if (jugador.Perfil != null && !jugador.Perfil.TutorialVisto)
            {
                MostrarTutorial();
                jugador.Perfil.TutorialVisto = true;
                Perfiles.Guardar();
            }
            Actualizar();
        }

        // Tutorial: foto de la pantalla oscurecida y se ilumina cada parte con su globo
        private void MostrarTutorial()
        {
            letreroVida = 0;
            Actualizar();
            Update();
            Bitmap foto = new Bitmap(Pixel.ANCHO, Pixel.ALTO);
            if (Pixel.Esc == 1f)
                DrawToBitmap(foto, new Rectangle(0, 0, Pixel.ANCHO, Pixel.ALTO));
            else
            {
                // Pantalla completa: la foto se toma del tamaño real y se baja a 1280 x 720
                using (Bitmap grande = new Bitmap(ClientSize.Width, ClientSize.Height))
                using (Graphics gf = Graphics.FromImage(foto))
                {
                    DrawToBitmap(grande, new Rectangle(0, 0, ClientSize.Width, ClientSize.Height));
                    gf.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    gf.DrawImage(grande, 0, 0, Pixel.ANCHO, Pixel.ALTO);
                }
            }
            Rectangle[] zonas =
            {
                new Rectangle(0, 0, 800, 84), new Rectangle(20, 104, 380, 404), Diseno(btnMercado.Bounds),
                new Rectangle(420, 104, 500, 380), Diseno(btnVender.Bounds), new Rectangle(20, 508, 380, 108),
                new Rectangle(940, 104, 320, 512), Diseno(btnOficina.Bounds), Diseno(btnPasar.Bounds)
            };
            string[] titulos = { "EL DÍA Y TU LANA", "MI LOTE", "MERCADO", "EL CLIENTE", "VENDER Y REGATEAR", "TUS CARTAS", "RANKING", "OFICINA", "PASAR TURNO" };
            string[] textos =
            {
                $"Son {Partida.DIAS} días y cada día tiene 2 turnos: mañana y tarde. Arriba ves de quién es el turno, tu reputación (REP) y tu dinero.",
                "Aquí están tus autos. Dale clic a uno para escogerlo. Con uno escogido aparece TALLER: reparar, tunear o rematar.",
                "Compra autos para tu lote. Los CHOCADOS salen a mitad de precio, pero hay que repararlos o bajan tu reputación.",
                "Cada turno llega un cliente: te dice qué tipo de auto busca y cuánto trae. Si lo quiere de otro color, pintarlo cuesta $10,000.",
                $"Escoge un auto y dale VENDER. Tú pones el precio y el cliente regatea: tienes {partida.IntentosMaximos} intentos antes de que se vaya.",
                "Tus ABILICARDS salen boca abajo para que nadie las vea. REGATEO te ayuda, ESTAFA le pega a un rival y ASEGURADORA te protege.",
                "Gana el que termine con más patrimonio: dinero + lo que valen tus autos − lo que debes.",
                "En la OFICINA pides préstamos (banco o compadre) y contratas vendedor o mecánico.",
                "Cuando acabes, PASA EL TURNO. Con MENÚ o Esc pausas. El botón ? vuelve a abrir este tutorial."
            };
            using (FormTutorial tutorial = new FormTutorial(foto, zonas, titulos, textos))
            {
                tutorial.StartPosition = FormStartPosition.Manual;
                tutorial.Location = Location;
                tutorial.ShowDialog(this);
            }
            foto.Dispose();
        }

        // Cuántas personas (no bots) siguen jugando
        private int Personas()
        {
            int n = 0;
            for (int i = 0; i < partida.NumJugadores; i++)
                if (!partida.ObtenerJugador(i).EsBot && !partida.ObtenerJugador(i).Rendido) n++;
            return n;
        }

        // Letrero "TURNO DE ..." en cada turno. Si juegan varias personas en la misma compu,
        // además sale un aviso para que se pasen la compu sin ver lo del otro.
        private void CambioDeTurno(Concesionario jugador)
        {
            letrero = jugador.EsBot ? $"TURNO DE {jugador.Jugador.ToUpper()}" : $"¡TE TOCA, {jugador.Jugador.ToUpper()}!";
            letreroVida = 30;
            timerLetrero.Start();
            Invalidate();
            if (!jugador.EsBot && Personas() > 1)
            {
                Recursos.Sonar("turno");
                FormAviso.Mostrar(this, $"TURNO DE {jugador.Jugador.ToUpper()}",
                                  $"Pásale la compu a {jugador.Jugador}. Los demás no se asomen a sus cartas. Dale OK cuando esté listo.",
                                  Paleta.Agua, Recursos.Cara(jugador));
            }
        }

        private void DibujarLetrero(Graphics g)
        {
            if (letreroVida <= 0 || letrero == "") return;
            // entra deslizándose desde la izquierda y se queda
            int avance = Math.Min(30 - letreroVida, 6);
            int x = -Pixel.ANCHO + avance * Pixel.ANCHO / 6;
            Pixel.R(g, x, 294, Pixel.ANCHO, 92, Paleta.Negro);
            Pixel.R(g, x, 300, Pixel.ANCHO, 80, Paleta.Rosa);
            Pixel.SombraTexto(g, letrero, x + 640, 338, 44, Paleta.Oro, Paleta.Rosa2);
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
            btnRapido.Visible = esperando && jugador.EsBot;
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
            btnRematar.Location = Aqui(236, 498);
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
                Pixel.Texto(g, $"debes {Pixel.DineroCorto(jugador.DeudaBanco + jugador.DeudaCompadre)}", 1046, 68, 8, Paleta.Rosa);
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
                Pixel.TextoAjustado(g, auto.Nombre, f.X + 148, f.Y + 34, 12, Paleta.Noche, StringAlignment.Near, f.Width - 156);
                Pixel.Texto(g, "costó " + Pixel.Dinero(auto.Costo), f.X + 148, f.Y + 53, 8, Paleta.Gris);
                Pixel.Texto(g, Pixel.Dinero(jugador.PrecioVenta(auto)), f.X + 148, f.Y + 70, 12, Paleta.Verde2);
                if (auto.Estado != Auto.NORMAL)
                    Pixel.Texto(g, Auto.NombresEstado[auto.Estado], f.Right - 8, f.Y + 14, 7,
                                auto.Estado == Auto.CHOCADO ? Paleta.Rosa2 : Paleta.Agua2, StringAlignment.Far);
                if (auto.TieneFalla)
                    Pixel.Texto(g, "FALLA", f.Right - 8, auto.Estado != Auto.NORMAL ? f.Y + 32 : f.Y + 14, 7, Paleta.Rosa2, StringAlignment.Far);
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
            DibujarTiempo(g);

            // Evento del día
            if (partida.Evento != -1)
            {
                Pixel.R(g, 428, 118, 484, 44, ColorEvento(partida.Evento));
                Pixel.Texto(g, $"HOY: {Partida.NombresEvento[partida.Evento]}", 670, 140, 11, Paleta.Noche, StringAlignment.Center);
            }

            int temblor = (humor == 2 && !esperando) || sePaso ? ((vibrar & 1) == 0 ? -4 : 4) : 0;
            int desliza = entrada;   // viene desde la izquierda
            Color fondoCliente = humor == 2 && ((vibrar / 3 & 1) == 0) ? Paleta.Rosa2 : humor == 1 ? Paleta.Morado2 : Paleta.Morado3;
            Pixel.R(g, 472 + temblor, 176, 192, 240, Paleta.Negro);
            Pixel.R(g, 480 + temblor, 184, 176, 224, fondoCliente);
            if (cliente != null && !jugador.PierdeTurno)
            {
                Region recorte = g.Clip;
                if (desliza > 0) g.SetClip(new Rectangle(420, 104, 500, 380));   // no se sale del showroom al entrar
                Pixel.Sprite(g, Recursos.Clientes[cliente.Retrato], 480 + temblor - desliza, 200 + (desliza > 0 ? (desliza / 22 & 1) * 6 : 0), 11);
                g.Clip = recorte;
                if (humor == 1) Pixel.Texto(g, "...", 640, 206, 18, Paleta.Oro, StringAlignment.Far);
                if (humor == 2) Pixel.Texto(g, "!!", 640, 206, 22, Paleta.Rosa, StringAlignment.Far);
                Pixel.Texto(g, cliente.Nombre, 568, 452, 13, Paleta.Crema, StringAlignment.Center);
            }
            else
            {
                Pixel.Texto(g, "?", 568, 296, 30, Paleta.Morado2, StringAlignment.Center);
                Pixel.Texto(g, "(nadie)", 568, 452, 13, Paleta.Tenue, StringAlignment.Center);
            }

            Pixel.Globo(g, 684 + temblor, 170, 228, 238);
            DibujarDialogo(g, new Rectangle(704 + temblor, 186, 196, 214));

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
            DibujarVenta(g);
            DibujarFlotante(g);
            DibujarLetrero(g);
            DibujarBannerVenta(g);
            if (pausado)
            {
                Pixel.R(g, 0, 0, Pixel.ANCHO, Pixel.ALTO, Color.FromArgb(190, 12, 4, 30));
                Pixel.SombraTexto(g, "PAUSA", 640, 300, 76, Paleta.Oro, Paleta.Rosa2);
                Pixel.Texto(g, "El reloj del cliente y el turno del bot estan detenidos", 640, 384, 14, Paleta.Crema, StringAlignment.Center);
                Pixel.Texto(g, "REANUDAR o la tecla P para seguir", 640, 414, 12, Paleta.Agua, StringAlignment.Center);
            }
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
            Repintar(new Rectangle(1000, 60, 280, 100));
        }

        // Si el texto no cabe en el globo, se hace más chiquita la letra
        private void DibujarDialogo(Graphics g, Rectangle area)
        {
            // frase nueva: se vuelve a escribir desde el principio
            if (dialogo != ultimoDialogo)
            {
                ultimoDialogo = dialogo;
                letras = 0;
                if (!timerAnim.Enabled) timerAnim.Start();
            }
            string visible = letras >= dialogo.Length ? dialogo : dialogo.Substring(0, Math.Max(0, letras));
            // el tamano se calcula con la frase completa (para que no cambie mientras se escribe)
            float tam = Pixel.TamQueCabe(g, dialogo, area, 13);
            Pixel.Parrafo(g, visible, area, tam, Paleta.Noche);
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
                    Actualizar();       // así aparece el botón TALLER en cuanto escoges el auto
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
            Auto aVender = partida.JugadorActual.ObtenerAuto(seleccionado);
            using (FormNegociar negociar = new FormNegociar(partida, seleccionado))
            {
                vendido = negociar.ShowDialog(this) == DialogResult.OK;
                dialogo = negociar.Resultado == "" ? dialogo : negociar.Resultado;
                if (vendido)
                {
                    MostrarFlotante(negociar.Recibido);
                    IniciarVenta(aVender, negociar.Recibido);
                }
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
            timerBot.Interval = botRapido ? BOT_PASO_RAPIDO_MS : BOT_PASO_MS;
            Actualizar();
            timerBot.Start();
            swPaso.Restart();
            timerAnim.Start();
        }

        private void timerBot_Tick(object sender, EventArgs e)
        {
            Concesionario bot = partida.JugadorActual;
            string quien = bot.Jugador;
            string hizo = null;
            swPaso.Restart();

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
                        IniciarVenta(null, recibido, quien.ToUpper() + " VENDIO");
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
            timerTiempo.Stop();
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
            if (e.KeyCode == Keys.P)
                AlternarPausa();
            else if (e.KeyCode == Keys.Escape && !pausado)
                AbrirMenu();
        }
    }
}
