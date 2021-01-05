using System;
using System.Collections.Generic;

namespace Act2_RolesUsuario.Models
{
    public partial class Maestro
    {
        public Maestro()
        {
            Alumno = new HashSet<Alumno>();
        }

        public int IdMaestro { get; set; }
        public int NumControl { get; set; }
        public string Nombre { get; set; }
        public string MaesContra { get; set; }
        public ulong? Activo { get; set; }

        public virtual ICollection<Alumno> Alumno { get; set; }
    }
}
