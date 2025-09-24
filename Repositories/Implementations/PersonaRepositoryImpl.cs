using System.Data;
using MySql.Data.MySqlClient;
using inmobiliariaULP.Repositories.Interfaces;
using inmobiliariaULP.Models;
using inmobiliariaULP.Models.ViewModels;


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

    public async Task<(IEnumerable<Persona> Personas, int Total)> GetAllAsync(int page, int pageSize, string? search = null)
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
            countCommand.CommandText = $@"SELECT COUNT(*) FROM personas p {where}";

            if (!string.IsNullOrEmpty(search))
                countCommand.Parameters.AddWithValue("@search", $"%{search}%");

            total = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
        }

        // 3. Consulta paginada y filtrada
        var personas = new List<Persona>();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $@"
                SELECT p.id_persona, p.dni, p.apellido, p.nombre, p.telefono, p.email, p.estado,
                    i.id_inquilino, pr.id_propietario, e.id_empleado

                FROM personas p

                LEFT JOIN inquilinos i ON p.id_persona = i.id_persona AND i.estado = 1
                LEFT JOIN propietarios pr ON p.id_persona = pr.id_persona AND pr.estado = 1
                LEFT JOIN empleados e ON p.id_persona = e.id_persona AND e.estado = 1

                {where}
                LIMIT @limit OFFSET @offset;
            ";

            if (!string.IsNullOrEmpty(search))
                command.Parameters.AddWithValue("@search", $"%{search}%");

            command.Parameters.AddWithValue("@limit", pageSize);
            command.Parameters.AddWithValue("@offset", (page - 1) * pageSize);

            using var reader = await command.ExecuteReaderAsync();
            var personasDict = new Dictionary<int, Persona>();

            while (await reader.ReadAsync())
            {
                int personaId = reader.GetInt32("id_persona");

                // Evita instancias duplicadas de Persona
                if (!personasDict.TryGetValue(personaId, out var persona))
                {
                    persona = new Persona
                    {
                        PersonaId = reader.GetInt32("id_persona"),
                        Dni = reader.GetString("dni"),
                        Apellido = reader.GetString("apellido"),
                        Nombre = reader.GetString("nombre"),
                        Telefono = reader.GetString("telefono"),
                        Email = reader.GetString("email"),
                        Estado = reader.GetBoolean("estado"),
                        TipoPersona = new List<string>()
                    };
                    personasDict.Add(personaId, persona);
                }
                if (!reader.IsDBNull(reader.GetOrdinal("id_inquilino")) && !persona.TipoPersona.Contains("inquilino"))
                    persona.TipoPersona.Add("inquilino");
                if (!reader.IsDBNull(reader.GetOrdinal("id_propietario")) && !persona.TipoPersona.Contains("propietario"))
                    persona.TipoPersona.Add("propietario");
                if (!reader.IsDBNull(reader.GetOrdinal("id_empleado")) && !persona.TipoPersona.Contains("empleado"))
                    persona.TipoPersona.Add("empleado");

            }

            return (personasDict.Values.ToList(), total);
        }


    }

    public async Task<Persona?> GetByIdAsync(int id)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 
                p.id_persona, 
                p.dni, 
                p.apellido, 
                p.nombre, 
                p.telefono, 
                p.email, 
                p.estado,
                i.id_inquilino, 
                pr.id_propietario, 
                e.id_empleado

                FROM personas p
                    LEFT JOIN inquilinos i 
                        ON p.id_persona = i.id_persona AND i.estado = 1
                    LEFT JOIN propietarios pr 
                        ON p.id_persona = pr.id_persona AND pr.estado = 1
                    LEFT JOIN empleados e 
                        ON p.id_persona = e.id_persona AND e.estado = 1
                WHERE p.id_persona = @Id;
        ";

        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();

        // Valida si no se encontraron resultados
        if (!await reader.ReadAsync()) return null;

        // Crea lista de tipos de persona
        var tipoPersonas = new List<string>();

        //Valida si es inquilino , propietario  o empleado
        if (!reader.IsDBNull(reader.GetOrdinal("id_inquilino")))
            tipoPersonas.Add("inquilino");

        if (!reader.IsDBNull(reader.GetOrdinal("id_propietario")))
            tipoPersonas.Add("propietario");

        if (!reader.IsDBNull(reader.GetOrdinal("id_empleado")))
            tipoPersonas.Add("empleado");

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

    public async Task<PersonaUsuarioDTO> GetDetalleByIdAsync(int id)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 
                p.id_persona AS PersonaId, 
                p.dni AS Dni, 
                p.apellido AS Apellido, 
                p.nombre AS Nombre, 
                p.telefono AS Telefono, 
                p.email AS Email, 
                p.estado AS Estado,
                i.id_inquilino AS InquilinoId, 
                pr.id_propietario AS PropietarioId, 
                e.id_empleado AS EmpleadoId,
                u.id_usuario AS UsuarioId, 
                u.rol AS Rol,
                u.password AS Password
                
            FROM personas p
                LEFT JOIN inquilinos i 
                    ON p.id_persona = i.id_persona
                LEFT JOIN propietarios pr 
                    ON p.id_persona = pr.id_persona
                LEFT JOIN empleados e 
                    ON p.id_persona = e.id_persona
                LEFT JOIN usuarios u 
                    ON e.id_empleado = u.id_empleado
            WHERE p.id_persona = @Id;
        ";

        command.Parameters.AddWithValue("@Id", id);
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        // Crea lista de tipos de persona
        var tipoPersonas = new List<string>();

        //Valida si es inquilino , propietario  o empleado
        if (!reader.IsDBNull(reader.GetOrdinal("InquilinoId")))
            tipoPersonas.Add("inquilino");

        if (!reader.IsDBNull(reader.GetOrdinal("PropietarioId")))
            tipoPersonas.Add("propietario");

        if (!reader.IsDBNull(reader.GetOrdinal("EmpleadoId")))
            tipoPersonas.Add("empleado");

        // Persona
        var persona = new PersonaUsuarioDTO
        {
            PersonaId = reader.GetInt32("PersonaId"),
            Dni = reader.GetString("Dni"),
            Apellido = reader.GetString("Apellido"),
            Nombre = reader.GetString("Nombre"),
            Telefono = reader.GetString("Telefono"),
            Email = reader.GetString("Email"),
            Estado = reader.GetBoolean("Estado"),
            TipoPersona = tipoPersonas,
            Rol = reader.IsDBNull(reader.GetOrdinal("Rol")) ? "" : reader.GetString("Rol"),
            Password = reader.IsDBNull(reader.GetOrdinal("Password")) ? "" : reader.GetString("Password")
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

    public async Task<List<string>> GetTiposAsync(int id)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 
                i.id_inquilino, 
                pr.id_propietario, 
                e.id_empleado
            FROM personas p

                LEFT JOIN inquilinos i 
                    ON p.id_persona = i.id_persona AND i.estado = 1

                LEFT JOIN propietarios pr 
                    ON p.id_persona = pr.id_persona AND pr.estado = 1

                LEFT JOIN empleados e 
                    ON p.id_persona = e.id_persona AND e.estado = 1
                    
            WHERE p.id_persona = @Id;
        ";

        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();

        var tipos = new List<string>();

        if (await reader.ReadAsync())
        {
            if (!reader.IsDBNull(reader.GetOrdinal("id_inquilino")))
                tipos.Add("inquilino");

            if (!reader.IsDBNull(reader.GetOrdinal("id_propietario")))
                tipos.Add("propietario");

            if (!reader.IsDBNull(reader.GetOrdinal("id_empleado")))
                tipos.Add("empleado");
        }

        return tipos;
    }

    public async Task<DatosPersonalesDTO> GetDatosPersonalesByEmailAsync(string email)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 
                p.apellido AS Apellido,
                p.nombre AS Nombre,
                p.telefono AS Telefono,
                p.email AS Email,
                u.avatar AS Avatar
            FROM usuarios u
                JOIN empleados e ON u.id_empleado = e.id_empleado
                JOIN personas p ON e.id_persona = p.id_persona
            WHERE p.email = @Email;
        ";

        command.Parameters.AddWithValue("@Email", email);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new DatosPersonalesDTO
            {
                Apellido = reader.GetString("Apellido"),
                Nombre = reader.GetString("Nombre"),
                Telefono = reader.GetString("Telefono"),
                Email = reader.GetString("Email"),
                Avatar = reader.GetString("Avatar")
            };
        }

        return null;
    }

    public async Task<DatosPersonalesDTO> UpdateDatosPersonalesAsync(DatosPersonalesDTO datos)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE usuarios 
            SET avatar = @Avatar
            WHERE id_empleado = (
                SELECT e.id_empleado
                FROM empleados e
                JOIN personas p ON e.id_persona = p.id_persona
                WHERE p.email = @Email
            );
            UPDATE personas 
            SET apellido = @Apellido, nombre = @Nombre, telefono = @Telefono, email = @Email
            WHERE email = @Email;
        ";

        command.Parameters.AddWithValue("@Apellido", datos.Apellido);
        command.Parameters.AddWithValue("@Nombre", datos.Nombre);
        command.Parameters.AddWithValue("@Telefono", datos.Telefono);
        command.Parameters.AddWithValue("@Email", datos.Email);
        command.Parameters.AddWithValue("@Avatar", datos.Avatar ?? "default-avatar.png");

        await command.ExecuteNonQueryAsync();
        return datos;
    }

    public async Task<string> GetPasswordByEmailAsync(string email)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT u.password
            FROM usuarios u
            JOIN empleados e ON u.id_empleado = e.id_empleado
            JOIN personas p ON e.id_persona = p.id_persona
            WHERE p.email = @Email;
        ";
        command.Parameters.AddWithValue("@Email", email);

        var result = await command.ExecuteScalarAsync();
        return result?.ToString();
        
    }
}