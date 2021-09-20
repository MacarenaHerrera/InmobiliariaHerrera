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
        [Authorize]
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

        // GET: PorContrato

        [Authorize]
        public ActionResult PorContrato(int id)
        {
            
                try
                { 
                    IList<Pago> lista = repositorioPago.ObtenerPorContrato(id);
                    ViewBag.Contrato = repositorioContrato.ObtenerContrato(id);
                    ViewBag.ContratoId = id;
                    return View("Index", lista);
                }
                catch (Exception ex)
                {

                    Json(new { Error = ex.Message });
                    return RedirectToAction(nameof(Index));
                }
            
        }

        // GET: Pago/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {

            var entidad = repositorioPago.ObtenerPorId(id);
            ViewBag.NumeroPago = entidad.ObtenerPago();
            if (entidad == null) return RedirectToAction(nameof(Index));
            return View(entidad);

        }

        // GET: Pago/Crear
        [Authorize]
        public ActionResult Crear(int id)
        {
           try {
                ViewBag.Contrato = repositorioContrato.ObtenerContrato(id);
                return View();
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                ViewBag.StackTrate = e.StackTrace;
                return View();
            }

        }

        // POST: Pago/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Crear(int id, Pago entidad)
        {
            try
            {
                entidad.Id = id;
                repositorioPago.Alta(entidad);

                TempData["Id"] = id;
                TempData["Mensaje"] = $"Pago realizado con éxito! Id: {id}";

                var list = repositorioPago.ObtenerPorContrato(entidad.ContratoId);
                ViewBag.Contrato = repositorioContrato.ObtenerContrato(id);
                ViewBag.ContratoId = id;
                return View("Index", list);
                
            }
            catch (SqlException e)
            {
                TempData["Error"] = e.Number + " " + e.Message;
                ViewBag.Contratos = repositorioContrato.ObtenerTodos();
                return View();
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                ViewBag.StackTrate = e.StackTrace;
                return View(entidad);
            }
        }

        // GET: Pago/Edit/5
        [Authorize]
        public ActionResult Editar(int id)
        {

            var entidad = repositorioPago.ObtenerPorId(id);
            ViewBag.Pago = entidad;

            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(entidad);

        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Editar(int id, Pago entidad)
        {
            try
            {
                entidad.Id = id;
                repositorioPago.Modificar(entidad);
                TempData["Mensaje"] = "Datos guardados correctamente";

                var list = repositorioPago.ObtenerPorContrato(entidad.ContratoId);
                ViewBag.Contrato = repositorioContrato.ObtenerContrato(entidad.ContratoId);
                ViewBag.ContratoId = entidad.ContratoId;

                return View("Index", list);
            }
            catch (SqlException e)
            {
                TempData["Error"] = e.Number + " " + e.Message;
                ViewBag.Contratos = repositorioContrato.ObtenerTodos();
                return View(entidad);
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
        [Authorize(Policy = "Administrador")]
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
       
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
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
