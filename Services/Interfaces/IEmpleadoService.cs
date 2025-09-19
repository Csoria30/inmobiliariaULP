using inmobiliariaULP.Models;
namespace inmobiliariaULP.Services.Interfaces;

public interface IEmpleadoService
{
    Task<int> NuevoAsync(int personaId);
    Task<(IEnumerable<Empleado> Empleados, int Total)> ObtenerTodosAsync(int page, int pageSize, string? search = null);
    Task<Empleado> ObtenerIdAsync(int empleadoId);
    Task<int> EliminarAsync(int empleadoId);
    Task<int> ActualizarAsync(int empleadoId, Boolean estado); 
}