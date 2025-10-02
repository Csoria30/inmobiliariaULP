document.addEventListener('DOMContentLoaded', function() {
    inicializarTooltips(); // INICIALIZAR tooltips de Bootstrap
    configurarValidacionesFormulario(); // VALIDACIONES del formulario en tiempo real
    configurarMejorasUX(); //MEJORAR experiencia de usuario
});

//* FUNCIÓN: Inicializar tooltips
function inicializarTooltips() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.forEach(function (tooltipTriggerEl) {
        new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

//* FUNCIÓN: Configurar validaciones del formulario
function configurarValidacionesFormulario() {
    const fechaInicio = document.getElementById('FechaInicio');
    const fechaFin = document.getElementById('FechaFin');
    const precioMin = document.getElementById('PrecioMin');
    const precioMax = document.getElementById('PrecioMax');
    const btnBuscar = document.getElementById('btnBuscar');

    if (!fechaInicio || !fechaFin) return;

    //? VALIDACION: Fechas en tiempo real
    function validarFechas() {
        const inicio = new Date(fechaInicio.value);
        const fin = new Date(fechaFin.value);
        
        if (fechaInicio.value && fechaFin.value) {
            if (inicio >= fin) {
                mostrarErrorCampo(fechaFin, 'La fecha de fin debe ser posterior a la fecha de inicio');
                return false;
            } else {
                limpiarErrorCampo(fechaFin);
                
                //? Calcular duración y mostrar información
                const duracion = Math.ceil((fin - inicio) / (1000 * 60 * 60 * 24));
                mostrarInfoDuracion(duracion);
                return true;
            }
        }
        return true;
    }

    //* VALIDACION: Precios en tiempo real
    function validarPrecios() {
        if (precioMin && precioMax && precioMin.value && precioMax.value) {
            const min = parseFloat(precioMin.value);
            const max = parseFloat(precioMax.value);
            
            if (min >= max) {
                mostrarErrorCampo(precioMax, 'El precio máximo debe ser mayor al precio mínimo');
                return false;
            } else {
                limpiarErrorCampo(precioMax);
                return true;
            }
        }
        return true;
    }

    //? EVENTOS: de validación
    fechaInicio?.addEventListener('change', validarFechas);
    fechaFin?.addEventListener('change', validarFechas);
    precioMin?.addEventListener('input', validarPrecios);
    precioMax?.addEventListener('input', validarPrecios);

    //* VALIDACION:  antes de enviar formulario
    const formulario = document.querySelector('form[action*="BuscarDisponibles"]');
    if (formulario && btnBuscar) {
        formulario.addEventListener('submit', function(e) {
            if (!validarFechas() || !validarPrecios()) {
                e.preventDefault();
                mostrarAlerta('Por favor corrija los errores en el formulario antes de continuar.', 'warning');
            } else {
                // Mostrar loading en el botón
                mostrarLoadingBoton(btnBuscar);
            }
        });
    }
}


//-  FUNCION: Mejoras de UX
function configurarMejorasUX() {
    //? AUTO-COMPLETAR fecha de inicio con hoy - Aunque ya venga del Controller 
    const fechaInicio = document.getElementById('FechaInicio');
    if (fechaInicio && !fechaInicio.value) {
        fechaInicio.value = new Date().toISOString().split('T')[0];
    }

    //? AUTO-COMPLETAR fecha de fin con un año después
    const fechaFin = document.getElementById('FechaFin');
    if (fechaFin && !fechaFin.value && fechaInicio?.value) {
        const fechaInicioDate = new Date(fechaInicio.value);
        fechaInicioDate.setFullYear(fechaInicioDate.getFullYear() + 1);
        fechaFin.value = fechaInicioDate.toISOString().split('T')[0];
    }

    //? FORMATO de números en campos de precio
    const precioMin = document.getElementById('PrecioMin');
    const precioMax = document.getElementById('PrecioMax');
    
    [precioMin, precioMax].forEach(campo => {
        if (campo) {
            campo.addEventListener('input', function() {
                formatearPrecio(this);
            });
        }
    });

    // !  ANIMACIONES suaves para alertas
    setTimeout(() => {
        const alertas = document.querySelectorAll('.alert');
        alertas.forEach(alerta => {
            alerta.classList.add('fade-in');
        });
    }, 100);
}


//*  FUNCION: Mostrar error en campo
function mostrarErrorCampo(campo, mensaje) {
    limpiarErrorCampo(campo);
    
    campo.classList.add('is-invalid');
    
    const feedback = document.createElement('div');
    feedback.className = 'invalid-feedback';
    feedback.textContent = mensaje;
    
    campo.parentNode.appendChild(feedback);
}

//*  FUNCION: Limpiar error de campo
function limpiarErrorCampo(campo) {
    campo.classList.remove('is-invalid');
    
    const feedback = campo.parentNode.querySelector('.invalid-feedback');
    if (feedback) {
        feedback.remove();
    }
}

//*  FUNCION: Mostrar información de duración
function mostrarInfoDuracion(dias) {
    const info = document.getElementById('info-duracion') || crearElementoInfo();
    
    let mensaje = `Período: ${dias} días`;
    if (dias >= 365) {
        const años = Math.floor(dias / 365);
        const mesesRestantes = Math.floor((dias % 365) / 30);
        mensaje += ` (${años} año${años > 1 ? 's' : ''}${mesesRestantes > 0 ? ` y ${mesesRestantes} mes${mesesRestantes > 1 ? 'es' : ''}` : ''})`;
    }
    
    info.innerHTML = `<i class="bi bi-info-circle me-1"></i>${mensaje}`;
    info.className = 'small text-muted mt-1';
}

//*  FUNCION: Crear elemento de información
function crearElementoInfo() {
    const fechaFin = document.getElementById('FechaFin');
    const info = document.createElement('div');
    info.id = 'info-duracion';
    fechaFin.parentNode.appendChild(info);
    return info;
}

//*  FUNCION: Formatear precio con separadores de miles
function formatearPrecio(campo) {
    let valor = campo.value.replace(/[^\d.,]/g, '');
    // Aquí puedes agregar lógica de formateo si quieres
    campo.value = valor;
}

//* FUNCION: Mostrar loading en botón
function mostrarLoadingBoton(boton) {
    const textoOriginal = boton.innerHTML;
    boton.innerHTML = '<i class="bi bi-arrow-clockwise spin me-1"></i>Buscando...';
    boton.disabled = true;
    
    // Agregar datos para restaurar después
    boton.dataset.textoOriginal = textoOriginal;
}

//*  FUNCION: Mostrar alerta dinámica
function mostrarAlerta(mensaje, tipo = 'info') {
    const contenedor = document.querySelector('.container-fluid') || document.body;
    
    const alerta = document.createElement('div');
    alerta.className = `alert alert-${tipo} alert-dismissible fade show`;
    alerta.innerHTML = `
        <i class="bi bi-${tipo === 'warning' ? 'exclamation-triangle' : 'info-circle'} me-2"></i>
        ${mensaje}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    contenedor.insertBefore(alerta, contenedor.firstChild);
    
    // Auto-remover después de 5 segundos
    setTimeout(() => {
        if (alerta.parentNode) {
            alerta.remove();
        }
    }, 5000);
}

// ✅ FUNCIÓN GLOBAL: Crear contrato con inmueble (para botones en la tabla)
function crearContratoConInmueble(inmuebleId) {
    const fechaInicio = document.getElementById('FechaInicio')?.value;
    const fechaFin = document.getElementById('FechaFin')?.value;
    
    if (!fechaInicio || !fechaFin) {
        mostrarAlerta('No se pueden obtener las fechas seleccionadas', 'warning');
        return;
    }
    
    const url = `/Contrato/Create?inmuebleId=${inmuebleId}&fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`;
    window.location.href = url;
}

// ! FUNCION: Limpiar formulario
function limpiarFormulario() {
    const formulario = document.querySelector('form[action*="BuscarDisponibles"]');
    if (formulario) {
        formulario.reset();
        
        // Limpiar errores de validación
        const camposConError = formulario.querySelectorAll('.is-invalid');
        camposConError.forEach(campo => limpiarErrorCampo(campo));
        
        // Restablecer valores por defecto
        configurarMejorasUX();
        
        mostrarAlerta('Filtros limpiados correctamente', 'success');
    }
}

// ! ESTILOS: Animaciones y estilos para validaciones
const estilos = document.createElement('style');
estilos.textContent = `
    .fade-in {
        animation: fadeIn 0.5s ease-in;
    }
    
    @keyframes fadeIn {
        from { opacity: 0; transform: translateY(-10px); }
        to { opacity: 1; transform: translateY(0); }
    }
    
    .spin {
        animation: spin 1s linear infinite;
    }
    
    @keyframes spin {
        from { transform: rotate(0deg); }
        to { transform: rotate(360deg); }
    }
    
    .is-invalid {
        border-color: #dc3545;
    }
    
    .invalid-feedback {
        display: block;
        color: #dc3545;
        font-size: 0.875em;
        margin-top: 0.25rem;
    }
`;
document.head.appendChild(estilos);