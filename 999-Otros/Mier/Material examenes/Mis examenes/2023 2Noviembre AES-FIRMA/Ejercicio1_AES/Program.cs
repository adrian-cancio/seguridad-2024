using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Apoyo;


namespace Ejercicio1_AES
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Ayuda a = new Ayuda();

            if(args.Length != 3)
            {
                Console.WriteLine("El programa requiere 3 argumentos: ");
                Environment.Exit(1);
            }

            string contraseña = args[0];
            string fich_entrada = args[1];
            string fich_salida = args[2];

            
            byte[] salt = new byte[16];
            for(int i = 0; i < 16; i++)
            {
                salt[i] = (byte)i;
            }


            // Generar la clave AES de 256 bits (32 bytes) usando Rfc2898DeriveBytes
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(contraseña, salt, 1000);
            byte[] clave = rfc.GetBytes(32); // Generamos la clave de manera aleatoria

            Console.WriteLine("Clave generada por Rfc2898DeriveBytes: ");
            a.WriteHex(clave, clave.Length);

            // Generar el vector de inicializacion
            byte[] VI = rfc.GetBytes(16);

            Console.WriteLine("VI generado por Rfc2898DeriveBytes: ");
            a.WriteHex(VI, VI.Length);


            // Crear el proveedor AES y asignamos las propiedades
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.KeySize = 256;
            aes.Key = clave;
            aes.IV = VI;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.ANSIX923;

            // preparamos la cadena de streams para leer el fichero de entrada. Un filestream y un binary reader
            FileStream fs_entrada = new FileStream(fich_entrada, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fs_entrada, Encoding.UTF8);

            // preparamos la cadena de streams para escribir el fichero de salida. FileStream, CryptoStream y binaryWriter
            FileStream fs_salida = new FileStream(fich_salida, FileMode.Create, FileAccess.Write);
            ICryptoTransform cifrador = aes.CreateEncryptor();
            CryptoStream cryptostream = new CryptoStream(fs_salida, cifrador, CryptoStreamMode.Write);
            BinaryWriter binaryWriter = new BinaryWriter(cryptostream);


            // Creamos buffer de memoria intermedio
            int leidos = 0;
            long size = a.BytesFichero(fich_entrada);

            byte[] buffer = new byte[size];

            // Recorremos mediante un bucle while
            while(leidos < size)
            {
                buffer[leidos] = binaryReader.ReadByte();
                binaryWriter.Write(buffer[leidos]);
                leidos++;
            }

        }
    }
}
