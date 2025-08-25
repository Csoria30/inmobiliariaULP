using System.Data;
using inmobiliariaULP.Models;
using MySql.Data.MySqlClient;
using inmobiliariaULP.Repositories.Interfaces;
namespace inmobiliariaULP.Repositories.Implementations;

public class PersonaRepositoryImpl(IConfiguration configuration) : BaseRepository(configuration), IPersonaRepository
{

    public async Task<int> AddAsync(Persona persona)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO personas (dni, apellido, nombre, telefono, email) 
                                VALUES (@Dni, @Apellido, @Nombre, @Telefono, @Email);
                                SELECT LAST_INSERT_ID();";
        
        command.Parameters.AddWithValue("@Dni", persona.Dni);
        command.Parameters.AddWithValue("@Apellido", persona.Apellido);
        command.Parameters.AddWithValue("@Nombre", persona.Nombre);
        command.Parameters.AddWithValue("@Telefono", persona.Telefono);
        command.Parameters.AddWithValue("@Email", persona.Email);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM personas WHERE id_persona = @Id";
        command.Parameters.AddWithValue("@Id", id);

        return await command.ExecuteNonQueryAsync();
    }

    public async Task<IEnumerable<Persona>> GetAllAsync()
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT dni, apellido, nombre, telefono, email FROM personas";

        using var reader = await command.ExecuteReaderAsync();
        var personas = new List<Persona>();
        while (await reader.ReadAsync())
        {
            personas.Add(new Persona
            {
                Dni = reader.GetString("dni"),
                Apellido = reader.GetString("apellido"),
                Nombre = reader.GetString("nombre"),
                Telefono = reader.GetString("telefono"),
                Email = reader.GetString("email")
            });
        }

        return personas;
    }

    public async Task<Persona?> GetByIdAsync(int id)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT dni, apellido, nombre, telefono, email FROM personas WHERE id_persona = @Id";
        command.Parameters.AddWithValue("@Id", id);
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Persona
            {
                Dni = reader.GetString("dni"),
                Apellido = reader.GetString("apellido"),
                Nombre = reader.GetString("nombre"),
                Telefono = reader.GetString("telefono"),
                Email = reader.GetString("email")
            };
        }

        return null;
    }

    public async Task<int> UpdateAsync(Persona persona)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = @"UPDATE personas 
                                SET dni = @Dni, apellido = @Apellido, nombre = @Nombre, telefono = @Telefono, email = @Email 
                                WHERE id_persona = @Id";

        command.Parameters.AddWithValue("@Dni", persona.Dni);
        command.Parameters.AddWithValue("@Apellido", persona.Apellido);
        command.Parameters.AddWithValue("@Nombre", persona.Nombre);
        command.Parameters.AddWithValue("@Telefono", persona.Telefono);
        command.Parameters.AddWithValue("@Email", persona.Email);
        command.Parameters.AddWithValue("@Id", persona.PersonaId);

        return await command.ExecuteNonQueryAsync();
    }

}   