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

namespace VerificarUnUsuario_y_su_contraseñaTXT
{
    internal class UsuContra
    {
        // Declaraciones iniciales
        private const int maxUsu = 5;
        private const int maxNombre = 16;
        private const int maxSalt = 16;
        private const int maxResuContra = 32;

        // Declaramos los arrays de bytes para los datos de los usuarios y contraseñas
        private char[] Nombre = new char[maxNombre];
        private byte[] Salt = new byte[maxSalt];
        private byte[] ResuContra = new byte[maxResuContra];

        // Declaramos los arrays de bytes para la nueva contraseña
        public byte[] NuevaContraBytes;
        public byte[] SaltYContra;

        // Metodo que verifica una contraseña
        public int Verifica(String NombreIn, String ContraIn)
        {
            bool usuarioEncontrado = false;
            int retorno = 0;

            // 1) Copiar el nombre
            char[] NombreInChars = new char[maxNombre];
            for (int i = 0; i < NombreInChars.Length; i++) { NombreInChars[i] = ' '; }
            for (int i = 0; i < NombreIn.Length; i++) { NombreInChars[i] = NombreIn[i]; }

            // 2) Declarar el SaltChar
            int NumCarSalt = (int)(4 * Math.Ceiling(((double)maxSalt / 3.0)));
            char[] SaltChar = new char[NumCarSalt];

            // 3) Declarar el array de caracteres ResuContraChar
            int NumCarResuContra = (int)(4 * Math.Ceiling(((double)maxResuContra / 3.0)));
            char[] ResuContraChar = new char[NumCarResuContra];

            // 4) Buscar el nombre en el fichero de contraseñas
            FileStream fs = new FileStream("zz_Usuarios.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs, Encoding.ASCII);

            // ------ DO-WHILE ------ //
            int n = 0;
            do
            {
                // Obtener el nombre
                sr.Read(Nombre, 0, maxNombre);

                // Obtener el Salt
                sr.Read(SaltChar, 0, NumCarSalt);
                Salt = Convert.FromBase64CharArray(SaltChar, 0, NumCarSalt);

                // Obtener el ResuContra
                sr.Read(ResuContraChar, 0, NumCarResuContra);
                ResuContra = Convert.FromBase64CharArray(ResuContraChar, 0, NumCarResuContra);

                // Pasar al siguiente elemento
                sr.ReadLine();

                // Metodo de comparacion
                if (CompareCharArray(Nombre, NombreInChars))
                {
                    usuarioEncontrado = true;
                    break;
                }
                n++;
                // Lee un registro de casa usuario
            } while (!usuarioEncontrado && !sr.EndOfStream);
            // ------ END DO-WHILE ------ //

            // 5) Cerrar los lectores
            sr.Close();
            fs.Close();

            // 6) Si se ha encontrado el usuario se muestra
            if (usuarioEncontrado)
            {
                Console.WriteLine("\nUsuario encontrado");
            }
            // Si no se ha encontrado, se muestra y se sale
            else { 
                Console.WriteLine("\nUsuario no encontrado"); 
                retorno = 1; 
                return retorno; 
            }

            // 7) Calcular el resumen de la contraseña
            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(ContraIn, Salt);
            deriveBytes.IterationCount = 1000;
            byte[] resumenContra = deriveBytes.GetBytes(32);

            // SIGUIENTES LINEAS SON LOS PUNTOS 5 Y 6 DEL GUION
            /*NuevaContraBytes = Encoding.Unicode.GetBytes(ContraIn);
            SaltYContra = new byte[NuevaContraBytes.Length + Salt.Length];*/
            /*Salt.CopyTo(SaltYContra, 0);
            NuevaContraBytes.CopyTo(SaltYContra, Salt.Length);
            SHA256Managed sHA256 = new SHA256Managed();
            byte[] resumenContra = sHA256.ComputeHash(SaltYContra);*/

            // 8) Comprobar que los resumenes son iguales
            bool sonIguales = CompareByteArrays(resumenContra, ResuContra);
            if (sonIguales)
            {
                Console.WriteLine("\nLos resúmenes son iguales.");
                Console.WriteLine("Contraseña correcta.");
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