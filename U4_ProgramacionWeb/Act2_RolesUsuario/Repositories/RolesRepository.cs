using Act2_RolesUsuario.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Act2_RolesUsuario.Repositories
{
    public class RolesRepository<T> where T: class
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
        public T GetById(object id)
        {
            return Context.Find<T>(id);
        }
        public virtual bool Validar(T dato)
        {
            return true;
        }
        public virtual void Agregar(T dato)
        {
            if (Validar(dato))
            {
                Context.Add(dato);
                Context.SaveChanges();
            }
        }
        public virtual void Editar(T dato)
        {
            if (Validar(dato))
            {
                Context.Update<T>(dato);
                Context.SaveChanges();
            }
        }
        public virtual void Eliminar(T dato)
        {
            Context.Remove<T>(dato);
            Context.SaveChanges();
        }

        public class MaestroRepository : RolesRepository<Maestro>
        {
            public MaestroRepository(bd_rolesContext context) : base(context) { }
            public Maestro GetMaestroByNoCtrl(int numc)
            {
                return Context.Maestro.FirstOrDefault(x => x.NumControl == numc);
            }
            public Maestro GetAlumnoByMaestro(int maes)
            {
                return Context.Maestro.Include(x => x.Alumno).FirstOrDefault(x => x.IdMaestro == maes);
            }

            public override bool Validar(Maestro dato)
            {
                if (dato.NumControl <= 0)
                    throw new Exception("Escriba el número de control del maestro");
                if (dato.NumControl == 2908)
                    throw new Exception("El número de control ingresado no es válido");
                if (string.IsNullOrEmpty(dato.Nombre))
                    throw new Exception("Escriba el nombre del docente");
                if (string.IsNullOrWhiteSpace(dato.MaesContra))
                    throw new Exception("Escriba la contraseña del docente");
                if (dato.NumControl.ToString().Length < 4)
                    throw new Exception("El número de control debe ser de 4 números");
                if (dato.NumControl.ToString().Length > 4)
                    throw new Exception("El número de control no debe de exceder los 4 dígitos");
                return true;
            }
        }
        public class AlumnosRepository : RolesRepository<Alumno>
        {
            public AlumnosRepository(bd_rolesContext context) : base(context) { }
            public Alumno GetAlumnoByNoCtrl(string num)
            {
                return Context.Alumno.FirstOrDefault(x => x.NoControl.ToLower() == num.ToLower());
            }
            public override bool Validar(Alumno dato)
            {
                if (string.IsNullOrEmpty(dato.NoControl))
                    throw new Exception("Escriba el número de control del alumno");
                if (string.IsNullOrEmpty(dato.Nombre))
                    throw new Exception("Escriba el nombre del alumno");
                if (dato.MaesId.ToString()==null|| dato.MaesId<=0)
                    throw new Exception("El alumno no tiene un maestro designado");
                if (dato.NoControl == "2908")
                    throw new Exception("Este número de control es inválido");
                if (dato.NoControl.ToString().Length < 8)
                    throw new Exception("El número de control debe ser de 8 números");
                if (dato.NoControl.ToString().Length > 8)
                    throw new Exception("El número de control no debe de exceder los 8 dígitos");
                return true;
            }
        }
    }
}
