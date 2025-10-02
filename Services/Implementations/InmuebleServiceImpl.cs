using System.Data;
using inmobiliariaULP.Models;
using inmobiliariaULP.Models.ViewModels;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Repositories.Interfaces;
using Google.Protobuf;

namespace inmobiliariaULP.Services.Implementations;

public class InmuebleServiceImpl : IInmuebleService
{

    private readonly IInmuebleRepository _inmuebleRepository;
    private readonly IContratoRepository _contratoRepository;

    public InmuebleServiceImpl(
        IInmuebleRepository inmuebleRepository,
        IContratoRepository contratoRepository
    )
    {
        _inmuebleRepository = inmuebleRepository;
        _contratoRepository = contratoRepository;
    }


    public async Task<(bool exito, string mensaje, string tipo)> CrearAsync(Inmueble inmueble)
    {
        try
        {
            // Validación de negocio
            if (inmueble.TipoId == 0)
                return (false, "Debe seleccionar un tipo de inmueble.", "warning");

            if (inmueble.PropietarioId == null || inmueble.PropietarioId == 0)
                return (false, "Debe seleccionar un propietario.", "warning");

            await _inmuebleRepository.AddAsync(inmueble);

            return (true, "Inmueble creado con éxito.", "success");
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear el inmueble", ex);
        }
    }

    public async Task<(IEnumerable<Inmueble> Inmuebles, int Total)> ObtenerTodosAsync(int page, int pageSize, string? search = null)
    {
        try
        {
            return await _inmuebleRepository.GetAllAsync(page, pageSize, search);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener los inmuebles", ex);
        }
    }

    public async Task<(Inmueble inmueble, string mensaje, string tipo)> ObtenerIdAsync(int inmuebleId)
    {
        try
        {
            var inmueble = await _inmuebleRepository.GetByIdAsync(inmuebleId);

            if (inmueble == null)
                return (null, "Inmueble no encontrado.", "warning");

            return (inmueble, "Inmueble obtenido con éxito.", "success");
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener el inmueble por ID", ex);
        }
    }

    public async Task<(bool exito, string mensaje, string tipo)> EditarAsync(Inmueble inmueble)
    {
        try
        {

            var inmuebleActual = await _inmuebleRepository.GetByIdAsync(inmueble.InmuebleId);

            if (inmuebleActual == null)
                return (false, "El inmueble no existe.", "warning");

            // Validacion de cambios
            var campos = new[] { "Direccion", "Uso", "Ambientes", "Coordenadas", "PrecioBase", "PropietarioId", "TipoId" };

            bool hayCambios = false;

            foreach (var campo in campos)
            {
                var valorActual = inmuebleActual.GetType().GetProperty(campo)?.GetValue(inmuebleActual)?.ToString();
                var valorNuevo = inmueble.GetType().GetProperty(campo)?.GetValue(inmueble)?.ToString();

                if (!Equals(valorActual, valorNuevo))
                {
                    hayCambios = true;
                    break;
                }
            }

            var notificaciones = new List<string>();

            if (hayCambios)
            {
                inmuebleActual.Direccion = inmueble.Direccion;
                inmuebleActual.Uso = inmueble.Uso;
                inmuebleActual.Ambientes = inmueble.Ambientes;
                inmuebleActual.Coordenadas = inmueble.Coordenadas;
                inmuebleActual.PrecioBase = inmueble.PrecioBase;
                inmuebleActual.PropietarioId = inmueble.PropietarioId;
                inmuebleActual.TipoId = inmueble.TipoId;

                await _inmuebleRepository.UpdateAsync(
                    inmuebleActual
                );

                notificaciones.Add("Datos actualizados correctamente");
            }

            if (notificaciones.Count == 0)
                return (true, "No hubo cambios.", "info");

            var tipo = notificaciones.Any(n => n.Contains("deshabilitado")) ? "danger" : "success";
            return (true, string.Join(". ", notificaciones), tipo);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al actualizar el inmueble", ex);
        }
    }

    public async Task<(bool exito, string mensaje, string tipo)> CambiarEstadoAsync(int inmuebleId)
    {
        try
        {
            var inmuebleActual = await _inmuebleRepository.GetByIdAsync(inmuebleId);
            if (inmuebleActual == null)
                return (false, "El inmueble no existe.", "warning");

            bool nuevoEstado = !(inmuebleActual.Estado ?? false);
            inmuebleActual.Estado = nuevoEstado;

            await _inmuebleRepository.DeleteAsync(inmuebleActual.InmuebleId, nuevoEstado);


            string mensaje = nuevoEstado ? "Inmueble habilitado correctamente." : "Inmueble deshabilitado correctamente.";
            string tipo = nuevoEstado ? "success" : "danger";

            return (true, mensaje, tipo);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al eliminar el inmueble", ex);
        }
    }

    public async Task<IEnumerable<InmueblePropietarioDTO>> ListarActivosAsync(string term)
    {
        try
        {
            return await _inmuebleRepository.ListActiveAsync(term);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al listar inmuebles activos", ex);
        }
    }

    public async Task<bool> EstaDisponibleEnFechasAsync(int inmuebleId, DateTime fechaInicio, DateTime fechaFin)
    {
        try
        {
            // Primero verificar que el inmueble esté habilitado
            var (inmuebleContrato, _, _) = await ObtenerIdAsync(inmuebleId);
            if (inmuebleContrato == null || !inmuebleContrato.Estado.HasValue || !inmuebleContrato.Estado.Value)
                return false;

            // Verificar si tiene contratos vigentes en esas fechas
            var tieneContratosVigentes = await _contratoRepository.ExisteContratoVigenteAsync(inmuebleId, fechaInicio, fechaFin);

            return !tieneContratosVigentes;

        }
        catch (Exception ex)
        {
            throw new Exception("Error al verificar disponibilidad del inmueble", ex);
        }
    }

    public async Task<string> ObtenerEstadoDisponibilidadAsync(int inmuebleId, DateTime fechaInicio, DateTime fechaFin)
    {
        try
        {
            // Obtener el inmueble
            var (inmueble, _, _) = await ObtenerIdAsync(inmuebleId);

            if (inmueble == null)
                return "error";

            // Verificar si el inmueble está habilitado
            if (!inmueble.Estado.HasValue || !inmueble.Estado.Value)
                return "deshabilitado";

            // Verificar si está disponible en las fechas solicitadas
            var estaDisponible = await EstaDisponibleEnFechasAsync(inmuebleId, fechaInicio, fechaFin);

            return estaDisponible ? "disponible" : "ocupado";
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener el estado de disponibilidad del inmueble", ex);
        }
    }

    public async Task<IEnumerable<InmuebleDisponibilidadDTO>> BuscarDisponiblesAsync(DateTime fechaInicio, DateTime fechaFin, string? uso = null, string? ambientes = null, string? precioMin = null, string? precioMax = null)
    {
        try
        {
            // Validación de fechas
            if (fechaInicio >= fechaFin)
            {
                throw new ArgumentException("La fecha de fin debe ser posterior a la fecha de inicio");
            }


            // CONVERTIR y validar parametros 
            int? ambientesInt = null;
            if (!string.IsNullOrEmpty(ambientes))
            {
                if (!int.TryParse(ambientes, out int amb))
                {
                    throw new ArgumentException("El número de ambientes debe ser un valor numérico válido");
                }
                
                if (amb < 1 || amb > 20)
                {
                    throw new ArgumentException("El número de ambientes debe estar entre 1 y 20");
                }
                
                ambientesInt = amb;
            }

            decimal? precioMinDecimal = null;
            if (!string.IsNullOrEmpty(precioMin))
            {
                if (!decimal.TryParse(precioMin, out decimal pMin))
                {
                    throw new ArgumentException("El precio mínimo debe ser un valor numérico válido");
                }
                
                if (pMin < 0)
                {
                    throw new ArgumentException("El precio mínimo no puede ser negativo");
                }
                
                if (pMin > 10_000_000)
                {
                    throw new ArgumentException("El precio mínimo no puede exceder $10,000,00");
                }
                
                precioMinDecimal = pMin;
            }

            decimal? precioMaxDecimal = null;
            if (!string.IsNullOrEmpty(precioMax))
            {
                if (!decimal.TryParse(precioMax, out decimal pMax))
                {
                    throw new ArgumentException("El precio máximo debe ser un valor numérico válido");
                }
                
                if (pMax < 0)
                {
                    throw new ArgumentException("El precio máximo no puede ser negativo");
                }
                
                if (pMax > 10_000_000)
                {
                    throw new ArgumentException("El precio máximo no puede exceder $10,000,00");
                }
                
                precioMaxDecimal = pMax;
            }

            // Rangos de precios
            if (precioMinDecimal.HasValue && precioMaxDecimal.HasValue && precioMinDecimal >= precioMaxDecimal)
            {
                throw new ArgumentException("El precio máximo debe ser mayor al precio mínimo");
            }

            // Validacion _Uso
            if (!string.IsNullOrEmpty(uso))
            {
                var usosValidos = new[] { "residencial", "comercial", "industrial", "oficina", "deposito", "otro" };
                if (!usosValidos.Contains(uso.ToLower()))
                {
                    throw new ArgumentException($"El uso del inmueble debe ser uno de: {string.Join(", ", usosValidos)}");
                }
            }


            // Duracion del contrato
            var duracion = fechaFin - fechaInicio;
            if (duracion.TotalDays < 30)
            {
                throw new ArgumentException("El período de búsqueda debe ser de al menos 30 días");
            }

            if (duracion.TotalDays > 1095) // 3 años
            {
                throw new ArgumentException("El período de búsqueda no puede exceder 3 años");
            }

            // Repository
            var resultados = await _inmuebleRepository.SearchDisponiblesAsync(
                fechaInicio, 
                fechaFin, 
                uso, 
                ambientesInt, 
                precioMinDecimal, 
                precioMaxDecimal);

            
            var listaResultados = resultados.ToList();
            Console.WriteLine($"Servicio: Búsqueda completada - {listaResultados.Count} inmuebles encontrados");
            
            return listaResultados;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al buscar inmuebles disponibles", ex);
        }

    }
    


}