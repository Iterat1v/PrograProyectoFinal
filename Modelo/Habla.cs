namespace PrograProyectoFinal
{
    // Como habla cada cliente. Cada zona de la ciudad tiene su acento y sus modismos, y a veces llega un
    // visitante de otro lado (norteño, yucateco, argentino o gringo). Los textos con {0} llevan el dinero.
    internal static class Habla
    {
        public const int ECATEPEC = 0, NEZA = 1, COYOACAN = 2, SANTA_FE = 3, POLANCO = 4;
        public const int NORTENO = 5, YUCATECO = 6, ARGENTINO = 7, INGLES = 8;
        public const int ESTILOS = 9;

        public static readonly string[] Nombres =
        {
            "Ecatepec", "Neza", "Coyoacán", "Santa Fe", "Polanco", "Norteño", "Yucateco", "Argentino", "Inglés"
        };

        // Cada estilo: [0] saludos, [1] acepta, [2] contraoferta, [3] "estás loco", [4] otro tipo, [5] impaciente, [6] enojado
        private static readonly string[][][] textos =
        {
            // ----- ECATEPEC: barrio, directo -----
            new[]
            {
                new[] { "¡Quihubo, jefe!", "Ora, patrón, ¿qué me tiene?", "Buenas, ¿a cómo la unidad?" },
                new[] { "¡Simón, jefe, me lo llevo!", "¡Chido! Trato hecho, patrón.", "Va, va, ¡órale! Me lo quedo." },
                new[] { "Está bien caro, jefe. Le doy {0}.", "Ay, chale. Ofrezco {0} y ya.", "A ver, a ver... {0} y nos damos la mano." },
                new[] { "¡No manches, jefe! Le doy {0} y ya.", "¡Está bien cañón ese precio! {0} y no más.", "¿Qué le pasa, jefe? {0}, es mi última." },
                new[] { "Yo busco {0}, jefe... pero si me lo deja en {1} lo pienso.", "No es lo que busco, patrón. Con {1} le entro." },
                new[] { "Jefe, ándele, que se me hace tarde.", "¿Ya merito, jefe? Traigo prisa.", "Oiga, ¿me va a atender o qué?" },
                new[] { "¡Ya me cansé, jefe! Me voy a otro lote.", "¡Qué mala onda, tanto tiempo! Ahí nos vemos.", "¡No! Tardó mucho, mejor me voy." }
            },
            // ----- NEZA: barrio, con "raza" y "carnal" -----
            new[]
            {
                new[] { "¡Qué onda, raza!", "Ey, carnal, ¿qué hay de chido?", "¡Quiúbole! Vengo por un cochecito." },
                new[] { "¡Qué buen precio! Me lo llevo, raza.", "¡Simón, carnal, trato hecho!", "¡Órale, va! Me lo quedo." },
                new[] { "Se me hace caro, carnal. Te doy {0}.", "Ay, raza, no exageres. {0} y es mucho.", "Mira, carnalito, {0} y cerramos." },
                new[] { "¡Chale, carnal, no te pases! {0}.", "¡Ni de loco! Te doy {0} y ya, raza.", "Estás bien loquito, ¿no? {0} y nada más." },
                new[] { "Yo andaba buscando {0}, carnal... pero a {1} sí le entro.", "Ese no es lo que quiero, raza. Dámelo en {1}." },
                new[] { "Carnal, ándale, que la raza me espera.", "¿Ya mero, carnal? Se me va el micro.", "Hey, ¿me vas a atender o cómo?" },
                new[] { "¡Ya me ganó la tarde, carnal! Me voy a otro lado.", "¡Qué gacho, tanto rato! Ahí te ves, raza.", "¡Chale! Tardaste un chorro, me voy." }
            },
            // ----- COYOACÁN: bohemio, de café y plaza -----
            new[]
            {
                new[] { "Hola, ¿qué tal? Vengo del mercado de artesanías.", "Buen día. Busco un coche con buena vibra.", "Neta, qué bonito el lote. ¿Qué me recomiendas?" },
                new[] { "Me late. Es energía pura, me lo llevo.", "Perfecto, tiene buena vibra. Trato hecho.", "Me convenciste. El universo así lo quiere." },
                new[] { "Pues mira, está algo caro. Te doy {0}, ¿va?", "Con todo respeto, ofrezco {0}.", "Hmm... digamos {0} y nos tomamos un café." },
                new[] { "¡Güey, no! Eso es un abuso. {0} y ya.", "Qué fuerte precio, neta. {0} es lo justo.", "Eso es capitalismo salvaje. Doy {0}." },
                new[] { "Buscaba {0}, pero ya que estoy aquí... en {1} lo veo.", "No es lo que traía en mente. Con {1} lo pienso." },
                new[] { "Oye, tengo un taller de cerámica en una hora...", "¿Todo bien? Ya llevo un rato esperando.", "Mi tiempo también vale, ¿eh?" },
                new[] { "Qué mala onda, tanto tiempo. Me voy a otro lote.", "Esto me desequilibró el chakra. Adiós.", "Ya, neta, no. Me voy, ¿eh?" }
            },
            // ----- SANTA FE: corporativo, mezcla español e inglés -----
            new[]
            {
                new[] { "Hey, bro. I need a car, asap.", "Buenos días. Tengo una junta en 20, let's close this fast.", "Hola. Looking for something premium, ¿qué tienes?" },
                new[] { "Deal! Let's do it, me lo llevo.", "Perfect, we have a deal. Facturen a mi empresa.", "Awesome. Send me the invoice, por favor." },
                new[] { "My budget is {0}. That's my final offer.", "Let's be realistic: {0}, take it or leave it.", "I can do {0}. Win-win, ¿no?" },
                new[] { "That's not even close, bro. {0} and we're done.", "No way. Mi KPI dice {0}, nothing more.", "Seriously? {0}. Final answer." },
                new[] { "I was looking for {0}, pero con {1} lo reviso.", "Not what I need, bro. For {1}, maybe." },
                new[] { "Bro, time is money. ¿Me atiendes?", "I'm on a tight deadline here...", "Tengo un call en cinco minutos, hurry up." },
                new[] { "This is unacceptable. I'm out, bro.", "You wasted my time. Bye.", "Mala experiencia, 1 estrella. Me voy." }
            },
            // ----- POLANCO: fresa, "o sea", "literal" -----
            new[]
            {
                new[] { "Hola, o sea, busco algo súper nice.", "Hiii. Literal, necesito un coche para ya.", "Buenas. ¿Tienes algo que no sea, como, naco?" },
                new[] { "¡Ay, sí! Me encanta, me lo llevo, literal.", "Súper, o sea, perfecto. Trato hecho.", "Me fascina. Charge it to my dad's card." },
                new[] { "O sea, está carísimo. Te doy {0}, ¿ok?", "Ay no, literal es demasiado. {0} y gracias.", "Mmm, como que {0}, ¿no? Es mi límite." },
                new[] { "¿¡Qué!? O sea, no. {0} y ya, ay.", "Eso es literal un robo. {0}, no más.", "Ay, qué naco precio. Te doy {0}." },
                new[] { "Yo buscaba {0}, o sea... pero en {1} lo veo.", "No es lo que quería, literal. Con {1} me convences." },
                new[] { "Ay, o sea, ¿me atiendes? Tengo spa en una hora.", "Literal, llevo años esperando.", "Hello? ¿Hay alguien en este lote?" },
                new[] { "¡O sea, no! Qué mal servicio. Me voy.", "Literal me voy a quejar con tu gerente.", "Ay, qué horror. Bye, no vuelvo." }
            },
            // ----- NORTEÑO -----
            new[]
            {
                new[] { "¡Quiubo, compa! ¿Cómo andamos?", "¡Ándale pues! ¿Qué tiene por aquí, mi buen?", "Buenas, compadre. Vengo de Monterrey por un carro." },
                new[] { "¡Ándale pues, me lo llevo, compa!", "¡Eso! Trato hecho, mi buen.", "¡Eso! Me lo quedo, compa." },
                new[] { "Está muy carrrro, compa. Le doy {0}.", "Pos no, mi buen. {0} y nos echamos unas carnes asadas.", "Mire, compadre, {0} y le invito unas cervezas." },
                new[] { "¡Ora sí que se pasó, compa! {0} y ya.", "¡Ni que fuera de oro! Le doy {0}.", "¡Eso no, mi buen! {0} es lo justo." },
                new[] { "Yo buscaba {0}, compa... pero en {1} le entro.", "No es lo que ando buscando. Con {1} lo pienso, compadre." },
                new[] { "Ándele, compa, que se me hace tarde.", "¿Ya merito, mi buen? Me esperan.", "Oiga, ¿me va a atender o qué?" },
                new[] { "¡Ya estuvo, compa! Me voy a otro lote.", "¡Qué lentitud, mi buen! Ahí nos vemos.", "Pos así no se puede. Me voy, compadre." }
            },
            // ----- YUCATECO: mezcla con maya -----
            new[]
            {
                new[] { "¡Buenas, mi niño! ¿Cómo amaneció?", "¡Qué calor, mi niño! ¿Tiene algún carrito?", "Buenas, vengo desde Mérida por un carro." },
                new[] { "¡Qué bueno, mi niño! Me lo llevo.", "¡Está bien! Trato hecho, mi niño.", "Me gusta mucho, mi niño. Me lo quedo." },
                new[] { "Está caro, mi niño. Le doy {0}.", "Ay, no tanto. {0} y listo.", "A ver, a ver... {0}, mi niño." },
                new[] { "¡Ni que fuera de oro, mi niño! {0} y ya.", "¡Eso es muchísimo, mi niño! Le doy {0}.", "Eso no, mi niño. {0}, no más." },
                new[] { "Yo buscaba {0}, mi niño... pero en {1} lo veo.", "No es el que quiero. Con {1}, quién sabe." },
                new[] { "Mi niño, ándele, que hace calor.", "¿Ya merito? Me estoy derritiendo.", "Oiga, ¿me va a atender?" },
                new[] { "¡Ya me cansé, mi niño! Me voy a otro lado.", "¡Tanto tiempo! Me voy, ¿eh?", "¡Con este calor y tanta espera! Me voy, mi niño." }
            },
            // ----- ARGENTINO -----
            new[]
            {
                new[] { "¡Che, qué tal! Estoy buscando un auto.", "¡Hola, boludo! ¿Qué tenés para mostrarme?", "Buen día, che. Vengo por un coche." },
                new[] { "¡Dale! Me lo llevo, che.", "¡Joya! Trato hecho, ¿viste?", "¡Bárbaro! Me lo quedo." },
                new[] { "Está re caro, che. Te doy {0}.", "Pará, pará. {0} y listo, ¿dale?", "Mirá, {0} es lo que puedo, ¿viste?" },
                new[] { "¡Estás loco, che! {0} y chau.", "¡Ni en pedo! Te doy {0}.", "Eso es una locura, boludo. {0}, nada más." },
                new[] { "Buscaba {0}, che... pero con {1} lo pienso.", "No es el que quería. Por {1} capaz que sí." },
                new[] { "Dale, che, que se me hace tarde.", "¿Ya mismo, che? Tengo apuro.", "Che, ¿me vas a atender o no?" },
                new[] { "¡Me harté, che! Me voy a otro lado.", "¡Qué lentitud, boludo! Chau.", "Así no se puede. Me voy, ¿viste?" }
            },
            // ----- INGLÉS (turista) -----
            new[]
            {
                new[] { "Hello, my friend! I need a car, please.", "Hi! I am a tourist. You have a good car?", "Good morning! How much for the cars?" },
                new[] { "Deal! I take it, thank you!", "Perfect! I buy this car, yes!", "Great price! I take it." },
                new[] { "Too expensive! I give you {0}.", "Hmm... {0}. Is good for me.", "How about {0}? Friendly price, yes?" },
                new[] { "You are crazy! {0} and no more!", "No, no, no! {0}, my friend.", "Very expensive! I pay {0}." },
                new[] { "I want {0}, but... for {1} maybe.", "This is not what I look for. For {1}, I think." },
                new[] { "Hello? Please, I wait long time.", "My friend, I have a plane to catch!", "Excuse me, can you help me?" },
                new[] { "I am angry! You waste my time!", "No more! I go to another place. Bye!", "This is bad service! Goodbye!" }
            }
        };

        private static string Pon(string[] opciones, Random azar) => opciones[azar.Next(opciones.Length)];

        private static int E(int estilo) => estilo < 0 || estilo >= ESTILOS ? 0 : estilo;

        // Un visitante de otro lado: norteño, yucateco, argentino o inglés
        public static int Visitante(Random azar) => NORTENO + azar.Next(4);

        public static string Saludo(int estilo, Random azar) => Pon(textos[E(estilo)][0], azar);
        public static string Acepta(int estilo, Random azar) => Pon(textos[E(estilo)][1], azar);
        public static string Contra(int estilo, double limite, Random azar) => string.Format(Pon(textos[E(estilo)][2], azar), "$" + limite.ToString("N0"));
        public static string Loco(int estilo, double limite, Random azar) => string.Format(Pon(textos[E(estilo)][3], azar), "$" + limite.ToString("N0"));

        public static string OtroTipo(int estilo, string tipo, double limite, Random azar)
            => string.Format(Pon(textos[E(estilo)][4], azar), tipo, "$" + limite.ToString("N0"));

        public static string Impaciente(int estilo, Random azar) => Pon(textos[E(estilo)][5], azar);
        public static string Enojado(int estilo, Random azar) => Pon(textos[E(estilo)][6], azar);
    }
}
