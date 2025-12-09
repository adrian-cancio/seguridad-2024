using GenerarFicheroContraseñas;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerarFicheroContraseñas
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Crear la lista de usuaruios y rellenarla
            ListaU Lu = new ListaU();
            Lu.IniUsu(0, new User("Antonio", "conA"));
            Lu.IniUsu(1, new User("Benito", "conB"));
            Lu.IniUsu(2, new User("Carlos", "conC"));
            Lu.IniUsu(3, new User("David", "conD"));
            Lu.IniUsu(4, new User("Eduardo", "conE"));

            // Mostrar los usuarios
            Console.WriteLine("Lista de usuarios: \n");
            Lu.VerLista();

            // Guardar la lista en bin
            Console.WriteLine("\nGuardando la lista en bin...");
            Lu.GuardaListaBin("zz_Usuarios.bin");
            Console.WriteLine("\nGuardado con exito");

            // Guardar la lista en txt
            Console.WriteLine("\nGuardando la lista en txt...");
            Lu.GuardaListaTxt("zz_Usuarios.txt");
            Console.WriteLine("\nGuardado con exito");

            // Guardar la lista en xml
            Console.WriteLine("\nGuardando la lista en xml...");
            Lu.GuardaListaXml("zz_Usuarios.xml");
            Console.WriteLine("\nGuardado con exito\n");
        }
    }
}
