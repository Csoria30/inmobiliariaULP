
            document.addEventListener('DOMContentLoaded', function () {
                var modal = document.getElementById('modalEliminarPersona');
                if (modal) {
                    modal.addEventListener('show.bs.modal', function (event) {
                        var button = event.relatedTarget;
                        var personaId = button.getAttribute('data-personaid');
                        var personaNombre = button.getAttribute('data-personanombre');
                        var personaEstado = button.classList.contains('btn-outline-danger'); // true = habilitado, false = deshabilitado

                        document.getElementById('personaIdEliminar').value = personaId;
                        document.getElementById('nombrePersonaEliminar').textContent = personaNombre;

                        // Cambia la acción del form para que apunte al método Delete del controlador
                        document.getElementById('formEliminarPersona').action = '/Persona/Delete/' + personaId;

                        // Cambia el mensaje, color y botón según el estado
                        var accion = personaEstado ? "deshabilitar" : "habilitar";
                        var color = personaEstado ? "danger" : "success";
                        var icono = personaEstado ? "bi-exclamation-triangle" : "bi-arrow-repeat";

                        document.getElementById('accionPersonaEstado').textContent = accion;
                        document.getElementById('modalTituloEstado').textContent = personaEstado ? "Confirmar deshabilitación" : "Confirmar habilitación";
                        document.getElementById('btnConfirmarEstado').textContent = personaEstado ? "Deshabilitar" : "Habilitar";
                        document.getElementById('btnConfirmarEstado').className = "btn btn-" + color;

                        // Cambia el color del header y el icono
                        var header = document.getElementById('modalHeaderEstado');
                        header.className = "modal-header bg-" + color + " text-white";
                        header.querySelector('i').className = "bi " + icono + " me-2";
                    });
                }
            });
