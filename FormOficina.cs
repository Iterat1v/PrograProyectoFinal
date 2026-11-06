namespace PrograProyectoFinal
{
    // Oficina: préstamos (banco y compadre) y empleados (vendedor y mecánico)
    public class FormOficina : FormPixel
    {
        private const double MONTO = 50000;
        private Concesionario jugador;
        private BotonPixel btnVendedor, btnMecanico;
        private string aviso = "";

        internal FormOficina(Concesionario jugador)
        {
            this.jugador = jugador;
            ClientSize = new Size(1100, 640);
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;

            Boton("PEDIR $50K", 50, 330, 230, 64, Paleta.Verde, Paleta.Verde2, (s, e) => Hacer(jugador.PedirBanco(MONTO), "El banco te prestó $50,000."), 11);
            Boton("PAGAR $50K", 290, 330, 230, 64, Paleta.Crema, Paleta.Crema2, (s, e) => Hacer(jugador.Pagar(true, MONTO), "Le pagaste al banco."), 11);
            Boton("PEDIR $50K", 580, 330, 230, 64, Paleta.Verde, Paleta.Verde2, (s, e) => Hacer(jugador.PedirCompadre(MONTO), "Tu compadre te prestó $50,000. \"Nomás no se te olvide\"."), 11);
            Boton("PAGAR $50K", 820, 330, 230, 64, Paleta.Crema, Paleta.Crema2, (s, e) => Hacer(jugador.Pagar(false, MONTO), "Le pagaste a tu compadre."), 11);
            btnVendedor = Boton("", 50, 520, 470, 64, Paleta.Agua, Paleta.Agua2, (s, e) => Empleado(true), 11);
            btnMecanico = Boton("", 580, 520, 470, 64, Paleta.Agua, Paleta.Agua2, (s, e) => Empleado(false), 11);
            Boton("CERRAR", 820, 16, 250, 54, Paleta.Crema, Paleta.Crema2, (s, e) => Close(), 11);
            Actualizar();
        }

        private void Actualizar()
        {
            btnVendedor.Text = jugador.TieneVendedor ? "DESPEDIR AL VENDEDOR" : "CONTRATAR VENDEDOR";
            btnMecanico.Text = jugador.TieneMecanico ? "DESPEDIR AL MECÁNICO" : "CONTRATAR MECÁNICO";
            Invalidate();
        }

        private void Hacer(string error, string exito)
        {
            aviso = error ?? exito;
            Recursos.Sonar(error == null ? "compra" : "error");
            Actualizar();
        }

        private void Empleado(bool vendedor)
        {
            bool tiene = vendedor ? jugador.TieneVendedor : jugador.TieneMecanico;
            jugador.Contratar(vendedor, !tiene);
            aviso = tiene ? "Ya no trabaja contigo." : "¡Contratado! Se le paga cada mañana.";
            Recursos.Sonar("menu");
            Actualizar();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pixel.Preparar(g); Pixel.Escalar(g);
            Pixel.R(g, 0, 0, AnchoD, AltoD, Paleta.Morado);
            Pixel.Panel(g, 0, 0, AnchoD, AltoD, Paleta.Morado2, Paleta.Negro);
            Pixel.Texto(g, "OFICINA", 40, 44, 18, Paleta.Oro);
            Pixel.Sprite(g, Recursos.Moneda, 330, 30, 5);
            Pixel.Texto(g, $"Tienes {Pixel.Dinero(jugador.Dinero)}", 370, 44, 13, Paleta.Oro);

            // préstamos
            Pixel.Panel(g, 30, 90, 510, 320, Paleta.Crema);
            Pixel.Texto(g, "BANCO", 54, 124, 15, Paleta.Morado2);
            Pixel.Parrafo(g, $"Hasta {Pixel.Dinero(Concesionario.TOPE_BANCO)}. Cobra 8 % de interés cada mañana.", new Rectangle(54, 148, 460, 70), 11, Paleta.Noche);
            Pixel.Texto(g, $"Debes: {Pixel.Dinero(jugador.DeudaBanco)}", 54, 262, 14, jugador.DeudaBanco > 0 ? Paleta.Rosa2 : Paleta.Verde2);

            Pixel.Panel(g, 560, 90, 510, 320, Paleta.Crema);
            Pixel.Texto(g, "TU COMPADRE", 584, 124, 15, Paleta.Morado2);
            Pixel.Parrafo(g, $"Hasta {Pixel.Dinero(Concesionario.TOPE_COMPADRE)} sin intereses... pero a los 3 días te cobra en el grupo de la familia (-10 de reputación cada día).",
                          new Rectangle(584, 148, 460, 100), 11, Paleta.Noche);
            Pixel.Texto(g, $"Debes: {Pixel.Dinero(jugador.DeudaCompadre)}", 584, 262, 14, jugador.DeudaCompadre > 0 ? Paleta.Rosa2 : Paleta.Verde2);
            if (jugador.DeudaCompadre > 0) Pixel.Texto(g, $"Van {jugador.DiasCompadre} días", 584, 294, 10, Paleta.Gris);

            // empleados
            Pixel.Texto(g, "EMPLEADOS", 40, 440, 15, Paleta.Agua);
            Pixel.Parrafo(g, $"Vendedor ({Pixel.Dinero(Concesionario.SUELDO_VENDEDOR)} al día): si pasas turno sin vender, él atiende al cliente.",
                          new Rectangle(50, 462, 480, 60), 10, Paleta.Crema);
            Pixel.Parrafo(g, $"Mecánico ({Pixel.Dinero(Concesionario.SUELDO_MECANICO)} al día): repara y tunea a mitad de precio.",
                          new Rectangle(580, 462, 480, 60), 10, Paleta.Crema);

            Pixel.Texto(g, aviso, 40, 612, 11, Paleta.Oro);
        }
    }
}
