using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
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
    public class InmuebleController : Controller
    {
        RepositorioInmueble repositorioInmueble;
        RepositorioPropietario repoPropietario;
        private readonly IConfiguration config;

        public InmuebleController(IConfiguration config)
        {
            this.config = config;
            repositorioInmueble = new RepositorioInmueble(config);
            repoPropietario = new RepositorioPropietario(config);
        }

        // GET: Inmueble
        [Authorize]
        public ActionResult Index()
        {

            try
            {
                var lista = repositorioInmueble.ObtenerTodos();
                ViewData[nameof(Inmueble)] = lista;
                ViewData["Tittle"] = nameof(Inmueble);
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
        public ActionResult Disponibles()
        {
            IList<Inmueble> lista = repositorioInmueble.ObtenerDisponibles();
            return View(lista);
        }

        // GET: InmuebleController
        [HttpPost]
        [Authorize]
        public ActionResult Disponibles(FechasView f)
        {
            try {
                IList<Inmueble> lista = repositorioInmueble.ObtenerDisponiblesEntreFechas(f.FechaInicio,
                                                                                          f.FechaCierre);
                ViewBag.Fechas = f;
                TempData["FechaInicio"] = f.FechaInicio;
                TempData["FechaCierre"] = f.FechaCierre;

                return View(lista);
            }
            catch (Exception ex)
            {

                Json(new { Error = ex.Message });
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize]
        public ActionResult Fechas()
        {
            return View();
        }

        [Authorize]
        public ActionResult PorPropietario(int id)
        {
            try
            {
                var lista = repositorioInmueble.BuscarPorPropietario(id);
                ViewData[nameof(Inmueble)] = lista;
                ViewData["Tittle"] = nameof(Inmueble);
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


        // GET: Inmueble/Details/5
        public ActionResult Details(int id)
        {
           
                var entidad = repositorioInmueble.ObtenerPorId(id);
                if (entidad == null) return RedirectToAction(nameof(Index));
                return View(entidad);

        }

        // GET: Inmueble/Create
        [Authorize]
        public ActionResult Crear()
        {
            ViewBag.Propietario = repoPropietario.Obtener();
            return View();
           
        }

        // POST: Inmueble/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Crear(Inmueble entidad)
        {
            try
            {
                repositorioInmueble.Alta(entidad);

                var res = entidad.Id;
                TempData["Id"] = res;
                TempData["Mensaje"] = $"Inmueble creado con éxito! Id: {res}";
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                ViewBag.StackTrate = e.StackTrace;
                return View(entidad);
            }
        }

        [Authorize]
        // GET: Inmueble/Edit/5
        public ActionResult Editar(int id)
        {

            var entidad = repositorioInmueble.ObtenerPorId(id);
            ViewBag.Propietarios = repoPropietario.Obtener();

            //if (entidad == null) return RedirectToAction(nameof(Index));
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(entidad);
          
            
        }

        // POST: Inmueble/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Editar(int id, Inmueble entidad)
        {
            try
            {
                entidad.Id = id;
                repositorioInmueble.Modificar(entidad);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Propietarios = repoPropietario.Obtener();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        // GET: Inmueble/Eliminar/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Eliminar(int id)
        {
            
                var entidad = repositorioInmueble.ObtenerPorId(id);
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                if (TempData.ContainsKey("Error"))
                    ViewBag.Error = TempData["Error"];
                //if (entidad == null) return RedirectToAction(nameof(Index));
                return View(entidad);
           
        }

        // POST: Inmueble/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Eliminar(int id, Inmueble entidad)
        {
            try
            {
                repositorioInmueble.Baja(id);
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


