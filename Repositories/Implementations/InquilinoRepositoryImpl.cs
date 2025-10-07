using System.Data;
using inmobiliariaULP.Models;
using MySql.Data.MySqlClient;
using inmobiliariaULP.Repositories.Interfaces;
using inmobiliariaULP.Models.ViewModels;
namespace inmobiliariaULP.Repositories.Implementations;

public class InquilinoRepositoryImpl(IConfiguration configuration) : BaseRepository(configuration), IInquilinoRepository
{
    public async Task<int> AddAsync(int personaId)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "sp_InsertInquilino";

        command.Parameters.AddWithValue("@p_id_persona", personaId);

        var outputParam = new MySqlParameter("@p_inquilino_id", MySqlDbType.Int32)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(outputParam);

        await command.ExecuteNonQueryAsync();
        return Convert.ToInt32(outputParam.Value);
    }

    public async Task<int> DeleteAsync(int inquilinoId)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "sp_DeleteInquilino";

        command.Parameters.AddWithValue("@p_inquilino_id", inquilinoId);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
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
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "sp_GetInquilinoById";

        command.Parameters.AddWithValue("@p_persona_id", inquilinoId);

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
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "sp_UpdateInquilinoEstado";

        command.Parameters.AddWithValue("@p_inquilino_id", inquilinoId);
        command.Parameters.AddWithValue("@p_estado", estado ? 1 : 0);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<IEnumerable<InquilinoContratoDTO>> ListActiveAsync(string term)
    {
        var inquilinos = new List<InquilinoContratoDTO>();

        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "sp_ListInquilinosActivos";

        command.Parameters.AddWithValue("@p_term", term ?? string.Empty);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var inquilino = new InquilinoContratoDTO
            {
                InquilinoId = reader.GetInt32("InquilinoId"),
                Dni = reader.GetString("Dni"),
                NombreInquilino = reader.GetString("NombreInquilino"),
                Email = reader.GetString("Email"),
                Telefono = reader.GetString("Telefono")
            };

            inquilinos.Add(inquilino);
        }
        
        return inquilinos;
    }

}  