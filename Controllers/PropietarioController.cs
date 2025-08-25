using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Services.Implementations;
using Microsoft.Extensions.Configuration;

namespace inmobiliariaULP.Controllers;

public class PropietarioController : Controller
{
    private readonly ILogger<PropietarioController> _logger;
    private readonly IPropietarioService _propietarioService;

    public PropietarioController(ILogger<PropietarioController> logger)
    {
        _logger = logger;
        _propietarioService = new PropietarioServiceImpl();
    }

     public async Task<IActionResult> Index()
    {
        try
        {
            var propietarios = await _propietarioService.ObtenerTodosAsync();
            return View(propietarios);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener propietarios");
            TempData["Error"] = "Error al cargar las propietarios: " + ex.Message;
            return View(new List<Propietario>()); // Vista vacía con error
        }
    }

}
