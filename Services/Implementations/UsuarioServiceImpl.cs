using System.Data;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Repositories.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation; //Para hashear contraseñas



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

    public Task<int> ActualizarAsync(int usuarioId, bool estado)
    {
        throw new NotImplementedException();
    }

    public Task<int> EliminarAsync(int usuarioId)
    {
        throw new NotImplementedException();
    }

    public Task<Usuario> NuevoAsync(Usuario usuario)
    {
        try
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: usuario.Password,
                salt: System.Text.Encoding.UTF8.GetBytes(_configuration["Salt"]),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            usuario.Password = hashed;

            return _usuarioRepository.AddAsync(usuario);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear el nuevo usuario", ex);
        }
    }

    public Task<Usuario> ObtenerIdAsync(int usuarioId)
    {
        throw new NotImplementedException();
    }

    public Task<(IEnumerable<Usuario> Usuarios, int Total)> ObtenerTodosAsync(int page, int pageSize, string? search = null)
    {
        throw new NotImplementedException();
    }
}