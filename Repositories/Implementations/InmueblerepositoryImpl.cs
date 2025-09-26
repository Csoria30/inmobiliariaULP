using System.Data;
using inmobiliariaULP.Models;
using inmobiliariaULP.Models.ViewModels;
using MySql.Data.MySqlClient;
using inmobiliariaULP.Repositories.Interfaces;

namespace inmobiliariaULP.Repositories.Implementations;

public class InmuebleRepositoryImpl(IConfiguration configuration) : BaseRepository(configuration), IInmuebleRepository
{
    public async Task<Inmueble> AddAsync(Inmueble inmueble)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO inmuebles
            (direccion, uso, ambientes, coordenadas, precio_base,  id_propietario, id_tipo)
            VALUES
            (@Direccion, @Uso, @Ambientes, @Coordenadas, @PrecioBase,  @IdPropietario, @IdTipo);
        ";

        command.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
        command.Parameters.AddWithValue("@Uso", inmueble.Uso);
        command.Parameters.AddWithValue("@Ambientes", inmueble.Ambientes);
        command.Parameters.AddWithValue("@Coordenadas", inmueble.Coordenadas);
        command.Parameters.AddWithValue("@PrecioBase", inmueble.PrecioBase);
        command.Parameters.AddWithValue("@IdPropietario", inmueble.PropietarioId);
        command.Parameters.AddWithValue("@IdTipo", inmueble.TipoId);

        var result = await command.ExecuteScalarAsync();
        return inmueble;
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
            command.CommandText = @"
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

                WHERE i.id_inmueble = @InmuebleId;
            ";

            command.Parameters.AddWithValue("@InmuebleId", inmuebleId);

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
            command.CommandText = @"
                UPDATE inmuebles
                SET 
                    direccion = @Direccion,
                    uso = @Uso,
                    ambientes = @Ambientes,
                    coordenadas = @Coordenadas,
                    precio_base = @PrecioBase,
                    id_propietario = @IdPropietario,
                    id_tipo = @IdTipo
                WHERE id_inmueble = @InmuebleId;
            ";

            command.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
            command.Parameters.AddWithValue("@Uso", inmueble.Uso);
            command.Parameters.AddWithValue("@Ambientes", inmueble.Ambientes);
            command.Parameters.AddWithValue("@Coordenadas", inmueble.Coordenadas);
            command.Parameters.AddWithValue("@PrecioBase", inmueble.PrecioBase);
            command.Parameters.AddWithValue("@IdPropietario", inmueble.PropietarioId);
            command.Parameters.AddWithValue("@IdTipo", inmueble.TipoId);
            command.Parameters.AddWithValue("@InmuebleId", inmueble.InmuebleId);

            return await command.ExecuteNonQueryAsync();
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
        command.CommandText = @"
            UPDATE inmuebles
            SET estado = @Estado
            WHERE id_inmueble = @InmuebleId;";

        command.Parameters.AddWithValue("@Estado", estado ? 1 : 0);
        command.Parameters.AddWithValue("@InmuebleId", inmuebleId);

        return await command.ExecuteNonQueryAsync();
    }

    public async Task<IEnumerable<InmueblePropietarioDTO>> ListActiveAsync(string term)
    {
        var inmuebles = new List<InmueblePropietarioDTO>();

        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 	
                i.id_inmueble AS InmuebleId, 
                i.direccion AS Direccion, 
                i.uso AS Uso, 
                i.ambientes AS Ambientes, 
                i.coordenadas AS Coordenadas, 
                i.precio_base PrecioBase, 
                i.estado AS EstadoInmueble, 
                i.id_propietario AS PropietarioId, 
                CONCAT(p.apellido, ' ', p.nombre ) AS NombrePropietario,
                i.id_tipo AS Tipo, 
                t.descripcion AS Descripcion,
                p.email AS Email,
                p.telefono AS Telefono

            FROM inmuebles i
                JOIN propietarios pr
                    ON pr.id_propietario = i.id_propietario
                JOIN personas p
                    ON p.id_persona = pr.id_persona
                JOIN tipos t
                    ON i.id_tipo = t.id_tipo

            WHERE 
                (LOWER(i.direccion) LIKE LOWER(@term) OR 
                LOWER(i.uso) LIKE LOWER(@term)) AND 
                i.estado = 1
            LIMIT 10;
        ";

        command.Parameters.AddWithValue("@term", $"%{term}%");

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
}