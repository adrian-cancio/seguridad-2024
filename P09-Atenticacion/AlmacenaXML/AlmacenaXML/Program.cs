using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerificaContra;

namespace AlmacenaXML
{
    internal class Program
    {
        static void Main()
        {

            ListaU LU = new ListaU();

            // Generate 100 random users with random passwords and random hash method
            Random r = new Random();
            for (int i = 0; i < 100; i++)
            {
                String name = "User" + i;
                String password = "Password" + r.Next(1000);
                User.HASH_METHOD method = r.Next(2) == 0 ? User.HASH_METHOD.RFC : User.HASH_METHOD.SHA256;
                LU.IniUsu(new User(name, password, method));
                Console.WriteLine("User: " + name + " Password: " + password + " Method: " + method);
            }

            String filename = "zz_random_users.xml";
            LU.GuardaListaXML(filename, true);

            // Load the list from the XML file
            ListaU LU2 = new ListaU(filename, ListaU.SUPORTED_FILE_TYPE.XML);
            //LU2.VerLista();
            while (true)
            {
                Console.WriteLine("Introduce el nombre de usuario:");
                String name = Console.ReadLine();
                Console.WriteLine("Introduce la contraseña:");
                String password = Console.ReadLine();
                switch (LU2.Verifica(name, password))
                //switch (User.VerificaXML(filename, name, password))
                {
                    case 1:
                        Console.WriteLine("El usuario no existe.");
                        break;
                    case 2:
                        Console.WriteLine("La contraseña es incorrecta.");
                        break;
                    case 0:
                        Console.WriteLine("Usuario y contraseña correctos.");
                        break;
                    default:
                        Console.WriteLine("Error desconocido.");
                        break;
                }

            }
        }
    }
}
