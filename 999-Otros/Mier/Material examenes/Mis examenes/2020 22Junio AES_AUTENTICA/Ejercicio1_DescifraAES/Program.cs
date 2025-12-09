using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Apoyo;


namespace Ejercicio1_DescifraAES
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Ayuda a = new Ayuda();

            // Dos parametros
            if(args.Length != 2)
            {
                Console.WriteLine("El programa requiere dos parametros");
                return;
            }

            string fich_entrada_cifrado = args[0];
            string fich_salida_descifrado = args[1];

            // Creamos el objeto proveedor AES
            AesManaged aes = new AesManaged();

            // Generamos el vector clave
            int TamClave = 32;
            byte[] clave = new byte[TamClave];

            for (int i = 0; i < clave.Length; i++)
            {
                clave[i] = (byte)(i % 256);
            }

            // Generamos el vector de inicialización
            byte[] VI = new byte[16];

            for (int i = 0; i < VI.Length; i++)
            {
                VI[i] = (byte)((i + 160) % 256);
            }


            // Asignar propiedades al objeto proveedor AES
            aes.Key = clave;
            aes.IV = VI;
            aes.Padding = PaddingMode.ISO10126;
            aes.Mode = CipherMode.CBC;


            // Creamos el filestream de lectura
            FileStream fs_lectura = new FileStream(fich_entrada_cifrado, FileMode.Open, FileAccess.Read);

            // Creamos el binaryreader
            BinaryReader binaryReader = new BinaryReader(fs_lectura);

            // buffer intermedio para guardar la información
            byte[] buffer = new byte[a.BytesFichero(fich_entrada_cifrado)];


            // Filestream para escritura
            FileStream fs_escritura = new FileStream(fich_salida_descifrado, FileMode.Create, FileAccess.Write);

            ICryptoTransform cryptoTransform = aes.CreateDecryptor();
            CryptoStream cs = new CryptoStream(fs_escritura, cryptoTransform, CryptoStreamMode.Write);

            BinaryWriter binaryWriter = new BinaryWriter(cs);

            int leidos = 0;
            long size = a.BytesFichero(fich_entrada_cifrado);

            while ( leidos < size )
            {
                buffer[leidos] = binaryReader.ReadByte();
                binaryWriter.Write(buffer[leidos]);
                leidos++;
            }

            // Liberamos todos los recursos
            binaryReader.Close();
            binaryReader.Dispose();
            cs.Flush();
            binaryWriter.Close();
            binaryWriter.Dispose();
            fs_escritura.Close();

        }
    }
}
