using Microsoft.AspNetCore.Components;
using Aplicacion.DTOs.Pacientes;
using Aplicacion.Servicios.Interfaces;
using Presentacion.Servicios;

namespace Presentacion.Components.Pages.Pacientes
{
    public partial class CrearPacientes : ComponentBase
    {
        [Inject] private IPacienteService ServiciosPaciente { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        protected CrearPacienteDTO pacienteDto = new()
        {
            FechaNacimiento = DateTime.Today.AddYears(-20)
        };

        protected async Task GrabarPaciente()
        {
            var resultado = await ServiciosPaciente.CrearAsync(pacienteDto);
            if (resultado.Exitoso)
            {
                await Toastr.MsgExito("Paciente registrado exitosamente.");
                Navigation.NavigateTo("/pacientes");
            }
            else
            {
                await Toastr.MsgError("Error al registrar paciente: " + resultado.Mensaje);
            }
        }

        protected void Cancelar() => Navigation.NavigateTo("/pacientes");
    }
}
