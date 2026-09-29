namespace PrograProyectoFinal
{
    // Mapa del modo historia: las 5 zonas, el jefe de la semana y tu progreso
    public class FormHistoria : FormPixel
    {
        private TextBox txtNombre;
        private BotonPixel btnVendedor, btnJugar, btnFinal;
        private Historia historia;
        private int vendedor = 0;
        private string aviso = "Escribe tu nombre y dale CARGAR.";

        public FormHistoria()
        {
            Titulo = "MODO HISTORIA · DE ECATEPEC A POLANCO";
            txtNombre = new TextBox
            {
                BorderStyle = BorderStyle.None, BackColor = Paleta.Crema, ForeColor = Paleta.Noche,
                Location = new Point(68, 110), Size = new Size(330, 44), MaxLength = 12, Font = Recursos.Letra(15)
            };
            txtNombre.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { Cargar(); e.SuppressKeyPress = true; } };
            Controls.Add(txtNombre);
            Boton("CARGAR", 420, 92, 170, 66, Paleta.Agua, Paleta.Agua2, (s, e) => Cargar());
            btnVendedor = Boton("", 610, 92, 360, 66, Paleta.LilaClaro, Paleta.Crema2, CambiarVendedor, 11);
            btnJugar = Boton("JUGAR SEMANA 1", 820, 540, 420, 72, Paleta.Verde, Paleta.Verde2, Jugar);
            btnFinal = Boton("VER EL FINAL", 820, 540, 420, 72, Paleta.Oro, Paleta.Oro2, (s, e) => IrA(new FormFinal(historia)));
            Boton("NUEVA HISTORIA", 820, 626, 200, 66, Paleta.Rosa, Paleta.Rosa2, NuevaHistoria, 10);
            Boton("VOLVER", 1040, 626, 200, 66, Paleta.Crema, Paleta.Crema2, (s, e) => VolverAlTitulo(), 11);
            ActualizarBotones();
        }

        // Para regresar al mapa con la historia ya cargada
        internal FormHistoria(Historia h) : this()
        {
            txtNombre.Text = h.Nombre;
            Cargar();
        }

        private void ActualizarBotones()
        {
            btnVendedor.Text = $"VENDEDOR: {Recursos.NombresVendedor[vendedor].ToUpper()}";
            btnJugar.Visible = historia != null && !historia.Terminada;
            btnFinal.Visible = historia != null && historia.Terminada;
            if (historia != null) btnJugar.Text = $"JUGAR SEMANA {historia.Semana + 1}";
            Invalidate();
        }

        private void Cargar()
        {
            string nombre = txtNombre.Text.Trim().Replace(";", ",");
            if (nombre == "")
            {
                aviso = "Primero escribe tu nombre.";
                Invalidate();
                return;
            }
            historia = Historia.Cargar(nombre);
            if (historia == null)
            {
                historia = new Historia(nombre, vendedor);
                aviso = $"Historia nueva para {nombre}. ¡Suerte en Ecatepec!";
            }
            else
            {
                vendedor = historia.Vendedor;
                aviso = historia.Terminada ? "Ya terminaste la historia. Ve tu final o empieza otra." : $"Vas en la semana {historia.Semana + 1}.";
            }
            historia.Guardar();
            Recursos.Sonar("menu");
            ActualizarBotones();
        }

        private void CambiarVendedor(object sender, EventArgs e)
        {
            vendedor = (vendedor + 1) % 4;
            if (historia != null)
            {
                historia.Vendedor = vendedor;
                historia.Guardar();
            }
            ActualizarBotones();
        }

        private void NuevaHistoria(object sender, EventArgs e)
        {
            if (historia == null) { Cargar(); if (historia == null) return; }
            if (!FormAviso.Preguntar(this, "¿EMPEZAR DE CERO?", "Se borra tu avance y tus decisiones. Regresas a Ecatepec.", Paleta.Rosa))
                return;
            historia = new Historia(historia.Nombre, vendedor);
            historia.Guardar();
            aviso = "Historia nueva. De vuelta al lote de tierra.";
            ActualizarBotones();
        }

        private Rectangle Mapa(int i) => new Rectangle(52 + i * 244, 214, 200, 124);

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            Pixel.Panel(g, 40, 84, 370, 82, Paleta.Crema);
            if (txtNombre.Text == "") Pixel.Texto(g, "Tu nombre...", 68, 130, 13, Paleta.Gris);

            // mapa de la ciudad: zonas unidas con líneas rectas
            int semana = historia?.Semana ?? 0;
            for (int i = 0; i < Historia.SEMANAS; i++)
            {
                Rectangle r = Mapa(i);
                if (i > 0) Pixel.R(g, r.X - 44, r.Y + r.Height / 2 - 4, 44, 8, i <= semana ? Paleta.Oro : Paleta.Morado3);
                Color marco = i < semana ? Paleta.Verde : i == semana ? Paleta.Oro : Paleta.Negro;
                Pixel.R(g, r.X - 6, r.Y - 6, r.Width + 12, r.Height + 12, marco);
                Escenario.Dibujar(g, i, r);
                if (i > semana)
                    for (int y = r.Y; y < r.Bottom; y += 4)
                        Pixel.R(g, r.X, y, r.Width, 2, Color.FromArgb(160, 20, 8, 30));   // bloqueada
                Pixel.Texto(g, $"{i + 1}. {Partida.Zonas[i]}", r.X + r.Width / 2, r.Bottom + 24, 10, Paleta.Crema, StringAlignment.Center);
                string estado = i < semana ? "¡GANADA!" : i == semana ? "AQUÍ VAS" : "";
                Pixel.Texto(g, estado, r.X + r.Width / 2, r.Bottom + 52, 9, i < semana ? Paleta.Verde : Paleta.Oro, StringAlignment.Center);
            }

            // el jefe de la semana
            Pixel.Panel(g, 40, 420, 760, 280, Paleta.Crema);
            if (historia != null && historia.Terminada)
            {
                Pixel.Texto(g, "¡TERMINASTE LA TEMPORADA!", 420, 470, 16, Paleta.Rosa2, StringAlignment.Center);
                Pixel.Parrafo(g, "Llegaste a Polanco y le ganaste a El Bro. Dale VER EL FINAL para saber cómo terminó tu historia.",
                              new Rectangle(80, 510, 680, 160), 12, Paleta.Noche, StringAlignment.Center);
            }
            else
            {
                int s = Math.Min(semana, Historia.SEMANAS - 1);
                Pixel.R(g, 64, 444, 136, 136, Paleta.Negro);
                Pixel.R(g, 68, 448, 128, 128, Paleta.Morado3);
                Pixel.Sprite(g, Recursos.Clientes[Historia.RetratoJefe[s]], 68, 448, 8);
                Pixel.Texto(g, $"SEMANA {s + 1} · JEFE: {Historia.Jefes[s].ToUpper()}", 224, 460, 13, Paleta.Rosa2);
                Pixel.Texto(g, $"Nivel {Bot.NombresNivel[Historia.NivelJefe[s]]}", 224, 494, 10, Paleta.Gris);
                Pixel.Parrafo(g, Historia.Intros[s], new Rectangle(224, 512, 556, 160), 11, Paleta.Noche);
                Pixel.Texto(g, "Gana la semana si terminas con más que el jefe.", 64, 668, 10, Paleta.Morado2);
            }

            Pixel.Parrafo(g, aviso, new Rectangle(820, 420, 420, 110), 12, Paleta.Oro);
        }

        private void Jugar(object sender, EventArgs e)
        {
            if (historia == null || historia.Terminada) return;
            int s = historia.Semana;
            Partida p = new Partida(s, historia);

            Concesionario tu = new Concesionario(historia.Nombre, vendedor);
            tu.Perfil = Perfiles.ObtenerOCrear(historia.Nombre);
            if (tu.Perfil != null && tu.Perfil.Personalizado) tu.Cara = tu.Perfil.Avatar.Dibujar();
            p.AgregarJugador(tu);

            Concesionario jefe = new Concesionario(Historia.Jefes[s], (vendedor + 1 + s) % 4, Historia.NivelJefe[s]);
            jefe.Cara = Recursos.Clientes[Historia.RetratoJefe[s]];
            p.AgregarJugador(jefe);

            IrA(new FormDraft(p));
        }
    }
}
