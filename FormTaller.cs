namespace PrograProyectoFinal
{
    // Taller: reparar lo chocado, tunear para venderlo más caro, o rematarlo
    public class FormTaller : FormPixel
    {
        private Concesionario jugador;
        private int indice;
        private Auto auto;

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
            BotonPixel rep = Boton($"REPARAR ${auto.CostoReparar(mec) / 1000:N0}K", 40, 380, 240, 72, Paleta.Verde, Paleta.Verde2, Reparar, 11);
            BotonPixel tun = Boton($"TUNEAR ${auto.CostoTunear(mec) / 1000:N0}K", 290, 380, 240, 72, Paleta.Agua, Paleta.Agua2, Tunear, 11);
            Boton($"REMATAR ${jugador.PrecioRemate(auto) / 1000:N0}K", 540, 380, 240, 72, Paleta.Oro, Paleta.Oro2, Rematar, 11);
            Boton("CERRAR", 290, 470, 240, 66, Paleta.Crema, Paleta.Crema2, (s, e) => Close());
            rep.Enabled = auto.Estado == Auto.CHOCADO;
            tun.Enabled = auto.Estado == Auto.NORMAL;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pixel.Preparar(g);
            Pixel.R(g, 0, 0, Width, Height, Paleta.Morado);
            Pixel.Panel(g, 0, 0, Width, Height, Paleta.Crema);
            Pixel.R(g, 14, 14, Width - 28, 60, Paleta.Oro);
            Pixel.Texto(g, "TALLER", Width / 2, 44, 17, Paleta.Noche, StringAlignment.Center);

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
            Pixel.Texto(g, estado, 190, 322, 11, auto.Estado == Auto.CHOCADO ? Paleta.Rosa2 : Paleta.Verde2, StringAlignment.Center);

            string texto = "Reparar: un chocado vale la mitad y los clientes lo pagan a la mitad. Si lo vendes así, baja tu reputación.\n" +
                           "Tunear: vale 30 % más y los clientes pagan más.\nRematar: el lote de enfrente te da el 70 %.";
            if (jugador.TieneMecanico) texto += "\nTu mecánico cobra la mitad.";
            Pixel.Parrafo(g, texto, new Rectangle(370, 100, 420, 260), 10, Paleta.Noche);
            Pixel.Texto(g, $"Te ha costado {Pixel.Dinero(auto.Costo)}  ·  vale {Pixel.Dinero(auto.ValorActual())}", 410, 350, 10, Paleta.Gris, StringAlignment.Center);
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
        private void Tunear(object sender, EventArgs e) => Terminar(jugador.Tunear(indice), $"¡El {auto.Nombre} quedó tuneado! Rines, neón y todo.");

        private void Rematar(object sender, EventArgs e)
        {
            Rematado = jugador.Rematar(indice);
            Terminar(null, $"Remataste el {auto.Nombre}. Algo es algo.");
        }
    }
}
