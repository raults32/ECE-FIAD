using Dominio.Entidades.Citas;
using Dominio.Entidades.Doctores;
using Dominio.Entidades.Especialidades;
using Dominio.Entidades.Pacientes;

namespace Dominio.Interfaces
{
    public interface IUnitOfWork
    {
        IRepositorio<Paciente> Pacientes { get; }
        IRepositorio<Doctor> Doctores { get; }
        IRepositorio<Especialidad> Especialidades { get; }
        IRepositorio<Cita> Citas { get; }

        Task<int> GuardarCambiosAsync();
    }
}
