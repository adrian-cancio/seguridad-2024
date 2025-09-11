using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
namespace practica3AES_cadenas
{
    class Program
    {
        static void Main(string[] args)
        {
            //Generamos las claves y el IV del objeto AesManaged creado
            AesManaged provider = new AesManaged();
            provider.GenerateIV();
            provider.GenerateKey();
            
            string NombreFichero = "zz_cadenaCifrada.bin";
            FileStream fs = new FileStream(NombreFichero, FileMode.Create,
                FileAccess.Write, FileShare.None);
            //Creamos el objeto encriptador
            ICryptoTransform i_cifrador = provider.CreateEncryptor();
            //Creamos un cryptostream
            CryptoStream cs_cifrador = new CryptoStream(fs, i_cifrador,
                CryptoStreamMode.Write);
            //Creamos el streamwriter
            StreamWriter sw = new StreamWriter(cs_cifrador);
            sw.Write("Hola que tal a todos, probando para Seguridad");
            sw.Close();
            cs_cifrador.Close();
            i_cifrador.Dispose();
            fs.Close();

            //Ronda para poder leer del fichero
            FileStream fs_lectura = new FileStream(NombreFichero,
                FileMode.Open, FileAccess.Read, FileShare.None);
            //Creamos el objeto descriptor
            ICryptoTransform i_descriptor = provider.CreateDecryptor();
            //Cryptostream de descifrado
            CryptoStream cs_descriptor = new CryptoStream(fs_lectura,
                i_descriptor, CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cs_descriptor);
            Console.WriteLine(sr.ReadToEnd());
            

        }
    }
}
