// ~/js/buscar-inmuebles.js
function inicializarBusquedaInmuebles() {
    if (typeof $ === 'undefined' || typeof $.fn.DataTable === 'undefined') {
        console.log('Esperando jQuery y DataTables...');
        setTimeout(inicializarBusquedaInmuebles, 100);
        return;
    }

    $(document).ready(function () {
        console.log('✅ Inicializando búsqueda de inmuebles...');
        
        // ✅ Las fechas ya vienen del servidor (valores por defecto del ViewModel)
        
        // Verificar elementos
        if ($('#resultadosTable').length === 0) {
            console.error('❌ Tabla #resultadosTable no encontrada');
            return;
        }

        // Destruir tabla existente
        if ($.fn.DataTable.isDataTable('#resultadosTable')) {
            $('#resultadosTable').DataTable().destroy();
        }

        // ✅ INICIALIZAR DataTable
        var table = $('#resultadosTable').DataTable({
            processing: false,
            serverSide: false,
            deferLoading: 0, // No cargar automáticamente
            ajax: {
                url: '/Inmueble/GetInmueblesDisponibles',
                type: 'POST',
                data: function(d) {
                    return {
                        // ✅ USAR nombres exactos del ViewModel (PascalCase)
                        FechaInicio: $('#FechaInicio').val(),
                        FechaFin: $('#FechaFin').val(),
                        Uso: $('#Uso').val() || '',
                        Ambientes: $('#Ambientes').val() || '',
                        PrecioMin: $('#PrecioMin').val() || '',
                        PrecioMax: $('#PrecioMax').val() || '',
                        __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
                    };
                },
                dataSrc: function(json) {
                    console.log('📊 Respuesta del servidor:', json);
                    
                    if (json.success === false) {
                        console.error('❌ Error del servidor:', json.error);
                        alert('Error: ' + json.error);
                        return [];
                    }
                    
                    return json.data || [];
                },
                error: function(xhr, error, thrown) {
                    console.error('❌ Error AJAX:', xhr, error, thrown);
                    alert('Error de conexión: ' + (xhr.responseJSON?.error || 'No se pudo conectar con el servidor'));
                }
            },
            columns: [
                { data: 'direccion', title: 'Dirección', defaultContent: '-' },
                { data: 'tipoUso', title: 'Tipo/Uso', defaultContent: '-' },
                { data: 'ambientes', title: 'Ambientes', defaultContent: '0' },
                { data: 'precio', title: 'Precio', defaultContent: '$0' },
                { data: 'disponibilidad', title: 'Disponibilidad', orderable: false, defaultContent: '-' },
                { data: 'propietario', title: 'Propietario', defaultContent: '-' },
                { data: 'acciones', title: 'Acciones', orderable: false, searchable: false, defaultContent: '-' }
            ],
            columnDefs: [
                { targets: 0, className: 'text-start' },
                { targets: 3, className: 'text-end' },
                { targets: 4, className: 'text-center' },
                { targets: 6, className: 'text-center' }
            ],
            language: {
                search: "Buscar en resultados:",
                lengthMenu: "Mostrar _MENU_ entradas",
                info: "Mostrando _START_ a _END_ de _TOTAL_ entradas",
                emptyTable: "🔍 Seleccione fechas y presione 'Buscar' para ver inmuebles disponibles",
                zeroRecords: "No se encontraron inmuebles disponibles con los criterios seleccionados",
                paginate: {
                    previous: "Anterior",
                    next: "Siguiente"
                }
            }
        });

        // ✅ EVENTO: Buscar
        $('#btnBuscar').on('click', function() {
            console.log('🔍 Iniciando búsqueda...');
            
            const fechaInicio = $('#FechaInicio').val();
            const fechaFin = $('#FechaFin').val();
            
            if (!fechaInicio || !fechaFin) {
                alert('Por favor seleccione ambas fechas para buscar');
                return;
            }
            
            if (fechaInicio >= fechaFin) {
                alert('La fecha de inicio debe ser anterior a la fecha de fin');
                return;
            }
            
            const $btn = $(this);
            const textoOriginal = $btn.html();
            $btn.html('<i class="bi bi-arrow-clockwise spin me-1"></i>Buscando...').prop('disabled', true);
            
            table.ajax.reload(function(json) {
                console.log('✅ Búsqueda completada');
                $btn.html(textoOriginal).prop('disabled', false);
                
                const total = json.data ? json.data.length : 0;
                $('#contador-resultados').html(total > 0 ? `${total} inmuebles encontrados` : 'Sin resultados');
                $('#btnExportar').prop('disabled', total === 0);
            });
        });

        // ✅ EVENTO: Limpiar
        $('#btnLimpiar').on('click', function() {
            console.log('🗑️ Limpiando filtros...');
            
            // Reset form mantiene los valores por defecto del servidor
            document.getElementById('formBusqueda').reset();
            
            // Limpiar tabla
            table.clear().draw();
            $('#contador-resultados').html('');
            $('#btnExportar').prop('disabled', true);
        });
        
        console.log('✅ Búsqueda de inmuebles inicializada correctamente');
    });
}

// ✅ FUNCIÓN GLOBAL para crear contratos
function crearContratoConInmueble(inmuebleId) {
    const fechaInicio = $('#FechaInicio').val();
    const fechaFin = $('#FechaFin').val();
    
    window.location.href = `/Contrato/Create?inmuebleId=${inmuebleId}&fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`;
}

// Inicializar
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', inicializarBusquedaInmuebles);
} else {
    inicializarBusquedaInmuebles();
}