using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VerificaContra;

namespace MedirTiempos
{
    internal class Program
    {
        static void Main()
        {
            User.HASH_METHOD hashMethod = User.HASH_METHOD.SHA256;

            ListaU LU = new ListaU();
            LU.IniUsu(0, new User("Antonio", "conA", hashMethod));
            LU.IniUsu(1, new User("Benito", "conB", hashMethod));
            LU.IniUsu(2, new User("Carlos", "conC", hashMethod));
            LU.IniUsu(3, new User("David", "conD", hashMethod));
            LU.IniUsu(4, new User("Eduardo", "conE", hashMethod));
            LU.VerLista();

            int res = LU.Verifica("Antoio", "con");

            Console.WriteLine(res);

        }
    }
}
