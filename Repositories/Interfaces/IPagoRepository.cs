using inmobiliariaULP.Models;
using inmobiliariaULP.Models.ViewModels;
public interface IPagoRepository
{
    Task<(bool exito, string mensaje, Pago? pago)> CrearAsync(Pago pago);
    Task<(bool exito, string mensaje, Pago? pago)> ObtenerPorIdAsync(int id);
    Task<(List<PagoDTO> pagos, int total)> ObtenerTodosAsync(int pagina = 1, int tamañoPagina = 10, string filtro = "");
    Task<List<PagoDTO>> ObtenerPorContratoAsync(int contratoId);
    Task<(bool exito, string mensaje)> ActualizarAsync(Pago pago);
    Task<(bool exito, string mensaje)> AnularAsync(int id, int usuarioId);
    Task<int> ContarPagosPorContratoAsync(int contratoId);
    Task<decimal> ObtenerTotalPagadoAsync(int contratoId);
    Task<string> GenerarNumeroPrefijoAsync(int contratoId);
}