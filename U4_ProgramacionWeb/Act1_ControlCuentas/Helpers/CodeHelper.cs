using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Act1_ControlCuentas.Helpers
{
    public class CodeHelper
    {
        public static int GetCode()
        {
            Random random = new Random();
            int cod1 = random.Next(1000, 9999);
            int cod2 = random.Next(1000, 9999);
            return (cod1 + cod2);
        }
    }
}
