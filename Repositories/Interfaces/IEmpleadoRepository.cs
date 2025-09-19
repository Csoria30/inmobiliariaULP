using inmobiliariaULP.Models;
namespace inmobiliariaULP.Repositories.Interfaces;

public interface IEmpleadoRepository
{
    Task<int> AddAsync(int personaId);
    Task<(IEnumerable<Empleado> Empleados, int Total)> GetAllAsync(int page, int pageSize, string? search = null);
    Task<Empleado> GetByIdAsync(int empleadoId);
    Task<int> DeleteAsync(int empleadoId);
    Task<int> UpdateAsync(int empleadoId, bool estado);
}