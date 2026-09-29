namespace PrograProyectoFinal
{
    // Regateo: el jugador pone el precio y el cliente acepta, contraoferta o se va
    public partial class FormNegociar : FormPixel
    {
        private Partida partida;
        private int indice;
        private Auto auto;
        private double precio;
        private double contraoferta = 0;
        private string respuesta;

        public string Resultado { get; private set; } = "";
        public double Recibido { get; private set; } = 0;

        internal FormNegociar(Partida partida, int indice)
        {
            InitializeComponent();
            ClientSize = new Size(1000, 600);
            this.partida = partida;
            this.indice = indice;
            auto = partida.JugadorActual.ObtenerAuto(indice);
            precio = partida.JugadorActual.PrecioVenta(auto);
            Cliente cliente = partida.ClienteActual;
            respuesta = $"{cliente.OpinionEstado(auto)} {cliente.OpinionColor(auto)}".Trim();
            if (respuesta == "") respuesta = cliente.Saludo;

            btnOfrecer.Colores(Paleta.Verde, Paleta.Verde2);
            btnSuOferta.Colores(Paleta.Oro, Paleta.Oro2);
            btnYaNo.Colores(Paleta.Crema, Paleta.Crema2);
            foreach (BotonPixel b in new[] { btnMenos10, btnMenos1, btnMas1, btnMas10 })
                b.Colores(Paleta.LilaClaro, Paleta.Crema2);
            btnSuOferta.Visible = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pixel.Preparar(g);
            Pixel.R(g, 0, 0, Width, Height, Paleta.Morado);
            Pixel.Panel(g, 0, 0, Width, Height, Paleta.Crema);
            Pixel.R(g, 14, 14, Width - 28, 60, Paleta.Agua);
            Pixel.Texto(g, "¿EN CUÁNTO SE LO DEJAS?", Width / 2, 44, 16, Paleta.Noche, StringAlignment.Center);

            Cliente cliente = partida.ClienteActual;

            // cliente con su globo
            Pixel.R(g, 36, 100, 176, 176, Paleta.Negro);
            Pixel.R(g, 42, 106, 164, 164, Paleta.Morado3);
            Pixel.Sprite(g, Recursos.Clientes[cliente.Retrato], 44, 116, 10);
            Pixel.Texto(g, cliente.Nombre, 124, 296, 12, Paleta.Noche, StringAlignment.Center);
            Pixel.Panel(g, 240, 100, 360, 190, Paleta.LilaClaro);
            Pixel.R(g, 226, 180, 18, 16, Paleta.Negro);
            Pixel.R(g, 232, 184, 14, 8, Paleta.LilaClaro);
            Pixel.Parrafo(g, respuesta, new Rectangle(262, 116, 320, 164), 12, Paleta.Noche);

            // el auto
            Pixel.R(g, 630, 100, 334, 190, Paleta.LilaClaro);
            Pixel.Sprite(g, Recursos.Autos[auto.Tipo, auto.Color], 699, 112, 6);
            Pixel.Texto(g, $"{auto.Nombre} · {auto.NombreTipo}", 797, 226, 12, Paleta.Noche, StringAlignment.Center);
            Pixel.Texto(g, $"Te costó {Pixel.Dinero(auto.Costo)}", 797, 262, 11, Paleta.Gris, StringAlignment.Center);

            // precio
            Pixel.Texto(g, "TU PRECIO", Width / 2, 330, 12, Paleta.Morado2, StringAlignment.Center);
            Color colorPrecio = precio < auto.Costo ? Paleta.Rosa2 : Paleta.Verde2;
            Pixel.Texto(g, Pixel.Dinero(precio), Width / 2, 382, 22, colorPrecio, StringAlignment.Center);
            double ganancia = precio - auto.Costo;
            string texto = ganancia >= 0 ? $"ganas {Pixel.Dinero(ganancia)}" : $"pierdes {Pixel.Dinero(-ganancia)}";
            Pixel.Texto(g, texto, Width / 2, 432, 11, colorPrecio, StringAlignment.Center);

            Pixel.Texto(g, $"Intentos: {partida.Intentos}/{Partida.MAX_INTENTOS}", 40, 560, 11, Paleta.Gris);
        }

        private void Cambiar(double cuanto)
        {
            precio = Math.Max(1000, precio + cuanto);
            Invalidate();
        }

        private void btnMenos10_Click(object sender, EventArgs e) => Cambiar(-10000);
        private void btnMenos1_Click(object sender, EventArgs e) => Cambiar(-1000);
        private void btnMas1_Click(object sender, EventArgs e) => Cambiar(1000);
        private void btnMas10_Click(object sender, EventArgs e) => Cambiar(10000);

        private void btnOfrecer_Click(object sender, EventArgs e)
        {
            Ofrecer(precio);
        }

        private void btnSuOferta_Click(object sender, EventArgs e)
        {
            precio = contraoferta;
            Ofrecer(precio);
        }

        private void Ofrecer(double cantidad)
        {
            string nombre = auto.Nombre;
            bool vendido = partida.OfrecerPrecio(indice, cantidad, out respuesta, out contraoferta, out double recibido);
            if (vendido)
            {
                Recibido = recibido;
                Resultado = respuesta;
                Recursos.Sonar("venta");
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            Recursos.Sonar("error");
            if (partida.YaVendio)
            {
                // Se acabaron los intentos: el cliente se fue
                Resultado = respuesta;
                FormAviso.Mostrar(this, "SE FUE EL CLIENTE", $"{respuesta} Ya no puedes vender el {nombre} en este turno.", Paleta.Rosa);
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }
            btnSuOferta.Text = $"ACEPTAR {Pixel.Dinero(contraoferta)}";
            btnSuOferta.Visible = true;
            Invalidate();
        }

        private void btnYaNo_Click(object sender, EventArgs e)
        {
            Resultado = "¿Entonces qué? ¿No me lo vas a vender?";
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
