using System.Data;
using inmobiliariaULP.Models;
using inmobiliariaULP.Models.ViewModels;
using MySql.Data.MySqlClient;
using inmobiliariaULP.Repositories.Interfaces;

namespace inmobiliariaULP.Repositories.Implementations;

public class InmuebleRepositoryImpl(IConfiguration configuration) : BaseRepository(configuration), IInmuebleRepository
{
    public async Task<int> AddAsync(Inmueble inmueble)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "sp_InsertInmueble";

        // Parámetros de entrada
        command.Parameters.AddWithValue("@p_direccion", inmueble.Direccion);
        command.Parameters.AddWithValue("@p_uso", inmueble.Uso);
        command.Parameters.AddWithValue("@p_ambientes", inmueble.Ambientes);
        command.Parameters.AddWithValue("@p_coordenadas", inmueble.Coordenadas);
        command.Parameters.AddWithValue("@p_precio_base", inmueble.PrecioBase);
        command.Parameters.AddWithValue("@p_id_propietario", inmueble.PropietarioId);
        command.Parameters.AddWithValue("@p_id_tipo", inmueble.TipoId);

        var outputParam = new MySqlParameter("@p_inmueble_id", MySqlDbType.Int32)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(outputParam);

        await command.ExecuteNonQueryAsync();
        return Convert.ToInt32(outputParam.Value);
    }

    public async Task<(IEnumerable<Inmueble> Inmuebles, int Total)> GetAllAsync(int page, int pageSize, string? search = null)
    {
        try
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            // 1. Armar el WHERE si hay búsqueda
            string where = "";
            if (!string.IsNullOrEmpty(search))
            {
                where = @"
                    WHERE 
	                i.direccion LIKE @search OR 
	                i.uso LIKE @search"
                ;
            }

            // 2. Obtener el total de registros (filtrado si hay búsqueda)
            int total;
            using (var countCommand = connection.CreateCommand())
            {
                countCommand.CommandText = $@"SELECT COUNT(*) FROM inmuebles i {where}";

                if (!string.IsNullOrEmpty(search))
                    countCommand.Parameters.AddWithValue("@search", $"%{search}%");

                total = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
            }

            // 3. Consulta paginada y filtrada
            var inmuebles = new List<Inmueble>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $@"
                    SELECT 	
                        i.id_inmueble, i.direccion, i.uso, i.ambientes, 
                        i.coordenadas, i.precio_base, i.estado, 
                        i.id_propietario, p.nombre,p.apellido,
                        i.id_tipo, t.descripcion
    
                        FROM inmuebles i

                        JOIN propietarios pr
                        	ON pr.id_propietario = i.id_propietario
                        JOIN personas p
                        	ON p.id_persona = pr.id_persona
                        JOIN tipos t
                        	ON i.id_tipo = t.id_tipo
    
                        {where}
                        LIMIT @PageSize OFFSET @Offset
                ";

                if (!string.IsNullOrEmpty(search))
                    command.Parameters.AddWithValue("@search", $"%{search}%");

                command.Parameters.AddWithValue("@PageSize", pageSize);
                command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var inmueble = new Inmueble
                    {
                        InmuebleId = reader.GetInt32("id_inmueble"),
                        Direccion = reader.GetString("direccion"),
                        Uso = reader.GetString("uso"),
                        Ambientes = reader.GetInt32("ambientes"),
                        Coordenadas = reader.GetString("coordenadas"),
                        PrecioBase = reader.GetDecimal("precio_base"),
                        Estado = reader.GetBoolean("estado"),
                        PropietarioId = reader.GetInt32("id_propietario"),
                        TipoId = reader.GetInt32("id_tipo"),
                        TipoDescripcion = reader.GetString("descripcion")
                    };
                    inmuebles.Add(inmueble);
                }
            }

            return (inmuebles, total);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener los inmuebles", ex);
        }
    }

    public async Task<Inmueble> GetByIdAsync(int inmuebleId)
    {
        try
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "sp_GetInmuebleById";

            command.Parameters.AddWithValue("@p_inmueble_id", inmuebleId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Inmueble
                {
                    InmuebleId = reader.GetInt32("id_inmueble"),
                    Direccion = reader.GetString("direccion"),
                    Uso = reader.GetString("uso"),
                    Ambientes = reader.GetInt32("ambientes"),
                    Coordenadas = reader.GetString("coordenadas"),
                    PrecioBase = reader.GetDecimal("precio_base"),
                    Estado = reader.GetBoolean("estado"),
                    PropietarioId = reader.GetInt32("id_propietario"),
                    TipoId = reader.GetInt32("id_tipo"),
                    TipoDescripcion = reader.GetString("descripcion"),
                    PropietarioNombre = reader.GetString("apellido") + ", " + reader.GetString("nombre")
                };
            }
            else
            {
                throw new Exception("Inmueble no encontrado");
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener el inmueble por ID", ex);
        }
    }

    public async Task<int> UpdateAsync(Inmueble inmueble)
    {
        try
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "sp_UpdateInmueble";

            // Parámetros del procedimiento
            command.Parameters.AddWithValue("@p_id_inmueble", inmueble.InmuebleId);
            command.Parameters.AddWithValue("@p_direccion", inmueble.Direccion);
            command.Parameters.AddWithValue("@p_uso", inmueble.Uso);
            command.Parameters.AddWithValue("@p_ambientes", inmueble.Ambientes);
            command.Parameters.AddWithValue("@p_coordenadas", inmueble.Coordenadas);
            command.Parameters.AddWithValue("@p_precio_base", inmueble.PrecioBase);
            command.Parameters.AddWithValue("@p_id_propietario", inmueble.PropietarioId);
            command.Parameters.AddWithValue("@p_id_tipo", inmueble.TipoId);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al actualizar el inmueble", ex);
        }
    }

    public async Task<int> DeleteAsync(int inmuebleId, bool estado)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "sp_UpdateInmuebleEstado";

        command.Parameters.AddWithValue("@p_inmueble_id", inmuebleId);
        command.Parameters.AddWithValue("@p_estado", estado ? 1 : 0);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<IEnumerable<InmueblePropietarioDTO>> ListActiveAsync(string term)
    {
        var inmuebles = new List<InmueblePropietarioDTO>();

        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "sp_ListInmueblesActivos";

        command.Parameters.AddWithValue("@p_term", term ?? string.Empty);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var inmueble = new InmueblePropietarioDTO
            {
                InmuebleId = reader.GetInt32("InmuebleId"),
                Direccion = reader.GetString("Direccion"),
                UsoInmueble = reader.GetString("Uso"),
                Ambientes = reader.GetInt32("Ambientes"),
                Coordenadas = reader.GetString("Coordenadas"),
                PrecioBase = reader.GetDecimal("PrecioBase"),
                EstadoInmueble = reader.GetBoolean("EstadoInmueble"),
                TipoId = reader.GetInt32("Tipo"),
                TipoInmueble = reader.GetString("Descripcion"),
                PropietarioId = reader.GetInt32("PropietarioId"),
                NombrePropietario = reader.GetString("NombrePropietario"),
                EmailPropietario = reader.GetString("Email"),
                TelefonoPropietario = reader.GetString("Telefono")
            };
            inmuebles.Add(inmueble);
        }

        return inmuebles;
    }

    public async Task<IEnumerable<InmuebleDisponibilidadDTO>> SearchDisponiblesAsync(DateTime fechaInicio, DateTime fechaFin, string? uso = null, int? ambientes = null, decimal? precioMin = null, decimal? precioMax = null)
    {
        var inmueblesDisponibles = new List<InmuebleDisponibilidadDTO>();

        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 	
                i.id_inmueble AS InmuebleId,
                i.direccion AS Direccion,
                t.descripcion AS TipoInmueble,
                i.uso AS UsoInmueble,
                i.ambientes AS Ambientes,
                i.coordenadas AS Coordenadas,
                i.precio_base AS PrecioBase,
                'disponible' AS EstadoDisponibilidad,
                i.id_propietario AS PropietarioId,
                CONCAT(p.apellido, ' ', p.nombre) AS PropietarioNombre,
                p.email AS PropietarioEmail,
                p.telefono AS PropietarioTelefono

            FROM inmuebles i
                JOIN propietarios pr 
                    ON pr.id_propietario = i.id_propietario
                JOIN personas p 
                    ON p.id_persona = pr.id_persona
                JOIN tipos t 
                    ON i.id_tipo = t.id_tipo

            WHERE 
                i.estado = 1 AND 
                pr.estado = 1 AND 
                p.estado = 1 AND
                
                i.id_inmueble NOT IN (
                    SELECT DISTINCT c.id_inmueble 
                    FROM contratos c 
                    WHERE c.estado = 'vigente'
                    AND (c.fecha_inicio <= @fechaFin AND c.fecha_fin >= @fechaInicio)
                )";

        
        command.Parameters.AddWithValue("@fechaInicio", fechaInicio.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("@fechaFin", fechaFin.ToString("yyyy-MM-dd"));

        // FILTROS opcionales
        var filtrosAdicionales = new List<string>();

        if (!string.IsNullOrEmpty(uso))
        {
            filtrosAdicionales.Add("LOWER(i.uso) = LOWER(@uso)");
            command.Parameters.AddWithValue("@uso", uso);
        }

        if (ambientes.HasValue && ambientes.Value > 0)
        {
            filtrosAdicionales.Add("i.ambientes >= @ambientes");
            command.Parameters.AddWithValue("@ambientes", ambientes.Value);
        }

        if (precioMin.HasValue && precioMin.Value > 0)
        {
            filtrosAdicionales.Add("i.precio_base >= @precioMin");
            command.Parameters.AddWithValue("@precioMin", precioMin.Value);
        }

        if (precioMax.HasValue && precioMax.Value > 0)
        {
            filtrosAdicionales.Add("i.precio_base <= @precioMax");
            command.Parameters.AddWithValue("@precioMax", precioMax.Value);
        }

        // Agregra los filtros si existen 
        if (filtrosAdicionales.Count > 0)
        {
            command.CommandText += " AND " + string.Join(" AND ", filtrosAdicionales);
        }

        // OrdeBy
        command.CommandText += " ORDER BY i.direccion;";

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var inmueble = new InmuebleDisponibilidadDTO
            {
                InmuebleId = reader.GetInt32("InmuebleId"),
                Direccion = reader.GetString("Direccion"),
                TipoInmueble = reader.GetString("TipoInmueble"),
                UsoInmueble = reader.GetString("UsoInmueble"),
                Ambientes = reader.GetInt32("Ambientes"),
                PrecioBase = reader.GetDecimal("PrecioBase"),
                EstadoDisponibilidad = reader.GetString("EstadoDisponibilidad"),
                PropietarioId = reader.GetInt32("PropietarioId"),
                PropietarioNombre = reader.GetString("PropietarioNombre"),
                PropietarioEmail = reader.IsDBNull("PropietarioEmail") ? null : reader.GetString("PropietarioEmail"),
                PropietarioTelefono = reader.IsDBNull("PropietarioTelefono") ? null : reader.GetString("PropietarioTelefono")
            };

            inmueblesDisponibles.Add(inmueble);
        }

        return inmueblesDisponibles;
    }
}