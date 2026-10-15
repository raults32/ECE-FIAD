using FluentValidation;
using Aplicacion.DTOs.Doctores;

namespace Aplicacion.Validaciones.Doctores
{
    public class ActualizarDoctorValidator : AbstractValidator<ActualizarDoctorDTO>
    {
        public ActualizarDoctorValidator()
        {
            RuleFor(d => d.Id).GreaterThan(0).WithMessage("Id de doctor inválido");
            RuleFor(d => d.IdEspecialidad).GreaterThan(0).WithMessage("Debe seleccionar una especialidad");
            RuleFor(d => d.Nombres).NotEmpty().WithMessage("Los nombres son obligatorios").MaximumLength(100);
            RuleFor(d => d.Apellidos).NotEmpty().WithMessage("Los apellidos son obligatorios").MaximumLength(100);
            RuleFor(d => d.Email)
                .NotEmpty().WithMessage("El correo es obligatorio")
                .EmailAddress().WithMessage("Formato de correo incorrecto")
                .MaximumLength(150);
        }
    }
}
