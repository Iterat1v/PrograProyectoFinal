namespace PrograProyectoFinal
{
    // "Cómo se juega": las reglas de la presentación
    public partial class FormReglas : FormPixel
    {
        private string[] reglas =
        {
            "Juegan de 2 a 4 en la misma computadora (con + BOT juegas contra la compu). Cada quien empieza con $500,000.",
            "La partida dura 5 días de 2 turnos (mañana y tarde). Llega un cliente que dice qué tipo de auto busca, cuánto trae y a veces de qué color (si no lo tienes, lo pintas por $10,000).",
            "Compra autos en el MERCADO para tener qué vender. Hay 30 autos y cada uno solo lo puede comprar un jugador. Tu lote aguanta 10.",
            "Escoge un auto de tu lote y dale VENDER: tú pones el precio y el cliente acepta o te contraoferta (3 intentos). Si es de otro tipo, solo lo compra muy barato. Sin dinero, REMATA un auto al 70 %.",
            "Antes de empezar hay un draft: salen 16 ABILICARDS y cada quien escoge 3. Regateo te ayuda, Estafa ataca a un rival y Aseguradora te defiende sola.",
            "Cada vendedor tiene su ventaja. Licenciado: vende 5 % más caro. La Doña: sus clientes traen 10 % más. Junior: $20,000 extra. Don Chuy: sus autos valen 10 % más al final.",
            "Cada día puede pasar algo: terremoto, huracán, Black Friday, Navidad, Hot Sale, zombies o que no llegue nadie. También llega El Bro o un señor que te vende su coche. MENÚ o Esc para pausa, revancha o rendirse."
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
