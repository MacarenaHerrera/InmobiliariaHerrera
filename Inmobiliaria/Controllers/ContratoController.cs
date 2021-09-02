using Inmobiliaria.Models;
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
    public class ContratoController : Controller
    {
       RepositorioContrato repositorioContrato;
       RepositorioInquilino repositorioInquilino;
        RepositorioInmueble repositorioInmueble;
        private readonly IConfiguration configuration;

        public ContratoController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioInmueble = new RepositorioInmueble(configuration);
            repositorioInquilino = new RepositorioInquilino(configuration);
            repositorioContrato = new RepositorioContrato(configuration);
        }

        // GET: ContratoController
        public ActionResult Index()
        {
            try
            {
                var lista = repositorioContrato.ObtenerTodos();
                ViewData[nameof(Contrato)] = lista;
                ViewData["Tittle"] = nameof(Contrato);
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

        public ActionResult PorInmueble(int id)
        {
            IList<Contrato> lista = repositorioContrato.ObtenerPorInmueble(id);
            Inmueble ent = repositorioInmueble.ObtenerPorId(id);
            if (ent == null) return RedirectToAction(nameof(Index));
            ViewBag.PorInmueble = ent;
            return View("Index", lista);
        }

        // GET: ContratoController/Details/5
        public ActionResult Details(int id)
        {
            var entidad = repositorioContrato.ObtenerContrato(id);
            if (entidad == null) return RedirectToAction(nameof(Index));
            return View(entidad);
        }

        // GET: ContratoController/Create
        public ActionResult Crear()
        {
            ViewBag.Inquilino = repositorioInquilino.Obtener();
            ViewBag.Inmueble = repositorioInmueble.ObtenerTodos();
            return View();
        }

        // POST: ContratoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Contrato contrato)
        {
            try
            {
                repositorioContrato.Alta(contrato);

                var res = contrato.Id;
                TempData["Id"] = res;
                TempData["Mensaje"] = $"Contrato creado con éxito! Id: {res}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                ViewBag.StackTrate = e.StackTrace;
                return View(contrato);
            }
        }

        // GET: ContratoController/Edit/5
        public ActionResult Editar(int id)
        {
            var contrato = repositorioContrato.ObtenerContrato(id);
            ViewBag.Inquilinos = repositorioInquilino.Obtener();
            ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
          
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(contrato);
        }

        // POST: ContratoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int id, Contrato entidad)
        {
            try
            {
                entidad.Id = id;
                repositorioContrato.Modificar(entidad);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Propietarios = repositorioInquilino.Obtener();
                ViewBag.Inmueble = repositorioInmueble.ObtenerTodos();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        // GET: ContratoController/Delete/5
        public ActionResult Eliminar(int id)
        {
            var entidad = repositorioContrato.ObtenerContrato(id);
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(entidad);
        }

        // POST: ContratoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id, Contrato entidad)
        {
            try
            {
                repositorioContrato.Baja(id);
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
