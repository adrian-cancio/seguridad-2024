using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio1_AUTEN
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Introduce el nombre: ");
            string nombre = Console.ReadLine();

            Console.WriteLine("Introduce la contraseña: ");
            string contraseña = Console.ReadLine();

            //Creamos el objeto USUCONTRA
            UsuContra UC = new UsuContra();

            //Llamamos al metodo Verifica que devuelve un entero
            int verificado = UC.VerificaTxt(nombre, contraseña);

            //Dependiendo del entero se muestra un mensaje u otro
            switch (verificado)
            {
                case 0:
                    Console.WriteLine("USUARIO LOGUEADO CON ÉXITO"); break;
                case 1:
                    Console.WriteLine("USUARIO NO ENCONTRADO"); break;
                case 2:
                    Console.WriteLine("CONTRASEÑA INCORRECTA"); break;
            }

            Console.ReadKey();
        }
    }

    class UsuContra
    {

        // Declaramos en variables los tamaños
        private static int maxUsu = 7; // lo sacamos del fichero
        private static int saltSize = 32; // enunciado
        private static int contraSize = 64; // enunciado

        // Declaracion de los atributos (campos del usuario)
        public string nombre;
        public byte[] salt = new byte[saltSize];
        public byte[] resuContra = new byte[contraSize];
        public string infoAdicional;

        public int VerificaTxt(String nombreIn, String contraIn)
        {
            int iter = 0;
            bool encontrado = false;

            FileStream fs = new FileStream("zz_ListaUsuarios.txt", FileMode.Open, FileAccess.Read);

            // Se utiliza StreamReader porque es un ejercicio de AUTENTICACION CON TXT
            StreamReader reader = new StreamReader(fs, Encoding.Unicode);


            int numCarSalt = 44;
            int numCarResuContra = 88;
            char[] CarSalt = new char[numCarSalt];
            char[] CarResuContra = new char[numCarResuContra];

            do
            {
                nombre = reader.ReadLine();
                Console.WriteLine(nombre);

                reader.Read(CarSalt, 0, numCarSalt);
                Console.WriteLine(CarSalt);
                reader.ReadLine();

                reader.Read(CarResuContra, 0, numCarResuContra);
                Console.WriteLine(CarResuContra);
                reader.ReadLine();

                infoAdicional = reader.ReadLine();
                Console.WriteLine(infoAdicional);

                // Decodificamos el Base64
                salt = Convert.FromBase64CharArray(CarSalt, 0, numCarSalt);
                resuContra = Convert.FromBase64CharArray(CarResuContra, 0, numCarResuContra);

                iter++;

                // Comprobamos si se ha encontrado o no
                encontrado = (nombreIn == nombre);


            } while (!encontrado && iter < maxUsu);

            reader.Close();
            fs.Close();
            reader.Dispose();
            fs.Dispose();

            // Si no se ha encontrado el nombre en el fichero
            if(!encontrado)
            {
                return 1; // Usuario desconocido
            }


            // Para calcular el resumen de la contraseña:

            // Obtener los bytes de la contraseña con codificcion unicode
            byte[] contraBytes = Encoding.Unicode.GetBytes(contraIn);

            // Añadir al salt los bytes de la contraseña
            byte[] saltYContra = new byte[saltSize + contraBytes.Length];

            // Resumir los bytes obtenidos concatenando el salt y la contraseña con shamanaged
            SHA512Managed sha = new SHA512Managed();
            byte[] resumenContraIntro = sha.ComputeHash(saltYContra);

            if (VerificarContra(resuContra, resumenContraIntro))
            {
                return 0;
            }
            else
                return 2;

        }

        private bool VerificarContra(byte[] resuContra, byte[] resuContraIn)
        {
            bool verify = true;

            for (int i = 0; i < contraSize; i++)
            {
                verify = verify && resuContra[i] == resuContraIn[i];
            }
            return verify;
        }
    }
}
