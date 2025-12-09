using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Apoyo;

namespace AES_Ejercicio2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Ayuda a = new Ayuda();

            Console.WriteLine("Contraseña: ");
            string contraseña = Console.ReadLine();

            // extraer los bytes correspondientes a una codificacion unicode
            byte[] bytecontra = Encoding.Unicode.GetBytes(contraseña);

            byte[] clave = new byte[32];
            byte[] VI = new byte[16];


            SHA384Managed sha = new SHA384Managed();

            // resumen de la contraseña
            byte[] resumen = sha.ComputeHash(bytecontra);

            // copio el resultado del resumen de la contraseña a los vectores clave y VI
            for (int i = 0; i < clave.Length; i++)
            {
                clave[i] = resumen[i];
            }
            Console.WriteLine("Clave: ");
            a.WriteHex(clave, clave.Length);


            for (int i = 0; i < VI.Length; i++)
            {
                VI[i] = resumen[32 + i];
            }
            Console.WriteLine("VI: ");
            a.WriteHex(VI, VI.Length);


            // se crean la variable textoplano y se rellena
            byte[] textoplano = new byte[108];
            for (int i = 0; i < textoplano.Length; i++)
            {
                textoplano[i] = (byte)(i % 256);
            }
            Console.WriteLine("textoplano: ");
            a.WriteHex(textoplano, textoplano.Length);

            
            // Se crea el objeto proveedor aes
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.Key = clave;
            aes.IV = VI;    
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.ANSIX923;



            // --------- PROCESO DE CIFRADO ---------
            FileStream fileWrite = new FileStream("zz_TextoCifrado.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            
            //Para que el objeto aes pueda cifrar la info, crear un objeto cifrador
            ICryptoTransform cifrador = aes.CreateEncryptor();

            //Objeto de la clase CryptoStream para cifrar
            //3 param en constructor: el filestream de escritura, cifrador, modo --> read o write
            CryptoStream csCifrar = new CryptoStream(fileWrite, cifrador, CryptoStreamMode.Write);

            //Cifrar la información con el método Write()
            csCifrar.Write(textoplano, 0, textoplano.Length);


            //Para que los bytes se vuelquen al Stream de salida --> método Flush
            //y cerrar CryptoStream llamando al método Close()

            csCifrar.Flush();
            csCifrar.Close();

            //Cerar FileStream
            fileWrite.Close();



            // --------- PROCESO DE DESCIFRADO ---------
            byte[] TextoDescifrado = new byte[textoplano.Length];

            //Flujo de datos FileStream para leer el fichero
            FileStream fileRead = new FileStream("zz_TextoCifrado.bin", FileMode.Open, FileAccess.Read, FileShare.None);

            //Objeto descifrador
            ICryptoTransform descifrador = aes.CreateDecryptor();

            // Objeto para descifrar
            CryptoStream csDescifrar = new CryptoStream(fileRead, descifrador, CryptoStreamMode.Read);

            //Cifrar la informacion y volcar a la salida estandar
            csDescifrar.Read(TextoDescifrado, 0, TextoDescifrado.Length);
            csDescifrar.Flush();

            // Resultado de encriptar el texto plano y desencriptarlo
            Console.WriteLine("[Texto plano desencriptado]:");
            a.WriteHex(TextoDescifrado, TextoDescifrado.Length);

        }
    }
}
