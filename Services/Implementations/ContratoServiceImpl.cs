using System.Data;
using inmobiliariaULP.Models;
using inmobiliariaULP.Models.ViewModels;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Repositories.Interfaces;
using Google.Protobuf;

namespace inmobiliariaULP.Services.Implementations;

public class ContratoServiceImpl : IContratoService
{
    private readonly IContratoRepository _contratoRepository;

    public ContratoServiceImpl(IContratoRepository contratoRepository)
    {
        _contratoRepository = contratoRepository;
    }

    public async Task<( bool exito, string mensaje, string tipo )> CrearAsync(ContratoDetalleDTO contrato)
    {
        try
        {
            var contratoModelado = new Contrato
            {
                InmuebleId = contrato.InmuebleId,
                InquilinoId = contrato.InquilinoId,
                UsuarioId = contrato.UsuarioId,
                FechaInicio = contrato.FechaInicio,
                FechaFin = contrato.FechaFin,
                MontoMensual = contrato.MontoMensual,
                Estado = contrato.EstadoContrato
            };

            var data = await _contratoRepository.AddAsync(contratoModelado);

            if (data <= 0)
                return (false, "Contrato no puede ser nulo", "error");

            return (true, "Contrato creado exitosamente", "success");
            
        }
        catch (Exception ex)
        {
            return (false, ex.Message, "error");
        }
    }

    public async Task<ContratoDetalleDTO?> ObtenerPorIdAsync(int id)
    {
        return await _contratoRepository.GetByIdAsync(id);
    }

    public async Task<(IEnumerable<ContratoListadoDTO> Contratos, int Total)> ObtenerTodosAsync(int page, int pageSize, string? search = null)
    {
        return await _contratoRepository.GetAllAsync(page, pageSize, search);
    }
}