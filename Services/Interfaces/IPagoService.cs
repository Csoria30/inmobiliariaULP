using inmobiliariaULP.Models;
using inmobiliariaULP.Models.ViewModels;
namespace inmobiliariaULP.Services.Interfaces;

public interface IPagoService
{
    Task<(bool exito, string mensaje, string tipo)> CrearAsync(PagoDTO pagoDto);
    Task<(IEnumerable<PagoDTO> Pagos, int Total)> ObtenerTodosAsync(int page, int pageSize, string? search = null);
    Task<(Pago pago, string mensaje, string tipo)> ObtenerIdAsync(int pagoId);
    Task<(bool exito, string mensaje, string tipo)> EditarAsync(PagoDTO pagoDto);
    Task<(bool exito, string mensaje, string tipo)> AnularAsync(int pagoId, int usuarioId);
    Task<IEnumerable<PagoDTO>> ListarPorContratoAsync(int contratoId);
    Task<int> ContarPagosPorContratoAsync(int contratoId);
    Task<decimal> ObtenerTotalPagadoAsync(int contratoId);
    Task<string> GenerarNumeroAutomaticoAsync(int contratoId);
    Task<(bool puedeRealizarPago, string razon)> ValidarPosibilidadPagoAsync(int contratoId, decimal importe);
    
}