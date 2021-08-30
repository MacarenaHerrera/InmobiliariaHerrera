﻿using Inmobiliaria.Models;
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

        public ActionResult PorPropietario(int id)
        {
            try
            {
                Propietario ent = repoPropietario.obtenerPropietario(id);
                if (ent == null) return RedirectToAction(nameof(Index));
                ViewBag.Propietario = ent;
                IList<Inmueble> lista = repositorioInmueble.BuscarPorPropietario(id);
                return View("Index", lista);
            }
            catch (Exception)
            {

                throw;
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
        public ActionResult Crear()
        {
            ViewBag.Propietario = repoPropietario.Obtener();
            return View();
           
        }

        // POST: Inmueble/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
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


