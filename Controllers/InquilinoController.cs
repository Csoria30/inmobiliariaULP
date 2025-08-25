using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Services.Implementations;
using Microsoft.Extensions.Configuration;

namespace inmobiliariaULP.Controllers;

public class InquilinoController : Controller
{
    private readonly ILogger<InquilinoController> _logger;
    private readonly IInquilinoService _inquilinoService;

    public InquilinoController(ILogger<InquilinoController> logger)
    {
        _logger = logger;
        _inquilinoService = new InquilinoServiceImpl();
    }

     public async Task<IActionResult> Index()
    {
        try
        {
            var inquilinos = await _inquilinoService.ObtenerTodosAsync();
            return View(inquilinos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener inquilinos");
            TempData["Error"] = "Error al cargar las inquilinos: " + ex.Message;
            return View(new List<Inquilino>()); // Vista vacía con error
        }
    }

}
