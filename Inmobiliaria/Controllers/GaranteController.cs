using Inmobiliaria.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
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
        public ActionResult Crear(Garante garante)
        {
            try
            {
                repositorioGarante.Alta(garante);
                return RedirectToAction("Crear", "Contrato");

            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return RedirectToAction(nameof(Crear));
            }
        }

        // GET: Garante/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Garante/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Garante/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Garante/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
