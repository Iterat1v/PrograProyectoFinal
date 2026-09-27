using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PrograProyectoFinal
{
    public partial class FormSaludo : Form
    {
        // Guardar
        private List<string> jugadores = new List<string>();
        private const int MIN_JUGADORES = 2;
        private const int MAX_JUGADORES = 4;

        public FormSaludo()
        {
            InitializeComponent();
            ActualizarEstadoUI();
        }

        // Contador caracteres
        private void txtbName_TextChanged(object sender, EventArgs e)
        {
            lbCharCount.Text = $"{txtbName.Text.Length} / 200 caracteres";
        }

        // Botón: Aceptar y registrar otro usuario
        private void btnRegistrarOtro_Click(object sender, EventArgs e)
        {
            // Validar si ya se llegó al límite antes de intentar agregar
            if (jugadores.Count >= MAX_JUGADORES)
            {
                MessageBox.Show("Ya se ha alcanzado el límite máximo de 4 jugadores.", "Límite alcanzado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ValidarNombre(txtbName.Text))
            {
                jugadores.Add(txtbName.Text.Trim());
                txtbName.Text = "";
                ActualizarEstadoUI();

                // Notificar si con este registro se llenó el cupo máximo
                if (jugadores.Count == MAX_JUGADORES)
                {
                    MessageBox.Show("¡Jugador registrado! Se ha alcanzado el límite máximo de jugadores posibles (4).", "Cupo lleno", MessageBoxButtons.OK, MessageBoxIcon.None);
                }
                else
                {
                    MessageBox.Show($"¡Jugador registrado con éxito! Quedan {MAX_JUGADORES - jugadores.Count} lugares disponibles.", "Registro exitoso", MessageBoxButtons.OK, MessageBoxIcon.None);
                    txtbName.Focus();
                }
            }
        }

        // Botón: Aceptar y pasar al Draft / Finalizar
        private void btnAceptarFinal2_Click(object sender, EventArgs e)
        {
            // Validar que la lista de jugadores previamente registrados cumpla el mínimo
            if (jugadores.Count < MIN_JUGADORES)
            {
                MessageBox.Show($"Se requieren como mínimo {MIN_JUGADORES} jugadores para iniciar la partida. Actualmente tienes {jugadores.Count} registrados.", "Faltan jugadores", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ActualizarEstadoUI();
                txtbName.Focus();
                return; // Detiene la ejecución
            }

            // Pasar al Draft si la lista acumulada es válida
            MessageBox.Show($"¡Buena suerte empresarios {string.Join(", ", jugadores)}! Pasando a la nueva partida...", "Nueva Partida", MessageBoxButtons.OK, MessageBoxIcon.None);

            FormDraft pantallaDraft = new FormDraft(jugadores);
            pantallaDraft.Show();

            this.Hide();
        }

        // Botón: Limpiar
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtbName.Text = "";
            txtbName.Focus();
        }

        // Función auxiliar para validar entradas
        private bool ValidarNombre(string nombre)
        {
            string nombreLimpio = nombre.Trim();

            if (string.IsNullOrWhiteSpace(nombreLimpio))
            {
                MessageBox.Show("El nombre de usuario no puede estar vacío.", "Campo Vacío", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (jugadores.Contains(nombreLimpio))
            {
                MessageBox.Show("Ese nombre de usuario ya fue registrado. Ingresa uno distinto.", "Nombre Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        // Actualiza las etiquetas de contador de jugador
        private void ActualizarEstadoUI()
        {
            int proximoJugador = jugadores.Count + 1;
            if (proximoJugador <= MAX_JUGADORES)
            {
                lbContador.Text = $"Registrando Jugador {proximoJugador} de {MAX_JUGADORES}";
            }
            else
            {
                lbContador.Text = $"Cupo lleno ({MAX_JUGADORES} jugadores)";
            }
        }
    }
}