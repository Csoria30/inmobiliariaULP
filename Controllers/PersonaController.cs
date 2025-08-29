using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Services.Implementations;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations; //  ValidationException

namespace inmobiliariaULP.Controllers;

public class PersonaController : Controller
{
    private readonly ILogger<PersonaController> _logger;
    private readonly IPersonaService _personaService;
    private readonly IInquilinoService _inquilinoService;
    private readonly IPropietarioService _propietarioService;

    public PersonaController(ILogger<PersonaController> logger)
    {
        _logger = logger;
        _personaService = new PersonaServiceImpl();
        _inquilinoService = new InquilinoServiceImpl();
        _propietarioService = new PropietarioServiceImpl();
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var personas = await _personaService.ObtenerTodosAsync();
            return View(personas);
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
    public IActionResult Create()
    {
        try
        {   
            

            return View();
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
    [ValidateAntiForgeryToken] // Buena práctica para prevenir ataques CSRF
    public async Task<IActionResult> Create(Persona persona)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // Crea la persona y obtiene el ID generado
                var personaId = await _personaService.NuevoAsync(persona);

                // Obntiene el tipo de persona desde el formulario
                if (persona.TipoPersona == null || persona.TipoPersona.Count == 0)
                {
                    ModelState.AddModelError("TipoPersona", "Debe seleccionar al menos un tipo de persona.");
                    return View(persona);
                }

                foreach (var tipo in persona.TipoPersona)
                {
                    switch (tipo)
                    {
                        case "inquilino":
                            await _inquilinoService.NuevoAsync(personaId);
                            break;
                        case "propietario":
                            await _propietarioService.NuevoAsync(personaId);
                            break;
                        default:
                            throw new ValidationException("Tipo de persona inválido.");
                    }
                }

                /* switch (persona.TipoPersona)
                {
                    case "inquilino":
                        var inquilino = await _inquilinoService.NuevoAsync(personaId);
                        break;
                    case "propietario":
                        var propietario = await _propietarioService.NuevoAsync(personaId);
                        break;
                    default:
                        throw new ValidationException("Tipo de persona inválido.");
                }
                 */

                return RedirectToAction("Index");
            }
            else
            {
                Console.WriteLine("ModelState NO es válido"); // <-- Esto indica que falla validación
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"Error en {key}: {error.ErrorMessage}");
                    }
                }
                return View(persona);
            }
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
            var persona =  await  _personaService.ObtenerIdAsync(id);

            if (persona == null)
            {
                TempData["Error"] = "La persona no existe.";
                return RedirectToAction("Index");
            }

            return View("Create", persona);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al editar persona");
            TempData["Error"] = "Error al cargar la persona.";
            return View("Error");
        }
    }

}
