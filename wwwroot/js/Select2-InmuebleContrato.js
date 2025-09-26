$(document).ready(function() {
    $('#InmuebleId').select2({
        placeholder: 'Buscar inmueble...',
        minimumInputLength: 2,
        language: {
            noResults: function() { return "No se encontraron inmuebles"; },
            inputTooShort: function() { return "Ingrese al menos 2 caracteres"; },
            searching: function() { return "Buscando..."; }
        },
        ajax: {
            url: '/Contrato/BuscarHabilitados',
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return { term: params.term };
            }, 
            processResults: function (data) {
                console.log("Resultados recibidos de inmuebles:", data);
                return {
                    results: data.map(function (item) {
                        return {
                            id: item.inmuebleId,
                            text: item.text,
                            tipo: item.tipoInmueble,
                            uso: item.usoInmueble,
                            ambientes: item.ambientes,
                            coordenadas: item.coordenadas,
                            estado: item.estadoInmueble,
                            precioBase: item.precioBase,
                            propietarioId: item.propietarioId,
                            propietarioNombre: item.nombrePropietario,
                            propietarioEmail: item.emailPropietario,
                            propietarioTelefono: item.telefonoPropietario
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
    $('#InmuebleId').on('select2:select', function(e) {
        var data = e.params.data;
        $('#Direccion').val(data.text);
        $('#TipoInmueble').val(data.tipo);
        $('#UsoInmueble').val(data.uso);
        $('#Ambientes').val(data.ambientes);
        $('#Coordenadas').val(data.coordenadas);
        $('#EstadoInmueble').val(data.estado ? 'Habilitado' : 'Deshabilitado');
        $('#NombrePropietario').val(data.propietarioNombre);
        $('#EmailPropietario').val(data.propietarioEmail);
        $('#TelefonoPropietario').val(data.propietarioTelefono);
    });

    // Setear valor seleccionado al editar
    var inmuebleId = $('#InmuebleId').data('selected');
    // Intenta obtener la dirección desde el input o desde un atributo data-direccion
    var direccion = $('#Direccion').val() || $('#InmuebleId').data('direccion');

    if (inmuebleId && direccion) {
        var $select = $('#InmuebleId');
        if ($select.find("option[value='" + inmuebleId + "']").length === 0) {
            var newOption = new Option(direccion, inmuebleId, true, true);
            $select.append(newOption).trigger('change');
        } else {
            $select.val(inmuebleId).trigger('change');
        }
    }
});