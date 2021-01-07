using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Act2_RolesUsuario.Models;

namespace Act2_RolesUsuario.Repositories
{
    public class AlumnosRepository: RolesRepository<Alumno>
    {
        public AlumnosRepository(bd_rolesContext context) : base(context)
        {

        }
        public Alumno GetAlumnoByNumControl(string numControl)
        {
            return Context.Alumno.FirstOrDefault(x => x.NoControl.ToUpper() == numControl.ToUpper());
        }

        public override bool Validar(Alumno entidad)
        {
            if (string.IsNullOrEmpty(entidad.NoControl))
                throw new Exception("El numero de control no puede estar vacio");
            if (string.IsNullOrEmpty(entidad.Nombre))
                throw new Exception("El Nombre no puede estar vacio");
            if (entidad.MaesId <= 0 || entidad.MaesId.ToString() == null)
                throw new Exception("El alumno debe de estar asignado a un maestro");

            return true;
        }
    }
}
