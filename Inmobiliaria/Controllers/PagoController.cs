using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Inmobiliaria.Data;
using Inmobiliaria.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace Inmobiliaria.Controllers
{
    public class PagoController : Controller
    {
        RepositorioContrato repositorioContrato;
        RepositorioPago repositorioPago;
        private readonly IConfiguration config;

        public PagoController(IConfiguration config )
        {
            this.config = config;
            repositorioContrato = new RepositorioContrato(config);
            repositorioPago = new RepositorioPago(config);
        }

        // GET: Pago
        public ActionResult Index()
        {
            try
            {
                var lista = repositorioPago.ObtenerTodos();
                ViewData[nameof(Pago)] = lista;
                ViewData["Tittle"] = nameof(Pago);
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

        // GET: InmuebleController/PorPropietario/5
        
        public ActionResult PorContrato(int id)
        {
            if (id == 0)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                IList<Pago> lista = repositorioPago.ObtenerPorContrato(id);
                Contrato c = repositorioContrato.ObtenerContrato(id);
                if (c == null) return RedirectToAction(nameof(Index));
                ViewBag.Contrato = c;
                return View("Index", lista);
            }
        }

        // GET: Pago/Details/5
        public ActionResult Details(int id)
        {

            var entidad = repositorioPago.ObtenerPorId(id);
            if (entidad == null) return RedirectToAction(nameof(Index));
            return View(entidad);

        }

        // GET: Pago/Create
        public ActionResult Crear()
        {
           try {
                ViewBag.Contrato = repositorioContrato.ObtenerTodos();
                return View();
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                ViewBag.StackTrate = e.StackTrace;
                return View();
            }

        }

        // POST: Pago/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Pago entidad)
        {
            try
            {
                repositorioPago.Alta(entidad);

                var res = entidad.Id;
                TempData["Id"] = res;
                TempData["Mensaje"] = $"Pago realizado con éxito! Id: {res}";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                ViewBag.StackTrate = e.StackTrace;
                return View(entidad);
            }
        }

        // GET: Pago/Edit/5
        public ActionResult Editar(int id)
        {

            var entidad = repositorioPago.ObtenerPorId(id);
            ViewBag.Contratos = repositorioContrato.ObtenerTodos();

            //if (entidad == null) return RedirectToAction(nameof(Index));
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(entidad);

        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int id, Pago entidad)
        {
            try
            {
                entidad.Id = id;
                repositorioPago.Modificar(entidad);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Contratos = repositorioContrato.ObtenerTodos();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        // GET: Pago/Delete/5
        public ActionResult Eliminar(int id)
        {

            var entidad = repositorioPago.ObtenerPorId(id);
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            //if (entidad == null) return RedirectToAction(nameof(Index));
            return View(entidad);

        }

        // POST: Pago/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id, Pago entidad)
        {
            try
            {
                repositorioPago.Baja(id);
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
