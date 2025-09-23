using System.Data;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Repositories.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation; //Para hashear contraseñas
using inmobiliariaULP.Helpers;
using inmobiliariaULP.Models.ViewModels;


namespace inmobiliariaULP.Services.Implementations;

public class UsuarioServiceImpl : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IConfiguration _configuration;
    public UsuarioServiceImpl(IUsuarioRepository usuarioRepository, IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _configuration = configuration;
    }

    public async Task<bool> ActualizarAsync(Usuario usuario)
    {
        // Si se va a actualizar la contraseña, la hasheamos
        if (!string.IsNullOrEmpty(usuario.Password))
        {
            usuario.Password = PasswordHelper.HashPassword(usuario.Password, _configuration["Salt"]);
        }

        // Llama al repositorio para actualizar el usuario
        var filasAfectadas = await _usuarioRepository.UpdateAsync(usuario);

        return filasAfectadas > 0;
    }

    public async Task<int> EliminarAsync(int usuarioId)
    {
        throw new NotImplementedException();
    }

    public async Task<(bool Exito, string Mensaje, UsuarioLoginDTO Usuario)> ObtenerPorEmailAsync(string email, string passsword)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(passsword))
            return (false, "Debe ingresar correo y contraseña.", null);

        var emailRecibido = email;
        var passwordRecibido = PasswordHelper.HashPassword(passsword, _configuration["Salt"]);

        var usuario = await _usuarioRepository.GetByEmailAsync(emailRecibido);


        if (usuario == null || usuario.Password != passwordRecibido)
            return (false, "Usuario o Contraseña incorrecta", null);
        
        
        return (true, "Usuario encontrado", usuario);
    }

    public async Task<Usuario> NuevoAsync(Usuario usuario)
    {
        try
        {
            string hashed = PasswordHelper.HashPassword(usuario.Password, _configuration["Salt"]);
            usuario.Password = hashed;

            return await _usuarioRepository.AddAsync(usuario);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear el nuevo usuario", ex);
        }
    }

    public async Task<Usuario> ObtenerIdAsync(int usuarioId)
    {
        throw new NotImplementedException();
    }

    public async Task<(IEnumerable<Usuario> Usuarios, int Total)> ObtenerTodosAsync(int page, int pageSize, string? search = null)
    {
        throw new NotImplementedException();
    }

    
}