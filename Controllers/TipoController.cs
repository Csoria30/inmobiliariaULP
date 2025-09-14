using Microsoft.AspNetCore.Mvc;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Services.Implementations;

namespace inmobiliariaULP.Controllers;

public class TipoController : Controller
{
    private readonly ILogger<TipoController> _logger;
    private readonly ITipoService _tipoService;

    public TipoController(ILogger<TipoController> logger, ITipoService tipoService)
    {
        _logger = logger;
        _tipoService = tipoService;
    }

    
}