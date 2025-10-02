using Microsoft.AspNetCore.Mvc;
using inmobiliariaULP.Models;
using inmobiliariaULP.Models.ViewModels;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Services.Implementations;
using inmobiliariaULP.Helpers;
using System.Security.Claims;


namespace inmobiliariaULP.Controllers;

public class PagoController : Controller
{
    private readonly ILogger<PagoController> _logger;
    private readonly IPersonaService _personaService;
    private readonly IInquilinoService _inquilinoService;
    private readonly IPropietarioService _propietarioService;
    private readonly IUsuarioService _usuarioService;
    private readonly IContratoService _contratoService;
    private readonly IPagoService _pagoService;

    public PagoController(
        ILogger<PagoController> logger,
        IPersonaService personaService,
        IInquilinoService inquilinoService,
        IPropietarioService propietarioService,
        IUsuarioService usuarioService,
        IContratoService contratoService,
        IPagoService pagoService
    )
    {
        _logger = logger;
        _personaService = personaService;
        _inquilinoService = inquilinoService;
        _propietarioService = propietarioService;
        _usuarioService = usuarioService;
        _contratoService = contratoService;
        _pagoService = pagoService;
    }

    // Ejemplo de acción Index
    public IActionResult Index()
    {
        return View();
    }

    //* GET: PagoController/Create
    [HttpGet]
    public async Task<IActionResult> Create(int? contratoId)
    {
        try
        {
            var pagoDto = new PagoDTO
            {
                FechaPago = DateTime.Today,
                EstadoPago = "aprobado"
            };

            // Cargar la información del contrato
            if (contratoId.HasValue && contratoId.Value > 0)
            {
                var contrato = await _contratoService.ObtenerPorIdAsync(contratoId.Value);

                if (contrato != null)
                {
                    pagoDto.IdContrato = contrato.ContratoId;
                    pagoDto.DireccionInmueble = contrato.Direccion;
                    pagoDto.NombreInquilino = contrato.NombreInquilino;
                    pagoDto.MontoMensualContrato = contrato.MontoMensual;
                    pagoDto.Importe = contrato.MontoMensual;

                    // Generar número automático
                    pagoDto.NumeroPago = await _pagoService.GenerarNumeroAutomaticoAsync(contratoId.Value);

                    // Sugerir concepto
                    var mesActual = DateTime.Now.ToString("MMMM yyyy", new System.Globalization.CultureInfo("es-ES"));
                    pagoDto.Concepto = $"Pago de alquiler mes de {mesActual}";
                }
                else
                {
                    TempData["Error"] = "No se encontró el contrato especificado.";
                    return RedirectToAction("Index", "Contrato");
                }
            }
            else
            {
                TempData["Info"] = "Seleccione un contrato para registrar el pago.";
                return RedirectToAction("Index", "Contrato");
            }

            // Obtener el usuario actual
            var usuarioId = User.FindFirst("UsuarioId")?.Value;
            var fullName = User.FindFirst("FullName")?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (!string.IsNullOrEmpty(usuarioId) && int.TryParse(usuarioId, out int usuarioIdInt))
            {
                pagoDto.IdUsuario = usuarioIdInt;
                pagoDto.NombreUsuario = fullName;
                pagoDto.EmailUsuario = email;
            }

            return View(pagoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la vista Create de pagos");
            TempData["Error"] = "Error al cargar el formulario de pago: " + ex.Message;
            return RedirectToAction("Index", "Contrato");
        }
    }


    //! POST: PagoController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PagoDTO pagoDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {                
                var errores = ModelStateHelper.GetErrors(ModelState);
                TempData["Error"] = string.Join(". ", errores);
                return View(pagoDto);
            }

            // Validar posibilidad de realizar el pago
            var (puedeRealizarPago, razon) = await _pagoService.ValidarPosibilidadPagoAsync(pagoDto.IdContrato, pagoDto.Importe);
            if (!puedeRealizarPago)
            {
                TempData["Warning"] = razon;
                return View(pagoDto);
            }

            var (exito, mensaje, tipo) = await _pagoService.CrearAsync(pagoDto);
            
            TempData["Notificacion"] = mensaje;
            TempData["NotificacionTipo"] = tipo;

            if (exito)
            {
                //return RedirectToAction("Details", "Contrato", new { id = pagoDto.IdContrato });
                return RedirectToAction("Index", "Contrato");
            }

            return View(pagoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el pago");
            TempData["Error"] = "Error al registrar el pago: " + ex.Message;
            return View(pagoDto);
        }
    }

}