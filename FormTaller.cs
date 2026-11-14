namespace PrograProyectoFinal
{
    // Taller: reparar lo chocado, arreglar la falla, tunear para venderlo más caro, o rematarlo
    public class FormTaller : FormPixel
    {
        private Concesionario jugador;
        private int indice;
        private Auto auto;
        private bool eligiendoColor = false;       // ensenando los 8 colores para pintar

        public string Resultado { get; private set; } = "";
        public double Rematado { get; private set; } = 0;

        internal FormTaller(Concesionario jugador, int indice)
        {
            this.jugador = jugador;
            this.indice = indice;
            auto = jugador.ObtenerAuto(indice);
            ClientSize = new Size(820, 560);
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;

            bool mec = jugador.TieneMecanico;
            BotonPixel rep = Boton($"REPARAR ${auto.CostoReparar(mec) / 1000:N0}K", 30, 380, 184, 72, Paleta.Verde, Paleta.Verde2, Reparar, 9);
            BotonPixel fal = Boton(auto.FallaTieneArreglo ? $"ARREGLAR FALLA ${auto.CostoArreglarFalla(mec) / 1000:N0}K" : "ARREGLAR FALLA",
                                   222, 380, 184, 72, Paleta.Rosa, Paleta.Rosa2, ArreglarFalla, 8);
            BotonPixel tun = Boton($"TUNEAR ${auto.CostoTunear(mec) / 1000:N0}K", 414, 380, 184, 72, Paleta.Agua, Paleta.Agua2, Tunear, 9);
            Boton($"REMATAR ${jugador.PrecioRemate(auto) / 1000:N0}K", 606, 380, 184, 72, Paleta.Oro, Paleta.Oro2, Rematar, 9);
            Boton($"PINTAR ${jugador.CostoPintar / 1000:N0}K", 30, 470, 240, 66, Paleta.Lila, Paleta.Morado3,
                  (s, e) => { eligiendoColor = !eligiendoColor; Recursos.Sonar("menu"); Invalidate(); }, 11);
            Boton("CERRAR", 290, 470, 240, 66, Paleta.Crema, Paleta.Crema2, (s, e) => Close());
            rep.Enabled = auto.Estado == Auto.CHOCADO;
            fal.Enabled = auto.FallaTieneArreglo;
            tun.Enabled = auto.Estado == Auto.NORMAL;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pixel.Preparar(g); Pixel.Escalar(g);
            Pixel.R(g, 0, 0, AnchoD, AltoD, Paleta.Morado);
            Pixel.Panel(g, 0, 0, AnchoD, AltoD, Paleta.Crema);
            Pixel.R(g, 14, 14, AnchoD - 28, 60, Paleta.Oro);
            Pixel.Texto(g, "TALLER", AnchoD / 2, 44, 17, Paleta.Noche, StringAlignment.Center);

            Pixel.R(g, 40, 100, 300, 170, Paleta.LilaClaro);
            Pixel.Sprite(g, Recursos.Autos[auto.Tipo, auto.Color], 62, 130, 8);
            if (auto.Estado == Auto.CHOCADO)
            {
                Pixel.R(g, 150, 150, 24, 8, Paleta.Negro); Pixel.R(g, 200, 176, 16, 16, Paleta.Negro);   // abolladuras
                Pixel.R(g, 110, 190, 30, 6, Paleta.Gris);
            }
            if (auto.Estado == Auto.TUNEADO)
            {
                Pixel.R(g, 62, 238, 256, 6, Paleta.Rosa);      // luces neón
                Pixel.R(g, 66, 194, 20, 8, Paleta.Oro); Pixel.R(g, 262, 194, 20, 8, Paleta.Oro);
            }
            Pixel.Texto(g, auto.Nombre, 190, 290, 13, Paleta.Noche, StringAlignment.Center);
            string estado = auto.Estado == Auto.NORMAL ? "En buen estado" : Auto.NombresEstado[auto.Estado];
            if (auto.TieneFalla) estado = auto.NombreFalla;
            Pixel.Texto(g, estado, 190, 322, auto.TieneFalla ? 9 : 11, auto.Estado == Auto.CHOCADO || auto.TieneFalla ? Paleta.Rosa2 : Paleta.Verde2, StringAlignment.Center);

            if (eligiendoColor)
            {
                DibujarColores(g);
                Pixel.Texto(g, $"Te ha costado {Pixel.Dinero(auto.Costo)}  ·  vale {Pixel.Dinero(auto.ValorActual())}", 410, 350, 10, Paleta.Gris, StringAlignment.Center);
                return;
            }
            string texto = "Reparar: un chocado vale la mitad. Si lo vendes así, baja tu reputación.\n" +
                           "Arreglar falla: la falla le baja el valor y el cliente regresa enojado.\n" +
                           "Tunear: vale 30 % más.\nRematar: el lote de enfrente te da el 70 %.\nPintar: el color que quieras, aunque esté chocado.";
            if (jugador.TieneMecanico) texto += "\nTu mecánico cobra la mitad.";
            Pixel.Parrafo(g, texto, new Rectangle(370, 100, 420, 260), 10, Paleta.Noche);
            Pixel.Texto(g, $"Te ha costado {Pixel.Dinero(auto.Costo)}  ·  vale {Pixel.Dinero(auto.ValorActual())}", 410, 350, 10, Paleta.Gris, StringAlignment.Center);
        }

        // 8 colores en 2 filas de 4: cada uno es el mismo auto ya pintado
        private Rectangle Muestra(int k)
        {
            return new Rectangle(372 + (k % 4) * 104, 140 + (k / 4) * 100, 98, 78);
        }

        private void DibujarColores(Graphics g)
        {
            Pixel.Texto(g, $"ESCOGE EL COLOR  ·  pintar cuesta {Pixel.Dinero(jugador.CostoPintar)}", 580, 116, 11, Paleta.Morado2, StringAlignment.Center);
            for (int k = 0; k < Cliente.NombresColor.Length; k++)
            {
                Rectangle r = Muestra(k);
                bool actual = k == auto.Color;
                Pixel.R(g, r.X - 4, r.Y - 4, r.Width + 8, r.Height + 8, actual ? Paleta.Oro : Paleta.Negro);
                Pixel.R(g, r.X, r.Y, r.Width, r.Height, Paleta.LilaClaro);
                Pixel.Sprite(g, Recursos.Autos[auto.Tipo, k], r.X + 1, r.Y + 6, 3);
                string nombre = Cliente.NombresColor[k];
                Pixel.Texto(g, actual ? "ACTUAL" : nombre.ToUpper(), r.X + r.Width / 2, r.Bottom - 8, 8, actual ? Paleta.Oro2 : Paleta.Noche, StringAlignment.Center);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (!eligiendoColor) return;
            for (int k = 0; k < Cliente.NombresColor.Length; k++)
                if (Muestra(k).Contains(e.Location))
                {
                    int color = k;
                    string nombreColor = Cliente.NombresColor[color];
                    string estado = auto.Estado == Auto.CHOCADO ? " (sigue chocado: repáralo si quieres venderlo mejor)" : "";
                    Terminar(jugador.Pintar(indice, color), $"Pintaste el {auto.Nombre} de {nombreColor}.{estado}");
                    return;
                }
        }

        private void Terminar(string error, string exito)
        {
            if (error != null)
            {
                FormAviso.Mostrar(this, "NO SE PUDO", error, Paleta.Rosa);
                return;
            }
            Resultado = exito;
            Recursos.Sonar("compra");
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Reparar(object sender, EventArgs e) => Terminar(jugador.Reparar(indice), $"El {auto.Nombre} quedó como nuevo.");
        private void ArreglarFalla(object sender, EventArgs e) => Terminar(jugador.ArreglarFalla(indice), $"Le arreglaste \"{auto.NombreFalla}\" al {auto.Nombre}.");
        private void Tunear(object sender, EventArgs e) => Terminar(jugador.Tunear(indice), $"¡El {auto.Nombre} quedó tuneado! Rines, neón y todo.");

        private void Rematar(object sender, EventArgs e)
        {
            Rematado = jugador.Rematar(indice);
            Terminar(null, $"Remataste el {auto.Nombre}. Algo es algo.");
        }
    }
}
