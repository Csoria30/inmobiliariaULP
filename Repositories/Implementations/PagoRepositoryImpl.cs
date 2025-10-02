using System.Data;
using MySql.Data.MySqlClient;
using inmobiliariaULP.Repositories.Interfaces;
using inmobiliariaULP.Models;
using inmobiliariaULP.Models.ViewModels;

namespace inmobiliariaULP.Repositories.Implementations;

public class PagoRepositoryImpl(IConfiguration configuration) : BaseRepository(configuration), IPagoRepository
{
    public Task<(bool exito, string mensaje)> ActualizarAsync(Pago pago)
    {
        throw new NotImplementedException();
    }

    public Task<(bool exito, string mensaje)> AnularAsync(int id, int usuarioId)
    {
        throw new NotImplementedException();
    }

    public Task<int> ContarPagosPorContratoAsync(int contratoId)
    {
        throw new NotImplementedException();
    }

    public Task<(bool exito, string mensaje, Pago? pago)> CrearAsync(Pago pago)
    {
        throw new NotImplementedException();
    }

    public Task<string> GenerarNumeroPrefijoAsync(int contratoId)
    {
        throw new NotImplementedException();
    }

    public Task<List<PagoDTO>> ObtenerPorContratoAsync(int contratoId)
    {
        throw new NotImplementedException();
    }

    public Task<(bool exito, string mensaje, Pago? pago)> ObtenerPorIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<(List<PagoDTO> pagos, int total)> ObtenerTodosAsync(int pagina = 1, int tamañoPagina = 10, string filtro = "")
    {
        throw new NotImplementedException();
    }

    public Task<decimal> ObtenerTotalPagadoAsync(int contratoId)
    {
        throw new NotImplementedException();
    }
}