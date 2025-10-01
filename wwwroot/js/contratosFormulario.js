//=========================================
// FUNCIONES DE CÁLCULO
//=========================================

/**
 * Calcula los días restantes entre la fecha de fin y hoy
 */
function calcularDiasRestantes() {
    var inicio = document.getElementById('FechaInicio').value;
    var fin = document.getElementById('FechaFin').value;
    
    if (fin) {
        var fechaFin = new Date(fin);
        var hoy = new Date();
        hoy.setHours(0, 0, 0, 0);
        var dias = Math.ceil((fechaFin - hoy) / (1000 * 60 * 60 * 24));
        document.getElementById('DiasRestantes').value = dias > 0 ? dias : 0;
    } else {
        document.getElementById('DiasRestantes').value = '';
    }
}

//=========================================
// FUNCIONES DE VALIDACIÓN Y UX
//=========================================

/**
 * Encuentra el primer campo con error en el formulario
 */
function encontrarPrimerError() {
    const tiposDeError = [
        '.text-danger:not(:empty)',           // Mensajes de validación visibles
        '.select2-container.is-invalid',      // Select2 con errores
        'input.is-invalid',                   // Inputs con errores
        'select.is-invalid',                  // Selects con errores
        'textarea.is-invalid',                // Textareas con errores
        '.field-validation-error'             // Clase de ASP.NET para errores
    ];
    
    for (const selector of tiposDeError) {
        const elemento = document.querySelector(selector);
        if (elemento) {
            return elemento;
        }
    }
    
    return null;
}

/**
 * Función auxiliar para marcar campos con error
 */
function marcarCampoConError(fieldId, mensaje) {
    const field = document.getElementById(fieldId);
    if (field) {
        field.classList.add('is-invalid');
        
        const errorSpan = field.parentElement.querySelector('.text-danger');
        if (errorSpan) {
            errorSpan.textContent = mensaje;
            errorSpan.style.display = 'block';
        }
    }
}

/**
 * Función auxiliar para limpiar errores de un campo
 */
function limpiarErrorCampo(fieldId) {
    const field = document.getElementById(fieldId);
    if (field) {
        field.classList.remove('is-invalid');
        
        const errorSpan = field.parentElement.querySelector('.text-danger');
        if (errorSpan) {
            errorSpan.textContent = '';
            errorSpan.style.display = 'none';
        }
    }
}

/**
 * Validaciones personalizadas de fechas (como en el modelo C#)
 */
/* function validarFechasPersonalizadas() {
    const fechaInicio = document.getElementById('FechaInicio').value;
    const fechaFin = document.getElementById('FechaFin').value;
    
    if (fechaInicio && fechaFin) {
        const inicio = new Date(fechaInicio);
        const fin = new Date(fechaFin);
        const hoy = new Date();
        hoy.setHours(0, 0, 0, 0);
        
        // Limpiar errores previos de fechas
        limpiarErrorCampo('FechaInicio');
        limpiarErrorCampo('FechaFin');
        
        // 1. Validar que fecha fin sea posterior a fecha inicio
        if (fin <= inicio) {
            marcarCampoConError('FechaFin', 'La fecha de fin debe ser posterior a la fecha de inicio');
            return false;
        }
        
        // 2. Validar que fecha inicio no sea anterior a hoy
        if (inicio < hoy) {
            marcarCampoConError('FechaInicio', 'La fecha de inicio no puede ser anterior a la fecha actual');
            return false;
        }
        
        // 3. Validar duración mínima de 30 días
        const diasDiferencia = Math.ceil((fin - inicio) / (1000 * 60 * 60 * 24));
        if (diasDiferencia < 30) {
            marcarCampoConError('FechaFin', 'El contrato debe tener una duración mínima de 30 días');
            return false;
        }
    }
    
    return true;
} */

/**
 * Detecta errores del servidor y marca los campos
 */
function detectarErroresServidor() {
    const modelStateErrors = [
        { field: 'InmuebleId', message: 'Debe seleccionar un inmueble' },
        { field: 'InquilinoId', message: 'Debe seleccionar un inquilino' },
        { field: 'FechaInicio', message: 'La fecha de inicio es obligatoria' },
        { field: 'FechaFin', message: 'La fecha de fin es obligatoria' },
        { field: 'MontoMensual', message: 'El monto mensual es obligatorio' },
        { field: 'EstadoContrato', message: 'Debe seleccionar el estado del contrato' }
    ];
    
    // Verificar errores básicos SOLO si los campos están realmente vacíos
    modelStateErrors.forEach(error => {
        const field = document.getElementById(error.field);
        if (field) {
            let hasError = false;
            
            // Validar Select2 (InmuebleId, InquilinoId)
            if (error.field === 'InmuebleId' || error.field === 'InquilinoId') {
                if (!field.value || field.value === '0' || field.value === '') {
                    $(field).next('.select2-container').addClass('is-invalid');
                    hasError = true;
                }
            }
            // Validar campos de fecha - SOLO si están realmente vacíos
            else if (error.field === 'FechaInicio' || error.field === 'FechaFin') {
                if (!field.value || field.value === '') {
                    field.classList.add('is-invalid');
                    hasError = true;
                }
                // Si tiene valor, NO marcar como error aquí
            }
            // Validar campos numéricos
            else if (error.field === 'MontoMensual') {
                if (!field.value || field.value === '' || parseFloat(field.value) <= 0) {
                    field.classList.add('is-invalid');
                    hasError = true;
                }
            }
            // Validar selects normales
            else if (error.field === 'EstadoContrato') {
                if (!field.value || field.value === '') {
                    field.classList.add('is-invalid');
                    hasError = true;
                }
            }
            
            // Agregar mensaje de error SOLO si hay error
            if (hasError) {
                const errorSpan = field.parentElement.querySelector('.text-danger');
                if (errorSpan && errorSpan.textContent.trim() === '') {
                    errorSpan.textContent = error.message;
                    errorSpan.style.display = 'block';
                }
            }
        }
    });

    // VALIDACIONES PERSONALIZADAS - solo si ambas fechas tienen valor
    const fechaInicio = document.getElementById('FechaInicio').value;
    const fechaFin = document.getElementById('FechaFin').value;
    
    /* if (fechaInicio && fechaFin) {
        validarFechasPersonalizadas();
    } */
}

/**
 * Hace scroll automático al primer campo con error y lo destaca
 */
function scrollToFirstError() {
    const firstError = encontrarPrimerError();
    
    if (firstError) {
        const container = firstError.closest('.mb-3') || 
                         firstError.closest('.col-md-6') || 
                         firstError.closest('.card-body') ||
                         firstError.closest('.form-group');
        
        if (container) {
            // Scroll suave hacia el campo con error
            container.scrollIntoView({ 
                behavior: 'smooth', 
                block: 'center' 
            });
            
            // Destacar el campo con animación
            container.style.animation = 'errorPulse 1.5s ease-in-out';
            
            // Manejar diferentes tipos de campos
            const select2Field = container.querySelector('.select2-hidden-accessible');
            const inputField = container.querySelector('input, select, textarea');
            
            if (select2Field) {
                // Para Select2
                setTimeout(() => {
                    $(select2Field).select2('open');
                    setTimeout(() => $(select2Field).select2('close'), 800);
                }, 500);
            } else if (inputField && !inputField.readOnly && !inputField.disabled) {
                // Para campos normales
                setTimeout(() => {
                    inputField.focus();
                }, 500);
            }
        }
    }
}

/**
 * Validación completa del formulario antes del envío
 */
function validarFormulario() {
    // Limpiar TODAS las clases de validación antes de validar
    document.querySelectorAll('input, select, textarea').forEach(el => {
        el.classList.remove('is-invalid', 'valid', 'field-validation-error');
    });
    document.querySelectorAll('.select2-container').forEach(el => {
        el.classList.remove('is-invalid');
    });
    document.querySelectorAll('.text-danger').forEach(el => {
        el.textContent = '';
        el.style.display = 'none';
    });
    
    // Detectar errores del servidor
    detectarErroresServidor();
    
    // Verificar si hay campos con errores
    const camposConError = document.querySelectorAll('.select2-container.is-invalid, .is-invalid');
    const mensajesError = Array.from(document.querySelectorAll('.text-danger:not(:empty)')).map(el => el.textContent);
    
    if (camposConError.length > 0) {
        // Hacer scroll al primer error
        scrollToFirstError();
        
        // Mostrar mensaje detallado
        /* if (mensajesError.length > 0) {
            setTimeout(() => {
                alert('Errores encontrados:\n\n• ' + mensajesError.join('\n• '));
            }, 600);
        } */
        
        return false;
    }
    
    return true;
}

//=========================================
// INICIALIZACIÓN DE ESTILOS CSS
//=========================================

/**
 * Agrega estilos CSS para la animación de errores
 */
function initializeErrorStyles() {
    const errorAnimationStyle = document.createElement('style');
    errorAnimationStyle.textContent = `
        @keyframes errorPulse {
            0%, 100% { 
                transform: scale(1); 
            }
            50% { 
                transform: scale(1.02); 
                box-shadow: 0 0 15px rgba(220, 53, 69, 0.3); 
            }
        }
        
        .select2-container.is-invalid .select2-selection {
            border-color: #dc3545 !important;
            box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.25) !important;
        }
    `;
    document.head.appendChild(errorAnimationStyle);
}

//=========================================
// EVENT LISTENERS Y INICIALIZACIÓN
//=========================================

/**
 * Configuración inicial al cargar el DOM
 */
document.addEventListener('DOMContentLoaded', function() {
    // Inicializar estilos
    initializeErrorStyles();
    
    // Configurar eventos de fechas
    const fechaInicio = document.getElementById('FechaInicio');
    const fechaFin = document.getElementById('FechaFin');
    
    if (fechaInicio && fechaFin) {
        fechaInicio.addEventListener('change', calcularDiasRestantes);
        fechaFin.addEventListener('change', calcularDiasRestantes);
        
        // Calcular días restantes al cargar la página
        calcularDiasRestantes();
    }
    
    // Interceptar envío del formulario
    const form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', function(e) {
            if (!validarFormulario()) {
                e.preventDefault();
                return false;
            }
        });
    }

    setTimeout(() => {
        // Buscar errores reales que vienen del servidor (spans con contenido)
        const errorSpansWithContent = document.querySelectorAll('.text-danger:not(:empty)');
        
        if (errorSpansWithContent.length > 0) {
            console.log('Errores del servidor encontrados:', errorSpansWithContent);
            scrollToFirstError();
        } else {
            console.log('No hay errores del servidor, no ejecutar validaciones automáticas');
        }
    }, 300);
    
    // Limpiar errores cuando el usuario interactúe con los campos
    $('#InmuebleId, #InquilinoId').on('select2:select', function() {
        $(this).next('.select2-container').removeClass('is-invalid');
        const errorSpan = this.parentElement.querySelector('.text-danger');
        if (errorSpan) {
            errorSpan.textContent = '';
            errorSpan.style.display = 'none';
        }
    });
    
    // Limpiar errores en campos de fecha y número CON VALIDACIÓN EN TIEMPO REAL
    $('#FechaInicio, #FechaFin').on('change input', function() {
        // Limpiar errores visuales
        this.classList.remove('is-invalid', 'valid'); // ← Limpiar AMBAS clases
        const errorSpan = this.parentElement.querySelector('.text-danger');
        if (errorSpan) {
            errorSpan.textContent = '';
            errorSpan.style.display = 'none';
        }
        
        // Recalcular días restantes
        calcularDiasRestantes();
        
        // Validar fechas en tiempo real
        setTimeout(() => {
            validarFechasPersonalizadas();
        }, 100);
    });
    
    $('#MontoMensual, #EstadoContrato').on('change input', function() {
        this.classList.remove('is-invalid', 'valid'); // ← Limpiar AMBAS clases
        const errorSpan = this.parentElement.querySelector('.text-danger');
        if (errorSpan) {
            errorSpan.textContent = '';
            errorSpan.style.display = 'none';
        }
    });
});