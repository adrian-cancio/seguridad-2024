using Apoyo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CifraFicheroBigAES
{
    internal class Program
    {
        static void Main(string[] args)
        {
            String NombreFich = "fichero.txt";
            String NombreFichEnc = "ficheroEnc.txt";
            String NombreFichDec = "ficheroDec.txt";

            Ayuda Ayuda = new Ayuda();

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

            long maxBlockFich = aes.BlockSize * 8; // Maximos bloques cifrados/descifrados de cada vez
            long tamFich = Ayuda.BytesFichero(NombreFich);

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

            // Ciframos los datos del fichero teniendo en cuenta el tamBytesFich

            ICryptoTransform Encryptor = aes.CreateEncryptor();
            long bitsCifrados = 0;
            byte[] Bufer = new byte[maxBlockFich];
            byte[] BuferCifrado = new byte[maxBlockFich];
            FileStream fs = new FileStream(NombreFich, FileMode.Open, FileAccess.Read, FileShare.None);
            FileStream fsEnc = new FileStream(NombreFichEnc, FileMode.Create, FileAccess.Write, FileShare.None);

            while (bitsCifrados < tamFich)
            {
                // Leemos del fichero
                long bytesLeidos = fs.Read(Bufer, 0, (int)maxBlockFich);
                // Si no hemos leido nada, salimos
                if (bytesLeidos == 0) break;
                // Si hemos leido el último bloque, usamos TransformFinalBlock
                if (bitsCifrados + bytesLeidos >= tamFich)
                {
                    // Ciframos el último bloque
                    byte[] finalBlock = Encryptor.TransformFinalBlock(Bufer, 0, (int)bytesLeidos);
                    // Guardamos el último bloque cifrado
                    fsEnc.Write(finalBlock, 0, finalBlock.Length);
                }
                else
                {
                    // Ciframos los datos leidos
                    int bytesCifrados = Encryptor.TransformBlock(Bufer, 0, (int)bytesLeidos, BuferCifrado, 0);
                    // Guardamos los datos cifrados
                    fsEnc.Write(BuferCifrado, 0, bytesCifrados);
                }
                // Actualizamos el numúmero toal de bits cifrados
                bitsCifrados += bytesLeidos;
            }

            // Cerramos los ficheros
            fs.Close();
            fsEnc.Close();
            fs.Dispose();
            fsEnc.Dispose();
            fs = null;
            fsEnc = null;
            BuferCifrado = null;

            // Desciframos los datos del fichero cifrado
            
            long tamFichEnc = Ayuda.BytesFichero(NombreFichEnc);
            ICryptoTransform Decryptor = aes.CreateDecryptor();
            byte[] BuferDescrifrado = new byte[maxBlockFich];
            fs = new FileStream(NombreFichEnc, FileMode.Open, FileAccess.Read, FileShare.None);
            FileStream fsDec = new FileStream(NombreFichDec, FileMode.Create, FileAccess.Write, FileShare.None);

            long bitsDescifrados = 0;
            while (bitsDescifrados < tamFichEnc)
            {
               // Leemos del fichero
               long bytesLeidos = fs.Read(Bufer, 0, (int)maxBlockFich);
                // Si no hemos leido nada, salimos
                if (bytesLeidos == 0) break;
                // Si hemos leido el último bloque, usamos TransformFinalBlock
                if (bitsDescifrados + bytesLeidos >= tamFichEnc)
                {
                    // Desciframos el último bloque
                    byte[] finalBlock = Decryptor.TransformFinalBlock(Bufer, 0, (int)bytesLeidos);
                    // Guardamos el último bloque descifrado
                    fsDec.Write(finalBlock, 0, finalBlock.Length);
                }
                else
                {
                    // Desciframos los datos leidos
                    int bytesDescifrados = Decryptor.TransformBlock(Bufer, 0, (int)bytesLeidos, BuferDescrifrado, 0);
                    // Guardamos los datos descifrados
                    fsDec.Write(BuferDescrifrado, 0, bytesDescifrados);
                }
                // Actualizamos el numúmero toal de bits descifrados
                bitsDescifrados += bytesLeidos;
            }

            // Cerramos los ficheros
            fs.Close();
            fsDec.Close();
            fs.Dispose();
            fsDec.Dispose();
            fs = null;
            fsDec = null;
            BuferDescrifrado = null;
            Bufer = null;
        }
    }
}
