using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Apoyo;

namespace AES_Diciembre2019
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Ayuda a = new Ayuda();

            // Asignamos el tamaño a la clave
            int TamClave = 32;

            byte[] clave = new byte[TamClave];
            byte[] VI = new byte[16]; // Vector de inicializacion

            // rellenamos los vectores
            for (int i = 0; i < clave.Length; i++)
            {
                clave[i] = (byte)(i % 256);
            }

            Console.WriteLine("vector clave: \n");
            a.WriteHex(clave, clave.Length);

            for (int i = 0; i < VI.Length; i++)
            {
                VI[i] = (byte)((i+160) % 256);
            }

            Console.WriteLine("vector inicialización: \n");
            a.WriteHex(VI, VI.Length);


            // Creamos el objeto aes
            AesManaged aes = new AesManaged();

            // Le asignamos las propiedades correspondientes
            aes.KeySize = TamClave * 8;
            aes.Key = clave;
            aes.IV = VI;
            aes.Padding = PaddingMode.ISO10126;
            aes.Mode = CipherMode.CBC;

            // Declaramos las dos variables de entrada del programa
            string textoplano = args[0];
            string textocifrado = args[1];


            // ----------- LECTURA DEL FICHERO EN PLANO -----------

            // Stream de lectura
            FileStream fs_lectura = new FileStream(textoplano, FileMode.Open, FileAccess.Read, FileShare.None);

            // Declaramos el binary reader para leer
            BinaryReader binaryReader = new BinaryReader(fs_lectura);



            // ----------- ESCRITURA DEL FICHERO CIFRADO -----------
            FileStream fs_escritura = new FileStream(textocifrado, FileMode.Create, FileAccess.Write, FileShare.None);

            //Declaramos un objeto TCryptoTransform que necesitaremos para crear el Cryptostream
            ICryptoTransform encryptor = aes.CreateEncryptor();

            //Declaramos el objeto cryptostream, que toma como args el stream de lectura, el Icryptotransform
            //Y el modo escritura
            CryptoStream cs = new CryptoStream(fs_escritura, encryptor, CryptoStreamMode.Write);

            BinaryWriter bw = new BinaryWriter(cs);



            // CREACION DE LA PARTE DE ESCRITURA AL FICHERO CIFRADO

            // Se crean las variables para escribir
            long leidos = 0;

            // Cogemos el numero total de bytes del fichero plano que recibimos
            long bytes = a.BytesFichero(textoplano);

            // Se declara un buffer intermedio
            byte[] buffer = new byte[bytes];

            // Mientras que no hayamos acabado de recorrer todos los bytes del fichero plano
            while (leidos < bytes)
            {
                // Metemos el elemento leido en el buffer
                buffer[leidos] = binaryReader.ReadByte();
                // Y lo escribimos en el archivo de cifrado
                bw.Write(buffer[leidos]);
                //Aumentamos el contador
                leidos++;
            }

            Console.WriteLine("Fin del cifrado !"); // Mostrar por salida estándar para saber que acabo

            //Cerramos todos los streams que hemos abierto anteriormente
            bw.Flush(); 
            fs_lectura.Dispose();
            binaryReader.Dispose();
            cs.Dispose();
            bw.Dispose();
            bw.Close();
            fs_escritura.Close();





        }
    }
}
