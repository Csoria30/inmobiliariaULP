using System.Data;
using inmobiliariaULP.Models;
using inmobiliariaULP.Models.ViewModels;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Repositories.Interfaces;

namespace inmobiliariaULP.Services.Implementations;

public class PagoServiceImpl : IPagoService
{
    private readonly IPagoRepository _pagoRepository;
    private readonly IContratoRepository _contratoRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public PagoServiceImpl(
        IPagoRepository pagoRepository,
        IContratoRepository contratoRepository,
        IUsuarioRepository usuarioRepository
    )
    {
        _pagoRepository = pagoRepository;
        _contratoRepository = contratoRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<(bool exito, string mensaje, string tipo)> CrearAsync(PagoDTO pagoDto)
    {
        try
        {
            // Validaciones de negocio
            if (pagoDto.IdContrato == 0)
                return (false, "Debe seleccionar un contrato.", "warning");

            if (pagoDto.IdUsuario == 0)
                return (false, "El usuario es obligatorio.", "warning");

            if (pagoDto.Importe <= 0)
                return (false, "El importe debe ser mayor a 0.", "warning");

            // Verificar que el contrato existe y está vigente
            var contrato = await _contratoRepository.GetByIdAsync(pagoDto.IdContrato);
            if (contrato == null)
                return (false, "El contrato especificado no existe.", "warning");

            if (contrato.EstadoContrato != "vigente")
                return (false, "Solo se pueden registrar pagos para contratos vigentes.", "warning");

            // Verificar que la fecha de pago no sea futura
            if (pagoDto.FechaPago > DateTime.Today)
                return (false, "La fecha de pago no puede ser futura.", "warning");

            // Verificar que la fecha de pago esté dentro del período del contrato
            if (pagoDto.FechaPago < contrato.FechaInicio)
                return (false, "La fecha de pago no puede ser anterior al inicio del contrato.", "warning");

            if (pagoDto.FechaPago > contrato.FechaFin)
                return (false, "La fecha de pago no puede ser posterior al fin del contrato.", "warning");

            // Generar número de pago automáticamente si no se proporciona
            if (string.IsNullOrWhiteSpace(pagoDto.NumeroPago))
            {
                pagoDto.NumeroPago = await _pagoRepository.GenerateNumeroAsync(pagoDto.IdContrato);
            }

            // Validar que el importe no exceda significativamente el monto mensual
            var diferencia = Math.Abs(pagoDto.Importe - contrato.MontoMensual);
            var porcentajeDiferencia = (diferencia / contrato.MontoMensual) * 100;

            if (porcentajeDiferencia > 50) // Permitir hasta 50% de diferencia
            {
                return (false, $"El importe del pago ({pagoDto.Importe:C}) difiere significativamente del monto mensual del contrato ({contrato.MontoMensual:C}). Verifique el monto.", "warning");
            }

            // Crear el modelo Pago
            var pago = new Pago
            {
                IdContrato = pagoDto.IdContrato,
                IdUsuario = pagoDto.IdUsuario,
                FechaPago = pagoDto.FechaPago,
                NumeroPago = pagoDto.NumeroPago,
                Importe = pagoDto.Importe,
                Concepto = pagoDto.Concepto,
                EstadoPago = pagoDto.EstadoPago
            };

            var (exito, mensaje, pagoCreado) = await _pagoRepository.AddAsync(pago);

            if (!exito)
                return (false, mensaje, "error");

            return (true, "Pago registrado con éxito.", "success");
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear el pago", ex);
        }
    }

    public async Task<(IEnumerable<PagoDTO> Pagos, int Total)> ObtenerTodosAsync(int page, int pageSize, string? search = null)
    {
        try
        {
            var (pagos, total) = await _pagoRepository.GetAllAsync(page, pageSize, search ?? "");
            return (pagos, total);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener los pagos", ex);
        }
    }
    public async Task<(Pago pago, string mensaje, string tipo)> ObtenerIdAsync(int pagoId)
    {
        try
        {
            var (exito, mensaje, pago) = await _pagoRepository.GetByIdAsync(pagoId);

            if (!exito || pago == null)
                return (null, "Pago no encontrado.", "warning");

            return (pago, "Pago obtenido con éxito.", "success");
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener el pago por ID", ex);
        }
    }
    public async Task<(bool exito, string mensaje, string tipo)> EditarAsync(PagoDTO pagoDto)
    {
        try
        {
            // Obtener el pago actual
            var (pagoActual, _, _) = await ObtenerIdAsync(pagoDto.IdPago);

            if (pagoActual == null)
                return (false, "El pago no existe.", "warning");

            // Solo permitir editar pagos aprobados
            if (pagoActual.EstadoPago != "aprobado")
                return (false, "Solo se pueden editar pagos en estado aprobado.", "warning");

            // Validaciones similares al crear
            if (pagoDto.Importe <= 0)
                return (false, "El importe debe ser mayor a 0.", "warning");

            if (pagoDto.FechaPago > DateTime.Today)
                return (false, "La fecha de pago no puede ser futura.", "warning");

            // Validación de cambios
            var campos = new[] { "FechaPago", "NumeroPago", "Importe", "Concepto" };
            bool hayCambios = false;

            foreach (var campo in campos)
            {
                var valorActual = pagoActual.GetType().GetProperty(campo)?.GetValue(pagoActual)?.ToString();
                var valorNuevo = typeof(PagoDTO).GetProperty(campo)?.GetValue(pagoDto)?.ToString();

                if (!Equals(valorActual, valorNuevo))
                {
                    hayCambios = true;
                    break;
                }
            }

            var notificaciones = new List<string>();

            if (hayCambios)
            {
                pagoActual.FechaPago = pagoDto.FechaPago;
                pagoActual.NumeroPago = pagoDto.NumeroPago;
                pagoActual.Importe = pagoDto.Importe;
                pagoActual.Concepto = pagoDto.Concepto;

                var (exito, mensaje) = await _pagoRepository.UpdateAsync(pagoActual);

                if (!exito)
                    return (false, mensaje, "error");

                notificaciones.Add("Datos del pago actualizados correctamente");
            }

            if (notificaciones.Count == 0)
                return (true, "No hubo cambios.", "info");

            return (true, string.Join(". ", notificaciones), "success");
        }
        catch (Exception ex)
        {
            throw new Exception("Error al actualizar el pago", ex);
        }
    }
    public async Task<(bool exito, string mensaje, string tipo)> AnularAsync(int pagoId, int usuarioId)
    {
        try
        {
            var (pagoActual, _, _) = await ObtenerIdAsync(pagoId);
            if (pagoActual == null)
                return (false, "El pago no existe.", "warning");

            if (pagoActual.EstadoPago != "aprobado")
                return (false, "Solo se pueden anular pagos en estado aprobado.", "warning");

            var (exito, mensaje) = await _pagoRepository.DeleteAsync(pagoId, usuarioId);

            if (!exito)
                return (false, mensaje, "error");

            return (true, "Pago anulado correctamente.", "danger");
        }
        catch (Exception ex)
        {
            throw new Exception("Error al anular el pago", ex);
        }
    }
    public async Task<IEnumerable<PagoDTO>> ListarPorContratoAsync(int contratoId)
    {
        try
        {
            return await _pagoRepository.GetByContratoAsync(contratoId);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al listar pagos por contrato", ex);
        }
    }
    public async Task<int> ContarPagosPorContratoAsync(int contratoId)
    {
        try
        {
            return await _pagoRepository.CountByContratoAsync(contratoId);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al contar pagos del contrato", ex);
        }
    }
    public async Task<decimal> ObtenerTotalPagadoAsync(int contratoId)
    {
        try
        {
            return await _pagoRepository.GetTotalPagadoAsync(contratoId);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener el total pagado", ex);
        }
    }
    public async Task<string> GenerarNumeroAutomaticoAsync(int contratoId)
    {
        try
        {
            return await _pagoRepository.GenerateNumeroAsync(contratoId);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al generar número de pago", ex);
        }
    }
    public async Task<(bool puedeRealizarPago, string razon)> ValidarPosibilidadPagoAsync(int contratoId, decimal importe)
    {
        try
        {
            // Verificar que el contrato existe y está vigente
            var contrato = await _contratoRepository.GetByIdAsync(contratoId);
            if (contrato == null)
                return (false, "El contrato no existe");

            if (contrato.EstadoContrato != "vigente")
                return (false, "El contrato no está vigente");

            // Verificar si ya se pagaron todos los meses del contrato
            var totalPagos = await _pagoRepository.CountByContratoAsync(contratoId);
            var duracionMeses = ((contrato.FechaFin.Year - contrato.FechaInicio.Year) * 12) +
                               contrato.FechaFin.Month - contrato.FechaInicio.Month;

            if (totalPagos >= duracionMeses)
                return (false, "Ya se registraron todos los pagos correspondientes al período del contrato");

            // Validar importe
            var diferencia = Math.Abs(importe - contrato.MontoMensual);
            var porcentajeDiferencia = (diferencia / contrato.MontoMensual) * 100;

            if (porcentajeDiferencia > 50)
                return (false, $"El importe difiere significativamente del monto mensual ({contrato.MontoMensual:C})");

            return (true, "El pago puede ser procesado");
        }
        catch (Exception ex)
        {
            throw new Exception("Error al validar posibilidad de pago", ex);
        }
    }

   
}