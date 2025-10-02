using inmobiliariaULP.Models;
using inmobiliariaULP.Models.ViewModels;
public interface IPagoRepository
{
    Task<(bool exito, string mensaje, Pago? pago)> AddAsync(Pago pago);
    Task<(bool exito, string mensaje, Pago? pago)> GetByIdAsync(int id);
    Task<(bool exito, string mensaje)> UpdateAsync(Pago pago);
    Task<(bool exito, string mensaje)> DeleteAsync(int id, int usuarioId);
    Task<(List<PagoDTO> pagos, int total)> GetAllAsync(int pagina = 1, int tamañoPagina = 10, string filtro = "");
    Task<List<PagoDTO>> GetByContratoAsync(int contratoId);
    Task<int> CountByContratoAsync(int contratoId);
    Task<decimal> GetTotalPagadoAsync(int contratoId);
    Task<string> GenerateNumeroAsync(int contratoId);
}