using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Para el atributo [AllowAnonymous] - Login
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using inmobiliariaULP.Models.ViewModels;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Services.Implementations;

namespace inmobiliariaULP.Controllers;

[AllowAnonymous]
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
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol),
                new Claim("UsuarioId", usuario.UsuarioId.ToString()),
                new Claim("PersonaId", usuario.PersonaId.ToString()),
                new Claim("FullName", usuario.Nombre + " " + usuario.Apellido),
                new Claim("Role", usuario.Rol),
                new Claim("Avatar", usuario.Avatar ?? "default-avatar.png") 
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }
        else
        {
            ViewBag.Error = mensaje;
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Auth");
    }

    // Agrega aquí tus acciones adicionales...
}