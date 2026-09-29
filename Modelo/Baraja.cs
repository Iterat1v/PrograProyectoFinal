namespace PrograProyectoFinal
{
    // Las 25 ABILICARDS en un arreglo: 10 Regateo, 8 Estafa y 7 Aseguradora
    internal class Baraja
    {
        private static Random azar = new Random();

        private PowerUp[] cartas = new PowerUp[25];
        private int siguiente = 0;

        public Baraja()
        {
            int i = 0;
            cartas[i++] = new Regateo("Descuento del proveedor", "20 % de descuento en tu próxima compra.", Regateo.DESCUENTO_PROVEEDOR);
            cartas[i++] = new Regateo("Enchílame la lámina", "Reparas un auto y ganas +$50,000 al venderlo.", Regateo.ENCHILAME);
            for (int k = 0; k < 3; k++)
                cartas[i++] = new Regateo("¡¡Toma mi dinero!!", "Regateas con el cliente y aseguras la venta.", Regateo.TOMA_MI_DINERO);
            cartas[i++] = new Regateo("Amigo del mercado", "50 % menos en la compra que elijas.", Regateo.AMIGO_MERCADO);
            cartas[i++] = new Regateo("Seminuevo", "Le bajas el kilometraje a un coche: +$30,000 en tu próxima venta.", Regateo.SEMINUEVO);
            cartas[i++] = new Regateo("Remate", "25 % menos en el próximo auto que compres.", Regateo.REMATE);
            cartas[i++] = new Regateo("Bono navideño", "+$25,000 al instante.", Regateo.BONO_NAVIDENO);
            cartas[i++] = new Regateo("Evasión fiscal", "+$75,000 al instante.", Regateo.EVASION_FISCAL);

            cartas[i++] = new Estafa("Ya te cayó la ley", "Un rival pierde $30,000 por deudas al SAT.", Estafa.YA_TE_CAYO_LA_LEY);
            cartas[i++] = new Estafa("¿Por qué tan caro?", "Un rival asusta a su cliente y pierde 1 turno.", Estafa.POR_QUE_TAN_CARO);
            cartas[i++] = new Estafa("Manipulación del mercado", "Los precios de compra de un rival suben 20 % durante 1 turno.", Estafa.MANIPULACION);
            cartas[i++] = new Estafa("Error de cuentas", "Un rival regatea mal y tiene -20 % en su próxima venta.", Estafa.ERROR_DE_CUENTAS);
            for (int k = 0; k < 2; k++)
                cartas[i++] = new Estafa("¡¡Matanga!!", "Roba un coche de un rival.", Estafa.MATANGA);
            for (int k = 0; k < 2; k++)
                cartas[i++] = new Estafa("Combustión espontánea", "Destruye un auto de un rival.", Estafa.COMBUSTION);

            for (int k = 0; k < 3; k++)
                cartas[i++] = new Aseguradora("Seguro comercial", "Bloquea 1 carta de Estafa.", Aseguradora.SEGURO_COMERCIAL);
            for (int k = 0; k < 2; k++)
                cartas[i++] = new Aseguradora("Seguro contra estafas", "Evita que te suban los precios del mercado.", Aseguradora.CONTRA_ESTAFAS);
            for (int k = 0; k < 2; k++)
                cartas[i++] = new Aseguradora("Colchoncito", "Evita una pérdida de hasta $50,000.", Aseguradora.COLCHONCITO);
        }

        public int Cantidad => cartas.Length;

        public PowerUp ObtenerCarta(int indice)
        {
            return cartas[indice];
        }

        // Algoritmo de Fisher-Yates: recorre de atrás hacia adelante
        // e intercambia cada carta con una al azar de las que faltan
        public void Barajar()
        {
            for (int i = cartas.Length - 1; i > 0; i--)
            {
                int j = azar.Next(i + 1);
                PowerUp temporal = cartas[i];
                cartas[i] = cartas[j];
                cartas[j] = temporal;
            }
            siguiente = 0;
        }

        public PowerUp Sacar()
        {
            PowerUp carta = cartas[siguiente];
            siguiente++;
            return carta;
        }
    }
}
