using Credenciales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VerificarUnUsuario_y_su_contraseña_XML_SERIALIZADO
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Introduce el nombre de usuario: ");
            string Nombre = Console.ReadLine();
            Console.Write("Introduce la contraseña: ");
            string Contra = Console.ReadLine();

            switch(Verifica(Nombre, Contra, Almacen.Deserializar("AlmacenUsuarios.xml")))
            {
                case 0:
                    Console.WriteLine("\nUsuario y contraseña correctos.");
                    break;
                case 1:
                    Console.WriteLine("\nEl usuario no existe.");
                    break;
                case 2:
                    Console.WriteLine("\nContraseña incorrecta.");
                    break;
            }
        }

        static int Verifica(string NombreIn, string ContraIn, Almacen Alma)
        {
            string Nombre = "";
            byte[] Salt = new byte[Usuario.LongiSalt];
            byte[] ResuContra = new byte[Usuario.LongiResuContra];

            foreach (Usuario u in Alma.usuarios)
            {
                if (u.Nombre == NombreIn)
                {
                    Nombre = u.Nombre;
                    Salt = u.Salt;
                    ResuContra = u.ResuContra;
                    break;
                }
            }

            if (Nombre == "") return 1;

            byte[] Contra = Encoding.UTF8.GetBytes(ContraIn);
            byte[] ResuContraIntroducida = new byte[Usuario.LongiResuContra];

            using (Rfc2898DeriveBytes Derivador = new Rfc2898DeriveBytes(Contra, Salt, 10000))
            {
                ResuContraIntroducida = Derivador.GetBytes(Usuario.LongiResuContra);
            }

            if (!ResuContraIntroducida.SequenceEqual(ResuContra)) return 2;
            return 0;
        }
    }
}
