using System;
using System.Collections.Generic;

namespace Act2_RolesUsuario.Models
{
    public partial class Alumno
    {
        public int IdAlumno { get; set; }
        public string NoControl { get; set; }
        public string Nombre { get; set; }
        public int MaesId { get; set; }

        public virtual Maestro Maes { get; set; }
    }
}
