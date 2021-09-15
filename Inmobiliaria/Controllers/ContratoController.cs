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
    public class ContratoController : Controller
    {
       RepositorioContrato repositorioContrato;
       RepositorioInquilino repositorioInquilino;
        RepositorioInmueble repositorioInmueble;
        RepositorioGarante repositorioGarante;
        private readonly IConfiguration configuration;

        public ContratoController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioInmueble = new RepositorioInmueble(configuration);
            repositorioInquilino = new RepositorioInquilino(configuration);
            repositorioContrato = new RepositorioContrato(configuration);
            repositorioGarante = new RepositorioGarante(configuration);
        }

        // GET: ContratoController
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                ViewBag.Estados = Contrato.ObtenerEstados();
                List<Contrato> lista = repositorioContrato.ObtenerTodos();
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
        [Authorize]
        public ActionResult PorInmueble(int id)
        {
            List<Contrato> lista = repositorioContrato.ObtenerPorInmueble(id);
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


        [Authorize]
        public ActionResult Renovar(int id)
        {
            try
            {
                Contrato prev = repositorioContrato.ObtenerContrato(id);

                Contrato contrato = new Contrato
                {
                    FechaInicio = prev.FechaCierre,
                    FechaCierre = prev.FechaCierre.AddMonths(24),
                    Precio = prev.Inmueble.Precio
                };
                ViewBag.Inmueble = repositorioInmueble.ObtenerPorId(prev.InmuebleId);
                ViewBag.Inquilino = repositorioInquilino.ObtenerInquilino(prev.InquilinoId);
                
                return View(contrato);

            }
            catch (Exception ex)
            {
                
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(nameof(Index));
            }
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Renovar(int id, Contrato contrato)
        {
           try
            {
                if (contrato.FechaInicio < contrato.FechaCierre)
                {

                    contrato.Id = id;
                    repositorioContrato.Renovar(contrato);
                    var res = contrato.Id;
                    TempData["Mensaje"] = "Datos guardados correctamente";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Error, no se puede renovar el contrato con esa fecha de cierre";
                    return RedirectToAction(nameof(Index));
                }
                }
                catch (Exception e)
                {
                    ViewBag.Error = e.Message;
                    ViewBag.StackTrate = e.StackTrace;
                    return View(contrato);
                }

         }


        // GET: ContratoController/Crear
        [Authorize]
        public ActionResult Crear()
        {
            ViewBag.Estados = Contrato.ObtenerEstados();
            ViewBag.Inquilino = repositorioInquilino.Obtener();
            ViewBag.Inmueble = repositorioInmueble.ObtenerTodos();
            ViewBag.Garante = repositorioGarante.Obtener();
            ViewBag.FechaInicio = DateTime.Now;
            ViewBag.FechaCierre = DateTime.Now.AddMonths(24);
            return View();
        }


        // POST: ContratoController/Crear
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Contrato contrato)
        {
            
                try
                {
                ViewBag.Estados = Contrato.ObtenerEstados();

                if (contrato.FechaInicio < contrato.FechaCierre)
                {
                    repositorioContrato.Alta(contrato);

                    var res = contrato.Id;
                    TempData["Id"] = res;
                    TempData["Mensaje"] = $"Contrato creado con éxito! Id: {res}";
                    return RedirectToAction(nameof(Index));
                } 
                else 
                {
                    TempData["Error"] = $"Fechas inválidas";
                    return RedirectToAction(nameof(Index));
                }
                }
                catch (Exception e)
                {
                    ViewBag.Error = e.Message;
                    ViewBag.StackTrate = e.StackTrace;
                    return View(contrato);
                }
           
        }


        // GET: ContratoController/Edit/5
        [Authorize]
        public ActionResult Editar(int id)
        {
            ViewBag.Estados = Contrato.ObtenerEstados();
            var contrato = repositorioContrato.ObtenerContrato(id);
            ViewBag.Inquilinos = repositorioInquilino.Obtener();
            ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
            ViewBag.Garantes = repositorioGarante.Obtener();
          
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(contrato);
        }

        // POST: ContratoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Editar(int id, Contrato entidad)
        {
            try
            {
                if (entidad.FechaInicio < entidad.FechaCierre)
                {
                    entidad.Id = id;
                    repositorioContrato.Modificar(entidad);
                    TempData["Mensaje"] = "Datos guardados correctamente";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Error, no se puede editar el contrato con esas fechas";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ViewBag.Propietarios = repositorioInquilino.Obtener();
                ViewBag.Inmueble = repositorioInmueble.ObtenerTodos();
                ViewBag.Garantes = repositorioGarante.Obtener();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        // GET: ContratoController/Delete/5
        [Authorize(Policy = "Administrador")]
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
        [Authorize(Policy = "Administrador")]
        public ActionResult Eliminar(int id, Contrato entidad)
        {
            try
            {
                repositorioContrato.Baja(id, entidad.InmuebleId);
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
