using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Apoyo;

namespace AES_Junio2019
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Ayuda a = new Ayuda();

            if (args.Length != 2)
            {
                Console.WriteLine("El programa debe tener dos argumentos");
            }

            Console.WriteLine("Primer argumento: Nombre del fichero plano: " + args[0]);
            Console.WriteLine("Segundo argumento: Nombre del fichero cifrado: " + args[1]);

            String nombreficheroplano = args[0];
            String nombreficherocifrado = args[1];

            // Longitud de la clave: 256 bits => TamClave = 256 / 8 = 32 bytes
            int TamClave = 32;

            // Vector clave
            byte[] clave = new byte[TamClave];
            for (int i = 0; i < clave.Length; i++)
            {
                clave[i] = (byte)(i % 256);

            }

            Console.WriteLine("Vector clave:");
            a.WriteHex(clave, clave.Length);

            // Vector de inicializacion
            byte[] VI = new byte[16];
            for (int i = 0;i < VI.Length; i++)
            {
                VI[i] = (byte)((i+160) % 256);
            }

            Console.WriteLine("VI:");
            a.WriteHex(VI, VI.Length);

            // Creamos el objeto proveedor AES y le asignamos las propiedades
            AesManaged aes = new AesManaged();
            aes.KeySize = TamClave * 8;
            aes.Key = clave;
            aes.IV = VI;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.ISO10126;
            

            // Lectura del fichero plano
            FileStream fsOpen = new FileStream(nombreficheroplano, FileMode.Open, FileAccess.Read);

            BinaryReader binaryreader = new BinaryReader(fsOpen);



            // Proceso para cifrar
            FileStream fscifrado = new FileStream(nombreficherocifrado, FileMode.Create, FileAccess.Write);

            ICryptoTransform encryptor = aes.CreateEncryptor();

            CryptoStream streamCifrado = new CryptoStream(fscifrado, encryptor, CryptoStreamMode.Write);

            BinaryWriter binarywriter = new BinaryWriter(streamCifrado);


            // Cifrado
            long leidos = 0;
            long bytes = a.BytesFichero(nombreficheroplano);
            byte[] buffer = new byte[bytes];

            while (leidos < bytes)
            {
                buffer[leidos] = binaryreader.ReadByte();
                binarywriter.Write(buffer[leidos]);
                leidos++;
            }

            Console.WriteLine("Cifrado hecho!!");

            binarywriter.Flush();
            fsOpen.Dispose();
            binaryreader.Dispose();
            streamCifrado.Dispose();
            binarywriter.Close();
            fscifrado.Close();





        }
    }
}
