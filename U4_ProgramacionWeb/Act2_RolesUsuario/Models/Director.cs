using System;
using System.Collections.Generic;

namespace Act2_RolesUsuario.Models
{
    public partial class Director
    {
        public int IdDire { get; set; }
        public int NumControl { get; set; }
        public string Nombre { get; set; }
        public string DireContra { get; set; }
    }
}
