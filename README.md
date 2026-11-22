# CAR CASH · Equipo 4

Juego de gestión de un lote de autos usados, hecho en C# con Windows Forms (.NET 10).
Compras autos en el mercado, atiendes clientes, regateas, pintas y reparas en el taller,
y compites contra bots o contra tus amigos en la misma computadora.

## Cómo bajarlo y jugarlo

Necesitas Windows y el SDK de .NET 10 (https://dotnet.microsoft.com/download/dotnet/10.0)
o Visual Studio con la carga de trabajo "Desarrollo de escritorio de .NET".

```
git clone -b propuesta https://github.com/Iterat1v/PrograProyectoFinal.git
cd PrograProyectoFinal
dotnet run -c Release
```

Con Visual Studio: abre `PrograProyectoFinal.slnx` y presiona F5.
Para jugar en ventana (en vez de pantalla completa): `dotnet run -c Release -- --ventana`.

Tus perfiles, récords y partidas se guardan solos en `%AppData%\Concesionario`.

## Cómo se juega (resumen)

1. Escoge **Partida libre** (1 a 4 jugadores, zona y dificultad) o **Modo historia** (5 semanas contra 5 jefes).
2. Antes de empezar hay un draft: salen 16 cartas y cada jugador escoge 3 (Regateo, Estafa, Aseguradora).
3. Cada turno llega un cliente que pide un tipo de auto, trae cierto dinero y tiene un tiempo de paciencia.
4. En el **mercado** compras autos (los chocados salen a mitad de precio), en el **taller** los reparas o los pintas.
5. Vendes un auto de tu lote: tú pones el precio y el cliente acepta o contraoferta (3 intentos).
6. Gana quien termine con más dinero más valor de autos. Cuida tu reputación.

Atajos: **P** pausa el juego. El botón RÁPIDO acelera el turno de los bots.

## Para quien lo vaya a presentar

- Lo que se ve en pantalla se explica con tus palabras; no se lee la diapositiva.
- Antes de exponer, juega al menos una partida completa para saber dónde está cada botón.
- Si algo falla, se anota solo en `%AppData%\Concesionario\errores.txt`.

## Estructura del código

- `Modelo/` clases del juego (Partida, Concesionario, Auto, Cliente, Baraja, PowerUp, Bot...).
- `Interfaz/` dibujo propio en pixel art (Pixel, BotonPixel, FormPixel, Recursos).
- `Form*.cs` cada pantalla del juego.
- `Assets/` imágenes, sonidos y fuentes.
