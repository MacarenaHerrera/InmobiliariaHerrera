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
    public class ContratoController : Controller
    {
        private readonly RepositorioContrato repositorioContrato;
        private readonly IConfiguration configuration;

        public ContratoController(IConfiguration configuration)
        {
            repositorioContrato = new RepositorioContrato(configuration);
            this.configuration = configuration;
        }

        // GET: ContratoController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ContratoController/Details/5
        public ActionResult Details(int id)
        {
            var lista = repositorioContrato.ObtenerTodos();
            return View(lista);
        }

        // GET: ContratoController/Create
        public ActionResult Crear()
        {
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
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ContratoController/Edit/5
        public ActionResult Editar(int id)
        {
            var contrato = repositorioContrato.ObtenerContrato(id);
            return View();
        }

        // POST: ContratoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int id, IFormCollection collection)
        {
            try
            {
                var contrato = repositorioContrato.ObtenerContrato(id);
                repositorioContrato.Modificar(contrato);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ContratoController/Delete/5
        public ActionResult Eliminar(int id)
        {
            repositorioContrato.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: ContratoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id, IFormCollection collection)
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
