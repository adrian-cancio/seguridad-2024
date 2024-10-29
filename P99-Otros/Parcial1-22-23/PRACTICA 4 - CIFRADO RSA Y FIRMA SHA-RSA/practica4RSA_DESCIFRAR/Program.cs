using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using Apoyo;

namespace practica4RSA_DESCIFRAR
{
    class Program
    {
        static void Main(string[] args)
        {
            Ayuda help = new Ayuda();
            RSACryptoServiceProvider rsa_provider = new RSACryptoServiceProvider();
            //Mostramos alguna propiedad
            Console.WriteLine("Longitud de la clave empleada: " + rsa_provider.KeySize);

            //Obtener el tamaño de Priva.bin
            string nombreFichero = "zz_BlobRSA_Priva.bin";
            int tam = (int) help.BytesFichero(nombreFichero);
            Console.WriteLine("Tamaño del fichero BlobPriva: " + tam);

            //Creamos un array blob
            Byte[] rsa_blob = new byte[tam];

            //Cargamos el fichero
            help.CargaBufer(nombreFichero, rsa_blob);
            rsa_provider.ImportCspBlob(rsa_blob);
            //Algunos parametros para comprobar que es correcto
            RSAParameters rsa_param = rsa_provider.ExportParameters(true);
            Console.WriteLine("Exponente publico: "); help.WriteHex(rsa_param.Exponent, rsa_param.Exponent.Length);
            Console.WriteLine("InverseQ: "); help.WriteHex(rsa_param.InverseQ, rsa_param.InverseQ.Length);
            Console.WriteLine("Modulo: "); help.WriteHex(rsa_param.Modulus, rsa_param.Modulus.Length);

            Console.WriteLine();

            //Obtenemos el tamaño del fichero texto_cifrado
            Console.WriteLine("Texto cifrado: ");
            int tam_txtcifrado = (int) help.BytesFichero("zz_TextoCifrado.bin");
            Console.WriteLine("Tamaño de texto cifrado: " + tam_txtcifrado);

            byte[] txt_cifrado = new byte[tam_txtcifrado];
            help.CargaBufer("zz_TextoCifrado.bin", txt_cifrado);
            help.WriteHex(txt_cifrado, txt_cifrado.Length);
            
            Console.WriteLine();
            Console.WriteLine("Texto Descifrado: ");
            
            //Declaramos array de bytes para descifrar
            byte[] txt_descifrado = rsa_provider.Decrypt(txt_cifrado, true);
            help.WriteHex(txt_descifrado, txt_descifrado.Length);

            rsa_provider.Dispose();
        }
    }
}
