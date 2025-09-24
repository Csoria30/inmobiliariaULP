using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Models.ViewModels;
using inmobiliariaULP.Services.Implementations;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using Org.BouncyCastle.Asn1.Crmf; //  ValidationException
using Microsoft.AspNetCore.Authorization; // Para el atributo [Authorize]


namespace inmobiliariaULP.Controllers;

[Authorize]
public class PersonaController : Controller
{
    private readonly ILogger<PersonaController> _logger;
    private readonly IPersonaService _personaService;
    private readonly IInquilinoService _inquilinoService;
    private readonly IPropietarioService _propietarioService;
    private readonly IUsuarioService _usuarioService;
    private readonly IEmpleadoService _empleadoService;
     private readonly IWebHostEnvironment _environment;

    public PersonaController(
        ILogger<PersonaController> logger,
        IPersonaService personaService,
        IInquilinoService inquilinoService,
        IPropietarioService propietarioService,
        IUsuarioService usuarioService,
        IEmpleadoService empleadoService,
        IWebHostEnvironment environment
    )
    {
        _logger = logger;
        _personaService = personaService;
        _inquilinoService = inquilinoService;
        _propietarioService = propietarioService;
        _usuarioService = usuarioService;
        _empleadoService = empleadoService;
        _environment = environment;
    }

    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> Index()
    {
        try
        {
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener personas");
            TempData["Error"] = "Error al cargar las personas: " + ex.Message;
            return View(new List<Persona>()); // Vista vacía con error
        }
    }

    //* GET: PersonasController/Create
    [HttpGet]
    [Authorize(Roles = "administrador")]
    public IActionResult Create()
    {
        try
        {
            var model = new PersonaUsuarioDTO();

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener personas");
            TempData["Error"] = "Error al cargar las personas: " + ex.Message;
            return View("Error");
        }
    }


    //! POST: PersonasController/Create
    [HttpPost]
    [Authorize(Roles = "administrador")]
    [ValidateAntiForgeryToken] // Buena práctica para prevenir ataques CSRF
    public async Task<IActionResult> Create(PersonaUsuarioDTO model)
    {
        try
        {
             if (!model.TipoPersona.Contains("empleado"))
            {
                // Si NO es empleado, limpia los campos - errores de validación
                ModelState.Remove("Usuario.Password");
                ModelState.Remove("Usuario.Rol");
                ModelState.Remove("Usuario.EmpleadoId");
            }

            if (ModelState.IsValid)
            {
                // Instancia y crea la persona
                var persona = new Persona
                {
                    Dni = model.Dni,
                    Apellido = model.Apellido,
                    Nombre = model.Nombre,
                    Telefono = model.Telefono,
                    Email = model.Email,
                    Estado = model.Estado,
                    TipoPersona = model.TipoPersona
                };

                // Crear persona
                var (exitoPersona, mensaje, tipo, personaId) = await _personaService.CrearAsync(persona);

                //Validacion para continuar si se creo correctamente la persona
                if (!exitoPersona)
                {
                    TempData["Notificacion"] = mensaje;
                    TempData["NotificacionTipo"] = tipo;
                    return View("Create", model);
                }

                // Si es empleado, crearlo
                bool exitoUsuario = true;
                if (model.TipoPersona.Contains("empleado"))
                {
                    //Recuperar el empleado generado - ID Obtenido de la tupla al crear la persona
                    var empleado = await _empleadoService.ObtenerIdAsync(personaId);
                    if (empleado != null)
                    {
                        //Recperando el Id de empleado generado
                         var usuario = new Usuario
                        {
                            EmpleadoId = empleado.EmpleadoId,
                            Rol = model.Rol,
                            Password = model.Password
                            // Otros campos si es necesario
                        };
                        await _usuarioService.NuevoAsync(usuario);
                    }
                    else
                    {
                        exitoUsuario = false;
                        TempData["Notificacion"] = "Error al crear el usuario asociado al empleado.";
                        TempData["NotificacionTipo"] = "danger";
                    }
                }

                //Notificaciones
                if (exitoUsuario)
                {
                    TempData["Notificacion"] = mensaje;
                    TempData["NotificacionTipo"] = tipo;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Notificacion"] = mensaje;
                    TempData["NotificacionTipo"] = "danger";
                    return View("Create", model); // Volver al formulario con datos
                }


            }

            // Si ModelState no es válido, retornar a la vista con los datos ingresados
            return View("Create", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al Crear personas");
            TempData["Error"] = "Error al Crear la persona: " + ex.Message;
            return View("Error");
        }
    }


    //* GET: PersonasController/Edit
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
           var (model, mensaje, tipo) = await _personaService.ObtenerDtoIdAsync(id);

            if (model == null)
            {
                TempData["Notificacion"] = mensaje ?? "La persona no existe.";
                TempData["NotificacionTipo"] = tipo ?? "danger";
                return RedirectToAction(nameof(Index));
            }

            if (model.Estado == false)
            {
                TempData["Notificacion"] = "No se puede editar una persona deshabilitada.";
                TempData["NotificacionTipo"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            return View("Create", model);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener personas");
            TempData["Error"] = "Error al cargar las personas: " + ex.Message;
            return View("Error");
        }
    }


    //! POST: PersonasController/Edit    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(PersonaUsuarioDTO model)
    {
        try
        {
            if (!model.TipoPersona.Contains("empleado"))
            {
                // Si NO es empleado, limpia los campos de usuario para evitar errores de validación
                ModelState.Remove("Password");
                ModelState.Remove("Rol");
                ModelState.Remove("EmpleadoId");
            }

            if (ModelState.IsValid)
            {
                // Actualizar persona
                var persona = new Persona
                {
                    PersonaId = model.PersonaId,
                    Dni = model.Dni,
                    Apellido = model.Apellido,
                    Nombre = model.Nombre,
                    Telefono = model.Telefono,
                    Email = model.Email,
                    Estado = model.Estado,
                    TipoPersona = model.TipoPersona
                };

                var (exito, mensaje, tipo) = await _personaService.EditarAsync(persona);
                TempData["Notificacion"] = mensaje;
                TempData["NotificacionTipo"] = tipo;

                bool exitoUsuario = true;

                if (exito && model.TipoPersona.Contains("empleado"))
                {
                    string avatarFileName = "defaultAvatar.png";

                    // Si el modelo tiene un archivo de imagen subido
                    if (model.AvatarFile != null && model.AvatarFile.Length > 0)
                    {
                        string wwwPath = _environment.WebRootPath; // Obtiene la ruta raíz de la carpeta wwwroot del proyecto.
                        string path = Path.Combine(wwwPath, "images"); // Junta la ruta raíz con la carpeta img
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path); // Si no existe la carpeta, la crea

                        avatarFileName = Guid.NewGuid() + Path.GetExtension(model.AvatarFile.FileName); // Genera un nombre único para el archivo usando un GUID y conserva la extensión original
                        string filePath = Path.Combine(path, avatarFileName); // Ruta completa del archivo

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.AvatarFile.CopyToAsync(stream);
                        }

                    }

                    // Buscar el empleado asociado
                    var empleado = await _empleadoService.ObtenerIdAsync(model.PersonaId);
                    if (empleado != null)
                    {
                        var usuario = new Usuario
                        {
                            EmpleadoId = empleado.EmpleadoId,
                            Rol = model.Rol,
                            Password = model.Password,
                            Avatar = avatarFileName
                        };
                        exitoUsuario = await _usuarioService.ActualizarAsync(usuario);
                    }
                    else
                    {
                        exitoUsuario = false;
                        TempData["Notificacion"] = "No se encontró el empleado asociado para actualizar el usuario.";
                        TempData["NotificacionTipo"] = "danger";
                    }
                }

                if (exito && exitoUsuario)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Si hubo error, vuelve a mostrar el formulario con los datos ingresados
                    return View("Create", model);
                }
            }

            TempData["Notificacion"] = "Error al editar la persona.";
            TempData["NotificacionTipo"] = "danger";
            return View("Create", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar persona {PersonaId}", model.PersonaId);
            TempData["Error"] = "Error al guardar los cambios.";
            return View("Create", model);
        }

    }


    //* GET: PersonasController/Details
    public async Task<IActionResult> Details(int id, string? returnUrl = null)
    {
        try
        {
            var esEmpleado = await _personaService.EsEmpleado(id);
            ViewBag.soloLectura = true; // Flag para la vista
            ViewBag.ReturnUrl = returnUrl ?? Url.Action("Index", "Persona"); //Identificar de donde se llamo
            PersonaUsuarioDTO? model = null;

            if (esEmpleado)
            {
                var (persona, mensaje, tipo) = await _personaService.ObtenerDtoIdAsync(id);
                if (persona == null)
                {
                    TempData["Error"] = "La persona no existe.";
                    return RedirectToAction(nameof(Index));
                }

                model = persona;
            }
            else
            {
                var (persona, mensaje, tipo) = await _personaService.ObtenerDetalleAsync(id);
                if (persona == null)
                {
                    TempData["Error"] = "La persona no existe.";
                    return RedirectToAction(nameof(Index));
                }

                // Armar el DTO plano
                model = new PersonaUsuarioDTO
                {
                    PersonaId = persona.PersonaId,
                    Dni = persona.Dni,
                    Apellido = persona.Apellido,
                    Nombre = persona.Nombre,
                    Telefono = persona.Telefono,
                    Email = persona.Email,
                    Estado = persona.Estado,
                    TipoPersona = persona.TipoPersona,
                    // Usuario: campos vacíos porque no es empleado
                    Rol = "",
                    Password = ""
                };

                return View("Create", model);
            }

            return View("Create", model);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener personas");
            TempData["Error"] = "Error al cargar las personas: " + ex.Message;
            return View("Error");
        }
    }

    //! POST: PersonasController/Delete
    [HttpPost]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var (exito, mensaje, tipo) = await _personaService.CambiarEstadoAsync(id);
            TempData["Notificacion"] = mensaje;
            TempData["NotificacionTipo"] = tipo;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar persona {PersonaId}", id);
            TempData["Notificacion"] = "Error al eliminar la persona.";
            TempData["NotificacionTipo"] = "danger";
            return RedirectToAction(nameof(Index));
        }
    }


    [HttpPost]
    public async Task<IActionResult> ObtenerDataTable()
    {
        try
        {
            // Recibe parámetros de DataTables
            var draw = Request.Form["draw"].FirstOrDefault();
            // Es un número que envía DataTables en cada petición AJAX para identificar la respuesta y sincronizar el frontend.

            var start = Convert.ToInt32(Request.Form["start"].FirstOrDefault());
            //  Es el índice del primer registro que DataTables quiere mostrar (para paginación).

            var length = Convert.ToInt32(Request.Form["length"].FirstOrDefault());
            //  Es la cantidad de registros que DataTables quiere mostrar por página (tamaño de página).

            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            //  Es el valor de búsqueda que el usuario ha ingresado en el cuadro de búsqueda de DataTables.

            int page = (start / length) + 1;
            //  Es el número de página actual que DataTables está solicitando.

            int pageSize = length;
            //  Es la cantidad de registros por página que DataTables está solicitando.

            // Llama a tu servicio para obtener datos paginados y filtrados
            var (personas, total) = await _personaService.ObtenerTodosAsync(page, pageSize, searchValue);

            // Arma los datos para DataTables
            var data = personas.Select(persona => new
            {
                dni = persona.Dni,
                apellido = persona.Apellido,
                nombre = persona.Nombre,
                telefono = persona.Telefono,
                perfil = string.Join(" ", persona.TipoPersona.Select(tipo =>
                    tipo == "inquilino"
                        ? "<span class='badge bg-primary me-1'>Inquilino</span>"

                    : tipo == "propietario"
                            ? "<span class='badge bg-warning text-dark me-1'>Propietario</span>"

                    : tipo == "empleado"
                            ? "<span class='badge bg-success me-1'>Empleado</span>"

                    : $"<span class='badge bg-secondary me-1'>{tipo}</span>"
                )),

                estado = persona.Estado
                    ? "<span class='badge bg-success'>Habilitado</span>"
                    : "<span class='badge bg-danger'>Deshabilitado</span>",

                acciones = $@"
                <div class='btn-group' role='group'>
                    
                    <a 
                        href='/Persona/Details/{persona.PersonaId}' 
                        class='btn btn-sm btn-outline-info' 
                        data-bs-toggle='tooltip' 
                        data-bs-placement='top' 
                        title='Más información'>
                        <i class='bi bi-eye'></i>
                    </a>
                
                    <a 
                        href='/Persona/Edit/{persona.PersonaId}' 
                        class='btn btn-sm btn-outline-warning' 
                        data-bs-toggle='tooltip' 
                        data-bs-placement='top' 
                        title='Editar Persona'>
                        <i class='bi bi-pencil'></i>
                    </a>
                
                    <a 
                        href='/Persona/Delete/{persona.PersonaId}' 
                        class='btn btn-sm {(persona.Estado ? "btn-outline-danger" : "btn-outline-success")}'
                        data-bs-toggle='modal'
                        data-bs-target='#modalEliminarPersona'
                        data-personaid='{persona.PersonaId}'
                        data-personanombre='{persona.Apellido}, {persona.Nombre}'
                        data-bs-placement='top'
                        title='{(persona.Estado ? "Deshabilitar" : "Habilitar")}'>
                        <i class='bi {(persona.Estado ? "bi-trash3" : "bi-arrow-repeat")}'></i>
                    </a>

                </div>"
            });

            return Json(new
            {
                draw = draw,
                recordsTotal = total,
                recordsFiltered = total, // Si implementas búsqueda, cambia este valor
                data = data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener datos para DataTable");
            return StatusCode(500, "Error al procesar la solicitud.");
        }
    }
}