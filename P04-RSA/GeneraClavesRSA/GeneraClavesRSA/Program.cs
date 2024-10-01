using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Apoyo;

namespace GeneraClavesRSA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Ayuda ayuda = new Ayuda();
            //RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);

            // Mostrar por consola las propiedades basicas del proveedor
            KeySizes legalKeySizes = rsa.LegalKeySizes[0];
            Console.WriteLine("Tamaños legales de las claves: [{0}, {1}]", legalKeySizes.MinSize, legalKeySizes.MaxSize);
            Console.WriteLine("Tamaño de la clave actual: {0}", rsa.KeySize);
            Console.WriteLine("¿La clave deve conservarse en el proveedor actual? {0}",rsa.PersistKeyInCsp);
            Console.WriteLine("Algoritmo de intercambio de claves: {0}", rsa.KeyExchangeAlgorithm);
            Console.WriteLine("¿El objeto contiene solo una clave publica? {0}", rsa.PublicOnly);
            Console.WriteLine();

            String rsaXml = rsa.ToXmlString(true);
            String rsaString = rsa.ToString();

            //Console.WriteLine(rsaXml);

            RSAParameters rsaParameters = rsa.ExportParameters(true);

            byte[] PrivateExponent = rsaParameters.D;
            byte[] PublicExponent = rsaParameters.Exponent;
            byte[] Module = rsaParameters.Modulus;
            byte[] PrimeP = rsaParameters.P;
            byte[] PrimeQ = rsaParameters.Q;

            Console.WriteLine("Exponente Privado:");
            ayuda.WriteHex(PrivateExponent, PrivateExponent.Length);
            Console.WriteLine("Exponente Público:");
            ayuda.WriteHex(PublicExponent, PublicExponent.Length);
            Console.WriteLine("Modulo:");
            ayuda.WriteHex(Module, Module.Length);
            Console.WriteLine("Número Primo P:");
            ayuda.WriteHex(PrimeP, PrimeP.Length);
            Console.WriteLine("Número Primo Q:");
            ayuda.WriteHex(PrimeQ, PrimeQ.Length);


            // TODO: Guardar en fichero texto plano

            // Exportar a blob

            // Con clave privada
            byte[] cspBlobPriv = rsa.ExportCspBlob(true);
            String nombreFichBlobPriv = "zz_BlobRSA_Priva.bin";
            ayuda.GuardaBufer(nombreFichBlobPriv, cspBlobPriv);

            // Con clave pública
            byte[] cspBlobPubli = rsa.ExportCspBlob(false);
            String nombreFichBlobPubli = "zz_BlobRSA_Publi.bin";
            ayuda.GuardaBufer(nombreFichBlobPubli, cspBlobPubli);

            RSACryptoServiceProvider rsaImport = new RSACryptoServiceProvider();
            rsaImport.ImportCspBlob(cspBlobPriv);



        }
    }
}
