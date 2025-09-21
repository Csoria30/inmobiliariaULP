using Microsoft.AspNetCore.Mvc;
using inmobiliariaULP.Models.ViewModels;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Services.Implementations;
using System.Threading.Tasks;

namespace inmobiliariaULP.Controllers;

public class AuthController : Controller
{
    private readonly ILogger<AuthController> _logger;
    private readonly IUsuarioService _usuarioService;
    private readonly IPersonaService _personaService;
    

    public AuthController(
        ILogger<AuthController> logger,
        IUsuarioService usuarioService,
        IPersonaService personaService
    )
    {
        _logger = logger;
        _usuarioService = usuarioService;
        _personaService = personaService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(UsuarioLoginDTO model)
    {

        var (estado, mensaje, usuario) = await _usuarioService.ObtenerPorEmailAsync(model.Email, model.Password);

        if (estado)
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            return View();   
        }
    }

    // Agrega aquí tus acciones adicionales...
}