using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using Act1_ControlCuentas.Helpers;
using Act1_ControlCuentas.Models;
using Act1_ControlCuentas.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Act1_ControlCuentas.Controllers
{
    [Authorize(Roles = "Seguidor")]
    public class HomeController : Controller
    {
        public IWebHostEnvironment Environment { get; set; }
        public HomeController(IWebHostEnvironment env)
        {
            Environment = env;
        }

        public IActionResult Index()
        {
            return View();
        }


        [AllowAnonymous]
        public IActionResult IniciarSesion()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> IniciarSesion(Usuario user, bool recordar)
        {
            ctrlusersContext context = new ctrlusersContext();
            UsuarioRepository<Usuario> repos = new UsuarioRepository<Usuario>(context);
            var datos = repos.GetUsuarioCorreo(user.Correo);
            if (datos!=null && HashHelper.GetHash(user.Contra)==datos.Contra)
            {
                if (datos.Activo==1)
                {
                    List<Claim> info = new List<Claim>();
                    info.Add(new Claim(ClaimTypes.Name, datos.NomUsuario));
                    info.Add(new Claim(ClaimTypes.Role, "Seguidor"));
                    info.Add(new Claim("Correo", datos.Correo));
                    info.Add(new Claim("Nombre", datos.NomUsuario));

                    var claimsIdentity = new ClaimsIdentity(info, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    if (recordar== true)
                    {
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties { IsPersistent=true});
                    }
                    else
                    {
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties { IsPersistent = false });
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Tu cuenta no ha sido activada, dirígete al correo para activarla");
                    return View(user);
                }
            }
            else
            {
                ModelState.AddModelError("", "El nombre de usuario o la contraseña son incorrectos");
                return View(user);
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public IActionResult Denegado()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Registrar(Usuario usuario, string contra1, string contra2)
        {
            ctrlusersContext context = new ctrlusersContext();
            UsuarioRepository<Usuario> repos = new UsuarioRepository<Usuario>(context);
            try
            {
                if (context.Usuario.Any(x => x.Correo == usuario.Correo))
                {
                    ModelState.AddModelError("", "Ya hay un usuario registrado con este correo");
                    return View(usuario);
                }
                else
                {
                    if (contra1==contra2)
                    {
                        usuario.Contra = HashHelper.GetHash(contra1);
                        usuario.Codigo = CodeHelper.GetCode();
                        usuario.Activo = 0;
                        repos.Agregar(usuario);
                        MailMessage mensaje = new MailMessage();
                        mensaje.From = new MailAddress("sistemascomputacionales7g@gmail.com", "VLIVE");
                        mensaje.To.Add(usuario.Correo);
                        mensaje.Subject = "Confirma tu cuenta de VLIVE";
                        string text = System.IO.File.ReadAllText(Environment.WebRootPath + "/ConfirmarCorreo.html");
                        mensaje.Body= text.Replace("{##codigo##", usuario.Codigo.ToString());
                        mensaje.IsBodyHtml = true;

                        SmtpClient cliente = new SmtpClient("smtp.gmail.com", 380);
                        cliente.EnableSsl = true;
                        cliente.UseDefaultCredentials = false;
                        cliente.Credentials = new NetworkCredential("sistemascomputacionales7g@gmail.com", "sistemas7g");
                        cliente.Send(mensaje);
                        return RedirectToAction("Activar");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Las contraseñas no coinciden");
                        return View(usuario);
                    }
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(usuario);
            }

        }

        [AllowAnonymous]
        public IActionResult Activar()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Activar(int codigo)
        {
            return View();
        }
    }
}