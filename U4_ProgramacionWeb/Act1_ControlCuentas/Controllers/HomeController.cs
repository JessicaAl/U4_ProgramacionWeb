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
            if (datos != null && HashHelper.GetHash(user.Contra) == datos.Contra)
            {
                if (datos.Activo == 1)
                {
                    List<Claim> info = new List<Claim>();
                    info.Add(new Claim(ClaimTypes.Name, datos.NomUsuario));
                    info.Add(new Claim(ClaimTypes.Role, "Seguidor"));
                    info.Add(new Claim("Correo", datos.Correo));
                    info.Add(new Claim("Nombre", datos.NomUsuario));

                    var claimsIdentity = new ClaimsIdentity(info, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    if (recordar == true)
                    {
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties { IsPersistent = true });
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
                    if (contra1 == contra2)
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
                        mensaje.Body = text.Replace("{##codigo##}", usuario.Codigo.ToString());
                        mensaje.IsBodyHtml = true;

                        SmtpClient cliente = new SmtpClient("smtp.gmail.com", 587);
                        cliente.EnableSsl = true;
                        cliente.UseDefaultCredentials = false;
                        cliente.Credentials = new NetworkCredential("sistemascomputacionales7g@gmail.com", "sistemas7g");
                        cliente.Send(mensaje);
                        return RedirectToAction("ActivarCuenta");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Las contraseñas no coinciden");
                        return View(usuario);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(usuario);
            }

        }

        [AllowAnonymous]
        public IActionResult ActivarCuenta()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ActivarCuenta(int codigo)
        {
            ctrlusersContext context = new ctrlusersContext();
            UsuarioRepository<Usuario> repos = new UsuarioRepository<Usuario>(context);
            var usuario = context.Usuario.FirstOrDefault(x => x.Codigo == codigo);

            if (usuario != null && usuario.Activo == 0)
            {
                var userCode = usuario.Codigo;
                if (codigo == userCode)
                {
                    usuario.Activo = 1;
                    repos.Editar(usuario);
                    return RedirectToAction("IniciarSesion");
                }
                else
                {
                    ModelState.AddModelError("", "El código ingresado es incorrecto.");
                    return View((object)codigo);
                }
            }
            else
            {
                ModelState.AddModelError("", "Este usuario no existe");
                return View((object)codigo);
            }
        }

        public IActionResult CambiarContra()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CambiarContra(string correo, string contra, string newContra, string newConfirm)
        {
            ctrlusersContext context = new ctrlusersContext();
            UsuarioRepository<Usuario> repos = new UsuarioRepository<Usuario>(context);
            try
            {
                var usuario = repos.GetUsuarioCorreo(correo);
                if (usuario.Contra != HashHelper.GetHash(contra))
                {
                    ModelState.AddModelError("", "La constraseña es incorrecta");
                    return View();
                }
                else
                {
                    if (newContra != newConfirm)
                    {
                        ModelState.AddModelError("", "Las contraseñas no coinciden");
                        return View();
                    }
                    else if (usuario.Contra == HashHelper.GetHash(newContra))
                    {
                        ModelState.AddModelError("", "La contraseña actual no puede ser igual a la nueva");
                        return View();
                    }
                    else
                    {
                        usuario.Contra = HashHelper.GetHash(newContra);
                        repos.Editar(usuario);
                        return RedirectToAction("IniciarSesion");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        public IActionResult RecuperarContra()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous] 
        public IActionResult RecuperarContra(string correo)
        {
            try
            {
                ctrlusersContext context = new ctrlusersContext();
                UsuarioRepository<Usuario> repos = new UsuarioRepository<Usuario>(context);
                var usuario = repos.GetUsuarioCorreo(correo);

                if (usuario!=null)
                {
                    var defaultPass = CodeHelper.GetCode();
                    MailMessage mensaje = new MailMessage();
                    mensaje.From= new MailAddress("sistemascomputacionales7g@gmail.com", "VLIVE");
                    mensaje.To.Add(correo);
                    mensaje.Subject = "Recupera tu contraseña de VLIVE";
                    string text = System.IO.File.ReadAllText(Environment.WebRootPath + "/RecuperarContra.html");
                    mensaje.Body = text.Replace("{##defaultPass##}", defaultPass.ToString() );
                    mensaje.IsBodyHtml = true;

                    SmtpClient cliente = new SmtpClient("smtp.gmail.com", 587);
                    cliente.EnableSsl = true;
                    cliente.UseDefaultCredentials = false;
                    cliente.Credentials = new NetworkCredential("sistemascomputacionales7g@gmail.com", "sistemas7g");
                    cliente.Send(mensaje);
                    usuario.Contra = HashHelper.GetHash(defaultPass.ToString());
                    repos.Editar(usuario);
                    return RedirectToAction("IniciarSesion");
                }
                else
                {
                    ModelState.AddModelError("", "Este correo electrónico con está registrado");
                    return View();
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View((object)correo);
            }
        }

        public IActionResult EliminarCuenta()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EliminarCuenta(string correo, string contra)
        {
            try
            {
                ctrlusersContext context = new ctrlusersContext();
                UsuarioRepository<Usuario> repos = new UsuarioRepository<Usuario>(context);
                var usuario = repos.GetUsuarioCorreo(correo);
                if (usuario != null)
                {
                    if (HashHelper.GetHash(contra) == usuario.Contra)
                    {
                        repos.Eliminar(usuario);
                    }
                    else
                    {
                        ModelState.AddModelError("", "La contraseña es incorrecta");
                        return View();
                    }
                }
                return RedirectToAction("IniciarSesion");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Error. Intente de nuevo");
                return View();
            }
        }
    }
}