using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GenerarFicheroContraseñas
{
    internal class User
    {
        // El nombre
        private const int numNombre = 16;
        private char[] Nombre = new char[numNombre];
        public char[] getNombre() { return Nombre; }

        // El salt
        private const int numSalt = 16;
        private byte[] Salt = new byte[numSalt];
        public byte[] getSalt() { return Salt; }

        // Resumen de contraseña
        private const int numResuContra = 32;
        private byte[] ResuContra = new byte[numResuContra];
        public byte[] getResuContra() { return ResuContra; }

        // Variables para la contraseña y el salt de la contraseña en bytes
        public byte[] NuevaContraBytes;
        public byte[] SaltYContra;

        // Constructor User
        public User(String NuevoNombre, String NuevaContra)
        {
            // 1) Dar valor al nombre
            for (int i = 0; i < Nombre.Length; i++)
            {
                // Se puede rellenar con " "
                Nombre[i] = ' ';
                // Se puede rellenar con  "0"
                //Nombre[i] = 0;
            }
            // Dar valor a nuevo nombre -> Si no se rellena la longitud de nombre queda con " " o "0"
            for (int i = 0; i < NuevoNombre.Length; i++)
            {
                Nombre[i] = NuevoNombre[i];
            }

            // 2) Generar el Salt

            // Cryptear un proveedor de numeros aleatorios
            RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();

            // Utilizar el metodo getBytes() para rellenar el salt
            rNGCryptoServiceProvider.GetBytes(Salt);

            // 3) Generar el resumen de contraseña

            // Crear el objeto de la contraseña
            NuevaContraBytes = Encoding.Unicode.GetBytes(NuevaContra);
            SaltYContra = new byte[NuevaContraBytes.Length + Salt.Length];

            // Asignar la contraseña
            /*Salt.CopyTo(SaltYContra,0);
            NuevaContraBytes.CopyTo(SaltYContra,Salt.Length);
            SHA256Managed sHA256 = new SHA256Managed();
            ResuContra = sHA256.ComputeHash(SaltYContra);*/

            Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(NuevaContra, Salt);
            rfc2898DeriveBytes.IterationCount = 1000;
            ResuContra = rfc2898DeriveBytes.GetBytes(numResuContra);
        }

    }
}
