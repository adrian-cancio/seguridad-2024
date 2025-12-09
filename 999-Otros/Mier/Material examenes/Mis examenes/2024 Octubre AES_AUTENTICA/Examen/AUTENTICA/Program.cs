using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AUTENTICA
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
            int verificado = UC.Verifica(nombre, contraseña);

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
        private static int maxUsu = 7; // enunciado
        private static int saltSize = 63; // enunciado
        private static int contraSize = 64; // enunciado

        // Declaracion de los atributos (campos del usuario)
        public string Nombre;
        public byte[] salt = new byte[saltSize];
        public byte[] resuContra = new byte[contraSize];
        public string infoAdicional;

        public int Verifica(String nombreIn, String contraIn)
        {
            int iter = 0;
            bool encontrado = false;

            // Creamos objeos filestream y binaryreader
            FileStream fs = new FileStream("zzFichero.bin", FileMode.Open, FileAccess.Read);
            BinaryReader bin_reader = new BinaryReader(fs, Encoding.Unicode);

            do
            {
                // Datos a buscar
                //Nombre = bin_reader.Read(aux, 0, aux.Length);
                Nombre = bin_reader.ReadString();
                salt = bin_reader.ReadBytes(saltSize);
                resuContra = bin_reader.ReadBytes(contraSize);
                infoAdicional = bin_reader.ReadString();

                // Realizamos la comparacion
                if(CompareCharArray(nombreIn.ToCharArray(), Nombre.ToCharArray()))
                {
                    encontrado = true;
                    break; // Salimos del bucle
                }

                iter++;

            } while (bin_reader.BaseStream.Position < bin_reader.BaseStream.Length);

            // Cerramos los lectores
            bin_reader.Close();
            fs.Close();
            bin_reader.Dispose();
            fs.Dispose();

            // Si no se ha encontrado el nombre en el fichero
            if (!encontrado)
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

            if (CompareByteArrays(resuContra, resumenContraIntro))
            {
                return 0;
            }
            else
                return 2;
        }

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
