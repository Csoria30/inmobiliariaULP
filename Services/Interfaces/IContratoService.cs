using inmobiliariaULP.Models;
using inmobiliariaULP.Models.ViewModels;
namespace inmobiliariaULP.Services.Interfaces;

public interface IContratoService
{
    Task<(IEnumerable<ContratoListadoDTO> Contratos, int Total)> ObtenerTodosAsync(int page, int pageSize, string? search = null);

    Task<ContratoDetalleDTO?> ObtenerPorIdAsync(int id);
    Task<(bool exito, string mensaje, string tipo)> CrearAsync(ContratoDetalleDTO contrato);
    Task<(bool exito, string mensaje, string tipo)> EditarAsync(ContratoDetalleDTO contrato);
}