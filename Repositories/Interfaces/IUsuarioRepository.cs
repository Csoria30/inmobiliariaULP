using inmobiliariaULP.Models;
namespace inmobiliariaULP.Repositories.Interfaces;
using inmobiliariaULP.Models.ViewModels;
public interface IUsuarioRepository
{
    Task<Usuario> AddAsync(Usuario usuario);
    Task<UsuarioLoginDTO> GetByEmailAsync(string email);
    Task<int> UpdateAsync(Usuario usuario);
    Task<bool> UpdatePasswordAsync(string password, string email);

}