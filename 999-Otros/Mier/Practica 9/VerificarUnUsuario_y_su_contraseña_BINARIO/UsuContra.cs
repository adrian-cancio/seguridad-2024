using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VerificarUnUsuario_y_su_contraseña
{
    internal class UsuContra
    {
        // Declaraciones iniciales
        private const int maxUsu = 5;
        private const int maxNombre = 16;
        private const int maxSalt = 16;
        private const int maxResuContra = 32;
        private char[] Nombre = new char[maxNombre];
        private byte[] Salt = new byte[maxSalt];
        private byte[] ResuContra = new byte[maxResuContra];

        // Declaraciones de variables para la nueva contraseña y el resumen de la misma con el salt del usuario encontrado en el fichero
        public byte[] NuevaContraBytes;
        public byte[] SaltYContra;

        // Metodo que verifica una contraseña
        public int Verifica(String NombreIn, String ContraIn)
        {
            if (NombreIn == null)
            {
                throw new ArgumentNullException("NombreIn");
            }

            bool usuarioEncontrado = false;
            int retorno = 0;
            int contador = 0;

            // 1) Copiar el nombre
            char[] NombreInChars = new char[maxNombre];
            for (int i = 0; i < NombreInChars.Length; i++) { NombreInChars[i] = ' '; }
            for (int i = 0; i < NombreIn.Length; i++) { NombreInChars[i] = NombreIn[i]; }

            // 2) Buscar el nombre en el fichero de contraseñas
            FileStream fs = new FileStream("zz_Usuarios.bin", FileMode.Open);
            BinaryReader bin_reader = new BinaryReader(fs, Encoding.Unicode);

            // ------ DO-WHILE ------ //
            do
            {
                // Datos a buscar
                Nombre = bin_reader.ReadChars(maxNombre);
                Salt = bin_reader.ReadBytes(maxSalt);
                ResuContra = bin_reader.ReadBytes(maxResuContra);

                // Metodo de comparacion
                if (CompareCharArray(Nombre, NombreInChars))
                {
                    usuarioEncontrado = true;
                    break;
                }
                contador++;

                // Lee un registro de cada usuario (Nombre, Salt y Resumen de la contraseña)
            } while (bin_reader.BaseStream.Position < bin_reader.BaseStream.Length);
            // ------ END DO-WHILE ------ //

            // 3) Cerrar los lectores
            bin_reader.Close();
            fs.Close();

            // 4) Si se ha encontrado el usuario se muestra
            if (usuarioEncontrado)
            {
                Console.WriteLine("\nUsuario encontrado");
            }
            // Si no se ha encontrado, se muestra y se sale
            else { Console.WriteLine("\nUsuario no encontrado"); retorno = 1; return retorno; }

            // 5) Calcular el resumen de la contraseña
            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(ContraIn, Salt);
            deriveBytes.IterationCount = 1000;
            byte[] resumenContra = deriveBytes.GetBytes(32);

            // MEJORAS DE APARTADOS SIGUIENTES
            /*NuevaContraBytes = Encoding.Unicode.GetBytes(ContraIn);
            SaltYContra = new byte[NuevaContraBytes.Length + Salt.Length];*/
            /*Salt.CopyTo(SaltYContra,0);
            NuevaContraBytes.CopyTo(SaltYContra,Salt.Length);
            SHA256Managed sHA256 = new SHA256Managed();
            byte[] resumenContra = sHA256.ComputeHash(SaltYContra);*/

            // 6) Comprobar que los resumenes son iguales
            bool sonIguales = CompareByteArrays(resumenContra, ResuContra);
            if (sonIguales)
            {
                Console.WriteLine("\nLos resúmenes son iguales.");
            }
            else
            {
                Console.WriteLine("\nLos resúmenes son diferentes.");
                retorno = 2;
                return retorno;

            }
            return retorno;
        }

        // Metodo que compara dos arrays de caracteres
        static bool CompareCharArray(char[] array1, char[] array2)
        {
            if (array1.Length != array2.Length)
                return false;

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                    return false;
            }
            return true;
        }

        // Metodo que compara dos arrays de bytes
        static bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
                return false;

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                    return false;
            }
            return true;
        }
    }
}


