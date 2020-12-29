using Act1_ControlCuentas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Act1_ControlCuentas.Repositories
{
    public class UsuarioRepository<T> where T: class
    {
        public ctrlusersContext Context { get; set; }
        public UsuarioRepository(ctrlusersContext contexto)
        {
            Context = contexto;
        }

        public Usuario GetUsuarioId(int id)
        {
            return Context.Usuario.FirstOrDefault(x => x.Id==id);
        }

        public Usuario GetUsuarioCorreo(string correo)
        {
            return Context.Usuario.FirstOrDefault(x => x.Correo == correo);
        }

        public Usuario GetUsuario(Usuario id)
        {
            return Context.Find<Usuario>(id);
        }

        public bool Validar(Usuario usuario)
        {
            if (string.IsNullOrEmpty(usuario.NomUsuario))
                throw new Exception("Escriba un nombre de usuario");
            if (string.IsNullOrEmpty(usuario.Correo))
                throw new Exception("Escriba un correo electrónico");
            if (string.IsNullOrEmpty(usuario.Contra))
                throw new Exception("Ingrese una contraseña");
            return true;
        }

        public virtual void Agregar(Usuario usuario)
        {
            if (Validar(usuario)==true)
            {
                Context.Add(usuario);
                Context.SaveChanges();
            }
        }

        public virtual void Eliminar(Usuario usuario)
        {
            Context.Remove<Usuario>(usuario);
            Context.SaveChanges();
        }

        public virtual void Editar(Usuario usuario)
        {
            if (Validar(usuario))
            {
                Context.Update<Usuario>(usuario);
                Context.SaveChanges();
            }
        }
    }
}
