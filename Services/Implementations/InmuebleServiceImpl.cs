using System.Data;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Repositories.Implementations;
using Google.Protobuf;

namespace inmobiliariaULP.Services.Implementations;

public class InmuebleServiceImpl : IInmuebleService
{


    // Getters Fabrica
    private InmuebleRepositoryImpl GetInmuebleRepository()
    {
        return FactoryRepository.CreateInmuebleRepository();
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

            var inmuebleRepository = GetInmuebleRepository();
            await inmuebleRepository.AddAsync(inmueble);

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
            var inmuebleRepository = GetInmuebleRepository();
            return await inmuebleRepository.GetAllAsync(page, pageSize, search);
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
            var inmuebleRepository = GetInmuebleRepository();
            var inmueble = await inmuebleRepository.GetByIdAsync(inmuebleId);

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
            var inmuebleRepository = GetInmuebleRepository();
            var inmuebleActual = await inmuebleRepository.GetByIdAsync(inmueble.InmuebleId);

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

                await inmuebleRepository.UpdateAsync(
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
            var inmuebleActual = await GetInmuebleRepository().GetByIdAsync(inmuebleId);
            if (inmuebleActual == null)
                return (false, "El inmueble no existe.", "warning");

            bool nuevoEstado = !(inmuebleActual.Estado ?? false);
            inmuebleActual.Estado = nuevoEstado;

            await GetInmuebleRepository().DeleteAsync(inmuebleActual.InmuebleId, nuevoEstado);


            string mensaje = nuevoEstado ? "Inmueble habilitado correctamente." : "Inmueble deshabilitado correctamente.";
            string tipo = nuevoEstado ? "success" : "danger";

            return (true, mensaje, tipo);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al eliminar el inmueble", ex);
        }
    }
}