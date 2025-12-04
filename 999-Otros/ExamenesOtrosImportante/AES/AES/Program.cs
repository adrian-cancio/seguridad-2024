using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Apoyo;

namespace AES
{
    internal class Program
    {
        static void Main(string[] args)
        {

            if (args.Length != 2) {
                Console.WriteLine("Debes pasar 2 argumentos");
            }


            String NombreFich = args[0];
            String NombreFichEnc = args[1];
            String NombreFichDec = args[1]+"dec.txt";

            Ayuda Ayuda = new Ayuda();

            const int TamClave = 32;
            byte[] Clave = new byte[TamClave];
            for (int i = 0; i < Clave.Length; i++) Clave[i] = (byte)(i % 256);

            byte[] VI = new byte[16];
            for (int i = 0; i < VI.Length; i++) VI[i] = (byte)((i + 160) % 256);


            AesManaged aes = new AesManaged();

            Console.WriteLine("Datos originales de AES:");
            Console.WriteLine("Block size: " + aes.BlockSize);
            Console.WriteLine("Key size: " + aes.KeySize);
            Console.WriteLine("Padding: " + aes.Padding);
            Console.WriteLine("Mode: " + aes.Mode);
            Console.WriteLine();

            // Establecemos los valores del AESManaged (Provider)

            aes.BlockSize = 128; // ??????
            aes.KeySize = 256;
            aes.Padding = PaddingMode.ISO10126;
            aes.Mode = CipherMode.CBC;

            // --- Configuración de AES completa (BlockSize, KeySize, Padding, Mode) ---

            Console.WriteLine(aes.IV.Length);

            Console.WriteLine("Datos modificados de AES:");
            Console.WriteLine("Block size: " + aes.BlockSize);
            Console.WriteLine("Key size: " + aes.KeySize);
            Console.WriteLine("Padding: " + aes.Padding);
            Console.WriteLine("Mode: " + aes.Mode);
            Console.WriteLine();

            long maxBlockFich = aes.BlockSize / 8; // Maximos bloques cifrados/descifrados de cada vez
            long tamFich = Ayuda.BytesFichero(NombreFich);

            // Generamos la clave y el vector de inicialización

            // --- Generación / asignación de clave (Key) y vector de inicialización (IV) ---

            aes.Key = Clave;
            aes.IV = VI;

            // Mostramos la clave y el vector de inicialización

            Console.WriteLine("Clave generada:");
            Ayuda.WriteHex(aes.Key, aes.Key.Length);
            Console.WriteLine();

            Console.WriteLine("Vector de inicialización:");
            Ayuda.WriteHex(aes.IV, aes.IV.Length);
            Console.WriteLine();

            // --- Apertura de streams: entrada, cifrado y salida ---
            FileStream fsEntrada = new FileStream(NombreFich, FileMode.Open, FileAccess.Read);
            FileStream fsSalida = new FileStream(NombreFichEnc, FileMode.Create, FileAccess.Write);
            CryptoStream cs = new CryptoStream(fsSalida, aes.CreateEncryptor(), CryptoStreamMode.Write);
            BinaryReader br = new BinaryReader(fsEntrada, Encoding.UTF8);
            BinaryWriter bw = new BinaryWriter(cs, Encoding.UTF8);

            // --- Bucle de lectura/escritura en chunks ---
            // Leemos el fichero de entrada en bloques pequeños (chunk) y escribimos
            // exactamente los bytes leídos en el BinaryWriter; el CryptoStream
            // realizará el cifrado y el buffering internamente.
            byte[] buffer = new byte[10];
            int bytesLeidos = 0;

            // Leemos por primera vez antes de entrar al bucle y luego seguimos leyendo al final de cada iteración.
            bytesLeidos = br.Read(buffer, 0, buffer.Length);
            while (bytesLeidos > 0)
            {
                bw.Write(buffer, 0, bytesLeidos);

                // Leemos el siguiente chunk
                bytesLeidos = br.Read(buffer, 0, buffer.Length);
            }

            // --- Cierre y liberación de recursos ---
            cs.Close();
            cs.Dispose();
            bw.Close();
            bw.Dispose();
            br.Close();
            br.Dispose();

            

        }
    }
}
