using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace practica3AES_ficheros_desencriptar
{
    class Program
    {
        static void Main(string[] args)
        {
            AesManaged provider = new AesManaged();
            int TamClave = 16;
            byte[] Clave = new byte[TamClave];
            for (int i = 0; i < Clave.Length; i++)
            {
                Clave[i] = (byte)(i % 256);
            }

            //Fichero de entrada plano
            FileStream fs_entrada = new FileStream("hopla.bin", FileMode.Open, FileAccess.Read, FileShare.None);
            provider.GenerateIV();
            provider.Key = Clave;

            //Fichero de encriptacion
            FileStream fs_salida = new FileStream("hopla_2.txt", FileMode.Create, FileAccess.Write, FileShare.None);
            ICryptoTransform descriptor = provider.CreateDecryptor();
            CryptoStream cs = new CryptoStream(fs_entrada, descriptor, CryptoStreamMode.Read);

            byte[] arrayBufferMem = new byte[fs_entrada.Length];
            int bytesSinLeer = (int)fs_entrada.Length;
            int bytesLeidos = 0;

            while (bytesLeidos < bytesSinLeer)
            {
                int n = fs_entrada.Read(arrayBufferMem, bytesLeidos, bytesSinLeer);
                cs.Read(arrayBufferMem, bytesLeidos, bytesSinLeer);
                bytesLeidos += n;
                bytesSinLeer -= n;
            }
            cs.Close();
            fs_entrada.Close();
            fs_salida.Close();
            descriptor.Dispose();
            cs.Dispose();

        }
    }
}
