using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using Apoyo;
namespace practica4RSA_CIFRAR
{
    class Program
    {
        static void Main(string[] args)
        {
            Ayuda help = new Ayuda();
            RSACryptoServiceProvider rsa_provider = new RSACryptoServiceProvider();
            string FPubli = "zz_BlobRSA_Publi.bin";

            //Mostrar la longitud de la clave
            Console.WriteLine("Longitud de la clave que utiliza: " + rsa_provider.KeySize);
            //Especificamos la longitud del Publi
            long long_blobPub = help.BytesFichero(FPubli);
            //Mostramos la longitud por consola para saber que ha funcionado
            Console.WriteLine("Longitud del fichero zz_BlobRS_Publi.bin: " + long_blobPub);
            //Declaramos el array que almacenara el blob
            Byte[] rsa_blob = new byte[long_blobPub];
            //Cargamos el array con el contenido del fichero
            help.CargaBufer(FPubli, rsa_blob);
            rsa_provider.ImportCspBlob(rsa_blob);

            RSAParameters rsa_param = rsa_provider.ExportParameters(false);
            Console.WriteLine("Exponente publico: "); help.WriteHex(rsa_param.Exponent, rsa_param.Exponent.Length);
            Console.WriteLine("Modulo: " ); help.WriteHex(rsa_param.Modulus, rsa_param.Modulus.Length);
            //Creamos el vector del texto plano a cifrar
            byte[] texto_plano = new byte[64];
            //Lo mostramos por pantalla para asegurarnos que esta bien
            Console.WriteLine("Comprobacion de que el vector esta bien rellenado: ");
            for (int i = 0; i < texto_plano.Length; i++)
            {
                texto_plano[i] = (byte) i;
                Console.WriteLine(texto_plano[i]);

            }

            //Ciframos el texto plano
            Byte[] texto_cifrado = rsa_provider.Encrypt(texto_plano, true);
            //Lo guardamos en un fichero
            help.GuardaBufer("zz_TextoCifrado.bin", texto_cifrado);
            for (int i = 0; i < texto_cifrado.Length; i++)
            {
                Console.WriteLine(texto_cifrado[i]);

            }
            //Liberamos los recursos
            rsa_provider.Dispose();

        }
    }
}
