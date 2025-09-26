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

    public async Task<(bool exito, string mensaje, string tipo)> EditarAsync(ContratoDetalleDTO contrato)
    {
        try
        {
            var contratoActual = await _contratoRepository.GetByIdAsync(contrato.ContratoId);

            if (contratoActual == null)
                return (false, "El contrato no existe.", "error");

            // Lista de campos a comparar
            var campos = new[] { "InmuebleId", "InquilinoId", "UsuarioId", "FechaInicio", "FechaFin", "MontoMensual", "EstadoContrato" };

            bool hayCambios = false;

            foreach (var campo in campos)
            {
                var valorActual = contratoActual.GetType().GetProperty(campo)?.GetValue(contratoActual)?.ToString();
                var valorNuevo = contrato.GetType().GetProperty(campo)?.GetValue(contrato)?.ToString();

                if (!Equals(valorActual, valorNuevo))
                {
                    hayCambios = true;
                    break;
                }
            }

            if (!hayCambios)
                return (false, "No se detectaron cambios en el contrato.", "warning");

            // Mapear DTO a entidad Contrato
            var contratoModelado = new Contrato
            {
                ContratoId = contrato.ContratoId,
                InmuebleId = contrato.InmuebleId,
                InquilinoId = contrato.InquilinoId,
                UsuarioId = contrato.UsuarioId,
                FechaInicio = contrato.FechaInicio,
                FechaFin = contrato.FechaFin,
                MontoMensual = contrato.MontoMensual,
                FechaFinalizacionAnticipada = contrato.FechaAnticipada,
                Multa = contrato.Multa,
                Estado = contrato.EstadoContrato
            };

            var filasAfectadas = await _contratoRepository.UpdateAsync(contratoModelado);

            if (filasAfectadas > 0)
                return (true, "Contrato actualizado exitosamente.", "success");
            else
                return (false, "No se pudo actualizar el contrato.", "error");

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