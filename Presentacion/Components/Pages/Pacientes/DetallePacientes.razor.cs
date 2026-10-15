using Microsoft.AspNetCore.Components;
using Aplicacion.DTOs.Pacientes;
using Aplicacion.Servicios.Interfaces;
using Presentacion.Servicios;

namespace Presentacion.Components.Pages.Pacientes
{
    public partial class DetallePacientes : ComponentBase
    {
        [Inject] private IPacienteService ServiciosPaciente { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        [Parameter] public int id { get; set; }

        protected PacienteDTO? paciente;

        protected override async Task OnInitializedAsync()
        {
            await CargarPaciente();
        }

        private async Task CargarPaciente()
        {
            var resultado = await ServiciosPaciente.ObtenerPorIdAsync(id);
            if (resultado.Exitoso && resultado.Datos != null)
            {
                paciente = resultado.Datos;
            }
            else
            {
                await Toastr.MsgError("Paciente no encontrado o error al cargar.");
                Navigation.NavigateTo("/pacientes");
            }
        }

        protected void Volver() => Navigation.NavigateTo("/pacientes");
        protected void Editar() => Navigation.NavigateTo($"/editar-paciente/{id}");
    }
}
