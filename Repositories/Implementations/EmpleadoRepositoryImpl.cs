using System.Data;
using inmobiliariaULP.Models;
using MySql.Data.MySqlClient;
using inmobiliariaULP.Repositories.Interfaces;

namespace inmobiliariaULP.Repositories.Implementations;

public class EmpleadoRepositoryImpl(IConfiguration configuration) : BaseRepository(configuration), IEmpleadoRepository
{
    public async Task<int> AddAsync(int personaId)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO empleados (id_persona) VALUES (@PersonaId);
            SELECT LAST_INSERT_ID();
        ";

        command.Parameters.AddWithValue("@PersonaId", personaId);
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<int> DeleteAsync(int empleadoId)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            DELETE FROM empleados
            WHERE id_empleado = @empleadoId
        ";

        command.Parameters.AddWithValue("@empleadoId", empleadoId);
        return await command.ExecuteNonQueryAsync();
    }

    public async Task<(IEnumerable<Empleado> Empleados, int Total)> GetAllAsync(int page, int pageSize, string? search = null)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        // 1. Armar el WHERE si hay búsqueda
        string where = "";
        if (!string.IsNullOrEmpty(search))
        {
            where = @"
                WHERE 
                    p.dni LIKE @search OR
                    p.apellido LIKE @search OR
                    p.nombre LIKE @search
            ";
        }

        // 2. Obtener el total de registros (filtrado si hay búsqueda)
        int total;
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $@"
                SELECT COUNT(*) 
                FROM personas p
                INNER JOIN empleados e ON p.id_persona = e.id_persona
                {where}
            ";

            if (!string.IsNullOrEmpty(search))
                command.Parameters.AddWithValue("@search", $"%{search}%");

            total = Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        // 3. Obtener los registros de la página (filtrado si hay búsqueda)
        var empleados = new List<Empleado>();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $@"
                Select 
                    e.id_persona AS PersonaId, 
                    e.id_empleado AS EmpleadoId, 
                    e.estado AS Estado, 
                    p.dni AS Dni, 
                    p.nombre AS Nombre, 
                    p.apellido AS Apellido, 
                    p.telefono AS Telefono, 
                    p.email AS Email

                    FROM empleados e
                        Join personas p 
                        On p.id_persona = e.id_persona
                
                    {where}
                    LIMIT @Offset, @PageSize
            ";

            if (!string.IsNullOrEmpty(search))
                command.Parameters.AddWithValue("@search", $"%{search}%");

            command.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
            command.Parameters.AddWithValue("@pageSize", pageSize);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                empleados.Add(new Empleado
                {
                    EmpleadoId = reader.GetInt32(nameof(Empleado.EmpleadoId)),
                    PersonaId = reader.GetInt32(nameof(Empleado.PersonaId)),
                    Dni = reader.GetString(nameof(Empleado.Dni)),
                    Apellido = reader.GetString(nameof(Empleado.Apellido)),
                    Nombre = reader.GetString(nameof(Empleado.Nombre)),
                    Telefono = reader.GetString(nameof(Empleado.Telefono)),
                    Email = reader.GetString(nameof(Empleado.Email)),
                    Estado = reader.GetBoolean(nameof(Empleado.Estado)),
                });
            }
        }

        return (empleados, total);
    }

    public async Task<Empleado> GetByIdAsync(int empleadoId)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 
                id_empleado,
                id_persona, 
                estado 
            
            FROM empleados
            WHERE id_persona = @EmpleadoId;
        ";

        command.Parameters.AddWithValue("@EmpleadoId", empleadoId);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Empleado
            {
                EmpleadoId = reader.GetInt32("id_empleado"),
                PersonaId = reader.GetInt32("id_persona"),
                Estado = reader.GetBoolean("estado")
            };
        }

        return null;
    }

    public async Task<int> UpdateAsync(int empleadoId, bool estado)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE empleados 
            SET estado = @Estado 
            WHERE id_empleado = @EmpleadoId
        ";

        // Asignamos los parámetros
        command.Parameters.AddWithValue("@Estado", estado ? 1 : 0);
        command.Parameters.AddWithValue("@EmpleadoId", empleadoId);

        return await command.ExecuteNonQueryAsync();
    }

}   