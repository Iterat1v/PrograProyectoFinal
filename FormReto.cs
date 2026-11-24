namespace PrograProyectoFinal
{
    // Reto del día: misma partida para todos hoy. Solo cuenta tu mejor resultado.
    public class FormReto : FormPixel
    {
        private TextBox txtNombre;
        private BotonPixel btnVendedor;
        private int vendedor = 0;
        private string aviso = "";
        private string[] nombres = new string[Reto.TOP];
        private double[] puntos = new double[Reto.TOP];
        private int cuantos;

        public FormReto()
        {
            Titulo = $"RETO DEL DÍA · {DateTime.Today:dd/MM/yyyy}";
            BotonRegresar((s, e) => IrA(new FormModo()));
            txtNombre = new TextBox
            {
                BorderStyle = BorderStyle.None, BackColor = Paleta.Crema, ForeColor = Paleta.Noche,
                Location = new Point(690, 506), Size = new Size(260, 40), MaxLength = 12, Font = Recursos.Letra(15),
                Text = Perfiles.NombreActivo
            };
            txtNombre.TextChanged += (s, e) => Invalidate();
            Controls.Add(txtNombre);
            btnVendedor = Boton("", 970, 494, 250, 60, Paleta.LilaClaro, Paleta.Crema2, (s, e) =>
            {
                vendedor = (vendedor + 1) % 4;
                btnVendedor.Text = Recursos.NombresVendedor[vendedor].ToUpper();
                Invalidate();
            }, 10);
            btnVendedor.Text = Recursos.NombresVendedor[vendedor].ToUpper();
            Boton("¡JUGAR EL RETO!", 690, 612, 550, 90, Paleta.Verde, Paleta.Verde2, Jugar, 18);
            cuantos = Reto.MejoresDeHoy(nombres, puntos);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            int zona = Reto.Zona;

            // ----- el reto de hoy -----
            Pixel.Panel(g, 40, 76, 620, 626, Paleta.Crema);
            Pixel.Texto(g, $"HOY SE JUEGA EN {Partida.Zonas[zona]}", 350, 110, 15, Paleta.Morado2, StringAlignment.Center);
            Pixel.R(g, 64, 134, 572, 212, Paleta.Negro);
            Escenario.Dibujar(g, zona, new Rectangle(70, 140, 560, 200));
            Pixel.Parrafo(g, "A todos los que juegan hoy les tocan las mismas cartas, los mismos autos chocados y los mismos eventos. " +
                             "Mañana cambia. Solo cuenta tu mejor partida del día.",
                          new Rectangle(66, 356, 570, 100), 10, Paleta.Noche, StringAlignment.Center);
            Pixel.Texto(g, "TUS RIVALES", 66, 470, 12, Paleta.Rosa2);
            for (int i = 0; i < Reto.Rivales.Length; i++)
            {
                int x = 66 + i * 290;
                Pixel.R(g, x, 494, 92, 92, Paleta.Negro);
                Pixel.R(g, x + 4, 498, 84, 84, Paleta.Morado3);
                Pixel.Sprite(g, Recursos.Vendedores[(vendedor + 1 + i) % 4], x + 10, 504, 4.5f);
                Pixel.Texto(g, Reto.Rivales[i], x + 104, 522, 12, Paleta.Noche);
                Pixel.Texto(g, $"bot {Bot.NombresNivel[Reto.NivelRivales[i]]}", x + 104, 554, 9, Paleta.Gris);
            }
            Pixel.TextoAjustado(g, "Premio: racha diaria (+20 monedas por día seguido, hasta 7).", 66, 624, 10, Paleta.Verde2, StringAlignment.Near, 570);
            Pixel.Texto(g, "Cuenta para la misión \"Juega el reto del día\".", 66, 656, 10, Paleta.Gris);

            // ----- los mejores de hoy -----
            Pixel.Panel(g, 680, 76, 560, 380, Paleta.Crema);
            Pixel.Texto(g, "LOS MEJORES DE HOY", 960, 110, 15, Paleta.Morado2, StringAlignment.Center);
            if (cuantos == 0)
                Pixel.Parrafo(g, "Nadie ha jugado el reto hoy. ¡Sé el primero!", new Rectangle(710, 170, 500, 80), 12, Paleta.Gris, StringAlignment.Center);
            for (int i = 0; i < cuantos; i++)
            {
                int y = 146 + i * 58;
                Pixel.R(g, 704, y, 512, 50, i == 0 ? Paleta.OroClaro : Paleta.LilaClaro);
                Pixel.Texto(g, $"{i + 1}", 726, y + 25, 15, Paleta.Morado2);
                Pixel.Texto(g, nombres[i], 770, y + 25, 13, Paleta.Noche);
                Pixel.Texto(g, Pixel.Dinero(puntos[i]), 1200, y + 25, 12, Paleta.Verde2, StringAlignment.Far);
            }

            // ----- tu nombre -----
            Pixel.Panel(g, 680, 474, 560, 120, Paleta.Crema);
            if (txtNombre.Text == "") Pixel.Texto(g, "Tu nombre...", 690, 526, 13, Paleta.Gris);
            Perfil p = txtNombre.Text.Trim() == "" ? null : Perfiles.Buscar(txtNombre.Text.Trim());
            string info = p == null ? "Escribe tu nombre y tu vendedor."
                        : p.UltimoReto == Reto.Hoy ? $"Racha: {p.Racha} días. Ya jugaste hoy: puedes mejorar tu marca."
                        : p.UltimoReto == Reto.Ayer ? $"Racha: {p.Racha} días. ¡Juega hoy para no perderla!"
                        : "Empieza tu racha hoy.";
            Pixel.Texto(g, aviso != "" ? aviso : info, 700, 574, 9, aviso != "" ? Paleta.Rosa : Paleta.Morado2);
        }

        private void Jugar(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim().Replace(";", ",");
            if (nombre == "")
            {
                aviso = "Primero escribe tu nombre.";
                Recursos.Sonar("error");
                Invalidate();
                return;
            }
            IrA(new FormDraft(Reto.CrearPartida(nombre, vendedor)));
        }
    }
}
