using System.Linq.Expressions;
using Dominio.Entidades.Bases;
using Dominio.Interfaces;
using Infraestructura.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Repositorios
{
    public class Repositorio<T> : IRepositorio<T> where T : EntidadBase
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repositorio(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> ObtenerPorIdAsync(int id) =>
            await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.Eliminado);

        public async Task<IEnumerable<T>> ObtenerTodosAsync() =>
            await _dbSet.Where(e => !e.Eliminado).ToListAsync();

        public async Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> filtro) =>
            await _dbSet.Where(e => !e.Eliminado).Where(filtro).ToListAsync();

        public async Task AgregarAsync(T entidad) => await _dbSet.AddAsync(entidad);

        public void Actualizar(T entidad) => _dbSet.Update(entidad);
    }
}