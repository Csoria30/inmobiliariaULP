// wwwroot/js/contratos-datatable.js
$(document).ready(function () {
    if (!$.fn.DataTable.isDataTable('#contratosTable')) {
        $('#contratosTable').DataTable({
            processing: true,
            serverSide: true,
            ajax: {
                url: '/Contrato/ObtenerDataTable',
                type: 'POST'
            },
            columnDefs: [
                { targets: 0, className: 'text-center' } // Centra la primera columna (ID)
            ],
            columns: [
                { data: 'contratoId' },
                { data: 'direccion' },
                { data: 'tipoInmueble' },
                { data: 'nombrePropietario' },
                { data: 'nombreInquilino' },
                { data: 'fechaInicio' },
                { data: 'fechaFin' },
                { data: 'montoMensual' },
                { data: 'estadoContrato' },
                { data: 'pagosRealizados' },
                { data: 'acciones', orderable: false, searchable: false }
            ],
            dom: '<"row mb-3"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>' + 'rt' + '<"row mt-3"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
            language: {
                url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/es-ES.json',
                search: "Buscar:",
                lengthMenu: "Mostrar _MENU_ entradas",
                info: "Mostrando _START_ a _END_ de _TOTAL_ entradas",
                paginate: {
                    previous: "Anterior",
                    next: "Siguiente"
                }
            },
            drawCallback: function () {
                // Inicializa tooltips de Bootstrap en los nuevos elementos
                var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
                tooltipTriggerList.map(function (tooltipTriggerEl) {
                    return new bootstrap.Tooltip(tooltipTriggerEl);
                });
            }
        });
    }
});