using FluentValidation;
using Aplicacion.DTOs.Pacientes;

namespace Aplicacion.Validaciones.Pacientes
{
    public class ActualizarPacienteValidator : AbstractValidator<ActualizarPacienteDTO>
    {
        public ActualizarPacienteValidator()
        {
            RuleFor(p => p.Id).GreaterThan(0).WithMessage("Id de paciente inválido");

            RuleFor(p => p.Nombres)
                .NotEmpty().WithMessage("Los nombres son obligatorios")
                .MaximumLength(100).WithMessage("Los nombres no pueden superar 100 caracteres");

            RuleFor(p => p.Apellidos)
                .NotEmpty().WithMessage("Los apellidos son obligatorios")
                .MaximumLength(100).WithMessage("Los apellidos no pueden superar 100 caracteres");

            RuleFor(p => p.Email)
                .EmailAddress().WithMessage("Formato de email incorrecto")
                .MaximumLength(150).WithMessage("El email no puede superar 150 caracteres");
        }
    }
}