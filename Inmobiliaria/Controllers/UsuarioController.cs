
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Inmobiliaria.Data;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace Inmobiliaria.Controllers
{
    public class UsuarioController : Controller
    {
        RepositorioUsuario repositorio;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;

        public UsuarioController(RepositorioUsuario repositorio, IWebHostEnvironment environment, IConfiguration configuration)
        {
            this.repositorio = repositorio;
            this.environment = environment;
            this.configuration = configuration;
        }

        // GET: UsuarioController
        [Authorize(Policy = "Administrador")]
        public ActionResult Index()
        {
            IList<Usuario> lista = repositorio.ObtenerTodos();
            return View(lista);
        }

        // GET: UsuarioController/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Crear()
        {
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View();
        }

        // POST: UsuarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Crear(Usuario ent)
        {
            if (!ModelState.IsValid)
                return View();
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: ent.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                ent.Clave = hashed;
                ent.Avatar = "/uploads/avatarDefault.png";

                int res = repositorio.Alta(ent);
                if (ent.AvatarFile != null && ent.Id > 0)
                {
                    string path = Path.Combine(environment.WebRootPath, "uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string fileName = "avatar" + ent.Id + Path.GetExtension(ent.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        ent.AvatarFile.CopyTo(stream);
                    }
                    ent.Avatar = Path.Combine("/uploads", fileName);

                    repositorio.Alta(ent);
                }
                TempData["Mensaje"] = $"Usuario creado con éxito! Id: {res}";
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException e)
            {
                TempData["Error"] = e.Number + " " + e.Message;
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View();
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocurrió un error inesperado.";
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View();
            }
        }

        // GET: UsuarioController/Edit/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Editar(int id)
        {
            var ent = repositorio.Obtener(id);
            if (ent == null) return RedirectToAction(nameof(Index));
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View(ent);
        }

        // GET: UsuarioController/EditarPerfil
        [Authorize(Policy = "Administrador")]
        public ActionResult EditarPerfil()
        {
            var ent = repositorio.ObtenerPorEmail(User.Identity.Name);
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View("Editar", ent);
        }

        // POST: UsuarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Editar(int id, Usuario ent)
        {
            try
            {
                
                    var usuarioActual = repositorio.ObtenerPorEmail(User.Identity.Name);
                    if (usuarioActual.Id != id)//si no es admin, solo puede modificarse él mismo
                        return RedirectToAction(nameof(Index), "Home");
                
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: ent.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                ent.Clave = hashed;
                ent.Rol = User.IsInRole("Administrador") ? ent.Rol : (int)rol.Empleado;
                var entOriginal = repositorio.Obtener(id);
                ent.Avatar = entOriginal.Avatar;
                if (ent.AvatarFile != null)
                {
                    string path = Path.Combine(environment.WebRootPath, "uploads");
                    if (Path.GetFileName(ent.Avatar) != "avatarDefault.png")
                    {
                        if (FileSystem.FileExists(Path.Combine(path, Path.GetFileName(ent.Avatar))))
                            FileSystem.DeleteFile(Path.Combine(path, Path.GetFileName(ent.Avatar)));
                    }
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string fileName = "avatar" + ent.Id + Path.GetExtension(ent.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        ent.AvatarFile.CopyTo(stream);
                    }
                    ent.Avatar = Path.Combine("/uploads", fileName);
                }

                repositorio.Modificacion(ent);

                if (!User.IsInRole("Administrador"))
                {
                    return RedirectToAction(nameof(Perfil));
                }
                else
                {
                    TempData["Mensaje"] = "Usuario modificado con éxito!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (SqlException e)
            {
                TempData["Error"] = e.Number + " " + e.Message;
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View(ent);
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocurrió un error inesperado.";
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View(ent);
            }
        }

        // GET: UsuarioController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Baja(int id)
        {
            var ent = repositorio.Obtener(id);
            if (ent == null) return RedirectToAction(nameof(Index));
            return View(ent);
        }

        // POST: UsuarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Baja(int id, Usuario ent)
        {
            try
            {
               string filename = Path.GetFileName((string) TempData["AvatarPath"]);
                repositorio.Baja(id);
                if (filename != "avatarDefault.png")
                {
                    if (FileSystem.FileExists(Path.Combine(environment.WebRootPath, "uploads", filename)))
                        FileSystem.DeleteFile(Path.Combine(environment.WebRootPath, "uploads", filename));
                }
                TempData["Mensaje"] = "Usuario eliminado con éxito!";
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException e)
            {
                if (e.Number == 547)
                {
                    TempData["Error"] = "No se pudo eliminar, está en uso.";
                }
                return RedirectToAction(nameof(Index)); ;
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocurrió un error inesperado.";
                return RedirectToAction(nameof(Index)); ;
            }
        }

       
        // GET: UsuarioController/Login/
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginView login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: login.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));

                    var ent = repositorio.ObtenerPorEmail(login.Usuario);
                    if (ent == null || ent.Clave != hashed)
                    {
                        ModelState.AddModelError("", "El email o la clave no son correctos");
                        return View();
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, ent.Email),
                        new Claim("FullName", ent.Nombre + " " + ent.Apellido),
                        new Claim(ClaimTypes.Role, ent.RolNombre),
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }


        // GET: UsuarioController/Perfil
        [Authorize(Policy = "Administrador")]
        public ActionResult Perfil()
        {
            var ent = repositorio.ObtenerPorEmail(User.Identity.Name);
            return View(ent);
        }

        [Authorize]
        public IActionResult Autenticado()
        {
            return View();
        }

        [Authorize]
        public IActionResult SuperPrivado()
        {
            return View();
        }

        [Authorize]
        
        // GET: Usuarios/Logout/
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
