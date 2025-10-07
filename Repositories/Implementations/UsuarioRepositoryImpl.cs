using System.Data;
using inmobiliariaULP.Models;
using MySql.Data.MySqlClient;
using inmobiliariaULP.Repositories.Interfaces;
using inmobiliariaULP.Models.ViewModels;


namespace inmobiliariaULP.Repositories.Implementations;

public class UsuarioRepositoryImpl(IConfiguration configuration) : BaseRepository(configuration), IUsuarioRepository
{
    public async Task<int> AddAsync(Usuario usuario)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "sp_InsertUsuario";

        command.Parameters.AddWithValue("@p_id_empleado", usuario.EmpleadoId);
        command.Parameters.AddWithValue("@p_password", usuario.Password);
        command.Parameters.AddWithValue("@p_rol", usuario.Rol);
        command.Parameters.AddWithValue("@p_avatar", usuario.Avatar ?? "defaultAvatar.png");

        var outputParam = new MySqlParameter("@p_usuario_id", MySqlDbType.Int32)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(outputParam);

        await command.ExecuteNonQueryAsync();
        
        return Convert.ToInt32(outputParam.Value);
    }

    public async Task<UsuarioLoginDTO?> GetByIdAsync(int usuarioId)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "sp_GetUsuarioById";

        command.Parameters.AddWithValue("@p_usuario_id", usuarioId);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new UsuarioLoginDTO
            {
                PersonaId = reader.GetInt32("PersonaId"),
                UsuarioId = reader.GetInt32("UsuarioId"),
                EmpleadoId = reader.GetInt32("EmpleadoId"),
                Password = reader.GetString("Password"),
                Rol = reader.GetString("Rol"),
                Avatar = reader.GetString("Avatar"),
                Estado = reader.GetBoolean("Estado"),
                Email = reader.GetString("Email"),
                Apellido = reader.GetString("Apellido"),
                Nombre = reader.GetString("Nombre"),
                Telefono = reader.GetString("Telefono")
            };
        }

        return null;
    }

    public async Task<UsuarioLoginDTO> GetByEmailAsync(string email)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "sp_GetUsuarioByEmail";

        command.Parameters.AddWithValue("@p_email", email);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new UsuarioLoginDTO
            {
                PersonaId = reader.GetInt32("PersonaId"),
                UsuarioId = reader.GetInt32("UsuarioId"),
                EmpleadoId = reader.GetInt32("EmpleadoId"),
                Password = reader.GetString("Password"),
                Rol = reader.GetString("Rol"),
                Avatar = reader.GetString("Avatar"),
                Estado = reader.GetBoolean("Estado"),
                Email = reader.GetString("Email"),
                Apellido = reader.GetString("Apellido"),
                Nombre = reader.GetString("Nombre"),
                Telefono = reader.GetString("Telefono")
            };
        }

        return null;
    }

    public async Task<int> UpdateAsync(Usuario usuario)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "sp_UpdateUsuario";

        command.Parameters.AddWithValue("@p_id_empleado", usuario.EmpleadoId);
        command.Parameters.AddWithValue("@p_password", usuario.Password);
        command.Parameters.AddWithValue("@p_rol", usuario.Rol);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<bool> UpdatePasswordAsync(string password, string email)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "sp_UpdatePasswordByEmail";

        command.Parameters.AddWithValue("@p_password", password);
        command.Parameters.AddWithValue("@p_email", email);

        var result = await command.ExecuteScalarAsync();
        var rowsAffected = Convert.ToInt32(result);
        
        return rowsAffected > 0;
    }
}