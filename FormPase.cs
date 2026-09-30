namespace PrograProyectoFinal
{
    // Pase de temporada: 30 niveles en 3 páginas de 10. Arriba la fila GRATIS, abajo la PREMIUM.
    // Cada casilla se reclama con un clic cuando ya llegaste a ese nivel.
    public class FormPase : FormPixel
    {
        private const int POR_PAGINA = 10;

        private Perfil perfil;
        private int pagina;
        private string aviso = "";
        private BotonPixel btnPremium;
        private Image[,] vistas = new Image[2, Pase.NIVELES + 1];

        internal FormPase(Perfil perfil)
        {
            this.perfil = perfil;
            Titulo = "PASE DE TEMPORADA";
            BotonRegresar((s, e) => IrA(new FormLobby()));
            pagina = Math.Min(2, Math.Max(0, (perfil.NivelPase - 1) / POR_PAGINA));
            btnPremium = Boton($"DESBLOQUEAR PREMIUM · {Pase.PRECIO_PREMIUM}", 800, 84, 440, 64, Paleta.Oro, Paleta.Oro2, ComprarPremium, 12);
            Boton("<", 40, 620, 70, 70, Paleta.LilaClaro, Paleta.Crema2, (s, e) => { pagina = Math.Max(0, pagina - 1); Invalidate(); }, 16);
            Boton(">", 120, 620, 70, 70, Paleta.LilaClaro, Paleta.Crema2, (s, e) => { pagina = Math.Min(2, pagina + 1); Invalidate(); }, 16);
            Boton("RECLAMAR TODO", 900, 620, 340, 70, Paleta.Verde, Paleta.Verde2, (s, e) => ReclamarTodo(), 13);

            // cómo te verías con cada artículo del pase
            for (int fila = 0; fila < 2; fila++)
                for (int n = 1; n <= Pase.NIVELES; n++)
                    if (Pase.Tipo(fila == 1, n) == Pase.ARTICULO)
                    {
                        Avatar prueba = perfil.Avatar.Copia();
                        prueba.Cambiar(Pase.Parte(fila == 1, n), Pase.Opcion(fila == 1, n));
                        vistas[fila, n] = prueba.Dibujar();
                    }
            btnPremium.Visible = !perfil.PasePremium;
        }

        private Rectangle Casilla(bool premium, int k) => new Rectangle(60 + k * 118, premium ? 440 : 262, 110, 150);

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // ----- encabezado -----
            Pixel.Panel(g, 40, 70, 740, 110, Paleta.Morado2, Paleta.Negro);
            Pixel.Texto(g, Pase.NOMBRE, 64, 98, 13, Paleta.Oro);
            Pixel.Texto(g, $"NIVEL {perfil.NivelPase}/{Pase.NIVELES}", 64, 136, 17, Paleta.Crema);
            int enNivel = perfil.NivelPase >= Pase.NIVELES ? Pase.XP_POR_NIVEL : perfil.PaseXp % Pase.XP_POR_NIVEL;
            Pixel.R(g, 300, 126, 300, 22, Paleta.Negro);
            Pixel.R(g, 304, 130, 292 * enNivel / Pase.XP_POR_NIVEL, 14, Paleta.Agua);
            Pixel.Texto(g, $"{enNivel}/{Pase.XP_POR_NIVEL} XP", 616, 138, 10, Paleta.Tenue);
            Pixel.Texto(g, perfil.PasePremium ? "PREMIUM ACTIVO" : "PASE GRATIS", 760, 98, 10, perfil.PasePremium ? Paleta.Oro : Paleta.Tenue, StringAlignment.Far);
            if (perfil.PasePremium)
                Pixel.Parrafo(g, "Tienes el PREMIUM: reclama un premio en cada nivel.", new Rectangle(800, 90, 440, 60), 11, Paleta.Oro);
            Pixel.Sprite(g, Recursos.Moneda, 800, 158, 3);
            Pixel.Texto(g, $"{perfil.Monedas} monedas · la XP de tus partidas sube el pase", 830, 170, 8, Paleta.Tenue);

            // ----- números de nivel y barra de avance -----
            for (int k = 0; k < POR_PAGINA; k++)
            {
                int n = pagina * POR_PAGINA + k + 1;
                Rectangle c = Casilla(false, k);
                bool alcanzado = n <= perfil.NivelPase;
                Pixel.R(g, c.X, 206, c.Width, 30, alcanzado ? Paleta.Agua : Paleta.Morado3);
                Pixel.Texto(g, $"{n}", c.X + c.Width / 2, 221, 12, alcanzado ? Paleta.Noche : Paleta.Tenue, StringAlignment.Center);
            }
            Pixel.Texto(g, "GRATIS", 60, 250, 10, Paleta.Verde);
            Pixel.Texto(g, "PREMIUM", 60, 428, 10, Paleta.Oro);

            for (int fila = 0; fila < 2; fila++)
                for (int k = 0; k < POR_PAGINA; k++)
                    DibujarCasilla(g, fila == 1, pagina * POR_PAGINA + k + 1, Casilla(fila == 1, k));

            Pixel.Texto(g, $"{pagina + 1}/3", 210, 655, 13, Paleta.Crema);
            Pixel.Parrafo(g, aviso != "" ? aviso : "Los artículos del pase son exclusivos: no salen en la tienda.", new Rectangle(270, 626, 610, 60), 11, aviso != "" ? Paleta.Oro : Paleta.Tenue);
        }

        private void DibujarCasilla(Graphics g, bool premium, int n, Rectangle c)
        {
            int tipo = Pase.Tipo(premium, n);
            bool reclamado = perfil.Reclamado(premium, n);
            bool listo = Pase.SePuedeReclamar(perfil, premium, n);
            bool bloqueado = n > perfil.NivelPase || (premium && !perfil.PasePremium);

            Color marco = listo ? Paleta.Oro : Paleta.Negro;
            Pixel.R(g, c.X - 3, c.Y - 3, c.Width + 6, c.Height + 6, marco);
            if (tipo == Pase.NADA)
            {
                Pixel.R(g, c.X, c.Y, c.Width, c.Height, Paleta.Morado2);
                return;
            }
            Color fondo = tipo == Pase.ARTICULO ? Tienda.ColorRareza[Cosmeticos.Rareza(Pase.Parte(premium, n), Pase.Opcion(premium, n))] : Paleta.Morado3;
            Pixel.R(g, c.X, c.Y, c.Width, c.Height, fondo);
            Pixel.R(g, c.X, c.Bottom - 42, c.Width, 42, Paleta.Noche);

            if (tipo == Pase.ARTICULO)
                Pixel.Sprite(g, vistas[premium ? 1 : 0, n], c.X + 15, c.Y + 8, 5);
            else
            {
                Pixel.Sprite(g, Recursos.Moneda, c.X + 31, c.Y + 22, 6);
                Pixel.Texto(g, $"{Pase.Monedas(premium, n)}", c.X + c.Width / 2, c.Y + 88, 14, Paleta.Oro, StringAlignment.Center);
            }
            Pixel.Parrafo(g, Pase.Descripcion(premium, n), new Rectangle(c.X + 4, c.Bottom - 40, c.Width - 8, 40), 7, Paleta.Crema, StringAlignment.Center);

            if (reclamado)
            {
                using (SolidBrush velo = new SolidBrush(Color.FromArgb(120, 20, 60, 30)))
                    g.FillRectangle(velo, c);
                Pixel.R(g, c.Right - 30, c.Y + 6, 24, 24, Paleta.Verde);
                Pixel.Texto(g, "OK", c.Right - 18, c.Y + 18, 7, Paleta.Noche, StringAlignment.Center);
            }
            else if (bloqueado)
            {
                using (SolidBrush velo = new SolidBrush(Color.FromArgb(140, 18, 8, 24)))
                    g.FillRectangle(velo, c);
                // candado de cuadritos
                int x = c.X + c.Width / 2 - 12, y = c.Y + 40;
                Pixel.R(g, x + 4, y, 16, 4, Paleta.Tenue); Pixel.R(g, x + 4, y, 4, 12, Paleta.Tenue); Pixel.R(g, x + 16, y, 4, 12, Paleta.Tenue);
                Pixel.R(g, x, y + 12, 24, 18, Paleta.Tenue); Pixel.R(g, x + 10, y + 18, 4, 6, Paleta.Negro);
            }
            else if (listo)
                Pixel.Texto(g, "¡RECLAMAR!", c.X + c.Width / 2, c.Y + 12, 8, Paleta.Oro, StringAlignment.Center);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            for (int fila = 0; fila < 2; fila++)
                for (int k = 0; k < POR_PAGINA; k++)
                    if (Casilla(fila == 1, k).Contains(e.Location))
                        Reclamar(fila == 1, pagina * POR_PAGINA + k + 1);
        }

        private void Reclamar(bool premium, int n)
        {
            string error = Pase.Reclamar(perfil, premium, n);
            if (error != null)
            {
                aviso = error;
                Recursos.Sonar("error");
                Invalidate();
                return;
            }
            Recursos.Sonar("compra");
            aviso = $"¡Reclamaste {Pase.Descripcion(premium, n)}!";
            Perfiles.Guardar();
            if (Pase.Tipo(premium, n) == Pase.ARTICULO &&
                FormAviso.Preguntar(this, "¡ES TUYO!", $"¿Te pones {Pase.Descripcion(premium, n)}?", Paleta.Oro, vistas[premium ? 1 : 0, n]))
            {
                perfil.Avatar.Cambiar(Pase.Parte(premium, n), Pase.Opcion(premium, n));
                perfil.Personalizado = true;
                Perfiles.Guardar();
            }
            Invalidate();
        }

        private void ReclamarTodo()
        {
            int cuantos = 0;
            for (int fila = 0; fila < 2; fila++)
                for (int n = 1; n <= Pase.NIVELES; n++)
                    if (Pase.SePuedeReclamar(perfil, fila == 1, n) && Pase.Reclamar(perfil, fila == 1, n) == null)
                        cuantos++;
            aviso = cuantos == 0 ? "No hay nada nuevo para reclamar. ¡Juega para subir de nivel!" : $"Reclamaste {cuantos} premios. Póntelos en el CASILLERO.";
            Recursos.Sonar(cuantos == 0 ? "error" : "compra");
            Perfiles.Guardar();
            Invalidate();
        }

        private void ComprarPremium(object sender, EventArgs e)
        {
            if (!FormAviso.Preguntar(this, "PASE PREMIUM", $"Cuesta {Pase.PRECIO_PREMIUM} monedas y tienes {perfil.Monedas}. Desbloquea un premio en cada nivel, con artículos exclusivos. ¿Lo compras?", Paleta.Oro))
                return;
            string error = perfil.ComprarPremium();
            if (error != null)
            {
                aviso = error;
                Recursos.Sonar("error");
            }
            else
            {
                aviso = "¡Ya tienes el PREMIUM! Reclama todo lo que ya alcanzaste.";
                Recursos.Sonar("victoria");
                btnPremium.Visible = false;
                Perfiles.Guardar();
            }
            Invalidate();
        }
    }
}
