using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Apoyo;

namespace CIFRA
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Uso: CIFRA <contraseña> <fichero_entrada> <fichero_salida>");
                return;
            }

            string password = args[0];
            string inputFile = args[1];
            string outputFile = args[2];

            byte[] salt = Enumerable.Range(0, 16).Select(i => (byte)i).ToArray();

            // Generamos la clave AES de 256 bits
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, salt, 1000);
            byte[] key = pdb.GetBytes(32);
            Console.WriteLine("Clave AES:");
            (new Ayuda()).WriteHex(key, key.Length);

            // Generamos el vector de inicialización
            byte[] iv = pdb.GetBytes(16);
            Console.WriteLine("\nVector de Inicialización:");
            (new Ayuda()).WriteHex(iv, iv.Length);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Padding = PaddingMode.ANSIX923;
                aes.Mode = CipherMode.CBC;

                using (FileStream fsInput = new FileStream(inputFile, FileMode.Open))
                using (BinaryReader reader = new BinaryReader(fsInput, Encoding.UTF8))
                using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create))
                using (CryptoStream cryptoStream = new CryptoStream(fsOutput, aes.CreateEncryptor(), CryptoStreamMode.Write))
                using (BinaryWriter writer = new BinaryWriter(cryptoStream, Encoding.UTF8))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        writer.Write(buffer, 0, bytesRead);
                    }
                }
            }

            Console.WriteLine("Cifrado completado con éxito.");
        }
    }
}
