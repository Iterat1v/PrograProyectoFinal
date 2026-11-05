namespace PrograProyectoFinal
{
    // Lobby: tu personaje en grande sobre su base, con CASILLERO, LOGROS y JUGAR.
    // El que entra aquí queda como Perfiles.Activo y su nombre sale en las demás pantallas.
    public class FormLobby : FormPixel
    {
        private TextBox txtNombre;
        private BotonPixel btnEntrar, btnCambiar;
        private string aviso = "";
        private int brillo = 0;                // para que los reflectores se muevan
        private System.Windows.Forms.Timer timerLuces = new System.Windows.Forms.Timer { Interval = 80 };

        public FormLobby()
        {
            Titulo = "LOBBY";
            txtNombre = new TextBox
            {
                BorderStyle = BorderStyle.None, BackColor = Paleta.Crema, ForeColor = Paleta.Noche,
                Location = new Point(64, 132), Size = new Size(300, 40), MaxLength = 12, Font = Recursos.Letra(15)
            };
            txtNombre.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { Entrar(); e.SuppressKeyPress = true; } };
            Controls.Add(txtNombre);
            btnEntrar = Boton("ENTRAR", 64, 188, 300, 56, Paleta.Agua, Paleta.Agua2, (s, e) => Entrar(), 12);
            btnCambiar = Boton("CAMBIAR DE JUGADOR", 64, 188, 300, 56, Paleta.LilaClaro, Paleta.Crema2, (s, e) => Salir(), 10);

            Boton("CASILLERO", 940, 150, 300, 96, Paleta.Lila, Paleta.Morado3, (s, e) => Abrir("casillero"), 20);
            Boton("LOGROS", 940, 262, 300, 96, Paleta.Agua, Paleta.Agua2, (s, e) => Abrir("logros"), 20);
            Boton("CÓMO SE JUEGA", 300, 632, 280, 70, Paleta.Crema, Paleta.Crema2, (s, e) =>
            {
                using (FormReglas reglas = new FormReglas()) reglas.ShowDialog(this);
            }, 11);
            Boton("¡JUGAR!", 900, 604, 340, 100, Paleta.Verde, Paleta.Verde2, (s, e) => IrA(new FormModo()), 26);
            Boton("REGRESAR", 40, 632, 240, 70, Paleta.Crema, Paleta.Crema2, (s, e) => VolverAlTitulo(), 12);

            timerLuces.Tick += (s, e) => { brillo = (brillo + 1) % 40; Invalidate(new Rectangle(300, 60, 680, 560)); };
            timerLuces.Start();
            FormClosed += (s, e) => timerLuces.Stop();
            Refrescar();
        }

        private Perfil P => Perfiles.Activo;

        private void Refrescar()
        {
            txtNombre.Visible = P == null;
            btnEntrar.Visible = P == null;
            btnCambiar.Visible = P != null;
            if (P != null) P.PrepararMisiones(Reto.Hoy);
            Invalidate();
        }

        private void Entrar()
        {
            string nombre = txtNombre.Text.Trim().Replace(";", ",");
            if (nombre == "")
            {
                aviso = "Escribe tu nombre para entrar.";
                Recursos.Sonar("error");
                Invalidate();
                return;
            }
            Perfiles.Activo = Perfiles.ObtenerOCrear(nombre);
            aviso = "";
            Recursos.Sonar("compra");
            Refrescar();
        }

        private void Salir()
        {
            Perfiles.Activo = null;
            txtNombre.Text = "";
            Refrescar();
            txtNombre.Focus();
        }

        private void Abrir(string que)
        {
            if (P == null)
            {
                aviso = "Primero escribe tu nombre y dale ENTRAR.";
                Recursos.Sonar("error");
                Invalidate();
                return;
            }
            Recursos.Sonar("menu");
            if (que == "logros") IrA(new FormLogros(P));
            else
            {
                using (FormPersonaje casillero = new FormPersonaje(P))
                    casillero.ShowDialog(this);
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // ----- escenario del centro: cielo, reflectores y la base -----
            Pixel.R(g, 400, 60, 480, 560, Paleta.Noche);
            Pixel.Estrellas(g, 30, 5);
            Pixel.R(g, 0, 60, 400, 0, Paleta.Noche);
            for (int k = 0; k < 2; k++)
            {
                // reflector: triángulo hecho con rectángulos que se va abriendo hacia abajo
                int centro = k == 0 ? 470 + brillo * 3 : 810 - brillo * 3;
                for (int y = 70; y < 540; y += 10)
                {
                    int ancho = 8 + (y - 70) / 3;
                    using (SolidBrush luz = new SolidBrush(Color.FromArgb(28, 255, 212, 71)))
                        g.FillRectangle(luz, centro - ancho / 2 + (k == 0 ? (y - 70) / 4 : -(y - 70) / 4), y, ancho, 10);
                }
            }
            // base (pedestal) en tres escalones
            Pixel.R(g, 470, 540, 340, 20, Paleta.Negro);
            Pixel.R(g, 440, 556, 400, 30, Paleta.Morado3);
            Pixel.R(g, 440, 556, 400, 6, Paleta.Lila);
            Pixel.R(g, 410, 584, 460, 30, Paleta.Morado2);

            if (P == null)
            {
                Pixel.Sprite(g, Recursos.Vendedores[0], 480, 220, 20);
                Pixel.SombraTexto(g, "¿QUIÉN ERES?", 640, 130, 40, Paleta.Oro, Paleta.Rosa);
            }
            else
            {
                Image cara = P.Personalizado ? P.Avatar.Dibujar() : Recursos.Vendedores[0];
                int salto = brillo < 20 ? brillo / 5 : (40 - brillo) / 5;          // respira
                Pixel.Sprite(g, cara, 480, 220 - salto * 2, 20);
                Pixel.SombraTexto(g, P.Nombre.ToUpper(), 640, 110, 44, Paleta.Oro, Paleta.Rosa);
                Pixel.Texto(g, $"NIVEL {P.Nivel}", 640, 160, 13, Paleta.Agua, StringAlignment.Center);
                Pixel.R(g, 490, 178, 300, 16, Paleta.Negro);
                Pixel.R(g, 494, 182, 292 * P.XpEnNivel / Perfil.XP_POR_NIVEL, 8, Paleta.Agua);
            }

            // ----- panel izquierdo: jugador y misiones -----
            Pixel.Panel(g, 40, 76, 340, 184, Paleta.Crema);
            Pixel.Texto(g, P == null ? "TU NOMBRE" : "JUGANDO COMO", 64, 106, 11, Paleta.Morado2);
            if (P == null && txtNombre.Text == "")
                Pixel.Texto(g, "Escribe aquí...", 64, 152, 13, Paleta.Gris);
            if (P != null)
                Pixel.Texto(g, P.Nombre, 64, 150, 16, Paleta.Noche);

            Pixel.Panel(g, 40, 276, 340, 340, Paleta.Crema);
            Pixel.Texto(g, "MISIONES DE HOY", 64, 306, 12, Paleta.Rosa2);
            int[] hoy = Misiones.DelDia(Reto.Hoy);
            for (int k = 0; k < Misiones.POR_DIA; k++)
            {
                int y = 336 + k * 80;
                int meta = Misiones.Metas[hoy[k]];
                int avance = P == null ? 0 : Math.Min(meta, P.Progreso(k));
                bool lista = P != null && P.Cobrada(k);
                Pixel.Parrafo(g, Misiones.Textos[hoy[k]], new Rectangle(62, y, 300, 40), 10, lista ? Paleta.Verde2 : Paleta.Noche);
                Pixel.R(g, 64, y + 44, 240, 14, Paleta.Negro);
                Pixel.R(g, 66, y + 46, 236 * avance / meta, 10, lista ? Paleta.Verde : Paleta.Oro);
                Pixel.Texto(g, lista ? "¡LISTA!" : $"{avance}/{meta}", 312, y + 51, 9, lista ? Paleta.Verde2 : Paleta.Gris);
            }
            Pixel.Texto(g, $"+{Misiones.MONEDAS} monedas cada una", 64, 592, 9, Paleta.Gris);

            // ----- derecha: monedas y racha -----
            Pixel.Panel(g, 940, 76, 300, 64, Paleta.Morado2, Paleta.Negro);
            Pixel.Sprite(g, Recursos.Moneda, 956, 88, 5);
            Pixel.Texto(g, P == null ? "---" : $"{P.Monedas}", 1010, 108, 17, Paleta.Oro);
            if (P != null && P.Racha > 0)
                Pixel.Texto(g, $"RACHA {P.Racha}", 1228, 108, 9, P.UltimoReto >= Reto.Ayer ? Paleta.Rosa : Paleta.Tenue, StringAlignment.Far);
            if (P != null)
                Pixel.Texto(g, $"Logros {P.CuantosLogros()}/{Logros.CANTIDAD}", 940, 528, 10, Paleta.Tenue);

            if (aviso != "")
                Pixel.Parrafo(g, aviso, new Rectangle(600, 640, 290, 60), 10, Paleta.Rosa);
        }
    }
}
