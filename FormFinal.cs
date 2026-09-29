namespace PrograProyectoFinal
{
    // El final del modo historia: cambia según tus decisiones y tu reputación
    public class FormFinal : FormPixel
    {
        private Historia historia;
        private string[] final;
        private Image cara;

        internal FormFinal(Historia historia)
        {
            this.historia = historia;
            final = historia.Final(historia.Reputacion);
            Perfil p = Perfiles.Buscar(historia.Nombre);
            cara = p != null && p.Personalizado ? p.Avatar.Dibujar() : Recursos.Vendedores[historia.Vendedor];
            Boton("VER EL MAPA", 300, 628, 320, 72, Paleta.Agua, Paleta.Agua2, (s, e) => IrA(new FormHistoria(historia)));
            Boton("MENÚ PRINCIPAL", 660, 628, 320, 72, Paleta.Rosa, Paleta.Rosa2, (s, e) => VolverAlTitulo());
            Recursos.Sonar("victoria");
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pixel.Preparar(g);
            Pixel.R(g, 0, 0, Pixel.ANCHO, Pixel.ALTO, Paleta.Noche);
            Pixel.Estrellas(g, 60, 5);
            Pixel.SombraTexto(g, "FINAL", 640, 56, 48, Paleta.Oro, Paleta.Rosa);
            Pixel.Texto(g, final[0], 640, 128, 18, Paleta.Agua, StringAlignment.Center);

            // el camino de la ciudad, con líneas rectas, y tú en Polanco
            for (int i = 0; i < Historia.SEMANAS; i++)
            {
                Rectangle r = new Rectangle(90 + i * 230, 180, 180, 110);
                if (i > 0) Pixel.R(g, r.X - 50, r.Y + 51, 50, 8, Paleta.Oro);
                Pixel.R(g, r.X - 5, r.Y - 5, r.Width + 10, r.Height + 10, Paleta.Oro);
                Escenario.Dibujar(g, i, r);
                Pixel.Texto(g, Partida.Zonas[i], r.X + 90, r.Bottom + 22, 9, Paleta.Crema, StringAlignment.Center);
            }
            Pixel.Sprite(g, cara, 1030, 196, 5);

            Pixel.Panel(g, 140, 350, 1000, 250, Paleta.Crema);
            Pixel.Parrafo(g, final[1], new Rectangle(180, 374, 920, 110), 14, Paleta.Noche, StringAlignment.Center);
            string decisiones = $"Coyote: {(historia.Coyote ? "le compraste" : "no le compraste")}  ·  " +
                                $"Influencer: {(historia.Influencer ? "sí" : "no")}  ·  " +
                                $"Mordida al SAT: {(historia.Mordida ? "sí" : "no")}  ·  Reputación: {historia.Reputacion}";
            Pixel.Texto(g, "TUS DECISIONES", 640, 512, 11, Paleta.Rosa2, StringAlignment.Center);
            Pixel.Texto(g, decisiones, 640, 552, 10, Paleta.Morado2, StringAlignment.Center);
        }
    }
}
