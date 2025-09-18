document.addEventListener('DOMContentLoaded', function () {
    var modal = document.getElementById('modalEliminarInmueble');
    if (modal) {
        modal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            var inmuebleId = button.getAttribute('data-inmuebledid');
            var inmuebleNombre = button.getAttribute('data-inmueblednombre');
            var inmuebleEstado = button.getAttribute('data-inmuebleestado') == '1'; // true = habilitado, false = deshabilitado}

            document.getElementById('inmuebleIdEliminar').value = inmuebleId;

            //Cambia accion del form para que apunte al metodo Delete del controlador
            document.getElementById('formEliminarInmueble').action = '/Inmueble/Delete/' + inmuebleId;

            //Cambia el mensaje, color y boton segun el estado
            var accion = inmuebleEstado ? "deshabilitar" : "habilitar";
            var color = inmuebleEstado ? "danger" : "success";
            var icono = inmuebleEstado ? "bi-exclamation-triangle" : "bi-arrow-repeat";

            document.getElementById('modalTituloEstado').textContent = inmuebleEstado ? "Confirmar deshabilitación" : "Confirmar habilitación";
            document.getElementById('btnConfirmarEstado').textContent = inmuebleEstado ? "Deshabilitar" : "Habilitar";
            document.getElementById('btnConfirmarEstado').className = "btn btn-" + color;

            //Cambia color del Header
            var header = document.getElementById('modalHeaderEstado');
            header.className = "modal-header bg-" + color + " text-white";
            header.querySelector('i').className = "bi " + icono + " me-2";
            
        });
    }
});