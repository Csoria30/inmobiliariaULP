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


    public async Task<IEnumerable<Inquilino>> GetAllAsync()
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 	
	            i.id_inquilino, 	
	            i.id_persona, 
	            per.dni, 
	            per.apellido, 
	            per.nombre, 
	            per.telefono, 
	            per.email
	
	            FROM inquilinos i JOIN personas per 
	            ON i.id_persona = per.id_persona
        ";

        using var reader = await command.ExecuteReaderAsync();
        var inquilinos = new List<Inquilino>();
        while (await reader.ReadAsync())
        {
            inquilinos.Add(new Inquilino
            {
                Dni = reader.GetString("dni"),
                Apellido = reader.GetString("apellido"),
                Nombre = reader.GetString("nombre"),
                Telefono = reader.GetString("telefono"),
                Email = reader.GetString("email")
            });
        }
        return inquilinos;
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