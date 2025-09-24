using System.Data;
using inmobiliariaULP.Models;
using MySql.Data.MySqlClient;
using inmobiliariaULP.Repositories.Interfaces;

namespace inmobiliariaULP.Repositories.Implementations;

public class ContratoRepositoryImpl(IConfiguration configuration) : BaseRepository(configuration), IContratosRepository
{
    public Task<int> AddAsync(Contrato contrato)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteAsync(int contratoId)
    {
        throw new NotImplementedException();
    }

    public Task<(IEnumerable<Contrato> Contratos, int Total)> GetAllAsync(int page, int pageSize, string? search = null)
    {
        throw new NotImplementedException();
    }

    public Task<Contrato> GetByIdAsync(int contratoId)
    {
        throw new NotImplementedException();
    }

    public Task<int> UpdateAsync(int contratoId)
    {
        throw new NotImplementedException();
    }
}