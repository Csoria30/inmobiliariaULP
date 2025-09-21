using inmobiliariaULP.Models;
using inmobiliariaULP.Models.ViewModels;

namespace inmobiliariaULP.Services.Interfaces;
public interface IUsuarioService
{
    Task<Usuario> NuevoAsync(Usuario usuario);
    Task<(IEnumerable<Usuario> Usuarios, int Total)> ObtenerTodosAsync(int page, int pageSize, string? search = null);
    Task<Usuario> ObtenerIdAsync(int usuarioId);
    Task<int> EliminarAsync(int usuarioId);
    Task<int> ActualizarAsync(int usuarioId, Boolean estado);
    Task<(bool Exito, string Mensaje, UsuarioLoginDTO Usuario)> ObtenerPorEmailAsync(string email, string passsword);
}