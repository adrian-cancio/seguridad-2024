using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using Apoyo;

namespace AUTENTICA
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Introduce el nombre de usuario: ");
            string nombre = Console.ReadLine();
            Console.WriteLine("Introduce la contraseña: ");
            string contra = Console.ReadLine();

            // Leer datos del usuario del fichero
            string[] lines = File.ReadAllLines("usuarios.txt");
            string nombreUsuario = null;
            byte[] salt = null;
            byte[] resumenAlmacenado = null;
            string infoAdicional = null;

            for (int i = 0; i < lines.Length; i += 4)
            {
                if (lines[i] == nombre)
                {
                    nombreUsuario = lines[i];
                    salt = Convert.FromBase64String(lines[i + 1]);
                    resumenAlmacenado = Convert.FromBase64String(lines[i + 2]);
                    infoAdicional = lines[i + 3];
                    break;
                }
            }

            if (nombreUsuario == null)
            {
                Console.WriteLine(1); // Usuario desconocido
                return;
            }

            // Verificar la contraseña
            byte[] contraBytes = Encoding.Unicode.GetBytes(contra);
            byte[] saltYContra = salt.Concat(contraBytes).ToArray();

            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] resumenContra = sha512.ComputeHash(saltYContra);
                if (resumenContra.SequenceEqual(resumenAlmacenado))
                {
                    Console.WriteLine(0); // Usuario y contraseña correctos
                }
                else
                {
                    Console.WriteLine(2); // Contraseña inválida
                }
            }
        }
    }
}
