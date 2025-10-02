using System.Data;
using inmobiliariaULP.Models;
using MySql.Data.MySqlClient;
using inmobiliariaULP.Repositories.Interfaces;
namespace inmobiliariaULP.Repositories.Implementations;

public class PropietarioRepositoryImpl(IConfiguration configuration) : BaseRepository(configuration), IPropietarioRepository
{
    public async Task<int> AddAsync(int personaId)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO propietarios (id_persona) VALUES (@PersonaId);
            SELECT LAST_INSERT_ID();
        ";

        command.Parameters.AddWithValue("@PersonaId", personaId);
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<int> DeleteAsync(int propietarioId)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            DELETE FROM propietarios 
            WHERE id_propietario = @PropietarioId
        ";

        command.Parameters.AddWithValue("@PropietarioId", propietarioId);
        return await command.ExecuteNonQueryAsync();

    }

    public async Task<(IEnumerable<Propietario> Personas, int Total)> GetAllAsync(int page, int pageSize, string? search = null)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        // 1. Armar el WHERE si hay búsqueda
        string where = "";
        if (!string.IsNullOrEmpty(search))
        {
            where = @"
                WHERE p.dni LIKE @search OR
                        p.apellido LIKE @search OR
                        p.nombre LIKE @search";
        }

        // 2. Obtener el total de registros (filtrado si hay búsqueda)
        int total;
        using (var countCommand = connection.CreateCommand())
        {
            countCommand.CommandText = $@"
                SELECT COUNT(*) 
                FROM personas p
                INNER JOIN propietarios pr ON p.id_persona = pr.id_persona
                {where}
            ";

            if (!string.IsNullOrEmpty(search))
                countCommand.Parameters.AddWithValue("@search", $"%{search}%");

            total = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
        }

        // 3. Consulta paginada y filtrada
        var propietarios = new List<Propietario>();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = $@"
                Select pr.id_propietario, pr.id_persona, pr.estado AS EstadoPropietario, p.dni, p.nombre, p.apellido, p.telefono, p.email
                
                from propietarios pr
                Join personas p 
                On p.id_persona = pr.id_persona
                {where}
                LIMIT @Offset, @PageSize
            ";

            if (!string.IsNullOrEmpty(search))
                command.Parameters.AddWithValue("@search", $"%{search}%");

            command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
            command.Parameters.AddWithValue("@PageSize", pageSize);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                propietarios.Add(new Propietario
                {
                    PropietarioId = reader.GetInt32("id_propietario"),
                    PersonaId = reader.GetInt32("id_persona"),
                    Dni = reader.GetString("dni"),
                    Apellido = reader.GetString("apellido"),
                    Nombre = reader.GetString("nombre"),
                    Telefono = reader.GetString("telefono"),
                    Email = reader.GetString("email"),
                    Estado = reader.GetBoolean("EstadoPropietario"),
                });
            }
        }

        return (propietarios, total);
    }

    public async Task<Propietario?> GetByIdAsync(int propietarioId)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 
	            id_propietario, 
	            id_persona, 
	            estado 
	
	        FROM propietarios 
	        WHERE id_propietario = @PropietarioId;
        ";

        command.Parameters.AddWithValue("@PropietarioId", propietarioId);
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Propietario
            {
                PropietarioId = reader.GetInt32("id_propietario"),
                PersonaId = reader.GetInt32("id_persona"),
                Estado = reader.GetBoolean("estado")
            };
        }

        return null;
    }

    public async Task<int> UpdateAsync(int propietarioId, bool estado)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE propietarios 

            SET estado = @Estado 
            WHERE id_propietario = @PropietarioId
        ";

        command.Parameters.AddWithValue("@Estado", estado ? 1 : 0);
        command.Parameters.AddWithValue("@PropietarioId", propietarioId);
        return await command.ExecuteNonQueryAsync();
    }

    public async Task<IEnumerable<Propietario>> ListActiveAsync(string term)
    {
        var propietarios = new List<Propietario>();

        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT
                pr.id_propietario, 
                p.id_persona, 
                p.apellido, 
                p.nombre
            FROM personas p

            INNER JOIN propietarios pr 
                ON p.id_persona = pr.id_persona

            WHERE pr.estado = 1 AND (p.apellido LIKE @Term OR p.nombre LIKE @Term)
            LIMIT 10;
        ";

        command.Parameters.AddWithValue("@Term", $"%{term}%");

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            propietarios.Add(new Propietario
            {
                PropietarioId = reader.GetInt32("id_propietario"),
                PersonaId = reader.GetInt32("id_persona"),
                Apellido = reader.GetString("apellido"),
                Nombre = reader.GetString("nombre")
            });
        }

        return propietarios;
    }
}   