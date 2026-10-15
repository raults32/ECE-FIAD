using FluentValidation;
using Aplicacion.DTOs.Citas;

namespace Aplicacion.Validaciones.Citas
{
    public class CrearCitaValidator : AbstractValidator<CrearCitaDTO>
    {
        public CrearCitaValidator()
        {
            RuleFor(c => c.IdPaciente).GreaterThan(0).WithMessage("Debe seleccionar un paciente");
            RuleFor(c => c.IdDoctor).GreaterThan(0).WithMessage("Debe seleccionar un doctor");

            RuleFor(c => c.FechaHora)
                .GreaterThan(DateTime.Now).WithMessage("La fecha y hora de la cita debe ser futura");

            RuleFor(c => c.Motivo)
                .MaximumLength(500).WithMessage("El motivo no puede superar 500 caracteres");
        }
    }
}
