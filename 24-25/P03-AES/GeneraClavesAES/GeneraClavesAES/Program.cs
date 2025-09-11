using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Apoyo;

namespace GeneraClavesAES
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Introduce la clave: ");
            String Contra = Console.ReadLine();

            byte[] Sal = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 };

            Rfc2898DeriveBytes DeriveBytes = new Rfc2898DeriveBytes(Contra, Sal, 1000);

            byte AESByteNum = 32; // 16 bytes para 128 bits, 24 bytes para 192 bits y 32 bytes para 256 bits
            byte[] ClaveAES = DeriveBytes.GetBytes(AESByteNum);
            byte[] VIAES = DeriveBytes.GetBytes(16);
            
            Ayuda ayuda = new Ayuda();

            Console.WriteLine("Clave AES: ");
            ayuda.WriteHex(ClaveAES, AESByteNum);
            Console.WriteLine("Vector de Inicialización AES: ");
            ayuda.WriteHex(VIAES, 16);
            Console.WriteLine();

            Console.WriteLine("Reseteamos el objeto DeriveBytes");
            DeriveBytes.Reset();

            byte[] NuevaClaveAES = DeriveBytes.GetBytes(AESByteNum);
            byte[] NuevaVIAES = DeriveBytes.GetBytes(16);

            Console.WriteLine("Nueva Clave AES: ");
            ayuda.WriteHex(NuevaClaveAES, AESByteNum);
            Console.WriteLine("Nuevo Vector de Inicialización AES: ");
            ayuda.WriteHex(NuevaVIAES, 16);
            Console.WriteLine();

            Console.WriteLine("Pero si lo hacemos si reseterar:");
            byte[] ClaveAES2 = DeriveBytes.GetBytes(AESByteNum);
            byte[] VIAES2 = DeriveBytes.GetBytes(16);
            Console.WriteLine("Clave AES: ");
            ayuda.WriteHex(ClaveAES2, AESByteNum);
            Console.WriteLine("Vector de Inicialización AES: ");
            ayuda.WriteHex(VIAES2, 16);


        }
    }
}
