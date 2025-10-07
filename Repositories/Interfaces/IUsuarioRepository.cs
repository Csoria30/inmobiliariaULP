using inmobiliariaULP.Models;
namespace inmobiliariaULP.Repositories.Interfaces;
using inmobiliariaULP.Models.ViewModels;
public interface IUsuarioRepository
{
    Task<int> AddAsync(Usuario usuario);
    Task<UsuarioLoginDTO?> GetByIdAsync(int usuarioId);
    Task<UsuarioLoginDTO> GetByEmailAsync(string email);
    Task<int> UpdateAsync(Usuario usuario);
    Task<bool> UpdatePasswordAsync(string password, string email);

}