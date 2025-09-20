using inmobiliariaULP.Models;
namespace inmobiliariaULP.Repositories.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario> AddAsync(Usuario usuario);
    
}