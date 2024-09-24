using Apoyo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CifradoAES
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const int TamClave = 32;
            byte[] Clave = new byte[TamClave];
            for (int i = 0; i < Clave.Length; i++) Clave[i] = (byte)(i % 256);

            byte[] VI = new byte[16];
            for (int i = 0; i < VI.Length; i++) VI[i] = (byte)((i + 160) % 256);

            byte[] TextoPlano =
            { 
                0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F,
                0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F,
                0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F
            };

            AesManaged Provider = new AesManaged();

            Console.WriteLine("Provider default properties:");
            Console.WriteLine("BlockSize: " + Provider.BlockSize);
            Console.WriteLine("KeySize: " + Provider.KeySize);
            Console.WriteLine("Padding: " + Provider.Padding);
            Console.WriteLine("Mode: " + Provider.Mode);
            Console.WriteLine();

            Provider.Key = Clave;
            Provider.Padding = PaddingMode.PKCS7;
            Provider.Mode = CipherMode.ECB;

            Console.WriteLine("Provider modified properties:");
            Console.WriteLine("BlockSize: " + Provider.BlockSize);
            Console.WriteLine("KeySize: " + Provider.KeySize);
            Console.WriteLine("Padding: " + Provider.Padding);
            Console.WriteLine("Mode: " + Provider.Mode);
            Console.WriteLine();

            Provider.GenerateKey();
            
            Ayuda ayuda = new Ayuda();
            Console.WriteLine("Clave de cifrado:");
            ayuda.WriteHex(Provider.Key, TamClave);

            Provider.GenerateIV();
            Console.WriteLine("Vector de inicialización:");
            ayuda.WriteHex(Provider.IV, 16);

            const String NombreFichero = "zz_TextoCifrado.bin";
            FileStream fs = new FileStream(NombreFichero, 
                FileMode.Create, FileAccess.Write, FileShare.None);

            ICryptoTransform encryptor = Provider.CreateEncryptor();
            
            CryptoStream cs = new CryptoStream(fs, encryptor, CryptoStreamMode.Write);

            cs.Write(TextoPlano, 0, TextoPlano.Length);
            
            cs.Flush();
            cs.Close();
            fs.Close();
            cs.Dispose();

            // Mostrar texto cifrado
            long TamFich = ayuda.BytesFichero(NombreFichero);
            byte[] TextoCifrado = new byte[TamFich];
            for (int i = 0; i < TamFich; i++)
            {
                TextoCifrado[i] = 0xFF;
            }
            ayuda.CargaBufer(NombreFichero, TextoCifrado);

            Console.WriteLine("\nTexto cifrado:");
            ayuda.WriteHex(TextoCifrado, TextoCifrado.Length);


            byte[] TextoDescifrado = new byte[TamFich];
            fs = new FileStream(NombreFichero,
                FileMode.Open, FileAccess.Read, FileShare.None);

            ICryptoTransform decryptor = Provider.CreateDecryptor();

            cs = new CryptoStream(fs, decryptor, CryptoStreamMode.Read);

            cs.Read(TextoDescifrado, 0, TextoDescifrado.Length);
            cs.Flush();
            cs.Close();
            fs.Close();

            Console.WriteLine("\nTexto descifrado: ");
            ayuda.WriteHex(TextoDescifrado, TextoDescifrado.Length);

        }
    }
}
