
console.log('Select2 cargado');
console.log($('#PropietarioId').val());

$('#PropietarioId').select2({
    placeholder: 'Buscar propietario...',
    minimumInputLength: 2,
    language: {
        noResults: function() {
            return "No se encontraron propietarios";
        },
        inputTooShort: function() {
            return "Ingrese al menos 2 caracteres";
        },
        searching: function() {
            return "Buscando...";
        }
    },
    ajax: {
        url: '/Propietario/Buscar', // Acción que retorna JSON con los propietarios
        dataType: 'json',
        delay: 250,
        data: function (params) {
            return { term: params.term };
        },
        processResults: function (data) {
            return {
                results: data.map(function (item) {
                    return { id: item.id, text: item.text };
                })
            };
        }
    }
});