using Apoyo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CifraFicheroSmallAES
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            String NombreFich = "fichero.txt";
            String NombreFichEnc = "ficheroEnc.txt";
            String NombreFichDec = "ficheroDec.txt";
            long maxBytesFich = 1<<10;
            
            Ayuda Ayuda = new Ayuda();

            long TamFich = Ayuda.BytesFichero(NombreFich);
            byte[] DatosFichero = new byte[TamFich];
            
            Ayuda.CargaBufer(NombreFich, DatosFichero);
            
            if (TamFich <= maxBytesFich)
            {
                Console.WriteLine("Fichero original:");
                Ayuda.WriteHex(DatosFichero, DatosFichero.Length);
                Console.WriteLine();
            }

            AesManaged aes = new AesManaged();

            Console.WriteLine("Datos originales de AES:");
            Console.WriteLine("Block size: " + aes.BlockSize);
            Console.WriteLine("Key size: " + aes.KeySize);
            Console.WriteLine("Padding: " + aes.Padding);
            Console.WriteLine("Mode: " + aes.Mode);
            Console.WriteLine();

            // Establecemos los valores del AESManaged (Provider)
            
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Padding = PaddingMode.ISO10126;
            aes.Mode = CipherMode.ECB;

            Console.WriteLine("Datos modificados de AES:");
            Console.WriteLine("Block size: " + aes.BlockSize);
            Console.WriteLine("Key size: " + aes.KeySize);
            Console.WriteLine("Padding: " + aes.Padding);
            Console.WriteLine("Mode: " + aes.Mode);
            Console.WriteLine();

            // Generamos la clave y el vector de inicialización

            aes.GenerateKey();
            aes.GenerateIV();

            // Mostramos la clave y el vector de inicialización

            Console.WriteLine("Clave generada:");
            Ayuda.WriteHex(aes.Key, aes.Key.Length);
            Console.WriteLine();

            Console.WriteLine("Vector de inicialización:");
            Ayuda.WriteHex(aes.IV, aes.IV.Length);
            Console.WriteLine();

            // Ciframos los datos

            ICryptoTransform Encryptor = aes.CreateEncryptor();
            byte[] datosEnc = Encryptor.TransformFinalBlock(DatosFichero, 0, DatosFichero.Length);

            // Guardamos los datos cifrados
            
            Ayuda.GuardaBufer(NombreFichEnc, datosEnc);

            if (TamFich <= maxBytesFich)
            {
                Console.WriteLine("Fichero cifrado:");
                Ayuda.WriteHex(datosEnc, datosEnc.Length);
                Console.WriteLine();
            }

            // Desciframos los datos

            ICryptoTransform Decryptor = aes.CreateDecryptor();
            byte[] datosDec = Decryptor.TransformFinalBlock(datosEnc, 0, datosEnc.Length);

            // Guardamos los datos descifrados

            Ayuda.GuardaBufer(NombreFichDec, datosDec);

            if (TamFich <= maxBytesFich)
            {
                Console.WriteLine("Fichero descifrado:");
                Ayuda.WriteHex(datosDec, datosDec.Length);
                Console.WriteLine();
            }

            // Comprobamos que los datos descifrados son iguales a los del vector original

            bool ok = true;

            if (DatosFichero.Length == datosDec.Length )
            {
                for(int i=0; i<DatosFichero.Length; i++)
                {
                    if (DatosFichero[i] != datosDec[i])
                    {
                        ok = false;
                        break;
                    }
                }
            }

            Console.WriteLine("Los datos originales y descifrados {0}son iguales.", ok ? "" : "no ");
            Console.WriteLine();


        }
    }
}
