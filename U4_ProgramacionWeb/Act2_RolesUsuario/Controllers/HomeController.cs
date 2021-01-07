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

        [AllowAnonymous]
        public IActionResult InicioDirector()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> InicioDirector(Director dire)
        {
            bd_rolesContext context = new bd_rolesContext();
            RolesRepository<Director> repos = new RolesRepository<Director>(context);
            var director = context.Director.FirstOrDefault(x => x.NumControl == dire.NumControl);

            try
            {
                if (director != null && director.DireContra == HashHelper.GetHash(dire.DireContra))
                {

                    List<Claim> info = new List<Claim>();
                    info.Add(new Claim(ClaimTypes.Name, "Director" + director.Nombre));
                    info.Add(new Claim(ClaimTypes.Role, "Director"));
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
        public IActionResult InicioMaestro()
        {
            return View();
        }

        [AllowAnonymous][HttpPost]
        public async Task<IActionResult> InicioMaestro(Maestro m)
        {
            bd_rolesContext context = new bd_rolesContext();
            MaestroRepository repos = new MaestroRepository(context);
            var maestro = repos.GetMaestroByNoCtrl(m.NumControl.ToString());
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

        [Authorize(Roles = "Director")]
        public IActionResult ListaMaestros()
        {
            bd_rolesContext context = new bd_rolesContext();
            MaestroRepository repos = new MaestroRepository(context);
            var maestros = repos.GetAll();

            return View(maestros);
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
                var m = repos.GetMaestroByNoCtrl(mas.NumControl.ToString());
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

        [Authorize(Roles = "Director")]
        public IActionResult EditarMaestro(int id)
        {
            bd_rolesContext context = new bd_rolesContext();
            MaestroRepository repos = new MaestroRepository(context);
            var maestro = repos.GetById(id);

            if (maestro == null)
            {
                return RedirectToAction("StatusMaestro");
            }

            return View(maestro);
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
            MaestroRepository repos = new MaestroRepository(context);
            var maestro = repos.GetById(m.IdMaestro);
            try
            {
                if (maestro != null)
                {
                    if (nvaCon == confirmPass)
                    {
                        maestro.MaesContra = confirmPass;
                        maestro.MaesContra = HashHelper.GetHash(nvaCon);
                        repos.Editar(maestro);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Las contraseñas no coinciden");
                        return View(maestro);
                    }

                }
                return RedirectToAction("ListaMaestros");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(maestro);
            }
        }

        [HttpPost]
        public IActionResult DesactivarMaestro(Maestro m)
        {
            bd_rolesContext context = new bd_rolesContext();
            MaestroRepository repos = new MaestroRepository(context);
            var desactivar = repos.GetById(m.IdMaestro);

            if (desactivar != null && desactivar.Activo == 1)
            {
                desactivar.Activo = 0;
                repos.Editar(desactivar);
            }
            else
            {
                desactivar.Activo = 1;
                repos.Editar(desactivar);
            }
            return RedirectToAction("ListaMaestros");
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
                    if (User.Claims.FirstOrDefault(x=>x.Type=="Id").Value==alumnovm.Maestro.IdMaestro.ToString())
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

        [Authorize(Roles = "Maestro, Director")]
        [HttpPost]
        public IActionResult AgregarAlumno(AlumnoViewModel avm)
        {
            bd_rolesContext contexto = new bd_rolesContext();
            MaestroRepository mrepos = new MaestroRepository(contexto);
            AlumnosRepository arepos = new AlumnosRepository(contexto);
            try
            {
                var idMaes = mrepos.GetMaestroByNoCtrl(avm.Maestro.NumControl.ToString()).IdMaestro;
                avm.Alumno.MaesId = idMaes;
                arepos.Agregar(avm.Alumno);
                return RedirectToAction("ListaAlumnos", new { id = idMaes });
            }
            catch (Exception ex)
            {
                avm.Maestro = mrepos.GetById(avm.Maestro.IdMaestro);
                avm.Maestros = mrepos.GetAll();
                ModelState.AddModelError("", ex.Message);
                return View(avm);
            }
        }
      
        [Authorize(Roles = "Maestro, Director")]
        public IActionResult ListaAlumnos(int id)
        {
            bd_rolesContext context = new bd_rolesContext();
            MaestroRepository repos = new MaestroRepository(context);
            var maestro = repos.GetAlumnosByMaes(id);

            if (maestro != null)
            {
                if (User.IsInRole("Maestro"))
                {
                    if (User.Claims.FirstOrDefault(x => x.Type == "Id").Value == maestro.IdMaestro.ToString())
                    {
                        return View(maestro);
                    }
                    else
                    {
                        return RedirectToAction("Denegado");
                    }
                }
                else if (maestro.Activo != 1)
                    return RedirectToAction("ListaMaestros");
                else
                    return View(maestro);
            }
            else
                return RedirectToAction("ListaMaestros");
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

            var ae = repos.GetById(alumno.IdAlumno);
            if (ae != null)
            {
                repos.Eliminar(ae);
            }
            else
            {
                ModelState.AddModelError("", "El alumno a eliminar no se encuentra");
            }
            return RedirectToAction("ListaAlumnos", new { id = ae.MaesId });
        }


        [AllowAnonymous]
        public IActionResult Denegado()
        {
            return View();
        }
    }
}
