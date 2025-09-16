$(document).ready(function () {
    if(!$.fn.DataTable.isDataTable('#inmueblesTable')){
        $('#inmueblesTable').DataTable({
            processing: true,
            serverSide: true,
            ajax: {
                url: '/Inmueble/ObtenerDataTable',
                type: 'POST'
            },
            columns: [
                { data: 'direccion' },
                { data: 'uso' },
                { data: 'estado' },
                { data: 'descripcion' } ,
                { data: 'acciones', orderable: false, searchable: false },
                /* { data: 'ambientes' },
                { data: 'tipo.descripcion' } 
                { data: 'coordenadas' },
                { data: 'propietario.nombre' },
                { data: 'precio_base' },
                { data: 'propietario.apellido' },
                 */
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