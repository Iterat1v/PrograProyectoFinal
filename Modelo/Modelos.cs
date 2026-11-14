namespace PrograProyectoFinal
{
    // Los 66 modelos que pueden llegar al mercado (22 por tipo), con los años en que se vendieron en México.
    // Cada mañana llegan autos nuevos: se escoge un modelo al azar, un año, el kilometraje
    // y, a veces, una falla escondida. El precio sale del año y los kilómetros.
    internal static class Modelos
    {
        public const int ANIO_ACTUAL = 2026;
        private const double ESCALA = 0.6;              // la economía del juego es más chica que la real
        private const double DEPRECIACION = 0.93;       // pierde 7 % por cada año de antigüedad

        // { marca, modelo, año más nuevo, precio de ese año, año más viejo }
        private static readonly object[,] lista =
        {
            // ----- sedanes (tipo 0) -----
            { "Nissan", "Tsuru", 2017, 95000, 2004 }, { "Volkswagen", "Jetta", 2023, 380000, 2008 },
            { "Nissan", "Sentra", 2023, 390000, 2008 }, { "Chevrolet", "Aveo", 2023, 240000, 2010 },
            { "Nissan", "Versa", 2024, 300000, 2012 }, { "Toyota", "Corolla", 2024, 420000, 2008 },
            { "Honda", "Civic", 2024, 450000, 2008 }, { "Mazda", "Mazda 3", 2024, 430000, 2010 },
            { "Volkswagen", "Vento", 2022, 230000, 2014 }, { "Hyundai", "Accent", 2023, 250000, 2013 },
            { "Kia", "Rio", 2024, 300000, 2016 }, { "Toyota", "Yaris", 2023, 280000, 2016 },
            { "Chevrolet", "Cavalier", 2023, 290000, 2018 }, { "Ford", "Fiesta", 2019, 170000, 2011 },
            { "Dodge", "Attitude", 2023, 250000, 2015 }, { "Honda", "City", 2023, 320000, 2014 },
            { "Seat", "Toledo", 2019, 200000, 2013 }, { "Renault", "Logan", 2022, 200000, 2016 },
            { "Nissan", "Altima", 2020, 380000, 2008 }, { "Hyundai", "Elantra", 2023, 380000, 2012 },
            { "Chevrolet", "Malibu", 2020, 330000, 2012 }, { "Kia", "Forte", 2024, 380000, 2018 },
            // ----- compactos (tipo 1) -----
            { "Volkswagen", "Vocho", 2003, 70000, 1992 }, { "Chevrolet", "Chevy", 2012, 90000, 2004 },
            { "Nissan", "March", 2023, 230000, 2012 }, { "Chevrolet", "Spark", 2022, 200000, 2012 },
            { "Dodge", "Atos", 2012, 80000, 2006 }, { "Volkswagen", "Beetle", 2019, 330000, 2012 },
            { "Mini", "Cooper", 2022, 520000, 2010 }, { "Fiat", "500", 2019, 260000, 2012 },
            { "Hyundai", "Grand i10", 2023, 240000, 2015 }, { "Renault", "Kwid", 2024, 220000, 2019 },
            { "Volkswagen", "Gol", 2023, 220000, 2009 }, { "Volkswagen", "Polo", 2022, 260000, 2011 },
            { "Chevrolet", "Beat", 2021, 200000, 2018 }, { "Ford", "Ka", 2020, 190000, 2015 },
            { "Toyota", "Etios", 2019, 200000, 2014 }, { "Suzuki", "Swift", 2024, 300000, 2012 },
            { "Kia", "Picanto", 2023, 230000, 2018 }, { "Renault", "Clio", 2019, 180000, 2010 },
            { "Seat", "Ibiza", 2021, 290000, 2010 }, { "Honda", "Fit", 2020, 300000, 2009 },
            { "Mazda", "Mazda 2", 2023, 300000, 2016 }, { "Peugeot", "208", 2024, 340000, 2013 },
            // ----- pickups (tipo 2) -----
            { "Toyota", "Hilux", 2024, 560000, 2008 }, { "Ford", "Lobo", 2023, 950000, 2008 },
            { "Toyota", "Tacoma", 2024, 850000, 2008 }, { "Ford", "Ranger", 2023, 600000, 2010 },
            { "Nissan", "NP300", 2024, 480000, 2008 }, { "Mitsubishi", "L200", 2024, 520000, 2010 },
            { "RAM", "RAM 700", 2024, 330000, 2015 }, { "Chevrolet", "Tornado", 2022, 300000, 2011 },
            { "Nissan", "Frontier", 2024, 620000, 2010 }, { "Chevrolet", "Silverado", 2023, 900000, 2006 },
            { "Chevrolet", "S10", 2024, 470000, 2019 }, { "Ford", "F-150", 2023, 900000, 2008 },
            { "RAM", "RAM 1500", 2023, 950000, 2010 }, { "Chevrolet", "Colorado", 2023, 700000, 2012 },
            { "Volkswagen", "Saveiro", 2023, 300000, 2011 }, { "Toyota", "Tundra", 2023, 1100000, 2008 },
            { "GMC", "Sierra", 2023, 1000000, 2008 }, { "Isuzu", "D-Max", 2023, 500000, 2015 },
            { "Mazda", "BT-50", 2023, 550000, 2012 }, { "Ford", "Maverick", 2024, 600000, 2022 },
            { "Jeep", "Gladiator", 2024, 1100000, 2020 }, { "Dodge", "Dakota", 2011, 220000, 2005 }
        };


        // Autos de lujo: llegan sobre todo a Polanco y Santa Fe. Mismo formato que la lista de arriba, 12 por tipo.
        // (precios nuevos reales en pesos; abajo se escalan igual que los demas)
        private static readonly object[,] premium =
        {
            // ----- sedanes de lujo (tipo 0) -----
            { "BMW", "Serie 3", 2024, 950000, 2012 }, { "Mercedes", "Clase C", 2024, 1050000, 2012 },
            { "Audi", "A4", 2024, 980000, 2012 }, { "Tesla", "Model 3", 2024, 950000, 2019 },
            { "Porsche", "Panamera", 2023, 2800000, 2012 }, { "Lexus", "ES", 2023, 1100000, 2013 },
            { "Mercedes", "Clase E", 2023, 1300000, 2010 }, { "BMW", "Serie 5", 2023, 1250000, 2011 },
            { "Audi", "A6", 2023, 1250000, 2011 }, { "Jaguar", "XE", 2020, 900000, 2016 },
            { "Volvo", "S60", 2023, 900000, 2012 }, { "Tesla", "Model S", 2023, 1900000, 2013 },
            // ----- compactos de lujo (tipo 1) -----
            { "Mini", "Cooper S", 2024, 650000, 2012 }, { "Audi", "A1", 2020, 450000, 2012 },
            { "BMW", "Serie 1", 2023, 700000, 2012 }, { "Mercedes", "Clase A", 2024, 750000, 2014 },
            { "Audi", "A3", 2024, 750000, 2011 }, { "Porsche", "911", 2023, 3200000, 2012 },
            { "BMW", "i3", 2020, 800000, 2015 }, { "Smart", "Fortwo", 2019, 350000, 2012 },
            { "Alfa Romeo", "Giulietta", 2019, 600000, 2012 }, { "Audi", "TT", 2022, 1000000, 2008 },
            { "Tesla", "Model Y", 2024, 1100000, 2021 }, { "Lexus", "CT", 2017, 450000, 2012 },
            // ----- camionetas y pickups de lujo (tipo 2) -----
            { "Mercedes", "Clase G", 2024, 3200000, 2012 }, { "Land Rover", "Range Rover", 2024, 2300000, 2012 },
            { "Porsche", "Cayenne", 2024, 2000000, 2010 }, { "BMW", "X5", 2024, 1600000, 2010 },
            { "Audi", "Q7", 2024, 1500000, 2010 }, { "Tesla", "Cybertruck", 2024, 1900000, 2024 },
            { "Ford", "F-150 Raptor", 2023, 1500000, 2012 }, { "RAM", "2500 Limited", 2023, 1600000, 2012 },
            { "Cadillac", "Escalade", 2023, 2000000, 2010 }, { "Lincoln", "Navigator", 2023, 1900000, 2012 },
            { "Porsche", "Macan", 2024, 1500000, 2015 }, { "Mercedes", "GLE", 2024, 1700000, 2012 }
        };

        // De cada 100 autos que llegan, cuantos son de lujo segun la zona (Ecatepec ... Polanco)
        public static readonly int[] PorcentajeLujo = { 0, 3, 18, 50, 85 };
        private const int TOPE_NORMAL = 400000;
        private const int TOPE_LUJO = 1500000;

        public const int POR_TIPO = 22;
        public static int Cantidad => lista.GetLength(0);

        // Un auto nuevo al azar del tipo que se pide (sin zona = como antes)
        public static Auto Nuevo(Random azar, int tipo)
        {
            return Nuevo(azar, tipo, -1);
        }

        // Un auto nuevo segun la zona: en Polanco puro lujo (BMW, Mercedes, Tesla, Porsche...), en Ecatepec y Neza
        // carros sencillos y baratos (Tsuru, Aveo, Jetta, Chevy...)
        public static Auto Nuevo(Random azar, int tipo, int zona)
        {
            object[,] fuente = lista;
            int POR = POR_TIPO;
            bool lujo = zona >= 0 && azar.Next(100) < PorcentajeLujo[zona];
            if (lujo) { fuente = premium; POR = 12; }
            int m = tipo * POR + azar.Next(POR);
            // en las zonas pobres se prefiere el mas barato de dos al azar; en las ricas, el mas caro
            if (!lujo && zona >= 0 && (zona <= 1 || zona >= 3))
            {
                int m2 = tipo * POR + azar.Next(POR);
                bool masBarato = zona <= 1;
                if (((int)fuente[m2, 3] < (int)fuente[m, 3]) == masBarato) m = m2;
            }
            string marca = (string)fuente[m, 0], modelo = (string)fuente[m, 1];
            int nuevo = (int)fuente[m, 2], viejo = (int)fuente[m, 4];
            double base_ = (int)fuente[m, 3];

            int anio = viejo + azar.Next(nuevo - viejo + 1);
            int edad = Math.Max(1, ANIO_ACTUAL - anio);
            int km = edad * (8000 + azar.Next(14000)) + azar.Next(5000);

            // Precio: se deprecia por año y se ajusta si tiene muchos o pocos kilómetros
            double precio = base_ * ESCALA * Math.Pow(DEPRECIACION, nuevo - anio);
            double esperado = 15000.0 * edad;
            double ajuste = 1 - (km - esperado) / esperado * 0.12;
            precio *= Math.Max(0.88, Math.Min(1.10, ajuste));
            precio = Math.Max(25000, Math.Min(lujo ? TOPE_LUJO : TOPE_NORMAL, Math.Round(precio / 1000) * 1000));

            Auto auto = new Auto(marca, modelo, anio, precio, tipo, azar.Next(Cliente.NombresColor.Length));
            auto.Km = km;
            if (azar.Next(100) < 15) auto.Estado = Auto.CHOCADO;               // se nota a simple vista
            if (azar.Next(100) < 25) auto.Falla = azar.Next(Auto.NombresFalla.Length);   // escondida
            return auto;
        }
    }
}
