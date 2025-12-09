using Apoyo;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
namespace CIFRA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Ayuda ayuda = new Ayuda();
            if (args.Length != 3)
            {
                Console.WriteLine("Uso: CIFRA Contraseña Nombre de Fentrada Fsalida ");
                Environment.Exit(1);
            }
            string password = args[0];
            string ficheroEntrada = args[1];
            string ficheroSalida = args[2];
            byte[] Sal = new byte[16]; //Sal de 16 bytes
            for (int i = 0; i < Sal.Length; i++)
            {
                Sal[i] = (byte)i;
            }
            ayuda.WriteHex(Sal, Sal.Length);
            Rfc2898DeriveBytes generador = new Rfc2898DeriveBytes(password, Sal, 1000); //Genera la contraseña
            byte[] clave = generador.GetBytes(32); //Clave de 32 bytes
            byte[] IV = generador.GetBytes(16); //Vector de inicialización de 16 bytes
            Console.WriteLine("Clave:");
            ayuda.WriteHex(clave, clave.Length);
            Console.WriteLine("Vector IV:");
            ayuda.WriteHex(IV, IV.Length);

            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.KeySize = 256;
            aes.Key = clave;
            aes.IV = IV;
            aes.Padding = PaddingMode.ANSIX923;
            aes.Mode = CipherMode.CBC;

            FileStream fsEntrada = new FileStream(ficheroEntrada, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fsEntrada, Encoding.UTF8);

            FileStream fsSalida = new FileStream(ficheroSalida, FileMode.Create, FileAccess.Write);
            CryptoStream cs = new CryptoStream(fsSalida, aes.CreateEncryptor(), CryptoStreamMode.Write);
            BinaryWriter bw = new BinaryWriter(cs, Encoding.UTF8);

            byte[] buffer = new byte[ayuda.BytesFichero(ficheroEntrada)];
            br.Read(buffer, 0, buffer.Length);//Guardamos el fichero en el buffer

            bw.Write(buffer);//Escribimos el buffer cifrado en el fichero de salida

            br.Close();
            fsEntrada.Close();

            
            bw.Close();
            cs.Dispose();
            fsSalida.Close();

            //Descifrar el fichero (Opcional)
            fsEntrada = new FileStream(ficheroSalida, FileMode.Open, FileAccess.Read);
            CryptoStream cs2 = new CryptoStream(fsEntrada, aes.CreateDecryptor(), CryptoStreamMode.Read);
            BinaryReader br2 = new BinaryReader(cs2, Encoding.UTF8);

            byte[] desencriptado = new byte[ayuda.BytesFichero(ficheroSalida)];
            br2.Read(desencriptado, 0, desencriptado.Length);

            br2.Close();
            cs2.Dispose();
            fsEntrada.Close();

            fsSalida = new FileStream("DESCIFRADO.txt", FileMode.Create, FileAccess.Write);
            BinaryWriter bw2 = new BinaryWriter(fsSalida, Encoding.UTF8);
            bw2.Write(desencriptado);

            bw2.Close();
            fsSalida.Close();

        }   
    }
}
