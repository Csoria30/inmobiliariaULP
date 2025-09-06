using System.Data;
using inmobiliariaULP.Models;
using MySql.Data.MySqlClient;
using inmobiliariaULP.Repositories.Interfaces;
namespace inmobiliariaULP.Repositories.Implementations;

public class InquilinoRepositoryImpl(IConfiguration configuration) : BaseRepository(configuration), IInquilinoRepository
{
    public async Task<int> AddAsync(int personaId)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO inquilinos (id_persona) VALUES (@PersonaId);
            SELECT LAST_INSERT_ID();
        ";

        command.Parameters.AddWithValue("@PersonaId", personaId);
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<int> DeleteAsync(int inquilinoId)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            DELETE FROM inquilinos 
            WHERE id_propietario = @inquilinoId
        ";

        command.Parameters.AddWithValue("@inquilinoId", inquilinoId);
        return await command.ExecuteNonQueryAsync();
    }


    public async Task<(IEnumerable<Inquilino> Personas, int Total)> GetAllAsync(int page, int pageSize, string? search = null)
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
                INNER JOIN inquilinos i ON p.id_persona = i.id_persona
                {where}
            ";

            if (!string.IsNullOrEmpty(search))
                countCommand.Parameters.AddWithValue("@search", $"%{search}%");

            total = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
        }

        // 3. Consulta paginada y filtrada
        var inquilinos = new List<Inquilino>();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = $@"
                Select i.id_persona, i.id_inquilino , i.estado AS EstadoInquilino, p.dni, p.nombre, p.apellido, p.telefono, p.email

                from inquilinos i 
                Join personas p 
                On p.id_persona = i.id_persona
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
                inquilinos.Add(new Inquilino
                {
                    InquilinoId = reader.GetInt32("id_inquilino"),
                    PersonaId = reader.GetInt32("id_persona"),
                    Dni = reader.GetString("dni"),
                    Apellido = reader.GetString("apellido"),
                    Nombre = reader.GetString("nombre"),
                    Telefono = reader.GetString("telefono"),
                    Email = reader.GetString("email"),
                    Estado = reader.GetBoolean("EstadoInquilino"),
                });
            }
        }
         return (inquilinos, total);
    }

    public async Task<Inquilino?> GetByIdAsync(int inquilinoId)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 
                id_inquilino,
                id_persona, 
                estado 
            
            FROM inquilinos
            WHERE id_persona = @InquilinoId;
        ";

        command.Parameters.AddWithValue("@InquilinoId", inquilinoId);
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Inquilino
            {
                InquilinoId = reader.GetInt32("id_inquilino"),
                PersonaId = reader.GetInt32("id_persona"),
                Estado = reader.GetBoolean("estado")
            };
        }

        return null;
    }

    public async Task<int> UpdateAsync(int inquilinoId, bool estado)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE inquilinos 
            SET estado = @Estado 
            WHERE id_inquilino = @InquilinoId
        ";

        // Asignamos los parámetros
        command.Parameters.AddWithValue("@Estado", estado ? 1 : 0);
        command.Parameters.AddWithValue("@InquilinoId", inquilinoId);

        return await command.ExecuteNonQueryAsync();
    }
}  