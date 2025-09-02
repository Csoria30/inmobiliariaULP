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
                if (persona.PersonaId > 0)
                {
                    
                    

                }
                else
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

                }
                


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
            var persona = await _personaService.ObtenerIdAsync(id);

            if (persona == null)
            {
                TempData["Error"] = "La persona no existe.";
                return RedirectToAction(nameof(Index));
            }

            // Inicializamos la lista de tipos vacía si es null
            if (persona.TipoPersona == null)
                persona.TipoPersona = new List<string>();

            return View("Create", persona);
            
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

    public async Task<IActionResult> Edit(Persona persona)
    {

        // Mensaje inicial para saber que entró al método
        Console.WriteLine($"Entrando al método Edit para PersonaId: {persona.PersonaId}");

        // Validar el modelo
        if (!ModelState.IsValid)
        {
            // Si el modelo no es válido, retornar a la vista con los errores
            return View("Create", persona);
        }

        // Validar que la persona exista en la base de datos
        var personaExistente = await _personaService.ObtenerIdAsync(persona.PersonaId);
        if (personaExistente == null)
        {
            TempData["Error"] = "La persona que intenta editar no existe.";
            return RedirectToAction(nameof(Index));
        }

        // Actualizamos los datos básicos de la persona
        personaExistente.Nombre = persona.Nombre;
        personaExistente.Apellido = persona.Apellido;
        personaExistente.Dni = persona.Dni;
        personaExistente.Telefono = persona.Telefono;
        personaExistente.Email = persona.Email;

        try
        {
            
            //* Guardamos los cambios en la base de datos - Tabla personas
            await _personaService.ActualizarAsync(personaExistente);
            Console.WriteLine($"Persona {persona.PersonaId} actualizada.");

            

            //- PROPIETARIO
            var propietario = await _propietarioService.ObtenerIdAsync(persona.PersonaId);
            bool esPropietario = persona.TipoPersona != null && persona.TipoPersona.Contains("propietario");

            if (propietario != null)
            {
                await _propietarioService.ActualizarAsync(propietario.PropietarioId, esPropietario);
                TempData["Notificacion"] = "Perfil asignado correctamente.";
            }
            else
            {
                // No existe como propietario, lo creamos si fue seleccionado
                await _propietarioService.NuevoAsync(persona.PersonaId);
            }


            //- INQUILINO
            var inquilino = await _inquilinoService.ObtenerIdAsync(persona.PersonaId);
            bool esInquilino = persona.TipoPersona != null && persona.TipoPersona.Contains("inquilino");

            if (inquilino != null)
            {
                await _inquilinoService.ActualizarAsync(inquilino.InquilinoId, esInquilino);
            }
            else
            {
                // No existe como inquilino, lo creamos si fue seleccionado
                await _inquilinoService.NuevoAsync(persona.PersonaId);
            }

            
            // ✅ Al terminar exitosamente, redirigimos al Index
            TempData["Exito"] = "Persona actualizada correctamente.";
            return RedirectToAction(nameof(Index));

                        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar persona {PersonaId}", persona.PersonaId);
            TempData["Error"] = "Error al guardar los cambios.";
            return View("Create", persona);
        }
        
    }

}