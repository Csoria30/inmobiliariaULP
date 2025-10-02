using inmobiliariaULP.Models;
using inmobiliariaULP.Models.ViewModels;
namespace inmobiliariaULP.Repositories.Interfaces;

public interface IContratoRepository
{
    Task<int> AddAsync(Contrato contrato);
    Task<(IEnumerable<ContratoListadoDTO> Contratos, int Total)> GetAllAsync(int page, int pageSize, string? search = null);
    Task<ContratoDetalleDTO> GetByIdAsync(int contratoId);
    Task<int> DeleteAsync(int contratoId);
    Task<int> UpdateAsync(Contrato contrato);
    Task<bool> ExisteContratoVigenteAsync(int inmuebleId, DateTime fechaInicio, DateTime fechaFin);
}