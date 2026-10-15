using System.Linq.Expressions;
using Dominio.Entidades.Bases;

namespace Dominio.Interfaces
{
    public interface IRepositorio<T> where T : EntidadBase
    {
        Task<T?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<T>> ObtenerTodosAsync();
        Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> filtro);
        Task AgregarAsync(T entidad);
        void Actualizar(T entidad);
    }
}