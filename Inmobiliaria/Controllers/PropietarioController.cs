
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Controllers
{
    public class PropietarioController : Controller
    {
        RepositorioPropietario repositorioPropietario;
        private readonly IConfiguration config;

        public PropietarioController(IConfiguration config)
        {
            this.config = config;
            repositorioPropietario = new RepositorioPropietario(config);
        }
        // GET: PropietarioController
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                var lista = repositorioPropietario.Obtener();
                ViewData[nameof(Propietario)] = lista;
                ViewData["Tittle"] = nameof(Propietario);
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

        // GET: PropietarioController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PropietarioController/Create
        [Authorize]
        public ActionResult Crear()
        {

            return View();
        }

        // POST: PropietarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Crear(Propietario propietario)
        {
            try
            {
                repositorioPropietario.Alta(propietario);
                TempData["Id"] = propietario.Id;
                return RedirectToAction(nameof(Index)); 
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                ViewBag.StackTrate = e.StackTrace;
                return View(nameof(Index));
            }
        }

        // GET: PropietarioController/Edit/5
        [Authorize]
        public ActionResult Editar(int id)
        {
            var prop = repositorioPropietario.obtenerPropietario(id);
            return View(prop);
        }

        // POST: PropietarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Editar(int id, IFormCollection collection)
        {
            try
            {
                Propietario prop = repositorioPropietario.obtenerPropietario(id);
                prop.Nombre = collection["Nombre"];
                prop.Apellido = collection["Apellido"];
                prop.Dni = collection["Dni"];
                prop.Email = collection["Email"];
                prop.Telefono = collection["Telefono"];

                repositorioPropietario.Modificar(prop);

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

        // GET: PropietarioController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Eliminar(int id)
        {
            try
            {
                var entidad = repositorioPropietario.obtenerPropietario(id);
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                if (TempData.ContainsKey("Error"))
                    ViewBag.Error = TempData["Error"];
                return View(entidad);
            }
            catch (Exception)
            {

                throw;
            }
        }

        // POST: PropietarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Eliminar(int id, Propietario entidad)
        {
            try 
            {
                repositorioPropietario.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
           
        }
    }
}
