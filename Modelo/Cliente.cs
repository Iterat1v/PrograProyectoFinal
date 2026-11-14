namespace PrograProyectoFinal
{
    // El cliente que llega en cada turno
    internal class Cliente
    {
        public static readonly string[] NombresColor = { "rojo", "aqua", "amarillo", "azul", "rosa", "blanco", "verde", "gris" };

        // La frase que siempre dice cada cliente (retrato 0 a 8)
        public static readonly string[] Muletillas =
        {
            "¿Y el Uber qué, joven?", "Ay mijo...", "Nel, carnal.", "Literal.",
            "Según mis cálculos...", "¡Ah, chirrión!", "¿Neta?", "Eso viene en el examen.", "Arre, carnal."
        };

        private string nombre;
        private double presupuesto;
        private int tipoBuscado;
        private string frase;
        private int retrato;
        private double factorPago = 1;      // El Bro paga más
        private int colorBuscado = -1;      // -1 = le da igual el color
        private int colorFavorito = -1;
        private int colorOdiado = -1;
        private double factorRecuerdo = 1;  // si ya te compró antes: le fue bien o le fue mal
        private int estilo = -1;            // como habla (Habla.*); -1 = como antes
        private string saludoEstilo = "";   // lo que dice al llegar segun su estilo, escogido una sola vez

        public Cliente(string nombre, double presupuesto, int tipoBuscado, string frase, int retrato, double factorPago)
            : this(nombre, presupuesto, tipoBuscado, frase, retrato)
        {
            this.factorPago = factorPago;
        }

        public Cliente(string nombre, double presupuesto, int tipoBuscado, string frase, int retrato)
        {
            this.nombre = nombre;
            Presupuesto = presupuesto;
            this.tipoBuscado = tipoBuscado;
            this.frase = frase;
            this.retrato = retrato;
        }

        public string Nombre => nombre;
        public int TipoBuscado => tipoBuscado;
        public int Retrato => retrato;
        public double FactorPago => factorPago;
        public bool AceptaCualquiera => tipoBuscado == -1;   // -1 = le da igual el tipo
        public string Muletilla => Muletillas[retrato];

        public int ColorBuscado { get => colorBuscado; set => colorBuscado = value; }
        public int Estilo => estilo;
        public string NombreEstilo => estilo < 0 ? "" : Habla.Nombres[estilo];

        // Le da su forma de hablar (zona o visitante) y escoge UNA vez como saluda
        public void Hablar(int estiloNuevo, Random azar)
        {
            estilo = estiloNuevo;
            saludoEstilo = azar.Next(100) < 60 ? Habla.Saludo(estilo, azar) + " " : "";
        }

        // Lo que dice mientras espera y se va impacientando (lo usa el tablero con el reloj)
        public string FraseImpaciente(Random azar) => Habla.Impaciente(estilo < 0 ? 0 : estilo, azar);

        // Lo que dice cuando se harta y se va
        public string FraseEnojado(Random azar) => Habla.Enojado(estilo < 0 ? 0 : estilo, azar);
        public double FactorRecuerdo { get => factorRecuerdo; set => factorRecuerdo = value; }

        public void Gustos(int favorito, int odiado)
        {
            colorFavorito = favorito;
            colorOdiado = odiado;
        }

        public double Presupuesto
        {
            get => presupuesto;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("El presupuesto no puede ser negativo");
                else
                    presupuesto = value;
            }
        }

        // Lo que opina del color del auto: le sube o le baja lo que paga
        public double FactorColor(Auto auto)
        {
            if (auto.Color == colorFavorito) return 1.10;
            if (auto.Color == colorOdiado) return 0.85;
            return 1;
        }

        public string OpinionColor(Auto auto)
        {
            string c = NombresColor[auto.Color];
            if (auto.Color == colorFavorito) return $"¡Uy, {c}! Justo el color que quería.";
            if (auto.Color == colorOdiado) return $"¿{char.ToUpper(c[0])}{c.Substring(1)}? Ni loco... bueno, a ver cuánto.";
            return "";
        }

        public string OpinionEstado(Auto auto)
        {
            if (auto.Estado == Auto.CHOCADO) return "Oiga, ese está todo madreado.";
            if (auto.Estado == Auto.TUNEADO) return "¡Qué chulada! ¿Le pusiste rines?";
            return "";
        }

        public const int ACEPTA = 0;
        public const int CONTRAOFERTA = 1;

        // Lo máximo que pagaría por ese auto (el bot nivel Dios lo "adivina")
        public double Limite(Auto auto, bool licenciado, bool ventaSegura)
        {
            double limite;
            if (AceptaCualquiera || ventaSegura)
                limite = auto.ValorActual() * 1.6;          // El Bro o "¡¡Toma mi dinero!!"
            else
            {
                limite = presupuesto;
                if (licenciado) limite *= 1.05;             // el Licenciado convence mejor
                if (auto.Tipo != tipoBuscado) limite *= 0.60;   // no es lo que busca: solo muy barato
                if (auto.Estado == Auto.CHOCADO) limite *= 0.50;
                if (auto.Estado == Auto.TUNEADO) limite *= 1.25;
                limite *= FactorColor(auto) * factorRecuerdo;
            }
            return Math.Floor(limite / 1000) * 1000;
        }

        // El jugador pide un precio y el cliente decide.
        // Si no acepta, "contraoferta" es lo máximo que daría.
        public int Evaluar(Auto auto, double precio, bool licenciado, bool ventaSegura, Random azar,
                           out string respuesta, out double contraoferta)
        {
            bool otroTipo = !AceptaCualquiera && auto.Tipo != tipoBuscado;
            double limite = Limite(auto, licenciado, ventaSegura);
            contraoferta = limite;
            bool conEstilo = estilo >= 0;
            string muletilla = !conEstilo && azar.Next(2) == 0 ? Muletilla + " " : "";

            if (precio <= limite)
            {
                if (conEstilo) respuesta = Habla.Acepta(estilo, azar);
                else respuesta = otroTipo ? "No es lo que buscaba... pero a ese precio me lo llevo."
                                          : $"{muletilla}¡Trato hecho! Me lo llevo.";
                return ACEPTA;
            }
            if (precio <= limite * 1.15 && azar.Next(100) < 40)
            {
                respuesta = conEstilo ? Habla.Acepta(estilo, azar) : "Mmm... ándale pues, me convenciste.";
                return ACEPTA;
            }

            if (otroTipo)
                respuesta = conEstilo ? Habla.OtroTipo(estilo, Recursos.Tipos[tipoBuscado].ToLower(), limite, azar)
                                      : $"Yo busco un {Recursos.Tipos[tipoBuscado].ToLower()}... pero si me lo dejas en ${limite:N0} lo pienso.";
            else if (precio > limite * 1.4)
                respuesta = conEstilo ? Habla.Loco(estilo, limite, azar) : $"{muletilla}¡Estás loco! Te doy ${limite:N0} y ya.";
            else
                respuesta = conEstilo ? Habla.Contra(estilo, limite, azar) : $"{muletilla}Está caro. Te doy ${limite:N0}.";
            return CONTRAOFERTA;
        }

        // Lo que dice el cliente al llegar
        public string Saludo
        {
            get
            {
                if (AceptaCualquiera) return saludoEstilo + frase;
                string color = colorBuscado == -1 ? "" : $" Y lo quiero {NombresColor[colorBuscado]}.";
                string cuerpo = $"{frase} Traigo ${presupuesto:N0}.{color}";
                // si ya viene largo (historia + recuerdo), se quita el saludo de relleno para que no se corte
                return saludoEstilo.Length + cuerpo.Length > 150 ? cuerpo : saludoEstilo + cuerpo;
            }
        }
    }
}
