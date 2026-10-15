using FluentValidation;
using Aplicacion.DTOs.Doctores;

namespace Aplicacion.Validaciones.Doctores
{
    public class CrearDoctorValidator : AbstractValidator<CrearDoctorDTO>
    {
        public CrearDoctorValidator()
        {
            RuleFor(d => d.IdEspecialidad)
                .GreaterThan(0).WithMessage("Debe seleccionar una especialidad");

            RuleFor(d => d.Nombres)
                .NotEmpty().WithMessage("Los nombres son obligatorios")
                .MaximumLength(100).WithMessage("Los nombres no pueden superar 100 caracteres");

            RuleFor(d => d.Apellidos)
                .NotEmpty().WithMessage("Los apellidos son obligatorios")
                .MaximumLength(100).WithMessage("Los apellidos no pueden superar 100 caracteres");

            RuleFor(d => d.Email)
                .NotEmpty().WithMessage("El correo es obligatorio")
                .EmailAddress().WithMessage("Formato de correo incorrecto")
                .MaximumLength(150).WithMessage("El correo no puede superar 150 caracteres");

            RuleFor(d => d.Telefono)
                .MaximumLength(15).WithMessage("El teléfono no puede superar 15 caracteres");
        }
    }
}
