using FluentValidation;
using Aplicacion.DTOs.Citas;

namespace Aplicacion.Validaciones.Citas
{
    public class ActualizarCitaValidator : AbstractValidator<ActualizarCitaDTO>
    {
        public ActualizarCitaValidator()
        {
            RuleFor(c => c.Id).GreaterThan(0).WithMessage("Id de cita inválido");

            RuleFor(c => c.FechaHora)
                .GreaterThan(DateTime.Now).WithMessage("La fecha y hora de la cita debe ser futura");

            RuleFor(c => c.Motivo)
                .MaximumLength(500).WithMessage("El motivo no puede superar 500 caracteres");
        }
    }
}
