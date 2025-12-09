using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerificarUnUsuario_y_su_contraseña_XML
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Introduce usuario: ");
            string nombre = Console.ReadLine();

            Console.Write("Introduce contraseña: ");
            string contra = Console.ReadLine();

            UsuContra usuContra = new UsuContra();

            // PRUEBA CON UN USUARIO CONCRETO
            //int resultado = usuContra.Verifica("Eduardo", "conE");

            int resultado = usuContra.Verifica(nombre, contra);
            switch (resultado)
            {
                case 0:
                    Console.WriteLine("\nUsuario correcto");
                    break;
                case 1:
                    Console.WriteLine("\nUsuario no valido");
                    break;
                case 2:
                    Console.WriteLine("\nContraseña incorrecta");
                    break;
                default:
                    break;
            }
        }
    }
}
