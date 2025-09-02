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
        command.CommandText = @"
            INSERT INTO personas (dni, apellido, nombre, telefono, email) 
            VALUES (@Dni, @Apellido, @Nombre, @Telefono, @Email);
            
            SELECT LAST_INSERT_ID();
        ";
        
        command.Parameters.AddWithValue("@Dni", persona.Dni);
        command.Parameters.AddWithValue("@Apellido", persona.Apellido);
        command.Parameters.AddWithValue("@Nombre", persona.Nombre);
        command.Parameters.AddWithValue("@Telefono", persona.Telefono);
        command.Parameters.AddWithValue("@Email", persona.Email);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<int> DeleteAsync(int personaId, bool estado)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE personas
            SET estado = @Estado 
            WHERE id_persona = @PersonaId
        ";
        
        // Asignamos los parámetros
        command.Parameters.AddWithValue("@Estado", estado ? 1 : 0);
        command.Parameters.AddWithValue("@PersonaId", personaId);

        return await command.ExecuteNonQueryAsync();
    }
    
    

    public async Task<IEnumerable<Persona>> GetAllAsync()
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT p.id_persona, p.dni, p.apellido, p.nombre, p.telefono, p.email, p.estado,
               i.id_inquilino, pr.id_propietario
        
            FROM personas p
            LEFT JOIN inquilinos i 
                ON p.id_persona = i.id_persona AND i.estado = 1

            LEFT JOIN propietarios pr 
                ON p.id_persona = pr.id_persona AND pr.estado = 1;
        ";

        using var reader = await command.ExecuteReaderAsync();
        var personas = new List<Persona>();

        while (await reader.ReadAsync())
        {
            var tipoPersonas = new List<string>();

            if (!reader.IsDBNull(reader.GetOrdinal("id_inquilino")))
                tipoPersonas.Add("inquilino");

            if (!reader.IsDBNull(reader.GetOrdinal("id_propietario")))
                tipoPersonas.Add("propietario");

            personas.Add(new Persona
            {
                PersonaId = reader.GetInt32("id_persona"),
                Dni = reader.GetString("dni"),
                Apellido = reader.GetString("apellido"),
                Nombre = reader.GetString("nombre"),
                Telefono = reader.GetString("telefono"),
                Email = reader.GetString("email"),
                Estado = reader.GetBoolean("estado"),
                TipoPersona = new List<string>(tipoPersonas)
            });
        }

        return personas;
    }

    public async Task<Persona?> GetByIdAsync(int id)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT p.id_persona, p.dni, p.apellido, p.nombre, p.telefono, p.email, p.estado,
               i.id_inquilino, pr.id_propietario

            FROM personas p
            LEFT JOIN inquilinos i 
                ON p.id_persona = i.id_persona AND i.estado = 1
            LEFT JOIN propietarios pr 
                ON p.id_persona = pr.id_persona AND pr.estado = 1
            WHERE p.id_persona = @Id;
        ";

        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();

        // Valida si no se encontraron resultados
        if (!await reader.ReadAsync()) return null;

        // Crea lista de tipos de persona
        var tipoPersonas = new List<string>();

        //Valida si es inquilino o propietario
        if (!reader.IsDBNull(reader.GetOrdinal("id_inquilino")))
            tipoPersonas.Add("inquilino");
            
        if( !reader.IsDBNull(reader.GetOrdinal("id_propietario")) )
            tipoPersonas.Add("propietario");

        // Crea y retorna el objeto Persona
        var persona = new Persona
        {
            PersonaId = reader.GetInt32("id_persona"),
            Dni = reader.GetString("dni"),
            Apellido = reader.GetString("apellido"),
            Nombre = reader.GetString("nombre"),
            Telefono = reader.GetString("telefono"),
            Email = reader.GetString("email"),
            Estado = reader.GetBoolean("estado"),
            TipoPersona = new List<string>(tipoPersonas)
        };

        return persona;
    }

    public async Task<int> UpdateAsync(Persona persona)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE personas 
            SET dni = @Dni, apellido = @Apellido, nombre = @Nombre, telefono = @Telefono, email = @Email 
            WHERE id_persona = @Id
        ";

        command.Parameters.AddWithValue("@Dni", persona.Dni);
        command.Parameters.AddWithValue("@Apellido", persona.Apellido);
        command.Parameters.AddWithValue("@Nombre", persona.Nombre);
        command.Parameters.AddWithValue("@Telefono", persona.Telefono);
        command.Parameters.AddWithValue("@Email", persona.Email);
        command.Parameters.AddWithValue("@Id", persona.PersonaId);

        return await command.ExecuteNonQueryAsync();
    }

}   