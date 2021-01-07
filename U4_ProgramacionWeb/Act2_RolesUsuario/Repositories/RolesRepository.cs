using Act2_RolesUsuario.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Act2_RolesUsuario.Repositories
{
    public class RolesRepository<T> where T : class
    {
        public bd_rolesContext Context { get; set; }

        public RolesRepository(bd_rolesContext context)
        {
            Context = context;
        }
        public virtual IEnumerable<T> GetAll()
        {
            return Context.Set<T>();
        }

        public virtual T GetById(object id)
        {
            return Context.Find<T>(id);
        }

        public virtual void Agregar(T entidad)
        {
            if (Validar(entidad))
            {
                Context.Add(entidad);
                Guardar();
            }
        }

        public virtual void Editar(T entidad)
        {
            if (Validar(entidad))
            {
                Context.Update(entidad);
                Guardar();
            }
        }

        public virtual void Eliminar(T entidad)
        {

            Context.Remove(entidad);
            Guardar();

        }

        public virtual void Guardar()
        {
            Context.SaveChanges();
        }

        public virtual bool Validar(T entidad)
        {
            return true;
        }

    }
}
