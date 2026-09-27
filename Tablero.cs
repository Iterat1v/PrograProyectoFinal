using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PrograProyectoFinal
{
    public partial class Tablero : Form
    {
        private class Auto
        {
            public string Modelo { get; set; } = string.Empty;
            public double Precio { get; set; }

            public override string ToString()
            {
                return Modelo;
            }
        }

        private List<Auto> listaAutos = new List<Auto>();
        private List<string> jugadores;

        public Tablero()
        {
            InitializeComponent();
        }

        public Tablero(List<string> jugadoresRegistrados) : this()
        {
            this.jugadores = jugadoresRegistrados;
            if (jugadores != null && jugadores.Count > 0)
            {
                lblJugadorTurno.Text = $"Jugador en turno: {jugadores[0]}";
            }
        }

        private void Tablero_Load(object sender, EventArgs e)
        {
            CargarDatosPredefinidos();
            InicializarCheckedListBox();
            LimpiarResultados();
        }

        private void CargarDatosPredefinidos()
        {
            listaAutos = new List<Auto>
            {
                new Auto { Modelo = "Nissan Versa", Precio = 334900 },
                new Auto { Modelo = "Chevrolet Aveo", Precio = 290400 },
                new Auto { Modelo = "Volkswagen Jetta", Precio = 419900 },
                new Auto { Modelo = "Mazda 3", Precio = 392900 },
                new Auto { Modelo = "Kia Rio", Precio = 320900 },
                new Auto { Modelo = "Toyota Corolla", Precio = 419900 },
                new Auto { Modelo = "Honda Civic", Precio = 545900 },
                new Auto { Modelo = "Hyundai Accent", Precio = 310500 },
                new Auto { Modelo = "Ford Mustang", Precio = 995000 },
                new Auto { Modelo = "Chevrolet Onix", Precio = 305800 },
                new Auto { Modelo = "Nissan Sentra", Precio = 390900 },
                new Auto { Modelo = "Volkswagen Polo", Precio = 329900 },
                new Auto { Modelo = "Mazda CX-30", Precio = 459900 },
                new Auto { Modelo = "Toyota Yaris", Precio = 312800 },
                new Auto { Modelo = "Honda CR-V", Precio = 714900 },
                new Auto { Modelo = "Kia Forte", Precio = 382900 },
                new Auto { Modelo = "Hyundai Elantra", Precio = 410000 },
                new Auto { Modelo = "BMW Serie 3", Precio = 925000 },
                new Auto { Modelo = "Audi A4", Precio = 890000 },
                new Auto { Modelo = "Mercedes-Benz Clase C", Precio = 1050000 }
            };
        }

        private void InicializarCheckedListBox()
        {
            chkListAutos.Items.Clear();
            foreach (var auto in listaAutos)
            {
                chkListAutos.Items.Add(auto, true);
            }
        }

        private void LimpiarResultados()
        {
            lstSobrePromedio.Items.Clear();
            lblPromedio.Text = "Precio promedio (0 autos): $0.00";
            lblConteo.Text = "Autos arriba del promedio y con mayor comisión: 0";
        }

        private void btnCalcular_Click(object sender, EventArgs e)
        {
            CalcularEstadisticas();
        }

        private void CalcularEstadisticas()
        {
            var seleccionados = chkListAutos.CheckedItems.Cast<Auto>().ToList();

            lstSobrePromedio.Items.Clear();

            if (seleccionados.Count == 0)
            {
                lblPromedio.Text = "Precio promedio (0 autos): $0.00";
                lblConteo.Text = "Autos arriba del promedio y con mayor comisión: 0";
                return;
            }

            double promedio = seleccionados.Average(a => a.Precio);
            var sobrePromedio = seleccionados.Where(a => a.Precio > promedio).ToList();

            foreach (var auto in sobrePromedio)
            {
                lstSobrePromedio.Items.Add($"{auto.Modelo} (${auto.Precio:N2})");
            }

            lblPromedio.Text = $"Precio promedio ({seleccionados.Count} autos): ${promedio:N2}";
            lblConteo.Text = $"Autos arriba del promedio y con mayor comisión: {sobrePromedio.Count}";
        }
    }
}
