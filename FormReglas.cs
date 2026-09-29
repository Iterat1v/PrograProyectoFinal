namespace PrograProyectoFinal
{
    // "Cómo se juega": las reglas de la presentación
    public partial class FormReglas : FormPixel
    {
        private string[] reglas =
        {
            "Juegan de 2 a 4 personas en la misma computadora. Cada quien administra su propio concesionario y empieza con $500,000.",
            "La partida dura 10 rondas. En tu turno llega un cliente que te dice qué tipo de auto busca (sedán, compacto o pickup) y cuánto trae.",
            "Compra autos en el MERCADO para tener qué vender. Hay 30 autos y cada uno solo lo puede comprar un jugador. Tu lote aguanta 10.",
            "Escoge un auto de tu lote y dale VENDER. Si es del tipo que busca y le alcanza, ganas 30 % sobre lo que pagaste. Si no te conviene, PASAR TURNO.",
            "Las ABILICARDS (Regateo, Estafa y Aseguradora) te dan ventajas o afectan a un rival.",
            "Gana quien termine con la mayor ganancia:  dinero final + valor de los autos que le queden."
        };

        public FormReglas()
        {
            InitializeComponent();
            btnVolver.Colores(Paleta.Verde, Paleta.Verde2);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            Pixel.Panel(g, 40, 84, 1200, 500, Paleta.Crema);
            for (int i = 0; i < reglas.Length; i++)
            {
                int y = 112 + i * 76;
                Pixel.R(g, 68, y + 10, 16, 16, i % 3 == 0 ? Paleta.Oro : i % 3 == 1 ? Paleta.Rosa : Paleta.Agua);
                Pixel.Parrafo(g, reglas[i], new Rectangle(104, y - 4, 1110, 76), 12, Paleta.Noche);
            }
            Pixel.Texto(g, "Ganancia = Dinero final + Valor de los autos finales", 640, 636, 13, Paleta.Oro, StringAlignment.Center);
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
