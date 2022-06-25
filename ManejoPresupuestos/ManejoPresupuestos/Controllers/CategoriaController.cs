using ManejoPresupuestos.Models;
using ManejoPresupuestos.Models.ViewModels;
using ManejoPresupuestos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuestos.Controllers {
    public class CategoriaController : Controller {
        private readonly IRepositoryCategoria repositoryCategoria;
        private readonly IServicioUsuario servicioUsuario;

        public CategoriaController (
            IRepositoryCategoria repositoryCategoria,
            IServicioUsuario servicioUsuario
        ) {
            this.repositoryCategoria = repositoryCategoria;
            this.servicioUsuario = servicioUsuario;
        }

        [HttpGet]
        public async Task<IActionResult> Index(PaginacionViewModel paginacion) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var categorias = await repositoryCategoria.Obtener(usuarioId, paginacion);
            var totalCategorias = await repositoryCategoria.Contar(usuarioId);

            var respuestaVM = new PaginacionRespuesta<Categoria> {
                Elementos = categorias,
                Pagina = paginacion.Pagina,
                RecordsPorPagina = paginacion.RecordsPorPagina,
                CantidadTotalRecords = totalCategorias,
                BaseURL = Url.Action()
            };

            return View(respuestaVM);
        }

        [HttpGet]
        public IActionResult Crear() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Categoria categoria) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();

            if (!ModelState.IsValid) {
                return View(categoria);
            }

            categoria.UsuarioId = usuarioId;
            await repositoryCategoria.Crear(categoria);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var categoria = await repositoryCategoria.ObtenerPorId(id, usuarioId);

            if (categoria is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Categoria categoriaEditar) {
            if (!ModelState.IsValid) {
                return View(categoriaEditar);
            }

            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var categoria = await repositoryCategoria.ObtenerPorId(categoriaEditar.Id, usuarioId);

            if (categoria is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            categoriaEditar.UsuarioId = usuarioId;
            await repositoryCategoria.Actualizar(categoriaEditar);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Borrar(int id) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var categoria = await repositoryCategoria.ObtenerPorId(id, usuarioId);

            if (categoria is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCategoria(int id) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var categoria = await repositoryCategoria.ObtenerPorId(id, usuarioId);

            if (categoria is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositoryCategoria.Borrar(id);

            return RedirectToAction("Index");
        }
    }
}