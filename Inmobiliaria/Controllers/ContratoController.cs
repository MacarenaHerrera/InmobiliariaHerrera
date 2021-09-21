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
        RepositorioPago repositorioPago;
        private readonly IConfiguration configuration;
       

        public ContratoController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioInmueble = new RepositorioInmueble(configuration);
            repositorioInquilino = new RepositorioInquilino(configuration);
            repositorioContrato = new RepositorioContrato(configuration);
            repositorioGarante = new RepositorioGarante(configuration);
            repositorioPago = new RepositorioPago(configuration);
        }

        // GET: ContratoController
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                //ViewBag.Estados = Contrato.ObtenerEstados();
                List<Contrato> lista = repositorioContrato.ObtenerTodos();
                //ViewData[nameof(Contrato)] = lista;
                //ViewData["Tittle"] = nameof(Contrato);
                //ViewBag.Id = TempData["Id"];
                //if (TempData.ContainsKey("Mensaje"))
                  //  ViewBag.Mensaje = TempData["Mensaje"];
                return View(lista);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(nameof(Index));
            }
        }
        [Authorize]
        public ActionResult PorInmueble(int id)
        {
            try
            {
                List<Contrato> lista = repositorioContrato.ObtenerPorInmueble(id);
                Inmueble ent = repositorioInmueble.ObtenerPorId(id);
                if (ent == null) return RedirectToAction(nameof(Index));
                ViewBag.PorInmueble = ent;
                return View("Index", lista);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(nameof(Index));
            }
        }
        [Authorize]
        public ActionResult Vigentes()
        {
            IList<Contrato> lista = repositorioContrato.ObtenerVigentes();
            ViewBag.Cancelar = true;
            return View(lista);
        }

        // GET: ContratoController/ConfirmarCancelar
        [Authorize]
        public ActionResult Cancelar(int id)
        {
            
            try
            {
                var ent = repositorioContrato.ObtenerContrato(id);

                var numPagos = repositorioPago.ObtenerPorContrato(ent.Id).Count;
                if (numPagos > 0)
                {
                    var nummeses = ent.CalcularMeses();

                    var debe = nummeses - numPagos;

                    ViewBag.Debe = debe;
                }

                if (ent == null) return RedirectToAction(nameof(Index));

                return View(ent);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(nameof(Index));
            }
        }

        // POST: ContratoController/ConfirmarCancelar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Cancelar(int id, Contrato ent)
        {
            
        try 
        {
            repositorioContrato.Cancelar(id);
                
                TempData["Mensaje"] = "Contrato cancelado con éxito!";
                return View(nameof(Vigentes));
            }
            catch (Exception ex)
            {

                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(nameof(Index));
            }
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
            ViewBag.Inquilinos = repositorioInquilino.Obtener();
            ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
            ViewBag.Garantes = repositorioGarante.Obtener();
            ViewBag.FechaInicio = DateTime.Now;
            return View();
        }

        [Authorize]
        public ActionResult CrearPara(int id)
        {
            try
            {
            ViewBag.Inquilinos = repositorioInquilino.Obtener();
            ViewBag.Garantes = repositorioGarante.Obtener();
            ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
            
           //Inmueble inmueble = repositorioInmueble.ObtenerPorId(id);
                ViewBag.InmuebleId = id;

                ViewBag.FechaInicio = TempData.ContainsKey("FechaInicio") ? TempData["FechaInicio"] : DateTime.Now;
            ViewBag.FechaFinal = TempData.ContainsKey("FechaFinal") ? TempData["FechaFinal"] : DateTime.Now.AddMonths(24);

                return View(nameof(Crear));
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                ViewBag.StackTrate = e.StackTrace;
                return View(nameof(Index));
            }
        }

        // POST: ContratoController/Crear
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Contrato ent)
        {
                    
            try
                {
                //ViewBag.Estados = Contrato.ObtenerEstados();
                repositorioContrato.Alta(ent);

                        int res = ent.Id;
                        TempData["Id"] = res;
                        TempData["Mensaje"] = $"Contrato creado con éxito! Id: {res}";
                        return RedirectToAction(nameof(Index));
                    
                }
                catch (Exception e)
                {
                    ViewBag.Error = e.Message;
                    ViewBag.StackTrate = e.StackTrace;
                    return View(ent);
                }
        }


        // GET: ContratoController/Edit/5
        [Authorize]
        public ActionResult Editar(int id)
        {
            //ViewBag.Estados = Contrato.ObtenerEstados();
            try
            {
                var contrato = repositorioContrato.ObtenerContrato(id);
                ViewBag.Contrato = contrato;
                //ViewBag.Inquilinos = repositorioInquilino.Obtener();
                //ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
                ViewBag.Garantes = repositorioGarante.Obtener();

                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                if (TempData.ContainsKey("Error"))
                    ViewBag.Error = TempData["Error"];
                return View(contrato);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(nameof(Index));
            }
        }

        // POST: ContratoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Editar(int id, Contrato ent)
        {
            
            try
            {
                repositorioContrato.Modificar(ent);
                TempData["Mensaje"] = "Contrato modificado con éxito!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Inquilinos = repositorioInquilino.Obtener();
                ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
                ViewBag.Garantes = repositorioGarante.Obtener();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(ent);
            }
        }

        // GET: ContratoController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Eliminar(int id)
        {
            Contrato entidad = repositorioContrato.ObtenerContrato(id);
            ViewBag.Inmueble = repositorioInmueble.ObtenerPorId(entidad.InmuebleId);
            
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
        public ActionResult Eliminar(int id, Contrato c)
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
                return View(c);
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocurrió un error inesperado.";
                return View(c);
            }
        }
    }
}
