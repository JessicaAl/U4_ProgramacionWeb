using Act2_RolesUsuario.Helpers;
using Act2_RolesUsuario.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using static Act2_RolesUsuario.Repositories.RolesRepository<T>;
using Act2_RolesUsuario.Repositories;
using System.Linq;
using Act2_RolesUsuario.Models.ViewModels;

namespace Act2_RolesUsuario.Controllers
{
    public class HomeController : Controller
    {
        [Authorize(Roles ="Maestro, Director")]
        public IActionResult Index(int numControl)
        {
            return View();
        }

        [AllowAnonymous][HttpPost]
        public async Task<IActionResult> InicioMaestro(Maestro m)
        {
            bd_rolesContext context = new bd_rolesContext();
            MaestroRepository repos = new MaestroRepository(context);
            var maestro = repos.GetMaestroByNoCtrl(m.NumControl);
            try
            {
                if (maestro!= null && maestro.MaesContra==HashHelper.GetHash(m.MaesContra))
                {
                    if (maestro.Activo==1)
                    {
                        List<Claim> info = new List<Claim>();
                        info.Add(new Claim(ClaimTypes.Name, "Docente" + maestro.Nombre));
                        info.Add(new Claim(ClaimTypes.Role, "Maestro"));
                        info.Add(new Claim("NumControl", maestro.NumControl.ToString()));
                        info.Add(new Claim("Nombre", maestro.Nombre));
                        info.Add(new Claim("Id", maestro.IdMaestro.ToString()));

                        var claimIdentity = new ClaimsIdentity(info, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimPrincipal = new ClaimsPrincipal(claimIdentity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal, new AuthenticationProperties { IsPersistent = true });
                        return RedirectToAction("Index", maestro.NumControl);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Su cuenta está inactiva");
                        return View(m);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "El número de control o la contraseña del docente son incorrectas");
                    return View(m);
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(m);
            }
        }

        [AllowAnonymous]
        public IActionResult InicioDirector()
        {
            return View();
        }

        [AllowAnonymous] [HttpPost]
        public async Task<IActionResult> InicioDirector(Director dire)
        {
            bd_rolesContext context = new bd_rolesContext();
            RolesRepository<Director> repos = new RolesRepository<Director>(context);
            var director = context.Director.FirstOrDefault(x => x.NumControl == dire.NumControl);

            try
            {
                if (dire != null && dire.DireContra == HashHelper.GetHash(director.DireContra))
                {

                        List<Claim> info = new List<Claim>();
                        info.Add(new Claim(ClaimTypes.Name, "Docente" + dire.Nombre));
                        info.Add(new Claim(ClaimTypes.Role, "Maestro"));
                        info.Add(new Claim("NumControl", director.NumControl.ToString()));
                        info.Add(new Claim("Nombre", director.Nombre));
                        info.Add(new Claim("Id", director.IdDire.ToString()));

                        var claimIdentity = new ClaimsIdentity(info, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimPrincipal = new ClaimsPrincipal(claimIdentity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal, new AuthenticationProperties { IsPersistent = true });
                        return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "El número de control o la contraseña del director son incorrectas");
                    return View(dire);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dire);
            }

        }

        [AllowAnonymous]
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }

        [Authorize(Roles ="Director")]
        public IActionResult AceptarMaestro()
        {
            return View();
        }

        [Authorize(Roles = "Director")] [HttpPost]
        public IActionResult AceptarMaestro(Maestro mas)
        {
            bd_rolesContext context = new bd_rolesContext();
            MaestroRepository repos = new MaestroRepository(context);
            try
            {
                var m = repos.GetMaestroByNoCtrl(mas.NumControl);
                if (m==null)
                {
                    mas.Activo = 1;
                    mas.MaesContra = HashHelper.GetHash(mas.MaesContra);
                    repos.Agregar(mas);
                    return RedirectToAction("ListaMaestros");
                }
                else
                {
                    ModelState.AddModelError("", "El número de control ingresado es inválido");
                    return View(mas);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(mas);
            }
        }

        [Authorize(Roles = "Director")]
        public IActionResult StatusMaestro(Maestro m)
        {
            bd_rolesContext context = new bd_rolesContext();
            MaestroRepository repos = new MaestroRepository(context);
            var maestro = repos.GetById(m.IdMaestro);
            if (maestro!=null&& maestro.Activo==0)
            {
                maestro.Activo = 1;
                repos.Editar(maestro);
            }
            else
            {
                maestro.Activo = 0;
                repos.Editar(maestro);
            }
            return RedirectToAction("ListaMaestros");
        }

        [Authorize(Roles ="Director")][HttpPost]
        public IActionResult EditarMaestro(Maestro m)
        {
            bd_rolesContext context = new bd_rolesContext();
            MaestroRepository repos = new MaestroRepository(context);
            var maestro = repos.GetById(m.IdMaestro);
            try
            {
                if (maestro!=null)
                {
                    maestro.Nombre = m.Nombre;
                    repos.Editar(maestro);
                }
                return RedirectToAction("ListaMaestros");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(maestro);
            }
        }

        [Authorize(Roles = "Director")]
        public IActionResult CambiarContraMaestro(int id)
        {
            bd_rolesContext context = new bd_rolesContext();
            MaestroRepository repos = new MaestroRepository(context);
            var maestro = repos.GetById(id);
            if (maestro==null)
            {
                return RedirectToAction("ListaMaestros");
            }
            return View(maestro);
        }

        [Authorize(Roles = "Director")] [HttpPost]
        public IActionResult CambiarContraMaestro(Maestro m, string nvaCon, string confirmPass)
        {
            bd_rolesContext context = new bd_rolesContext();
            MaestroRepository repos =new MaestroRepository(context);
            var maestro = repos.GetById(m.IdMaestro);
            try
            {
                if (maestro!=null)
                {
                    if (nvaCon!=confirmPass)
                    {
                        ModelState.AddModelError("", "Las constraseñas no coinciden");
                        return View(maestro);
                    }
                    else if (maestro.MaesContra==HashHelper.GetHash(nvaCon))
                    {
                        ModelState.AddModelError("", "La nueva contraseña no puede ser la misma que la actual");
                        return View(maestro);
                    }
                    else
                    {
                        maestro.MaesContra = HashHelper.GetHash(nvaCon);
                        repos.Editar(maestro);
                        return RedirectToAction("ListaMaestros");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "El docente a modificar no existe");
                    return View(maestro);
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(maestro);
            }
        }

        [Authorize(Roles = "Maestro, Director")]
        public IActionResult AgregarAlumno(int id)
        {
            bd_rolesContext contexto = new bd_rolesContext();
            MaestroRepository repos = new MaestroRepository(contexto);
            AlumnoViewModel alumnovm = new AlumnoViewModel();
            alumnovm.Maestro = repos.GetById(id);
            if (alumnovm.Maestro!=null)
            {
                if (User.IsInRole("Maestro"))
                {
                    if (User.Claims.FirstOrDefault(x=>x.Type=="IdMaestro").Value==alumnovm.Maestro.IdMaestro.ToString())
                    {
                        return View(alumnovm);
                    }
                    else
                    {
                        return RedirectToAction("Denegado");
                    }
                }
                else
                {
                    return View(alumnovm);
                }
            }
            return View(alumnovm);
        }

        [Authorize(Roles = "Maestro, Director")][HttpPost]
        public IActionResult AgregarAlumno(AlumnoViewModel avm)
        {
            bd_rolesContext contexto = new bd_rolesContext();
            MaestroRepository mrepos = new MaestroRepository(contexto);
            AlumnosRepository arepos = new AlumnosRepository(contexto);
            try
            {
                if (contexto.Alumno.Any(x=>x.NoControl==avm.Alumno.NoControl))
                {
                    ModelState.AddModelError("", "Este número de control ya está registrado");
                    return View(avm);

                }
                else
                {
                    var maestro = mrepos.GetMaestroByNoCtrl(avm.Maestro.NumControl).IdMaestro;
                    avm.Alumno.MaesId = maestro;
                    arepos.Agregar(avm.Alumno);
                    return RedirectToAction("ListaAlumnos", new { id = maestro });
                }
            }
            catch(Exception ex)
            {
                avm.Maestro = mrepos.GetById(avm.Maestro.IdMaestro);
                    avm.Maestros = mrepos.GetAll();
                ModelState.AddModelError("", ex.Message);
                return View(avm);
            }
        }

        [Authorize(Roles = "Maestro, Director")]
        public IActionResult EditarAlumno(int id)
        {
            bd_rolesContext context = new bd_rolesContext();
            MaestroRepository mrepos = new MaestroRepository(context);
            AlumnosRepository arepos = new AlumnosRepository(context);
            AlumnoViewModel avm = new AlumnoViewModel();
            avm.Alumno = arepos.GetById(id);
            avm.Maestros = mrepos.GetAll();
            if (avm.Alumno != null)
            {
                avm.Maestro = mrepos.GetById(avm.Alumno.MaesId);
                if (User.IsInRole("Maestro"))
                {
                    avm.Maestro = mrepos.GetById(avm.Alumno.MaesId);
                    if (User.Claims.FirstOrDefault(x => x.Type == "NumControl").Value == avm.Maestro.NumControl.ToString())
                    {
                        return View(avm);
                    }
                }
                return View(avm);
            }
            else return RedirectToAction("Index");
        }


        [Authorize(Roles = "Maestro, Director")] [HttpPost]
        public IActionResult EditarAlumno(AlumnoViewModel avm)
        {
            bd_rolesContext context = new bd_rolesContext();
            MaestroRepository mrepos = new MaestroRepository(context);
            AlumnosRepository arepos = new AlumnosRepository(context);
            try
            {
                var alumno = arepos.GetById(avm.Alumno.IdAlumno);
                if (alumno!=null)
                {
                    alumno.Nombre = avm.Alumno.Nombre;
                    arepos.Editar(alumno);
                    return RedirectToAction("ListaAlumnos", new { id = alumno.MaesId });
                }
                else
                {
                    ModelState.AddModelError("", "El alumno seleccionado no existe");
                    avm.Maestro = mrepos.GetById(avm.Alumno.MaesId);
                    avm.Maestros = mrepos.GetAll();
                    return View(avm);
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                avm.Maestro = mrepos.GetById(avm.Alumno.MaesId);
                avm.Maestros = mrepos.GetAll();
                return View(avm);
            }
        }

        [Authorize(Roles = "Maestro, Director")] [HttpPost]
        public IActionResult EliminarAlumno(Alumno alumno)
        {
            bd_rolesContext context = new bd_rolesContext();
            AlumnosRepository repos = new AlumnosRepository(context);
            var a = repos.GetById(alumno.IdAlumno);
            if (a!=null)
            {
                repos.Eliminar(a);
            }
            else
            {
                ModelState.AddModelError("", "El alumno seleccionado no existe");
            }
            return RedirectToAction("ListaAlumnos", new { id = a.MaesId });
        }


        [AllowAnonymous]
        public IActionResult Denegado()
        {
            return View();
        }
    }
}
