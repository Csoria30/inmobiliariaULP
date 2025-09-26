$(document).ready(function() {
    $('#inquilinoId').select2({
        placeholder: 'Buscar inmueble...',
        minimumInputLength: 2,
        language: {
            noResults: function() { return "No se encontraron Inquilinos"; },
            inputTooShort: function() { return "Ingrese al menos 2 caracteres"; },
            searching: function() { return "Buscando..."; }
        },
        ajax: {
            url: '/Contrato/BuscarHabilitadosInquilinos',
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return { term: params.term };
            }, 
            processResults: function (data) {
                console.log("Resultados recibidos de inquilinos:", data);
                return {
                    results: data.map(function (item) {
                        return {
                            id: item.inquilinoId,
                            text: item.text,
                            dni: item.dni,
                            nombreInquilino: item.nombreInquilino,
                            email: item.email,
                            telefono: item.telefono
                        };
                    })
                };
            }
        },
        templateResult: function (item) {
            return item.text;
        },
        templateSelection: function (item) {
            return item.text;
        }
    });

    // Autocompletar campos al seleccionar un inmueble
    $('#inquilinoId').on('select2:select', function(e) {
        var data = e.params.data;
        $('#NombreInquilino').val(data.nombreInquilino);
        $('#EmailInquilino').val(data.email);
        $('#TelefonoInquilino').val(data.telefono);
    });
});