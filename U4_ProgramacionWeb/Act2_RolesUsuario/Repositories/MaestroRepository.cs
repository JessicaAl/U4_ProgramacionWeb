using Act2_RolesUsuario.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Act2_RolesUsuario.Repositories
{
    public class MaestroRepository : RolesRepository<Maestro>
    {
        public MaestroRepository(bd_rolesContext ctx) : base(ctx)
        {

        }
        public virtual Maestro GetMaestroByNoCtrl(string num)
        {
            return Context.Maestro.FirstOrDefault(x => x.NumControl.ToString() == num);
        }

        public Maestro GetAlumnosByMaes(int id)
        {
            return Context.Maestro.Include(x => x.Alumno).FirstOrDefault(x => x.IdMaestro == id);
        }
        public override bool Validar(Maestro entidad)
        {
            if (string.IsNullOrEmpty(entidad.Nombre))
                throw new Exception("El nombre no puede estar vacio");
            if (string.IsNullOrWhiteSpace(entidad.MaesContra))
                throw new Exception("La contraseña no puede estar vacía");
            if (entidad.MaesContra.Length <= 8)
                throw new Exception("La contraseña debe contener 8 caracteres");
            if (entidad.NumControl <= 0)
                throw new Exception("Escriba el número de control de docente");
            return true;
        }
    }
}
