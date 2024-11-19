using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerificaContra;

namespace MejoraLista
{
    internal class Program
    {
        static void Main()
        {
            ListaU LU = new ListaU();
            User.HASH_METHOD hashMethod = User.HASH_METHOD.RFC;

            LU.IniUsu(0, new User("Antonio", "conA", hashMethod));
            LU.IniUsu(1, new User("Benito", "conB", hashMethod));
            LU.IniUsu(2, new User("Carlos", "conC", hashMethod));
            LU.IniUsu(3, new User("David", "conD", hashMethod));
            LU.IniUsu(4, new User("Eduardo", "conE", hashMethod));
            //LU.VerLista();

            LU.GuardaListaBin("ListaU.bin");

            ListaU LU1 = new ListaU("ListaU.bin", ListaU.SUPORTED_FILE_TYPE.BIN, hashMethod);
            LU1.VerLista();

            LU.GuardaListaTxt("ListaU.txt");

            ListaU LU2 = new ListaU("ListaU.txt", ListaU.SUPORTED_FILE_TYPE.TXT, hashMethod);
            LU2.VerLista();

            int Res = LU.Verifica("Antonio", "conA");

            Console.WriteLine(Res);
        }
    }
}
