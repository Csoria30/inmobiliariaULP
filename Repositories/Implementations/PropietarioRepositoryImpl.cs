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

    public async Task<IEnumerable<Propietario>> GetAllAsync()
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 
	            pr.id_propietario, 
	            pr.id_persona, 
	            per.dni, 
	            per.apellido, 
	            per.nombre, 
	            per.telefono, 
	            per.email
            
	            FROM propietarios pr JOIN personas per 
	            ON pr.id_persona = per.id_persona
        ";

        using var reader = await command.ExecuteReaderAsync();
        var propietarios = new List<Propietario>();
        while (await reader.ReadAsync())
        {
            propietarios.Add(new Propietario
            {
                Dni = reader.GetString("dni"),
                Apellido = reader.GetString("apellido"),
                Nombre = reader.GetString("nombre"),
                Telefono = reader.GetString("telefono"),
                Email = reader.GetString("email")
            });
        }
        return propietarios;
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
	        WHERE id_persona = @PropietarioId;
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
}   