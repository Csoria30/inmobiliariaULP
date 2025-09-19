using System.Data;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Repositories.Interfaces;

namespace inmobiliariaULP.Services.Implementations;

public class EmpleadoServiceImpl : IEmpleadoService
{
    private readonly IEmpleadoRepository _empleadoRepository;

    public EmpleadoServiceImpl(IEmpleadoRepository empleadoRepository)
    {
        _empleadoRepository = empleadoRepository;
    }

    public Task<int> EliminarAsync(int empleadoId)
    {
        try
        {
            return _empleadoRepository.DeleteAsync(empleadoId);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al eliminar el empleado", ex);
        }
    }

    public Task<int> NuevoAsync(int personaId)
    {
        try
        {
            return _empleadoRepository.AddAsync(personaId);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear el nuevo empleado", ex);
        }
    }

    public Task<Empleado> ObtenerIdAsync(int empleadoId)
    {
        try
        {
            return _empleadoRepository.GetByIdAsync(empleadoId);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener el empleado por ID", ex);
        }
    }

    public Task<(IEnumerable<Empleado> Empleados, int Total)> ObtenerTodosAsync(int page, int pageSize, string? search = null)
    {
        try
        {
            return _empleadoRepository.GetAllAsync(page, pageSize, search);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener todos los empleados", ex);
        }
    }


    public Task<int> ActualizarAsync(int empleadoId, bool estado)
    {
        try
        {
            return _empleadoRepository.UpdateAsync(empleadoId, estado);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al actualizar el empleado", ex);
        }
    }
}