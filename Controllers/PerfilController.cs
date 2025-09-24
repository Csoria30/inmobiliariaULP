using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Para el atributo [Authorize]
using System.Security.Claims;
using inmobiliariaULP.Models.ViewModels;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Services.Implementations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace inmobiliariaULP.Controllers;

[Authorize]
public class PerfilController : Controller
{
    private readonly ILogger<PerfilController> _logger;
    private readonly IPersonaService _personaService;
    private readonly IInquilinoService _inquilinoService;
    private readonly IPropietarioService _propietarioService;
    private readonly IUsuarioService _usuarioService;
    private readonly IWebHostEnvironment _environment;

    public PerfilController(
        ILogger<PerfilController> logger,
        IPersonaService personaService,
        IInquilinoService inquilinoService,
        IPropietarioService propietarioService,
        IUsuarioService usuarioService,
        IWebHostEnvironment environment
    )
    {
        _logger = logger;
        _personaService = personaService;
        _inquilinoService = inquilinoService;
        _propietarioService = propietarioService;
        _usuarioService = usuarioService;
        _environment = environment;
    }

    // Ejemplo de acción Index
    public IActionResult Index()
    {
        return View();
    }

    //* PerfilController/GET
    [HttpGet]
    public async Task<IActionResult> Perfil()
    {
        var email = User.FindFirst(ClaimTypes.Name)?.Value;

        var (data, estado) = await _personaService.ObtenerDatosPersonalesByEmailAsync(email);

        if (estado)
        {
            var model = new PerfilViewModel
            {
                DatosPersonalesDTO = data,
                CambiarPasswordDTO = new CambiarPasswordDTO()
            };
            return View("Index", model);
        }

        return RedirectToAction("Index", "Home");
    }

    //! POST: PerfilController/Perfil
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActualizarDatos(PerfilViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Vuelve a consultar los datos actuales para mostrar en la vista
            var email = User.FindFirst(ClaimTypes.Name)?.Value;
            var (data, _) = await _personaService.ObtenerDatosPersonalesByEmailAsync(email);
            model.DatosPersonalesDTO = data;
            return View("Index", model);
        }
        try
        {
            var idPersona = User.FindFirst("PersonaId")?.Value;

            // Si el usuario sube una nueva imagen
            if (model.DatosPersonalesDTO.AvatarFile != null && model.DatosPersonalesDTO.AvatarFile.Length > 0)
            {
                string wwwPath = _environment.WebRootPath;
                string path = Path.Combine(wwwPath, "images");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                // Nombre único usando el id de persona
                string extension = Path.GetExtension(model.DatosPersonalesDTO.AvatarFile.FileName);
                string avatarFileName = $"persona_{idPersona}_perfil{extension}";
                string filePath = Path.Combine(path, avatarFileName);

                //Nombre Aleatorio - para subir imagenes sin sobreescribir
                //avatarFileName = Guid.NewGuid() + Path.GetExtension(model.DatosPersonalesDTO.AvatarFile.FileName);

                // Elimina la imagen anterior si existe
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);


                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.DatosPersonalesDTO.AvatarFile.CopyToAsync(stream);
                }
                
                //Actualiza el nombre del avatar en el modelo
                model.DatosPersonalesDTO.Avatar = avatarFileName;
            }


            // Actualiza los datos personales usando el servicio
            var (data, estado) = await _personaService.ActualizarDatosPersonalesAsync(model.DatosPersonalesDTO);

            // Recarga los datos personales para obtener el avatar actualizado desde la base
            var email = User.FindFirst(ClaimTypes.Name)?.Value;
            var (datosActualizados, _) = await _personaService.ObtenerDatosPersonalesByEmailAsync(email);

            //Actualizar el claim de Avatar en la cookie de autenticación
            var identity = (ClaimsIdentity)User.Identity;
            var avatarClaim = identity.FindFirst("Avatar");
            if (avatarClaim != null)
                identity.RemoveClaim(avatarClaim);
            
            identity.AddClaim(new Claim("Avatar", datosActualizados.Avatar ?? "defaultAvatar.png"));

            // Refresca el principal
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );

            if (estado)
            {
                TempData["Notificacion"] = "Datos personales actualizados correctamente.";
                TempData["NotificacionTipo"] = "success";
                return RedirectToAction("Perfil");
            }
            else
            {
                TempData["Notificacion"] = "No se pudieron actualizar los datos personales.";
                TempData["NotificacionTipo"] = "danger";
                model.DatosPersonalesDTO = datosActualizados;
                return View("Index", model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar datos personales.");
            TempData["Notificacion"] = "No se pudieron actualizar los datos personales.";
            TempData["NotificacionTipo"] = "danger";
            var email = User.FindFirst(ClaimTypes.Name)?.Value;
            var (datosActuales, _) = await _personaService.ObtenerDatosPersonalesByEmailAsync(email);
            model.DatosPersonalesDTO = datosActuales;
            return View("Index", model);
        }

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarPassword(PerfilViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Si hay errores de validación, vuelve a mostrar la vista con el modelo
            return View("Index", model);
        }

        try
        {

            var email = User.FindFirst(ClaimTypes.Name)?.Value;
            var resultado = await _personaService.CambiarPasswordAsync(model.CambiarPasswordDTO, email);

            if (resultado)
            {
                TempData["Notificacion"] = "Contraseña actualizada correctamente.";
                TempData["NotificacionTipo"] = "success";
                return RedirectToAction("Perfil");
            }
            else
            {
                TempData["Notificacion"] = "No se pudo actualizar la contraseña.";
                TempData["NotificacionTipo"] = "danger";
                var (datosPersonales, _) = await _personaService.ObtenerDatosPersonalesByEmailAsync(email);
                model.DatosPersonalesDTO = datosPersonales;
                return View("Index", model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar la contraseña.");
            TempData["Notificacion"] = "No se pudo actualizar la contraseña.";
            TempData["NotificacionTipo"] = "danger";
            return View("Index", model);
        }
    }


    // Agrega aquí tus acciones adicionales...
}