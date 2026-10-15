using Dominio.Entidades.Citas;
using Dominio.Entidades.Doctores;
using Dominio.Entidades.Especialidades;
using Dominio.Entidades.Pacientes;
using Dominio.Interfaces;
using Infraestructura.Persistencia;

namespace Infraestructura.Repositorios
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Pacientes = new Repositorio<Paciente>(context);
            Doctores = new Repositorio<Doctor>(context);
            Especialidades = new Repositorio<Especialidad>(context);
            Citas = new Repositorio<Cita>(context);
        }

        public IRepositorio<Paciente> Pacientes { get; }
        public IRepositorio<Doctor> Doctores { get; }
        public IRepositorio<Especialidad> Especialidades { get; }
        public IRepositorio<Cita> Citas { get; }

        public async Task<int> GuardarCambiosAsync() => await _context.SaveChangesAsync();
    }
}
