using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using Apoyo;
namespace practica4RSA_GENERACION
{
    class Program
    {
        static void Main(string[] args)
        {
            //Creamos un objeto ayuda
            Ayuda help = new Ayuda();
            //Creamos un CspParameters
            CspParameters provider = new CspParameters();
            //Creamos un objeto un RSACryptoServiceProvider
            RSACryptoServiceProvider rsa_provider = new RSACryptoServiceProvider();
            //Mostramos por pantalla las propiedades basicas del proveedor
            Console.WriteLine("Tamaños legales de la claves: " + rsa_provider.LegalKeySizes); //Esto es privado -> legal
            Console.WriteLine("Tamaño de la clave actual: " + rsa_provider.KeySize);
            Console.WriteLine("Debe conservarse en el proveedor actual: " + rsa_provider.PersistKeyInCsp);
            Console.WriteLine("Nombre del algoritmo de intercambio: " + rsa_provider.KeyExchangeAlgorithm);
            Console.WriteLine("Si contiene una unica clave publica: " + rsa_provider.PublicOnly);
            Console.WriteLine("Algoritmo de firma disponible: " + rsa_provider.SignatureAlgorithm);
            Console.WriteLine("Informacion adicional sobre un par de claves cripto: " + rsa_provider.CspKeyContainerInfo);

            //Exportacion de las claves
            string rsa_xml = rsa_provider.ToXmlString(true);
            RSAParameters rsa_param =  rsa_provider.ExportParameters(true);
            //Console.WriteLine("Elementos que constituyen claves RSA: " + rsa_param.Exponent.Length);
            help.WriteHex(rsa_param.D, rsa_param.D.Length);
            help.WriteHex(rsa_param.DP, rsa_param.DP.Length);
            help.WriteHex(rsa_param.DQ, rsa_param.DQ.Length);
            help.WriteHex(rsa_param.InverseQ, rsa_param.InverseQ.Length);
            help.WriteHex(rsa_param.Modulus, rsa_param.Modulus.Length);
            help.WriteHex(rsa_param.P, rsa_param.P.Length);
            help.WriteHex(rsa_param.Q, rsa_param.Q.Length);
            
            
            //Creamos el objeto Blob
            Byte[] rsa_blob = rsa_provider.ExportCspBlob(true);
            //Exportamos la clave privada y publica
            help.GuardaBufer("zz_BlobRSA_Priva.bin", rsa_blob);
            Console.WriteLine("Ambas claves: ");
            help.WriteHex(rsa_blob, rsa_blob.Length);
            Console.WriteLine();
            //Exportamos la clave publica sin la privada
            Byte[] rsa_blob2 = rsa_provider.ExportCspBlob(false);
            help.GuardaBufer("zz_BlobRSA_Publi.bin", rsa_blob2);
            Console.WriteLine("Solo clave publica: ");
            help.WriteHex(rsa_blob2, rsa_blob2.Length);
            rsa_provider.Dispose();

            //Comprobacion de que se importa sin problemas
            RSACryptoServiceProvider rsa_read = new RSACryptoServiceProvider();
            rsa_read.ImportCspBlob(rsa_blob);
            help.WriteHex(rsa_blob, rsa_blob.Length);
        }
    }
}
