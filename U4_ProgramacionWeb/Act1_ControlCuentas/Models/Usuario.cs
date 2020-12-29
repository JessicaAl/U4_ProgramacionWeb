using System;
using System.Collections.Generic;

namespace Act1_ControlCuentas.Models
{
    public partial class Usuario
    {
        public int Id { get; set; }
        public string NomUsuario { get; set; }
        public string Correo { get; set; }
        public string Contra { get; set; }
        public ulong? Activo { get; set; }
        public int Codigo { get; set; }
    }
}
