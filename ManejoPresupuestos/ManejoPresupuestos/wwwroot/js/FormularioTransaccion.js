/*Este script es usado para el select box en cascada, ya que cuando
se escoge un valor en el tipo de operacion que queremos, se hace una
peticion al controlador de transacciones, en la accion de ObtenerCategorias
este nos regresa un json con las categorias para cargarlas en el segundo
select box*/

function inicializarFormularioTransacciones(urlObtenerCategorias) {
    $("#TipoOperacionId").change(async function () {
        const valorSeleccionado = $(this).val();

        const respuesta = await fetch(urlObtenerCategorias, {
            method: 'POST',
            body: valorSeleccionado,
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const json = await respuesta.json();
        const opciones = json.map(categoria => `<option value=${categoria.value}>${categoria.text}</option>`);
        $("#CategoriaId").html(opciones);
    });
}