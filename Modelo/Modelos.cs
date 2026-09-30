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

        public const int POR_TIPO = 22;
        public static int Cantidad => lista.GetLength(0);

        // Un auto nuevo al azar del tipo que se pide
        public static Auto Nuevo(Random azar, int tipo)
        {
            int m = tipo * POR_TIPO + azar.Next(POR_TIPO);
            string marca = (string)lista[m, 0], modelo = (string)lista[m, 1];
            int nuevo = (int)lista[m, 2], viejo = (int)lista[m, 4];
            double base_ = (int)lista[m, 3];

            int anio = viejo + azar.Next(nuevo - viejo + 1);
            int edad = Math.Max(1, ANIO_ACTUAL - anio);
            int km = edad * (8000 + azar.Next(14000)) + azar.Next(5000);

            // Precio: se deprecia por año y se ajusta si tiene muchos o pocos kilómetros
            double precio = base_ * ESCALA * Math.Pow(DEPRECIACION, nuevo - anio);
            double esperado = 15000.0 * edad;
            double ajuste = 1 - (km - esperado) / esperado * 0.12;
            precio *= Math.Max(0.88, Math.Min(1.10, ajuste));
            precio = Math.Max(25000, Math.Min(400000, Math.Round(precio / 1000) * 1000));

            Auto auto = new Auto(marca, modelo, anio, precio, tipo, azar.Next(Cliente.NombresColor.Length));
            auto.Km = km;
            if (azar.Next(100) < 15) auto.Estado = Auto.CHOCADO;               // se nota a simple vista
            if (azar.Next(100) < 25) auto.Falla = azar.Next(Auto.NombresFalla.Length);   // escondida
            return auto;
        }
    }
}
