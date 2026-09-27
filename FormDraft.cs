using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PrograProyectoFinal
{
    public partial class FormDraft : Form
    {
        private List<string> jugadoresRegistrados;
        private int jugadorActualIndex = 0;
        private Dictionary<string, int> cartasPorJugador = new Dictionary<string, int>();

        private const int CARTAS_POR_JUGADOR = 3;
        private int totalCartasASeleccionar;

        public FormDraft(List<string> jugadores)
        {
            InitializeComponent();

            this.jugadoresRegistrados = (jugadores != null && jugadores.Count > 0)
                ? jugadores
                : new List<string> { "Jugador 1", "Jugador 2" };

            totalCartasASeleccionar = jugadoresRegistrados.Count * CARTAS_POR_JUGADOR;

            foreach (var j in jugadoresRegistrados)
            {
                cartasPorJugador[j] = 0;
            }

            GenerarBotonesCartas();
            ActualizarTextoTurno();
        }

        public FormDraft() : this(new List<string> { "Jugador 1", "Jugador 2" }) { }

        // Agrega los 16 botones dentro del panel reservado del diseñador
        private void GenerarBotonesCartas()
        {
            for (int i = 1; i <= 16; i++)
            {
                Button btnCarta = new Button
                {
                    Text = $"🎴\nCarta {i}",
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    BackColor = Color.LightSteelBlue,
                    FlatStyle = FlatStyle.Flat,
                    Margin = new Padding(6),
                    Tag = i
                };

                btnCarta.Click += BtnCarta_Click;
                panelCuadricula.Controls.Add(btnCarta);
            }
        }

        private void BtnCarta_Click(object sender, EventArgs e)
        {
            Button boton = sender as Button;
            if (boton == null || !boton.Enabled) return;

            string jugadorActual = jugadoresRegistrados[jugadorActualIndex];

            cartasPorJugador[jugadorActual]++;
            int numCarta = (int)boton.Tag;

            MessageBox.Show($"¡{jugadorActual} ha tomado la Carta #{numCarta}!",
                            "Carta Elegida",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

            boton.Enabled = false;
            boton.BackColor = Color.DarkGray;
            boton.Text += $"\n({jugadorActual})";

            if (cartasPorJugador[jugadorActual] >= CARTAS_POR_JUGADOR)
            {
                jugadorActualIndex++;
            }

            int totalCartasTomadas = 0;
            foreach (var count in cartasPorJugador.Values) totalCartasTomadas += count;

            if (totalCartasTomadas >= totalCartasASeleccionar || jugadorActualIndex >= jugadoresRegistrados.Count)
            {
                FinalizarDraft();
            }
            else
            {
                ActualizarTextoTurno();
            }
        }

        private void ActualizarTextoTurno()
        {
            if (jugadorActualIndex < jugadoresRegistrados.Count)
            {
                string jugador = jugadoresRegistrados[jugadorActualIndex];
                int cartasTomadas = cartasPorJugador[jugador];
                lbTurno.Text = $"Turno de: {jugador}  |  Cartas seleccionadas: {cartasTomadas} de {CARTAS_POR_JUGADOR}";
            }
        }

        private void FinalizarDraft()
        {
            lbTurno.Text = "¡Todas las cartas han sido distribuidas exitosamente!";
            lbTurno.ForeColor = Color.DarkGreen;

            foreach (Control ctrl in panelCuadricula.Controls)
            {
                ctrl.Enabled = false;
            }

            btnContinuarTablero.Enabled = true;
            btnContinuarTablero.BackColor = Color.MediumSeaGreen;
            btnContinuarTablero.ForeColor = Color.White;

            MessageBox.Show("¡Cada jugador ha recibido sus 3 cartas!\nHaz clic en 'Iniciar Partida' para ir al Tablero.",
                            "Draft Finalizado",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        private void BtnContinuarTablero_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Cargando el Tablero de juego...", "Siguiente Fase", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Hide();
        }
    }
}