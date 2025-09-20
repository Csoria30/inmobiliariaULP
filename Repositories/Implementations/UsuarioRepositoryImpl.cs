using System.Data;
using inmobiliariaULP.Models;
using MySql.Data.MySqlClient;
using inmobiliariaULP.Repositories.Interfaces;


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
}