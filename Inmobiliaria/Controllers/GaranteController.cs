using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Controllers
{
    public class GaranteController : Controller
    {
        private readonly IConfiguration config;
        RepositorioGarante repositorioGarante;
        RepositorioContrato repositorioContrato;

        public GaranteController(IConfiguration config)
        {
            this.config = config;
            repositorioGarante = new RepositorioGarante(config);
            repositorioContrato = new RepositorioContrato(config);
        }

        // GET: Garante
        public ActionResult Index()
        {
            try
            {
                var lista = repositorioGarante.Obtener();
                ViewData[nameof(Garante)] = lista;
                ViewData["Tittle"] = nameof(Garante);
                ViewBag.Id = TempData["Id"];
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                return View(lista);
            }
            catch (Exception ex)
            {

                Json(new { Error = ex.Message });
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Garante/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Garante/Create
        public ActionResult Crear()
        {
            return View();
        }

        // POST: Garante/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Crear(Garante garante)
        {
            try
            {
                int res = repositorioGarante.Alta(garante);
                TempData["Id"] = garante.Id;
                TempData["Mensaje"] = $"Garante creado con éxito! Id: {res}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return RedirectToAction(nameof(Crear));
            }
        }

        // GET: Garante/Edit/5
        [Authorize]
        public ActionResult Editar(int id)
        {
            var garante = repositorioGarante.ObtenerGarante(id);
            return View(garante);
        }

        // POST: Garante/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int id, IFormCollection collection)
        {
            try
            {
                Garante garante = repositorioGarante.ObtenerGarante(id);
                garante.Nombre = collection["Nombre"];
                garante.Dni = collection["Dni"];
                garante.Telefono = collection["Telefono"];
                

                repositorioGarante.Modificar(garante);

                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(null);
            }
        }

        // GET: Garante/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Eliminar(int id)
        {
            try
            {
                var entidad = repositorioGarante.ObtenerGarante(id);
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                if (TempData.ContainsKey("Error"))
                    ViewBag.Error = TempData["Error"];
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Eliminar(int id, Garante entidad)
        {
            try
            {
                repositorioGarante.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException e)
            {
                if (e.Number == 547)
                {
                    TempData["Error"] = "No se pudo eliminar, está en uso.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocurrió un error inesperado.";
                return RedirectToAction(nameof(Index));
            }

        }
    }
}
