using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerificaContra
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

            Console.WriteLine();
            Console.Write("Introduce tu nombre de usuario: ");
            string nombre = Console.ReadLine();
            Console.Write("Introduce tu contraseña: ");
            string contra = Console.ReadLine();
            Console.WriteLine();

            int loginResult = LU.Verifica(nombre, contra);

            if (loginResult == 0)
            {
                Console.WriteLine("Login exitoso");
            }
            else if (loginResult == 1)
            {
                Console.WriteLine("Usuario no encontrado");
            }
            else if (loginResult == 2)
            {
                Console.WriteLine("Contraseña incorrecta");
            }

            LU.GuardaListaTxt("zz_Usuarios.txt");
            LU.GuardaListaBin("zz_Usuarios.bin");

            int txt = User.VerificaTxt("zz_Usuarios.txt", nombre, contra);
            Console.WriteLine("txt output: {0}",txt);

            int bin = User.VerificaBin("zz_Usuarios.bin", nombre, contra);
            Console.WriteLine("bin output: {0}", bin);

        }
    }
}
