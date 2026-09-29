# Concesionario · Notas para las siguientes etapas

Ideas para que el juego fluya mejor y se vea más como juego. Van ordenadas por etapa, con las fechas del Trello.
Los mockups de cómo se vería están en las imágenes que se mandaron al grupo (estilo pixel).

## Etapa 2 · ya programado (rama `etapa2`)

El juego completo corre de principio a fin con el diseño pixel de la presentación:
Título → Registro (nombre + vendedor) → Draft de cartas → Tablero (10 rondas) → Podio.
Además: Cómo se juega, Las cartas (los 3 tipos y las 25) y el Mercado.

**Clases (carpeta `Modelo/`)**, igual que el diagrama UML:

- `Partida`: jugadores `Concesionario[4]`, catálogo `Auto[30]`, ronda, turno, cliente y el draft `PowerUp[4,4]`.
- `Concesionario`: el jugador, su dinero, su lote `Auto[10]` y sus cartas `PowerUp[3]`.
- `Auto`, `Cliente`.
- `PowerUp` (abstracta) y sus hijas `Regateo`, `Estafa` y `Aseguradora`, que sobreescriben `Aplicar()` (herencia y polimorfismo).
- `Baraja`: las 25 cartas en un arreglo, revueltas con Fisher-Yates.

**Algoritmos:** búsqueda lineal (`Partida.BuscarAuto`, `Concesionario.Defender`, nombre repetido en el registro),
burbuja (`Partida.Ranking`), Fisher-Yates (`Baraja.Barajar`), `turno = (turno + 1) % n` y el draft en serpiente.

**Estilo (carpeta `Interfaz/`):** colores (`Paleta`), fuentes, sprites y sonidos (`Recursos`), dibujo pixel (`Pixel`),
botón de juego (`BotonPixel`) y la base de todas las pantallas (`FormPixel`). El arte está en `Assets/`
(sprites de los mockups, fuentes Jersey 10 y Silkscreen, sonidos .wav) y se puede cambiar por dibujos propios del mismo tamaño.

**Reglas que se programaron:**

- Venta: el cliente busca un tipo (sedán, compacto o pickup) y trae un presupuesto. Si el auto es de ese tipo y le alcanza,
  lo compra 30 % más caro de lo que costó. Una venta por turno.
- Cartas: 25 en la baraja (10 Regateo, 8 Estafa, 7 Aseguradora), salen 16 y cada jugador escoge 3 en serpiente.
  Las Aseguradoras no se usan con el botón: se activan solas cuando alguien te lanza una Estafa.
- Vendedores: Licenciado vende 5 % más caro, La Doña tiene clientes con 10 % más dinero, Junior empieza con $20,000 extra
  y a Don Chuy sus autos le valen 10 % más al final.

**Lo que investigamos (no se vio en clase):** `Dictionary` (caché de fuentes), `PrivateFontCollection`, `SoundPlayer`,
`Timer` para la animación de voltear carta, `DllImport` para que el TextBox use la fuente pixel.

## Etapa 2 · 11 oct · Que el juego fluya (notas originales)

- [ ] **Arreglar "Iniciar partida".** Hoy `BtnContinuarTablero_Click` solo hace `this.Hide()` y nunca abre el tablero.
      Por eso parecía que "tardaba en cargar". Hay que abrir el tablero y pasarle los jugadores:
      ```csharp
      private void BtnContinuarTablero_Click(object sender, EventArgs e)
      {
          Tablero tablero = new Tablero(jugadoresRegistrados);
          tablero.FormClosed += (s, args) => Application.Exit();  // si cierran el tablero, se cierra el juego
          tablero.Show();
          this.Hide();
      }
      ```
      Lo mismo en `FormSaludo`: si solo se esconde, el programa se queda corriendo aunque no se vea ninguna ventana.
- [ ] **Una clase `Partida`** que guarde jugadores, cartas de cada quien, ronda y turno, y que se pase de un formulario a otro.
      Así no se pasan listas sueltas entre pantallas.
- [ ] **Baraja de 25 cartas y sacar 16 al azar** (revolver con `Random`, algoritmo Fisher-Yates).
      Boca abajo solo se ve "?"; al escogerla se voltea y enseña qué hace.
- [ ] **Draft de a una carta, en serpiente:** 1-2-3-4, luego 4-3-2-1. Así el último no se queda con lo que sobra.
- [ ] **Menos MessageBox.** Cada "Aceptar" corta el ritmo. Los avisos van en un `Label` de estado;
      el MessageBox solo para cosas importantes (fin del draft, fin de la partida).
- [ ] **Enter y Esc funcionan:** `AcceptButton` y `CancelButton` del formulario.
- [ ] **Tablero del juego** (el actual es el ejercicio de promedio): cliente por turno, vender, usar carta, pasar turno, ranking.
      De lo que ya está se reusa la lista de 20 autos con sus precios.

## Etapa 3 · 21 oct · Que se vea mejor (arte base y UI)

- [ ] **Carpeta `Assets/`** con `Sprites/`, `Fuentes/` y `Sonidos/`. En cada archivo: Propiedades → Copiar en el directorio de salida → Copiar si es posterior.
- [ ] **Clase `Recursos`** que carga todas las imágenes una vez al arrancar (en `Program.cs`). Cada pantalla abre al instante.
- [ ] **`PixelBox`** (PictureBox con `InterpolationMode.NearestNeighbor`) para que el pixel art no se vea borroso.
- [ ] **Fuente pixel** con `PrivateFontCollection` (Pixelify Sans, gratis en Google Fonts).
- [ ] **Vendedores:** cada jugador escoge personaje al registrarse (El Licenciado, La Doña, El Junior, Don Chuy). Ventajas las define Iker.
- [ ] **Clientes con retrato y frase** ("Busco algo para Uber, traigo $100,000").
- [ ] **Autos y cartas dibujados** en Piskel (personajes 16×16, autos 32×16, exportar ya agrandados ×4).
- [ ] **Botones de juego:** `FlatStyle.Flat`, imagen de fondo y cambio al pasar el mouse.
- [ ] **Animación de voltear carta** con un `Timer` (angostar a 0, cambiar imagen, volver a abrir).
- [ ] **El dinero sube contando** con un `Timer` cuando vendes, en vez de cambiar de golpe.
- [ ] **Sonidos** con `SoundPlayer` (.wav de sfxr.me): vender, voltear carta, turno nuevo.
- [ ] **`DoubleBuffered = true`** en los formularios para que no parpadeen.

## Etapa 4 · 31 oct · Entrega final

- [ ] **Pantalla de título** y **pantalla de resultados** con podio (ranking con burbuja).
- [ ] **Pantalla "Cómo se juega"** con las reglas y los créditos del arte que se use de internet.
- [ ] **Guardar y cargar partida** en archivo de texto (`StreamWriter` / `StreamReader`).
- [ ] **Transiciones** suaves entre pantallas (subir `Opacity` con un `Timer`).
- [ ] **Balance:** jugar varias partidas completas y ajustar precios, presupuestos y cartas.
- [ ] **Pulir:** icono `.ico`, títulos de ventana, tamaño fijo (`FormBorderStyle.FixedSingle`, `MaximizeBox = false`).

## Lo que investigamos por nuestra cuenta

Cosas que no se han visto en clase y que usamos para ir más allá del temario. Conviene mencionarlas en la exposición:

- `List<T>` y `Dictionary<TKey, TValue>` para jugadores y cartas.
- LINQ (`Average`, `Where`, `Cast`) en el tablero.
- `PrivateFontCollection`, `SoundPlayer` y animaciones con `Timer` (etapa 3).
- Algoritmo Fisher-Yates para revolver la baraja.
