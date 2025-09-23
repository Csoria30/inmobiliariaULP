using System.Data;
using inmobiliariaULP.Models;
using MySql.Data.MySqlClient;
using inmobiliariaULP.Repositories.Interfaces;
using inmobiliariaULP.Models.ViewModels;


namespace inmobiliariaULP.Repositories.Implementations;

public class UsuarioRepositoryImpl(IConfiguration configuration) : BaseRepository(configuration), IUsuarioRepository
{
    public async Task<Usuario> AddAsync(Usuario usuario)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO usuarios (id_empleado, password, rol, avatar) 
            VALUES (@EmpleadoId, @Password, @Rol, @Avatar);
            SELECT LAST_INSERT_ID();
        ";

        command.Parameters.AddWithValue("@EmpleadoId", usuario.EmpleadoId);
        command.Parameters.AddWithValue("@Password", usuario.Password);
        command.Parameters.AddWithValue("@Rol", usuario.Rol);
        command.Parameters.AddWithValue("@Avatar", "defaultAvatar.png"); // Asigna un avatar por defecto
        var result = await command.ExecuteScalarAsync();
        usuario.UsuarioId = Convert.ToInt32(result); // Asigna el ID generado al objeto

        return usuario;
    }

    public async Task<UsuarioLoginDTO> GetByEmailAsync(string email)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            Select 
                u.id_usuario AS UsuarioId,
                u.id_empleado AS EmpleadoId,
                u.password AS Password,
                u.rol AS Rol,
                u.avatar AS Avatar,
                e.estado AS Estado,
                p.id_persona AS PersonaId,
                p.email AS Email,
                p.apellido AS Apellido,
                p.nombre AS Nombre,
                p.telefono AS Telefono
                
            From usuarios u
                JOIN empleados e 
                    ON u.id_empleado = e.id_empleado
                JOIN personas p
                    ON e.id_persona = p.id_persona
                
            WHERE p.email = @Email;
        ";

        command.Parameters.AddWithValue("@Email", email);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new UsuarioLoginDTO
            {
                UsuarioId = reader.GetInt32("UsuarioId"),
                EmpleadoId = reader.GetInt32("EmpleadoId"),
                Password = reader.GetString("Password"),
                Rol = reader.GetString("Rol"),
                Avatar = reader.GetString("Avatar"),
                Estado = reader.GetBoolean("Estado"),
                Email = reader.GetString("Email"),
                Apellido = reader.GetString("Apellido"),
                Nombre = reader.GetString("Nombre"),
                Telefono = reader.GetString("Telefono"),
            };
        }

        return null;
    }

    public async Task<int> UpdateAsync(Usuario usuario)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE usuarios
            SET password = @Password,
                rol = @Rol
            WHERE id_empleado = @EmpleadoId;
        ";

        command.Parameters.AddWithValue("@Password", usuario.Password);
        command.Parameters.AddWithValue("@Rol", usuario.Rol);
        command.Parameters.AddWithValue("@EmpleadoId", usuario.EmpleadoId);

        return await command.ExecuteNonQueryAsync();
    }

    public async Task<bool> UpdatePasswordAsync(string password, string email)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE usuarios u
            JOIN empleados e ON u.id_empleado = e.id_empleado
            JOIN personas p ON e.id_persona = p.id_persona
            SET u.password = @NewPassword
            WHERE p.email = @Email;
        ";

        command.Parameters.AddWithValue("@NewPassword", password);
        command.Parameters.AddWithValue("@Email", email);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}