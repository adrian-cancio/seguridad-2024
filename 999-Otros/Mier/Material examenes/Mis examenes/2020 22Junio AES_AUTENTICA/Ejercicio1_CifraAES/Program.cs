using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Apoyo;


namespace Ejercicio1_CifraAES
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Ayuda a = new Ayuda();

            if(args.Length != 2 )
            {
                Console.WriteLine("Uso: CifraAES <archivo_entrada> <archivo_salida>");
                return;
            }
            // El programa recibe dos argumentos
            string fich_entrada = args[0];
            string fich_salida = args[1];

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

            for (int i = 0;i < VI.Length; i++)
            {
                VI[i] = (byte)((i+160) % 256);
            }


            // Asignar propiedades al objeto proveedor AES
            aes.Key = clave;
            aes.IV = VI;
            aes.Padding = PaddingMode.ISO10126;
            aes.Mode = CipherMode.CBC;


            // Preparamos la cadena de streams para cifrar el fichero de entrada
            FileStream fs = new FileStream(fich_entrada, FileMode.Open, FileAccess.Read);

            BinaryReader binaryReader = new BinaryReader(fs);

            //Definimos un buffer intermedio que recogerá la lectura del archivo original y será recogido por el BinaryWriter
            byte[] buf = new byte[a.BytesFichero(fich_entrada)];

            a.WriteHex(buf, buf.Length);


            // Preparamos la cadena de streams para realizar la escritura
            FileStream fs_escritura = new FileStream(fich_salida, FileMode.Create, FileAccess.Write);
            ICryptoTransform cryptoTransform = aes.CreateEncryptor();
            CryptoStream cs = new CryptoStream(fs_escritura, cryptoTransform, CryptoStreamMode.Write);

            BinaryWriter binaryWriter = new BinaryWriter(cs);

            int leidos = 0;
            long size = a.BytesFichero(fich_entrada);

            while(leidos < size)
            {
                buf[leidos] = binaryReader.ReadByte();
                binaryWriter.Write(buf[leidos]);
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
