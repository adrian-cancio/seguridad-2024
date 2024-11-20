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
            User.HASH_METHOD hashMethod = User.HASH_METHOD.RFC;
            ListaU LU = new ListaU(hashMethod);

            LU.IniUsu(0, new User("Antonio", "conA", hashMethod));
            LU.IniUsu(1, new User("Benito", "conB", hashMethod));
            LU.IniUsu(2, new User("Carlos", "conC", hashMethod));
            LU.IniUsu(3, new User("David", "conD", hashMethod));
            LU.IniUsu(4, new User("Eduardo", "conE", hashMethod));
            LU.IniUsu(5, new User("Fernando", "conF", hashMethod));
            LU.IniUsu(6, new User("Gonzalo", "conG", hashMethod));
            LU.IniUsu(7, new User("Hector", "conH", hashMethod));
            LU.IniUsu(8, new User("Ismael", "conI", hashMethod));
            LU.IniUsu(9, new User("Juan", "conJ", hashMethod));
            LU.IniUsu(10, new User("Kiko", "conK", hashMethod));
            LU.ModUsu("Antonio", "conA2");
            LU.ModUsu("Eduardo", "conE2");
            LU.DelUsu("Benito");

            Console.WriteLine("ListaU:");
            LU.VerLista();

            LU.GuardaListaBin("ListaU.bin");

            ListaU LU1 = new ListaU("ListaU.bin", ListaU.SUPORTED_FILE_TYPE.BIN, hashMethod);
            Console.WriteLine("\nListaU1:");
            LU1.VerLista();

            LU.GuardaListaTxt("ListaU.txt");

            ListaU LU2 = new ListaU("ListaU.txt", ListaU.SUPORTED_FILE_TYPE.TXT, hashMethod);
            Console.WriteLine("\nListaU2:");
            LU2.VerLista();

            int Res0 = LU.Verifica("Antonio", "conA");
            Console.WriteLine("Res0 -> {0}", Res0);
            
            int Res1 = LU1.Verifica("Benito", "conB");
            Console.WriteLine("Res1 -> {0}", Res1);
            
            int Res2 = LU2.Verifica("Carlos", "conC");
            Console.WriteLine("Res2 -> {0}", Res2);
            
            int Res3 = User.VerificaBin("ListaU.bin", "David", "conD", hashMethod);
            Console.WriteLine("Res3 -> {0}", Res3);
            
            int Res4 = User.VerificaTxt("ListaU.txt", "Eduardo", "conE", hashMethod);
            Console.WriteLine("Res4 -> {0}", Res4);

        }
    }
}
