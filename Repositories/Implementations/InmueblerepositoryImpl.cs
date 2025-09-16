using System.Data;
using inmobiliariaULP.Models;
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
                        Estado = reader.GetByte("estado"),
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
}