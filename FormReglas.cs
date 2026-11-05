namespace PrograProyectoFinal
{
    // "Cómo se juega": las reglas de la presentación
    public partial class FormReglas : FormPixel
    {
        private string[] reglas =
        {
            "De 1 a 4 jugadores: solo contra bots o con amigos en la misma compu, en la zona que escojas (de Ecatepec a Polanco). En MODO HISTORIA juegas 5 semanas contra 5 jefes. Empiezas con $500,000.",
            "La partida dura 5 días de 2 turnos (mañana y tarde). Llega un cliente que dice qué tipo de auto busca, cuánto trae y a veces de qué color (si no lo tienes, lo pintas por $10,000).",
            "En el MERCADO escoges el auto y su color (a cada cliente le gustan unos y odia otros). Los CHOCADOS salen a mitad de precio: repáralos o túnealos en el TALLER.",
            "Escoge un auto de tu lote y dale VENDER: tú pones el precio y el cliente acepta o te contraoferta (3 intentos). Si es de otro tipo, solo lo compra muy barato. Sin dinero, REMATA un auto al 70 %.",
            "Antes de empezar hay un draft: salen 16 ABILICARDS y cada quien escoge 3. Regateo te ayuda, Estafa ataca a un rival y Aseguradora te defiende sola.",
            "Cuida tu REPUTACIÓN: estafar, vender chatarra o que se te vaya un cliente la baja, y con mala fama llegan menos clientes. En la OFICINA pides préstamos y contratas vendedor y mecánico.",
            "Cada día puede pasar algo (terremoto, Black Friday, zombies, subasta...) y una vez por partida todos votan el evento. Los clientes regresan y la partida se guarda sola."
        };

        public FormReglas()
        {
            InitializeComponent();
            btnVolver.Colores(Paleta.Verde, Paleta.Verde2);
            btnCartas.Colores(Paleta.Rosa, Paleta.Rosa2);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            Pixel.Panel(g, 40, 84, 1200, 500, Paleta.Crema);
            for (int i = 0; i < reglas.Length; i++)
            {
                int y = 100 + i * 68;
                Pixel.R(g, 68, y + 10, 16, 16, i % 3 == 0 ? Paleta.Oro : i % 3 == 1 ? Paleta.Rosa : Paleta.Agua);
                Pixel.Parrafo(g, reglas[i], new Rectangle(104, y - 2, 1110, 68), 11, Paleta.Noche);
            }
            Pixel.Texto(g, "Ganancia = Dinero + Valor de los autos", 40, 656, 13, Paleta.Oro);
        }

        private void btnCartas_Click(object sender, EventArgs e)
        {
            using (FormCartas cartas = new FormCartas())
                cartas.ShowDialog(this);
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
