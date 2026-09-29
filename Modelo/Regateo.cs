namespace PrograProyectoFinal
{
    // TIPO 1 · REGATEO: te dan ventaja a ti
    internal class Regateo : PowerUp
    {
        public const int DESCUENTO_PROVEEDOR = 0;
        public const int ENCHILAME = 1;
        public const int TOMA_MI_DINERO = 2;
        public const int AMIGO_MERCADO = 3;
        public const int SEMINUEVO = 4;
        public const int REMATE = 5;
        public const int BONO_NAVIDENO = 6;
        public const int EVASION_FISCAL = 7;

        public Regateo(string nombre, string descripcion, int efecto) : base(nombre, descripcion, efecto)
        {
        }

        public override string Tipo => "REGATEO";
        public override Color Color => Paleta.Oro;
        public override int Icono => 0;

        public override string Aplicar(Concesionario usuario, Concesionario rival, out bool usada)
        {
            usada = true;
            switch (efecto)
            {
                case DESCUENTO_PROVEEDOR:
                    usuario.DarDescuento(0.20);
                    return "¡20 % de descuento en tu próxima compra del MERCADO!";
                case AMIGO_MERCADO:
                    usuario.DarDescuento(0.50);
                    return "Tu compa del mercado te deja el siguiente auto a mitad de precio.";
                case REMATE:
                    usuario.DarDescuento(0.25);
                    return "¡Remate! 25 % menos en el próximo auto que compres.";
                case ENCHILAME:
                    usuario.BonoVenta += 50000;
                    return "Le enchulaste la lámina: +$50,000 en tu próxima venta.";
                case SEMINUEVO:
                    usuario.BonoVenta += 30000;
                    return "Le bajaste el kilometraje: +$30,000 en tu próxima venta.";
                case TOMA_MI_DINERO:
                    usuario.VentaSegura = true;
                    return "¡Toma mi dinero! Este cliente te compra el auto que le enseñes.";
                case BONO_NAVIDENO:
                    usuario.Dinero += 25000;
                    return "¡Bono navideño! +$25,000.";
                default:
                    usuario.Dinero += 75000;
                    return "Evasión fiscal... +$75,000. Nadie vio nada.";
            }
        }
    }
}
