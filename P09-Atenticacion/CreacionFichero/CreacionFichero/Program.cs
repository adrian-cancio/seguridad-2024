using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreacionFichero
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ListaU LU = new ListaU();
            LU.IniUsu(0, new User("Antonio", "conA"));
            LU.IniUsu(1, new User("Benito", "conB"));
            LU.IniUsu(2, new User("Carlos", "conC"));
            LU.IniUsu(3, new User("David", "conD"));
            LU.IniUsu(4, new User("Eduardo", "conE"));
            LU.VerLista();
            LU.GuardaListaBin("ListaU.bin");
            LU.GuardaListaTxt("ListaU.txt");
        }
    }
}
