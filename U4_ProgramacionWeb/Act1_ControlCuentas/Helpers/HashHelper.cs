using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Act1_ControlCuentas.Helpers
{
    public class HashHelper
    {
        public static string GetHash(string cadena)
        {
            var sha = SHA256.Create();
            byte[] codificar = Encoding.UTF8.GetBytes(cadena);
            byte[] hash = sha.ComputeHash(codificar);
            string a = "";
            foreach (var objeto in hash)
            {
                a += objeto.ToString("x2");
            }
            return a;
        }
    }
}
