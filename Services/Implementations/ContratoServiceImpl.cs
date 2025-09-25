using System.Data;
using inmobiliariaULP.Models;
using inmobiliariaULP.Models.ViewModels;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Repositories.Interfaces;
using Google.Protobuf;

namespace inmobiliariaULP.Services.Implementations;

public class ContratoServiceImpl : IContratoService
{
    private readonly IContratoRepository _contratoRepository;

    public ContratoServiceImpl(IContratoRepository contratoRepository)
    {
        _contratoRepository = contratoRepository;
    }

    public async Task<(IEnumerable<ContratoListadoDTO> Contratos, int Total)> ObtenerTodosAsync(int page, int pageSize, string? search = null)
    {
        return await _contratoRepository.GetAllAsync(page, pageSize, search);
    }
}